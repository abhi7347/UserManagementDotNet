using DomainLayer.DTOs;
using IRepositoryLayer.UserLogin;
using IServiceLayer.UserLogin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class LoginService: ILoginService
    {
        private readonly IUserLogin _IUserLogin;
        public LoginService(IUserLogin IUserLogin)
        {
            _IUserLogin = IUserLogin;
        }
        public async Task<bool> LoginUser(LoginDTO loginValue)
        {
            return await _IUserLogin.UserLogin(loginValue);
        }
    }
}
