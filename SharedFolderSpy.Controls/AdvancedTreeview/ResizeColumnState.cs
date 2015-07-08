#region

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview
{
    internal class ResizeColumnState : InputState
    {
        private const int MinColumnWidth = 10;

        private readonly TreeColumn _column;
        private readonly int _initWidth;
        private Point _initLocation;

        public ResizeColumnState(TreeViewAdv tree, TreeColumn column, Point p)
            : base(tree)
        {
            _column = column;
            _initLocation = p;
            _initWidth = column.Width;
        }

        private void FinishResize()
        {
            Tree.ChangeInput();
            Tree.FullUpdate();
            Tree.OnColumnWidthChanged(_column);
        }

        public override void KeyDown(KeyEventArgs args)
        {
            args.Handled = true;
            if (args.KeyCode == Keys.Escape)
                FinishResize();
        }

        public override void MouseDown(TreeNodeAdvMouseEventArgs args)
        {
        }

        public override bool MouseMove(MouseEventArgs args)
        {
            var w = _initWidth + args.Location.X - _initLocation.X;
            _column.Width = Math.Max(MinColumnWidth, w);
            Tree.UpdateView();
            return true;
        }

        public override void MouseUp(TreeNodeAdvMouseEventArgs args)
        {
            FinishResize();
        }
    }
}