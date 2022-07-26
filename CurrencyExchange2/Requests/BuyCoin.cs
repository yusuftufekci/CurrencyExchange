using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange2.Requests
{
    public class BuyCoin
    {
        [Required(ErrorMessage = "User email is required")]
        [EmailAddress]
        public string UserEmail { get; set; }


        [Required(ErrorMessage = "Please enter the coin name you want to use to buy")]
        public string CoinToBuy { get; set; }

        [Required(ErrorMessage = "Please enter the coin name you want to buy")]
        public string BuyWİthThisCoin { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [RegularExpression(@"^\d+(.\d{1,2})?$")]
        public double Amount { get; set; }

    }
}
