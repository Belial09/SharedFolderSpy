#region

using System.Drawing;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview.NodeControls
{
    public class NodeIcon : BindableControl
    {
        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            var image = GetIcon(node);
            if (image != null)
            {
                var point = new Point(context.Bounds.X,
                    context.Bounds.Y + (context.Bounds.Height - image.Height)/2);
                context.Graphics.DrawImage(image, point);
            }
        }

        protected virtual Image GetIcon(TreeNodeAdv node)
        {
            return GetValue(node) as Image;
        }

        public override Size MeasureSize(TreeNodeAdv node)
        {
            var image = GetIcon(node);
            if (image != null)
                return image.Size;
            return Size.Empty;
        }
    }
}