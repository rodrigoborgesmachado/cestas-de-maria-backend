using IBlobStorageService = CestasDeMaria.Domain.Interfaces.Services.IBlobStorageService;
using ILoggerService = CestasDeMaria.Application.Interfaces.ILoggerAppService;
using IMainRepository = CestasDeMaria.Domain.Interfaces.Repository.IFamiliesRepository;
using IMainService = CestasDeMaria.Application.Interfaces.IFamiliesAppService;
using IHistoryService = CestasDeMaria.Application.Interfaces.IFamilyfamilystatushistoryAppService;
using Main = CestasDeMaria.Domain.Entities.Families;
using MainDTO = CestasDeMaria.Application.DTO.FamiliesDTO;
using Microsoft.Extensions.Options;
using CestasDeMaria.Domain.ModelClasses;
using CestasDeMaria.Application.Helpers;
using System.Text.RegularExpressions;

namespace CestasDeMaria.Application.Services
{
    public class FamiliesAppService : ServiceBase<MainDTO>, IMainService
    {
        private readonly IMainRepository _mainRepository;
        private readonly ILoggerService _loggerService;
        private readonly IHistoryService _historyService;

        private string[] allowInclude = new string[] { "Familystatus", "Familyfamilystatushistory", "Familyfamilystatushistory.NewFamilystatus", "Familyfamilystatushistory.OldFamilystatus", "Admins" };

        public FamiliesAppService(IBlobStorageService blobStorageService, IOptions<Settings> options, IMainRepository mainRepository, ILoggerService loggerService, IHistoryService historyService)
            : base(blobStorageService, options)
        {
            _mainRepository = mainRepository;
            _loggerService = loggerService;
            _historyService = historyService;
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

        public async Task<MainDTO> GetByDocumentAsync(string document, string? include = null)
        {
            var result = await _mainRepository.GetByDocumentAsync(document, IncludesMethods.GetIncludes(include, allowInclude));
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

            main.Document = Regex.Replace(main.Document, @"\D", "");
            main.Phone = Regex.Replace(main.Phone, @"\D", "");

            _mainRepository.Add(main);
            await _mainRepository.CommitAsync();

            return main.ProjectedAs<MainDTO>();
        }

        public async Task<MainDTO> UpdateAsync(long id, MainDTO mainDto)
        {
            var main = await _mainRepository.GetAsync(id);

            if (main.Familystatusid != mainDto.Familystatusid)
            {
                await _loggerService.InsertAsync($"Alterando status da família {main.Id} pelo usuário {mainDto.Updatedby}", mainDto.Updatedby);

                await _historyService.InsertAsync(new DTO.FamilyfamilystatushistoryDTO()
                {
                    Familyid = main.Id,
                    Createdby = mainDto.Updatedby,
                    Updatedby = mainDto.Updatedby,
                    Newfamilystatusid = mainDto.Familystatusid,
                    Oldfamilystatusid = main.Familystatusid,
                });
            }

            main.Updatedby = mainDto.Updatedby;
            main.Updated = DateTime.UtcNow;
            main.Document = Regex.Replace(mainDto.Document, @"\D", "");
            main.Phone = Regex.Replace(mainDto.Phone, @"\D", "");
            main.Housingsituation = mainDto.Housingsituation;
            main.Children = mainDto.Children;
            main.Address = mainDto.Address;
            main.Neighborhood = mainDto.Neighborhood;
            main.Adults = mainDto.Adults;
            main.Basketquantity = mainDto.Basketquantity;
            main.DeliveryWeek = mainDto.DeliveryWeek;
            main.Familystatusid = mainDto.Familystatusid;
            main.Hasseverelimitation = mainDto.Hasseverelimitation;
            main.IsActive = mainDto.IsActive;
            main.IsDeleted = mainDto.IsDeleted;

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
