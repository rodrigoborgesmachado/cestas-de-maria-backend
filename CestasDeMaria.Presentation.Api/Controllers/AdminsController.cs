using CestasDeMaria.Presentation.Api.Handler;
using CestasDeMaria.Application.Helpers;
using CestasDeMaria.Domain.ModelClasses;
using CestasDeMaria.Presentation.Model.Returns;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using IMainAppService = CestasDeMaria.Application.Interfaces.IAdminsAppService;
using MainDTO = CestasDeMaria.Application.DTO.AdminsDTO;
using MainViewModel = CestasDeMaria.Presentation.Model.ViewModels.AdminsViewModel;
using CestasDeMaria.Presentation.Model.Requests;

namespace CestasDeMaria.Presentation.Api.Controllers
{
    /// <summary>
    /// Controller Class
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("Admins")]
    public class AdminsController : ControllerBase, IDisposable
    {
        private readonly IMainAppService _mainAppService;
        private readonly Settings _settings;

        private readonly TokenHandler tokenController;

        /// <summary>
        /// Class constructor
        /// </summary>
        public AdminsController(IMainAppService mainAppService, IOptions<Settings> options, IHttpContextAccessor httpContextAccessor)
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
        public async Task<IActionResult> GetPagged(int page, int quantity, string isActive = null, string term = null, string orderBy = null, string? include = null)
        {
            var result = await _mainAppService.GetAllPagedAsync(page, quantity, isActive, term, orderBy: orderBy, include: include);

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
        /// List paged
        /// </summary>
        /// <param name="quantityMax"></param>
        /// <param name="isActive"></param>
        /// <param name="term"></param>
        /// <param name="orderBy"></param>
        /// <param name="include"></param>
        /// <returns><![CDATA[Task<PaggedBaseReturn<MainViewModel>>]]></returns>
        [HttpGet("export")]
        public async Task<IActionResult> Export(int quantityMax, string isActive = null, string term = null, string orderBy = null, string? include = null)
        {
            var result = await _mainAppService.GetReport(quantityMax, isActive, term, orderBy: orderBy, include: include);

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

            var exists = await _mainAppService.GetByUsernameAsync(mainDto.Username);
            if(exists != null)
            {
                return BadRequest("Usuário já existe!");
            }

            var result = await _mainAppService.InsertAsync(mainDto);

            return Ok(result);
        }

        /// <summary>
		/// Insert new
		/// </summary>
		/// <param name="model"></param>
		/// <returns><![CDATA[Task<IActionResult>]]></returns>
        [HttpPost("confirm-user")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmUser([FromBody] ConfirmUserRequest model)
        {
            if (model.Password != model.VerifyPassword)
            {
                return BadRequest("Senha precisa ser igual!");
            }

            var result = await _mainAppService.ConfirmUser(model.Password, model.Guid);

            return Ok(result);
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
		/// Update
		/// </summary>
		/// <param name="model"></param>
		/// <returns><![CDATA[Task<IActionResult>]]></returns>
        [HttpPost("inactive-user/{id}")]
        public async Task<IActionResult> InactiveUserAsync(long id)
        {
            var user = await tokenController.GetUserFromRequest();

            var result = await _mainAppService.InactiveUserAsync(id, user.id);

            return Ok(result);
        }

        /// <summary>
		/// Update
		/// </summary>
		/// <param name="model"></param>
		/// <returns><![CDATA[Task<IActionResult>]]></returns>
        [HttpPost("active-user/{id}")]
        public async Task<IActionResult> ActiveUserAsync(long id)
        {
            var user = await tokenController.GetUserFromRequest();

            var result = await _mainAppService.ActiveUserAsync(id, user.id);

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
