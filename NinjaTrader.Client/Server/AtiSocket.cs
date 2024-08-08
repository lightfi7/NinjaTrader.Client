using System;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NinjaTrader.Data;

namespace NinjaTrader.Server
{
	// Token: 0x02000008 RID: 8
	public sealed class AtiSocket : IDisposable
	{
		// Token: 0x06000055 RID: 85 RVA: 0x00002D50 File Offset: 0x00001D50
		public AtiSocket(Socket socket, Action<string> commandHandler, Action<int> confirmOrdersHandler, Action<string, MarketDataType, double, int, DateTime> dataHandler, Action<string, bool> subscribeHandler, Action<string, string> valueHandler)
		{
			this.commandHandler = commandHandler;
			this.confirmOrdersHandler = confirmOrdersHandler;
			this.dataHandler = dataHandler;
			this.socket = socket;
			this.subscribeHandler = subscribeHandler;
			Thread thread = new Thread(new ThreadStart(this.Loop))
			{
				Name = "NT AtiSocket"
			};
			this.valueHandler = valueHandler;
			thread.Start();
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002DD0 File Offset: 0x00001DD0
		public void Dispose()
		{
			lock (this)
			{
				if (this.socket != null && this.socket.Connected)
				{
					try
					{
						this.socket.Shutdown(SocketShutdown.Both);
					}
					catch (Exception)
					{
					}
					this.socket.Close();
				}
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00002E48 File Offset: 0x00001E48
		public bool IsConnected
		{
			get
			{
				Socket socket = this.socket;
				return socket != null && socket.Connected;
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00002E5C File Offset: 0x00001E5C
		private void Loop()
		{
			try
			{
				for (;;)
				{
					switch (this.ReadInteger())
					{
					case 0:
					{
						string obj = this.ReadString();
						Action<string> action = this.commandHandler;
						if (action != null)
						{
							action(obj);
						}
						break;
					}
					case 1:
					{
						MarketDataType arg = (MarketDataType)this.ReadInteger();
						string arg2 = this.ReadString();
						double arg3 = this.ReadDouble();
						int arg4 = this.ReadInteger();
						DateTime arg5 = new DateTime(1800, 1, 1);
						string text = this.ReadString();
						if (text.Length > 0)
						{
							try
							{
								arg5 = new DateTime(Convert.ToInt32(text.Substring(0, 4), CultureInfo.InvariantCulture), Convert.ToInt32(text.Substring(4, 2), CultureInfo.InvariantCulture), Convert.ToInt32(text.Substring(6, 2), CultureInfo.InvariantCulture), Convert.ToInt32(text.Substring(8, 2), CultureInfo.InvariantCulture), Convert.ToInt32(text.Substring(10, 2), CultureInfo.InvariantCulture), Convert.ToInt32(text.Substring(12, 2), CultureInfo.InvariantCulture));
							}
							catch
							{
							}
						}
						Action<string, MarketDataType, double, int, DateTime> action2 = this.dataHandler;
						if (action2 != null)
						{
							action2(arg2, arg, arg3, arg4, arg5);
						}
						break;
					}
					case 2:
					{
						string arg6 = this.ReadString();
						string arg7 = this.ReadString();
						Action<string, string> action3 = this.valueHandler;
						if (action3 != null)
						{
							action3(arg6, arg7);
						}
						break;
					}
					case 3:
					{
						int obj2 = this.ReadInteger();
						Action<int> action4 = this.confirmOrdersHandler;
						if (action4 != null)
						{
							action4(obj2);
						}
						break;
					}
					case 4:
					{
						string arg8 = this.ReadString();
						int num = this.ReadInteger();
						Action<string, bool> action5 = this.subscribeHandler;
						if (action5 != null)
						{
							action5(arg8, num != 0);
						}
						break;
					}
					default:
						Console.WriteLine(this.ReadString());
						break;
					}
				}
			}
			catch
			{
				this.Dispose();
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003040 File Offset: 0x00002040
		public double ReadDouble()
		{
			return Convert.ToDouble(this.ReadString(), CultureInfo.InvariantCulture);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003052 File Offset: 0x00002052
		public int ReadInteger()
		{
			return Convert.ToInt32(this.ReadString(), CultureInfo.InvariantCulture);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003064 File Offset: 0x00002064
		public string ReadString()
		{
			string text = string.Empty;
			for (;;)
			{
				if (this.nextByte2Read >= this.bytesInBuffer)
				{
					try
					{
						this.bytesInBuffer = this.socket.Receive(this.readBuffer, this.readBuffer.Length, SocketFlags.None);
					}
					catch
					{
						this.Dispose();
						throw;
					}
					if (this.bytesInBuffer == 0)
					{
						break;
					}
					this.nextByte2Read = 0;
				}
				int num = 0;
				bool flag = false;
				while (this.nextByte2Read + num < this.bytesInBuffer)
				{
					if (this.readBuffer[this.nextByte2Read + num] == 0)
					{
						flag = true;
						break;
					}
					num++;
				}
				text += this.asciiEncoding.GetString(this.readBuffer, this.nextByte2Read, num);
				this.nextByte2Read += num + 1;
				if (flag)
				{
					return text;
				}
			}
			this.Dispose();
			throw new ArgumentException("bytesInBuffer");
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00003148 File Offset: 0x00002148
		public void Send(double val)
		{
			this.Send(val.ToString(CultureInfo.InvariantCulture));
		}

		// Token: 0x0600005D RID: 93 RVA: 0x0000315C File Offset: 0x0000215C
		public void Send(int val)
		{
			this.Send(val.ToString(CultureInfo.InvariantCulture));
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00003170 File Offset: 0x00002170
		public void Send(string buf)
		{
			if (this.socket == null || !this.socket.Connected)
			{
				return;
			}
			lock (this)
			{
				byte[] array = new byte[buf.Length + 2];
				int bytes = this.asciiEncoding.GetBytes(buf, 0, buf.Length, array, 0);
				array[bytes] = 0;
				try
				{
					this.socket.Send(array, bytes + 1, SocketFlags.None);
				}
				catch
				{
					this.Dispose();
				}
			}
		}

		// Token: 0x0400002D RID: 45
		private readonly ASCIIEncoding asciiEncoding = new ASCIIEncoding();

		// Token: 0x0400002E RID: 46
		private int bytesInBuffer;

		// Token: 0x0400002F RID: 47
		private readonly Action<string> commandHandler;

		// Token: 0x04000030 RID: 48
		private readonly Action<int> confirmOrdersHandler;

		// Token: 0x04000031 RID: 49
		private readonly Action<string, MarketDataType, double, int, DateTime> dataHandler;

		// Token: 0x04000032 RID: 50
		private int nextByte2Read;

		// Token: 0x04000033 RID: 51
		private readonly byte[] readBuffer = new byte[1024];

		// Token: 0x04000034 RID: 52
		private readonly Socket socket;

		// Token: 0x04000035 RID: 53
		private readonly Action<string, bool> subscribeHandler;

		// Token: 0x04000036 RID: 54
		private readonly Action<string, string> valueHandler;

		// Token: 0x04000037 RID: 55
		public const string DefaultHost = "127.0.0.1";

		// Token: 0x04000038 RID: 56
		public const int DefaultPort = 36973;

		// Token: 0x02000009 RID: 9
		public enum Message
		{
			// Token: 0x0400003A RID: 58
			Command,
			// Token: 0x0400003B RID: 59
			Data,
			// Token: 0x0400003C RID: 60
			Value,
			// Token: 0x0400003D RID: 61
			ConfirmOrders,
			// Token: 0x0400003E RID: 62
			Subscribe
		}
	}
}
