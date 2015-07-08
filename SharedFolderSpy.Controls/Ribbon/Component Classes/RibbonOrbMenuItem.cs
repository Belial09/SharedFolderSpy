#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Designers;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Enums;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon
{
    [Designer(typeof (RibbonOrbMenuItemDesigner))]
    public class RibbonOrbMenuItem
        : RibbonButton
    {
        #region Fields

        #endregion

        #region Ctor

        public RibbonOrbMenuItem()
        {
            DropDownArrowDirection = RibbonArrowDirection.Left;
            SetDropDownMargin(new Padding(10));
            DropDownShowing += RibbonOrbMenuItem_DropDownShowing;
        }

        public RibbonOrbMenuItem(string text)
            : this()
        {
            Text = text;
        }

        #endregion

        #region Props

        public override Image Image
        {
            get { return base.Image; }
            set
            {
                base.Image = value;

                SmallImage = value;
            }
        }

        [Browsable(false)]
        public override Image SmallImage
        {
            get { return base.SmallImage; }
            set { base.SmallImage = value; }
        }

        #endregion

        #region Methods

        public override void OnClick(EventArgs e)
        {
            base.OnClick(e);
        }

        internal override Point OnGetDropDownMenuLocation()
        {
            if (Owner == null) return base.OnGetDropDownMenuLocation();

            var b = Owner.RectangleToScreen(Bounds);
            var c = Owner.OrbDropDown.RectangleToScreen(Owner.OrbDropDown.ContentRecentItemsBounds);

            return new Point(b.Right, c.Top);
        }

        internal override Size OnGetDropDownMenuSize()
        {
            var r = Owner.OrbDropDown.ContentRecentItemsBounds;
            r.Inflate(-1, -1);
            return r.Size;
        }

        public override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (RibbonDesigner.Current == null)
            {
                if (Owner.OrbDropDown.LastPoppedMenuItem != null)
                {
                    Owner.OrbDropDown.LastPoppedMenuItem.CloseDropDown();
                }

                if (Style == RibbonButtonStyle.DropDown || Style == RibbonButtonStyle.SplitDropDown)
                {
                    ShowDropDown();

                    Owner.OrbDropDown.LastPoppedMenuItem = this;
                }
            }
        }

        public override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
        }

        private void RibbonOrbMenuItem_DropDownShowing(object sender, EventArgs e)
        {
            if (DropDown != null)
            {
                DropDown.DrawIconsBar = false;
            }
        }

        #endregion
    }
}