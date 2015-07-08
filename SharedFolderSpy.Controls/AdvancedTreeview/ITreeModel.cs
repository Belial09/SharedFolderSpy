#region

using System;
using System.Collections;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview
{
    public interface ITreeModel
    {
        IEnumerable GetChildren(TreePath treePath);
        bool IsLeaf(TreePath treePath);

        event EventHandler<TreeModelEventArgs> NodesChanged;
        event EventHandler<TreeModelEventArgs> NodesInserted;
        event EventHandler<TreeModelEventArgs> NodesRemoved;
        event EventHandler<TreePathEventArgs> StructureChanged;
    }
}