#region

using System.Drawing;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs
{
    public class RibbonItemBoundsEventArgs
        : RibbonItemRenderEventArgs
    {
        public RibbonItemBoundsEventArgs(Ribbon owner, Graphics g, Rectangle clip, RibbonItem item, Rectangle bounds)
            : base(owner, g, clip, item)
        {
            Bounds = bounds;
        }

        #region Properties

        private Rectangle _bounds;

        /// <summary>
        ///     Gets or sets the suggested bounds
        /// </summary>
        public Rectangle Bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }

        #endregion
    }
}