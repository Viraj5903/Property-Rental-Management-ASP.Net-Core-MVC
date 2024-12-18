using System.ComponentModel.DataAnnotations;

namespace PropertyRentalManagement.Models.Metadata
{
    public class BuildingMetadata
    {
        [Required(ErrorMessage = "Building Code is Required.")]
        [Display(Name = "Building Code")]
        public string BuildingCode { get; set; } = null!;

        [Required(ErrorMessage = "Owner ID is Required.")]
        [Display(Name = "Owner ID")]
        public int OwnerId { get; set; }

        [Required(ErrorMessage = "Manager ID is Required.")]
        [Display(Name = "Manager ID")]
        public int? ManagerId { get; set; }

        [Required(ErrorMessage = "Building Name is Required.")]
        [StringLength(100, ErrorMessage = "Building Name cannot be longer than 100 characters.")]
        [Display(Name = "Building Name")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Description is Required.")]
        [StringLength(100, ErrorMessage = "Description cannot be longer than 100 characters.")]
        [Display(Name = "Description")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Address is Required.")]
        [StringLength(100, ErrorMessage = "Address cannot be longer than 100 characters.")]
        [Display(Name = "Address")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "City is Required.")]
        [StringLength(100, ErrorMessage = "City cannot be longer than 100 characters.")]
        [Display(Name = "City")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Province is Required.")]
        [StringLength(100, ErrorMessage = "Province cannot be longer than 100 characters.")]
        [Display(Name = "Province")]
        public string Province { get; set; } = null!;

        [Required(ErrorMessage = "Zip Code is Required.")]
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z] \d[A-Za-z]\d$", ErrorMessage = "Zip code must be in A1A 1A1 format.")]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; } = null!;
    }
}
