#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Designers;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Renderers;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon
{
    [ToolboxItem(false)]
    public class RibbonPopup
        : Control
    {
        #region Fields

        #endregion

        #region Events

        public event EventHandler Showed;

        /// <summary>
        ///     Raised when the popup is closed
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        ///     Raised when the popup is about to be closed
        /// </summary>
        public event ToolStripDropDownClosingEventHandler Closing;

        /// <summary>
        ///     Raised when the Popup is about to be opened
        /// </summary>
        public event CancelEventHandler Opening;

        #endregion

        #region Ctor

        public RibbonPopup()
        {
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.Selectable, false);
            BorderRoundness = 3;
        }

        #endregion

        #region Props

        /// <summary>
        ///     Gets or sets the roundness of the border
        /// </summary>
        [Browsable(false)]
        public int BorderRoundness { get; set; }


        /// <summary>
        ///     Gets the related ToolStripDropDown
        /// </summary>
        internal RibbonWrappedDropDown WrappedDropDown { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Overriden. Used to drop a shadow on the popup
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;

                if (WinApi.IsXP)
                {
                    cp.ClassStyle |= WinApi.CS_DROPSHADOW;
                }

                return cp;
            }
        }

        /// <summary>
        ///     Closes this popup.
        /// </summary>
        public void Close()
        {
            if (WrappedDropDown != null)
            {
                WrappedDropDown.Close();
            }
        }

        /// <summary>
        ///     Raises the <see cref="Closed" /> event.
        ///     <remarks>
        ///         If you override this event don't forget to call base! Otherwise the popup will not be unregistered and
        ///         hook will not work!
        ///     </remarks>
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnClosed(EventArgs e)
        {
            RibbonPopupManager.Unregister(this);

            if (Closed != null)
            {
                Closed(this, e);
            }

            //if (NextPopup != null)
            //{
            //    NextPopup.CloseForward();
            //    NextPopup = null;
            //}

            //if (PreviousPopup != null && PreviousPopup.NextPopup.Equals(this))
            //{
            //    PreviousPopup.NextPopup = null;
            //}
        }

        /// <summary>
        ///     Raises the <see cref="Closing" /> event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnClosing(ToolStripDropDownClosingEventArgs e)
        {
            if (Closing != null)
            {
                Closing(this, e);
            }
        }

        /// <summary>
        ///     Called when pop-up is being opened
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnOpening(CancelEventArgs e)
        {
            if (Opening != null)
            {
                Opening(this, e);
            }
        }

        /// <summary>
        ///     Raises the <see cref="Paint" /> event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (GraphicsPath p = RibbonProfessionalRenderer.RoundRectangle(new Rectangle(Point.Empty, Size), BorderRoundness))
            {
                using (var r = new Region(p))
                {
                    WrappedDropDown.Region = r;
                }
            }
        }

        /// <summary>
        ///     Raises the Showed event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnShowed(EventArgs e)
        {
            if (Showed != null)
            {
                Showed(this, e);
            }
        }

        /// <summary>
        ///     Shows this Popup on the specified location of the screen
        /// </summary>
        /// <param name="screenLocation"></param>
        public void Show(Point screenLocation)
        {
            if (WrappedDropDown == null)
            {
                var host = new ToolStripControlHost(this);
                WrappedDropDown = new RibbonWrappedDropDown();
                WrappedDropDown.AutoClose = RibbonDesigner.Current != null;
                WrappedDropDown.Items.Add(host);

                WrappedDropDown.Padding = Padding.Empty;
                WrappedDropDown.Margin = Padding.Empty;
                host.Padding = Padding.Empty;
                host.Margin = Padding.Empty;

                WrappedDropDown.Opening += ToolStripDropDown_Opening;
                WrappedDropDown.Closing += ToolStripDropDown_Closing;
                WrappedDropDown.Closed += ToolStripDropDown_Closed;
                WrappedDropDown.Size = Size;
            }
            WrappedDropDown.Show(screenLocation);
            RibbonPopupManager.Register(this);

            OnShowed(EventArgs.Empty);
        }

        /// <summary>
        ///     Handles the closed event of the ToolStripDropDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripDropDown_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            OnClosed(EventArgs.Empty);
        }

        /// <summary>
        ///     Handles the Closing event of the ToolStripDropDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripDropDown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            OnClosing(e);
        }

        /// <summary>
        ///     Handles the Opening event of the ToolStripDropDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripDropDown_Opening(object sender, CancelEventArgs e)
        {
            OnOpening(e);
        }

        #endregion
    }
}