#region

using System;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview
{
    public class TreeViewAdvEventArgs : EventArgs
    {
        private readonly TreeNodeAdv _node;

        public TreeViewAdvEventArgs(TreeNodeAdv node)
        {
            _node = node;
        }

        public TreeNodeAdv Node
        {
            get { return _node; }
        }
    }
}