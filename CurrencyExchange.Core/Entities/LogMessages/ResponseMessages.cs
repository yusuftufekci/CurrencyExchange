namespace CurrencyExchange.Core.Entities.Log
{
    public class ResponseMessages : BaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Language { get; set; } = "tr";
    }
}
