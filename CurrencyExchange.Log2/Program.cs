﻿namespace CurrencyExchange.Log2
{
    class Program
    {
        private static Consumer _consumer;
        static void Main(string[] args)
        {
            _consumer = new Consumer("Log");
        }
    }
}