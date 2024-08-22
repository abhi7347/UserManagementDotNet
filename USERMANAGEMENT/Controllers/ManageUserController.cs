using DomainLayer.DTOs.UserManageDTO;
using IServiceLayer.IManageUserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using ServiceLayer.ManageUserService;
using System.Reflection.Metadata.Ecma335;
using USERMANAGEMENT.Models;
using Newtonsoft.Json;
using System.Text.Json;
using OfficeOpenXml;
using DomainLayer.DTOs;
using Microsoft.IdentityModel.Tokens;
using IServiceLayer.ITokenService;


namespace USERMANAGEMENT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageUserController : ControllerBase
    {
        string path = null;
        private readonly IMangeUserService _iservice;
        private readonly ITokenService _tokenService;

        public ManageUserController(IMangeUserService iservice, ITokenService tokenService)
        {
            _iservice = iservice;
            _tokenService = tokenService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUser([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var users = await _iservice.GetAllUser(pageNumber, pageSize);

            // Use System.Text.Json for JSON serialization with appropriate settings
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true // Optional: pretty print the JSON
            };

            // Return the users as a JsonResult with the specified options
            return new JsonResult(users.Users, options);

            /*  return Ok(users.Users); */
        }


        [HttpPost("CreateUser")]
        public async Task<IActionResult> AddUser([FromForm] UserDetailsDTO user)
        {
            if (user == null)
            {
                return BadRequest("Invalid user data.");
            }

            var (userId, password) = await _iservice.AddUser(user);

            if (userId > 0)
            {
                var token = _tokenService.GenerateToken(user.Email);
                return Ok(new { Token = token, UserId = userId });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "User could not be created.");
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _iservice.DeleteUser(id);
                return Ok("User deleted successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UserDetailsDTO user)
        {
            if (user.UserId == null)
            {
                return BadRequest("User is null.");
            }

            try
            {
                await _iservice.UpdateUser(user.UserId, user);
                return Ok("User updated successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("FilterUser")]
        public async Task<IActionResult> FilterUser([FromQuery] bool filter)
        {
                var users = await _iservice.FilterUsers(filter);

            // Use System.Text.Json for JSON serialization with appropriate settings
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true // Optional: pretty print the JSON
            };

            // Return the users as a JsonResult with the specified options
            return new JsonResult(users, options);
        }

        [HttpGet("SortUser/{sortColumn}")]
        public async Task<IActionResult> SortUsers(string sortColumn)
        {
            var users = await _iservice.SortUsers(sortColumn);

            // Use System.Text.Json for JSON serialization with appropriate settings
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true // Optional: pretty print the JSON
            };

            // Return the users as a JsonResult with the specified options
            return new JsonResult(users, options);
            //return Ok(users);
        }

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword request)
        {
            if (request.Email == null)
            {
                return BadRequest("Token is required.");
            }

            try
            {
                await _iservice.ChangePassword(request);
                return Ok(true);
            }
            catch (SecurityTokenException)
            {
                return Unauthorized("Invalid token.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while changing the password.");
            }
        }

        // Dowload Excel file Controller
        [HttpGet("ExportExcel")]
        public async Task<IActionResult> ExportExcelDownload()
        {
            var users = await _iservice.ExportToExcel();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Users");

                // Adding Header
                worksheet.Cells[1, 1].Value = "FirstName";
                worksheet.Cells[1, 2].Value = "MiddleName";
                worksheet.Cells[1, 3].Value = "LastName";
                worksheet.Cells[1, 4].Value = "Gender";
                worksheet.Cells[1, 5].Value = "DateOfJoining";
                worksheet.Cells[1, 6].Value = "DOB";
                worksheet.Cells[1, 7].Value = "Email";
                worksheet.Cells[1, 8].Value = "Phone";
                worksheet.Cells[1, 9].Value = "AlternatePhone";

                // Adding Data
                for (int i = 0; i < users.Count(); i++)
                {
                    var user = users.ElementAt(i);
                    worksheet.Cells[i + 2, 1].Value = user.FirstName;
                    worksheet.Cells[i + 2, 2].Value = user.MiddleName;
                    worksheet.Cells[i + 2, 3].Value = user.LastName;
                    worksheet.Cells[i + 2, 4].Value = user.Gender;
                    worksheet.Cells[i + 2, 5].Value = user.DateOfJoining.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 6].Value = user.Dob.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 7].Value = user.Email;
                    worksheet.Cells[i + 2, 8].Value = user.Phone;
                    worksheet.Cells[i + 2, 9].Value = user.AlternatePhone;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string excelName = $"Users-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
    }
}
