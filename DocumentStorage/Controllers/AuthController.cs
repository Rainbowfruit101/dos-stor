using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services;
using ViewModels;

namespace DocumentStorage.Controllers
{
    [ApiController, Route("api/auth")]
    public class AuthController: ControllerBase
    {
        private readonly DocumentStorageContext _dbContext;
        private readonly UserService _userService;

        public AuthController(DocumentStorageContext dbContext, UserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }
        
        [HttpPost("token")]
        public async Task<IActionResult> Token(UserAuth userAuth)
        {
            var identity = _userService.GetIdentity(userAuth);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Неверный логин или пароль" });
            }
 
            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: AuthOptions.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.Lifetime)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
 
            var response = new
            {
                accessToken = encodedJwt,
                username = identity.Name
            };
 
            return Ok(new UserToken()
            {
                Token = encodedJwt,
                Username =  identity.Name
            });
        }

        [HttpGet("hash")]
        public async Task<IActionResult> HashPassword(string password) => Ok(_userService.HashPassword(password));
    }
}