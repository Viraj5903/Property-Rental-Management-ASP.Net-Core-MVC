using System;
using System.Collections.Generic;

namespace PropertyRentalManagement.Models;

public partial class Building
{
    public string BuildingCode { get; set; } = null!;

    public int OwnerId { get; set; }

    public int? ManagerId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Province { get; set; } = null!;

    public string ZipCode { get; set; } = null!;

    public virtual ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();

    public virtual User? Manager { get; set; }

    public virtual User Owner { get; set; } = null!;
}
