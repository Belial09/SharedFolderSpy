#region

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs
{
    public class RibbonCanvasEventArgs
        : System.EventArgs
    {
        #region ctor

        public RibbonCanvasEventArgs(
            Ribbon owner, Graphics g, Rectangle bounds, Control canvas, object relatedObject
            )
        {
            Owner = owner;
            Graphics = g;
            Bounds = bounds;
            Canvas = canvas;
            RelatedObject = relatedObject;
        }

        #endregion

        #region Props

        private Rectangle _bounds;
        public object RelatedObject { get; set; }


        /// <summary>
        ///     Gets or sets the Ribbon that raised the event
        /// </summary>
        public Ribbon Owner { get; set; }

        /// <summary>
        ///     Gets or sets the graphics to paint
        /// </summary>
        public Graphics Graphics { get; set; }

        /// <summary>
        ///     Gets or sets the bounds that should be painted
        /// </summary>
        public Rectangle Bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }

        /// <summary>
        ///     Gets or sets the control where to be painted
        /// </summary>
        public Control Canvas { get; set; }

        #endregion
    }
}