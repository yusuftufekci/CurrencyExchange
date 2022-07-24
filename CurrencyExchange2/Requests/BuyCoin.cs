using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange2.Requests
{
    public class BuyCoin
    {
        public string UserEmail { get; set; }
        [Required(ErrorMessage = "Which coin do you want to buy with which coin?")]
        public string CoinToBuy { get; set; }

        public string BuyWİthThisCoin { get; set; }

        public double Amount { get; set; }

    }
}
