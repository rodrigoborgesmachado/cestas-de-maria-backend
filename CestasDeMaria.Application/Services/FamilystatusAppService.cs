using IBlobStorageService = CestasDeMaria.Domain.Interfaces.Services.IBlobStorageService;
using ILoggerService = CestasDeMaria.Application.Interfaces.ILoggerAppService;
using IMainRepository = CestasDeMaria.Domain.Interfaces.Repository.IFamilystatusRepository;
using IMainService = CestasDeMaria.Application.Interfaces.IFamilystatusAppService;
using Main = CestasDeMaria.Domain.Entities.Familystatus;
using MainDTO = CestasDeMaria.Application.DTO.FamilystatusDTO;
using Microsoft.Extensions.Options;
using CestasDeMaria.Domain.ModelClasses;
using CestasDeMaria.Application.Helpers;

namespace CestasDeMaria.Application.Services
{
    public class FamilystatusAppService : ServiceBase<MainDTO>, IMainService
    {
        private readonly IMainRepository _mainRepository;
        private readonly ILoggerService _loggerService;

        private string[] allowInclude = new string[] { };

        public FamilystatusAppService(IBlobStorageService blobStorageService, IOptions<Settings> options, IMainRepository mainRepository, ILoggerService loggerService)
            : base(blobStorageService, options)
        {
            _mainRepository = mainRepository;
            _loggerService = loggerService;
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

        public void Dispose()
        {
            _mainRepository.Dispose();
        }
    }
}
