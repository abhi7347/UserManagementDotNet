using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USERMANAGEMENT.Models;

namespace DomainLayer.Models
{
    public class AddresssMasterAbhi
    {
        public int AId { get; set; }
        public string AType { get; set; }

        // Navigation properties
        public ICollection<AddresssAbhi> AddresssAbhis { get; set; }
    }
}
