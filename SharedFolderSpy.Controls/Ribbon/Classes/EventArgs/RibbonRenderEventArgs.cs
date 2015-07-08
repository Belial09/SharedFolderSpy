#region

using System.Drawing;
using System;
#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs
{
    /// <remarks>Ribbon rendering event data</remarks>
    public class RibbonRenderEventArgs
        : System.EventArgs
    {
        private Rectangle _clipRectangle;

        public RibbonRenderEventArgs(Ribbon owner, Graphics g, Rectangle clip)
        {
            Ribbon = owner;
            Graphics = g;
            ClipRectangle = clip;
        }

        /// <summary>
        ///     Gets the Ribbon related to the render
        /// </summary>
        public Ribbon Ribbon { get; set; }

        /// <summary>
        ///     Gets the Device to draw into
        /// </summary>
        public Graphics Graphics { get; set; }

        /// <summary>
        ///     Gets the Rectangle area where to draw into
        /// </summary>
        public Rectangle ClipRectangle
        {
            get { return _clipRectangle; }
            set { _clipRectangle = value; }
        }
    }
}