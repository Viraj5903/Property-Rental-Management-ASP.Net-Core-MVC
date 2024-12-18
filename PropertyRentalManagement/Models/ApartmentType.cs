using System;
using System.Collections.Generic;

namespace PropertyRentalManagement.Models;

public partial class ApartmentType
{
    public int ApartmentTypeId { get; set; }

    public string ApartmentTypeDescription { get; set; } = null!;

    public virtual ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
}
