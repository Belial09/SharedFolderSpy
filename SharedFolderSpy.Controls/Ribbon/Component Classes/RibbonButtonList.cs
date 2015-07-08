#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Collections;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Designers;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Enums;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Interfaces;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon
{
    /// <summary>
    ///     Represents a list of buttons that can be navigated
    /// </summary>
    [Designer(typeof (RibbonButtonListDesigner))]
    public sealed class RibbonButtonList : RibbonItem,
        IContainsSelectableRibbonItems, IScrollableRibbonItem, IContainsRibbonComponents
    {
        #region Subtypes

        public enum ListScrollType
        {
            UpDownButtons,

            Scrollbar
        }

        #endregion

        #region Fields

        public delegate void RibbonItemEventHandler(object sender, RibbonItemEventArgs e);

        private readonly RibbonButtonCollection _buttons;
        private readonly RibbonItemCollection _dropDownItems;
        private Size _ItemsInDropwDownMode;
        private bool _avoidNextThumbMeasure;
        private Rectangle _buttonDownBounds;
        private bool _buttonDownEnabled;
        private bool _buttonDownPressed;
        private bool _buttonDownSelected;
        private Rectangle _buttonDropDownBounds;
        private bool _buttonDropDownPressed;
        private bool _buttonDropDownSelected;
        private Rectangle _buttonUpBounds;
        private bool _buttonUpEnabled;
        private bool _buttonUpPressed;
        private bool _buttonUpSelected;
        private RibbonElementSizeMode _buttonsSizeMode;
        private Rectangle _contentBounds;
        private int _controlButtonsWidth;
        private RibbonDropDown _dropDown;
        private bool _dropDownVisible;
        private int _itemsInLargeMode;
        private int _itemsInMediumMode;
        private int _jumpDownSize;
        private int _jumpUpSize;
        private int _offset;
        private bool _scrollBarEnabled;
        private ListScrollType _scrollType;
        private int _scrollValue;
        private RibbonItem _selectedItem;
        private Rectangle _thumbBounds;
        private int _thumbOffset;
        private bool _thumbPressed;
        private bool _thumbSelected;
        private Rectangle fullContentBounds;

        public event RibbonItemEventHandler ButtonItemClicked;
        public event RibbonItemEventHandler DropDownItemClicked;

        #endregion

        #region Ctor

        public RibbonButtonList()
        {
            _buttons = new RibbonButtonCollection(this);
            _dropDownItems = new RibbonItemCollection();

            _controlButtonsWidth = 16;
            _itemsInLargeMode = 7;
            _itemsInMediumMode = 3;
            _ItemsInDropwDownMode = new Size(7, 5);
            _buttonsSizeMode = RibbonElementSizeMode.Large;
            _scrollType = ListScrollType.UpDownButtons;
        }

        public RibbonButtonList(IEnumerable<RibbonButton> buttons)
            : this(buttons, null)
        {
        }

        public RibbonButtonList(IEnumerable<RibbonButton> buttons, IEnumerable<RibbonItem> dropDownItems)
            : this()
        {
            if (buttons != null)
            {
                var items = new List<RibbonButton>(buttons);

                _buttons.AddRange(items.ToArray());

                //add the handlers
                foreach (RibbonItem item in buttons)
                {
                    item.Click += item_Click;
                }
            }

            if (dropDownItems != null)
            {
                _dropDownItems.AddRange(dropDownItems);

                //add the handlers
                foreach (var item in dropDownItems)
                {
                    item.Click += item_Click;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var item in Buttons)
                {
                    item.Click -= item_Click;
                }
                foreach (var item in DropDownItems)
                {
                    item.Click -= item_Click;
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Props

        [Description("If activated, buttons will flow to bottom inside the list")]
        public bool FlowToBottom { get; set; }


        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle ScrollBarBounds
        {
            get { return Rectangle.FromLTRB(ButtonUpBounds.Left, ButtonUpBounds.Top, ButtonDownBounds.Right, ButtonDownBounds.Bottom); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ScrollBarEnabled
        {
            get { return _scrollBarEnabled; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ListScrollType ScrollType
        {
            get { return _scrollType; }
        }

        /// <summary>
        ///     Gets the percent of scrolled content
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double ScrolledPercent
        {
            get
            {
                return (ContentBounds.Top - (double) fullContentBounds.Top)/
                       (fullContentBounds.Height - (double) ContentBounds.Height);
            }
            set
            {
                _avoidNextThumbMeasure = true;
                ScrollTo(-Convert.ToInt32((fullContentBounds.Height - ContentBounds.Height)*value));
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ScrollMinimum
        {
            get
            {
                if (ScrollType == ListScrollType.Scrollbar)
                {
                    return ButtonUpBounds.Bottom;
                }
                return 0;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ScrollMaximum
        {
            get
            {
                if (ScrollType == ListScrollType.Scrollbar)
                {
                    //return ButtonDownBounds.Top - ThumbBounds.Height;
                    return ButtonDownBounds.Top - ThumbBounds.Height;
                }
                return 0;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ScrollValue
        {
            get { return _scrollValue; }
            set
            {
                if (value > ScrollMaximum || value < ScrollMinimum)
                {
                    throw new IndexOutOfRangeException("Scroll value must exist between ScrollMinimum and Scroll Maximum");
                }

                _thumbBounds.Y = value;

                double scrolledPixels = value - ScrollMinimum;
                double pixelsAvailable = ScrollMaximum - ScrollMinimum;

                ScrolledPercent = scrolledPixels/pixelsAvailable;

                _scrollValue = value;
            }
        }

        /// <summary>
        ///     Gets if the scrollbar thumb is currently selected
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ThumbSelected
        {
            get { return _thumbSelected; }
        }

        /// <summary>
        ///     Gets if the scrollbar thumb is currently pressed
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ThumbPressed
        {
            get { return _thumbPressed; }
        }


        /// <summary>
        ///     Gets the bounds of the scrollbar thumb
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle ThumbBounds
        {
            get { return _thumbBounds; }
        }


        /// <summary>
        ///     Gets if the DropDown button is present on thelist
        /// </summary>
        public bool ButtonDropDownPresent
        {
            get { return ButtonDropDownBounds.Height > 0; }
        }

        /// <summary>
        ///     Gets the collection of items shown on the dropdown pop-up when Style allows it
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RibbonItemCollection DropDownItems
        {
            get { return _dropDownItems; }
        }

        /// <summary>
        ///     Gets or sets the size that the buttons on the list should be
        /// </summary>
        public RibbonElementSizeMode ButtonsSizeMode
        {
            get { return _buttonsSizeMode; }
            set
            {
                _buttonsSizeMode = value;
                if (Owner != null) Owner.OnRegionsChanged();
            }
        }

        /// <summary>
        ///     Gets a value indicating if the button that scrolls up the content is currently enabled
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ButtonUpEnabled
        {
            get { return _buttonUpEnabled; }
        }

        /// <summary>
        ///     Gets a value indicating if the button that scrolls down the content is currently enabled
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ButtonDownEnabled
        {
            get { return _buttonDownEnabled; }
        }

        /// <summary>
        ///     Gets a value indicating if the DropDown button is currently selected
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ButtonDropDownSelected
        {
            get { return _buttonDropDownSelected; }
        }

        /// <summary>
        ///     Gets a value indicating if the DropDown button is currently pressed
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ButtonDropDownPressed
        {
            get { return _buttonDropDownPressed; }
        }

        /// <summary>
        ///     Gets a vaule indicating if the button that scrolls down the content is currently selected
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ButtonDownSelected
        {
            get { return _buttonDownSelected; }
        }

        /// <summary>
        ///     Gets a vaule indicating if the button that scrolls down the content is currently pressed
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ButtonDownPressed
        {
            get { return _buttonDownPressed; }
        }

        /// <summary>
        ///     Gets a vaule indicating if the button that scrolls up the content is currently selected
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ButtonUpSelected
        {
            get { return _buttonUpSelected; }
        }

        /// <summary>
        ///     Gets a vaule indicating if the button that scrolls up the content is currently pressed
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ButtonUpPressed
        {
            get { return _buttonUpPressed; }
        }

        /// <summary>
        ///     Gets the bounds of the button that scrolls the items up
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle ButtonUpBounds
        {
            get { return _buttonUpBounds; }
        }

        /// <summary>
        ///     Gets the bounds of the button that scrolls the items down
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle ButtonDownBounds
        {
            get { return _buttonDownBounds; }
        }

        /// <summary>
        ///     Gets the bounds of the button that scrolls
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle ButtonDropDownBounds
        {
            get { return _buttonDropDownBounds; }
        }

        /// <summary>
        ///     Gets or sets the with of the buttons that allow to navigate thru the list
        /// </summary>
        [DefaultValue(16)]
        public int ControlButtonsWidth
        {
            get { return _controlButtonsWidth; }
            set
            {
                _controlButtonsWidth = value;
                if (Owner != null) Owner.OnRegionsChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating the amount of items to show
        ///     (wide) when SizeMode is Large
        /// </summary>
        [DefaultValue(7)]
        public int ItemsWideInLargeMode
        {
            get { return _itemsInLargeMode; }
            set
            {
                _itemsInLargeMode = value;
                if (Owner != null) Owner.OnRegionsChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating the amount of items to show
        ///     (wide) when SizeMode is Medium
        /// </summary>
        [DefaultValue(3)]
        public int ItemsWideInMediumMode
        {
            get { return _itemsInMediumMode; }
            set
            {
                _itemsInMediumMode = value;
                if (Owner != null) Owner.OnRegionsChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating the amount of items to show
        ///     (wide) when SizeMode is Medium
        /// </summary>
        public Size ItemsSizeInDropwDownMode
        {
            get { return _ItemsInDropwDownMode; }
            set
            {
                _ItemsInDropwDownMode = value;
                if (Owner != null) Owner.OnRegionsChanged();
            }
        }

        /// <summary>
        ///     Gets the collection of buttons of the list
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RibbonButtonCollection Buttons
        {
            get { return _buttons; }
        }

        /// <summary>
        ///     Gets the bounds of the content where items are shown
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Rectangle ContentBounds
        {
            get { return _contentBounds; }
        }

        /// <summary>
        ///     Redraws the scroll part of the list
        /// </summary>
        private void RedrawScroll()
        {
            if (Canvas != null)
                Canvas.Invalidate(Rectangle.FromLTRB(ButtonDownBounds.X, ButtonUpBounds.Y, ButtonDownBounds.Right, ButtonDownBounds.Bottom));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Scrolls the list down
        /// </summary>
        public void ScrollDown()
        {
            ScrollOffset(-(_jumpDownSize + 1));
        }

        /// <summary>
        ///     Scrolls the list up
        /// </summary>
        public void ScrollUp()
        {
            ScrollOffset(_jumpDownSize + 1);
        }

        /// <summary>
        ///     Closes the DropDown if opened
        /// </summary>
        public void CloseDropDown()
        {
            if (_dropDown != null)
            {
                //RibbonDropDown.DismissTo(_dropDown);
            }

            SetDropDownVisible(false);
        }

        /// <summary>
        ///     Ignores deactivation of canvas if it is a volatile window
        /// </summary>
        private void IgnoreDeactivation()
        {
            if (Canvas is RibbonPanelPopup)
            {
                (Canvas as RibbonPanelPopup).IgnoreNextClickDeactivation();
            }

            if (Canvas is RibbonDropDown)
            {
                (Canvas as RibbonDropDown).IgnoreNextClickDeactivation();
            }
        }

        /// <summary>
        ///     Redraws the control buttons: up, down and dropdown
        /// </summary>
        private void RedrawControlButtons()
        {
            if (Canvas != null)
            {
                if (ScrollType == ListScrollType.Scrollbar)
                {
                    Canvas.Invalidate(ScrollBarBounds);
                }
                else
                {
                    Canvas.Invalidate(Rectangle.FromLTRB(ButtonUpBounds.Left, ButtonUpBounds.Top, ButtonDropDownBounds.Right, ButtonDropDownBounds.Bottom));
                }
            }
        }

        /// <summary>
        ///     Pushes the amount of _offset of the top of items
        /// </summary>
        /// <param name="amount"></param>
        private void ScrollOffset(int amount)
        {
            ScrollTo(_offset + amount);
        }

        /// <summary>
        ///     Scrolls the content to the specified offset
        /// </summary>
        /// <param name="offset"></param>
        private void ScrollTo(int offset)
        {
            var minOffset = ContentBounds.Height - fullContentBounds.Height;
            if (offset < minOffset)
            {
                offset = minOffset;
            }

            _offset = offset;
            SetBounds(Bounds);
            RedrawItem();
        }

        /// <summary>
        ///     Sets the value of DropDownVisible
        /// </summary>
        /// <param name="visible"></param>
        internal void SetDropDownVisible(bool visible)
        {
            _dropDownVisible = visible;
        }

        /// <summary>
        ///     Shows the drop down items of the button, as if the dropdown part has been clicked
        /// </summary>
        public void ShowDropDown()
        {
            if (DropDownItems.Count == 0)
            {
                SetPressed(false);
                return;
            }

            IgnoreDeactivation();

            _dropDown = new RibbonDropDown(this, DropDownItems, Owner);
            //_dropDown.FormClosed += new FormClosedEventHandler(dropDown_FormClosed);
            //_dropDown.StartPosition = FormStartPosition.Manual;
            _dropDown.ShowSizingGrip = true;
            var location = Canvas.PointToScreen(new Point(Bounds.Left, Bounds.Top));

            SetDropDownVisible(true);
            _dropDown.Show(location);
        }

        private void dropDown_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetDropDownVisible(false);
        }

        #endregion

        #region Overrides

        protected override bool ClosesDropDownAt(Point p)
        {
            return !(
                ButtonDropDownBounds.Contains(p) ||
                ButtonDownBounds.Contains(p) ||
                ButtonUpBounds.Contains(p) ||
                (ScrollType == ListScrollType.Scrollbar && ScrollBarBounds.Contains(p))
                );
        }

        public override Size MeasureSize(object sender, RibbonElementMeasureSizeEventArgs e)
        {
            if (!Visible && !Owner.IsDesignMode())
            {
                SetLastMeasuredSize(new Size(0, 0));
                return LastMeasuredSize;
            }

            #region Determine items

            var itemsWide = 0;

            switch (e.SizeMode)
            {
                case RibbonElementSizeMode.DropDown:
                    itemsWide = ItemsSizeInDropwDownMode.Width;
                    break;
                case RibbonElementSizeMode.Large:
                    itemsWide = ItemsWideInLargeMode;
                    break;
                case RibbonElementSizeMode.Medium:
                    itemsWide = ItemsWideInMediumMode;
                    break;
                case RibbonElementSizeMode.Compact:
                    itemsWide = 0;
                    break;
            }

            #endregion

            var height = OwnerPanel.ContentBounds.Height - Owner.ItemPadding.Vertical - 4;
            var scannedItems = 0;
            var widthSum = 1;
            var buttonHeight = 0;
            var heightSum = 0;
            var sumWidth = true;

            foreach (RibbonButton button in Buttons)
            {
                var s = button.MeasureSize(this,
                    new RibbonElementMeasureSizeEventArgs(e.Graphics, ButtonsSizeMode));

                if (sumWidth)
                    widthSum += s.Width + 1;

                buttonHeight = button.LastMeasuredSize.Height;
                heightSum += buttonHeight;

                if (++scannedItems == itemsWide) sumWidth = false;
            }

            if (e.SizeMode == RibbonElementSizeMode.DropDown)
            {
                height = buttonHeight*ItemsSizeInDropwDownMode.Height;
            }

            if (ScrollBarRenderer.IsSupported)
            {
                _thumbBounds = new Rectangle(Point.Empty, ScrollBarRenderer.GetSizeBoxSize(e.Graphics, ScrollBarState.Normal));
            }
            else
            {
                _thumbBounds = new Rectangle(Point.Empty, new Size(16, 16));
            }

            //if (height < 0)
            //{
            //    throw new Exception("???");
            //}

            //Got off the patch site from logicalerror
            //SetLastMeasuredSize(new Size(widthSum + ControlButtonsWidth, height));
            SetLastMeasuredSize(new Size(Math.Max(0, widthSum + ControlButtonsWidth), Math.Max(0, height)));

            return LastMeasuredSize;
        }

        public void OnButtonItemClicked(ref RibbonItemEventArgs e)
        {
            if (ButtonItemClicked != null)
            {
                ButtonItemClicked(e.Item, e);
            }
        }

        public override void OnCanvasChanged(EventArgs e)
        {
            base.OnCanvasChanged(e);

            if (Canvas is RibbonDropDown)
            {
                _scrollType = ListScrollType.Scrollbar;
            }
            else
            {
                _scrollType = ListScrollType.UpDownButtons;
            }
        }

        public override void OnClick(EventArgs e)
        {
            //we need to override the onclick otherwise clicking on the scrollbar will close the popup window
            var pop = Canvas as RibbonPopup;
            if (pop == null)
            {
                base.OnClick(e);
            }
        }

        public void OnDropDownItemClicked(ref RibbonItemEventArgs e)
        {
            if (DropDownItemClicked != null)
            {
                DropDownItemClicked(e.Item, e);
            }
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (ButtonDownSelected || ButtonUpSelected || ButtonDropDownSelected)
            {
                IgnoreDeactivation();
            }

            if (ButtonDownSelected && ButtonDownEnabled)
            {
                _buttonDownPressed = true;
                ScrollDown();
            }

            if (ButtonUpSelected && ButtonUpEnabled)
            {
                _buttonUpPressed = true;
                ScrollUp();
            }

            if (ButtonDropDownSelected)
            {
                _buttonDropDownPressed = true;
                ShowDropDown();
            }

            if (ThumbSelected)
            {
                _thumbPressed = true;
                _thumbOffset = e.Y - _thumbBounds.Y;
            }

            if (
                ScrollType == ListScrollType.Scrollbar &&
                ScrollBarBounds.Contains(e.Location) &&
                e.Y >= ButtonUpBounds.Bottom && e.Y <= ButtonDownBounds.Y &&
                !ThumbBounds.Contains(e.Location) &&
                !ButtonDownBounds.Contains(e.Location) &&
                !ButtonUpBounds.Contains(e.Location))
            {
                //clicked the scroll area above or below the thumb
                if (e.Y < ThumbBounds.Y)
                {
                    ScrollOffset(ContentBounds.Height);
                }
                else
                {
                    ScrollOffset(-ContentBounds.Height);
                }
            }
        }

        public override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            var mustRedraw = _buttonUpSelected || _buttonDownSelected || _buttonDropDownSelected;

            _buttonUpSelected = false;
            _buttonDownSelected = false;
            _buttonDropDownSelected = false;

            if (mustRedraw)
                RedrawControlButtons();
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (ButtonDownPressed && ButtonDownSelected && ButtonDownEnabled)
            {
                ScrollOffset(-1);
            }

            if (ButtonUpPressed && ButtonUpSelected && ButtonUpEnabled)
            {
                ScrollOffset(1);
            }

            var upCache = _buttonUpSelected;
            var downCache = _buttonDownSelected;
            var dropCache = _buttonDropDownSelected;
            var thumbCache = _thumbSelected;

            _buttonUpSelected = _buttonUpBounds.Contains(e.Location);
            _buttonDownSelected = _buttonDownBounds.Contains(e.Location);
            _buttonDropDownSelected = _buttonDropDownBounds.Contains(e.Location);
            _thumbSelected = _thumbBounds.Contains(e.Location) && ScrollType == ListScrollType.Scrollbar && ScrollBarEnabled;

            if ((upCache != _buttonUpSelected)
                || (downCache != _buttonDownSelected)
                || (dropCache != _buttonDropDownSelected)
                || (thumbCache != _thumbSelected))
            {
                RedrawControlButtons();
            }

            if (ThumbPressed)
            {
                var newval = e.Y - _thumbOffset;

                if (newval < ScrollMinimum)
                {
                    newval = ScrollMinimum;
                }
                else if (newval > ScrollMaximum)
                {
                    newval = ScrollMaximum;
                }

                ScrollValue = newval;
                RedrawScroll();
            }
        }

        public override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            _buttonDownPressed = false;
            _buttonUpPressed = false;
            _buttonDropDownPressed = false;
            _thumbPressed = false;
        }

        public override void OnPaint(object sender, RibbonElementPaintEventArgs e)
        {
            Owner.Renderer.OnRenderRibbonItem(new RibbonItemRenderEventArgs(Owner, e.Graphics, e.Clip, this));

            if (e.Mode != RibbonElementSizeMode.Compact)
            {
                var lastClip = e.Graphics.Clip;
                var newClip = new Region(lastClip.GetBounds(e.Graphics));
                newClip.Intersect(ContentBounds);
                e.Graphics.SetClip(newClip.GetBounds(e.Graphics));

                foreach (RibbonButton button in Buttons)
                {
                    if (!button.Bounds.IsEmpty)
                        button.OnPaint(this, new RibbonElementPaintEventArgs(button.Bounds, e.Graphics, ButtonsSizeMode));
                }
                e.Graphics.SetClip(lastClip.GetBounds(e.Graphics));
            }
        }

        public override void SetBounds(Rectangle bounds)
        {
            base.SetBounds(bounds);

            #region Assign control buttons bounds

            if (ScrollType != ListScrollType.Scrollbar)
            {
                #region Custom Buttons

                var cbtns = 3; // Canvas is RibbonDropDown ? 2 : 3;
                var buttonHeight = bounds.Height/cbtns;
                var buttonWidth = _controlButtonsWidth;

                _buttonUpBounds = Rectangle.FromLTRB(bounds.Right - buttonWidth,
                    bounds.Top, bounds.Right, bounds.Top + buttonHeight);

                _buttonDownBounds = Rectangle.FromLTRB(_buttonUpBounds.Left, _buttonUpBounds.Bottom,
                    bounds.Right, _buttonUpBounds.Bottom + buttonHeight);

                if (cbtns == 2)
                {
                    _buttonDropDownBounds = Rectangle.Empty;
                }
                else
                {
                    _buttonDropDownBounds = Rectangle.FromLTRB(_buttonDownBounds.Left, _buttonDownBounds.Bottom,
                        bounds.Right, bounds.Bottom + 1);
                }

                _thumbBounds.Location = Point.Empty;

                #endregion
            }
            else
            {
                #region Scrollbar

                var bwidth = ThumbBounds.Width;
                var bheight = ThumbBounds.Width;

                _buttonUpBounds = Rectangle.FromLTRB(bounds.Right - bwidth,
                    bounds.Top + 1, bounds.Right, bounds.Top + bheight + 1);

                _buttonDownBounds = Rectangle.FromLTRB(_buttonUpBounds.Left, bounds.Bottom - bheight,
                    bounds.Right, bounds.Bottom);

                _buttonDropDownBounds = Rectangle.Empty;

                _thumbBounds.X = _buttonUpBounds.Left;

                #endregion
            }

            _contentBounds = Rectangle.FromLTRB(bounds.Left + 1, bounds.Top + 1, _buttonUpBounds.Left - 1, bounds.Bottom - 1);

            #endregion

            #region Assign buttons regions

            _buttonUpEnabled = _offset < 0;
            if (!_buttonUpEnabled) _offset = 0;
            _buttonDownEnabled = false;

            var curLeft = ContentBounds.Left + 1;
            var curTop = ContentBounds.Top + 1 + _offset;
            var maxBottom = curTop; // int.MinValue;
            var iniTop = curTop;

            foreach (var item in Buttons)
            {
                item.SetBounds(Rectangle.Empty);
            }

            for (var i = 0; i < Buttons.Count; i++)
            {
                var button = Buttons[i] as RibbonButton;
                if (button == null) break;

                if (curLeft + button.LastMeasuredSize.Width > ContentBounds.Right)
                {
                    curLeft = ContentBounds.Left + 1;
                    curTop = maxBottom + 1;
                }
                button.SetBounds(new Rectangle(curLeft, curTop, button.LastMeasuredSize.Width, button.LastMeasuredSize.Height));

                curLeft = button.Bounds.Right + 1;
                maxBottom = Math.Max(maxBottom, button.Bounds.Bottom);

                if (button.Bounds.Bottom > ContentBounds.Bottom) _buttonDownEnabled = true;

                _jumpDownSize = button.Bounds.Height;
                _jumpUpSize = button.Bounds.Height;
            }
            //Kevin - The bottom row of buttons were always getting cropped off a tiny bit
            maxBottom += 1;

            #endregion

            #region Adjust thumb size

            double contentHeight = maxBottom - iniTop;
            double viewHeight = ContentBounds.Height;

            if (contentHeight > viewHeight && contentHeight != 0)
            {
                var viewPercent = viewHeight/contentHeight;
                double availHeight = ButtonDownBounds.Top - ButtonUpBounds.Bottom;
                var thumbHeight = Math.Ceiling(viewPercent*availHeight);

                if (thumbHeight < 30)
                {
                    if (availHeight >= 30)
                    {
                        thumbHeight = 30;
                    }
                    else
                    {
                        thumbHeight = availHeight;
                    }
                }

                _thumbBounds.Height = Convert.ToInt32(thumbHeight);

                fullContentBounds = Rectangle.FromLTRB(ContentBounds.Left, iniTop, ContentBounds.Right, maxBottom);

                _scrollBarEnabled = true;

                UpdateThumbPos();
            }
            else
            {
                _scrollBarEnabled = false;
            }

            #endregion
        }

        internal override void SetOwner(Ribbon owner)
        {
            base.SetOwner(owner);

            _buttons.SetOwner(owner);
            _dropDownItems.SetOwner(owner);
        }

        internal override void SetOwnerPanel(RibbonPanel ownerPanel)
        {
            base.SetOwnerPanel(ownerPanel);

            _buttons.SetOwnerPanel(ownerPanel);
            _dropDownItems.SetOwnerPanel(ownerPanel);
        }

        internal override void SetOwnerTab(RibbonTab ownerTab)
        {
            base.SetOwnerTab(ownerTab);

            _buttons.SetOwnerTab(ownerTab);
            _dropDownItems.SetOwnerTab(OwnerTab);
        }

        internal override void SetSizeMode(RibbonElementSizeMode sizeMode)
        {
            base.SetSizeMode(sizeMode);

            foreach (var item in Buttons)
            {
                item.SetSizeMode(ButtonsSizeMode);
            }
        }

        /// <summary>
        ///     Updates the position of the scroll thumb depending on the current offset
        /// </summary>
        private void UpdateThumbPos()
        {
            if (_avoidNextThumbMeasure)
            {
                _avoidNextThumbMeasure = false;
                return;
            }

            var scrolledp = ScrolledPercent;

            if (!double.IsInfinity(scrolledp))
            {
                double availSpace = ScrollMaximum - ScrollMinimum;
                var scrolledSpace = Math.Ceiling(availSpace*ScrolledPercent);

                _thumbBounds.Y = ScrollMinimum + Convert.ToInt32(scrolledSpace);
            }
            else
            {
                _thumbBounds.Y = ScrollMinimum;
            }

            if (_thumbBounds.Y > ScrollMaximum)
            {
                _thumbBounds.Y = ScrollMaximum;
            }
        }

        #endregion

        internal void item_Click(object sender, EventArgs e)
        {
            // Steve
            _selectedItem = (sender as RibbonItem);

            //Kevin Carbis
            var ev = new RibbonItemEventArgs(_selectedItem);
            if (DropDownItems.Contains(_selectedItem))
                OnDropDownItemClicked(ref ev);
            else
                OnButtonItemClicked(ref ev);
        }

        #region IContainsRibbonItems Members

        public IEnumerable<RibbonItem> GetItems()
        {
            return Buttons;
        }

        public Rectangle GetContentBounds()
        {
            return ContentBounds;
        }

        #endregion

        #region IContainsRibbonComponents Members

        public IEnumerable<Component> GetAllChildComponents()
        {
            var result = new List<Component>(Buttons.ToArray());

            result.AddRange(DropDownItems.ToArray());

            return result;
        }

        #endregion
    }
}