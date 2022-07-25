using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange2.Requests
{
    public class DepositFunds
    {
        [EmailAddress]
        [Required(ErrorMessage ="Please enter the user email")]
        public string? UserEmail { get; set; }
        [Required(ErrorMessage = "Please enter the amount you want to fund")]
        [RegularExpression(@"^\d+(.\d{1,2})?$")]
        public double TotalBalance { get; set; } = 0;


    }
}
