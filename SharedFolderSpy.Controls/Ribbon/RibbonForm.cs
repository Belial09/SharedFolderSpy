#region

using System.Windows.Forms;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Interfaces;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon
{
    public class RibbonForm
        : Form, IRibbonForm
    {
        #region Fields

        private readonly RibbonFormHelper _helper;

        #endregion

        #region Ctor

        public RibbonForm()
        {
            if (WinApi.IsWindows && !WinApi.IsGlassEnabled)
            {
                FormBorderStyle = FormBorderStyle.None;
                SetStyle(ControlStyles.ResizeRedraw, true);
                SetStyle(ControlStyles.Opaque, WinApi.IsGlassEnabled);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                DoubleBuffered = true;
            }
            _helper = new RibbonFormHelper(this);
        }

        #endregion

        #region Overrides

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                if (WinApi.IsWindows && !WinApi.IsGlassEnabled)
                {
                    cp.Style |= 0x20000 | 0x80000 | 0x40000; //WS_MINIMIZEBOX | WS_SYSMENU | WS_SIZEBOX;
                    //cp.ClassStyle |= 0x8 | 0x20000; //CS_DBLCLKS | CS_DROPSHADOW;
                }
                return cp;
            }
        }

        /// <summary>
        ///     Just for debugging messages
        /// </summary>
        /// <param name="m"></param>
        protected override void OnNotifyMessage(Message m)
        {
            base.OnNotifyMessage(m);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // override OnPaint and do NOT call base, otherwise problems as MDI parent occur
            _helper.Form_Paint(this, e);
        }

        /// <summary>
        ///     Overrides the WndProc funciton
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (!Helper.WndProc(ref m))
            {
                base.WndProc(ref m);
            }
        }

        #endregion

        #region IRibbonForm Members

        /// <summary>
        ///     Gets the helper for making the form a ribbon form
        /// </summary>
        public RibbonFormHelper Helper
        {
            get { return _helper; }
        }

        #endregion
    }
}