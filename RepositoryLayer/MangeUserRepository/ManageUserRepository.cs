using DomainLayer.DTOs.UserManageDTO;
using IRepositoryLayer.IMangeUserRepository;
using Microsoft.EntityFrameworkCore;
using USERMANAGEMENT.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Security.Cryptography;
using System.Web.Mvc;
using Microsoft.AspNetCore.Http;
using DomainLayer.DTOs;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using SharedLayer;
using Microsoft.Azure.ActiveDirectory.GraphClient.Internal;
using System.Web.Helpers;
using Microsoft.Data.SqlClient;



namespace RepositoryLayer.MangeUserRepository
{
    public class ManageUserRepository : IManageUserRepository
    {
        private readonly SDirectContext _context;
        private readonly IConfiguration _config;



        public ManageUserRepository(SDirectContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }


        public async Task<(int UserId, string Password)> AddUser(UserDetailsDTO user)
        {
            string randomPassword = string.Empty;
            int userId = 0;

            try
            {

                string imagePath = null;

                if (user.Image != null && user.Image.Length > 0)
                {
                    // Define custom image directory within the current directory
                    var fileName = Path.GetFileName(user.Image.FileName);
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "USERMANAGEMENT", "wwwroot", "images");
                    var filePath = Path.Combine(folderPath, fileName);

                    // Ensure the directory exists
                    Directory.CreateDirectory(folderPath);

                    // Save the file to disk
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await user.Image.CopyToAsync(stream);
                    }

                    // Set the new image path for the user
                    imagePath = $"/images/{fileName}";
                }

                // Validate and adjust DateTime values
                DateTime validMinDate = new DateTime(1753, 1, 1);
                DateTime validMaxDate = new DateTime(9999, 12, 31);

                DateTime dateOfJoining = user.DateOfJoining < validMinDate ? validMinDate :
                                          user.DateOfJoining > validMaxDate ? validMaxDate :
                                          user.DateOfJoining;

                DateTime dob = user.Dob < validMinDate ? validMinDate :
                               user.Dob > validMaxDate ? validMaxDate :
                               user.Dob;

                DateTime createdAt = DateTime.UtcNow < validMinDate ? validMinDate :
                                     DateTime.UtcNow > validMaxDate ? validMaxDate :
                                     DateTime.UtcNow;

                DateTime modifiedAt = DateTime.UtcNow < validMinDate ? validMinDate :
                                      DateTime.UtcNow > validMaxDate ? validMaxDate :
                                      DateTime.UtcNow;

                //Generate a random password
                randomPassword = GenerateRandomPassword(12);

                /*
                // Hash the password
                var passwordHasher = new PasswordHasher<AbhiUser>();
                string hashedPassword = passwordHasher.HashPassword(null, randomPassword);*/

                var hashedPassword = EncryptionShare.Encrypt(randomPassword, _config["Encryption:AbhiKey"], _config["Encryption:AbhiIv"]);

                // Map DTO to Entity
                var userDetailsEntity = new AbhiUser
                {
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    Gender = user.Gender,
                    DateOfJoining = dateOfJoining,
                    Dob = dob,
                    Email = user.Email,
                    Password = randomPassword,
                    Phone = user.Phone,
                    AlternatePhone = user.AlternatePhone,
                    ImagePath = imagePath,
                    IsActive = user.IsActive,
                    CreatedAt = createdAt,
                    ModifiedAt = modifiedAt,
                    IsDeleted = false,
                    AddresssAbhis = user.AddresssAbhi.Select(a => new AddresssAbhi
                    {
                        City = a.City,
                        State = a.State,
                        Country = a.Country,
                        ZipCode = a.ZipCode,
                        AId = Convert.ToInt32(a.AId)
                    }).ToList()
                };

                // Add to database
                var addedUser = await _context.AbhiUsers.AddAsync(userDetailsEntity); await _context.SaveChangesAsync();
                userId = addedUser.Entity.UserId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
            return (userId, randomPassword);
        }


        public async Task DeleteUser(int id)
        {
            var user = await _context.AbhiUsers.FindAsync(id);
            if (user != null)
            {
                user.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }
        }

