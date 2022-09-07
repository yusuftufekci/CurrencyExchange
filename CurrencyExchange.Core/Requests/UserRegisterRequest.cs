using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.Core.Requests
{
    public class UserRegisterRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string UserEmail { get; set; } 
        public string Password { get; set; }


    }
}
