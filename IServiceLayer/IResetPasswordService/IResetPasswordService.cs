using DomainLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServiceLayer.IResetPasswordService
{
    public interface IResetPasswordService
    {
        Task<bool> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);
    }
}
