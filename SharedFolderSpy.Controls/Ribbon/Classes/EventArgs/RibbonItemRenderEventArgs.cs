#region

using System.Drawing;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs
{
    public class RibbonItemRenderEventArgs : RibbonRenderEventArgs
    {
        public RibbonItemRenderEventArgs(Ribbon owner, Graphics g, Rectangle clip, RibbonItem item)
            : base(owner, g, clip)
        {
            Item = item;
        }

        public RibbonItem Item { get; set; }
    }
}