using CestasDeMaria.Application.Helpers;
using CestasDeMaria.Domain.Interfaces.Services;
using CestasDeMaria.Domain.ModelClasses;
using Microsoft.Extensions.Options;
using IMainRepository = CestasDeMaria.Domain.Interfaces.Repository.ILoggerRepository;
using IMainService = CestasDeMaria.Application.Interfaces.ILoggerAppService;
using Main = CestasDeMaria.Domain.Entities.Logger;
using MainDTO = CestasDeMaria.Application.DTO.LoggerDTO;

namespace CestasDeMaria.Application.Services
{
    public class LoggerAppService : ServiceBase<MainDTO>, IMainService
    {
        private readonly IMainRepository _mainRepository;
        private readonly Settings _settings;
        private string[] allowInclude = new string[] { "Admins" };

        public LoggerAppService(IBlobStorageService blobStorageService, IMainRepository mainRepository, IOptions<Settings> options)
            : base(blobStorageService, options)
        {
            _mainRepository = mainRepository;
            _settings = options.Value;
        }

        public async Task<Main> InsertAsync(Exception ex)
        {
            var main = new Main(ex);

            _mainRepository.Add(main);
            await _mainRepository.CommitAsync();
            return main;
        }

        public async Task<Main> InsertAsync(string message)
        {
            return await InsertAsync(message, 1);
        }

        public async Task<Main> InsertAsync(string message, long userId)
        {
            var main = new Main(message, userId);

            _mainRepository.Add(main);
            await _mainRepository.CommitAsync();

            return main;
        }

        public async Task<MainDTO> GetAsync(long code)
        {
            var result = await _mainRepository.GetAsync(code);

            return result.ProjectedAs<MainDTO>();
        }

        public async Task<Tuple<int, int, IEnumerable<MainDTO>>> GetAllPagedAsync(int page, int quantity, DateTime? startDate, DateTime? endDate, string filtro = "", string orderby = "", string include = "")
        {
            var tuple = await _mainRepository.GetAllPagedAsync(page, quantity, startDate, endDate, filtro, orderby: orderby, include: IncludesMethods.GetIncludes(include, allowInclude));
            var total = tuple.Item1;
            var pages = (int)Math.Ceiling((double)total / quantity);

            var list = tuple.Item2.ProjectedAsCollection<MainDTO>();

            //foreach (var item in list)
            //{
            //    if (!string.IsNullOrEmpty(item.PersonCode) && item.PersonCode != "0")
            //    {
            //        var person = await _personService.GetAsync(item.PersonCode);
            //        if (person != null)
            //            item.PersonCode = person.Name;
            //    }
            //}

            return Tuple.Create(total, pages, list);
        }

        public async Task<Tuple<int, int, IEnumerable<MainDTO>>> GetAllAsync(DateTime dateBegin, DateTime dateFinal, string filtro, int page = 0, int quantity = int.MaxValue, string orderby = "", string include = "")
        {
            if (dateFinal.Hour == 0)
            {
                dateFinal = dateFinal.AddHours(23);
                dateFinal = dateFinal.AddMinutes(59);
            }

            var tuple = await _mainRepository.GetAllAsync(dateBegin, dateFinal, filtro, page, quantity, orderby: orderby, include: IncludesMethods.GetIncludes(include, allowInclude));
            var total = tuple.Item1;
            var pages = (int)Math.Ceiling((double)total / quantity);

            var list = tuple.Item2.ProjectedAsCollection<MainDTO>();

            //foreach (var item in list)
            //{
            //    if (!string.IsNullOrEmpty(item.PersonCode) && item.PersonCode != "0")
            //    {
            //        var person = await _personService.GetAsync(item.PersonCode);
            //        if (person != null)
            //            item.PersonCode = person.Name;
            //    }
            //}

            return Tuple.Create(total, pages, list);
        }

        public async Task<string> GetReport(int quantityMax, DateTime? startDate, DateTime? endDate, string isActive = null, string term = null, string orderBy = null, string? include = null)
        {
            await InsertAsync($"Report - Starting GetReport - {this.GetType().Name}");

            var result = await GetAllPagedAsync(1, quantityMax == 0 ? int.MaxValue : quantityMax, startDate, endDate, term, orderBy, include);
            string link = await UploadReport(result.Item3.ToList());

            await InsertAsync($"Report - Finishing GetReport - {this.GetType().Name}");
            return link;
        }

        public void Dispose()
        {
            _mainRepository.Dispose();
        }
    }
}
