using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using EcoDatUnpacker.Properties;

namespace ShComp
{
    class SspConverter
    {
        public static void SaveAsCsv(string path, bool b)
        {
            if (Path.GetExtension(path).ToLower() == ".ssp")
            {
                var heads = new Header[10000];

                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new BinaryReader(stream))
                {
                    for (int i = 0; i < heads.Length; i++)
                    {
                        heads[i] = new Header { Addr = reader.ReadInt32() };
                    }

                    for (int i = 0; i < heads.Length; i++)
                    {
                        heads[i].Size = reader.ReadInt16();
                    }

                    var name = Path.ChangeExtension(Path.GetFileNameWithoutExtension(path), ".csv");
                    var savepath = Path.Combine(Settings.Default.DstFolderName, name);
                    if (!Directory.Exists(Settings.Default.DstFolderName))
                    {
                        Directory.CreateDirectory(Settings.Default.DstFolderName);
                    }

                    using (var writer = new StreamWriter(savepath, false, Encoding.Default))
                    {
                        writer.WriteLine("Id,Name,Description,IsActive,MaxLv,Lv,Unknown1,Mp,Sp,Ep,Range,TargetType,RangeType,EffectRange,Equip,Unknown2,SkillFlags,Unknown3,Unknown4,Unknown5,Unknown6,Effect1,Effect2,Effect3,Effect4,Effect5,Effect6,Effect7,Effect8,Effect9,Anim1,Anim2,Anim3");

                        try
                        {
                            foreach (var item in heads)
                            {
                                if (item.Size == 0) continue;

                                stream.Seek(item.Addr, SeekOrigin.Begin);
                                var id = reader.ReadInt16();
                                //if (id == 712)
                                //{
                                //    Console.WriteLine();
                                //}
                                writer.Write(id);
                                writer.Write(",");
                                var s = Encoding.Unicode.GetString(reader.ReadBytes(0x80));
                                if (b)
                                {
                                    s = s.Replace("\r", "").Replace("\n", "").Replace(",", "");
                                }
                                else
                                {
                                    s = s.Remove(s.IndexOf('\0'));
                                }
                                writer.Write(s);
                                writer.Write(",");
                                s = Encoding.Unicode.GetString(reader.ReadBytes(0x200));
                                if (b)
                                {
                                    s = s.Replace("\r", "").Replace("\n", "").Replace(",", "");
                                }
                                else
                                {
                                    s = s.Remove(s.IndexOf('\0'));
                                }
                                writer.Write(s);
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
                                writer.Write(reader.ReadInt16());
                                writer.Write(",");
                                writer.Write(reader.ReadInt16());
                                writer.Write(",");
                                writer.Write(reader.ReadSByte());
                                writer.Write(",");
                                writer.Write(reader.ReadByte());
                                writer.Write(",");
                                writer.Write(reader.ReadByte());
                                writer.Write(",");
                                writer.Write(reader.ReadByte());
                                writer.Write(",");
                                writer.Write(reader.ReadInt32());
                                writer.Write(",");
                                writer.Write(reader.ReadInt32());
                                writer.Write(",");
                                writer.Write(reader.ReadInt32());
                                writer.Write(",");
                                writer.Write(reader.ReadInt16());
                                writer.Write(",");
                                writer.Write(reader.ReadInt16());
                                writer.Write(",");
                                writer.Write(reader.ReadInt16());
                                writer.Write(",");
                                writer.Write(reader.ReadInt16());
                                writer.Write(",");
                                writer.Write(reader.ReadInt32());
                                writer.Write(",");
                                writer.Write(reader.ReadInt32());
                                writer.Write(",");
                                writer.Write(reader.ReadInt32());
                                writer.Write(",");
                                writer.Write(reader.ReadInt32());
                                writer.Write(",");
                                writer.Write(reader.ReadInt32());
                                writer.Write(",");
                                writer.Write(reader.ReadInt32());
                                writer.Write(",");
                                writer.Write(reader.ReadInt32());
                                writer.Write(",");
                                writer.Write(reader.ReadInt32());
                                writer.Write(",");
                                writer.Write(reader.ReadInt32());
                                writer.Write(",");
                                writer.Write(reader.ReadInt16());
                                writer.Write(",");
                                writer.Write(reader.ReadInt16());
                                writer.Write(",");
                                writer.Write(reader.ReadInt16());
                                writer.WriteLine();

                            }
                        }
                        catch { }
                    }
                }
            }
        }

        private class Header
        {
            public int Addr { get; set; }
            public int Size { get; set; }
        }

        private class Data
        {
            public Header Head { get; set; }
            public short Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public byte IsActive { get; set; }
            public byte MaxLv { get; set; }
            public byte Lv { get; set; }
            public byte Unknown1 { get; set; }
            public short Mp { get; set; }
            public short Sp { get; set; }
            public short Ep { get; set; }
            public byte Range { get; set; }
            public byte TargetType { get; set; }
            public byte RangeType { get; set; }
            public byte EffectRange { get; set; }
            public int Equip { get; set; }
            public int Unknown2 { get; set; }
            public int SkillFlags { get; set; }
            public short Unknown3 { get; set; }
            public short Unknown4 { get; set; }
            public short Unknown5 { get; set; }
            public short Unknown6 { get; set; }
            public int Effect1 { get; set; }
            public int Effect2 { get; set; }
            public int Effect3 { get; set; }
            public int Effect4 { get; set; }
            public int Effect5 { get; set; }
            public int Effect6 { get; set; }
            public int Effect7 { get; set; }
            public int Effect8 { get; set; }
            public int Effect9 { get; set; }
            public short Anim1 { get; set; }
            public short Anim2 { get; set; }
            public short Anim3 { get; set; }
        }
    }
}
