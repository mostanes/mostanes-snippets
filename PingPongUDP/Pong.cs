using System;
using System.Net;
using System.Net.Sockets;

namespace PingPongUDP
{
	public class Pong
	{
		Socket ponger;

		public Pong()
		{
			ponger = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		}

		public void GoPong()
		{
			ponger.Bind(new IPEndPoint(IPAddress.Any, 9500));
			PongSocket(ponger);
		}

		void PongSocket(Socket s)
		{
			byte[] png = new byte[64];
			byte[] xng = new byte[64];
			byte rex = 0xff;
			IPEndPoint iep = new IPEndPoint(IPAddress.Any, 0);
			EndPoint rep = iep;
			while (true)
			{
				ponger.ReceiveFrom(png, 64, SocketFlags.None, ref rep);
				for (int i = 0; i < 64; i++) xng[i] = (byte)(rex ^ png[i]);
				ponger.SendTo(xng, rep);
			}
		}
	}
}
