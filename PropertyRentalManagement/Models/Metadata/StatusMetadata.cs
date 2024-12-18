using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PropertyRentalManagement.Models;

public class StatusMetadata
{
    [Display(Name = "Status ID")]
    public int StatusId { get; set; }

    [Display(Name = "Status")]
    public string Description { get; set; } = null!;

    [Display(Name = "Category")]
    public string Category { get; set; } = null!;

}
