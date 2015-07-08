#region

using System.ComponentModel;
using System.Drawing;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Enums;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs;

#endregion

namespace System.Windows.Forms
{
    public sealed class RibbonSeparator : RibbonItem
    {
        private bool _drawBackground = true;

        public RibbonSeparator()
        {
        }

        public RibbonSeparator(string text)
        {
            Text = text;
        }

        /// <summary>
        ///     Gets or sets a value indicating if the separator should draw the divider lines
        /// </summary>
        [DefaultValue(true)]
        [Description("Background drawing should be avoided when group contains only TextBoxes and ComboBoxes")]
        public bool DrawBackground
        {
            get { return _drawBackground; }
            set { _drawBackground = value; }
        }

        public override Size MeasureSize(object sender, RibbonElementMeasureSizeEventArgs e)
        {
            if (e.SizeMode == RibbonElementSizeMode.DropDown)
            {
                if (string.IsNullOrEmpty(Text))
                {
                    SetLastMeasuredSize(new Size(1, 3));
                }
                else
                {
                    var sz = e.Graphics.MeasureString(Text, new Font(Owner.Font, FontStyle.Bold)).ToSize();
                    SetLastMeasuredSize(new Size(sz.Width + Owner.ItemMargin.Horizontal, sz.Height + Owner.ItemMargin.Vertical));
                }
            }
            else
            {
                SetLastMeasuredSize(new Size(2, OwnerPanel.ContentBounds.Height - Owner.ItemPadding.Vertical - Owner.ItemMargin.Vertical));
            }

            return LastMeasuredSize;
        }

        public override void OnPaint(object sender, RibbonElementPaintEventArgs e)
        {
            if ((Owner == null || !DrawBackground) && !Owner.IsDesignMode())
                return;

            Owner.Renderer.OnRenderRibbonItem(new RibbonItemRenderEventArgs(
                Owner, e.Graphics, e.Clip, this));

            if (!string.IsNullOrEmpty(Text))
            {
                Owner.Renderer.OnRenderRibbonItemText(new RibbonTextEventArgs(
                    Owner, e.Graphics, e.Clip, this,
                    Rectangle.FromLTRB(
                        Bounds.Left + Owner.ItemMargin.Left,
                        Bounds.Top + Owner.ItemMargin.Top,
                        Bounds.Right - Owner.ItemMargin.Right,
                        Bounds.Bottom - Owner.ItemMargin.Bottom), Text, FontStyle.Bold));
            }
        }

        public override void SetBounds(Rectangle bounds)
        {
            base.SetBounds(bounds);
        }
    }
}