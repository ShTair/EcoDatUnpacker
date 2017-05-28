using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EcoDatUnpacker.Properties;
using EcoDatUnpacker.ViewModels;
using Microsoft.Win32;
using IO = System.IO;

namespace EcoDatUnpacker
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			if (ApplicationDeployment.IsNetworkDeployed)
			{
				ApplicationDeployment.CurrentDeployment.CheckForUpdateCompleted
					+= new CheckForUpdateCompletedEventHandler(CurrentDeployment_CheckForUpdateCompleted);
				ApplicationDeployment.CurrentDeployment.CheckForUpdateAsync();
			}
		}

		private void CurrentDeployment_CheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
		{
			try
			{
				if (e.UpdateAvailable)
				{
					var cv = ApplicationDeployment.CurrentDeployment.CurrentVersion;
					var uv = e.AvailableVersion;
					if (cv.Major == uv.Major && cv.Minor == uv.Minor
						&& ((cv.Build < uv.Build)
						|| (cv.Build == uv.Build && cv.Revision < uv.Revision)))
					{
						ApplicationDeployment.CurrentDeployment.UpdateAsync();
					}
				}
			}
			catch { }
		}

		private MainViewModel _vm;
		private string _dataPath;

		private void Window_Initialized(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(Settings.Default.DstFolderName))
			{
				Settings.Default.DstFolderName
					= IO.Path.Combine(
						Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
						"Unpack");
			}

			Left = Settings.Default.Left;
			Top = Settings.Default.Top;

			if (Settings.Default.Width != 0 && Settings.Default.Height != 0)
			{
				Width = Settings.Default.Width;
				Height = Settings.Default.Height;
			}

			var reg = Registry.CurrentUser.OpenSubKey(@"Software\GungHo\Emil chronicle online");
			try
			{
				var ecoPath = reg.GetValue("LaunchPath") as string;
				if (ecoPath == null || !Directory.Exists(ecoPath))
				{
					throw new Exception();
				}
				_dataPath = IO.Path.Combine(ecoPath, "data");
			}
			catch
			{
				MessageBox.Show("ECOがインストールされていないか、一度も起動されていません。\nインストール済みの場合はECOを起動してください。");
				Close();
				return;
			}

			_vm = new MainViewModel();
			_vm.Root.Add(new DataFolder(_dataPath, ""));

			DataContext = _vm;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (WindowState == System.Windows.WindowState.Normal)
			{
				Settings.Default.Left = Left;
				Settings.Default.Top = Top;
				Settings.Default.Width = Width;
				Settings.Default.Height = Height;
			}

			Settings.Default.Save();
		}

		private void OptionsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var w = new OptionsWindow();
			w.Owner = this;
			w.ShowDialog();
		}

		private void TreeView_SelectedItemChanged(object sender,
			RoutedPropertyChangedEventArgs<object> e)
		{
			if (e.NewValue != null)
			{
				_vm.TreeSelectedNode = e.NewValue as INode;
			}
		}

		private void ExpandCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			string dstF;
			if (Settings.Default.MaintainingHierarchy
				&& Settings.Default.ToBaseStartExpantionNode)
			{
				dstF = Settings.Default.DstFolderName;
			}
			else
			{
				dstF = (e.Parameter as INode).RelativePath;
			}
			var w = new ExpandingWindow(e.Parameter as INode, dstF);
			w.Owner = this;
			w.ShowDialog();
		}

		private void ReadmeMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("準備中だよっ");
		}

		private void DstFolderMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(Settings.Default.DstFolderName))
			{
				Directory.CreateDirectory(Settings.Default.DstFolderName);
			}
			Process.Start(Settings.Default.DstFolderName);
		}

		private void VersionMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var w = new VersionWindow();
			w.Owner = this;
			w.ShowDialog();
		}

		private void SspMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var w = new SspWindow(_dataPath);
			w.Owner = this;
			w.ShowDialog();
		}

		private void EcoDataMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(_dataPath);
		}

		private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}

		private void GridViewColumnHeader_MouseDoubleClick(object sender, RoutedEventArgs e)
		{
			if (_vm.ListSelectedNode != null )
			{
				if (_vm.ListSelectedNode is IFolder)
				{
					if (_vm.ListSelectedNode is DataFolder)
					{
						(_vm.ListSelectedNode as DataFolder).IsExpanded = true;
					}
					_vm.TreeSelectedNode = _vm.ListSelectedNode;
				}
				else if (_vm.ListSelectedNode is INode)
				{
				}
			}
		}

		private void ChangeDataFileMenuItem_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var w = new ChangeDataFileWindow
				{
					Owner = this,
				};
				w.dataFileTextBox.Text = _dataPath;

				if (w.ShowDialog() ?? false)
				{
					_dataPath = w.DataFile;
					_vm.Root.Add(new DataFolder(_dataPath, ""));
				}
			}
			catch (Exception exp)
			{
				MessageBox.Show(exp.ToString(),
					ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString());
			}
		}
	}
}
