#region

using System.ComponentModel;
using System.Drawing;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Enums;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs
{
    public class RibbonHostSizeModeHandledEventArgs : HandledEventArgs
    {
        private readonly Graphics _graphics;
        private readonly RibbonElementSizeMode _sizeMode;
        private Size _Size;

        /// <summary>
        ///     Creates a new RibbonElementMeasureSizeEventArgs object
        /// </summary>
        /// <param name="graphics">Device info to draw and measure</param>
        /// <param name="sizeMode">Size mode to measure</param>
        internal RibbonHostSizeModeHandledEventArgs(Graphics graphics, RibbonElementSizeMode sizeMode)
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

        public Size ControlSize
        {
            get { return _Size; }
            set { _Size = value; }
        }
    }
}