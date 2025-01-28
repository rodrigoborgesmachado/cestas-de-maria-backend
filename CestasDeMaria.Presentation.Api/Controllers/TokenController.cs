using CestasDeMaria.Presentation.Api.Handler;
using CestasDeMaria.Application.Interfaces;
using CestasDeMaria.Domain.ModelClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using CestasDeMaria.Presentation.Model.Requests;
using Amazon.Runtime;

namespace CestasDeMaria.Presentation.Api.Controllers
{
    [ApiController]
    [Route("token")]
    [Authorize]
    public class TokenController : IDisposable
    {
        private readonly IConfiguration _builder;
        private readonly IAdminsAppService _adminsAppService;
        private readonly ILoggerAppService _loggerAppService;
        private readonly Settings _settings;
        private readonly TokenHandler _tokenHandler;

        public TokenController(IConfiguration builder, ILoggerAppService loggerAppService, IOptions<Settings> settings, IHttpContextAccessor httpContextAccessor, IAdminsAppService adminsAppService)
        {
            _builder = builder;
            _loggerAppService = loggerAppService;
            _settings = settings.Value;
            _tokenHandler = new TokenHandler(httpContextAccessor);
            _adminsAppService = adminsAppService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IResult> TokenAsync(TokenRequest request)
        {
            var result = await _adminsAppService.GetByUsernameAsync(request.UserName);

            if (result == null)
            {
                return Results.Unauthorized();
            }

            if(!result.Passwordhash.Equals(request.Password)) 
            { 
                return Results.Json(new {
                    code = 401,
                    message = "Senha incorreta!" 
                }); 
            }

            var claims = new[]
            {
                new Claim("Id", result.Id.ToString()),
                new Claim("Email", result.Username),
                new Claim("Name", result.Name),
                new Claim("UserName", result.Username),
                new Claim("IsAdmin", "1")
            };

            Token token = _tokenHandler.CreateToken(claims, _builder);
            return Results.Ok(token);
        }

        public void Dispose()
        {
            _loggerAppService.Dispose();
        }
    }
}
