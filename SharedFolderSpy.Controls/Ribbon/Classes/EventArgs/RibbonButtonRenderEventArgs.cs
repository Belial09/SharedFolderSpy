#region

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs
{
    public sealed class RibbonButtonRenderEventArgs : RibbonRenderEventArgs
    {
        public RibbonButtonRenderEventArgs(Ribbon owner, Graphics g, Rectangle clip, RibbonButton button)
            : base(owner, g, clip)
        {
            Button = button;
        }

        /// <summary>
        ///     Gets or sets the RibbonButton related to the evennt
        /// </summary>
        public RibbonButton Button { get; set; }
    }
}