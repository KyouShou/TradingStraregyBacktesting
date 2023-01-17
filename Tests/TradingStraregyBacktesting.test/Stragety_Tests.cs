using NetTrader.Indicator;
using TradingStraregyBacktesting.Models;

namespace TradingStraregyBacktesting.Tests
{
    public class Stragety_Tests
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
            ITradingStrategy strategy = new OpenPositionWhenMACDHistogramLowerLowWithPriceDeviateAndAdjustableATR_OpenPositionWhenPriceMoreCheapStrategy();
            new Market().Run(candlestickList, strategy);
            TradingResultModel result = strategy.GetTradingResult();

            this.result = result;
        }

        [Test]
        public void Stragety_IsResultCorrect()
        {
            Assert.IsTrue(result.HowManyChanceToTradeInOneDay == 0.103m);
            Assert.IsTrue(result.LosingStreakMaximum == 8);
            Assert.IsTrue((int)result.LowestMoneyInPurse == 8519);
            Assert.IsTrue((int)result.MoneyDiffernceBetweenHighAndLow == 150434);
            Assert.IsTrue((int)result.MoneyInPurse == 110730);
            Assert.IsTrue((int)result.MoneyOnceLossMaximum == -33601);
            Assert.IsTrue((int)result.TotalFee == 25752);
            Assert.IsTrue((int)result.TotalIncome == 100730);
            Assert.IsTrue(result.TradingHistoryList.Count == 206);
            Assert.IsTrue(result.TransactionTimes == 103);
            Assert.IsTrue(result.WinRate == "43.69%");
            Assert.IsTrue(result.WinningStreakMaximum == 5);
        }
    }
}