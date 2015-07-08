#region

using System;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview
{
    public class TreeColumnEventArgs : EventArgs
    {
        private readonly TreeColumn _column;

        public TreeColumnEventArgs(TreeColumn column)
        {
            _column = column;
        }

        public TreeColumn Column
        {
            get { return _column; }
        }
    }
}