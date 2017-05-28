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
using System.Threading.Tasks;
using System.Windows.Threading;
using System.IO;
using EcoDatUnpacker.Properties;
using IO = System.IO;
using ShComp;
using System.Drawing.Imaging;

namespace EcoDatUnpacker
{
	/// <summary>
	/// ExpandingWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class ExpandingWindow : Window
	{
		public ExpandingWindow(INode target, string dstPath)
		{
			_target = target;
			_dstPath = dstPath;

			InitializeComponent();

			_progressTimer = new DispatcherTimer();
			_progressTimer.Tick += new EventHandler(_progressTimer_Tick);
			_progressTimer.Interval = TimeSpan.FromMilliseconds(100);
		}

		void _progressTimer_Tick(object sender, EventArgs e)
		{
			var l = (double)_current / _sum;
			allProgressBar.Value = l;
			textBlock1.Text = l.ToString("P") + " " + _current + "/" + _sum;
			textBlock2.Text = _currentName;
		}

		private INode _target;
		private string _dstPath;
		private DispatcherTimer _progressTimer;
		private Task _expandTask;
		private bool _expandEnabled;

		private int _sum;
		private int _current;
		private string _currentName;

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			_sum = Sum(_target);
			_progressTimer.Start();

			_expandEnabled = true;
			_expandTask = Task.Factory.StartNew(() =>
			{
				Expand(_target, _dstPath);
				Dispatcher.BeginInvoke((Action)(() => Close()));
			});
		}

		private int Sum(INode target)
		{
			if (target is DataFolder)
			{
				return (target as DataFolder).Children.Sum(t => Sum(t));
			}
			else if (target is HeaderFile)
			{
				return (target as HeaderFile).Children.Count;
			}

			return 1;
		}

		private void Expand(INode target, string dstPath)
		{
			try
			{
				if (_expandEnabled)
				{
					if (target is DataFolder)
					{
						foreach (var item in (target as DataFolder).Children)
						{
							Expand(item, IO.Path.Combine(dstPath, (target as DataFolder).Name));
						}
					}
					else if (target is HeaderFile)
					{
						foreach (var item in (target as HeaderFile).Children)
						{
							Expand(item, IO.Path.Combine(dstPath, (target as HeaderFile).Name));
						}
					}
					else if (target is EcoFile)
					{
						var i = (target as EcoFile).FileInfo;

						string df;

						if (Settings.Default.MaintainingHierarchy)
						{
							df = IO.Path.Combine(Settings.Default.DstFolderName, dstPath);
						}
						else
						{
							df = Settings.Default.DstFolderName;
						}

						if (!Directory.Exists(df))
						{
							Directory.CreateDirectory(df);
						}

						_currentName = IO.Path.Combine(dstPath, i.Name);

						if ((IO.Path.GetExtension(i.Name).ToLower() == ".tga"
							|| IO.Path.GetExtension(i.Name).ToLower() == ".bmp")
							&& Settings.Default.ConvertingTga)
						{
							var bmp = TgaConverter.ToBitmap(i.GetBytes());
							bmp.Save(IO.Path.Combine(df,
								IO.Path.ChangeExtension(i.Name, "png")), ImageFormat.Png);
						}
						else if (IO.Path.GetExtension(i.Name).ToLower() == ".map"
							&& Settings.Default.ConvertingMap)
						{
							MapConverter.Convert(i.GetBytes(), df, i.Name);
						}
						else
						{
							using (var stream = File.Open(
								IO.Path.Combine(df, i.Name),
									FileMode.Create, FileAccess.Write, FileShare.None))
							{
								if (i.Size != 0)
								{
									stream.Write(i.GetBytes(), 0, i.Size);
								}
                                else
                                {
                                    Console.WriteLine("");
                                }
							}
						}

						_current++;
					}
				}
			}
			catch(Exception exp)
			{
				var sb = new StringBuilder();
				sb.AppendLine(exp.ToString());
				sb.AppendLine("target =");
				sb.AppendLine(target.GetType().ToString());
				sb.AppendLine(target.RelativePath);
				sb.AppendLine("dstPath = " + dstPath);
				ErrorLogWriter.Write(sb.ToString());
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (_expandEnabled)
			{
				_expandEnabled = false;
				_expandTask.Wait();
			}
			_progressTimer.Stop();
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
