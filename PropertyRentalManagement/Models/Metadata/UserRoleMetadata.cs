using System.ComponentModel.DataAnnotations;

namespace PropertyRentalManagement.Models.Metadata
{
    public class UserRoleMetadata
    {
        [Display(Name = "Role ID")]
        public int RoleId { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; } = null!;
    }
}
