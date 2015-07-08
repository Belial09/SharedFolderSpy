#region

using System.Drawing;
using Fesslersoft.SharedFolderSpy.Controls.Properties;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview.NodeControls
{
    public class NodeStateIcon : NodeIcon
    {
        private readonly Image _closed;
        private readonly Image _leaf;
        private readonly Image _opened;

        public NodeStateIcon()
        {
            _leaf = MakeTransparent(Resources.Leaf);
            _opened = MakeTransparent(Resources.Folder);
            _closed = MakeTransparent(Resources.FolderClosed);
        }

        protected override Image GetIcon(TreeNodeAdv node)
        {
            var icon = base.GetIcon(node);
            if (icon != null)
                return icon;
            if (node.IsLeaf)
                return _leaf;
            if (node.CanExpand && node.IsExpanded)
                return _opened;
            return _closed;
        }

        private static Image MakeTransparent(Bitmap bitmap)
        {
            bitmap.MakeTransparent(bitmap.GetPixel(0, 0));
            return bitmap;
        }
    }
}