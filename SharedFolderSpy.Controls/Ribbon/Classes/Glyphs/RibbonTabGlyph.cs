#region

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Designers;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Renderers;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Glyphs
{
    public class RibbonTabGlyph
        : Glyph
    {
        private readonly BehaviorService _behaviorService;
        private readonly Ribbon _ribbon;
        private RibbonDesigner _componentDesigner;
        private Size size;

        public RibbonTabGlyph(BehaviorService behaviorService, RibbonDesigner designer, Ribbon ribbon)
            : base(new RibbonTabGlyphBehavior(designer, ribbon))
        {
            _behaviorService = behaviorService;
            _componentDesigner = designer;
            _ribbon = ribbon;
            size = new Size(60, 16);
        }

        public override Rectangle Bounds
        {
            get
            {
                Point edge = _behaviorService.ControlToAdornerWindow(_ribbon);
                var tab = new Point(5, _ribbon.OrbBounds.Bottom + 5);

                //If has tabs
                if (_ribbon.Tabs.Count > 0 && _ribbon.RightToLeft == RightToLeft.No)
                {
                    //Place glyph next to the last tab
                    var t = _ribbon.Tabs[_ribbon.Tabs.Count - 1];
                    tab.X = t.Bounds.Right + 5;
                    tab.Y = t.Bounds.Top + 2;
                }
                else if (_ribbon.Tabs.Count > 0 && _ribbon.RightToLeft == RightToLeft.Yes)
                {
                    //Place glyph next to the first tab
                    var t = _ribbon.Tabs[_ribbon.Tabs.Count - 1];
                    tab.X = t.Bounds.Left - 5 - size.Width;
                    tab.Y = t.Bounds.Top + 2;
                }
                return new Rectangle(
                    edge.X + tab.X,
                    edge.Y + tab.Y,
                    size.Width, size.Height);
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
            SmoothingMode smbuff = pe.Graphics.SmoothingMode;
            pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath p = RibbonProfessionalRenderer.RoundRectangle(Bounds, 2))
            {
                using (var b = new SolidBrush(Color.FromArgb(50, Color.Blue)))
                {
                    pe.Graphics.FillPath(b, p);
                }
            }
            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            pe.Graphics.DrawString("Add Tab", SystemFonts.DefaultFont, Brushes.White, Bounds, sf);
            pe.Graphics.SmoothingMode = smbuff;
        }
    }

    public class RibbonTabGlyphBehavior
        : Behavior
    {
        private readonly RibbonDesigner _designer;
        private Ribbon _ribbon;

        public RibbonTabGlyphBehavior(RibbonDesigner designer, Ribbon ribbon)
        {
            _designer = designer;
            _ribbon = ribbon;
        }


        public override bool OnMouseUp(Glyph g, MouseButtons button)
        {
            _designer.AddTabVerb(this, System.EventArgs.Empty);
            return base.OnMouseUp(g, button);
        }
    }
}