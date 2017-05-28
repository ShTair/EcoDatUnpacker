using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ShComp
{
	class MapConverter
	{
		public static void Convert(byte[] data, string dstPath, string name)
		{
			if (!Directory.Exists(dstPath))
			{
				Directory.CreateDirectory(dstPath);
			}

			var dataPath = Path.Combine(dstPath, Path.ChangeExtension(name, "csv"));
			var headPath = Path.Combine(dstPath, Path.ChangeExtension(name, "txt"));

			using (var srcStream = new MemoryStream(data))
			using (var reader = new BinaryReader(srcStream))
			{
				short width, height;

				using (var writer = new StreamWriter(headPath, false, Encoding.Default))
				{
					writer.WriteLine("MapId={0}", reader.ReadInt32());
					var b = reader.ReadBytes(0x20);
					var s = Encoding.UTF8.GetString(b);
					s = s.Remove(s.IndexOf('\0'));
					writer.WriteLine("MapName={0}", s);
					writer.WriteLine("Width={0}", width = reader.ReadInt16());
					writer.WriteLine("Height={0}", height = reader.ReadInt16());
					writer.WriteLine("Holy={0}", reader.ReadByte());
					writer.WriteLine("Dark={0}", reader.ReadByte());
					writer.WriteLine("Unknown1={0}", reader.ReadByte());
					writer.WriteLine("Fire={0}", reader.ReadByte());
					writer.WriteLine("Wind={0}", reader.ReadByte());
					writer.WriteLine("Water={0}", reader.ReadByte());
					writer.WriteLine("Earth={0}", reader.ReadByte());
					writer.WriteLine("Unknown2={0}", reader.ReadByte());
					writer.WriteLine("Unknown3={0}", reader.ReadByte());
				}

				using (var writer = new StreamWriter(dataPath, false, Encoding.Default))
				{
					writer.WriteLine("X,Y,EventId,Holy,Dark,Unknown1,Fire,Wind,Water,Earth,Z,Flug,Unknown2,Unknown3,Unknown4");

					for (int y = 0; y < height; y++)
					{
						for (int x = 0; x < width; x++)
						{
							writer.Write(x);
							writer.Write(",");
							writer.Write(y);
							writer.Write(",");
							writer.Write(reader.ReadInt32());
							writer.Write(",");
							writer.Write(reader.ReadByte());
							writer.Write(",");
							writer.Write(reader.ReadByte());
							writer.Write(",");
							writer.Write(reader.ReadByte());
							writer.Write(",");
							writer.Write(reader.ReadByte());
							writer.Write(",");
							writer.Write(reader.ReadByte());
							writer.Write(",");
							writer.Write(reader.ReadByte());
							writer.Write(",");
							writer.Write(reader.ReadByte());
							writer.Write(",");
							writer.Write(reader.ReadInt16());
							writer.Write(",");
							writer.Write(reader.ReadByte());
							writer.Write(",");
							writer.Write(reader.ReadByte());
							writer.Write(",");
							writer.Write(reader.ReadByte());
							writer.Write(",");
							writer.Write(reader.ReadByte());
							writer.Write(",");
							writer.WriteLine();
						}
					}
				}
			}
		}
	}
}
