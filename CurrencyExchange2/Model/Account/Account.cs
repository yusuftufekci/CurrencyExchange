using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange2.Model.Account
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public int UserId { get; set; }
        public ICollection<Balance> Balances { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}
