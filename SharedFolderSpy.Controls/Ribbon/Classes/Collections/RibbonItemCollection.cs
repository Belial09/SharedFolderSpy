#region

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Collections
{
    [Editor("System.Windows.Forms.RibbonItemCollectionEditor", typeof (UITypeEditor))]
    public class RibbonItemCollection
        : List<RibbonItem>, IList
    {
        #region Fields

        private Ribbon _owner;
        private RibbonPanel _ownerPanel;
        private RibbonTab _ownerTab;

        #endregion

        #region Ctor

        /// <summary>
        ///     Creates a new ribbon item collection
        /// </summary>
        internal RibbonItemCollection()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the Ribbon owner of this collection
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Ribbon Owner
        {
            get { return _owner; }
        }

        /// <summary>
        ///     Gets the RibbonPanel where this item is located
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RibbonPanel OwnerPanel
        {
            get { return _ownerPanel; }
        }

        /// <summary>
        ///     Gets the RibbonTab that contains this item
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RibbonTab OwnerTab
        {
            get { return _ownerTab; }
        }

        #endregion

        #region Overrides

        /// <summary>
        ///     Adds the specified item to the collection
        /// </summary>
        public new virtual void Add(RibbonItem item)
        {
            item.SetOwner(Owner);
            item.SetOwnerPanel(OwnerPanel);
            item.SetOwnerTab(OwnerTab);

            base.Add(item);
        }

        /// <summary>
        ///     Adds the specified range of items
        /// </summary>
        /// <param name="items">Items to add</param>
        public new virtual void AddRange(IEnumerable<RibbonItem> items)
        {
            foreach (var item in items)
            {
                item.SetOwner(Owner);
                item.SetOwnerPanel(OwnerPanel);
                item.SetOwnerTab(OwnerTab);
            }

            base.AddRange(items);
        }

        /// <summary>
        ///     Inserts the specified item at the desired index
        /// </summary>
        /// <param name="index">Desired index of the item</param>
        /// <param name="item">Item to insert</param>
        public new virtual void Insert(int index, RibbonItem item)
        {
            item.SetOwner(Owner);
            item.SetOwnerPanel(OwnerPanel);
            item.SetOwnerTab(OwnerTab);

            base.Insert(index, item);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Centers the items horizontally on the specified rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        internal void CenterItemsHorizontallyInto(Rectangle rectangle)
        {
            CenterItemsHorizontallyInto(this, rectangle);
        }

        /// <summary>
        ///     Centers the items horizontally on the specified rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        internal void CenterItemsHorizontallyInto(IEnumerable<RibbonItem> items, Rectangle rectangle)
        {
            var x = rectangle.Left + (rectangle.Width - GetItemsWidth(items))/2;
            var y = GetItemsTop(items);

            MoveTo(items, new Point(x, y));
        }

        /// <summary>
        ///     Centers the items on the specified rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        internal void CenterItemsInto(Rectangle rectangle)
        {
            CenterItemsInto(this, rectangle);
        }

        /// <summary>
        ///     Centers the items on the specified rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        internal void CenterItemsInto(IEnumerable<RibbonItem> items, Rectangle rectangle)
        {
            var x = rectangle.Left + (rectangle.Width - GetItemsWidth())/2;
            var y = rectangle.Top + (rectangle.Height - GetItemsHeight())/2;

            MoveTo(items, new Point(x, y));
        }

        /// <summary>
        ///     Centers the items vertically on the specified rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        internal void CenterItemsVerticallyInto(Rectangle rectangle)
        {
            CenterItemsVerticallyInto(this, rectangle);
        }

        /// <summary>
        ///     Centers the items vertically on the specified rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        internal void CenterItemsVerticallyInto(IEnumerable<RibbonItem> items, Rectangle rectangle)
        {
            var x = GetItemsLeft(items);
            var y = rectangle.Top + (rectangle.Height - GetItemsHeight(items))/2;

            MoveTo(items, new Point(x, y));
        }

        /// <summary>
        ///     Gets the bottom of items as a group of shapes
        /// </summary>
        /// <returns></returns>
        internal int GetItemsBottom(IEnumerable<RibbonItem> items)
        {
            if (Count == 0) return 0;

            var max = int.MinValue;

            foreach (var item in items)
            {
                if (item.Bounds.Bottom > max)
                {
                    max = item.Bounds.Bottom;
                }
            }

            return max;
        }

        /// <summary>
        ///     Gets the bottom of items as a group of shapes
        /// </summary>
        /// <returns></returns>
        internal int GetItemsBottom()
        {
            if (Count == 0) return 0;

            var max = int.MinValue;

            foreach (var item in this)
            {
                if (item.Visible && item.Bounds.Bottom > max)
                {
                    max = item.Bounds.Bottom;
                }
            }
            if (max == int.MinValue)
            {
                max = 0;
            }

            return max;
        }

        /// <summary>
        ///     Gets the bounds of items as a group of shapes
        /// </summary>
        /// <returns></returns>
        internal Rectangle GetItemsBounds(IEnumerable<RibbonItem> items)
        {
            return Rectangle.FromLTRB(GetItemsLeft(items), GetItemsTop(items), GetItemsRight(items), GetItemsBottom(items));
        }

        /// <summary>
        ///     Gets the bounds of items as a group of shapes
        /// </summary>
        /// <returns></returns>
        internal Rectangle GetItemsBounds()
        {
            return Rectangle.FromLTRB(GetItemsLeft(), GetItemsTop(), GetItemsRight(), GetItemsBottom());
        }

        /// <summary>
        ///     Gets the height of items as a group of shapes
        /// </summary>
        /// <returns></returns>
        internal int GetItemsHeight(IEnumerable<RibbonItem> items)
        {
            return GetItemsBottom(items) - GetItemsTop(items);
        }

        /// <summary>
        ///     Gets the height of items as a group of shapes
        /// </summary>
        /// <returns></returns>
        internal int GetItemsHeight()
        {
            return GetItemsBottom() - GetItemsTop();
        }

        /// <summary>
        ///     Gets the left of items as a group of shapes
        /// </summary>
        /// <returns></returns>
        internal int GetItemsLeft(IEnumerable<RibbonItem> items)
        {
            if (Count == 0) return 0;

            var min = int.MaxValue;

            foreach (var item in items)
            {
                if (item.Bounds.X < min)
                {
                    min = item.Bounds.X;
                }
            }

            return min;
        }

        /// <summary>
        ///     Gets the left of items as a group of shapes
        /// </summary>
        /// <returns></returns>
        internal int GetItemsLeft()
        {
            if (Count == 0) return 0;

            var min = int.MaxValue;

            foreach (var item in this)
            {
                if (item.Bounds.X < min)
                {
                    min = item.Bounds.X;
                }
            }

            return min;
        }

        /// <summary>
        ///     Gets the right of items as a group of shapes
        /// </summary>
        /// <returns></returns>
        internal int GetItemsRight(IEnumerable<RibbonItem> items)
        {
            if (Count == 0) return 0;

            var max = int.MinValue;
            ;

            foreach (var item in items)
            {
                if (item.Bounds.Right > max)
                {
                    max = item.Bounds.Right;
                }
            }

            return max;
        }

        /// <summary>
        ///     Gets the right of items as a group of shapes
        /// </summary>
        /// <returns></returns>
        internal int GetItemsRight()
        {
            if (Count == 0) return 0;

            var max = int.MinValue;
            ;

            foreach (var item in this)
            {
                if (item.Visible && item.Bounds.Right > max)
                {
                    max = item.Bounds.Right;
                }
            }
            if (max == int.MinValue)
            {
                max = 0;
            }

            return max;
        }

        /// <summary>
        ///     Gets the top of items as a group of shapes
        /// </summary>
        /// <returns></returns>
        internal int GetItemsTop(IEnumerable<RibbonItem> items)
        {
            if (Count == 0) return 0;

            var min = int.MaxValue;

            foreach (var item in items)
            {
                if (item.Bounds.Y < min)
                {
                    min = item.Bounds.Y;
                }
            }

            return min;
        }

        /// <summary>
        ///     Gets the top of items as a group of shapes
        /// </summary>
        /// <returns></returns>
        internal int GetItemsTop()
        {
            if (Count == 0) return 0;

            var min = int.MaxValue;

            foreach (var item in this)
            {
                if (item.Bounds.Y < min)
                {
                    min = item.Bounds.Y;
                }
            }

            return min;
        }

        /// <summary>
        ///     Gets the width of items as a group of shapes
        /// </summary>
        /// <returns></returns>
        internal int GetItemsWidth(IEnumerable<RibbonItem> items)
        {
            return GetItemsRight(items) - GetItemsLeft(items);
        }

        /// <summary>
        ///     Gets the width of items as a group of shapes
        /// </summary>
        /// <returns></returns>
        internal int GetItemsWidth()
        {
            return GetItemsRight() - GetItemsLeft();
        }

        /// <summary>
        ///     Moves the bounds of items as a group of shapes
        /// </summary>
        /// <param name="p"></param>
        internal void MoveTo(Point p)
        {
            MoveTo(this, p);
        }

        /// <summary>
        ///     Moves the bounds of items as a group of shapes
        /// </summary>
        /// <param name="p"></param>
        internal void MoveTo(IEnumerable<RibbonItem> items, Point p)
        {
            var oldBounds = GetItemsBounds(items);

            foreach (var item in items)
            {
                var dx = item.Bounds.X - oldBounds.Left;
                var dy = item.Bounds.Y - oldBounds.Top;

                item.SetBounds(new Rectangle(new Point(p.X + dx, p.Y + dy), item.Bounds.Size));
            }
        }

        /// <summary>
        ///     Sets the owner Ribbon of the collection
        /// </summary>
        /// <param name="owner"></param>
        internal void SetOwner(Ribbon owner)
        {
            _owner = owner;

            foreach (var item in this)
            {
                item.SetOwner(owner);
            }
        }

        /// <summary>
        ///     Sets the owner panel of the collection
        /// </summary>
        /// <param name="panel"></param>
        internal void SetOwnerPanel(RibbonPanel panel)
        {
            _ownerPanel = panel;

            foreach (var item in this)
            {
                item.SetOwnerPanel(panel);
            }
        }

        /// <summary>
        ///     Sets the owner Tab of the collection
        /// </summary>
        /// <param name="tab"></param>
        internal void SetOwnerTab(RibbonTab tab)
        {
            _ownerTab = tab;

            foreach (var item in this)
            {
                item.SetOwnerTab(tab);
            }
        }

        #endregion

        #region IList

        int IList.Add(object item)
        {
            var ri = item as RibbonItem;
            if (ri == null)
                return -1;

            Add(ri);
            return Count - 1;
        }

        void IList.Insert(int index, object item)
        {
            var ri = item as RibbonItem;
            if (ri == null)
                return;

            Insert(index, ri);
        }

        #endregion
    }
}