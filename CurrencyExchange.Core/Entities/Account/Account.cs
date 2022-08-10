using CurrencyExchange.Core.Entities.Authentication;

namespace CurrencyExchange.Core.Entities.Account
{
    public class Account : BaseEntity
    {
        public string AccountName { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public ICollection<Balance> Balances { get; set; }

    }
}
