using IBlobStorageService = CestasDeMaria.Domain.Interfaces.Services.IBlobStorageService;
using ILoggerService = CestasDeMaria.Application.Interfaces.ILoggerAppService;
using IMainRepository = CestasDeMaria.Domain.Interfaces.Repository.IAdminsRepository;
using IMainService = CestasDeMaria.Application.Interfaces.IAdminsAppService;
using IMailMessageService = CestasDeMaria.Application.Interfaces.IMailMessageAppService;
using Main = CestasDeMaria.Domain.Entities.Admins;
using MainDTO = CestasDeMaria.Application.DTO.AdminsDTO;
using Microsoft.Extensions.Options;
using CestasDeMaria.Domain.ModelClasses;
using CestasDeMaria.Application.Helpers;

namespace CestasDeMaria.Application.Services
{
    public class AdminsAppService : ServiceBase<MainDTO>, IMainService
    {
        private readonly IMainRepository _mainRepository;
        private readonly ILoggerService _loggerService;
        private readonly IMailMessageService _mailMessageService;

        private string[] allowInclude = new string[] { };

        public AdminsAppService(IBlobStorageService blobStorageService, IOptions<Settings> options, IMainRepository mainRepository, ILoggerService loggerService, IMailMessageService mailMessageService)
            : base(blobStorageService, options)
        {
            _mainRepository = mainRepository;
            _loggerService = loggerService;
            _mailMessageService = mailMessageService;
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

        public async Task<MainDTO> GetByUsernameAsync(string username, string? include = null)
        {
            var result = await _mainRepository.GetByUsernameAsync(username, IncludesMethods.GetIncludes(include, allowInclude));
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
            
            main.Passwordhash = "12345";
            main.Guid = Guid.NewGuid().ToString();

            _mainRepository.Add(main);
            await _mailMessageService.SendMail(main.Username, new string[] { $"{_settings.PortalUrl}/confirma?token={main.Guid}", main.Name }, Infrastructure.CrossCutting.Enums.Enums.EmailType.Wellcome);

            await _mainRepository.CommitAsync();

            return main.ProjectedAs<MainDTO>();
        }

        public async Task<MainDTO> InactiveUserAsync(long id, long user)
        {
            var main = await _mainRepository.GetAsync(id);
            
            await _loggerService.InsertAsync($"Inativando usuário {main.Username}", user);

            main.Updated = DateTime.UtcNow;
            main.IsDeleted = 1;
            main.IsActive = 0;

            _mainRepository.Update(main);
            await _mainRepository.CommitAsync();

            return main.ProjectedAs<MainDTO>();
        }

        public async Task<MainDTO> ActiveUserAsync(long id, long user)
        {
            var main = await _mainRepository.GetAsync(id);

            await _loggerService.InsertAsync($"Ativando usuário {main.Username}", user);

            main.Updated = DateTime.UtcNow;
            main.IsDeleted = 0;
            main.IsActive = 1;

            _mainRepository.Update(main);
            await _mainRepository.CommitAsync();

            return main.ProjectedAs<MainDTO>();
        }

        public async Task<MainDTO> ConfirmUser(string password, string guid)
        {
            var main = await _mainRepository.GetByGuidAsync(guid);

            if(main == null)
            {
                return null;
            }

            main.Updated = DateTime.UtcNow;
            main.IsDeleted = 0;
            main.IsActive = 1;
            main.Guid = string.Empty;
            main.Passwordhash = password;

            _mainRepository.Update(main);
            await _mainRepository.CommitAsync();

            return main.ProjectedAs<MainDTO>();
        }

        public async Task<bool> RecoverPassword(string email)
        {
            var main = await _mainRepository.GetByUsernameAsync(email);

            if (main == null)
            {
                return false;
            }

            main.Guid = Guid.NewGuid().ToString();
            main.Updated = DateTime.Now;
            _mainRepository.Update(main);
            await _mainRepository.CommitAsync();

            await _mailMessageService.SendMail(main.Username, new string[] { $"{_settings.PortalUrl}/recover?token={main.Guid}", main.Name }, Infrastructure.CrossCutting.Enums.Enums.EmailType.RecoveryPassword);

            return true;
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

        public void Dispose()
        {
            _mainRepository.Dispose();
        }
    }
}
