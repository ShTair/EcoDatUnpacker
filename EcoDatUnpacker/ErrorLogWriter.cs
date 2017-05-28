using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EcoDatUnpacker
{
	class ErrorLogWriter
	{
		public static void Write(string log)
		{
			var p = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "EcoDatUnpacker.log");
			var ex = File.Exists(p);
			using (var writer = new StreamWriter(p, true, Encoding.Default))
			{
				if (!ex)
				{
					writer.WriteLine("このファイルは、えこだっとあんぱっかーのエラーログです。");
					writer.WriteLine("プログラムの実行中にエラーが発生したため、エラーログが保存されました。");
					writer.WriteLine("このファイルをサポートに送信してください。");
				}

				writer.WriteLine("=================================");
				writer.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

				writer.WriteLine(log);
			}
		}
	}
}
