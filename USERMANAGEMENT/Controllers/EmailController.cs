using DomainLayer.DTOs;
using IServiceLayer.EmailSetting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using USERMANAGEMENT.Models;

namespace USERMANAGEMENT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _iEmailService;
        private readonly SDirectContext _context;
        IConfiguration _config;

        public EmailController(IEmailService iEmailService, SDirectContext context, IConfiguration config)
        {
            _iEmailService = iEmailService;
            _context = context;
            _config = config;
        }
        [HttpPost]
        public async Task<IActionResult> EmailSender([FromBody] EmailRequestDTO model)
        {
            if (model == null || string.IsNullOrEmpty(model.ToEmail))
            {
                return BadRequest(new { message = "Invalid Email" });
            }

            try
            {
                
                var user = await _context.AbhiUsers.FirstOrDefaultAsync(ud => ud.Email == model.ToEmail);

                var resetPasswordLink = _config["Frontend:ResetPasswordLink"];
                var subject = "Reset Password";
                var email = user.Email;
                var htmlBody = "EmailBody.html";

                if (email != null)
                {
                     await _iEmailService.SendEmailAsync(email, subject, resetPasswordLink, htmlBody);
                    return Ok(new { message = "Email sent successfully" });
                }
                else
                {
                    return NotFound(new { message = "Email not registered" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

    }
}
