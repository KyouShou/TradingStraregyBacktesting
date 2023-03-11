using Binance.Spot.Models;
using NetTrader.Indicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TradingStraregyBacktesting.Models;

namespace TradingStraregyBacktesting.Stragety
{
    public interface ITradingStrategy
    {
        public void ExecuteStrategy(List<Ohlc> ohlcList, int listIndex);
        public TradingResultModel GetTradingResult();
    }

    public class SampleStrategy : ITradingStrategy
    {
        private double takeProfitPrice;
        private double stopLossPrice;
        private IExchanges exchanges;
        private List<double?> atrList;
        private List<double?> macdHistogramList;
        private string symbol;

        public SampleStrategy(IExchanges exchanges)
        {
            this.exchanges = exchanges;
        }

        public void ExecuteStrategy(List<Ohlc> sortedOhlcList, int listIndex)
        {
            if (exchanges.PositionExist())
            {
                //Do nothing
            }
            else
            {
                if (IsTimeToLong(sortedOhlcList, listIndex))
                {
                    exchanges.OpenPosition(sortedOhlcList, listIndex, "long");
                }

                else if (IsTimeToShort(sortedOhlcList, listIndex))
                {
                    exchanges.OpenPosition(sortedOhlcList, listIndex, "short");
                }
            }
        }

