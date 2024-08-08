using System;
using System.IO;
using System.Xml.Linq;

namespace NinjaTrader.Core
{
	// Token: 0x02000003 RID: 3
	public static class Globals
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00001050
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002134 File Offset: 0x00001134
		internal static XDocument ConfigFile
		{
			get
			{
				object[] obj = Globals.sync;
				lock (obj)
				{
					if (Globals.configFile != null)
					{
						return Globals.configFile;
					}
					Globals.configFile = new XDocument();
					if (File.Exists(Globals.UserDataDir + "\\Config.xml"))
					{
						StreamReader streamReader = null;
						try
						{
							streamReader = new StreamReader(Globals.UserDataDir + "\\Config.xml");
							Globals.configFile = XDocument.Parse(streamReader.ReadToEnd());
							goto IL_B3;
						}
						finally
						{
							if (streamReader != null)
							{
								streamReader.Close();
							}
						}
					}
					Globals.configFile.Add(new XElement(Globals.ProductName));
					Globals.configFile.Save(Globals.UserDataDir + "\\Config.xml");
				}
				IL_B3:
				return Globals.configFile;
			}
			set
			{
				Globals.configFile = value;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x0000213C File Offset: 0x0000113C
		public static string ProductName
		{
			get
			{
				return "NinjaTrader";
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002143 File Offset: 0x00001143
		public static string DataFolderName
		{
			get
			{
				return "NinjaTrader 8";
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000005 RID: 5 RVA: 0x0000214A File Offset: 0x0000114A
		public static string UserDataDir
		{
			get
			{
				return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\" + Globals.DataFolderName + "\\";
			}
		}

		// Token: 0x0400000D RID: 13
		private static XDocument configFile;

		// Token: 0x0400000E RID: 14
		private static readonly object[] sync = new object[0];
	}
}
