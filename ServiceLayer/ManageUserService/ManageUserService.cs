using DomainLayer.DTOs;
using DomainLayer.DTOs.UserManageDTO;
using IRepositoryLayer.IMangeUserRepository;
using IServiceLayer.EmailSetting;
using IServiceLayer.IManageUserService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using USERMANAGEMENT.Models;

namespace ServiceLayer.ManageUserService
{
    public class ManageUserService : IMangeUserService
    {
        private readonly IManageUserRepository _irepository;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public ManageUserService(IManageUserRepository irepository, IConfiguration config, IEmailService emailService)
        {
            _irepository = irepository;
            _config = config;
            _emailService = emailService;
        }

        public async Task<(int UserId, string Password)> AddUser(UserDetailsDTO user)
        {
            var (userId, generatedPassword) = await _irepository.AddUser(user);

            if (userId <= 0)
            {
                throw new InvalidOperationException("User could not be created.");
            }

            string changePasswordLink = _config["Frontend:ChangePasswordLink"];
            if (string.IsNullOrEmpty(changePasswordLink))
            {
                throw new InvalidOperationException("Change password link configuration is missing.");
            }

            var subject = "Change Password";
            var email = user.Email;
            var htmlBody = "ChangePasswordBody.html";

            try
            {
                await _emailService.SendEmailAsync(email, subject, changePasswordLink, htmlBody, generatedPassword);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while sending the email: {ex.Message}");
                throw;
            }

            return (UserId: userId, Password: generatedPassword);
        }

        public async Task DeleteUser(int id)
        {
            await _irepository.DeleteUser(id);
        }

        public async Task<(IEnumerable<AbhiUser> Users, int TotalRecords)> GetAllUser(int pageNumber, int pageSize)
        {
            return await _irepository.GetAllUser( pageNumber,  pageSize);
        }

        public async Task UpdateUser(int UserId, UserDetailsDTO user)
        {
            await _irepository.UpdateUser(UserId, user);
        }

        public async Task<IEnumerable<AbhiUser>> FilterUsers(bool filter)
        {
            return await _irepository.FilterUsers(filter);
        }

        public async Task ChangePassword(ChangePassword request)
        {
            await _irepository.ChangePassword(request);

        }

        public async Task<IEnumerable<AbhiUser>> SortUsers(string sortColumn)
        {
            return await _irepository.SortUsers(sortColumn);
        }

        // Excel code here
        public Task<IEnumerable<AbhiUser>> ExportToExcel()
        {
            return  _irepository.ExportToExcel();
        }

    }
}
