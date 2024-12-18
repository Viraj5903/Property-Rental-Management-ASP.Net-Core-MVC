using System.ComponentModel.DataAnnotations;

namespace PropertyRentalManagement.Models.Metadata
{
    public class AppointmentMetadata
    {
        [Display(Name = "Appointment ID")]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Tenant ID is required.")]
        [Display(Name = "Tenant ID")]
        public int TenantId { get; set; }

        [Required(ErrorMessage = "Manager ID is required.")]
        [Display(Name = "Manager ID")]
        public int ManagerId { get; set; }

        [Required(ErrorMessage = "Apartment ID is required.")]
        [Display(Name = "Apartment ID")]
        public int ApartmentId { get; set; }

        [Required(ErrorMessage = "Appointment Date Time is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Appointment Date Time must be in date time format.")]
        [Display(Name = "Appointment Date Time")]
        public DateTime AppointmentDateTime { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(100, ErrorMessage = "Description cannot be longer than 100 characters.")]
        [Display(Name = "Description")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Status is required.")]
        [Display(Name = "Status")]
        public int StatusId { get; set; }
    }
}