        public async Task<(IEnumerable<AbhiUser> Users, int TotalRecords)> GetAllUser(int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.AbhiUsers
                            .Where(user => !user.IsDeleted)
                            .Include(user => user.AddresssAbhis);

                var totalRecords = await query.CountAsync();

                var users = await query.Skip((pageNumber - 1) * pageSize)
                                       .Take(pageSize)
                                       .ToListAsync();

                return (users, totalRecords);
            }
            catch (Exception ex)
            {
                // Handle the exception (logging, rethrowing, etc.)
                throw new ApplicationException("An error occurred while retrieving users.", ex);
            }
        }

        public async Task UpdateUser(int UserId, UserDetailsDTO updatedUser)
        {
            try
            {
                // Fetch the user from the database
                var user = await _context.AbhiUsers.FirstOrDefaultAsync(x => x.UserId == updatedUser.UserId);
                if (user == null)
                {
                    throw new KeyNotFoundException($"User with id {updatedUser.UserId} not found.");
                }


                /*var passwordHasher = new PasswordHasher<AbhiUser>();
                string hashedPassword = passwordHasher.HashPassword(null, randomPassword);*/

                // Update user properties
                user.FirstName = updatedUser.FirstName;
                user.MiddleName = updatedUser.MiddleName;
                user.LastName = updatedUser.LastName;
                user.Gender = updatedUser.Gender;
                user.DateOfJoining = updatedUser.DateOfJoining;
                user.Dob = updatedUser.Dob;
                user.Email = updatedUser.Email;
                user.Phone = updatedUser.Phone;
                user.AlternatePhone = updatedUser.AlternatePhone;
                user.ModifiedAt = DateTime.UtcNow;
                user.IsActive = updatedUser.IsActive;
                user.IsDeleted = updatedUser.IsDeleted;

                // Handle image update if the image is provided
                if (updatedUser.Image != null && updatedUser.Image.Length > 0)
                {
                    // Delete the previous image if it exists
                    if (!string.IsNullOrEmpty(user.ImagePath))
                    {
                        var previousImagePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "USERMANAGEMENT", "wwwroot", "images", user.ImagePath.TrimStart('/'));
                        if (File.Exists(previousImagePath))
                        {
                            File.Delete(previousImagePath);
                        }
                    }

                    var extension = Path.GetExtension(updatedUser.Image.FileName);
                    var filename = $"{user.UserId}{extension}";
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "USERMANAGEMENT", "wwwroot", "images");
                    var filePath = Path.Combine(folderPath, filename);

                    // Ensure the directory exists
                    Directory.CreateDirectory(folderPath);

                    // Save the file to disk
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await updatedUser.Image.CopyToAsync(stream);
                    }

                    // Set the new image path for the user
                    user.ImagePath = $"/images/{filename}";
                }

                _context.AbhiUsers.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception (logging, rethrowing, etc.)
                throw new ApplicationException("An error occurred while updating the user.", ex);
            }
        }



        // Filter Method 
        public async Task<IEnumerable<AbhiUser>> FilterUsers(bool filter)
        {

            try
            {
                if (filter == true)
                {
                    var activUsers = await _context.AbhiUsers
                        .Where(a => a.IsActive == true && !a.IsDeleted)
                        .Include(a => a.AddresssAbhis).ToListAsync();
                    return activUsers;
                }
                else
                {
                    var InActiveUsers = await _context.AbhiUsers
                        .Where(a => a.IsActive == false && a.IsDeleted != true)
                        .Include(a => a.AddresssAbhis).ToListAsync();
                    return InActiveUsers;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("an error occurred while filtering the user.", ex);

            }
        }

        // Sort Method
        public async Task<IEnumerable<AbhiUser>> SortUsers(string sortColumn)
        {

            var users = _context.AbhiUsers
                .Where(user => !user.IsDeleted)
                .Include(user => user.AddresssAbhis).AsQueryable();

            switch (sortColumn)
            {
                case "FirstName":
                    users = users.OrderBy(u => u.FirstName);
                    break;
                case "MiddleName":
                    users = users.OrderBy(u => u.MiddleName);
                    break;
                case "LastName":
                    users = users.OrderBy(u => u.LastName);
                    break;
                case "Dob":
                    users = users.OrderBy(u => u.Dob);
                    break;
                case "DateOfJoining":
                    users = users.OrderBy(u => u.DateOfJoining);
                    break;
                case "City":
                    users = users.OrderBy(u => u.DateOfJoining);
                    break;
                case "State":
                    users = users.OrderBy(u => u.DateOfJoining);
                    break;
                case "Country":
                    users = users.OrderBy(u => u.DateOfJoining);
                    break;
                // Add other cases for other columns
                default:
                    users = users.OrderBy(u => u.FirstName); // Default sorting
                    break;
            }

            return await users.ToListAsync();
        }

        public async Task ChangePassword(ChangePassword request)
        {
            try
            {
                if (request.Email != null)
                {
                    var user = await _context.AbhiUsers.FirstOrDefaultAsync(u => u.Email == request.Email);
                    if (user == null)
                    {
                        throw new KeyNotFoundException($"User with ID {request.Email} not found.");
                    }

                    var passwordHasher = new PasswordHasher<AbhiUser>();
                    user.Password = passwordHasher.HashPassword(user, request.NewPassword);
                    user.ModifiedAt = DateTime.UtcNow;

                    _context.AbhiUsers.Update(user);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new SecurityTokenException("Invalid token.");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while changing the password.", ex);
            }
        }

        //Generating Random Password
        private static readonly string ValidChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
        private string GenerateRandomPassword(int length)
        {
            if (length < 8)
            {
                throw new ArgumentException("Password length must be at least 8 characters.");
            }

            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*?_-";
            const string allChars = upperChars + lowerChars + digits + specialChars;

            var randomBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            var passwordChars = new List<char>();
            passwordChars.Add(upperChars[randomBytes[0] % upperChars.Length]); // Ensure at least one uppercase letter
            passwordChars.Add(lowerChars[randomBytes[1] % lowerChars.Length]); // Ensure at least one lowercase letter
            passwordChars.Add(digits[randomBytes[2] % digits.Length]); // Ensure at least one digit
            passwordChars.Add(specialChars[randomBytes[3] % specialChars.Length]); // Ensure at least one special character

            // Fill the remaining characters with random selections from all allowed characters
            for (int i = 4; i < length; i++)
            {
                passwordChars.Add(allChars[randomBytes[i] % allChars.Length]);
            }

            // Shuffle the password characters to ensure the required characters are not in predictable positions
            var shuffledPassword = passwordChars.OrderBy(x => randomBytes[passwordChars.IndexOf(x)]).ToArray();

            return new string(shuffledPassword);
        }

        // Excel file to download
        public async Task<IEnumerable<AbhiUser>> ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var users = await _context.AbhiUsers
                      .Include(u => u.AddresssAbhis)
                      .ToListAsync();
            return users;
            
        }
    }
}
