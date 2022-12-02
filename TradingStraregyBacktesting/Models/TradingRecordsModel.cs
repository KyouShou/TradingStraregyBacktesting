using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingStraregyBacktesting.Models
{
    public class TradingRecordsModel
    {
        //多單或空單
        public string LongShortType { get; set; }
        //成交價格(商品)
        public decimal DealPrice { get; set; }
        //成交金額(錢包裡增減多少錢)
        public decimal DealMoney { get; set; }
        //手續費(交易價差盈虧以外的一切費用)
        public decimal Fee { get; set; }
        //開倉或平倉
        public string OpenClosePositionType { get; set; }
        //已實現損益(扣除手續費後)
        public decimal TotalIncome { get; set; }
        //成交時間
        public DateTime DealTime { get; set; }

    }
}
