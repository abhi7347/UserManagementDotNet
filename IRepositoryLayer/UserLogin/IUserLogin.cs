using DomainLayer.DTOs;

namespace IRepositoryLayer.UserLogin
{
    public interface IUserLogin 
    {
       public Task<bool> UserLogin(LoginDTO loginValue);
    }
}
