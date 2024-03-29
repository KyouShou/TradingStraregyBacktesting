﻿using System.Collections.Generic;
using TradingStraregyBacktesting;
using NetTrader.Indicator;
using TradingStraregyBacktesting.Models;
using TradingStraregyBacktesting.Stragety;

internal class Program
{
    private static void Main(string[] args)
    {
        //路徑與檔名可以寫進config中，使本程式符合開放封閉原則
        string filePath = "tempfile/";
        string fileName = "bnbusdt1hCandlestickCsv.csv";

        List<Ohlc> candlestickList = Candlestick.Load(filePath + fileName);

        //策略可以寫進config並用工廠生產，使本程式符合開放封閉原則
        IExchanges exchanges = new Exchanges();
        ITradingStrategy strategy = new SampleStrategy(exchanges);
        new Market().Run(candlestickList, strategy, exchanges.GetClosePositionHandler());
        TradingResultModel result = strategy.GetTradingResult();
        Console.ReadLine();
    }
}