using CurrencyExchange2.Model.Account;

namespace CurrencyExchange2.Responses
{
    public class UserInformation : Response
    {
        public string UserEmail { get; set; }
        public string UserAccountName { get; set; }
        public  List<UserBalancesInfo> UserBalances { get; set; }
    }
}
