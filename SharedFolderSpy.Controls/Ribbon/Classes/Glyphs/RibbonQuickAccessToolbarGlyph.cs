#region

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Designers;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Glyphs
{
    public class RibbonQuickAccessToolbarGlyph
        : Glyph

    {
        private readonly BehaviorService _behaviorService;
        private readonly Ribbon _ribbon;
        private RibbonDesigner _componentDesigner;

        public RibbonQuickAccessToolbarGlyph(BehaviorService behaviorService, RibbonDesigner designer, Ribbon ribbon)
            : base(new RibbonQuickAccessGlyphBehavior(designer, ribbon))
        {
            _behaviorService = behaviorService;
            _componentDesigner = designer;
            _ribbon = ribbon;
        }

        public override Rectangle Bounds
        {
            get
            {
                Point edge = _behaviorService.ControlToAdornerWindow(_ribbon);
                if (!_ribbon.CaptionBarVisible || !_ribbon.QuickAcessToolbar.Visible)
                {
                    return Rectangle.Empty;
                }
                if (_ribbon.RightToLeft == RightToLeft.No)
                {
                    return new Rectangle(
                        edge.X + _ribbon.QuickAcessToolbar.Bounds.Right + _ribbon.QuickAcessToolbar.Bounds.Height/2 + 4 + _ribbon.QuickAcessToolbar.DropDownButton.Bounds.Width,
                        edge.Y + _ribbon.QuickAcessToolbar.Bounds.Top, _ribbon.QuickAcessToolbar.Bounds.Height, _ribbon.QuickAcessToolbar.Bounds.Height);
                }
                return new Rectangle(
                    _ribbon.QuickAcessToolbar.Bounds.Left - _ribbon.QuickAcessToolbar.Bounds.Height/2 - 4 - _ribbon.QuickAcessToolbar.DropDownButton.Bounds.Width,
                    edge.Y + _ribbon.QuickAcessToolbar.Bounds.Top, _ribbon.QuickAcessToolbar.Bounds.Height, _ribbon.QuickAcessToolbar.Bounds.Height);
            }
        }

        public override Cursor GetHitTest(Point p)
        {
            if (Bounds.Contains(p))
            {
                return Cursors.Hand;
            }

            return null;
        }


        public override void Paint(PaintEventArgs pe)
        {
            if (_ribbon.CaptionBarVisible && _ribbon.QuickAcessToolbar.Visible)
            {
                SmoothingMode smbuff = pe.Graphics.SmoothingMode;
                pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var b = new SolidBrush(Color.FromArgb(50, Color.Blue)))
                {
                    pe.Graphics.FillEllipse(b, Bounds);
                }
                var sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                pe.Graphics.DrawString("+", SystemFonts.DefaultFont, Brushes.White, Bounds, sf);
                pe.Graphics.SmoothingMode = smbuff;
            }
        }
    }

    public class RibbonQuickAccessGlyphBehavior
        : Behavior
    {
        private readonly RibbonDesigner _designer;
        private readonly Ribbon _ribbon;

        public RibbonQuickAccessGlyphBehavior(RibbonDesigner designer, Ribbon ribbon)
        {
            _designer = designer;
            _ribbon = ribbon;
        }


        public override bool OnMouseUp(Glyph g, MouseButtons button)
        {
            _designer.CreateItem(_ribbon, _ribbon.QuickAcessToolbar.Items, typeof (RibbonButton));
            return base.OnMouseUp(g, button);
        }
    }
}