#region

using System.Drawing;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs
{
    public sealed class RibbonTabRenderEventArgs : RibbonRenderEventArgs
    {
        public RibbonTabRenderEventArgs(Ribbon owner, Graphics g, Rectangle clip, RibbonTab tab)
            : base(owner, g, clip)
        {
            Tab = tab;
        }

        /// <summary>
        ///     Gets or sets the RibbonTab related to the evennt
        /// </summary>
        public RibbonTab Tab { get; set; }
    }
}