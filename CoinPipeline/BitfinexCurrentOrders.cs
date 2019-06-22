using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace CoinPipeline
{
	public static class BitfinexCurrentOrders
	{
		const string APIURI = "https://api.bitfinex.com/v1/book/BTCUSD";
		static WebClient wclient = new WebClient();

		public static void Retrieve()
		{
			Stopwatch swatch = new Stopwatch();
			swatch.Start();

			List<decimal> BidPrice = new List<decimal>();
			List<decimal> BidAmount = new List<decimal>();
			List<decimal> AskPrice = new List<decimal>();
			List<decimal> AskAmount = new List<decimal>();

			TimeSpan prequery = swatch.Elapsed;
			string w = wclient.DownloadString(APIURI);
			TimeSpan query = swatch.Elapsed;

			object o = Newtonsoft.Json.JsonConvert.DeserializeObject(w);
			IEnumerable<object> ienum = (IEnumerable<object>)o;
			IEnumerator<object> inr = ienum.GetEnumerator();
			inr.MoveNext();
			IEnumerator<object> iolo = ((IEnumerable<object>)inr.Current).GetEnumerator();
			iolo.MoveNext();
			IEnumerable<object> ioio = ((IEnumerable<object>)iolo.Current);
			int ct = 0;
			foreach (object obj in ioio)
			{
				IEnumerator<object> iojo = ((IEnumerable<object>)obj).GetEnumerator();
				iojo.MoveNext();
				Newtonsoft.Json.Linq.JToken jtk = (Newtonsoft.Json.Linq.JToken)iojo.Current;
				decimal dj = (decimal)jtk.ToObject(typeof(decimal));
				BidPrice.Add(dj);
				iojo.MoveNext();
				jtk = (Newtonsoft.Json.Linq.JToken)iojo.Current;
				dj = (decimal)jtk.ToObject(typeof(decimal));
				BidAmount.Add(dj);
			}
			inr.MoveNext();
			iolo = ((IEnumerable<object>)inr.Current).GetEnumerator();
			iolo.MoveNext();
			ioio = ((IEnumerable<object>)iolo.Current);
			foreach (object obj in ioio)
			{
				IEnumerator<object> iojo = ((IEnumerable<object>)obj).GetEnumerator();
				iojo.MoveNext();
				Newtonsoft.Json.Linq.JToken jtk = (Newtonsoft.Json.Linq.JToken)iojo.Current;
				decimal dj = (decimal)jtk.ToObject(typeof(decimal));
				AskPrice.Add(dj);
				iojo.MoveNext();
				jtk = (Newtonsoft.Json.Linq.JToken)iojo.Current;
				dj = (decimal)jtk.ToObject(typeof(decimal));
				AskAmount.Add(dj);
			}

			long time = (long)(DateTime.Now - MainClass.UnixEpoch).TotalSeconds;
			StreamWriter tw = new StreamWriter("Bitfinex_" + time.ToString() + ".csv");
			tw.WriteLine("Bid Price,Bid Amount,Ask Price,Ask Amount");
			for (int i = 0; i < 25; i++)
				tw.WriteLine(BidPrice[i] + "," + BidAmount[i] + "," + AskPrice[i] + "," + AskAmount[i]);
			tw.Close();
			tw.Dispose();
			TimeSpan postprocess = swatch.Elapsed;
			swatch.Stop();
			File.AppendAllText("Timings", "Entry: Bitfinex " + time.ToString() + "\nPrequery: " + prequery.TotalMilliseconds
				   + "ms\nQuery: " + (query - prequery).TotalMilliseconds + "ms\nPostprocessing: " + (postprocess - query).TotalMilliseconds + "ms\n");
			;
		}
	}
}
