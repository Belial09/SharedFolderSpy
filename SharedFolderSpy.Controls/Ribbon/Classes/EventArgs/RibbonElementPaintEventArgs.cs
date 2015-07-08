#region

using System.Drawing;
using System.Windows.Forms;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Enums;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs
{
    /// <summary>
    ///     Holds data and tools to draw the element
    /// </summary>
    public class RibbonElementPaintEventArgs
        : System.EventArgs
    {
        private readonly Rectangle _clip;
        private readonly Control _control;
        private readonly Graphics _graphics;
        private readonly RibbonElementSizeMode _mode;

        /// <param name="clip">Rectangle clip</param>
        /// <param name="graphics">Device to draw</param>
        /// <param name="mode">Size mode to draw</param>
        internal RibbonElementPaintEventArgs(Rectangle clip, Graphics graphics, RibbonElementSizeMode mode)
        {
            _clip = clip;
            _graphics = graphics;
            _mode = mode;
        }

        internal RibbonElementPaintEventArgs(Rectangle clip, Graphics graphics, RibbonElementSizeMode mode, Control control)
            : this(clip, graphics, mode)
        {
            _control = control;
        }

        /// <summary>
        ///     Area that element should occupy
        /// </summary>
        public Rectangle Clip
        {
            get { return _clip; }
        }

        /// <summary>
        ///     Gets the Device where to draw
        /// </summary>
        public Graphics Graphics
        {
            get { return _graphics; }
        }

        /// <summary>
        ///     Gets the mode to draw the element
        /// </summary>
        public RibbonElementSizeMode Mode
        {
            get { return _mode; }
        }


        /// <summary>
        ///     Gets the control where element is being painted
        /// </summary>
        public Control Control
        {
            get { return _control; }
        }
    }
}