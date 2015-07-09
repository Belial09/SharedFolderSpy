#region

using System.Drawing;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Enums;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon
{
    public class RibbonOrbRecentItem
        : RibbonButton
    {
        #region Ctor

        public RibbonOrbRecentItem()
        {
        }

        public RibbonOrbRecentItem(string text)
            : this()
        {
            Text = text;
        }

        #endregion

        #region Methods

        internal override Rectangle OnGetImageBounds(RibbonElementSizeMode sMode, Rectangle bounds)
        {
            return Rectangle.Empty;
        }

        internal override Rectangle OnGetTextBounds(RibbonElementSizeMode sMode, Rectangle bounds)
        {
            var r = base.OnGetTextBounds(sMode, bounds);

            r.X = Bounds.Left + 3;

            return r;
        }

        #endregion
    }
}