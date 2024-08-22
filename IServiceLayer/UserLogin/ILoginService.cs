using DomainLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServiceLayer.UserLogin
{
    public interface ILoginService
    {
        public Task<bool> LoginUser(LoginDTO loginValue);
    }
}
