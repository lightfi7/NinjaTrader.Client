using System;

namespace NinjaTrader.Cbi
{
	// Token: 0x02000004 RID: 4
	[Flags]
	public enum TraceLevels
	{
		// Token: 0x04000010 RID: 16
		All = 268435455,
		// Token: 0x04000011 RID: 17
		None = 0,
		// Token: 0x04000012 RID: 18
		Bars = 1,
		// Token: 0x04000013 RID: 19
		Connect = 4,
		// Token: 0x04000014 RID: 20
		Database = 8,
		// Token: 0x04000015 RID: 21
		Gui = 16,
		// Token: 0x04000016 RID: 22
		Indicator = 32,
		// Token: 0x04000017 RID: 23
		ResolveInstrument = 64,
		// Token: 0x04000018 RID: 24
		MarketData = 128,
		// Token: 0x04000019 RID: 25
		MarketDepth = 256,
		// Token: 0x0400001A RID: 26
		Native = 512,
		// Token: 0x0400001B RID: 27
		News = 1024,
		// Token: 0x0400001C RID: 28
		Order = 2048,
		// Token: 0x0400001D RID: 29
		Strategy = 4096,
		// Token: 0x0400001E RID: 30
		Strict = 8192,
		// Token: 0x0400001F RID: 31
		Test = 16384,
		// Token: 0x04000020 RID: 32
		Timer = 32768,
		// Token: 0x04000021 RID: 33
		Server = 65536,
		// Token: 0x04000022 RID: 34
		Alerts = 131072,
		// Token: 0x04000023 RID: 35
		FundamentalData = 262144
	}
}
