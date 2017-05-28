using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IO = System.IO;
using ShComp;

namespace EcoDatUnpacker
{
	/// <summary>
	/// SspWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class SspWindow : Window
	{
		public SspWindow(string dataPath)
		{
			_dataPath = dataPath;

			InitializeComponent();
		}

		private string _dataPath;

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var sspPath = IO.Path.Combine(_dataPath, "effect\\effect.ssp");
			SspConverter.SaveAsCsv(sspPath, checkBox1.IsChecked ?? false);

			MessageBox.Show("たぶん変換が完了しました。");
			Close();
		}
	}
}
