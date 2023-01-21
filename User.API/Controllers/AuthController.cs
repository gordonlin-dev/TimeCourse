using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace User.API.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        
        public AuthController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost]
        public async Task<IActionResult> Auth([FromBody] string token)
        {
            var jwtModel = _jwtService.ValidateToken(token);
            if (jwtModel != null)
            {
                try
                {
                    var user = await _userService.GetUserByEmailAsync(jwtModel.Email);
                    if (user == null)
                    {                   
                        user = await _userService.CreateUserAsync(jwtModel.Email, jwtModel.Name);
                    }
                    var newToken = _jwtService.GenerateToken(_jwtService.GenerateClaimsPrincipalFromUser(user));
                    return new OkObjectResult(newToken);
                }
                catch (Exception ex)
                {

                }                             
            }
            return new BadRequestResult();
        }
    }
}
