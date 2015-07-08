namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview
{
    public class TreePath
    {
        public static readonly TreePath Empty = new TreePath();

        private readonly object[] _path;

        public TreePath()
        {
            _path = new object[0];
        }

        public TreePath(object node)
        {
            _path = new[] {node};
        }

        public TreePath(object[] path)
        {
            _path = path;
        }

        public object[] FullPath
        {
            get { return _path; }
        }

        public object LastNode
        {
            get
            {
                if (_path.Length > 0)
                    return _path[_path.Length - 1];
                return null;
            }
        }

        public object FirstNode
        {
            get
            {
                if (_path.Length > 0)
                    return _path[0];
                return null;
            }
        }

        public bool IsEmpty()
        {
            return (_path.Length == 0);
        }
    }
}