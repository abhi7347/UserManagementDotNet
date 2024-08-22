using DomainLayer.DTOs;
using DomainLayer.DTOs.UserManageDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using USERMANAGEMENT.Models;

namespace IRepositoryLayer.IMangeUserRepository
{
    public interface IManageUserRepository
    {
        public  Task<(IEnumerable<AbhiUser> Users, int TotalRecords)> GetAllUser(int pageNumber, int pageSize);
        public  Task<(int UserId, string Password)> AddUser(UserDetailsDTO user);
        public Task UpdateUser(int UserId, UserDetailsDTO user);
        public Task DeleteUser(int id);
        public Task<IEnumerable<AbhiUser>> FilterUsers(bool filter);
        public Task<IEnumerable<AbhiUser>> SortUsers(string sortColumn);
        public Task ChangePassword(ChangePassword request);

        public Task<IEnumerable<AbhiUser>> ExportToExcel();

    }
}
