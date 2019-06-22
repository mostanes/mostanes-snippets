using System;
using System.IO;
using System.Threading;
using System.Timers;

namespace CoinPipeline
{
    class MainClass
    {
		public static DateTime UnixEpoch = new DateTime(1970, 1, 1);

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
			//BitfinexData.Retrieve();
			//BinanceData.Retrieve();
			System.Timers.Timer tr = new System.Timers.Timer(5000);
			tr.Elapsed += (object sender, ElapsedEventArgs e) =>
			{
				try
				{
					BitfinexCurrentOrders.Retrieve();
					BinanceCurrentOrders.Retrieve();
				}
				catch
				{
					File.AppendAllText("Timings", "Error at time: " + DateTime.Now.ToString() + "\n");
				}
			};
			tr.Start();
			Thread.Sleep(int.MaxValue);
        }
    }
}
