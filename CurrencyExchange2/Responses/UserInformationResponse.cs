using CurrencyExchange2.Model.Account;

namespace CurrencyExchange2.Responses
{
    public class UserInformationResponse : Response
    {
        public string UserEmail { get; set; }
        public string UserAccountName { get; set; }
        public  List<UserBalances> UserBalances { get; set; }
    }
}
