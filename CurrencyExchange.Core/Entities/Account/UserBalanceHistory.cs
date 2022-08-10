using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyExchange.Core.Entities.Account
{
    [Table("TransactionHistory")]
    public class UserBalanceHistory : BaseEntity
    {

        public Account Account { get; set; }
        public int AccountId { get; set; }
        [Column("Description")]
        public string MessageForChanging { get; set; }

        public string BoughtCryptoCoin { get; set; }

        public string SoldCryptoCoin { get; set; }

        [Column("ChangedAmountBoughtCryptoCoin")]

        public double ChangedAmount { get; set; }

        public double ChangedAmountSoldCryptoCoin { get; set; }


    }
}
