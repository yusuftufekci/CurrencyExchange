namespace CurrencyExchange2.Model.Account
{
    public class UserTransactionHistory
    {
        public string AccountName { get; set; }

        public string MessageForChanging { get; set; }

        public string ExchangedCoinName { get; set; }

        public double ChangedAmount { get; set; }
    }
}
