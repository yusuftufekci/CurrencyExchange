using CurrencyExchange2.Model.Account;

namespace CurrencyExchange2.Responses
{
    public class UserBalanceInformationResponse  : Response
    {
        public List<UserBalances> UserBalances { get; set; }

    }
}
