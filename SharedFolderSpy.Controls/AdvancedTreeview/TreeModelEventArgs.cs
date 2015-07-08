#region

using System;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview
{
    public class TreeModelEventArgs : TreePathEventArgs
    {
        private readonly object[] _children;

        private readonly int[] _indices;

        /// <summary>
        /// </summary>
        /// <param name="parent">Path to a parent node</param>
        /// <param name="children">Child nodes</param>
        public TreeModelEventArgs(TreePath parent, object[] children)
            : this(parent, null, children)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="parent">Path to a parent node</param>
        /// <param name="indices">Indices of children in parent nodes collection</param>
        /// <param name="children">Child nodes</param>
        public TreeModelEventArgs(TreePath parent, int[] indices, object[] children)
            : base(parent)
        {
            if (children == null)
                throw new ArgumentNullException();

            if (indices != null && indices.Length != children.Length)
                throw new ArgumentException("indices and children arrays must have the same length");

            _indices = indices;
            _children = children;
        }

        public object[] Children
        {
            get { return _children; }
        }

        public int[] Indices
        {
            get { return _indices; }
        }
    }
}