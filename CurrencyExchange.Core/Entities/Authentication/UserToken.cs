namespace CurrencyExchange.Core.Entities.Authentication
{
    public class UserToken : BaseEntity
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public string? Token { get; set; }

        public DateTime ExpDate { get; set; }
    }
}
