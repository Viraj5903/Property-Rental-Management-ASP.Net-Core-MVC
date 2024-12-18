using System;
using System.Collections.Generic;

namespace PropertyRentalManagement.Models;

/// <summary>
/// Owner, Manager, Tenant
/// </summary>
public partial class UserRole
{
    public int RoleId { get; set; }

    /// <summary>
    /// Owner, Manager, Tenant
    /// </summary>
    public string Role { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
