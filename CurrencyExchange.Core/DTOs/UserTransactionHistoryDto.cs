namespace CurrencyExchange.Core.DTOs
{
    public class UserTransactionHistoryDto
    {
        public string AccountName { get; set; }

        public string MessageForChanging { get; set; }

        public string BoughtCryptoCoin { get; set; }

        public string SoldCryptoCoin { get; set; }

        public double ChangedAmount { get; set; }
    }
}