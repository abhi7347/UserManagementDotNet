using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTOs.UserManageDTO
{
    public class UserDTO
    {
        public UserDetailsDTO UserDetails { get; set; }
        public IFormFile image { get; set; }
    }
}
