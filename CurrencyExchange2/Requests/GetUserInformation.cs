using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange2.Requests
{
    public class GetUserInformation
    {
        [EmailAddress]

        public string UserEmail { get; set; }

    }
}
