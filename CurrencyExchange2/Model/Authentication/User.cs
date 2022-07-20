using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange2.Model.Authentication
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "User Name is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public byte[] Password { get; set; }
        public byte[] PasswordSalt { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }
}
