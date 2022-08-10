namespace CurrencyExchange.Core.Entities.Authentication
{
    public class Password : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
