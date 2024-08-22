using DomainLayer.DTOs;
using IRepositoryLayer.UserLogin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USERMANAGEMENT.Models;

namespace RepositoryLayer.UserLogin
{
    public class UserLoginImp:IUserLogin
    {
        private readonly SDirectContext _context;
        private readonly IConfiguration _config;
        public UserLoginImp(SDirectContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<bool> UserLogin(LoginDTO loginValue)
        {
            var user = await _context.AbhiUsers.SingleOrDefaultAsync(u => u.Email == loginValue.Email && u.Password == loginValue.Password);

            return user != null;

            /*// Decrypt the stored password
            var decryptedPassword = EncryptionShare.Encrypt(loginValue.Password, _config["Encryption:AbhiKey"], _config["Encryption:AbhiIv"]);

            // Compare the decrypted password with the provided password
            if (decryptedPassword != null && decryptedPassword == user.Password)
            {
                return true; // Login successful
            }

            return false; // Login failed*/
        }

    }
}
