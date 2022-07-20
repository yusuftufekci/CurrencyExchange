using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange2.Model.Authentication
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "User Name is required")]
        public string? UserEmail { get; set; }

        public string IpAdress { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}
