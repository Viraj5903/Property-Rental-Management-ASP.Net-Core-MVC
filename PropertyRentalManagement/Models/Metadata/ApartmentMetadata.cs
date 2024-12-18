using System.ComponentModel.DataAnnotations;

namespace PropertyRentalManagement.Models.Metadata
{
    public class ApartmentMetadata
    {
        [Key]
        [Display(Name = "Apartment ID")]
        public int ApartmentId { get; set; }

        [Required(ErrorMessage = "Apartment Code is required.")]
        [Display(Name = "Apartment Code")]
        public string ApartmentCode { get; set; } = null!;

        [Required(ErrorMessage = "Building Code is required.")]
        [Display(Name = "Building Code")]
        public string BuildingCode { get; set; } = null!;

        [Required(ErrorMessage = "Apartment Type is required.")]
        [Display(Name = "Apartment Type")]
        public int ApartmentTypeId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(100, ErrorMessage = "Description must be 100 characters.")]
        [Display(Name = "Description")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Rent is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Rent must be in money format.")]
        [Display(Name = "Rent")]
        public decimal Rent { get; set; }

        [Display(Name = "Status")]
        public int StatusId { get; set; }

        [Display(Name = "Apartment Image")]
        public byte[]? ApartmentImage { get; set; }

        [Display(Name = "Apartment Type")]
        public virtual ApartmentType ApartmentType { get; set; } = null!;
    }
}
