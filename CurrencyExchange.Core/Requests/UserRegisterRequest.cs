namespace CurrencyExchange.Core.Requests
{
    public class UserRegisterRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;


    }
}
