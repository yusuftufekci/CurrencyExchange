using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange2.Model.Authentication
{
    public class PasswordInfo
    {
        [Key]
        public string UserEmail { get; set; }

        public byte[] Password { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}
