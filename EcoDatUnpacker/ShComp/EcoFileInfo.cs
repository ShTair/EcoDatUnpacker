using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace ShComp
{
	class EcoFileInfo
	{
		private EcoFileInfo()
		{
		}

		/// <summary>このファイルを格納する書庫のフルネーム</summary>
		private string _datName;
		/// <summary>このファイルの名前を取得します。</summary>
		public string Name { get; private set; }
		/// <summary>書庫内の開始アドレス</summary>
		private int _addr;
		/// <summary>圧縮データのサイズ</summary>
		private int _packSize;
		/// <summary>展開データのサイズ</summary>
		private int _unpackSize;
		/// <summary>抽出データのサイズを取得します。</summary>
		public int Size { get { return _unpackSize; } }
		/// <summary>このファイルが圧縮されて格納されているかどうか</summary>
		private bool _isPacked;

		/// <summary>展開データ</summary>
		private WeakReference _dataRef;

		public static List<EcoFileInfo> GetList(string hedFileName)
		{
			var datName = System.IO.Path.ChangeExtension(hedFileName, "dat");
			var result = new List<EcoFileInfo>();
			var header = new EcoFileInfo();

			using (var stream = File.Open(hedFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (var reader = new BinaryReader(stream))
			{
				header._datName = datName;
				SetAddr(reader.ReadBytes(12), header);

				var headerData = header.GetBytes();
				var dataNum = BitConverter.ToInt32(headerData, 0);
				var rawStr = Encoding.UTF8.GetString(headerData, 4, headerData.Length - 4);
				var names = rawStr.Split('\0');

				for (int i = 0; i < dataNum; i++)
				{
					var temp = new EcoFileInfo
					{
						_datName = datName,
						Name = names[i],
					};
					SetAddr(reader.ReadBytes(12), temp);
					result.Add(temp);
				}
			}

			return result;
		}

		private static void SetAddr(byte[] src, EcoFileInfo dst)
		{
			dst._addr = BitConverter.ToInt32(src, 0);
			var packSize = BitConverter.ToUInt32(src, 4);
			if (dst._isPacked = packSize > 0x80000000)
			{
				dst._packSize = (int)(packSize - 0x80000000);
			}
			else
			{
				dst._packSize = (int)packSize;
			}
			dst._unpackSize = BitConverter.ToInt32(src, 8);
		}

		public Stream GetStream()
		{
			var data = GetBytes();
			return new MemoryStream(data);
		}

		public byte[] GetBytes()
		{
			byte[] res;

			if (_dataRef == null || (res = _dataRef.Target as byte[]) == null)
			{
				byte[] rawData;

				using (var stream = File.Open(_datName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				using (var reader = new BinaryReader(stream))
				{
					stream.Seek(_addr, SeekOrigin.Begin);
					rawData = reader.ReadBytes(_packSize);
				}

				if (_isPacked)
				{
					var dstPtr = Marshal.AllocHGlobal(_unpackSize);
					try
					{
						if (Unpack(rawData, _packSize, ref dstPtr, ref _unpackSize, 1) != 1)
						{
							throw new Exception("圧縮ファイルの展開に失敗しました。");
						}
						var dstData = new byte[_unpackSize];
						Marshal.Copy(dstPtr, dstData, 0, _unpackSize);
						res = dstData;
					}
					finally
					{
						Marshal.FreeHGlobal(dstPtr);
					}
				}
				else
				{
					res = rawData;
				}

				_dataRef = new WeakReference(res);
			}

			return res;
		}

		public override string ToString()
		{
			return Name;
		}

		[DllImport("Unpack.dll")]
		private static extern int Unpack(byte[] src, int srcSize, ref IntPtr dst, ref int dstSize, int dw);
	}
}
