using System.ComponentModel;

namespace CurrencyExchange.Core.Entities
{
    public abstract class BaseEntity 
    {
        public int Id { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;


    }
}
