using NetTrader.Indicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace TradingStraregyBacktesting
{
    internal class Market
    {
        public void Run(List<Ohlc> ohlcList, ITradingStrategy strategy)
        {
            //先以日期進行排序，DateTime越大，表示日期越靠近現在，index越小(以吻合TradingView的邏輯)
            var orderByOhlcDescList = OrderByOhlcDescList(ohlcList);

            //這裡的forloop的i值從50開始，數到Count <50為止，因為要前後都要預留K棒，才能進行技術分析            
            for (int i = 50; i < orderByOhlcDescList.Count - 50; i++)
            {
                strategy.ExecuteStrategy(orderByOhlcDescList, i);
            }
        }

        private List<Ohlc> OrderByOhlcDescList(List<Ohlc> ohlcList)
        {
            return ohlcList.OrderByDescending(e => e.Date).ToList();
        }
    }
}
