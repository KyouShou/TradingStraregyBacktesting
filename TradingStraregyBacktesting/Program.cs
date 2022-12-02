using System.Collections.Generic;
using TradingStraregyBacktesting;
using NetTrader.Indicator;
using TradingStraregyBacktesting.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        //路徑與檔名可以寫進config中，使本程式符合開放封閉原則
        string filePath = "tempfile/";
        string fileName = "BtcusdtFifteenMinutesCandlestickCsv.csv";

        List<Ohlc> candlestickList = Candlestick.Load(filePath + fileName);

        //策略可以寫進config並用工廠生產，使本程式符合開放封閉原則
        ITradingStrategy strategy = new JingStrategy();
        new Market().Run(candlestickList, strategy);
        TradingResultModel result = strategy.GetTradingResult();
        Console.ReadLine();

    }
}