using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace EcoDatUnpacker
{
	static class MainCommands
	{
		private static readonly RoutedCommand _expand
			= new RoutedCommand("Expand", typeof(MainCommands));
		public static RoutedCommand Expand
		{
			get { return _expand; }
		}
	}
}
