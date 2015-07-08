#region

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs
{
    public sealed class RibbonPanelRenderEventArgs : RibbonRenderEventArgs
    {
        public RibbonPanelRenderEventArgs(Ribbon owner, Graphics g, Rectangle clip, RibbonPanel panel, Control canvas)
            : base(owner, g, clip)
        {
            Panel = panel;
            Canvas = canvas;
        }


        /// <summary>
        ///     Gets or sets the panel related to the events
        /// </summary>
        public RibbonPanel Panel { get; set; }

        /// <summary>
        ///     Gets or sets the control where the panel is being rendered
        /// </summary>
        public Control Canvas { get; set; }
    }
}