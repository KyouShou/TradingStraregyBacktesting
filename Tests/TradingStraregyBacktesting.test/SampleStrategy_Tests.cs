using NetTrader.Indicator;
using TradingStraregyBacktesting.Models;
using TradingStraregyBacktesting.Stragety;

namespace TradingStraregyBacktesting.Tests
{
    public class SampleStragety_Tests
    {
        TradingResultModel result;

        [SetUp]
        public void Setup()
        {
            //路徑與檔名可以寫進config中，使本程式符合開放封閉原則
            string filePath = "tempfile/";
            string fileName = "bnbusdt1hCandlestickCsv.csv";

            List<Ohlc> candlestickList = Candlestick.Load(filePath + fileName);

            //策略的切換可以寫進config並用工廠生產，使本程式符合開放封閉原則
            IExchanges exchanges = new Exchanges();
            ITradingStrategy strategy = new SampleStrategy(exchanges);
            new Market().Run(candlestickList, strategy, exchanges.GetClosePositionHandler());
            TradingResultModel result = strategy.GetTradingResult();

            this.result = result;
        }

        [Test]
        public void Stragety_IsResultCorrect()
        {
            Assert.IsTrue(decimal.Parse(result.HowManyChanceToTradeInOneDay.ToString("f3")) == 1.056m);
            Assert.IsTrue(result.LosingStreakMaximum == 8);
            Assert.IsTrue((int)result.LowestMoneyInPurse == 9937);
            Assert.IsTrue((int)result.MoneyDiffernceBetweenHighAndLow == 27291);
            Assert.IsTrue((int)result.MoneyInPurse == 26525);
            Assert.IsTrue((int)result.MoneyOnceLossMaximum == -1299);
            Assert.IsTrue((int)result.TotalFee == 19793);
            Assert.IsTrue((int)result.TotalIncome == 16525);
            Assert.IsTrue(result.TradingHistoryList.Count == 2160);
            Assert.IsTrue(result.TransactionTimes == 1080);
            Assert.IsTrue(result.WinRate == "54.35%");
            Assert.IsTrue(result.WinningStreakMaximum == 14);
        }
    }
}