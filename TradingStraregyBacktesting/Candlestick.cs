using NetTrader.Indicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingStraregyBacktesting
{
    public class Candlestick
    {
        public static List<Ohlc> Load(string filePathAndfileName)
        {
            List<Ohlc> resultList = new List<Ohlc>();

            using (var reader = new StreamReader(filePathAndfileName))
            {
                while (!reader.EndOfStream)
                {
                    Ohlc ohlc = new Ohlc();
                    var dataRow = reader.ReadLine();
                    var dataRowArray = dataRow.Split(',');

                    ohlc.Date = DateTime.ParseExact(dataRowArray[5], "yyyyMMddHHmmss", null);
                    ohlc.Open = double.Parse(dataRowArray[0]);
                    ohlc.High = double.Parse(dataRowArray[2]);
                    ohlc.Low = double.Parse(dataRowArray[3]);
                    ohlc.Close = double.Parse(dataRowArray[1]);
                    ohlc.Volume = double.Parse(dataRowArray[4]);
                    ohlc.AdjClose = double.Parse(dataRowArray[1]);

                    resultList.Add(ohlc);
                }
            }

            return resultList;
        }
    }
}
