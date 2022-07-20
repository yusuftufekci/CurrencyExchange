using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange2.Model.Authentication
{
    public class UserToken
    {
        public int UserId { get; set; }
        [Key]
        public int TokenId{ get; set; }

        public string? Token { get; set; }

        //public DateTime ExpDate { get; set; } = DateTime.Now.AddMonths(1);
    }
}
