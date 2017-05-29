using ShComp;
using System.Collections.ObjectModel;
using System.IO;

namespace EcoDatUnpacker
{
    class DataFolder : PropertyChangedBase, INode, IFolder
    {
        /// <param name="relativePath">このノードの親要素の相対パス</param>
        public DataFolder(string path, string relativePath)
        {
            RelativePath = relativePath;
            FullName = path;
            Name = Path.GetFileName(FullName);
        }

        public string RelativePath { get; private set; }
        public string FullName { get; set; }
        public string Name { get; private set; }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged("IsExpanded");
                }
            }
        }

        private ObservableCollection<INode> _children;
        public ObservableCollection<INode> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new ObservableCollection<INode>();
                    foreach (var item in Directory.GetDirectories(FullName))
                    {
                        Children.Add(new DataFolder(
                            Path.Combine(FullName, item),
                            Path.Combine(RelativePath, Name)));
                    }

                    foreach (var item in Directory.GetFiles(FullName))
                    {
                        if (Path.GetExtension(item).ToLower() == ".hed")
                        {
                            Children.Add(new HeaderFile(
                                Path.Combine(FullName, item),
                                Path.Combine(RelativePath, Name)));
                        }
                    }
                }

                return _children;
            }
        }
    }
}
