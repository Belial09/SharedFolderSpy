#region

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Fesslersoft.SharedFolderSpy.Controls.Properties;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview.NodeControls
{
    internal class NodePlusMinus : NodeControl
    {
        public const int ImageSize = 9;
        public const int Width = 16;
        private readonly Bitmap _minus;
        private readonly Bitmap _plus;

        public NodePlusMinus()
        {
            _plus = Resources.plus;
            _minus = Resources.minus;
        }

        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            if (node.CanExpand)
            {
                var r = context.Bounds;
                var dy = (int) Math.Round((float) (r.Height - ImageSize)/2);
                if (Application.RenderWithVisualStyles)
                {
                    VisualStyleRenderer renderer;
                    if (node.IsExpanded)
                        renderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Opened);
                    else
                        renderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Closed);
                    renderer.DrawBackground(context.Graphics, new Rectangle(r.X, r.Y + dy, ImageSize, ImageSize));
                }
                else
                {
                    Image img;
                    if (node.IsExpanded)
                        img = _minus;
                    else
                        img = _plus;
                    context.Graphics.DrawImageUnscaled(img, new Point(r.X, r.Y + dy));
                }
            }
        }

        public override Size MeasureSize(TreeNodeAdv node)
        {
            return new Size(Width, Width);
        }

        public override void MouseDoubleClick(TreeNodeAdvMouseEventArgs args)
        {
            args.Handled = true; // Supress expand/collapse when double click on plus/minus
        }

        public override void MouseDown(TreeNodeAdvMouseEventArgs args)
        {
            if (args.Button == MouseButtons.Left)
            {
                args.Handled = true;
                if (args.Node.CanExpand)
                    args.Node.IsExpanded = !args.Node.IsExpanded;
            }
        }
    }
}