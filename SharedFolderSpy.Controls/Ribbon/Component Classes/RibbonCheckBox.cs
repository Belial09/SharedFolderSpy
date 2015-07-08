#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Enums;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon
{
    public class RibbonCheckBox
        : RibbonItem
    {
        public enum CheckBoxOrientationEnum
        {
            Left = 0,
            Right = 1
        }

        public enum CheckBoxStyle
        {
            CheckBox,
            RadioButton
        }

        #region Fields

        private const int spacing = 3;
        private CheckBoxOrientationEnum _CheckBoxOrientation;
        private Rectangle _checkboxBounds;
        private int _checkboxSize;
        private Rectangle _labelBounds;
        private bool _labelVisible = true;
        private int _labelWidth;
        private CheckBoxStyle _style;

        #endregion

        #region Events

        /// <summary>
        ///     Raised when the <see cref="CheckBox" /> property value has changed
        /// </summary>
        public event EventHandler CheckBoxCheckChanged;

        /// <summary>
        ///     Raised when the <see cref="CheckBox Checked" /> property value is changing
        /// </summary>
        public event CancelEventHandler CheckBoxCheckChanging;

        #endregion

        #region Ctor

        public RibbonCheckBox()
        {
            _checkboxSize = 16;
        }

        #endregion

        #region Props

        /// <summary>
        ///     Gets or sets the style of the checkbox item
        /// </summary>
        [DefaultValue(CheckBoxStyle.CheckBox)]
        public CheckBoxStyle Style
        {
            get { return _style; }
            set
            {
                _style = value;
                NotifyOwnerRegionsChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the width of the Label
        /// </summary>
        [DefaultValue(CheckBoxOrientationEnum.Left)]
        public CheckBoxOrientationEnum CheckBoxOrientation
        {
            get { return _CheckBoxOrientation; }
            set
            {
                _CheckBoxOrientation = value;
                NotifyOwnerRegionsChanged();
            }
        }

        /// <summary>
        ///     Gets the bounds of the label that is shown next to the textbox
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Rectangle LabelBounds
        {
            get { return _labelBounds; }
        }

        /// <summary>
        ///     Gets a value indicating if the label is currently visible
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool LabelVisible
        {
            get { return _labelVisible; }
        }

        /// <summary>
        ///     Gets the bounds of the text
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Rectangle CheckBoxBounds
        {
            get { return _checkboxBounds; }
        }

        /// <summary>
        ///     Gets or sets the width of the Label
        /// </summary>
        [DefaultValue(0)]
        public int LabelWidth
        {
            get { return _labelWidth; }
            set
            {
                _labelWidth = value;
                NotifyOwnerRegionsChanged();
            }
        }

        #endregion

        #region Methods

        private bool checkedGlyphSize;

        /// <summary>
        ///     Measures the suposed height of the boxbox
        /// </summary>
        /// <returns></returns>
        protected virtual int MeasureHeight()
        {
            return _checkboxSize + Owner.ItemMargin.Vertical;
        }

        public override Size MeasureSize(object sender, RibbonElementMeasureSizeEventArgs e)
        {
            if (!checkedGlyphSize)
            {
                try
                {
                    if (Style == CheckBoxStyle.CheckBox)
                        _checkboxSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, Checked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal).Height + spacing;
                    else
                        _checkboxSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, Checked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal).Height + spacing;
                }
                catch
                {
                    /* I don't mind at all */
                }
                checkedGlyphSize = true;
            }

            if (!Visible && !Owner.IsDesignMode())
            {
                SetLastMeasuredSize(new Size(0, 0));
                return LastMeasuredSize;
            }

            var size = Size.Empty;

            var w = Owner.ItemMargin.Horizontal;
            var iwidth = Image != null ? Image.Width + spacing : 0;
            var lwidth = string.IsNullOrEmpty(Text) ? 0 : _labelWidth > 0 ? _labelWidth : e.Graphics.MeasureString(Text, Owner.Font).ToSize().Width;

            w += _checkboxSize;

            switch (e.SizeMode)
            {
                case RibbonElementSizeMode.Large:
                    w += iwidth + lwidth;
                    break;
                case RibbonElementSizeMode.Medium:
                    w += iwidth;
                    break;
            }

            SetLastMeasuredSize(new Size(w, MeasureHeight()));

            return LastMeasuredSize;
        }

        /// <summary>
        ///     Raises the <see cref="CheckBox Check Changed" /> event
        /// </summary>
        /// <param name="e"></param>
        public void OnCheckChanged(EventArgs e)
        {
            if (!Enabled) return;

            NotifyOwnerRegionsChanged();

            if (CheckBoxCheckChanged != null)
            {
                CheckBoxCheckChanged(this, e);
            }
        }

        /// <summary>
        ///     Raises the <see cref="CheckBox Check Changing" /> event
        /// </summary>
        /// <param name="e"></param>
        public void OnCheckChanging(CancelEventArgs e)
        {
            if (!Enabled) return;

            if (CheckBoxCheckChanging != null)
            {
                CheckBoxCheckChanging(this, e);
            }
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            if (!Enabled) return;

            base.OnMouseDown(e);

            if (CheckBoxBounds.Contains(e.X, e.Y))
            {
                var cev = new CancelEventArgs();
                OnCheckChanging(cev);
                if (!cev.Cancel)
                {
                    Checked = !Checked;
                    OnCheckChanged(e);
                }
            }
        }

        public override void OnMouseEnter(MouseEventArgs e)
        {
            if (!Enabled) return;

            base.OnMouseEnter(e);

            //Canvas.Cursor = Cursors.Default;
            //SetSelected(true);
        }

        public override void OnMouseLeave(MouseEventArgs e)
        {
            if (!Enabled) return;

            base.OnMouseLeave(e);

            //Canvas.Cursor = Cursors.Default;
            //SetSelected(false);
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (!Enabled) return;

            base.OnMouseMove(e);

            if (CheckBoxBounds.Contains(e.X, e.Y))
            {
                Debug.WriteLine("Owner.Cursor = Cursors.Hand e.X=" + e.X + " e.Y=" + e.Y + " CheckBoxBounds (" + CheckBoxBounds + ")");
                Owner.Cursor = Cursors.Hand;

                if (!Selected)
                    SetSelected(true);
            }
            else
            {
                Debug.WriteLine("Owner.Cursor = Cursors.Default e.X=" + e.X + " e.Y=" + e.Y + " CheckBoxBounds (" + CheckBoxBounds + ")");
                Owner.Cursor = Cursors.Default;

                if (Selected)
                    SetSelected(false);
            }
        }

        public override void OnPaint(object sender, RibbonElementPaintEventArgs e)
        {
            if (Owner != null)
            {
                Owner.Renderer.OnRenderRibbonItem(new RibbonItemRenderEventArgs(Owner, e.Graphics, Bounds, this));

                if (Style == CheckBoxStyle.CheckBox)
                {
                    var CheckState = Checked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal;
                    if (Selected)
                        CheckState += 1;

                    if (CheckBoxOrientation == CheckBoxOrientationEnum.Left)
                        CheckBoxRenderer.DrawCheckBox(e.Graphics, new Point(_checkboxBounds.Left, _checkboxBounds.Top), CheckState);
                    else
                        CheckBoxRenderer.DrawCheckBox(e.Graphics, new Point(_checkboxBounds.Left + spacing, _checkboxBounds.Top), CheckState);
                }
                else
                {
                    var RadioState = Checked ? RadioButtonState.CheckedNormal : RadioButtonState.UncheckedNormal;
                    if (Selected)
                        RadioState += 1;
                    if (CheckBoxOrientation == CheckBoxOrientationEnum.Left)
                        RadioButtonRenderer.DrawRadioButton(e.Graphics, new Point(_checkboxBounds.Left, _checkboxBounds.Top), RadioState);
                    else
                        RadioButtonRenderer.DrawRadioButton(e.Graphics, new Point(_checkboxBounds.Left + spacing, _checkboxBounds.Top), RadioState);
                }

                if (LabelVisible)
                {
                    var f = new StringFormat();
                    if (_CheckBoxOrientation == CheckBoxOrientationEnum.Left)
                        f.Alignment = StringAlignment.Near;
                    else
                        f.Alignment = StringAlignment.Far;

                    f.LineAlignment = StringAlignment.Far; //Top
                    f.Trimming = StringTrimming.None;
                    f.FormatFlags |= StringFormatFlags.NoWrap;
                    Owner.Renderer.OnRenderRibbonItemText(new RibbonTextEventArgs(Owner, e.Graphics, _labelBounds, this, LabelBounds, Text, f));
                }
            }
        }

        public override void SetBounds(Rectangle bounds)
        {
            base.SetBounds(bounds);
            if (CheckBoxOrientation == CheckBoxOrientationEnum.Left)
            {
                _checkboxBounds = new Rectangle(
                    bounds.Left + Owner.ItemMargin.Left,
                    bounds.Top + Owner.ItemMargin.Top + ((bounds.Height - _checkboxSize)/2),
                    _checkboxSize,
                    _checkboxSize);

                _labelBounds = Rectangle.FromLTRB(
                    _checkboxBounds.Right,
                    bounds.Top + Owner.ItemMargin.Top,
                    bounds.Right - Owner.ItemMargin.Right,
                    bounds.Bottom - Owner.ItemMargin.Bottom);
            }
            else
            {
                _checkboxBounds = new Rectangle(
                    bounds.Right - Owner.ItemMargin.Right - _checkboxSize,
                    bounds.Top + Owner.ItemMargin.Top + ((bounds.Height - _checkboxSize)/2),
                    _checkboxSize,
                    _checkboxSize);

                _labelBounds = Rectangle.FromLTRB(
                    bounds.Left + Owner.ItemMargin.Left,
                    bounds.Top + Owner.ItemMargin.Top,
                    _checkboxBounds.Left,
                    bounds.Bottom - Owner.ItemMargin.Bottom);
            }

            if (SizeMode == RibbonElementSizeMode.Large)
            {
                _labelVisible = true;
            }
            else if (SizeMode == RibbonElementSizeMode.Medium)
            {
                _labelVisible = true;
                _labelBounds = Rectangle.Empty;
            }
            else if (SizeMode == RibbonElementSizeMode.Compact)
            {
                _labelBounds = Rectangle.Empty;
                _labelVisible = false;
            }
        }

        #endregion
    }
}