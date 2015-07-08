#region

using System.Drawing;
using System.Windows.Forms;
using Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview.NodeControls;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview
{
    public class TreeNodeAdvMouseEventArgs : MouseEventArgs
    {
        private Point _absoluteLocation;
        private Rectangle _controlBounds;
        private Point _viewLocation;

        public TreeNodeAdvMouseEventArgs(MouseEventArgs args)
            : base(args.Button, args.Clicks, args.X, args.Y, args.Delta)
        {
        }

        public TreeNodeAdv Node { get; internal set; }

        public NodeControl Control { get; internal set; }

        public Point ViewLocation
        {
            get { return _viewLocation; }
            internal set { _viewLocation = value; }
        }

        public Point AbsoluteLocation
        {
            get { return _absoluteLocation; }
            internal set { _absoluteLocation = value; }
        }

        public Keys ModifierKeys { get; internal set; }

        public bool Handled { get; internal set; }

        public Rectangle ControlBounds
        {
            get { return _controlBounds; }
            internal set { _controlBounds = value; }
        }
    }
}