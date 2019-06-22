using System;
using System.Collections.Generic;
using System.Net;

namespace CoinPipeline
{
	public static class BinanceData
	{
		const string Candles = "https://api.binance.com/api/v1/klines?symbol=BTCUSDT&interval=1m";
		static DateTime UnixEpoch = new DateTime(1970, 1, 1);
		static TimeSpan PrevTimes = new TimeSpan(20, 0, 0, 0);
		static Dictionary<string, List<decimal>> Data = new Dictionary<string, List<decimal>> { { "MSTS", new List<decimal>() }, {"OPEN", new List<decimal>()},
			{"CLOSE", new List<decimal>()}, {"HIGH", new List<decimal>()}, {"LOW", new List<decimal>()}, {"VOLUME", new List<decimal>()}};

		public static void Retrieve()
		{
			WebClient wclient = new WebClient();

			string w = wclient.DownloadString(Candles);
			object o = Newtonsoft.Json.JsonConvert.DeserializeObject(w);
			IEnumerable<object> ienum = (IEnumerable<object>)o;
			int ct = 0;
			foreach (object obj in ienum)
			{
				ct++;
				IEnumerator<object> iobb = ((IEnumerable<object>)obj).GetEnumerator();
				UpdateData(iobb);
				;
			}
			;
		}

		static void UpdateData(IEnumerator<object> Input)
		{
			string[] SF = { "MSTS", "OPEN", "CLOSE", "HIGH", "LOW", "VOLUME" };
			foreach (string s in SF)
			{
				Input.MoveNext();
				Newtonsoft.Json.Linq.JToken jtk = (Newtonsoft.Json.Linq.JToken)Input.Current;
				decimal dj = (decimal)jtk.ToObject(typeof(decimal));
				Data[s].Add(dj);
			}
		}
	}
}
