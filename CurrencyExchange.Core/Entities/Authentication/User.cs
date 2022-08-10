namespace CurrencyExchange.Core.Entities.Authentication
{
    public class User : BaseEntity
    {
        public string? UserEmail { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string IpAddress { get; set; }

    }
}
