using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace EcoDatUnpacker.Converters
{
	class FileImageConverter:IValueConverter
	{
		public object Convert(object value, Type targetType,
			object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is DataFolder)
			{
				return "Images/folder.png";
			}
			else if (value is HeaderFile)
			{
				return "Images/library.png";
			}

			return "Images/file.png";
		}

		public object ConvertBack(object value, Type targetType,
			object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
