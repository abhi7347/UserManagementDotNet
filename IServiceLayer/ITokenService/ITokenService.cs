using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServiceLayer.ITokenService
{
    public interface ITokenService
    {
        string GenerateToken(string email);

    }
}
