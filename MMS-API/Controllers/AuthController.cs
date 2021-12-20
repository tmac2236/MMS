using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using API.DTOs;

namespace API.Controllers
{
    public class AuthController : ApiController
    {
        public AuthController(IConfiguration config, IWebHostEnvironment webHostEnvironment, ILogger<AuthController> logger)
                 : base(config, webHostEnvironment,logger)
        {
            
        }
        [HttpPost("login")]
        public IActionResult Login(UserDto userForLoginDto)
        {
            _logger.LogInformation(String.Format(@"******  AuthController Login fired!! ******"));
            //var theModel = _userDAO
            //        .FindSingle(x => x.Account == userForLoginDto.Account && x.Password == userForLoginDto.Password);
            var theModel = "test";
            if (theModel == null)
            {
                return Unauthorized();
            }
            var version = _config.GetSection("AppSettings:Version").Value;
            var updateTime = _config.GetSection("AppSettings:UpdateTime").Value;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "Stan"),
                new Claim(ClaimTypes.Role, "ADM"),
                new Claim(ClaimTypes.Version, "v.10"),
                new Claim(ClaimTypes.DateOfBirth, "1990-02-27")
                };
            var tokenName = _config.GetSection("AppSettings:Token").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(tokenName));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });

        }

    }
}