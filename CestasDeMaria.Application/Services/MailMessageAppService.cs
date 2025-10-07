using CestasDeMaria.Application.Helpers;
using CestasDeMaria.Domain.ModelClasses;
using CestasDeMaria.Infrastructure.CrossCutting.Enums;
using CestasDeMaria.Infrastructure.CrossCutting.Mail;
using Microsoft.Extensions.Options;
using IBlobStorageService = CestasDeMaria.Domain.Interfaces.Services.IBlobStorageService;
using ILoggerService = CestasDeMaria.Application.Interfaces.ILoggerAppService;
using IMainRepository = CestasDeMaria.Domain.Interfaces.Repository.IMailmessageRepository;
using IMainService = CestasDeMaria.Application.Interfaces.IMailMessageAppService;
using ISendGridService = CestasDeMaria.Domain.Interfaces.Services.ISendGridService;
using Main = CestasDeMaria.Domain.Entities.Mailmessage;
using MainDTO = CestasDeMaria.Application.DTO.MailmessageDTO;

namespace CestasDeMaria.Application.Services
{
    public class MailMessageAppService : ServiceBase<MainDTO>, IMainService
    {
        private readonly IMainRepository _mainRepository;
        private readonly ILoggerService _loggerService;
        private readonly ISendGridService _sendGridService;

        private string[] allowInclude = new string[] { };

        public MailMessageAppService(IBlobStorageService blobStorageService, IOptions<Settings> options, IMainRepository mainRepository, ILoggerService loggerService, ISendGridService sendGridService)
            : base(blobStorageService, options)
        {
            _mainRepository = mainRepository;
            _loggerService = loggerService;
            _sendGridService = sendGridService;
        }

        public async Task<IEnumerable<MainDTO>> GetAllAsync(string? include = null)
        {
            var list = await _mainRepository.GetAllAsync(IncludesMethods.GetIncludes(include, allowInclude));
            return list.ProjectedAsCollection<MainDTO>();
        }

        public async Task<IEnumerable<MainDTO>> GetAllAsync(string parentCode, string? include = null)
        {
            var list = await _mainRepository.GetAllAsync(parentCode, IncludesMethods.GetIncludes(include, allowInclude));
            return list.ProjectedAsCollection<MainDTO>();
        }

        public async Task<MainDTO> GetAsync(long code, string? include = null)
        {
            var result = await _mainRepository.GetAsync(code, IncludesMethods.GetIncludes(include, allowInclude));
            return result.ProjectedAs<MainDTO>();
        }

        public async Task<Tuple<int, int, IEnumerable<MainDTO>>> GetAllPagedAsync(int page, int quantity, string isActive = null, string term = null, string orderBy = null, string? include = null)
        {
            var tuple = await _mainRepository.GetAllPagedAsync(page, quantity, isActive, term, orderBy, IncludesMethods.GetIncludes(include, allowInclude));

            var total = tuple.Item1;
            var pages = (int)Math.Ceiling((double)total / quantity);

            var list = tuple.Item2.ProjectedAsCollection<MainDTO>();

            return Tuple.Create(total, pages, list);
        }

        public async Task<MainDTO> InsertAsync(MainDTO mainDto)
        {
            var main = mainDto.ProjectedAs<Main>();

            _mainRepository.Add(main);
            await _mainRepository.CommitAsync();

            return main.ProjectedAs<MainDTO>();
        }

        public async Task<MainDTO> UpdateAsync(MainDTO mainDto)
        {
            var main = mainDto.ProjectedAs<Main>();
            main.Updated = DateTime.UtcNow;

            _mainRepository.Update(main);
            await _mainRepository.CommitAsync();

            return main.ProjectedAs<MainDTO>();
        }

        public async Task<MainDTO> RemoveAsync(MainDTO mainDto)
        {
            var main = mainDto.ProjectedAs<Main>();

            _mainRepository.Remove(main);
            await _mainRepository.CommitAsync();

            return main.ProjectedAs<MainDTO>();
        }

        public async Task<string> GetReport(int quantityMax, string isActive = null, string term = null, string orderBy = null, string? include = null)
        {
            await _loggerService.InsertAsync($"Report - Starting GetReport - {this.GetType().Name}");

            var result = await GetAllPagedAsync(1, quantityMax == 0 ? int.MaxValue : quantityMax, isActive, term, orderBy, include);

            string link = await UploadReport(result.Item3.ToList());

            await _loggerService.InsertAsync($"Report - Finishing GetReport - {this.GetType().Name}");

            return link;
        }

        public async Task<MainDTO> SendMail(string to, string[] values, Enums.EmailType typeMail)
        {
            string subject = string.Empty;
            string body = Mail.PrepareTemplate(typeMail, values);

            switch (typeMail)
            {
                case Enums.EmailType.RecoveryPassword:
                    subject = "Recuperar de senha";
                    break;
                case Enums.EmailType.Wellcome:
                    subject = "Bem vindo - Cestas de Maria";
                    break;
            }

            return await SendMail(body, to, subject);
        }

        public async Task<MainDTO> SendMail(string mail, string to, string subject)
        {
            MainDTO mainDTO = new MainDTO()
            {
                Subject = subject,
                To = string.IsNullOrEmpty(_settings.ForceMailTo) ? to : _settings.ForceMailTo,
                Body = mail,
                IsActive = 1,
                IsDeleted = 0,
                Retries = 0,
                Adminid = 1
            };

            return await SendMail(mainDTO);
        }

        public async Task<MainDTO> ResendMail(long code)
        {
            var main = await _mainRepository.GetAsync(code);

            if (main == null)
            {
                return null;
            }

            return await ResendMail(main);
        }

        private async Task<MainDTO> SendMail(MainDTO mainDTO)
        {
            try
            {
                bool sent = await _sendGridService.SendMailV2(mainDTO.ProjectedAs<Main>());
                mainDTO.Mailmessagefamilystatus = sent ? Enums.StatusMail.Sent.ToString() : Enums.StatusMail.Error.ToString();
            }
            catch (Exception ex)
            {
                mainDTO.Mailmessagefamilystatus = Enums.StatusMail.Error.ToString();
                mainDTO.Message = ex.Message;
                await _loggerService.InsertAsync(ex);
            }
            finally
            {
                if (mainDTO.Id == 0)
                {
                    await InsertAsync(mainDTO);
                }
                else
                {
                    await UpdateAsync(mainDTO);
                }
            }

            return mainDTO.ProjectedAs<MainDTO>();
        }

        private async Task<MainDTO> ResendMail(Main main)
        {
            try
            {
                bool sent = await _sendGridService.SendMailV2(main);
                main.Mailmessagefamilystatus = sent ? Enums.StatusMail.Sent.ToString() : Enums.StatusMail.Error.ToString();
            }
            catch (Exception ex)
            {
                main.Mailmessagefamilystatus = Enums.StatusMail.Error.ToString();
                main.Message = ex.Message;
                await _loggerService.InsertAsync(ex);
            }
            finally
            {
                _mainRepository.Update(main);
            }

            return main.ProjectedAs<MainDTO>();
        }

        public void Dispose()
        {
            _mainRepository.Dispose();
        }
    }
}
