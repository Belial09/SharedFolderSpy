#region

using System.Drawing;
using System.Windows.Forms;
using Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview.NodeControls;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview
{
    public struct EditorContext
    {
        private Rectangle _bounds;
        public TreeNodeAdv CurrentNode { get; set; }

        public Control Editor { get; set; }

        public NodeControl Owner { get; set; }

        public Rectangle Bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }
    }
}