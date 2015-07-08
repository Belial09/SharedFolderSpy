#region

using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview.NodeControls
{
    internal class NodeControlsCollection : Collection<NodeControl>
    {
        private readonly TreeViewAdv _tree;

        public NodeControlsCollection(TreeViewAdv tree)
        {
            _tree = tree;
        }

        protected override void ClearItems()
        {
            _tree.BeginUpdate();
            try
            {
                while (Count != 0)
                    RemoveAt(Count - 1);
            }
            finally
            {
                _tree.EndUpdate();
            }
        }

        protected override void InsertItem(int index, NodeControl item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (item.Parent != _tree)
            {
                if (item.Parent != null)
                {
                    item.Parent.NodeControls.Remove(item);
                }
                base.InsertItem(index, item);
                item.AssignParent(_tree);
                _tree.FullUpdate();
            }
        }

        protected override void RemoveItem(int index)
        {
            var value = this[index];
            value.AssignParent(null);
            base.RemoveItem(index);
            _tree.FullUpdate();
        }

        protected override void SetItem(int index, NodeControl item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _tree.BeginUpdate();
            try
            {
                RemoveAt(index);
                InsertItem(index, item);
            }
            finally
            {
                _tree.EndUpdate();
            }
        }
    }

    internal class NodeControlCollectionEditor : CollectionEditor
    {
        private readonly Type[] _types;

        public NodeControlCollectionEditor(Type type)
            : base(type)
        {
            _types = new[]
            {
                typeof (NodeTextBox), typeof (NodeComboBox), typeof (NodeCheckBox),
                typeof (NodeStateIcon), typeof (NodeIcon)
            };
        }

        protected override Type[] CreateNewItemTypes()
        {
            return _types;
        }
    }
}