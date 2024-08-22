using DomainLayer.DTOs;
using IServiceLayer.ITokenService;
using IServiceLayer.UserLogin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace USERMANAGEMENT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILoginService _loginService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<LoginController> _logger;


        public LoginController(IConfiguration config, ILoginService ILoginService, ITokenService tokenService, ILogger<LoginController> logger)
        {
            _config = config;
            _loginService = ILoginService;
            _tokenService = tokenService;
            _logger = logger;

        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginValue)
        {
            if (loginValue.Email == null || loginValue.Password == null)
            {
                return BadRequest("Invalid client request");
            }

            bool isAuthenticated = await _loginService.LoginUser(loginValue);

            if (isAuthenticated)
            {
                var token = _tokenService.GenerateToken(loginValue.Email);
                return Ok(new { Token = token });
            }
            else
            {
                // Log the login attempt details for troubleshooting
                _logger.LogWarning($"Failed login attempt for email: {loginValue.Email}");
                return Unauthorized("Invalid login credentials");
            }
        }

    }
}
