using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTOs.UserManageDTO
{
    public class UserDetailsDTO
    {
        public int UserId { get; set; }

        public string? FirstName { get; set; }

        public string? MiddleName { get; set; } 

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public String? Password { get; set; }

        public String? Gender { get; set; }

        public DateTime DateOfJoining { get; set; }

        public DateTime Dob { get; set; }

        public string? Phone { get; set; }
        public string? AlternatePhone { get; set; }
        public IFormFile? Image { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
        public List<UserAddressDTO> AddresssAbhi { get; set; }
    }
}
