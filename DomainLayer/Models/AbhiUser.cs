using System;
using System.Collections.Generic;

namespace USERMANAGEMENT.Models;

public partial class AbhiUser
{
    public int UserId { get; set; }

    public string? FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public string? Gender { get; set; }

    public DateTime DateOfJoining { get; set; }

    public DateTime Dob { get; set; }

    public string? Email { get; set; } = null!;

    public string? Password { get; set; }

    public string? Phone { get; set; } = null!;

    public string? AlternatePhone { get; set; }

    public string? ImagePath { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }

    public ICollection<AddresssAbhi> AddresssAbhis { get; set; }
}
