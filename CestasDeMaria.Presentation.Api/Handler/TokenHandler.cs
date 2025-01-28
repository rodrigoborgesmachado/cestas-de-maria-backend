using CestasDeMaria.Domain.ModelClasses;
using CestasDeMaria.Infrastructure.CrossCutting.Enums;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CestasDeMaria.Presentation.Api.Handler
{
    public class TokenHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserInfo> GetUserFromRequest()
        {
            UserInfo info = new UserInfo()
            {
                id = long.Parse(ValueFromClaimRequest("id")),
                email = ValueFromClaimRequest("email"),
                name = ValueFromClaimRequest("name"),
                username = ValueFromClaimRequest("username"),
                IsAdmin = ValueFromClaimRequest("isadmin").Equals("1"),
            };

            return info;
        }

        public string GetLocalPah()
        {
            return _httpContextAccessor.HttpContext.Request.Path;
        }

        public string GetHeaderToString()
        {
            return _httpContextAccessor.HttpContext.Request.Headers.ToString();
        }

        private string ValueFromClaimRequest(string key)
        {
            if (_httpContextAccessor.HttpContext == null || _httpContextAccessor.HttpContext.User == null || _httpContextAccessor.HttpContext.User.Claims.Count() == 0)
                return string.Empty;

            var to_return = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(u => u.Type.ToUpper().Equals(key.ToUpper()))?.Value;

            return to_return;
        }

        public Token CreateToken(IEnumerable<Claim> claims, IConfiguration _builder)
        {
            var identity = new ClaimsIdentity(claims);

            var key = Encoding.ASCII.GetBytes(_builder.GetValue<string>("Jwt:Key"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature),
                NotBefore = DateTime.Now.AddDays(-1),
                Issuer = _builder.GetValue<string>("Jwt:Issuer"),
                Audience = _builder.GetValue<string>("Jwt:Audience"),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var stringToken = tokenHandler.WriteToken(token);

            TimeSpan duration = tokenDescriptor.Expires.Value - DateTime.UtcNow;

            Token result = new Token()
            {
                access_token = stringToken,
                expires_in = (int)duration.TotalSeconds,
                token_type = "Bearer",
                id = claims.Where(i => i.Type.Equals("Id")).FirstOrDefault().Value,
                issued = DateTime.Now.ToLongDateString(),
                expires = DateTime.UtcNow.AddDays(1).ToLongDateString(),
                username = claims.Where(i => i.Type.Equals("UserName")).FirstOrDefault().Value,
                name = claims.Where(i => i.Type.Equals("Name")).FirstOrDefault().Value,
                email = claims.Where(i => i.Type.Equals("Email")).FirstOrDefault().Value,
                isadmin = claims.Where(i => i.Type.Equals("IsAdmin")).FirstOrDefault().Value
            };

            return result;
        }
    }
}
