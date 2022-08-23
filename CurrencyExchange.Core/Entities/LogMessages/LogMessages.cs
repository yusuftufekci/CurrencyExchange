namespace CurrencyExchange.Core.Entities.LogMessages
{
    public class LogMessages : BaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Language { get; set; } = "tr";
    }
}