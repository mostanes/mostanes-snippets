using System;
using System.Collections.Generic;
using System.Net;

namespace CoinPipeline
{
    public static class BitfinexData
    {
		const string Candles0 = "https://api.bitfinex.com/v2/candles/trade";
		const string Candles1 = Candles0 + ":1m:tBTCUSD" + "/hist";
		static DateTime UnixEpoch = new DateTime(1970, 1, 1);
		static TimeSpan PrevTimes = new TimeSpan(20, 0, 0, 0);
		static Dictionary<string, List<decimal>> Data = new Dictionary<string, List<decimal>> { { "MSTS", new List<decimal>() }, {"OPEN", new List<decimal>()},
			{"CLOSE", new List<decimal>()}, {"HIGH", new List<decimal>()}, {"LOW", new List<decimal>()}, {"VOLUME", new List<decimal>()}};
		
		public static void RetrieveOriginal()
		{
			WebClient wclient = new WebClient();
			TimeSpan tspNow = DateTime.UtcNow - UnixEpoch;
			TimeSpan tspPrev = tspNow - PrevTimes;
			long NowMS = (long)tspNow.TotalMilliseconds;
			int ax = 3600_000;
			long ms = (long)tspPrev.TotalMilliseconds;
			for (; ms < NowMS; ms += ax)
			{
				wclient.Headers.Add("start", ms.ToString());
				wclient.Headers.Add("end", (ms + ax).ToString());
				string w = wclient.DownloadString(Candles1);
				object o = Newtonsoft.Json.JsonConvert.DeserializeObject(w);
				IEnumerable<object> ienum = (IEnumerable<object>)o;
				int ct=0;
				foreach(object obj in ienum)
				{
					ct++;
					IEnumerator<object> iobb = ((IEnumerable<object>)obj).GetEnumerator();
					UpdateData(iobb);
					;
				}
				System.Threading.Thread.Sleep(10);
			}
			;
		}

		public static void Retrieve()
		{
			WebClient wclient = new WebClient();
			TimeSpan tspNow = DateTime.UtcNow - UnixEpoch;
			TimeSpan tspPrev = tspNow - PrevTimes;
			long NowMS = (long)tspNow.TotalMilliseconds;
			int ax = 3600_000;
			wclient.Headers.Add("start", (NowMS - ax).ToString());
			wclient.Headers.Add("end", NowMS.ToString());
			string w = wclient.DownloadString(Candles1);
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
