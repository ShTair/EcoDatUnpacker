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
using System.Deployment.Application;
using System.Reflection;

namespace EcoDatUnpacker
{
	/// <summary>
	/// VersionWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class VersionWindow : Window
	{
		public VersionWindow()
		{
			InitializeComponent();
		}

		private void Window_Initialized(object sender, EventArgs e)
		{
			if (ApplicationDeployment.IsNetworkDeployed)
			{
				versionTextBlock.Text
					= ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
			}
			else
			{
				versionTextBlock.Text
					= "正式配布前のzip配布版では、バージョン情報を確認できません。\nサポートにお問い合わせください。";
			}
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
