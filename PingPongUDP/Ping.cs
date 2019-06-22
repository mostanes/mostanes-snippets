using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace PingPongUDP
{
	public class Ping
	{
		Socket pinger;
		public List<double> mmsec = new List<double>();
		IPEndPoint remote;

		public Ping(string address)
		{
			pinger = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			//pinger.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9500));
			remote = new IPEndPoint(IPAddress.Parse(address), 9500);
		}

		public void GoPing()
		{
			byte[] pingdata = new byte[64];
			Random random = new Random();
			random.NextBytes(pingdata);
			byte[] resp = new byte[64];
			IPEndPoint iep = remote;
			EndPoint rep = iep;
			Stopwatch sw = new Stopwatch();
			sw.Start();
			for (int c = 0; true; c++)
			{
				TimeSpan ts1 = sw.Elapsed;
				pinger.SendTo(pingdata, iep);
				pinger.ReceiveFrom(resp, 64, SocketFlags.None, ref rep);
				TimeSpan ts2 = sw.Elapsed;
				Console.WriteLine((ts2 - ts1).TotalMilliseconds);
				mmsec.Add((ts2 - ts1).TotalMilliseconds);
				for (int i = 0; i < 64; i++) if ((pingdata[i] ^ resp[i]) != 0xff) throw new Exception();
				pingdata[c] ^= pingdata[c + 1];
				if (c > 60) c = 0;
			}
		}
	}
}
