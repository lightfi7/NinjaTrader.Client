using NinjaTrader.Server;
using System;
using System.Runtime.InteropServices;

namespace NinjaTrader.Client
{
	// Token: 0x02000006 RID: 6
	[Guid("41A63BA8-2028-4ef1-9693-6E2707A18E0F")]
	[ComVisible(true)]
	public interface IClient
	{
		// Token: 0x0600002F RID: 47
		int Ask(string instrument, double price, int size);

		// Token: 0x06000030 RID: 48
		int AskPlayback(string instrument, double price, int size, string timestamp);

		// Token: 0x06000031 RID: 49
		double AvgEntryPrice(string instrument, string account);

		// Token: 0x06000032 RID: 50
		double AvgFillPrice(string orderId);

		// Token: 0x06000033 RID: 51
		int Bid(string instrument, double price, int size);

		// Token: 0x06000034 RID: 52
		int BidPlayback(string instrument, double price, int size, string timestamp);

		// Token: 0x06000035 RID: 53
		double BuyingPower(string account);

		// Token: 0x06000036 RID: 54
		double CashValue(string account);

		// Token: 0x06000037 RID: 55
		int Command(string command, string account, string instrument, string action, int quantity, string orderType, double limitPrice, double stopPrice, string timeInForce, string oco, string orderId, string tpl, string strategy);

		// Token: 0x06000038 RID: 56
		int ConfirmOrders(int confirm);

		// Token: 0x06000039 RID: 57
		int Connected(int showMessage);

		// Token: 0x0600003A RID: 58
		int Filled(string orderId);

		// Token: 0x0600003B RID: 59
		double GetDouble(string key);

		// Token: 0x0600003C RID: 60
		int GetInt(string key);

		// Token: 0x0600003D RID: 61
		string GetString(string key);

		// Token: 0x0600003E RID: 62
		int Last(string instrument, double price, int size);

		// Token: 0x0600003F RID: 63
		int LastPlayback(string instrument, double price, int size, string timestamp);

		// Token: 0x06000040 RID: 64
		double MarketData(string instrument, int type);

		// Token: 0x06000041 RID: 65
		int MarketPosition(string instrument, string account);

		// Token: 0x06000042 RID: 66
		string NewOrderId();

		// Token: 0x06000043 RID: 67
		string Orders(string account);

		// Token: 0x06000044 RID: 68
		string OrderStatus(string orderId);

		// Token: 0x06000045 RID: 69
		string QueryInstrument();

		// Token: 0x06000046 RID: 70
		double RealizedPnL(string account);

		// Token: 0x06000047 RID: 71
		int SetAllocReturnString(int value);

		// Token: 0x06000048 RID: 72
		int SetMaxReturnStringLength(int value);

		// Token: 0x06000049 RID: 73
		int SetUp(string host, int hort);

		// Token: 0x0600004A RID: 74
		string StopOrders(string strategyId);

		// Token: 0x0600004B RID: 75
		int SubscribeMarketData(string instrument);

		// Token: 0x0600004C RID: 76
		string Strategies(string account);

		// Token: 0x0600004D RID: 77
		int StrategyPosition(string strategyId);

		// Token: 0x0600004E RID: 78
		string TargetOrders(string strategyId);

		// Token: 0x0600004F RID: 79
		int TearDown();

		// Token: 0x06000050 RID: 80
		int UnsubscribeMarketData(string instrument);

		int SubscribeMarketDepth(string instrument);

        int UnsubscribeMarketDepth(string instrument);

    }
}
