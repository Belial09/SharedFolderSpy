#region

using System.ComponentModel;
using System.Drawing;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs;

#endregion

namespace System.Windows.Forms
{
    public class RibbonLabel : RibbonItem
    {
        #region Fields

        private const int spacing = 3;
        private int _labelWidth;

        #endregion

        #region Methods

        protected virtual int MeasureHeight()
        {
            if (Owner != null)
                return 16 + Owner.ItemMargin.Vertical;
            return 16 + 4;
        }

        /// <summary>
        ///     Measures the size of the panel on the mode specified by the event object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public override Size MeasureSize(object sender, RibbonElementMeasureSizeEventArgs e)
        {
            if (!Visible && ((Site == null) || !Site.DesignMode))
            {
                return new Size(0, 0);
            }
            var f = new Font("Microsoft Sans Serif", 8);
            if (Owner != null)
                f = Owner.Font;

            var w = string.IsNullOrEmpty(Text) ? 0 : ((_labelWidth > 0) ? _labelWidth : (e.Graphics.MeasureString(Text, f).ToSize().Width + 6));
            base.SetLastMeasuredSize(new Size(w, MeasureHeight()));
            return base.LastMeasuredSize;
        }

        /// <summary>
        ///     Raises the paint event and draws the
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnPaint(object sender, RibbonElementPaintEventArgs e)
        {
            if (Owner != null)
            {
                base.Owner.Renderer.OnRenderRibbonItem(new RibbonItemRenderEventArgs(base.Owner, e.Graphics, base.Bounds, this));
                var f = new StringFormat();
                f.Alignment = (StringAlignment) TextAlignment;
                f.LineAlignment = StringAlignment.Center;
                f.Trimming = StringTrimming.None;
                f.FormatFlags |= StringFormatFlags.NoWrap;
                var clipBounds = Rectangle.FromLTRB(Bounds.Left + 3, Bounds.Top + Owner.ItemMargin.Top, Bounds.Right - 3, Bounds.Bottom - Owner.ItemMargin.Bottom);
                base.Owner.Renderer.OnRenderRibbonItemText(new RibbonTextEventArgs(Owner, e.Graphics, Bounds, this, clipBounds, Text, f));
            }
        }

        /// <summary>
        ///     Sets the bounds of the panel
        /// </summary>
        /// <param name="bounds"></param>
        public override void SetBounds(Rectangle bounds)
        {
            base.SetBounds(bounds);
        }

        #endregion

        #region Properties

        [Description("Sets the width of the label portion of the control")]
        [DefaultValue(0)]
        public int LabelWidth
        {
            get { return _labelWidth; }
            set
            {
                _labelWidth = value;
                base.NotifyOwnerRegionsChanged();
            }
        }

        #endregion
    }
}