using System;
using System.Collections.Generic;

namespace PropertyRentalManagement.Models;

public partial class Event
{
    public int EventId { get; set; }

    public int ManagerId { get; set; }

    public int ApartmentId { get; set; }

    public string Description { get; set; } = null!;

    public DateOnly EventDate { get; set; }

    /// <summary>
    /// New, Pending, Solved
    /// </summary>
    public int StatusId { get; set; }

    public virtual Apartment Apartment { get; set; } = null!;

    public virtual User Manager { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}
