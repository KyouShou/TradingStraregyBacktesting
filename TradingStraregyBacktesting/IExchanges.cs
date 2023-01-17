using NetTrader.Indicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingStraregyBacktesting.Models;

namespace TradingStraregyBacktesting
{
    public interface IExchanges
    {
        public void SetLever(int lever);
        public void SetLever(string symbol, int lever);

        public bool PositionExist();

        //預設以當前K棒開盤價開倉
        public void OpenPosition(List<Ohlc> ohlcList, int nowListIndex, string LongOrShort);

        public void OpenPosition(List<Ohlc> ohlcList, int nowListIndex, string LongOrShort, decimal openPositionPrice);

        public void OpenPosition(string symbol, List<Ohlc> ohlcList, int nowListIndex, string LongOrShort, decimal openPositionPrice);

        public void ClosePosition(List<Ohlc> ohlcList, int nowListIndex, decimal closePositionDealPrice);

        public void ClosePosition(string symbol, List<Ohlc> ohlcList, int nowListIndex, decimal closePositionDealPrice);
        public TradingResultModel GetTradingResult();

        public void CancelAllOrders(string symbol);

        public void SetStopLoss(string symbol, string longOrShort, decimal stopLoss);
        public void SetTakeProfit(string symbol, string longOrShort, decimal stopLoss);
        public ClosePositionHandler GetClosePositionHandler();
    }
}
