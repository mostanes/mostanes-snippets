using System;
using System.Threading;

namespace PingPongUDP
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			Pong ponger = new Pong();
			Ping pinger;

			ThreadStart ts1 = new ThreadStart(ponger.GoPong);
			Thread th1 = new Thread(ts1), th2;
			th1.Start();
			System.Threading.Thread.Sleep(50);


			if (args.Length == 1)
			{
				pinger = new Ping(args[0]);
				ThreadStart ts2 = new ThreadStart(pinger.GoPing);
				th2 = new Thread(ts2);
				th2.Start();
			}
			else { pinger = null; th2 = null; }

			Console.ReadKey();
			double avg=0, zvar=0, stdev;
			th1.Abort();

			if (args.Length == 1)
			{
				th2.Abort();

				for (int i = 0; i < pinger.mmsec.Count; i++)
				{
					double loc = pinger.mmsec[i];
					avg += loc;
					zvar += loc * loc;
				}
				avg /= pinger.mmsec.Count;
				zvar /= pinger.mmsec.Count;
				stdev = Math.Sqrt(zvar - avg * avg);
				Console.WriteLine("Average time: " + avg + "; StDev: " + stdev);
			}
		}
	}
}
