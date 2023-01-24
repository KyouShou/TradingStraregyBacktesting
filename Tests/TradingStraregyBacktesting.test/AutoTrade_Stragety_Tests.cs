using NetTrader.Indicator;
using TradingStraregyBacktesting.Models;
using TradingStraregyBacktesting.Stragety;

namespace TradingStraregyBacktesting.Tests
{
    public class AutoTrade_Stragety_Tests
    {
        TradingResultModel result;

        [SetUp]
        public void Setup()
        {
            //���|�P�ɦW�i�H�g�iconfig���A�ϥ��{���ŦX�}��ʳ���h
            string filePath = "tempfile/";
            string fileName = "bnbusdt1hCandlestickCsv.csv";

            List<Ohlc> candlestickList = Candlestick.Load(filePath + fileName);

            //�����������i�H�g�iconfig�åΤu�t�Ͳ��A�ϥ��{���ŦX�}��ʳ���h
            IExchanges exchanges = new Exchanges();
            ITradingStrategy strategy = new AutoTrade_Banmusha_BNB_OneHour_Strategy(exchanges);
            new Market().Run(candlestickList, strategy, exchanges.GetClosePositionHandler());
            TradingResultModel result = strategy.GetTradingResult();

            this.result = result;
        }

        [Test]
        public void Stragety_IsResultCorrect()
        {
            Assert.IsTrue(result.HowManyChanceToTradeInOneDay == 0.105m);
            Assert.IsTrue(result.LosingStreakMaximum == 8);
            Assert.IsTrue((int)result.LowestMoneyInPurse == 8548);
            Assert.IsTrue((int)result.MoneyDiffernceBetweenHighAndLow == 288719);
            Assert.IsTrue((int)result.MoneyInPurse == 206819);
            Assert.IsTrue((int)result.MoneyOnceLossMaximum == -63046);
            Assert.IsTrue((int)result.TotalFee == 40360);
            Assert.IsTrue((int)result.TotalIncome == 196819);
            Assert.IsTrue(result.TradingHistoryList.Count == 210);
            Assert.IsTrue(result.TransactionTimes == 105);
            Assert.IsTrue(result.WinRate == "45.71%");
            Assert.IsTrue(result.WinningStreakMaximum == 5);
        }
    }
}