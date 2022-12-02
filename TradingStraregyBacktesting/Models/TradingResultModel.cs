using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingStraregyBacktesting.Models
{
    public class TradingResultModel
    {
        //交易紀錄
        public List<TradingRecordsModel> TradingHistoryList { get; set; }
        //本金的最大回撤
        public decimal MoneyRetracementMaximum { get; set; }
        //高點的最大回撤
        public decimal MoneyDiffernceBetweenHighAndLow { get; set; }
        //單次最大損失
        public decimal MoneyOnceLossMaximum { get; set; }
        //勝率
        public string WinRate { get; set; }
        //最大連勝次數
        public int WinningStreakMaximum { get; set; }
        //最大連敗次數
        public int LosingStreakMaximum { get; set; }
        //交易次數，開倉並平倉算一次交易
        public int TransactionTimes { get; set; }
        //平均一天有幾次交易機會
        public decimal HowManyChanceToTradeInOneDay { get; set; }
        //手續費總額
        public decimal TotalFee { get; set; }
        //淨額或淨損(扣除手續費後)
        public decimal TotalIncome { get; set; }

    }
}
