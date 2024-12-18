using System;
using System.Collections.Generic;

namespace PropertyRentalManagement.Models;

public partial class Apartment
{
    public int ApartmentId { get; set; }

    public string ApartmentCode { get; set; } = null!;

    public string BuildingCode { get; set; } = null!;

    public int ApartmentTypeId { get; set; }

    public string Description { get; set; } = null!;

    public decimal Rent { get; set; }

    /// <summary>
    /// Available, Rented, Unavailable
    /// </summary>
    public int StatusId { get; set; }

    public byte[]? ApartmentImage { get; set; }

    public virtual ApartmentType ApartmentType { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Building Building { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual Status Status { get; set; } = null!;
}
