namespace PropertyRentalManagement.Models
{
    public partial class VMLogin
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool KeepLoggedIn { get; set; }
    }
}
