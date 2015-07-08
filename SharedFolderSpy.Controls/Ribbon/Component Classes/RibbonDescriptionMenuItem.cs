#region

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Enums;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon
{
    /// <summary>
    ///     Large menu item with a description bellow the text
    /// </summary>
    public class RibbonDescriptionMenuItem
        : RibbonButton
    {
        #region Fields

        private Rectangle _descBounds;

        #endregion

        #region Ctor

        public RibbonDescriptionMenuItem()
        {
            DropDownArrowDirection = RibbonArrowDirection.Left;
            SetDropDownMargin(new Padding(10));
        }

        /// <summary>
        ///     Creates a new menu item with description
        /// </summary>
        /// <param name="text">Text of the menuitem</param>
        public RibbonDescriptionMenuItem(string text)
            : this(null, text, null)
        {
        }

        /// <summary>
        ///     Creates a new menu item with description
        /// </summary>
        /// <param name="text">Text of the menuitem</param>
        /// <param name="description">Descripion of the menuitem</param>
        public RibbonDescriptionMenuItem(string text, string description)
            : this(null, text, description)
        {
        }

        /// <summary>
        ///     Creates a new menu item with description
        /// </summary>
        /// <param name="image">Image for the menuitem</param>
        /// <param name="text">Text for the menuitem</param>
        /// <param name="description">Description for the menuitem</param>
        public RibbonDescriptionMenuItem(Image image, string text, string description)
        {
            Image = image;
            Text = text;
            Description = description;
        }

        #endregion

        #region Props

        /// <summary>
        ///     Gets or sets the bounds of the description
        /// </summary>
        public Rectangle DescriptionBounds
        {
            get { return _descBounds; }
            set { _descBounds = value; }
        }


        /// <summary>
        ///     Gets or sets the image of the menu item
        /// </summary>
        public override Image Image
        {
            get { return base.Image; }
            set
            {
                base.Image = value;

                SmallImage = value;
            }
        }

        /// <summary>
        ///     This property is not relevant for this class
        /// </summary>
        [Browsable(false)]
        public override Image SmallImage
        {
            get { return base.SmallImage; }
            set { base.SmallImage = value; }
        }

        /// <summary>
        ///     Gets or sets the description of the button
        /// </summary>
        [DefaultValue(null)]
        public string Description { get; set; }

        #endregion

        #region Methods

        public override Size MeasureSize(object sender, RibbonElementMeasureSizeEventArgs e)
        {
            if (!Visible && !Owner.IsDesignMode())
            {
                SetLastMeasuredSize(new Size(0, 0));
                return LastMeasuredSize;
            }

            var s = base.MeasureSize(sender, e);

            s.Height = 52;

            SetLastMeasuredSize(s);

            return s;
        }

        internal override Rectangle OnGetTextBounds(RibbonElementSizeMode sMode, Rectangle bounds)
        {
            var r = base.OnGetTextBounds(sMode, bounds);
            DescriptionBounds = r;

            r.Height = 20;

            DescriptionBounds = Rectangle.FromLTRB(DescriptionBounds.Left, r.Bottom, DescriptionBounds.Right, DescriptionBounds.Bottom);

            return r;
        }

        protected override void OnPaintText(RibbonElementPaintEventArgs e)
        {
            if (e.Mode == RibbonElementSizeMode.DropDown)
            {
                var sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Near;

                Owner.Renderer.OnRenderRibbonItemText(new RibbonTextEventArgs(
                    Owner, e.Graphics, e.Clip, this, TextBounds, Text, Color.Empty, FontStyle.Bold, sf));

                sf.Alignment = StringAlignment.Near;

                Owner.Renderer.OnRenderRibbonItemText(new RibbonTextEventArgs(
                    Owner, e.Graphics, e.Clip, this, DescriptionBounds, Description, sf));
            }
            else
            {
                base.OnPaintText(e);
            }
        }

        #endregion
    }
}