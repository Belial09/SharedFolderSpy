#region

using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview
{
    public class Node
    {
        #region NodeCollection

        private class NodeCollection : Collection<Node>
        {
            private readonly Node _owner;

            public NodeCollection(Node owner)
            {
                _owner = owner;
            }

            protected override void ClearItems()
            {
                while (Count != 0)
                    RemoveAt(Count - 1);
            }

            protected override void InsertItem(int index, Node item)
            {
                if (item == null)
                    throw new ArgumentNullException("item");

                if (item.Parent != _owner)
                {
                    if (item.Parent != null)
                        item.Parent.Nodes.Remove(item);
                    item._parent = _owner;
                    base.InsertItem(index, item);

                    var model = _owner.FindModel();
                    if (model != null)
                        model.OnNodeInserted(_owner, index, item);
                }
            }

            protected override void RemoveItem(int index)
            {
                var item = this[index];
                item._parent = null;
                base.RemoveItem(index);

                var model = _owner.FindModel();
                if (model != null)
                    model.OnNodeRemoved(_owner, index, item);
            }

            protected override void SetItem(int index, Node item)
            {
                if (item == null)
                    throw new ArgumentNullException("item");

                RemoveAt(index);
                InsertItem(index, item);
            }
        }

        #endregion

        #region Properties

        private readonly NodeCollection _nodes;
        private CheckState _checkState;

        private Node _parent;
        private string _text;
        internal TreeModel Model { get; set; }

        public Collection<Node> Nodes
        {
            get { return _nodes; }
        }

        public Node Parent
        {
            get { return _parent; }
            set
            {
                if (value != _parent)
                {
                    if (_parent != null)
                        _parent.Nodes.Remove(this);

                    if (value != null)
                        value.Nodes.Add(this);
                }
            }
        }

        public int Index
        {
            get
            {
                if (_parent != null)
                    return _parent.Nodes.IndexOf(this);
                return -1;
            }
        }

        public Node PreviousNode
        {
            get
            {
                var index = Index;
                if (index > 0)
                    return _parent.Nodes[index - 1];
                return null;
            }
        }

        public Node NextNode
        {
            get
            {
                var index = Index;
                if (index >= 0 && index < _parent.Nodes.Count - 1)
                    return _parent.Nodes[index + 1];
                return null;
            }
        }

        public virtual string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    NotifyModel();
                }
            }
        }

        public virtual CheckState CheckState
        {
            get { return _checkState; }
            set
            {
                if (_checkState != value)
                {
                    _checkState = value;
                    NotifyModel();
                }
            }
        }

        public bool IsChecked
        {
            get { return CheckState != CheckState.Unchecked; }
            set
            {
                if (value)
                    CheckState = CheckState.Checked;
                else
                    CheckState = CheckState.Unchecked;
            }
        }

        public virtual bool IsLeaf
        {
            get { return false; }
        }

        #endregion

        public Node()
            : this(string.Empty)
        {
        }

        public Node(string text)
        {
            _text = text;
            _nodes = new NodeCollection(this);
        }

        private TreeModel FindModel()
        {
            var node = this;
            while (node != null)
            {
                if (node.Model != null)
                    return node.Model;
                node = node.Parent;
            }
            return null;
        }

        protected void NotifyModel()
        {
            var model = FindModel();
            if (model != null && Parent != null)
            {
                var path = model.GetPath(Parent);
                if (path != null)
                {
                    var args = new TreeModelEventArgs(path, new[] {Index}, new object[] {this});
                    model.OnNodesChanged(args);
                }
            }
        }
    }
}