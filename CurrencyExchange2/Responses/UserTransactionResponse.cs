using CurrencyExchange2.Model.Account;

namespace CurrencyExchange2.Responses
{
    public class UserTransactionResponse : Response
    {
        public List<UserTransactionHistory> UserTransactions { get; set; }

    }
}
