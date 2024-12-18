using System.ComponentModel.DataAnnotations;

namespace PropertyRentalManagement.Models.Metadata
{
    public class MessageMetadata
    {
        [Display(Name = "Message ID")]
        public int MessageId { get; set; }

        [Required(ErrorMessage = "Sender User ID is required.")]
        [Display(Name = "Sender User ID")]
        public int SenderUserId { get; set; }

        [Required(ErrorMessage = "Receiver User ID is required.")]
        [Display(Name = "Recevier User ID")] 
        public int ReceiverUserId { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [Display(Name = "Subject")]
        public string Subject { get; set; } = null!;

        [Required(ErrorMessage = "Message Body is required.")]
        [Display(Name = "Message Body")]
        public string MessageBody { get; set; } = null!;

        [Required(ErrorMessage = "Message Date Time is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Message Date Time must be in Date Time format")]
        [Display(Name = "Message Date Time")]
        public DateTime MessageDateTime { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [Display(Name = "Status")]
        public int StatusId { get; set; }

        [Display(Name = "Apartment")]
        public virtual Apartment Apartment { get; set; } = null!;

        [Display(Name = "Receiver User")]
        public virtual User ReceiverUser { get; set; } = null!;

        [Display(Name = "Sender User")]
        public virtual User SenderUser { get; set; } = null!;

        [Display(Name = "Status")]
        public virtual Status Status { get; set; } = null!;
    }
}
