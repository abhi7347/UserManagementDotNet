using DomainLayer.DTOs;
using IServiceLayer.IResetPasswordService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace USERMANAGEMENT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResetPasswordController : ControllerBase
    {
        private readonly IResetPasswordService _resetPasswordService;

        public ResetPasswordController(IResetPasswordService resetPasswordService)
        {
            _resetPasswordService = resetPasswordService;
        }

        [HttpPut]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            if (model == null)
            {
                return BadRequest("Invalid client request");
            }

            var result = await _resetPasswordService.ResetPasswordAsync(model);

            if (!result)
            {
                return BadRequest("Password reset failed");
            }
            return Ok(new { result = "Password reset successfully"});
        }
    }
}
