namespace CurrencyExchange.Core.Constants
{
    public static class TransactionHistoryMessages
    {
        public static string DepositMessage { get; set; } = " deposit into the account";
        public static string RollBackMessage { get; set; } = "Rollback successful for transaction id number ";
        public static string SellCoinMessage { get; set; } = " sold. It's equal to";

        public static string DepositCoinMessage(double coinAmount, string coinName)
        {
            return coinAmount + " " + coinName + DepositMessage;
        }

        public static string TransactionRollBackMessage(int transactionId)
        {
            return RollBackMessage + transactionId;
        }
        public static string SoldCoinMessage(double coinAmount, string coinName, double totalUsdt)
        {
            return coinAmount + " " + coinName + SellCoinMessage + totalUsdt + " USDT";
        }
    }
}
