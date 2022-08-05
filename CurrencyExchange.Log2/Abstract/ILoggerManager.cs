﻿namespace CurrencyExchange.Log2.Abstract
{
    public interface ILoggerManager
    {
        void LogInfo(string message);
        void LogWarn(string message );
        void LogDebug(string message );
        void LogError(string message );
    }
}
