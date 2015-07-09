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
    public class RibbonPanelGlyph
        : Glyph
    {
        private readonly BehaviorService _behaviorService;
        private readonly RibbonTab _tab;
        private RibbonTabDesigner _componentDesigner;
        private Size size;

        public RibbonPanelGlyph(BehaviorService behaviorService, RibbonTabDesigner designer, RibbonTab tab)
            : base(new RibbonPanelGlyphBehavior(designer, tab))
        {
            _behaviorService = behaviorService;
            _componentDesigner = designer;
            _tab = tab;
            size = new Size(60, 16);
        }

        public override Rectangle Bounds
        {
            get
            {
                if (!_tab.Active || !_tab.Owner.Tabs.Contains(_tab))
                {
                    return Rectangle.Empty;
                }
                Point edge = _behaviorService.ControlToAdornerWindow(_tab.Owner);
                var pnl = new Point(5, _tab.TabBounds.Bottom + 5); //_tab.Bounds.Y *2 + (_tab.Bounds.Height - size.Height) / 2);

                //If has panels
                if (_tab.Panels.Count > 0)
                {
                    //Place glyph next to the last panel
                    var p = _tab.Panels[_tab.Panels.Count - 1];
                    if (_tab.Owner.RightToLeft == RightToLeft.No)
                        pnl.X = p.Bounds.Right + 5;
                    else
                        pnl.X = p.Bounds.Left - 5 - size.Width;
                }

                return new Rectangle(
                    edge.X + pnl.X,
                    edge.Y + pnl.Y,
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
            using (GraphicsPath p = RibbonProfessionalRenderer.RoundRectangle(Bounds, 9))
            {
                using (var b = new SolidBrush(Color.FromArgb(50, Color.Blue)))
                {
                    pe.Graphics.FillPath(b, p);
                }
            }
            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            pe.Graphics.DrawString("Add Panel", SystemFonts.DefaultFont, Brushes.White, Bounds, sf);
            pe.Graphics.SmoothingMode = smbuff;
        }
    }

    public class RibbonPanelGlyphBehavior
        : Behavior
    {
        private readonly RibbonTabDesigner _designer;
        private RibbonTab _tab;

        public RibbonPanelGlyphBehavior(RibbonTabDesigner designer, RibbonTab tab)
        {
            _designer = designer;
            _tab = tab;
        }


        public override bool OnMouseUp(Glyph g, MouseButtons button)
        {
            _designer.AddPanel(this, System.EventArgs.Empty);
            return base.OnMouseUp(g, button);
        }
    }
}