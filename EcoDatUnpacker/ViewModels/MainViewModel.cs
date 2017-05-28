using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShComp;
using System.Collections.ObjectModel;

namespace EcoDatUnpacker.ViewModels
{
	class MainViewModel : PropertyChangedBase
	{
		public MainViewModel()
		{
			Root = new ObservableCollection<INode>();
		}

		public ObservableCollection<INode> Root { get; set; }

		private string _status;
		public string Status
		{
			get { return _status; }
			set
			{
				if (_status != value)
				{
					_status = value;
					OnPropertyChanged("Status");
				}
			}
		}

		private INode _treeSelectedNode;
		public INode TreeSelectedNode
		{
			get { return _treeSelectedNode; }
			set
			{
				if (_treeSelectedNode != value)
				{
					_treeSelectedNode = value;
					OnPropertyChanged("TreeSelectedNode");
				}
			}
		}

		private INode _listSelectedNode;
		public INode ListSelectedNode
		{
			get { return _listSelectedNode; }
			set
			{
				if (_listSelectedNode != value)
				{
					_listSelectedNode = value;
					OnPropertyChanged("ListSelectedNode");
				}
			}
		}
	}
}
