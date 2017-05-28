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
using EcoDatUnpacker.ViewModels;
using EcoDatUnpacker.Properties;
using System.IO;
using Microsoft.Win32;

namespace EcoDatUnpacker
{
	/// <summary>
	/// OptionsWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class OptionsWindow : Window
	{
		public OptionsWindow()
		{
			InitializeComponent();
		}

		private void Window_Initialized(object sender, EventArgs e)
		{
			dstFolderTextBox.Text = Settings.Default.DstFolderName;
			maintainingHierarchyCheckBox.IsChecked = Settings.Default.MaintainingHierarchy;
			toBaseStartExpantionNodeCheckBox.IsChecked = Settings.Default.ToBaseStartExpantionNode;
			convertingTgaCheckBox.IsChecked = Settings.Default.ConvertingTga;
			convertingMapCheckBox.IsChecked = Settings.Default.ConvertingMap;
		}

		private void okButton_Click(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(dstFolderTextBox.Text))
			{
				switch (MessageBox.Show(dstFolderTextBox.Text + "フォルダは存在しません。作成しますか？",
					"オプション", MessageBoxButton.YesNoCancel))
				{
					case MessageBoxResult.Yes:
						Directory.CreateDirectory(dstFolderTextBox.Text);
						Settings.Default.DstFolderName = dstFolderTextBox.Text;
						break;
					case MessageBoxResult.Cancel:
						return;
				}
			}
			else
			{
				Settings.Default.DstFolderName = dstFolderTextBox.Text;
			}

			Settings.Default.MaintainingHierarchy
				= maintainingHierarchyCheckBox.IsChecked ?? false;
			Settings.Default.ToBaseStartExpantionNode
				= toBaseStartExpantionNodeCheckBox.IsChecked ?? false;
			Settings.Default.ConvertingTga
				= convertingTgaCheckBox.IsChecked ?? false;
			Settings.Default.ConvertingMap
				= convertingMapCheckBox.IsChecked ?? false;

			Close();
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void dstFolderButton_Click(object sender, RoutedEventArgs e)
		{
			var of = new System.Windows.Forms.FolderBrowserDialog();
			of.SelectedPath = dstFolderTextBox.Text;

			if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				dstFolderTextBox.Text = of.SelectedPath;
			}
		}
	}
}
