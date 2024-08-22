using DomainLayer.Models;
using Org.BouncyCastle.Asn1.Icao;
using System;
using System.Collections.Generic;

namespace USERMANAGEMENT.Models;

public partial class AddresssAbhi
{
    public int UserId { get; set; }

    public int AId { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string Country { get; set; }

    public int ZipCode { get; set; }

    public AbhiUser AbhiUser { get; set; }

    public AddresssMasterAbhi AddresssMasterAbhi { get; set; }
}
