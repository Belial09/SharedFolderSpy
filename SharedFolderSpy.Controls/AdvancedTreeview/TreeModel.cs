#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview
{
    public class TreeModel : ITreeModel
    {
        private readonly Node _root;

        public TreeModel()
        {
            _root = new Node();
            _root.Model = this;
        }

        public Node Root
        {
            get { return _root; }
        }

        public Collection<Node> Nodes
        {
            get { return _root.Nodes; }
        }

        public Node FindNode(TreePath path)
        {
            if (path.IsEmpty())
                return _root;
            return FindNode(_root, path, 0);
        }

        private Node FindNode(Node root, TreePath path, int level)
        {
            foreach (var node in root.Nodes)
                if (node == path.FullPath[level])
                {
                    if (level == path.FullPath.Length - 1)
                        return node;
                    return FindNode(node, path, level + 1);
                }
            return null;
        }

        public TreePath GetPath(Node node)
        {
            if (node == _root)
                return TreePath.Empty;
            var stack = new Stack<object>();
            while (node != _root)
            {
                stack.Push(node);
                node = node.Parent;
            }
            return new TreePath(stack.ToArray());
        }

        #region ITreeModel Members

        public IEnumerable GetChildren(TreePath treePath)
        {
            var node = FindNode(treePath);
            if (node != null)
                foreach (var n in node.Nodes)
                    yield return n;
            else
                yield break;
        }

        public bool IsLeaf(TreePath treePath)
        {
            var node = FindNode(treePath);
            if (node != null)
                return node.IsLeaf;
            throw new ArgumentException("treePath");
        }

        public event EventHandler<TreeModelEventArgs> NodesChanged;

        public event EventHandler<TreePathEventArgs> StructureChanged;

        public event EventHandler<TreeModelEventArgs> NodesInserted;

        public event EventHandler<TreeModelEventArgs> NodesRemoved;

        internal void OnNodeInserted(Node parent, int index, Node node)
        {
            if (NodesInserted != null)
            {
                var args = new TreeModelEventArgs(GetPath(parent), new[] {index}, new object[] {node});
                NodesInserted(this, args);
            }
        }

        internal void OnNodeRemoved(Node parent, int index, Node node)
        {
            if (NodesRemoved != null)
            {
                var args = new TreeModelEventArgs(GetPath(parent), new[] {index}, new object[] {node});
                NodesRemoved(this, args);
            }
        }

        internal void OnNodesChanged(TreeModelEventArgs args)
        {
            if (NodesChanged != null)
                NodesChanged(this, args);
        }

        public void OnStructureChanged(TreePathEventArgs args)
        {
            if (StructureChanged != null)
                StructureChanged(this, args);
        }

        #endregion
    }
}