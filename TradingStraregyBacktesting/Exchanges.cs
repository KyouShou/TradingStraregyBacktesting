using NetTrader.Indicator;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingStraregyBacktesting.Models;

namespace TradingStraregyBacktesting
{
    public class Exchanges
    {
        private decimal MoneyInPurse;
        private List<TradingRecordsModel> tradingRecordsList;
        private int lever;

        public Exchanges()
        {
            tradingRecordsList = new List<TradingRecordsModel>();
            MoneyInPurse = 10000;
            lever = 1;
        }

        public void SetLever(int lever)
        {
            this.lever = lever;
        }

        public bool PositionExist()
        {
            var openPositionCount = tradingRecordsList.FindAll(x => x.OpenClosePositionType == "open").ToList().Count;
            var closePositionCount = tradingRecordsList.FindAll(x => x.OpenClosePositionType == "close").ToList().Count;
            if (openPositionCount > closePositionCount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //預設以當前K棒開盤價開倉
        public void OpenPosition(List<Ohlc> ohlcList, int nowListIndex, string LongOrShort)
        {
            TradingRecordsModel tradingRecordsModel = new TradingRecordsModel();
            var fee = GetFee(MoneyInPurse);

            //以現在所處的K棒的開盤價作為建倉點位
            tradingRecordsModel.LongShortType = LongOrShort;
            tradingRecordsModel.DealPrice = (decimal)ohlcList[nowListIndex].Open;
            //控制進場點位可能可以大幅提高勝率和利潤
            //tradingRecordsModel.DealPrice = LongOrShort == "long" ? (decimal)ohlcList[nowListIndex].Open * 0.9992m : (decimal)ohlcList[nowListIndex].Open * 1.0008m;
            tradingRecordsModel.DealMoney = MoneyInPurse - fee;
            tradingRecordsModel.Fee = fee;
            tradingRecordsModel.OpenClosePositionType = "open";
            tradingRecordsModel.TotalIncome = -fee;
            tradingRecordsModel.DealTime = ohlcList[nowListIndex].Date;

            MoneyInPurse = 0;
            tradingRecordsList.Add(tradingRecordsModel);
        }

        public void OpenPosition(List<Ohlc> ohlcList, int nowListIndex, string LongOrShort , decimal openPositionPrice)
        {
            TradingRecordsModel tradingRecordsModel = new TradingRecordsModel();
            var fee = GetFee(MoneyInPurse);

            //以現在所處的K棒的開盤價作為建倉點位
            tradingRecordsModel.LongShortType = LongOrShort;
            tradingRecordsModel.DealPrice = openPositionPrice;
            tradingRecordsModel.DealMoney = MoneyInPurse - fee;
            tradingRecordsModel.Fee = fee;
            tradingRecordsModel.OpenClosePositionType = "open";
            tradingRecordsModel.TotalIncome = -fee;
            tradingRecordsModel.DealTime = ohlcList[nowListIndex].Date;

            MoneyInPurse = 0;
            tradingRecordsList.Add(tradingRecordsModel);
        }

        public void ClosePosition(List<Ohlc> ohlcList, int nowListIndex, decimal closePositionDealPrice)
        {
            TradingRecordsModel tradingRecordsModel = new TradingRecordsModel();

            var spreadIncome = GetSpreadIncome(tradingRecordsList[tradingRecordsList.Count - 1].DealPrice, closePositionDealPrice, tradingRecordsList[tradingRecordsList.Count - 1].DealMoney, tradingRecordsList[tradingRecordsList.Count - 1].LongShortType);
            var fee = GetFee(tradingRecordsList[tradingRecordsList.Count - 1].DealMoney + spreadIncome);

            //以現在所處的K棒的開盤價作為建倉點位
            tradingRecordsModel.LongShortType = IsNowHasLongPositioin() ? "short" : "long";
            tradingRecordsModel.DealPrice = closePositionDealPrice;
            tradingRecordsModel.DealMoney = tradingRecordsList[tradingRecordsList.Count - 1].DealMoney + spreadIncome - fee;
            tradingRecordsModel.Fee = fee;
            tradingRecordsModel.OpenClosePositionType = "close";
            tradingRecordsModel.TotalIncome = spreadIncome - fee;
            tradingRecordsModel.DealTime = ohlcList[nowListIndex].Date;

            MoneyInPurse = tradingRecordsModel.DealMoney;
            tradingRecordsList.Add(tradingRecordsModel);
        }

        public TradingResultModel GetTradingResult()
        {
            var result = new TradingResultModel();
            result.TradingHistoryList = tradingRecordsList;
            result.LowestMoneyInPurse = GetLowestMoneyInPurse();
            result.MoneyDiffernceBetweenHighAndLow = GetHighestMoneyInTradingRecords() - GetLowestMoneyInPurse();
            result.MoneyOnceLossMaximum = GetHighestLoss();
            result.WinRate = GetWinRate();
            result.TransactionTimes = GetClosePostionTimes();
            result.TotalFee = GetTatalFee();
            result.TotalIncome = GetTatalIncome();
            result.WinningStreakMaximum = GetWinningStreakMaximum(); ;
            result.LosingStreakMaximum = GetLosingStreakMaximum();
            result.HowManyChanceToTradeInOneDay = GetAvgDealTimesInOneDay();
            result.MoneyInPurse = GetMoneyInPurseAtLast();

            return result;
        }

        private decimal GetMoneyInPurseAtLast()
        {
            return tradingRecordsList[tradingRecordsList.Count - 1].DealMoney;
        }

        private int GetWinningStreakMaximum()
        {
            var maximunList = new List<int>();
            var closePosisionRecords = tradingRecordsList.FindAll(e => e.OpenClosePositionType == "close").ToList();
            for (int i = 0; i < closePosisionRecords.Count; i++)
            {
                var streakTimes = 0;
                while (closePosisionRecords[i].TotalIncome > 0)
                {
                    streakTimes++;
                    if (i == closePosisionRecords.Count - 1)
                    {
                        break;
                    }
                    else
                    {
                        i++;
                    }
                }
                maximunList.Add(streakTimes);
            }
            return maximunList.OrderByDescending(e => e).ToList()[0];
        }

        private int GetLosingStreakMaximum()
        {
            var maximunList = new List<int>();
            var closePosisionRecords = tradingRecordsList.FindAll(e => e.OpenClosePositionType == "close").ToList();
            for (int i = 0; i < closePosisionRecords.Count; i++)
            {
                var streakTimes = 0;
                while (closePosisionRecords[i].TotalIncome <= 0)
                {
                    streakTimes++;
                    if (i == closePosisionRecords.Count - 1)
                    {
                        break;
                    }
                    else
                    {
                        i++;
                    }
                }
                maximunList.Add(streakTimes);
            }
            return maximunList.OrderByDescending(e => e).ToList()[0];
        }

        //計算第一筆成交以及最後一筆成交的天數差距，並把交易次數除以天數
        private decimal GetAvgDealTimesInOneDay()
        {
            var totalDealTimes = GetClosePostionTimes();
            var differenceBetweenFirstDealDateAndLastDealDate = (tradingRecordsList[tradingRecordsList.Count - 1].DealTime - tradingRecordsList[0].DealTime).Days + 1;

            return (decimal)totalDealTimes / differenceBetweenFirstDealDateAndLastDealDate;
        }


        private decimal GetTatalIncome()
        {
            var clonedTradingRecordsList = CloneTradingRecordsList(tradingRecordsList);
            decimal result = 0;
            foreach (var tradingRecord in clonedTradingRecordsList)
            {
                result += tradingRecord.TotalIncome;
            }
            return result;
        }

        private decimal GetTatalFee()
        {
            var clonedTradingRecordsList = CloneTradingRecordsList(tradingRecordsList);
            decimal result = 0;
            foreach (var tradingRecord in clonedTradingRecordsList)
            {
                result += tradingRecord.Fee;
            }
            return result;
        }

        private int GetClosePostionTimes()
        {
            var getCloseRecord = CloneTradingRecordsList(tradingRecordsList);
            var closePositionList = getCloseRecord.FindAll(e => e.OpenClosePositionType == "close");
            return closePositionList.Count;
        }

        private string GetWinRate()
        {
            var getCloseRecord = CloneTradingRecordsList(tradingRecordsList);
            var closePositionList = getCloseRecord.FindAll(e => e.OpenClosePositionType == "close");
            var winRecordsList = closePositionList.FindAll(e => e.TotalIncome >= 0);
            var lossRecordsList = closePositionList.FindAll(e => e.TotalIncome < 0);
            decimal winRate = 0;
            if (winRecordsList.Count + lossRecordsList.Count != 0)
            {
                winRate = ((decimal)winRecordsList.Count) / (winRecordsList.Count + lossRecordsList.Count);
            }
            else
            {
                winRate = 0;
            }

            return winRate.ToString("P");
        }

        private decimal GetHighestLoss()
        {
            var getCloseRecord = CloneTradingRecordsList(tradingRecordsList);
            var sorttedCloseRecord = getCloseRecord.OrderBy(e => e.TotalIncome).ToList();
            return sorttedCloseRecord[0].TotalIncome;
        }

        private decimal GetHighestMoneyInTradingRecords()
        {
            var getCloseRecord = CloneTradingRecordsList(tradingRecordsList);
            var sorttedCloseRecord = getCloseRecord.OrderByDescending(e => e.DealMoney).ToList();
            return sorttedCloseRecord[0].DealMoney;
        }

        private decimal GetLowestMoneyInPurse()
        {
            var getCloseRecord = CloneTradingRecordsList(tradingRecordsList);
            var sorttedCloseRecord = getCloseRecord.OrderBy(e => e.DealMoney).ToList();
            return sorttedCloseRecord[0].DealMoney;
        }

        private List<TradingRecordsModel> CloneTradingRecordsList(List<TradingRecordsModel> targetList)
        {
            var result = new List<TradingRecordsModel>();
            foreach (var target in targetList)
            {
                result.Add(target);
            }
            return result;
        }

        private bool IsNowHasLongPositioin()
        {
            if (tradingRecordsList.Count > 0 && PositionExist() & tradingRecordsList[tradingRecordsList.Count - 1].LongShortType == "long")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //計算價差所產生的獲利或虧損，舉例：10000元經交易後獲利1000元，變成11000元，扣除手續費500元後，總資產變成10500元，此方法return：1000元
        private decimal GetSpreadIncome(decimal openPositionPrice, decimal closePositionPrice, decimal dealMoneyWhenOpenPosition, string LongOrShortWhenOpenPosition)
        {
            if (LongOrShortWhenOpenPosition == "long")
            {
                var earnPercent = (closePositionPrice / openPositionPrice) - 1;

                return dealMoneyWhenOpenPosition * earnPercent * lever;

            }
            else if (LongOrShortWhenOpenPosition == "short")
            {
                var earnPercent = ((closePositionPrice / openPositionPrice) - 1) * -1;

                return dealMoneyWhenOpenPosition * earnPercent * lever ;
            }
            throw new Exception();
        }
        //輸入成交總價，計算手續費，預設為0.04%
        //由於在開倉的當下就會被收取手續費，因此手續費越高，Income就會越低，舉個例子，10000元的本金，0%開倉手續費，賺10%共賺1000元；50%開倉手續費，本金剩5000，賺10%只賺500元
        private decimal GetFee(decimal dealPrice)
        {
            var dealPriceDicimal = (decimal)dealPrice;
            return dealPriceDicimal * 0.0004m * lever;
        }

    }
}
