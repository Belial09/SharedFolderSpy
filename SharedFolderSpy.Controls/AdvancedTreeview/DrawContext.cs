#region

using System.Drawing;
using Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview.NodeControls;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview
{
    public struct DrawContext
    {
        private Rectangle _bounds;
        public Graphics Graphics { get; set; }

        public Rectangle Bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }

        public Font Font { get; set; }

        public DrawSelectionMode DrawSelection { get; set; }

        public bool DrawFocus { get; set; }

        public NodeControl CurrentEditorOwner { get; set; }

        public bool Enabled { get; set; }
    }
}