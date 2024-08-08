using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace NinjaTrader.Client
{
	// Token: 0x02000007 RID: 7
	public partial class QueryInstrument : Window
	{
		// Token: 0x06000051 RID: 81 RVA: 0x00002C83 File Offset: 0x00001C83
		public QueryInstrument()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00002C94 File Offset: 0x00001C94
		private void OnOk(object sender, RoutedEventArgs e)
		{
			if (this.instrument.Text.Length == 0)
			{
				MessageBox.Show("Please enter a valid instrument name", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				this.instrument.Focus();
				return;
			}
			base.DialogResult = new bool?(true);
			base.Close();
		}
	}
}
