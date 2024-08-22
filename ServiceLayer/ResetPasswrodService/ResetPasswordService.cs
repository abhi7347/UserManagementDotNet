using DomainLayer.DTOs;
using IServiceLayer.IResetPasswordService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using USERMANAGEMENT.Models;

namespace ServiceLayer.ResetPasswrodService
{
    public class ResetPasswordService : IResetPasswordService
    {
        private readonly SDirectContext _context;

        public ResetPasswordService(SDirectContext context)
        {
            _context = context;
        }
        public async Task<bool> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            if (resetPasswordDTO.NewPassword != resetPasswordDTO.ConfirmPassword)
            {
                return false;
            }

            // Hash the password
            var passwordHasher = new PasswordHasher<AbhiUser>();
            string hashedPassword = passwordHasher.HashPassword(null, resetPasswordDTO.NewPassword);

            var user = await _context.AbhiUsers.FirstOrDefaultAsync(u => u.Email == resetPasswordDTO.Email);
            if (user != null)
            {
                user.Password = hashedPassword;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

    }
}
