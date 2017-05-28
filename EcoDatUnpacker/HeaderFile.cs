using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using ShComp;

namespace EcoDatUnpacker
{
	class HeaderFile : INode, IFolder
	{
		public HeaderFile(string path, string relativePath)
		{
			RelativePath = relativePath;
			FullName = path;
			Name = Path.GetFileNameWithoutExtension(FullName);
		}

		public string RelativePath { get; private set; }
		public string FullName { get; private set; }
		public string Name { get; private set; }

		private ObservableCollection<EcoFile> _children;
		public ObservableCollection<EcoFile> Children
		{
			get
			{
				if (_children == null)
				{
					_children = new ObservableCollection<EcoFile>();
					var list = EcoFileInfo.GetList(FullName);
					list.ForEach(t => _children.Add(new EcoFile(t,
						Path.Combine(RelativePath, Name))));
				}
				return _children;
			}
		}
	}
}
