using System.ComponentModel.DataAnnotations;

namespace PropertyRentalManagement.Models.Metadata
{
    public class EventMetadata
    {
        [Display(Name = "Event ID")]
        public int EventId { get; set; }

        [Required(ErrorMessage = "Manage ID is required.")]
        [Display(Name = "Manager ID")]
        public int ManagerId { get; set; }

        [Required(ErrorMessage = "Apartment ID is required.")]
        [Display(Name = "Apartment ID")]
        public int ApartmentId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Event Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Event Date must in date format.")]
        [Display(Name = "Event Date")]
        public DateOnly EventDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [Display(Name = "Status")]
        public int StatusId { get; set; }
    }
}
