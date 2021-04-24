using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using API.Data.Interface;
using API.DTOs;
using Microsoft.AspNetCore.Hosting;

namespace API.Controllers
{
    public class AuthController : ApiController
    {
        private readonly IDKSDAO _dksDAO;
        public AuthController(IConfiguration config, IWebHostEnvironment webHostEnvironment, IDKSDAO dksDAO)
                 : base(config, webHostEnvironment)
        {
            _dksDAO = dksDAO;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto userForLoginDto)
        {

            var userFromRepo = await _dksDAO.SearchStaffByLOGIN(userForLoginDto.Account);

            if (userFromRepo == null)
            {
                    return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.LOGINNAME.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.PROGNAME)
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