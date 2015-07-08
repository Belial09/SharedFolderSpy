#region

using System.Collections.ObjectModel;
using System.Windows.Forms;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview
{
    public class TreeNodeAdv
    {
        private readonly ReadOnlyCollection<TreeNodeAdv> _children;
        private readonly Collection<TreeNodeAdv> _nodes;

        #region Properties

        private readonly object _tag;
        private readonly TreeViewAdv _tree;
        private bool _isExpanded;
        private bool _isExpandedOnce;
        private bool _isSelected;
        private TreeNodeAdv _parent;

        internal TreeViewAdv Tree
        {
            get { return _tree; }
        }

        internal int Row { get; set; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    if (Tree.IsMyNode(this))
                    {
                        if (_isSelected)
                        {
                            if (!_tree.Selection.Contains(this))
                                _tree.Selection.Add(this);

                            if (_tree.Selection.Count == 1)
                                _tree.CurrentNode = this;
                        }
                        else
                            _tree.Selection.Remove(this);
                        _tree.UpdateView();
                        _tree.OnSelectionChanged();
                    }
                }
            }
        }

        public bool IsLeaf { get; internal set; }

        public bool IsExpandedOnce
        {
            get { return _isExpandedOnce; }
            internal set { _isExpandedOnce = value; }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (Tree.IsMyNode(this) && _isExpanded != value)
                {
                    if (value)
                        Tree.OnExpanding(this);
                    else
                        Tree.OnCollapsing(this);

                    if (value && !_isExpandedOnce)
                    {
                        var oldCursor = Tree.Cursor;
                        Tree.Cursor = Cursors.WaitCursor;
                        try
                        {
                            Tree.ReadChilds(this);
                        }
                        finally
                        {
                            Tree.Cursor = oldCursor;
                        }
                    }
                    _isExpanded = value; //&& CanExpand;
                    if (_isExpanded == value)
                        Tree.SmartFullUpdate();
                    else
                        Tree.UpdateView();

                    if (value)
                        Tree.OnExpanded(this);
                    else
                        Tree.OnCollapsed(this);
                }
            }
        }

        public TreeNodeAdv Parent
        {
            get { return _parent; }
            internal set { _parent = value; }
        }

        public int Level
        {
            get
            {
                if (_parent == null)
                    return 0;
                return _parent.Level + 1;
            }
        }

        public TreeNodeAdv NextNode
        {
            get
            {
                if (_parent != null)
                {
                    var index = _parent.Nodes.IndexOf(this);
                    if (index < _parent.Nodes.Count - 1)
                        return _parent.Nodes[index + 1];
                }
                return null;
            }
        }

        internal TreeNodeAdv BottomNode
        {
            get
            {
                var parent = Parent;
                if (parent != null)
                {
                    if (parent.NextNode != null)
                        return parent.NextNode;
                    return parent.BottomNode;
                }
                return null;
            }
        }

        public bool CanExpand
        {
            get { return (_nodes.Count > 0 || (!IsExpandedOnce && !IsLeaf)); }
        }

        public object Tag
        {
            get { return _tag; }
        }

        internal Collection<TreeNodeAdv> Nodes
        {
            get { return _nodes; }
        }

        public ReadOnlyCollection<TreeNodeAdv> Children
        {
            get { return _children; }
        }

        #endregion

        internal TreeNodeAdv(TreeViewAdv tree, object tag)
        {
            Row = -1;
            _tree = tree;
            _nodes = new Collection<TreeNodeAdv>();
            _children = new ReadOnlyCollection<TreeNodeAdv>(_nodes);
            _tag = tag;
        }

        public override string ToString()
        {
            if (Tag != null)
                return Tag.ToString();
            return base.ToString();
        }
    }
}