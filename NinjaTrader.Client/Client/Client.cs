using System;
using System.Collections;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using NinjaTrader.Data;
using NinjaTrader.Server;

namespace NinjaTrader.Client
{
	// Token: 0x02000005 RID: 5
	[Guid("093F5734-3610-4ab0-B9F9-C2CA4125AECB")]
	[ComVisible(true)]
	public sealed class Client : IClient, IDisposable
	{
		public delegate void CallbackHandler(Hashtable hashtable);

		public CallbackHandler callback;

		// Token: 0x06000007 RID: 7 RVA: 0x00002174 File Offset: 0x00001174
		private void AddValue(string key, string value)
		{

			Hashtable obj = this.values;
			lock (obj)
			{
				if (this.values.Contains(key))
				{
					this.values[key] = value;
				}
				else
				{
					this.values.Add(key, value);
				}
			}
    
            if (this.callback != null)
                this.callback(obj);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000021D8 File Offset: 0x000011D8
		public int Ask(string instrument, double price, int size)
		{
			if (this.SetUp(true) != 0)
			{
				return -1;
			}
			this.socket.Send(1);
			this.socket.Send(0);
			this.socket.Send(instrument);
			this.socket.Send(price);
			this.socket.Send(size);
			this.socket.Send(string.Empty);
			return 0;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002240 File Offset: 0x00001240
		public int AskPlayback(string instrument, double price, int size, string timestamp)
		{
			if (this.SetUp(true) != 0)
			{
				return -1;
			}
			this.socket.Send(1);
			this.socket.Send(0);
			this.socket.Send(instrument);
			this.socket.Send(price);
			this.socket.Send(size);
			this.socket.Send(timestamp);
			return 0;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000022A2 File Offset: 0x000012A2
		public double AvgEntryPrice(string instrument, string account)
		{
			if (this.SetUp(true) == 0)
			{
				return this.GetDouble("AvgEntryPrice|" + instrument + "|" + account);
			}
			return 0.0;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000022CE File Offset: 0x000012CE
		public double AvgFillPrice(string orderId)
		{
			if (this.SetUp(true) == 0)
			{
				return this.GetDouble("AvgFillPrice|" + orderId);
			}
			return 0.0;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000022F4 File Offset: 0x000012F4
		public int Bid(string instrument, double price, int size)
		{
			if (this.SetUp(true) != 0)
			{
				return -1;
			}
			this.socket.Send(1);
			this.socket.Send(1);
			this.socket.Send(instrument);
			this.socket.Send(price);
			this.socket.Send(size);
			this.socket.Send(string.Empty);
			return 0;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000235C File Offset: 0x0000135C
		public int BidPlayback(string instrument, double price, int size, string timestamp)
		{
			if (this.SetUp(true) != 0)
			{
				return -1;
			}
			this.socket.Send(1);
			this.socket.Send(1);
			this.socket.Send(instrument);
			this.socket.Send(price);
			this.socket.Send(size);
			this.socket.Send(timestamp);
			return 0;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000023BE File Offset: 0x000013BE
		public double BuyingPower(string account)
		{
			if (this.SetUp(true) == 0)
			{
				return this.GetDouble("BuyingPower|" + account);
			}
			return 0.0;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000023E4 File Offset: 0x000013E4
		public double CashValue(string account)
		{
			if (this.SetUp(true) == 0)
			{
				return this.GetDouble("CashValue|" + account);
			}
			return 0.0;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x0000240C File Offset: 0x0000140C
		public int Command(string command, string account, string instrument, string action, int quantity, string orderType, double limitPrice, double stopPrice, string timeInForce, string oco, string orderId, string tpl, string strategy)
		{
			if (this.SetUp(true) != 0)
			{
				return -1;
			}
			this.socket.Send(0);
			this.socket.Send(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12}", new object[]
			{
				command,
				account,
				instrument,
				action,
				quantity.ToString(CultureInfo.InvariantCulture),
				orderType,
				limitPrice.ToString(CultureInfo.InvariantCulture),
				stopPrice.ToString(CultureInfo.InvariantCulture),
				timeInForce,
				oco,
				orderId,
				tpl,
				strategy
			}));
			return 0;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000024AD File Offset: 0x000014AD
		public int ConfirmOrders(int confirm)
		{
			if (this.SetUp(true) != 0)
			{
				return -1;
			}
			this.socket.Send(3);
			this.socket.Send(confirm);
			return 0;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000024D4 File Offset: 0x000014D4
		public int Connected(int showMessage)
		{
			if (this.SetUp(showMessage == 1) == 0 && !this.showedError && this.socket != null && this.socket.IsConnected && !(this.GetString("ATI") != true.ToString(CultureInfo.InvariantCulture)))
			{
				return 0;
			}
			return -1;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x0000252D File Offset: 0x0000152D
		public void Dispose()
		{
			AtiSocket atiSocket = this.socket;
			if (atiSocket != null)
			{
				atiSocket.Dispose();
			}
			this.socket = null;
			System.Timers.Timer timer = this.timer;
			if (timer != null)
			{
				timer.Dispose();
			}
			this.timer = null;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000255F File Offset: 0x0000155F
		public int Filled(string orderId)
		{
			if (this.SetUp(true) == 0)
			{
				return this.GetInt("Filled|" + orderId);
			}
			return 0;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002580 File Offset: 0x00001580
		public double GetDouble(string key)
		{
			if (this.SetUp(true) != 0)
			{
				return 0.0;
			}
			double result = 0.0;
			string @string = this.GetString(key);
			if (@string.Length == 0)
			{
				return 0.0;
			}
			try
			{
				result = Convert.ToDouble(@string, CultureInfo.InvariantCulture);
			}
			catch
			{
			}
			return result;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000025E8 File Offset: 0x000015E8
		public int GetInt(string key)
		{
			if (this.SetUp(true) != 0)
			{
				return 0;
			}
			int result = 0;
			string @string = this.GetString(key);
			if (@string.Length == 0)
			{
				return 0;
			}
			try
			{
				result = Convert.ToInt32(@string, CultureInfo.InvariantCulture);
			}
			catch
			{
			}
			return result;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002638 File Offset: 0x00001638
		public string GetString(string key)
		{
			if (this.SetUp(true) != 0)
			{
				return string.Empty;
			}
			Hashtable obj = this.values;
			string result;
			lock (obj)
			{
				result = (this.values.Contains(key) ? ((string)this.values[key]) : string.Empty);
			}
			return result;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000026AC File Offset: 0x000016AC
		public int Last(string instrument, double price, int size)
		{
			if (this.SetUp(true) != 0)
			{
				return -1;
			}
			this.socket.Send(1);
			this.socket.Send(2);
			this.socket.Send(instrument);
			this.socket.Send(price);
			this.socket.Send(size);
			this.socket.Send(string.Empty);
			return 0;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002714 File Offset: 0x00001714
		public int LastPlayback(string instrument, double price, int size, string timestamp)
		{
			if (this.SetUp(true) != 0)
			{
				return -1;
			}
			this.socket.Send(1);
			this.socket.Send(2);
			this.socket.Send(instrument);
			this.socket.Send(price);
			this.socket.Send(size);
			this.socket.Send(timestamp);
			return 0;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002778 File Offset: 0x00001778
		public double MarketData(string instrument, int type)
		{
			if (this.SetUp(true) != 0)
			{
				return 0.0;
			}
			MarketDataType marketDataType = MarketDataType.Unknown;
			switch (type)
			{
			case 0:
				marketDataType = MarketDataType.Last;
				break;
			case 1:
				marketDataType = MarketDataType.Bid;
				break;
			case 2:
				marketDataType = MarketDataType.Ask;
				break;
			}
			return this.GetDouble(string.Format("MarketData|{0}|{1}", instrument, (int)marketDataType));
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000027D0 File Offset: 0x000017D0
		public int MarketPosition(string instrument, string account)
		{
			if (this.SetUp(true) == 0)
			{
				return this.GetInt("MarketPosition|" + instrument + "|" + account);
			}
			return 0;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000027F4 File Offset: 0x000017F4
		public string NewOrderId()
		{
			return Guid.NewGuid().ToString("N").ToUpperInvariant();
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002818 File Offset: 0x00001818
		private void OnTimerElapsed(object sender, ElapsedEventArgs e)
		{
			System.Timers.Timer timer = this.timer;
			if (timer != null)
			{
				timer.Dispose();
			}
			this.timer = null;
			this.SetUpNow(true);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x0000283A File Offset: 0x0000183A
		public string Orders(string account)
		{
			if (this.SetUp(true) == 0)
			{
				return this.GetString("Orders|" + account);
			}
			return string.Empty;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x0000285C File Offset: 0x0000185C
		public string OrderStatus(string orderId)
		{
			if (this.SetUp(true) == 0)
			{
				return this.GetString("OrderStatus|" + orderId);
			}
			return string.Empty;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002880 File Offset: 0x00001880
		public string QueryInstrument()
		{
			QueryInstrument queryInstrument = new QueryInstrument();
			bool? flag = queryInstrument.ShowDialog();
			bool flag2 = true;
			if (!(flag.GetValueOrDefault() == flag2 & flag != null))
			{
				return string.Empty;
			}
			return queryInstrument.instrument.Text;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000028C1 File Offset: 0x000018C1
		public double RealizedPnL(string account)
		{
			if (this.SetUp(true) == 0)
			{
				return this.GetDouble("RealizedPnL|" + account);
			}
			return 0.0;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000028E7 File Offset: 0x000018E7
		public int SetAllocReturnString(int value)
		{
			return value;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000028E7 File Offset: 0x000018E7
		public int SetMaxReturnStringLength(int value)
		{
			return value;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000028EC File Offset: 0x000018EC
		private int SetUp(bool showMessage)
		{
			int result;
			lock (this)
			{
				int num;
				if (!this.hadError)
				{
					AtiSocket atiSocket = this.socket;
					num = ((atiSocket != null && atiSocket.IsConnected) ? 0 : this.SetUpNow(showMessage));
				}
				else
				{
					num = -1;
				}
				result = num;
			}
			return result;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x0000294C File Offset: 0x0000194C
		private int SetUpNow(bool showMessage)
		{
			int result;
			lock (this)
			{
				try
				{
					IPAddress ipaddress;
					Socket socket;
					if (IPAddress.TryParse(this.host, out ipaddress))
					{
						socket = new Socket(ipaddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
						socket.Connect(ipaddress, this.port);
					}
					else
					{
						IPHostEntry hostEntry = Dns.GetHostEntry(this.host);
						socket = new Socket(hostEntry.AddressList[0].AddressFamily, SocketType.Stream, ProtocolType.Tcp);
						socket.Connect(hostEntry.AddressList[0], this.port);
					}
					this.socket = new AtiSocket(socket, null, null, null, null, new Action<string, string>(this.AddValue));
					for (int i = 0; i < 1000; i++)
					{
						Thread.Sleep(10);
						if (this.GetString("ATI").Length > 0)
						{
							break;
						}
					}
				}
				catch (Exception ex)
				{
					this.socket = null;
					this.hadError = true;
					if (!this.showedError)
					{
						this.showedError = true;
						if (showMessage)
						{
							MessageBox.Show(string.Format("Unable to connect to NinjaTrader ({0}/{1}). Please make sure NinjaTrader is running: {2}", this.host, this.port, ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
						}
					}
					this.timer = new System.Timers.Timer(10000.0)
					{
						AutoReset = false
					};
					this.timer.Elapsed += this.OnTimerElapsed;
					this.timer.Start();
					return -1;
				}
				this.hadError = false;
				this.showedError = false;
				result = 0;
			}
			return result;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002B08 File Offset: 0x00001B08
		public int SetUp(string pHost, int pPort)
		{
			this.host = pHost;
			this.port = pPort;
			return this.SetUp(true);
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002B1F File Offset: 0x00001B1F
		public string StopOrders(string strategyId)
		{
			if (this.SetUp(true) == 0)
			{
				return this.GetString("StopOrders|" + strategyId);
			}
			return string.Empty;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002B41 File Offset: 0x00001B41
		public string Strategies(string account)
		{
			if (this.SetUp(true) == 0)
			{
				return this.GetString("Strategies|" + account);
			}
			return string.Empty;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002B63 File Offset: 0x00001B63
		public int StrategyPosition(string strategyId)
		{
			if (this.SetUp(true) == 0)
			{
				return this.GetInt("StrategyPosition|" + strategyId);
			}
			return 0;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002B81 File Offset: 0x00001B81
		public int SubscribeMarketData(string instrument)
		{
			if (this.SetUp(true) != 0)
			{
				return -1;
			}
			this.socket.Send(4);
			this.socket.Send(instrument);
			this.socket.Send(1);
			return 0;
		}

        public int SubscribeMarketDepth(string instrument)
        {
            if (this.SetUp(true) != 0)
            {
                return -1;
            }
            this.socket.Send(5);
            this.socket.Send(instrument);
            this.socket.Send(1);
            return 0;
        }

        // Token: 0x0600002B RID: 43 RVA: 0x00002BB3 File Offset: 0x00001BB3
        public string TargetOrders(string strategyId)
		{
			if (this.SetUp(true) == 0)
			{
				return this.GetString("TargetOrders|" + strategyId);
			}
			return string.Empty;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002BD8 File Offset: 0x00001BD8
		public int TearDown()
		{
			lock (this)
			{
				AtiSocket atiSocket = this.socket;
				if (atiSocket != null)
				{
					atiSocket.Dispose();
				}
				this.socket = null;
			}
			return 0;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002C28 File Offset: 0x00001C28
		public int UnsubscribeMarketData(string instrument)
		{
			if (this.SetUp(true) != 0)
			{
				return -1;
			}
			this.socket.Send(4);
			this.socket.Send(instrument);
			this.socket.Send(0);
			return 0;
		}

        public int UnsubscribeMarketDepth(string instrument)
        {
            if (this.SetUp(true) != 0)
            {
                return -1;
            }
            this.socket.Send(4);
            this.socket.Send(instrument);
            this.socket.Send(0);
            return 0;
        }

        // Token: 0x04000024 RID: 36
        private bool hadError;

		// Token: 0x04000025 RID: 37
		private string host = "127.0.0.1";

		// Token: 0x04000026 RID: 38
		private int port = 36973;

		// Token: 0x04000027 RID: 39
		private bool showedError;

		// Token: 0x04000028 RID: 40
		public AtiSocket socket;

		// Token: 0x04000029 RID: 41
		private System.Timers.Timer timer;

		// Token: 0x0400002A RID: 42
		private readonly Hashtable values = new Hashtable();
	}
}
