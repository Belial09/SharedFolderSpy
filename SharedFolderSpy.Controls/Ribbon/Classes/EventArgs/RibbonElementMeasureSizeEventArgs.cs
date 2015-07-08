#region

using System.Drawing;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Enums;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs
{
    /// <summary>
    ///     Holds data and tools to measure the size
    /// </summary>
    public class RibbonElementMeasureSizeEventArgs
        : System.EventArgs
    {
        private readonly Graphics _graphics;
        private readonly RibbonElementSizeMode _sizeMode;

        /// <summary>
        ///     Creates a new RibbonElementMeasureSizeEventArgs object
        /// </summary>
        /// <param name="graphics">Device info to draw and measure</param>
        /// <param name="sizeMode">Size mode to measure</param>
        internal RibbonElementMeasureSizeEventArgs(Graphics graphics, RibbonElementSizeMode sizeMode)
        {
            _graphics = graphics;
            _sizeMode = sizeMode;
        }

        /// <summary>
        ///     Gets the size mode to measure
        /// </summary>
        public RibbonElementSizeMode SizeMode
        {
            get { return _sizeMode; }
        }

        /// <summary>
        ///     Gets the device to measure objects
        /// </summary>
        public Graphics Graphics
        {
            get { return _graphics; }
        }
    }
}