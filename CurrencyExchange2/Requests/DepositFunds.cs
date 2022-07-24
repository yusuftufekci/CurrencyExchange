using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange2.Requests
{
    public class DepositFunds
    {
        [EmailAddress]
        [Required]
        public string? UserEmail { get; set; }

        public double TotalBalance { get; set; } = 0;


    }
}
