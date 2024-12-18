using System;
using System.Collections.Generic;

namespace PropertyRentalManagement.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int TenantId { get; set; }

    public int ManagerId { get; set; }

    public int ApartmentId { get; set; }

    public DateTime AppointmentDateTime { get; set; }

    public string Description { get; set; } = null!;

    /// <summary>
    /// Pending, Confirmed, Canceled
    /// </summary>
    public int StatusId { get; set; }

    public virtual Apartment Apartment { get; set; } = null!;

    public virtual User Manager { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;

    public virtual User Tenant { get; set; } = null!;
}
