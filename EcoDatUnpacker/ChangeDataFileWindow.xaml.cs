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

namespace EcoDatUnpacker
{
	/// <summary>
	/// ChangeDataFileWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class ChangeDataFileWindow : Window
	{
		public ChangeDataFileWindow()
		{
			InitializeComponent();
		}

		public string DataFile { get; set; }

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			DataFile = dataFileTextBox.Text;
			DialogResult = true;
			Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
