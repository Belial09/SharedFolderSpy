#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Collections;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Designers;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Enums;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon
{
    public class RibbonOrbDropDown
        : RibbonPopup
    {
        #region const

        private const bool DefaultAutoSizeContentButtons = true;
        private const int DefaultContentButtonsMinWidth = 150;
        private const int DefaultContentRecentItemsMinWidth = 150;

        #endregion

        #region Fields

        private readonly RibbonItemCollection _menuItems;
        private readonly RibbonItemCollection _optionItems;
        private readonly RibbonItemCollection _recentItems;
        private readonly Ribbon _ribbon;
        internal RibbonOrbMenuItem LastPoppedMenuItem;
        private DateTime OpenedTime; //Steve - capture time popup was shown
        private int _RecentItemsCaptionLineSpacing = 8;
        //private GlobalHook _keyboardHook;
        private bool _autoSizeContentButtons = DefaultAutoSizeContentButtons;
        private int _contentButtonsMinWidth = DefaultContentButtonsMinWidth;
        private int _contentButtonsWidth = DefaultContentButtonsMinWidth;
        private Padding _contentMargin;
        private int _contentRecentItemsMinWidth = DefaultContentRecentItemsMinWidth;
        private string _recentItemsCaption;
        private RibbonMouseSensor _sensor;
        private Rectangle designerSelectedBounds;
        private int glyphGap = 3;

        #endregion

        #region Ctor

        internal RibbonOrbDropDown(Ribbon ribbon)
        {
            DoubleBuffered = true;
            _ribbon = ribbon;
            _menuItems = new RibbonItemCollection();
            _recentItems = new RibbonItemCollection();
            _optionItems = new RibbonItemCollection();

            _menuItems.SetOwner(Ribbon);
            _recentItems.SetOwner(Ribbon);
            _optionItems.SetOwner(Ribbon);

            OptionItemsPadding = 6;
            Size = new Size(527, 447);
            BorderRoundness = 8;

            //if (!(Site != null && Site.DesignMode))
            //{
            //   _keyboardHook = new GlobalHook(GlobalHook.HookTypes.Keyboard);
            //   _keyboardHook.KeyUp += new KeyEventHandler(_keyboardHook_KeyUp);
            //}
        }

        ~RibbonOrbDropDown()
        {
            if (_sensor != null)
            {
                _sensor.Dispose();
            }
            //if (_keyboardHook != null)
            //{
            //   _keyboardHook.Dispose();
            //}
        }

        #endregion

        #region Props

        /// <summary>
        ///     Gets all items involved in the dropdown
        /// </summary>
        internal List<RibbonItem> AllItems
        {
            get
            {
                var lst = new List<RibbonItem>();
                lst.AddRange(MenuItems);
                lst.AddRange(RecentItems);
                lst.AddRange(OptionItems);
                return lst;
            }
        }

        /// <summary>
        ///     Gets the margin of the content bounds
        /// </summary>
        [Browsable(false)]
        public Padding ContentMargin
        {
            get
            {
                if (_contentMargin.Size.IsEmpty)
                {
                    _contentMargin = new Padding(6, 17, 6, 29);
                }

                return _contentMargin;
            }
        }

        /// <summary>
        ///     Gets the bounds of the content (where menu buttons are)
        /// </summary>
        [Browsable(false)]
        public Rectangle ContentBounds
        {
            get
            {
                return Rectangle.FromLTRB(ContentMargin.Left, ContentMargin.Top,
                    ClientRectangle.Right - ContentMargin.Right,
                    ClientRectangle.Bottom - ContentMargin.Bottom);
            }
        }

        /// <summary>
        ///     Gets the bounds of the content part that contains the buttons on the left
        /// </summary>
        [Browsable(false)]
        public Rectangle ContentButtonsBounds
        {
            get
            {
                var r = ContentBounds;
                r.Width = _contentButtonsWidth;
                if (Ribbon.RightToLeft == RightToLeft.Yes)
                    r.X = ContentBounds.Right - _contentButtonsWidth;
                return r;
            }
        }

        /// <summary>
        ///     Gets or sets the minimum width for the content buttons.
        /// </summary>
        [DefaultValue(DefaultContentButtonsMinWidth)]
        public int ContentButtonsMinWidth
        {
            get { return _contentButtonsMinWidth; }
            set { _contentButtonsMinWidth = value; }
        }

        /// <summary>
        ///     Gets the bounds fo the content part that contains the recent-item list
        /// </summary>
        [Browsable(false)]
        public Rectangle ContentRecentItemsBounds
        {
            get
            {
                var r = ContentBounds;
                r.Width -= _contentButtonsWidth;

                //Steve - Recent Items Caption
                r.Height -= ContentRecentItemsCaptionBounds.Height;
                r.Y += ContentRecentItemsCaptionBounds.Height;

                if (Ribbon.RightToLeft == RightToLeft.No)
                    r.X += _contentButtonsWidth;

                return r;
            }
        }

        /// <summary>
        ///     Gets the bounds of the caption area on the content part of the recent-item list
        /// </summary>
        [Browsable(false)]
        public Rectangle ContentRecentItemsCaptionBounds
        {
            get
            {
                if (RecentItemsCaption != null)
                {
                    //Lets measure the height of the text so we take into account the font and its size
                    SizeF cs;
                    using (var g = CreateGraphics())
                    {
                        cs = g.MeasureString(RecentItemsCaption, Ribbon.RibbonTabFont);
                    }
                    var r = ContentBounds;
                    r.Width -= _contentButtonsWidth;
                    r.Height = Convert.ToInt32(cs.Height) + Ribbon.ItemMargin.Top + Ribbon.ItemMargin.Bottom; //padding
                    r.Height += _RecentItemsCaptionLineSpacing; //Spacing for the divider line

                    if (Ribbon.RightToLeft == RightToLeft.No)
                        r.X += _contentButtonsWidth;
                    return r;
                }
                return Rectangle.Empty;
            }
        }

        /// <summary>
        ///     Gets the bounds of the caption area on the content part of the recent-item list
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int RecentItemsCaptionLineSpacing
        {
            get { return _RecentItemsCaptionLineSpacing; }
        }

        /// <summary>
        ///     Gets or sets the minimum width for the recent items.
        /// </summary>
        [DefaultValue(DefaultContentRecentItemsMinWidth)]
        public int ContentRecentItemsMinWidth
        {
            get { return _contentRecentItemsMinWidth; }
            set { _contentRecentItemsMinWidth = value; }
        }

        /// <summary>
        ///     Gets if currently on design mode
        /// </summary>
        private bool RibbonInDesignMode
        {
            get { return RibbonDesigner.Current != null; }
        }

        /// <summary>
        ///     Gets the collection of items shown in the menu area
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RibbonItemCollection MenuItems
        {
            get { return _menuItems; }
        }

        /// <summary>
        ///     Gets the collection of items shown in the options area (bottom)
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RibbonItemCollection OptionItems
        {
            get { return _optionItems; }
        }

        [DefaultValue(6), Description("Spacing between option buttons (those on the bottom)")]
        public int OptionItemsPadding { get; set; }

        /// <summary>
        ///     Gets the collection of items shown in the recent items area
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RibbonItemCollection RecentItems
        {
            get { return _recentItems; }
        }

        /// <summary>
        ///     Gets or Sets the caption for the Recent Items area
        /// </summary>
        [DefaultValue(null)]
        public string RecentItemsCaption
        {
            get { return _recentItemsCaption; }
            set
            {
                _recentItemsCaption = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Gets the ribbon that owns this dropdown
        /// </summary>
        [Browsable(false)]
        public Ribbon Ribbon
        {
            get { return _ribbon; }
        }

        /// <summary>
        ///     Gets the sensor of the dropdown
        /// </summary>
        [Browsable(false)]
        public RibbonMouseSensor Sensor
        {
            get { return _sensor; }
        }

        /// <summary>
        ///     Gets the bounds of the glyph
        /// </summary>
        internal Rectangle ButtonsGlyphBounds
        {
            get
            {
                var s = new Size(50, 18);
                var rf = ContentButtonsBounds;
                var r = new Rectangle(rf.Left + (rf.Width - s.Width*2)/2, rf.Top + glyphGap, s.Width, s.Height);

                if (MenuItems.Count > 0)
                {
                    r.Y = MenuItems[MenuItems.Count - 1].Bounds.Bottom + glyphGap;
                }

                return r;
            }
        }

        /// <summary>
        ///     Gets the bounds of the glyph
        /// </summary>
        internal Rectangle ButtonsSeparatorGlyphBounds
        {
            get
            {
                var s = new Size(18, 18);

                var r = ButtonsGlyphBounds;

                r.X = r.Right + glyphGap;

                return r;
            }
        }

        /// <summary>
        ///     Gets the bounds of the recent items add glyph
        /// </summary>
        internal Rectangle RecentGlyphBounds
        {
            get
            {
                var s = new Size(50, 18);
                var rf = ContentRecentItemsBounds;
                var r = new Rectangle(rf.Left + glyphGap, rf.Top + glyphGap, s.Width, s.Height);

                if (RecentItems.Count > 0)
                {
                    r.Y = RecentItems[RecentItems.Count - 1].Bounds.Bottom + glyphGap;
                }

                return r;
            }
        }

        /// <summary>
        ///     Gets the bounds of the option items add glyph
        /// </summary>
        internal Rectangle OptionGlyphBounds
        {
            get
            {
                var s = new Size(50, 18);
                var rf = ContentBounds;
                var r = new Rectangle(rf.Right - s.Width, rf.Bottom + glyphGap, s.Width, s.Height);

                if (OptionItems.Count > 0)
                {
                    r.X = OptionItems[OptionItems.Count - 1].Bounds.Left - s.Width - glyphGap;
                }

                return r;
            }
        }

        [DefaultValue(DefaultAutoSizeContentButtons)]
        public bool AutoSizeContentButtons
        {
            get { return _autoSizeContentButtons; }
            set { _autoSizeContentButtons = value; }
        }

        #endregion

        #region Methods

        private RibbonItem GetNextSelectableMenuItem(int StartIndex)
        {
            for (var idx = StartIndex; idx < MenuItems.Count; idx++)
            {
                var btn = MenuItems[idx] as RibbonButton;
                if (btn != null)
                    return btn;
            }
            //nothing found so lets move on to the recent items
            var NextItem = GetNextSelectableRecentItem(0);
            if (NextItem == null)
            {
                //nothing found so lets try the option items
                NextItem = GetNextSelectableOptionItem(0);
                if (NextItem == null)
                {
                    //nothing again so go back to the top of the menu items
                    NextItem = GetNextSelectableMenuItem(0);
                }
            }
            return NextItem;
        }

        private RibbonItem GetNextSelectableOptionItem(int StartIndex)
        {
            for (var idx = StartIndex; idx < OptionItems.Count; idx++)
            {
                var btn = OptionItems[idx] as RibbonButton;
                if (btn != null)
                    return btn;
            }
            //nothing found so lets move on to the menu items
            var NextItem = GetNextSelectableMenuItem(0);
            if (NextItem == null)
            {
                //nothing found so lets try the recent items
                NextItem = GetNextSelectableRecentItem(0);
                if (NextItem == null)
                {
                    //nothing again so go back to the top of the option items
                    NextItem = GetNextSelectableOptionItem(0);
                }
            }
            return NextItem;
        }

        private RibbonItem GetNextSelectableRecentItem(int StartIndex)
        {
            for (var idx = StartIndex; idx < RecentItems.Count; idx++)
            {
                var btn = RecentItems[idx] as RibbonButton;
                if (btn != null)
                    return btn;
            }
            //nothing found so lets move on to the option items
            var NextItem = GetNextSelectableOptionItem(0);
            if (NextItem == null)
            {
                //nothing found so lets try the menu items
                NextItem = GetNextSelectableMenuItem(0);
                if (NextItem == null)
                {
                    //nothing again so go back to the top of the recent items
                    NextItem = GetNextSelectableRecentItem(0);
                }
            }
            return NextItem;
        }

        internal void HandleDesignerItemRemoved(RibbonItem item)
        {
            if (MenuItems.Contains(item))
            {
                MenuItems.Remove(item);
            }
            else if (RecentItems.Contains(item))
            {
                RecentItems.Remove(item);
            }
            else if (OptionItems.Contains(item))
            {
                OptionItems.Remove(item);
            }

            OnRegionsChanged();
        }

        protected override void OnClosed(EventArgs e)
        {
            Ribbon.OrbPressed = false;
            Ribbon.OrbSelected = false;
            LastPoppedMenuItem = null;
            foreach (var item in AllItems)
            {
                item.SetSelected(false);
                item.SetPressed(false);
            }
            base.OnClosed(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (Ribbon.RectangleToScreen(Ribbon.OrbBounds).Contains(PointToScreen(e.Location)))
            {
                Ribbon.OnOrbClicked(EventArgs.Empty);
                //Steve - if click time is within the double click time after the drop down was shown, then this is a double click
                if (DateTime.Compare(DateTime.Now, OpenedTime.AddMilliseconds(SystemInformation.DoubleClickTime)) < 0)
                    Ribbon.OnOrbDoubleClicked(EventArgs.Empty);
            }

            base.OnMouseClick(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (Ribbon.RectangleToScreen(Ribbon.OrbBounds).Contains(PointToScreen(e.Location)))
            {
                Ribbon.OnOrbDoubleClicked(EventArgs.Empty);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (RibbonInDesignMode)
            {
                #region DesignMode clicks

                if (ContentBounds.Contains(e.Location))
                {
                    if (ContentButtonsBounds.Contains(e.Location))
                    {
                        foreach (var item in MenuItems)
                        {
                            if (item.Bounds.Contains(e.Location))
                            {
                                SelectOnDesigner(item);
                                break;
                            }
                        }
                    }
                    else if (ContentRecentItemsBounds.Contains(e.Location))
                    {
                        foreach (var item in RecentItems)
                        {
                            if (item.Bounds.Contains(e.Location))
                            {
                                SelectOnDesigner(item);
                                break;
                            }
                        }
                    }
                }
                if (ButtonsGlyphBounds.Contains(e.Location))
                {
                    RibbonDesigner.Current.CreteOrbMenuItem(typeof (RibbonOrbMenuItem));
                }
                else if (ButtonsSeparatorGlyphBounds.Contains(e.Location))
                {
                    RibbonDesigner.Current.CreteOrbMenuItem(typeof (RibbonSeparator));
                }
                else if (RecentGlyphBounds.Contains(e.Location))
                {
                    RibbonDesigner.Current.CreteOrbRecentItem(typeof (RibbonOrbRecentItem));
                }
                else if (OptionGlyphBounds.Contains(e.Location))
                {
                    RibbonDesigner.Current.CreteOrbOptionItem(typeof (RibbonOrbOptionButton));
                }
                else
                {
                    foreach (var item in OptionItems)
                    {
                        if (item.Bounds.Contains(e.Location))
                        {
                            SelectOnDesigner(item);
                            break;
                        }
                    }
                }

                #endregion
            }
        }

        protected override void OnOpening(CancelEventArgs e)
        {
            base.OnOpening(e);

            UpdateRegions();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Ribbon.Renderer.OnRenderOrbDropDownBackground(
                new RibbonOrbDropDownEventArgs(Ribbon, this, e.Graphics, e.ClipRectangle));

            foreach (var item in AllItems)
            {
                item.OnPaint(this, new RibbonElementPaintEventArgs(e.ClipRectangle, e.Graphics, RibbonElementSizeMode.DropDown));
            }

            if (RibbonInDesignMode)
            {
                using (var b = new SolidBrush(Color.FromArgb(50, Color.Blue)))
                {
                    e.Graphics.FillRectangle(b, ButtonsGlyphBounds);
                    e.Graphics.FillRectangle(b, RecentGlyphBounds);
                    e.Graphics.FillRectangle(b, OptionGlyphBounds);
                    e.Graphics.FillRectangle(b, ButtonsSeparatorGlyphBounds);
                }

                using (var sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Trimming = StringTrimming.None;
                    e.Graphics.DrawString("+", Font, Brushes.White, ButtonsGlyphBounds, sf);
                    e.Graphics.DrawString("+", Font, Brushes.White, RecentGlyphBounds, sf);
                    e.Graphics.DrawString("+", Font, Brushes.White, OptionGlyphBounds, sf);
                    e.Graphics.DrawString("---", Font, Brushes.White, ButtonsSeparatorGlyphBounds, sf);
                }

                using (var p = new Pen(Color.Black))
                {
                    p.DashStyle = DashStyle.Dot;
                    e.Graphics.DrawRectangle(p, designerSelectedBounds);
                }

                //e.Graphics.DrawString("Press ESC to Hide", Font, Brushes.Black, Width - 100f, 2f);
            }
        }

        /// <summary>
        ///     Updates all areas and bounds of items
        /// </summary>
        internal void OnRegionsChanged()
        {
            UpdateRegions();
            UpdateSensor();
            UpdateDesignerSelectedBounds();
            Invalidate();
        }

        protected override void OnShowed(EventArgs e)
        {
            base.OnShowed(e);
            OpenedTime = DateTime.Now;
            UpdateSensor();
        }

        /// <summary>
        ///     Selects the specified item on the designer
        /// </summary>
        /// <param name="item"></param>
        internal void SelectOnDesigner(RibbonItem item)
        {
            if (RibbonDesigner.Current != null)
            {
                RibbonDesigner.Current.SelectedElement = item;
                UpdateDesignerSelectedBounds();
                Invalidate();
            }
        }

        /// <summary>
        ///     Gets the height that a separator should be on the DropDown
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private int SeparatorHeight(RibbonSeparator s)
        {
            if (!string.IsNullOrEmpty(s.Text))
            {
                return 20;
            }
            return 3;
        }

        /// <summary>
        ///     Updates the selection bounds on the designer
        /// </summary>
        internal void UpdateDesignerSelectedBounds()
        {
            designerSelectedBounds = Rectangle.Empty;

            if (RibbonInDesignMode)
            {
                var item = RibbonDesigner.Current.SelectedElement as RibbonItem;

                if (item != null && AllItems.Contains(item))
                {
                    designerSelectedBounds = item.Bounds;
                }
            }
        }

        /// <summary>
        ///     Updates the regions and bounds of items
        /// </summary>
        private void UpdateRegions()
        {
            var curtop = 0;
            var curright = 0;
            var menuItemHeight = 44;
            var recentHeight = 22;
            var mbuttons = 1; //margin
            var mrecent = 1; //margin
            var buttonsHeight = 0;
            var recentsHeight = 0;

            if (AutoSizeContentButtons)
            {
                #region important to do the item max width check before the ContentBounds and other stuff is used (internal Property stuff)

                var itemMaxWidth = 0;
                using (var g = CreateGraphics())
                {
                    foreach (var item in MenuItems)
                    {
                        var width = item.MeasureSize(this, new RibbonElementMeasureSizeEventArgs(g, RibbonElementSizeMode.DropDown)).Width;
                        if (width > itemMaxWidth)
                            itemMaxWidth = width;
                    }
                }
                itemMaxWidth = Math.Min(itemMaxWidth, ContentBounds.Width - ContentRecentItemsMinWidth);
                itemMaxWidth = Math.Max(itemMaxWidth, ContentButtonsMinWidth);
                _contentButtonsWidth = itemMaxWidth;

                #endregion
            }

            var rcontent = ContentBounds;
            var rbuttons = ContentButtonsBounds;
            var rrecent = ContentRecentItemsBounds;

            foreach (var item in AllItems)
            {
                item.SetSizeMode(RibbonElementSizeMode.DropDown);
                item.SetCanvas(this);
            }

            #region Menu Items

            curtop = rcontent.Top + 1;

            foreach (var item in MenuItems)
            {
                var ritem = new Rectangle(rbuttons.Left + mbuttons, curtop, rbuttons.Width - mbuttons*2, menuItemHeight);

                if (item is RibbonSeparator) ritem.Height = SeparatorHeight(item as RibbonSeparator);

                item.SetBounds(ritem);

                curtop += ritem.Height;
            }

            buttonsHeight = curtop - rcontent.Top + 1;

            #endregion

            #region Recent List

            //curtop = rbuttons.Top; //Steve - for recent documents
            curtop = rrecent.Top; //Steve - for recent documents

            foreach (var item in RecentItems)
            {
                var ritem = new Rectangle(rrecent.Left + mrecent, curtop, rrecent.Width - mrecent*2, recentHeight);

                if (item is RibbonSeparator) ritem.Height = SeparatorHeight(item as RibbonSeparator);

                item.SetBounds(ritem);

                curtop += ritem.Height;
            }

            recentsHeight = curtop - rbuttons.Top;

            #endregion

            #region Set size

            var actualHeight = Math.Max(buttonsHeight, recentsHeight);

            if (RibbonDesigner.Current != null)
            {
                actualHeight += ButtonsGlyphBounds.Height + glyphGap*2;
            }

            Height = actualHeight + ContentMargin.Vertical;
            rcontent = ContentBounds;

            #endregion

            #region Option buttons

            curright = ClientSize.Width - ContentMargin.Right;

            using (var g = CreateGraphics())
            {
                foreach (var item in OptionItems)
                {
                    var s = item.MeasureSize(this, new RibbonElementMeasureSizeEventArgs(g, RibbonElementSizeMode.DropDown));
                    curtop = rcontent.Bottom + (ContentMargin.Bottom - s.Height)/2;
                    item.SetBounds(new Rectangle(new Point(curright - s.Width, curtop), s));
                    curright = item.Bounds.Left - OptionItemsPadding;
                }
            }

            #endregion
        }

        /// <summary>
        ///     Refreshes the sensor
        /// </summary>
        private void UpdateSensor()
        {
            if (_sensor != null)
            {
                _sensor.Dispose();
            }

            _sensor = new RibbonMouseSensor(this, Ribbon, AllItems);
        }

        private void _keyboardHook_KeyUp(object sender, KeyEventArgs e)
        {
            //base.OnKeyUp(e);
            if (e.KeyCode == Keys.Down)
            {
                RibbonItem NextItem = null;
                RibbonItem SelectedItem = null;
                foreach (var itm in MenuItems)
                {
                    if (itm.Selected)
                    {
                        SelectedItem = itm;
                        break;
                    }
                }
                if (SelectedItem != null)
                {
                    //get the next item in the chain
                    var Index = MenuItems.IndexOf(SelectedItem);
                    NextItem = GetNextSelectableMenuItem(Index + 1);
                }
                else
                {
                    //nothing found so lets search through the recent buttons
                    foreach (var itm in RecentItems)
                    {
                        if (itm.Selected)
                        {
                            SelectedItem = itm;
                            itm.SetSelected(false);
                            itm.RedrawItem();
                            break;
                        }
                    }
                    if (SelectedItem != null)
                    {
                        //get the next item in the chain
                        var Index = RecentItems.IndexOf(SelectedItem);
                        NextItem = GetNextSelectableRecentItem(Index + 1);
                    }
                    else
                    {
                        //nothing found so lets search through the option buttons
                        foreach (var itm in OptionItems)
                        {
                            if (itm.Selected)
                            {
                                SelectedItem = itm;
                                itm.SetSelected(false);
                                itm.RedrawItem();
                                break;
                            }
                        }
                        if (SelectedItem != null)
                        {
                            //get the next item in the chain
                            var Index = OptionItems.IndexOf(SelectedItem);
                            NextItem = GetNextSelectableOptionItem(Index + 1);
                        }
                    }
                }
                //last check to make sure we found a selected item
                if (SelectedItem == null)
                {
                    //we should have the right item by now so lets select it
                    NextItem = GetNextSelectableMenuItem(0);
                    if (NextItem != null)
                    {
                        NextItem.SetSelected(true);
                        NextItem.RedrawItem();
                    }
                }
                else
                {
                    SelectedItem.SetSelected(false);
                    SelectedItem.RedrawItem();

                    NextItem.SetSelected(true);
                    NextItem.RedrawItem();
                }
                //_sensor.SelectedItem = NextItem;
                //_sensor.HittedItem = NextItem;
            }
            else if (e.KeyCode == Keys.Up)
            {
            }
        }

        #endregion
    }
}