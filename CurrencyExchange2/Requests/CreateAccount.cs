using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange2.Requests
{
    public class CreateAccount
    {
        [Required(ErrorMessage = "Account name is required")]
        public string AccountName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string? UserEmail { get; set; }
    }
}
