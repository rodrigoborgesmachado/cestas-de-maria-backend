using CestasDeMaria.Application.Helpers;
using CestasDeMaria.Domain.ModelClasses;
using CestasDeMaria.Infrastructure.CrossCutting.Enums;
using CestasDeMaria.Presentation.Api.Handler;
using CestasDeMaria.Presentation.Model.Returns;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using static CestasDeMaria.Infrastructure.CrossCutting.Enums.Enums;
using IMainAppService = CestasDeMaria.Application.Interfaces.IFamiliesAppService;
using IBasketDelivery = CestasDeMaria.Application.Interfaces.IBasketdeliveriesAppService;
using MainDTO = CestasDeMaria.Application.DTO.FamiliesDTO;
using MainViewModel = CestasDeMaria.Presentation.Model.ViewModels.FamiliesViewModel;

namespace CestasDeMaria.Presentation.Api.Controllers
{
    /// <summary>
    /// Controller Class
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("Families")]
    public class FamiliesController : ControllerBase, IDisposable
    {
        private readonly IMainAppService _mainAppService;
        private readonly IBasketDelivery _basketDelivery;
        private readonly Settings _settings;

        private readonly TokenHandler tokenController;

        /// <summary>
        /// Class constructor
        /// </summary>
        public FamiliesController(IMainAppService mainAppService, IOptions<Settings> options, IHttpContextAccessor httpContextAccessor, IBasketDelivery basketDelivery)
        {
            _mainAppService = mainAppService;
            _settings = options.Value;

            tokenController = new TokenHandler(httpContextAccessor);
            _basketDelivery = basketDelivery;
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
        /// Get by phone
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
        /// Get by document
        /// </summary>
        /// <param name="document"></param>
        /// <param name="include"></param>
        /// <returns><![CDATA[Task<MainViewModel>]]></returns>
        [HttpGet("getbydocument/{document}")]
        public async Task<IActionResult> GetByDocument(string document, string? include = null)
        {
            var mainDto = await _mainAppService.GetByDocumentAsync(document, include);

            return Ok(mainDto.ProjectedAs<MainViewModel>());
        }

        /// <summary>
        /// Get by phone
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="include"></param>
        /// <returns><![CDATA[Task<MainViewModel>]]></returns>
        [HttpGet("getbyphone/{phone}")]
        public async Task<IActionResult> GetByPhone(string phone, string? include = null)
        {
            var mainDto = await _mainAppService.GetByPhoneAsync(phone, include);

            return Ok(mainDto.ProjectedAs<MainViewModel>());
        }

        /// <summary>
        /// List paged
        /// </summary>
        /// <param name="page"></param>
        /// <param name="quantity"></param>
        /// <param name="isActive"></param>
        /// <param name="term"></param>
        /// <param name="orderBy"></param>
        /// <param name="include"></param>
        /// <returns><![CDATA[Task<PaggedBaseReturn<MainViewModel>>]]></returns>
        [HttpGet("pagged")]
        public async Task<IActionResult> GetPagged(int page, int quantity, Enums.FamilyStatus? status, string isActive = null, string term = null, string orderBy = null, string? include = null)
        {
            var result = await _mainAppService.GetAllPagedAsync(page, quantity, status, isActive, term, orderBy: orderBy, include: include);

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
        /// Report
        /// </summary>
        /// <param name="status"></param>
        /// <param name="isActive"></param>
        /// <param name="term"></param>
        /// <param name="orderBy"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        [HttpGet("export")]
        public async Task<IActionResult> Export(Enums.FamilyStatus? status, string isActive = null, string term = null, string orderBy = null, string? include = null)
        {
            var result = await _mainAppService.GetReport(status, isActive, term, orderBy, include);

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
            if(!string.IsNullOrEmpty(model.Document))
            {
                var exists = await _mainAppService.GetByDocumentAsync(Regex.Replace(model.Document, @"\D", ""));
                if(exists != null)
                {
                    return BadRequest(new
                    {
                        code = 401,
                        message = "Documento já cadastrado!"
                    });
                }
            }

            if (!string.IsNullOrEmpty(model.Phone))
            {
                var exists = await _mainAppService.GetByPhoneAsync(Regex.Replace(model.Phone, @"\D", ""));
                if (exists != null)
                {
                    return BadRequest(new
                    {
                        code = 401,
                        message = "Telefone já cadastrado!"
                    });
                }
            }

            var user = await tokenController.GetUserFromRequest();

            var mainDto = model.ProjectedAs<MainDTO>();
            mainDto.Createdby = user.id;
            mainDto.Updatedby = user.id;

            var result = await _mainAppService.InsertAsync(mainDto);

            return Ok(result);
        }

        /// <summary>
		/// Update
		/// </summary>
		/// <param name="model"></param>
		/// <returns><![CDATA[Task<IActionResult>]]></returns>
        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] MainViewModel model)
        {
            var user = await tokenController.GetUserFromRequest();
            
            var mainDto = model.ProjectedAs<MainDTO>();
            mainDto.Updatedby = user.id;

            var result = await _mainAppService.UpdateAsync(id, mainDto);

            await _basketDelivery.DeleteNextNonAttendFamilies(DateTime.Now, user.id);

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
		/// Insert new
		/// </summary>
		/// <param name="model"></param>
		/// <returns><![CDATA[Task<IActionResult>]]></returns>
        [HttpPost("update-status/{id}")]
        public async Task<IActionResult> UpdateStatus(long id, [FromQuery] FamilyStatus status)
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
