using System.ComponentModel.DataAnnotations;

namespace PropertyRentalManagement.Models.Metadata
{
    public class ApartmentTypeMetadata
    {
        [Display(Name = "Apartment Type ID")]
        public int ApartmentTypeId { get; set; }

        [Required(ErrorMessage = "Apartment Type is required.")]
        [StringLength(50)]
        [Display(Name = "Apartment Type")]
        public string ApartmentTypeDescription { get; set; } = null!;
    }
}
