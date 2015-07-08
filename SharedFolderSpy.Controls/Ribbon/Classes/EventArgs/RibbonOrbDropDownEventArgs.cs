#region

using System.Drawing;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs
{
    public class RibbonOrbDropDownEventArgs
        : RibbonRenderEventArgs
    {
        #region Fields

        private readonly RibbonOrbDropDown _dropDown;

        #endregion

        #region Ctor

        public RibbonOrbDropDownEventArgs(Ribbon ribbon, RibbonOrbDropDown dropDown, Graphics g, Rectangle clip)
            : base(ribbon, g, clip)
        {
            _dropDown = dropDown;
        }

        #endregion

        #region Props

        /// <summary>
        ///     Gets the RibbonOrbDropDown related to the event
        /// </summary>
        public RibbonOrbDropDown RibbonOrbDropDown
        {
            get { return _dropDown; }
        }

        #endregion
    }
}