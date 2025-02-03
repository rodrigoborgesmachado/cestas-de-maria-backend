using CestasDeMaria.Presentation.Api.Handler;
using CestasDeMaria.Application.Helpers;
using CestasDeMaria.Domain.ModelClasses;
using CestasDeMaria.Presentation.Model.Returns;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using IMainAppService = CestasDeMaria.Application.Interfaces.ILoggerAppService;
using MainDTO = CestasDeMaria.Application.DTO.LoggerDTO;
using MainViewModel = CestasDeMaria.Presentation.Model.ViewModels.LoggerViewModel;

namespace CestasDeMaria.Presentation.Api.Controllers
{
    /// <summary>
    /// Controller Class
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("logger")]
    public class LoggerController : ControllerBase, IDisposable
    {
        private readonly IMainAppService _mainAppService;
        private readonly Settings _settings;

        private readonly TokenHandler tokenController;

        /// <summary>
        /// Class constructor
        /// </summary>
        public LoggerController(IMainAppService mainAppService, IOptions<Settings> options, IHttpContextAccessor httpContextAccessor)
        {
            _mainAppService = mainAppService;
            _settings = options.Value;

            tokenController = new TokenHandler(httpContextAccessor);
        }

        /// <summary>
        /// List paged
        /// </summary>
        /// <param name="page"></param>
        /// <param name="quantity"></param>
        /// <param name="term"></param>
        /// <param name="orderBy"></param>
        /// <param name="include"></param>
        /// <returns><![CDATA[Task<PaggedBaseReturn<MainViewModel>>]]></returns>
        [HttpGet("pagged")]
        public async Task<IActionResult> GetPagged(int page, int quantity, DateTime? startDate, DateTime? endDate, string term = null, string orderBy = null, string? include = null)
        {
            var result = await _mainAppService.GetAllPagedAsync(page, quantity, startDate, endDate, term, orderBy, include);

            var list = result.Item3.ProjectedAsCollection<MainViewModel>();

            return Ok(
                new PaggedBaseReturn<MainViewModel>
                {
                    Page = page,
                    Quantity = quantity,
                    TotalCount = result.Item1,
                    TotalPages = result.Item2,
                    Results = list
                }
            );
        }

        /// <summary>
        /// Get all
        /// </summary>
        /// <param name="include"></param>
        /// <returns><![CDATA[Task<IEnumerable<MainViewModel>>]]></returns>
        [HttpGet]
        public async Task<IActionResult> Get(DateTime dateBegin, DateTime dateFinal, string filtro, int page = 0, int quantity = int.MaxValue, string orderby = "", string include = "")
        {
            var mainDto = await _mainAppService.GetAllAsync(dateBegin, dateFinal, filtro, page, quantity, orderby, include);

            return Ok(mainDto.Item3.ProjectedAsCollection<MainViewModel>());
        }

        /// <summary>
        /// Get by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns><![CDATA[Task<MainViewModel>]]></returns>
        [HttpGet("{code}")]
        public async Task<IActionResult> Get(long code)
        {
            var mainDto = await _mainAppService.GetAsync(code);

            return Ok(mainDto.ProjectedAs<MainViewModel>());
        }

        /// <summary>
		/// Insert new
		/// </summary>
		/// <param name="model"></param>
		/// <returns><![CDATA[Task<IActionResult>]]></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string texto)
        {
            var result = await _mainAppService.InsertAsync(texto);

            return Ok(result);
        }

        /// <summary>
        /// List paged
        /// </summary>
        /// <param name="quantityMax"></param>
        /// <param name="isActive"></param>
        /// <param name="term"></param>
        /// <param name="orderBy"></param>
        /// <param name="include"></param>
        /// <returns><![CDATA[Task<PaggedBaseReturn<MainViewModel>>]]></returns>
        [HttpGet("export")]
        public async Task<IActionResult> Export(int quantityMax, DateTime? startDate, DateTime? endDate, string isActive = null, string term = null, string orderBy = null, string? include = null)
        {
            var result = await _mainAppService.GetReport(quantityMax, startDate, endDate, isActive, term, orderBy: orderBy, include: include);

            return Ok(new BaseReturn<string>
            {
                Message = "Report created",
                Status = 200,
                Object = result
            });
        }

        /// <summary>
		/// Dispose
		/// </summary>
		/// <param name="disposing"></param>
		/// <returns><![CDATA[]]></returns>
        public void Dispose()
        {
            _mainAppService.Dispose();
        }
    }
}
