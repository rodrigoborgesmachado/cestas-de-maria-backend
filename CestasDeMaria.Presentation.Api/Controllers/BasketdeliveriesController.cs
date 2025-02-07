using CestasDeMaria.Presentation.Api.Handler;
using CestasDeMaria.Application.Helpers;
using CestasDeMaria.Domain.ModelClasses;
using CestasDeMaria.Presentation.Model.Returns;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using IMainAppService = CestasDeMaria.Application.Interfaces.IBasketdeliveriesAppService;
using MainDTO = CestasDeMaria.Application.DTO.BasketdeliveriesDTO;
using MainViewModel = CestasDeMaria.Presentation.Model.ViewModels.BasketdeliveriesViewModel;
using static CestasDeMaria.Infrastructure.CrossCutting.Enums.Enums;
using CestasDeMaria.Presentation.Model.ViewModels;

namespace CestasDeMaria.Presentation.Api.Controllers
{
    /// <summary>
    /// Controller Class
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("BasketDeliveries")]
    public class BasketdeliveriesController : ControllerBase, IDisposable
    {
        private readonly IMainAppService _mainAppService;
        private readonly Settings _settings;

        private readonly TokenHandler tokenController;

        /// <summary>
        /// Class constructor
        /// </summary>
        public BasketdeliveriesController(IMainAppService mainAppService, IOptions<Settings> options, IHttpContextAccessor httpContextAccessor)
        {
            _mainAppService = mainAppService;
            _settings = options.Value;

            tokenController = new TokenHandler(httpContextAccessor);
        }

        /// <summary>
        /// Get all
        /// </summary>
        /// <param name="include"></param>
        /// <returns><![CDATA[Task<IEnumerable<MainViewModel>>]]></returns>
        [HttpGet]
        public async Task<IActionResult> Get(string? include = null)
        {
            var mainDto = await _mainAppService.GetAllAsync(include);

            return Ok(mainDto.ProjectedAsCollection<MainViewModel>());
        }

        /// <summary>
        /// Get by code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="include"></param>
        /// <returns><![CDATA[Task<MainViewModel>]]></returns>
        [HttpGet("{code}")]
        public async Task<IActionResult> Get(long code, string? include = null)
        {
            var mainDto = await _mainAppService.GetAsync(code, include);

            return Ok(mainDto.ProjectedAs<MainViewModel>());
        }

        /// <summary>
        /// Get by code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="include"></param>
        /// <returns><![CDATA[Task<MainViewModel>]]></returns>
        [HttpGet("GetByDate")]
        public async Task<IActionResult> GetByDate(DateTime date, string? include = null)
        {
            var user = await tokenController.GetUserFromRequest();
            var mainDto = await _mainAppService.GetAndGenerateWeeklyBasketDeliveriesAsync(date, user.id);

            return Ok(mainDto.ProjectedAsCollection<MainViewModel>());
        }

        /// <summary>
        /// List paged
        /// </summary>
        /// <param name="page"></param>
        /// <param name="quantity"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isActive"></param>
        /// <param name="term"></param>
        /// <param name="orderBy"></param>
        /// <param name="include"></param>
        /// <returns><![CDATA[Task<PaggedBaseReturn<MainViewModel>>]]></returns>
        [HttpGet("pagged")]
        public async Task<IActionResult> GetPagged(int page, int quantity, DateTime? startDate, DateTime? endDate, string isActive = null, string term = null, string orderBy = null, string? include = null)
        {
            var result = await _mainAppService.GetAllPagedAsync(page, quantity, startDate, endDate, isActive, term, orderBy: orderBy, include: include);

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
        /// 
        /// </summary>
        /// <param name="quantityMax"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isActive"></param>
        /// <param name="term"></param>
        /// <param name="orderBy"></param>
        /// <param name="include"></param>
        /// <returns></returns>
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
		/// Insert new
		/// </summary>
		/// <param name="model"></param>
		/// <returns><![CDATA[Task<IActionResult>]]></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MainViewModel model)
        {
            var mainDto = model.ProjectedAs<MainDTO>();
            
            var user = await tokenController.GetUserFromRequest();
            mainDto.Createdby = user.id;
            mainDto.Updatedby = user.id;

            var result = await _mainAppService.InsertAsync(mainDto);

            return Ok(result);
        }

        /// <summary>
		/// Insert new
		/// </summary>
		/// <param name="model"></param>
		/// <returns><![CDATA[Task<IActionResult>]]></returns>
        [HttpPost("update-status/{id}")]
        public async Task<IActionResult> UpdateStatus(long id, [FromQuery]DeliveryStatus status)
        {
            var user = await tokenController.GetUserFromRequest();

            try
            {
                var result = await _mainAppService.UpdateStatus(id, user, status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
		/// Insert new
		/// </summary>
		/// <param name="model"></param>
		/// <returns><![CDATA[Task<IActionResult>]]></returns>
        [HttpPost("update-family/{id}/{newFamilyId}/{oldFamilyId}")]
        public async Task<IActionResult> UpdateFamily(long id, long newFamilyId, long oldFamilyId)
        {
            var user = await tokenController.GetUserFromRequest();

            try
            {
                var result = await _mainAppService.UpdateFamily(id, newFamilyId, oldFamilyId, user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
		/// Update
		/// </summary>
		/// <param name="model"></param>
		/// <returns><![CDATA[Task<IActionResult>]]></returns>
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] MainViewModel model)
        {
            var mainDto = model.ProjectedAs<MainDTO>();
            var result = await _mainAppService.UpdateAsync(mainDto);

            return Ok(result);
        }

        /// <summary>
		/// Remove
		/// </summary>
		/// <param name="model"></param>
		/// <returns><![CDATA[Task<IActionResult>]]></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] MainViewModel model)
        {
            var mainDto = model.ProjectedAs<MainDTO>();
            var result = await _mainAppService.RemoveAsync(mainDto);

            return Ok(result);
        }

        /// <summary>
        /// Método que busca os dados do dashboard
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet("dashboard-statistics")]
        public async Task<IActionResult> GetDashboardStatistics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate == default || endDate == default)
            {
                return BadRequest("Start date and end date are required.");
            }

            var statistics = await _mainAppService.GetDashboardStatisticsAsync(startDate, endDate);
            var viewModel = statistics.ProjectedAs<DashboardStatisticsViewModel>();

            return Ok(viewModel);
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
        [HttpGet("full-report")]
        public async Task<IActionResult> GetFullReport()
        {
            var result = await _mainAppService.GetFullReport();

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
