#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Collections;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Designers;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Enums;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Interfaces;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Renderers;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon
{
    /// <summary>
    ///     Represents a quick access toolbar hosted on the Ribbon
    /// </summary>
    public class RibbonQuickAccessToolbar : RibbonItem,
        IContainsSelectableRibbonItems, IContainsRibbonComponents
    {
        #region Fields

        private readonly RibbonButton _dropDownButton;
        private readonly RibbonQuickAccessToolbarItemCollection _items;
        private readonly Padding _margin;
        private readonly Padding _padding;
        private readonly RibbonMouseSensor _sensor;
        private bool _DropDownButtonVisible;

        #endregion

        #region Ctor

        internal RibbonQuickAccessToolbar(Ribbon ownerRibbon)
        {
            if (ownerRibbon == null) throw new ArgumentNullException("ownerRibbon");

            SetOwner(ownerRibbon);

            _dropDownButton = new RibbonButton();
            _dropDownButton.SetOwner(ownerRibbon);
            _dropDownButton.SmallImage = CreateDropDownButtonImage();
            _dropDownButton.Style = RibbonButtonStyle.DropDown;

            _margin = new Padding(9);
            _padding = new Padding(3, 0, 0, 0);
            _items = new RibbonQuickAccessToolbarItemCollection(this);
            _sensor = new RibbonMouseSensor(ownerRibbon, ownerRibbon, Items);
            _DropDownButtonVisible = true;
        }

        private Image CreateDropDownButtonImage()
        {
            var bmp = new Bitmap(7, 7);
            var renderer = Owner.Renderer as RibbonProfessionalRenderer;

            var dk = Color.Navy;
            var lt = Color.White;

            if (renderer != null)
            {
                dk = Theme.ColorTable.Arrow;
                lt = Theme.ColorTable.ArrowLight;
            }

            using (var g = Graphics.FromImage(bmp))
            {
                DrawDropDownButtonArrow(g, lt, 0, 1);
                DrawDropDownButtonArrow(g, dk, 0, 0);
            }


            return bmp;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && RibbonDesigner.Current == null)
            {
                foreach (var item in _items)
                    item.Dispose();
                _dropDownButton.Dispose();
                _sensor.Dispose();
            }
            base.Dispose(disposing);
        }

        private void DrawDropDownButtonArrow(Graphics g, Color c, int x, int y)
        {
            using (var p = new Pen(c))
            {
                using (var b = new SolidBrush(c))
                {
                    g.DrawLine(p, x, y, x + 4, y);
                    g.FillPolygon(b, new[]
                    {
                        new Point(x, y + 3),
                        new Point(x + 5, y + 3),
                        new Point(x + 2, y + 6)
                    });
                }
            }
        }

        #endregion

        #region Properties

        [Description("Shows or hides the dropdown button of the toolbar")]
        [DefaultValue(true)]
        public bool DropDownButtonVisible
        {
            get { return _DropDownButtonVisible; }
            set
            {
                _DropDownButtonVisible = value;
                Owner.OnRegionsChanged();
            }
        }


        /// <summary>
        ///     Gets the bounds of the toolbar including the graphic adornments
        /// </summary>
        [Browsable(false)]
        internal Rectangle SuperBounds
        {
            get { return Rectangle.FromLTRB(Bounds.Left - Padding.Horizontal, Bounds.Top, DropDownButton.Bounds.Right, Bounds.Bottom); }
        }

        /// <summary>
        ///     Gets the dropdown button
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RibbonButton DropDownButton
        {
            get { return _dropDownButton; }
        }

        [Description("The drop down items of the dropdown button of the toolbar")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RibbonItemCollection DropDownButtonItems
        {
            get { return _dropDownButton.DropDownItems; }
        }

        /// <summary>
        ///     Gets or sets the padding (internal) of the toolbar
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Padding Padding
        {
            get { return _padding; }
        }

        /// <summary>
        ///     Gets or sets the margin (external) of the toolbar
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Padding Margin
        {
            get { return _margin; }
        }

        /// <summary>
        ///     Gets or sets a value indicating if the button that shows the menu of the
        ///     QuickAccess toolbar should be visible
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool MenuButtonVisible { get; set; }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RibbonMouseSensor Sensor
        {
            get { return _sensor; }
        }

        /// <summary>
        ///     Gets the Items of the QuickAccess toolbar.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RibbonQuickAccessToolbarItemCollection Items
        {
            get
            {
                if (DropDownButtonVisible)
                {
                    if (!_items.Contains(DropDownButton))
                    {
                        _items.Add(DropDownButton);
                    }
                }
                else
                {
                    if (_items.Contains(DropDownButton))
                    {
                        _items.Remove(DropDownButton);
                    }
                }
                return _items;
            }
        }

        #endregion

        #region Methods

        public override Size MeasureSize(object sender, RibbonElementMeasureSizeEventArgs e)
        {
            ///For RibbonItemGroup, size is always compact, and it's designed to be on an horizontal flow
            ///tab panel.
            ///
            if (!Visible || !Owner.CaptionBarVisible)
            {
                SetLastMeasuredSize(new Size(0, 0));
                return LastMeasuredSize;
            }

            var widthSum = Padding.Horizontal;
            var maxHeight = 16;

            foreach (var item in Items)
            {
                if (item.Equals(DropDownButton)) continue;
                item.SetSizeMode(RibbonElementSizeMode.Compact);
                var s = item.MeasureSize(this, new RibbonElementMeasureSizeEventArgs(e.Graphics, RibbonElementSizeMode.Compact));
                widthSum += s.Width + 1;
                maxHeight = Math.Max(maxHeight, s.Height);
            }

            widthSum -= 1;

            if (Site != null && Site.DesignMode) widthSum += 16;

            var result = new Size(widthSum, maxHeight);
            SetLastMeasuredSize(result);
            return result;
        }

        public override void OnPaint(object sender, RibbonElementPaintEventArgs e)
        {
            if (Visible && Owner.CaptionBarVisible)
            {
                if (Owner.OrbStyle == RibbonOrbStyle.Office_2007)
                    Owner.Renderer.OnRenderRibbonQuickAccessToolbarBackground(new RibbonRenderEventArgs(Owner, e.Graphics, e.Clip));

                foreach (var item in Items)
                {
                    item.OnPaint(this, new RibbonElementPaintEventArgs(item.Bounds, e.Graphics, RibbonElementSizeMode.Compact));
                }
            }
        }

        public override void SetBounds(Rectangle bounds)
        {
            base.SetBounds(bounds);

            if (Owner.RightToLeft == RightToLeft.No)
            {
                var curLeft = bounds.Left + Padding.Left;

                foreach (var item in Items)
                {
                    item.SetBounds(new Rectangle(new Point(curLeft, bounds.Top), item.LastMeasuredSize));

                    curLeft = item.Bounds.Right + 1;
                }

                DropDownButton.SetBounds(new Rectangle(bounds.Right + bounds.Height/2 + 2, bounds.Top, 12, bounds.Height));
            }
            else
            {
                var curLeft = bounds.Left + Padding.Left;

                for (var i = Items.Count - 1; i >= 0; i--)
                {
                    Items[i].SetBounds(new Rectangle(new Point(curLeft, bounds.Top), Items[i].LastMeasuredSize));

                    curLeft = Items[i].Bounds.Right + 1;
                }

                DropDownButton.SetBounds(new Rectangle(bounds.Left - bounds.Height/2 - 14, bounds.Top, 12, bounds.Height));
            }
        }

        #endregion

        #region IContainsRibbonComponents Members

        public IEnumerable<Component> GetAllChildComponents()
        {
            return Items.ToArray();
        }

        #endregion

        #region IContainsSelectableRibbonItems Members

        public IEnumerable<RibbonItem> GetItems()
        {
            return Items;
        }

        public Rectangle GetContentBounds()
        {
            return Bounds;
        }

        #endregion
    }
}