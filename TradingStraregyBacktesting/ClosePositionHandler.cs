using NetTrader.Indicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingStraregyBacktesting
{
    //現實世界的停損、停利，是掛單之後交易所的自動行為，不應該把此行為的處理寫在Stragety中，因此獨立處理
    public class ClosePositionHandler
    {
        private double takeProfitPrice;
        private double stopLossPrice;
        private IExchanges exchanges;

        public ClosePositionHandler(IExchanges exchanges)
        {
            this.exchanges = exchanges;
        }

        public void Execute(List<Ohlc> sortedOhlcList, int listIndex)
        {
            if (exchanges.PositionExist())
            {
                if (IsTimeToStopLoss(sortedOhlcList, listIndex))
                {
                    StopLoss(sortedOhlcList, listIndex);
                }
                else if (IsTimeToTakeProfit(sortedOhlcList, listIndex))
                {
                    TakeProfit(sortedOhlcList, listIndex);
                }
            }
        }

        public void SetTakeProfitPrice(double takeProfitPrice)
        {
            this.takeProfitPrice = takeProfitPrice;
        }

        public void SetStopLossPrice(double stopLossPrice)
        {
            this.stopLossPrice = stopLossPrice;
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

        private void TakeProfit(List<Ohlc> ohlcList, int listIndex)
        {
            exchanges.ClosePosition(ohlcList, listIndex, (decimal)takeProfitPrice);
        }
        private void StopLoss(List<Ohlc> ohlcList, int listIndex)
        {
            exchanges.ClosePosition(ohlcList, listIndex, (decimal)stopLossPrice);
        }
    }
}
