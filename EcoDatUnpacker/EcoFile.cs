using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShComp;

namespace EcoDatUnpacker
{
	class EcoFile : INode
	{
		public EcoFile(EcoFileInfo fileInfo, string relativePath)
		{
			RelativePath = relativePath;
			FileInfo = fileInfo;
		}

		public string RelativePath { get; private set; }
		public EcoFileInfo FileInfo { get; private set; }
		public string Name { get { return FileInfo.Name; } }
		public int Size { get { return FileInfo.Size; } }
	}
}
