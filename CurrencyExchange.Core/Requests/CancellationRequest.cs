using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.Core.Requests
{
    public class CancellationRequest
    {
        public int TransactionHistoryId { get; set; }
    }
}