        private bool IsTimeToLong(List<Ohlc> ohlcList, int nowListIndex)
        {
            //檢查
            //1.前兩根K棒是否為波峰
            //2.最近20根K棒是否存在第二個波峰
            //3.波峰是否為負
            //皆通過的情況表示必定開倉，因此在此設定停損/停利點
            if (IsNextToLastCandlestickPeak(ohlcList, nowListIndex) &&
                IsHigherPeakExist(ohlcList, nowListIndex) &&
                IsMacdNegativeNumber(ohlcList, nowListIndex))
            {
                UpdateATRList(ohlcList);

                double stopLossPrice = (double)(ohlcList[nowListIndex].Open - atrList[nowListIndex]);
                double takeProfitPrice = (double)(ohlcList[nowListIndex].Open + atrList[nowListIndex]);

                SetStopLoss(stopLossPrice, "short");
                SetTakeProfit(takeProfitPrice, "short");
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsTimeToShort(List<Ohlc> ohlcList, int nowListIndex)
        {
            //檢查
            //1.前兩根K棒是否為波峰
            //2.最近20根K棒是否存在第二個波峰
            //3.波峰是否為正
            //皆通過的情況表示必定開倉，因此在此設定停損/停利點
            if (IsNextToLastCandlestickPeak(ohlcList, nowListIndex) &&
                IsHigherPeakExist(ohlcList, nowListIndex) &&
                !IsMacdNegativeNumber(ohlcList, nowListIndex))
            {
                UpdateATRList(ohlcList);

                double stopLossPrice = (double)(ohlcList[nowListIndex].Open + atrList[nowListIndex]);
                double takeProfitPrice = (double)(ohlcList[nowListIndex].Open - atrList[nowListIndex]);

                SetStopLoss(stopLossPrice, "long");
                SetTakeProfit(takeProfitPrice, "long");
                return true;
            }
            else
            {
                return false;
            }
        }

        public TradingResultModel GetTradingResult()
        {
            //相關資料皆在交易所，而交易所只在這個類別被實體化，因此利用此方法傳遞資料，減少類別間的耦合程度
            return exchanges.GetTradingResult();
        }

        private bool IsTimeToTakeProfit(List<Ohlc> ohlcList, int nowListIndex)
        {
            //檢查停利點是否比停損點大，是的(if)話表示目前是Long，否則(else)是Short
            if (takeProfitPrice >= stopLossPrice)
            {
                if (ohlcList[nowListIndex].High >= takeProfitPrice)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (ohlcList[nowListIndex].Low <= takeProfitPrice)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void UpdateATRList(List<Ohlc> ohlcList)
        {
            if (atrList == null)
            {
                //使用NetTrader.Indicator計算技術分析指標時，ohlcList必須OrderByDate
                var ohlcListOrderByDateTime = ohlcList.OrderBy(e => e.Date).ToList();
                ATR atr = new ATR(13);
                atr.Load(ohlcListOrderByDateTime);
                ATRSerie serie = atr.Calculate();

                atrList = serie.ATR;
                atrList.Reverse();
            }
            else
            {
                return;
            }
        }

        private void UpdateMACDList(List<Ohlc> ohlcList)
        {
            if (macdHistogramList == null)
            {
                //使用NetTrader.Indicator計算技術分析指標時，ohlcList必須OrderByDate
                var ohlcListOrderByDateTime = ohlcList.OrderBy(e => e.Date).ToList();
                MACD macd = new MACD(13, 34, 9);
                macd.Load(ohlcListOrderByDateTime);
                MACDSerie serie = macd.Calculate();

                macdHistogramList = serie.MACDHistogram;
                macdHistogramList.Reverse();
            }
            else
            {
                return;
            }
        }

        private bool IsTimeToStopLoss(List<Ohlc> ohlcList, int nowListIndex)
        {
            //檢查停利點是否比停損點大，是的(if)話表示目前是Long，否則(else)是Short
            if (takeProfitPrice >= stopLossPrice)
            {
                if (ohlcList[nowListIndex].Low <= stopLossPrice)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (ohlcList[nowListIndex].High >= stopLossPrice)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void SetStopLoss(double stopLoss, string longOrShortWhenClosePosition)
        {
            var longOrShort = longOrShortWhenClosePosition == "long" ? Side.BUY : Side.SELL;
            exchanges.SetStopLoss(symbol, longOrShort, (decimal)stopLoss);
        }
        private void SetTakeProfit(double takeProfit, string longOrShortWhenClosePosition)
        {
            var longOrShort = longOrShortWhenClosePosition == "long" ? Side.BUY : Side.SELL;
            exchanges.SetTakeProfit(symbol, longOrShort, (decimal)takeProfit);
        }
        private void TakeProfit(List<Ohlc> ohlcList, int listIndex, Exchanges exchanges)
        {
            exchanges.ClosePosition(ohlcList, listIndex, (decimal)takeProfitPrice);
        }
        private void StopLoss(List<Ohlc> ohlcList, int listIndex, Exchanges exchanges)
        {
            exchanges.ClosePosition(ohlcList, listIndex, (decimal)stopLossPrice);
        }

        //檢查前兩根K棒是否為波峰
        private bool IsNextToLastCandlestickPeak(List<Ohlc> ohlcList, int nowListIndex)
        {
            UpdateMACDList(ohlcList);

            //檢查nowListIndex + 2是否為檢查nowListIndex到nowListIndex+4之間最大的值
            var isNextToLastCandlestickIndexMaximum = true;
            if (macdHistogramList[nowListIndex + 2] > 0)
            {
                for (int i = nowListIndex + 1; i < nowListIndex + 5; i++)
                {
                    if (macdHistogramList[i] > macdHistogramList[nowListIndex + 2])
                    {
                        isNextToLastCandlestickIndexMaximum = false;
                    }
                }
            }
            else
            {
                for (int i = nowListIndex + 1; i < nowListIndex + 5; i++)
                {
                    if (macdHistogramList[i] < macdHistogramList[nowListIndex + 2])
                    {
                        isNextToLastCandlestickIndexMaximum = false;
                    }
                }
            }


            //檢查nowListIndex + 3是否大於nowListIndex + 4
            var isFourthandFifthHistogramIncreasing = false;
            if (macdHistogramList[nowListIndex + 2] > 0)
            {
                if (macdHistogramList[nowListIndex + 3] > macdHistogramList[nowListIndex + 4])
                {
                    isFourthandFifthHistogramIncreasing = true;
                }
            }
            else
            {
                if (macdHistogramList[nowListIndex + 3] < macdHistogramList[nowListIndex + 4])
                {
                    isFourthandFifthHistogramIncreasing = true;
                }
            }

            if (isNextToLastCandlestickIndexMaximum && isFourthandFifthHistogramIncreasing)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsHigherPeakExist(List<Ohlc> ohlcList, int nowListIndex)
        {
            UpdateMACDList(ohlcList);

            var result = false;

            if (macdHistogramList[nowListIndex + 2] > 0)
            {
                for (int i = nowListIndex + 3; i < nowListIndex + 23; i++)
                {
                    if (macdHistogramList[nowListIndex + 2] < macdHistogramList[i])
                    {
                        result = true;
                    }
                }
            }
            else
            {
                for (int i = nowListIndex + 3; i < nowListIndex + 23; i++)
                {
                    if (macdHistogramList[nowListIndex + 2] > macdHistogramList[i])
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        //大於等於零時，return false
        private bool IsMacdNegativeNumber(List<Ohlc> ohlcList, int nowListIndex)
        {
            UpdateMACDList(ohlcList);

            var targetNumber = macdHistogramList[nowListIndex + 2];

            if (targetNumber >= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

}
