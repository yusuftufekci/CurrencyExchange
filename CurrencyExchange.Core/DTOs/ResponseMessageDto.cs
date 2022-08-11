using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.DTOs
{
    public class ResponseMessageDto
    {
        public string Key { get; set; }
        public string LogValue { get; set; }
        public string ApiValue { get; set; }
        public string Language { get; set; }
    }
}
