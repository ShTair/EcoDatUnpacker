using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace ShComp
{
	class TgaConverter
	{
		public static Bitmap ToBitmap(byte[] src)
		{
			using (var stream = new MemoryStream(src))
			using (var reader = new BinaryReader(stream))
			{
				var format = reader.ReadInt32();
				var width = reader.ReadInt16();
				var height = reader.ReadInt16();
				var parts = reader.ReadInt16();
				var unknown = reader.ReadInt16();

				stream.Seek(parts * 12, SeekOrigin.Current);

				Func<BinaryReader, Color> getter;
				switch (format)
				{
					case 0:
						getter = GetColorA1R5G5B5;
						break;
					case 1:
						getter = GetColorA4R4G4B4;
						break;
					default:
						getter = GetColorA8R8G8B8;
						break;
				}

				var result = new Bitmap(width, height);

				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						result.SetPixel(x, y, getter(reader));
					}
				}

				return result;
			}
		}

		private static Color GetColorA1R5G5B5(BinaryReader reader)
		{
			var temp = reader.ReadUInt16();
			return Color.FromArgb(((temp & 0x8000) >> 15) * 0xff,
				((temp & 0x7C00) >> 10) * 0xff / 0x1f,
				((temp & 0x3E0) >> 5) * 0xff / 0x1f,
				(temp & 0x1f) * 0xff / 0x1f);
		}

		private static Color GetColorA4R4G4B4(BinaryReader reader)
		{
			var temp = reader.ReadUInt16();
			return Color.FromArgb(((temp & 0xf000) >> 12) * 0xff / 0xf,
				((temp & 0xf00) >> 8) * 0xff / 0xf,
				((temp & 0xf0) >> 4) * 0xff / 0xf,
				(temp & 0xf) * 0xff / 0xf);
		}

		private static Color GetColorA8R8G8B8(BinaryReader reader)
		{
			var temp = reader.ReadBytes(4);
			return Color.FromArgb(temp[3],
				temp[2],
				temp[1],
				temp[0]);
		}
	}
}
