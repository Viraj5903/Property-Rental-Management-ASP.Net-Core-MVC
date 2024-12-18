using System;
using System.Collections.Generic;

namespace PropertyRentalManagement.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Password { get; set; } = null!;

    /// <summary>
    /// Tenant, Owner, Manager
    /// </summary>
    public int RoleId { get; set; }

    public virtual ICollection<Appointment> AppointmentManagers { get; set; } = new List<Appointment>();

    public virtual ICollection<Appointment> AppointmentTenants { get; set; } = new List<Appointment>();

    public virtual ICollection<Building> BuildingManagers { get; set; } = new List<Building>();

    public virtual ICollection<Building> BuildingOwners { get; set; } = new List<Building>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Message> MessageReceiverUsers { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageSenderUsers { get; set; } = new List<Message>();

    public virtual UserRole Role { get; set; } = null!;
}
