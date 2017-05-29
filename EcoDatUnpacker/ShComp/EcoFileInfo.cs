using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ShComp
{
    class EcoFileInfo
    {
        /// <summary>このファイルを格納する書庫のパス</summary>
        private string _datPath;

        /// <summary>書庫内の開始アドレス</summary>
        private int _addr;

        /// <summary>圧縮データのサイズ</summary>
        private int _packSize;

        /// <summary>展開データのサイズ</summary>
        private int _unpackSize;

        /// <summary>このファイルが圧縮されて格納されているかどうか</summary>
        private bool _isPacked;

        /// <summary>展開データ</summary>
        private WeakReference _dataRef;

        /// <summary>このファイルの名前を取得します。</summary>
        public string Name { get; private set; }

        /// <summary>抽出データのサイズを取得します。</summary>
        public int Size { get { return _unpackSize; } }

        private EcoFileInfo(string name, string datPath, byte[] parameter)
        {
            Name = name;
            _datPath = datPath;

            _addr = BitConverter.ToInt32(parameter, 0);

            var packSize = BitConverter.ToUInt32(parameter, 4);
            if (_isPacked = packSize > 0x80000000) _packSize = (int)(packSize - 0x80000000);
            else _packSize = (int)packSize;

            _unpackSize = BitConverter.ToInt32(parameter, 8);
        }

        [DllImport("Unpack.dll")]
        private static extern int Unpack(byte[] src, int srcSize, ref IntPtr dst, ref int dstSize, int dw);

        public static List<EcoFileInfo> GetList(string hedFileName)
        {
            using (var stream = File.Open(hedFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new BinaryReader(stream))
            {
                var datName = Path.ChangeExtension(hedFileName, "dat");
                var header = new EcoFileInfo(null, datName, reader.ReadBytes(12));

                var headerData = header.GetBytes();
                var dataNum = BitConverter.ToInt32(headerData, 0);
                var rawStr = Encoding.UTF8.GetString(headerData, 4, headerData.Length - 4);
                var names = rawStr.Split('\0');

                var result = new List<EcoFileInfo>();
                for (int i = 0; i < dataNum; i++)
                {
                    result.Add(new EcoFileInfo(names[i], datName, reader.ReadBytes(12)));
                }

                return result;
            }
        }

        public Stream GetStream()
        {
            var data = GetBytes();
            return new MemoryStream(data);
        }

        public byte[] GetBytes()
        {
            var res = _dataRef?.Target as byte[];
            if (res == null)
            {
                byte[] rawData;

                using (var stream = File.Open(_datPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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
    }
}
