using CurrencyExchange.Core.CommonFunction;
using CurrencyExchange.Core.Entities.Log;
using CurrencyExchange.Core.Entities.LogMessages;
using CurrencyExchange.Core.RabbitMqLogger;

namespace CurrencyExchange.Service.LogFacade
{
    public class LogResponseFacade
    {
        private readonly ICommonFunctions _commonFunctions;
        private readonly ISenderLogger _logSender;
        private  LogMessages _logMessages;
        private  ResponseMessages _responseMessages;

        public LogResponseFacade(ISenderLogger logSender, ICommonFunctions commonFunctions)
        {
            _logSender = logSender;
            _commonFunctions = commonFunctions;
        }

        public async Task<ResponseMessages> GetLogAndResponseMessage(string logMessageKey, string responseMessageKey, string language){
            _logMessages = await _commonFunctions.GetLogResponseMessage(logMessageKey, language: language);
            _responseMessages = await _commonFunctions.GetApiResponseMessage(responseMessageKey, language: language);
            _logSender.SenderFunction("Log", _logMessages.Value);
            return _responseMessages;
        }
    }



}
