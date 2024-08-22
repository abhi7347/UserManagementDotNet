using DomainLayer.DTOs.UserManageDTO;
using Microsoft.AspNetCore.Http;
using USERMANAGEMENT.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainLayer.DTOs;
using System.Web.Mvc;

namespace IServiceLayer.IManageUserService
{
    public interface IMangeUserService
    {
        Task<(int UserId, string Password)> AddUser(UserDetailsDTO user);
        Task DeleteUser(int id);
        Task<(IEnumerable<AbhiUser> Users, int TotalRecords)> GetAllUser(int pageNumber, int pageSize);
        Task UpdateUser(int UserId, UserDetailsDTO user);
        Task<IEnumerable<AbhiUser>> FilterUsers(bool filter);
        Task<IEnumerable<AbhiUser>> SortUsers(string sortColumn);
        public Task ChangePassword(ChangePassword request);
        public Task<IEnumerable<AbhiUser>> ExportToExcel();

    }
}
