#region

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Designers;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Enums;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Renderers
{
    public class RibbonProfessionalRenderer
        : RibbonRenderer
    {
        #region Types

        public enum Corners
        {
            None = 0,
            NorthWest = 2,
            NorthEast = 4,
            SouthEast = 8,
            SouthWest = 16,
            All = NorthWest | NorthEast | SouthEast | SouthWest,
            North = NorthWest | NorthEast,
            South = SouthEast | SouthWest,
            East = NorthEast | SouthEast,
            West = NorthWest | SouthWest
        }

        #endregion

        #region Fields

        private Size arrowSize = new Size(5, 3);
        private Size moreSize = new Size(7, 7);

        #endregion

        #region Ctor

        #endregion

        #region Props

        //private RibbonProfesionalRendererColorTable _colorTable;

        //public RibbonProfesionalRendererColorTable ColorTable
        //{
        //    get { return _colorTable; }
        //    set { _colorTable = value; }
        //}

        #endregion

        #region Methods

        #region Util

        /// <summary>
        ///     Centers the specified rectangle on the specified container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public Rectangle CenterOn(Rectangle container, Rectangle r)
        {
            var result = new Rectangle(
                container.Left + ((container.Width - r.Width)/2),
                container.Top + ((container.Height - r.Height)/2),
                r.Width, r.Height);

            return result;
        }

        /// <summary>
        ///     Draws an arrow on the specified bounds
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        /// <param name="c"></param>
        public void DrawArrow(Graphics g, Rectangle b, Color c, RibbonArrowDirection d)
        {
            var path = new GraphicsPath();
            var bounds = b;

            if (b.Width%2 != 0 && (d == RibbonArrowDirection.Up))
                bounds = new Rectangle(new Point(b.Left - 1, b.Top - 1), new Size(b.Width + 1, b.Height + 1));

            if (d == RibbonArrowDirection.Up)
            {
                path.AddLine(bounds.Left, bounds.Bottom, bounds.Right, bounds.Bottom);
                path.AddLine(bounds.Right, bounds.Bottom, bounds.Left + bounds.Width/2, bounds.Top);
            }
            else if (d == RibbonArrowDirection.Down)
            {
                path.AddLine(bounds.Left, bounds.Top, bounds.Right, bounds.Top);
                path.AddLine(bounds.Right, bounds.Top, bounds.Left + bounds.Width/2, bounds.Bottom);
            }
            else if (d == RibbonArrowDirection.Left)
            {
                path.AddLine(bounds.Left, bounds.Top, bounds.Right, bounds.Top + bounds.Height/2);
                path.AddLine(bounds.Right, bounds.Top + bounds.Height/2, bounds.Left, bounds.Bottom);
            }
            else
            {
                path.AddLine(bounds.Right, bounds.Top, bounds.Left, bounds.Top + bounds.Height/2);
                path.AddLine(bounds.Left, bounds.Top + bounds.Height/2, bounds.Right, bounds.Bottom);
            }

            path.CloseFigure();

            using (var bb = new SolidBrush(c))
            {
                var sm = g.SmoothingMode;
                g.SmoothingMode = SmoothingMode.None;
                g.FillPath(bb, path);
                g.SmoothingMode = sm;
            }

            path.Dispose();
        }

        /// <summary>
        ///     Draws the pair of arrows that make a shadded arrow, centered on the specified bounds
        /// </summary>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="d"></param>
        /// <param name="enabled"></param>
        public void DrawArrowShaded(Graphics g, Rectangle b, RibbonArrowDirection d, bool enabled)
        {
            var arrSize = arrowSize;

            if (d == RibbonArrowDirection.Left || d == RibbonArrowDirection.Right)
            {
                //Invert size
                arrSize = new Size(arrowSize.Height, arrowSize.Width);
            }

            var arrowP = new Point(
                b.Left + (b.Width - arrSize.Width)/2,
                b.Top + (b.Height - arrSize.Height)/2
                );

            var bounds = new Rectangle(arrowP, arrSize);
            var boundsLight = bounds;
            boundsLight.Offset(0, 1);

            var lt = Theme.ColorTable.ArrowLight;
            var dk = Theme.ColorTable.Arrow;

            if (!enabled)
            {
                lt = Color.Transparent;
                dk = Theme.ColorTable.ArrowDisabled;
            }

            DrawArrow(g, boundsLight, lt, d);
            DrawArrow(g, bounds, dk, d);
        }

        /// <summary>
        ///     Draws a dot of the grip
        /// </summary>
        /// <param name="g"></param>
        /// <param name="location"></param>
        public void DrawGripDot(Graphics g, Point location)
        {
            var lt = new Rectangle(location.X - 1, location.Y + 1, 2, 2);
            var dk = new Rectangle(location, new Size(2, 2));

            using (var b = new SolidBrush(Theme.ColorTable.DropDownGripLight))
            {
                g.FillRectangle(b, lt);
            }

            using (var b = new SolidBrush(Theme.ColorTable.DropDownGripDark))
            {
                g.FillRectangle(b, dk);
            }
        }

        /// <summary>
        ///     Draws a shadow that indicates that the element is pressed
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        public void DrawPressedShadow(Graphics g, Rectangle r)
        {
            var shadow = Rectangle.FromLTRB(r.Left, r.Top, r.Right, r.Top + 4);

            using (var path = RoundRectangle(shadow, 3, Corners.NorthEast | Corners.NorthWest))
            {
                using (var b = new LinearGradientBrush(shadow,
                    Color.FromArgb(50, Color.Black),
                    Color.FromArgb(0, Color.Black),
                    90))
                {
                    b.WrapMode = WrapMode.TileFlipXY;
                    g.FillPath(b, path);
                }
            }
        }

        public static GraphicsPath FlatRectangle(Rectangle r)
        {
            var path = new GraphicsPath();

            path.AddLine(r.Left, r.Top, r.Right, r.Top);
            path.AddLine(r.Right, r.Top, r.Right, r.Bottom);
            path.AddLine(r.Right, r.Bottom, r.Left, r.Bottom);
            path.AddLine(r.Left, r.Bottom, r.Left, r.Top);
            path.CloseFigure();

            return path;
        }

        public Color GetTextColor(bool enabled, Color alternative)
        {
            if (enabled)
            {
                return alternative;
            }
            return Theme.ColorTable.ArrowDisabled;
        }

        /// <summary>
        ///     Draws a rectangle with a vertical gradient
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        /// <param name="northColor"></param>
        /// <param name="southColor"></param>
        private void GradientRect(Graphics g, Rectangle r, Color northColor, Color southColor)
        {
            using (Brush b = new LinearGradientBrush(
                new Point(r.X, r.Y - 1), new Point(r.Left, r.Bottom), northColor, southColor))
            {
                g.FillRectangle(b, r);
            }
        }

        /// <summary>
        ///     Creates a rectangle with rounded corners
        /// </summary>
        /// <param name="r"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static GraphicsPath RoundRectangle(Rectangle r, int radius)
        {
            return RoundRectangle(r, radius, Corners.All);
        }

        /// <summary>
        ///     Creates a rectangle with the specified corners rounded
        /// </summary>
        /// <param name="r"></param>
        /// <param name="radius"></param>
        /// <param name="corners"></param>
        /// <returns></returns>
        public static GraphicsPath RoundRectangle(Rectangle r, int radius, Corners corners)
        {
            var path = new GraphicsPath();
            var d = radius*2;

            var nw = (corners & Corners.NorthWest) == Corners.NorthWest ? d : 0;
            var ne = (corners & Corners.NorthEast) == Corners.NorthEast ? d : 0;
            var se = (corners & Corners.SouthEast) == Corners.SouthEast ? d : 0;
            var sw = (corners & Corners.SouthWest) == Corners.SouthWest ? d : 0;

            path.AddLine(r.Left + nw, r.Top, r.Right - ne, r.Top);

            if (ne > 0)
            {
                path.AddArc(Rectangle.FromLTRB(r.Right - ne, r.Top, r.Right, r.Top + ne),
                    -90, 90);
            }

            path.AddLine(r.Right, r.Top + ne, r.Right, r.Bottom - se);

            if (se > 0)
            {
                path.AddArc(Rectangle.FromLTRB(r.Right - se, r.Bottom - se, r.Right, r.Bottom),
                    0, 90);
            }

            path.AddLine(r.Right - se, r.Bottom, r.Left + sw, r.Bottom);

            if (sw > 0)
            {
                path.AddArc(Rectangle.FromLTRB(r.Left, r.Bottom - sw, r.Left + sw, r.Bottom),
                    90, 90);
            }

            path.AddLine(r.Left, r.Bottom - sw, r.Left, r.Top + nw);

            if (nw > 0)
            {
                path.AddArc(Rectangle.FromLTRB(r.Left, r.Top, r.Left + nw, r.Top + nw),
                    180, 90);
            }

            path.CloseFigure();

            return path;
        }

        #endregion

        #region Tab

        /// <summary>
        ///     Creates the path of the tab and its contents
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        public GraphicsPath CreateCompleteTabPath_2010(RibbonTab t)
        {
            //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
            var path = new GraphicsPath();
            var corner = 6;

            path.AddLine(t.TabBounds.Left + corner, t.TabBounds.Top,
                t.TabBounds.Right - corner, t.TabBounds.Top);
            path.AddArc(
                Rectangle.FromLTRB(t.TabBounds.Right - corner, t.TabBounds.Top, t.TabBounds.Right, t.TabBounds.Top + corner),
                -90, 90);
            path.AddLine(t.TabBounds.Right, t.TabBounds.Top + corner,
                t.TabBounds.Right, t.TabBounds.Bottom - corner);
            path.AddArc(Rectangle.FromLTRB(
                t.TabBounds.Right, t.TabBounds.Bottom - corner, t.TabBounds.Right + corner, t.TabBounds.Bottom),
                -180, -90);
            path.AddLine(t.TabBounds.Right + corner, t.TabBounds.Bottom, t.TabContentBounds.Right - corner, t.TabBounds.Bottom);
            path.AddArc(Rectangle.FromLTRB(
                t.TabContentBounds.Right - corner, t.TabBounds.Bottom, t.TabContentBounds.Right, t.TabBounds.Bottom + corner),
                -90, 90);
            path.AddLine(t.TabContentBounds.Right, t.TabContentBounds.Top + corner, t.TabContentBounds.Right, t.TabContentBounds.Bottom - corner);
            path.AddArc(Rectangle.FromLTRB(
                t.TabContentBounds.Right - corner, t.TabContentBounds.Bottom - corner, t.TabContentBounds.Right, t.TabContentBounds.Bottom),
                0, 90);
            path.AddLine(t.TabContentBounds.Right - corner, t.TabContentBounds.Bottom, t.TabContentBounds.Left + corner, t.TabContentBounds.Bottom);
            path.AddArc(Rectangle.FromLTRB(
                t.TabContentBounds.Left, t.TabContentBounds.Bottom - corner, t.TabContentBounds.Left + corner, t.TabContentBounds.Bottom),
                90, 90);
            path.AddLine(t.TabContentBounds.Left, t.TabContentBounds.Bottom - corner, t.TabContentBounds.Left, t.TabBounds.Bottom + corner);
            path.AddArc(Rectangle.FromLTRB(
                t.TabContentBounds.Left, t.TabBounds.Bottom, t.TabContentBounds.Left + corner, t.TabBounds.Bottom + corner),
                180, 90);
            path.AddLine(t.TabContentBounds.Left + corner, t.TabContentBounds.Top, t.TabBounds.Left - corner, t.TabBounds.Bottom);
            path.AddArc(Rectangle.FromLTRB(
                t.TabBounds.Left - corner, t.TabBounds.Bottom - corner, t.TabBounds.Left, t.TabBounds.Bottom),
                90, -90);
            path.AddLine(t.TabBounds.Left, t.TabBounds.Bottom - corner, t.TabBounds.Left, t.TabBounds.Top + corner);
            path.AddArc(Rectangle.FromLTRB(
                t.TabBounds.Left, t.TabBounds.Top, t.TabBounds.Left + corner, t.TabBounds.Top + corner),
                180, 90);
            path.CloseFigure();

            return path;
        }

        public GraphicsPath CreateCompleteTabPath_2013(RibbonTab t)
        {
            //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
            var path = new GraphicsPath();
            //int corner = 6;

            path.AddLine(t.TabBounds.Left, t.TabBounds.Top, t.TabBounds.Right, t.TabBounds.Top);
            path.AddLine(t.TabBounds.Right, t.TabBounds.Top, t.TabBounds.Right, t.TabBounds.Bottom);
            path.AddLine(t.TabBounds.Right, t.TabBounds.Bottom, t.TabContentBounds.Right, t.TabBounds.Bottom);
            path.AddLine(t.TabContentBounds.Right, t.TabContentBounds.Top, t.TabContentBounds.Right, t.TabContentBounds.Bottom);
            path.AddLine(t.TabContentBounds.Right, t.TabContentBounds.Bottom, t.TabContentBounds.Left, t.TabContentBounds.Bottom);
            path.AddLine(t.TabContentBounds.Left, t.TabContentBounds.Bottom, t.TabContentBounds.Left, t.TabBounds.Bottom);
            path.AddLine(t.TabContentBounds.Left, t.TabContentBounds.Top, t.TabBounds.Left, t.TabBounds.Bottom);
            path.AddLine(t.TabBounds.Left, t.TabBounds.Bottom, t.TabBounds.Left, t.TabBounds.Top);

            path.CloseFigure();

            return path;
        }

        /// <summary>
        ///     Creates the path of the tab and its contents
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        public GraphicsPath CreateTabPath_2010(RibbonTab t)
        {
            //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
            var path = new GraphicsPath();
            var corner = 6;
            var rightOffset = 1;

            path.AddLine(t.TabBounds.Left, t.TabBounds.Bottom, t.TabBounds.Left, t.TabBounds.Top + corner);
            path.AddArc(new Rectangle(t.TabBounds.Left, t.TabBounds.Top, corner, corner), 180, 90);
            path.AddLine(t.TabBounds.Left + corner, t.TabBounds.Top, t.TabBounds.Right - corner - rightOffset, t.TabBounds.Top);
            path.AddArc(new Rectangle(t.TabBounds.Right - corner - rightOffset, t.TabBounds.Top, corner, corner), -90, 90);
            path.AddLine(t.TabBounds.Right - rightOffset, t.TabBounds.Top + corner, t.TabBounds.Right - rightOffset, t.TabBounds.Bottom);

            return path;
        }

        public GraphicsPath CreateTabPath_2013(RibbonTab t)
        {
            //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
            var path = new GraphicsPath();
            var rightOffset = 1;

            path.AddLine(t.TabBounds.Left, t.TabBounds.Bottom, t.TabBounds.Left, t.TabBounds.Top);
            path.AddLine(t.TabBounds.Left, t.TabBounds.Top, t.TabBounds.Right - rightOffset, t.TabBounds.Top);
            path.AddLine(t.TabBounds.Right - rightOffset, t.TabBounds.Top, t.TabBounds.Right - rightOffset, t.TabBounds.Bottom);

            return path;
        }

        /// <summary>
        ///     Draws a complete tab
        /// </summary>
        /// <param name="e"></param>
        //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
        public void DrawCompleteTab(RibbonTabRenderEventArgs e)
        {
            DrawTabActive(e);

            if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2010)
            {
                //Background gradient
                using (var path = RoundRectangle(e.Tab.TabContentBounds, 4))
                {
                    var north = Theme.ColorTable.TabContentNorth;
                    var south = Theme.ColorTable.TabContentSouth;

                    if (e.Tab.Contextual)
                    {
                        north = Theme.ColorTable.DropDownBg;
                        south = north;
                    }
                    var tabCenter = e.Tab.TabContentBounds.Height/2;

                    using (var b = new LinearGradientBrush(
                        new Point(0, e.Tab.TabContentBounds.Top + tabCenter),
                        new Point(0, e.Tab.TabContentBounds.Bottom - 10), north, south))
                    {
                        b.WrapMode = WrapMode.TileFlipXY;
                        e.Graphics.FillPath(b, path);
                    }
                }

                //Glossy effect
                var glossy = Rectangle.FromLTRB(e.Tab.TabContentBounds.Left, e.Tab.TabContentBounds.Top + 0, e.Tab.TabContentBounds.Right, e.Tab.TabContentBounds.Top + 18);
                using (var path = RoundRectangle(glossy, 6, Corners.NorthWest | Corners.NorthEast))
                {
                    using (Brush b = new SolidBrush(Color.FromArgb(30, Color.White)))
                    {
                        e.Graphics.FillPath(b, path);
                    }
                }

                //Tab border

                using (var path = CreateCompleteTabPath_2010(e.Tab))
                {
                    using (var p = new Pen(Theme.ColorTable.TabBorder))
                    {
                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        e.Graphics.DrawPath(p, path);
                    }
                }

                if (e.Tab.Selected)
                {
                    //Selected glow
                    using (var path = CreateTabPath_2010(e.Tab))
                    {
                        var p = new Pen(Color.FromArgb(150, Color.Gold));
                        p.Width = 2;

                        e.Graphics.DrawPath(p, path);

                        p.Dispose();
                    }
                }
            }
            else if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2013)
            {
                //Background
                using (var path = FlatRectangle(e.Tab.TabContentBounds))
                {
                    using (var b = new SolidBrush(Theme.ColorTable.TabCompleteBackground_2013))
                    {
                        e.Graphics.FillPath(b, path);
                    }
                }

                //Tab border
                using (var path = CreateCompleteTabPath_2013(e.Tab))
                {
                    using (var p = new Pen(Theme.ColorTable.TabBorder_2013))
                    {
                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        e.Graphics.DrawPath(p, path);
                    }
                }
            }
        }

        /// <summary>
        ///     Draws an active tab
        /// </summary>
        /// <param name="e"></param>
        //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
        public void DrawTabActive(RibbonTabRenderEventArgs e)
        {
            DrawTabNormal(e);

            if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2010)
            {
                var glossy = new Rectangle(e.Tab.TabBounds.Left, e.Tab.TabBounds.Top, e.Tab.TabBounds.Width, 4);
                var shadow = e.Tab.TabBounds;
                shadow.Offset(2, 1);
                var tab = e.Tab.TabBounds; //tab.Inflate(0, 1);

                using (var path = RoundRectangle(shadow, 6, Corners.NorthWest | Corners.NorthEast))
                {
                    using (var b = new PathGradientBrush(path))
                    {
                        b.WrapMode = WrapMode.Clamp;

                        var cb = new ColorBlend(3);
                        cb.Colors = new[]
                        {
                            Color.Transparent,
                            Color.FromArgb(50, Color.Black),
                            Color.FromArgb(100, Color.Black)
                        };
                        cb.Positions = new[] {0f, .1f, 1f};

                        b.InterpolationColors = cb;

                        e.Graphics.FillPath(b, path);
                    }
                }

                using (var path = RoundRectangle(tab, 6, Corners.North))
                {
                    var north = Theme.ColorTable.TabNorth;
                    var south = Theme.ColorTable.TabSouth;

                    if (e.Tab.Contextual)
                    {
                        north = e.Tab.Context.GlowColor;
                        south = Color.FromArgb(10, north);
                    }

                    using (var p = new Pen(Theme.ColorTable.TabNorth, 1.6f))
                    {
                        e.Graphics.DrawPath(p, path);
                    }

                    using (var b = new LinearGradientBrush(
                        e.Tab.TabBounds, Theme.ColorTable.TabNorth, Theme.ColorTable.TabSouth, 90))
                    {
                        e.Graphics.FillPath(b, path);
                    }
                }

                using (var path = RoundRectangle(glossy, 6, Corners.North))
                {
                    using (Brush b = new SolidBrush(Color.FromArgb(180, Color.White)))
                    {
                        e.Graphics.FillPath(b, path);
                    }
                }
            }
            else if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2013)
            {
                var tab = e.Tab.TabBounds; //tab.Inflate(0, 1);

                using (var path = FlatRectangle(tab))
                {
                    using (var p = new Pen(Theme.ColorTable.TabActiveBorder_2013, 1.6f))
                    {
                        e.Graphics.DrawPath(p, path);
                    }

                    using (var b = new SolidBrush(Theme.ColorTable.TabActiveBackbround_2013))
                    {
                        e.Graphics.FillPath(b, path);
                    }
                }
            }
        }

        /// <summary>
        ///     Draws a selected tab
        /// </summary>
        /// <param name="e"></param>
        //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
        public void DrawTabMinimized(RibbonTabRenderEventArgs e)
        {
            if (e.Tab.Selected)
            {
                if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2010)
                {
                    //background
                    var outerR = Rectangle.FromLTRB(e.Tab.TabBounds.Left, e.Tab.TabBounds.Top, e.Tab.TabBounds.Right - 1, e.Tab.TabBounds.Bottom);
                    var innerR = Rectangle.FromLTRB(outerR.Left + 1, outerR.Top + 1, outerR.Right - 1, outerR.Bottom);
                    var glossyR = Rectangle.FromLTRB(innerR.Left + 1, innerR.Top + 1, innerR.Right - 1, innerR.Top + e.Tab.TabBounds.Height);

                    var outer = RoundRectangle(outerR, 3, Corners.NorthEast | Corners.NorthWest);
                    var inner = RoundRectangle(innerR, 3, Corners.NorthEast | Corners.NorthWest);
                    var glossy = RoundRectangle(glossyR, 3, Corners.NorthEast | Corners.NorthWest);

                    using (var b = new SolidBrush(Color.FromArgb(100, Color.White)))
                    {
                        e.Graphics.FillPath(b, glossy);
                    }

                    //Tab border
                    using (var path = CreateTabPath_2010(e.Tab))
                    {
                        using (var p = new Pen(Theme.ColorTable.TabBorder))
                        {
                            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            e.Graphics.DrawPath(p, path);
                        }
                    }

                    //Selected glow
                    using (var path = CreateTabPath_2010(e.Tab))
                    {
                        var p = new Pen(Color.FromArgb(150, Color.FromArgb(252, 184, 11)));
                        p.Width = 2;

                        e.Graphics.DrawPath(p, path);

                        p.Dispose();
                    }
                }
                else if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2013)
                {
                }
            }
            else
            {
                var lastClip = e.Graphics.ClipBounds;

                var clip = Rectangle.FromLTRB(
                    e.Tab.TabBounds.Left,
                    e.Tab.TabBounds.Top,
                    e.Tab.TabBounds.Right,
                    e.Tab.TabBounds.Bottom);

                var r = Rectangle.FromLTRB(
                    e.Tab.TabBounds.Left - 1,
                    e.Tab.TabBounds.Top - 1,
                    e.Tab.TabBounds.Right,
                    e.Tab.TabBounds.Bottom);

                e.Graphics.SetClip(clip);

                using (Brush b = new SolidBrush(Theme.ColorTable.RibbonBackground))
                {
                    e.Graphics.FillRectangle(b, r);
                }

                e.Graphics.SetClip(lastClip);
            }
        }

        /// <summary>
        ///     Draws a complete tab
        /// </summary>
        /// <param name="e"></param>
        public void DrawTabNormal(RibbonTabRenderEventArgs e)
        {
            var lastClip = e.Graphics.ClipBounds;

            var clip = Rectangle.FromLTRB(
                e.Tab.TabBounds.Left,
                e.Tab.TabBounds.Top,
                e.Tab.TabBounds.Right,
                e.Tab.TabBounds.Bottom);

            var r = Rectangle.FromLTRB(
                e.Tab.TabBounds.Left - 1,
                e.Tab.TabBounds.Top - 1,
                e.Tab.TabBounds.Right,
                e.Tab.TabBounds.Bottom);

            e.Graphics.SetClip(clip);

            //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
            if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2007 | e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2010)
            {
                using (Brush b = new SolidBrush(Theme.ColorTable.RibbonBackground))
                {
                    e.Graphics.FillRectangle(b, r);
                }
            }
            else if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2013)
            {
                using (Brush b = new SolidBrush(Theme.ColorTable.TabNormalBackground_2013))
                {
                    e.Graphics.FillRectangle(b, r);
                }
            }

            e.Graphics.SetClip(lastClip);
        }

        /// <summary>
        ///     Draws a pressed tab
        /// </summary>
        /// <param name="e"></param>
        public void DrawTabPressed(RibbonTabRenderEventArgs e)
        {
        }

        /// <summary>
        ///     Draws a selected tab
        /// </summary>
        /// <param name="e"></param>
        public void DrawTabSelected(RibbonTabRenderEventArgs e)
        {
            var outerR = Rectangle.FromLTRB(
                e.Tab.TabBounds.Left,
                e.Tab.TabBounds.Top,
                e.Tab.TabBounds.Right - 1,
                e.Tab.TabBounds.Bottom);
            var innerR = Rectangle.FromLTRB(
                outerR.Left + 1,
                outerR.Top + 1,
                outerR.Right - 1,
                outerR.Bottom);

            var glossyR = Rectangle.FromLTRB(
                innerR.Left + 1,
                innerR.Top + 1,
                innerR.Right - 1,
                innerR.Top + e.Tab.TabBounds.Height/2);

            var outer = RoundRectangle(outerR, 3, Corners.NorthEast | Corners.NorthWest);
            var inner = RoundRectangle(innerR, 3, Corners.NorthEast | Corners.NorthWest);
            var glossy = RoundRectangle(glossyR, 3, Corners.NorthEast | Corners.NorthWest);

            using (var p = new Pen(Theme.ColorTable.TabBorder))
            {
                e.Graphics.DrawPath(p, outer);
            }

            using (var p = new Pen(Color.FromArgb(200, Color.White)))
            {
                e.Graphics.DrawPath(p, inner);
            }

            using (var radialPath = new GraphicsPath())
            {
                radialPath.AddRectangle(innerR);
                //radialPath.AddEllipse(innerR);
                radialPath.CloseFigure();

                var gr = new PathGradientBrush(radialPath);
                gr.CenterPoint = new PointF(
                    Convert.ToSingle(innerR.Left + innerR.Width/2),
                    Convert.ToSingle(innerR.Top - 5));
                gr.CenterColor = Color.Transparent;
                gr.SurroundColors = new[] {Theme.ColorTable.TabSelectedGlow};

                var blend = new Blend(3);
                blend.Factors = new[] {0.0f, 0.9f, 0.0f};
                blend.Positions = new[] {0.0f, 0.8f, 1.0f};

                gr.Blend = blend;

                e.Graphics.FillPath(gr, radialPath);

                gr.Dispose();
            }
            using (var b = new SolidBrush(Color.FromArgb(100, Color.White)))
            {
                e.Graphics.FillPath(b, glossy);
            }

            outer.Dispose();
            inner.Dispose();
            glossy.Dispose();
        }

        #endregion

        #region Panel

        public void DrawButtonMoreGlyph(Graphics g, Rectangle b, bool enabled)
        {
            var dark = enabled ? Theme.ColorTable.Arrow : Theme.ColorTable.ArrowDisabled;
            var light = Theme.ColorTable.ArrowLight;

            var bounds = CenterOn(b, new Rectangle(Point.Empty, moreSize));
            var boundsLight = bounds;
            boundsLight.Offset(1, 1);

            DrawButtonMoreGlyph(g, boundsLight.Location, light);
            DrawButtonMoreGlyph(g, bounds.Location, dark);
        }

        public void DrawButtonMoreGlyph(Graphics gr, Point p, Color color)
        {
            /*
             
             a-------b-+
             |         |
             |         d
             c     g   |
             |         |
             +----e----f
             
             */

            var a = p;
            var b = new Point(p.X + moreSize.Width - 1, p.Y);
            var c = new Point(p.X, p.Y + moreSize.Height - 1);
            var f = new Point(p.X + moreSize.Width, p.Y + moreSize.Height);
            var d = new Point(f.X, f.Y - 3);
            var e = new Point(f.X - 3, f.Y);
            var g = new Point(f.X - 3, f.Y - 3);

            var lastMode = gr.SmoothingMode;

            gr.SmoothingMode = SmoothingMode.None;

            using (var pen = new Pen(color))
            {
                gr.DrawLine(pen, a, b);
                gr.DrawLine(pen, a, c);
                gr.DrawLine(pen, e, f);
                gr.DrawLine(pen, d, f);
                gr.DrawLine(pen, e, d);
                gr.DrawLine(pen, g, f);
            }

            gr.SmoothingMode = lastMode;
        }

        /// <summary>
        ///     Draws a panel in normal state (unselected)
        /// </summary>
        /// <param name="e"></param>
        public void DrawPanelNormal(RibbonPanelRenderEventArgs e)
        {
            if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2007 | e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2010) //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
            {
                var darkBorder = Rectangle.FromLTRB(
                    e.Panel.Bounds.Left,
                    e.Panel.Bounds.Top,
                    e.Panel.Bounds.Right,
                    e.Panel.Bounds.Bottom);

                var lightBorder = Rectangle.FromLTRB(
                    e.Panel.Bounds.Left + 1,
                    e.Panel.Bounds.Top + 1,
                    e.Panel.Bounds.Right + 1,
                    e.Panel.Bounds.Bottom);

                var textArea = Rectangle.FromLTRB(
                    e.Panel.Bounds.Left + 1,
                    e.Panel.ContentBounds.Bottom,
                    e.Panel.Bounds.Right - 1,
                    e.Panel.Bounds.Bottom - 1);

                var dark = RoundRectangle(darkBorder, 3);
                var light = RoundRectangle(lightBorder, 3);
                var txt = RoundRectangle(textArea, 3, Corners.SouthEast | Corners.SouthWest);

                using (var p = new Pen(Theme.ColorTable.PanelLightBorder))
                {
                    e.Graphics.DrawPath(p, light);
                }
                using (var p = new Pen(Theme.ColorTable.PanelDarkBorder))
                {
                    e.Graphics.DrawPath(p, dark);
                }

                using (var b = new SolidBrush(Theme.ColorTable.PanelTextBackground))
                {
                    e.Graphics.FillPath(b, txt);
                }

                txt.Dispose();
                dark.Dispose();
                light.Dispose();
            }
            else if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2013) //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
            {
                using (var p = new Pen(Theme.ColorTable.PanelBorder_2013))
                {
                    e.Graphics.DrawLine(p, new Point(e.Panel.Bounds.Right, e.Panel.Bounds.Top), new Point(e.Panel.Bounds.Right, e.Panel.Bounds.Bottom));
                }
            }

            if (e.Panel.ButtonMoreVisible)
            {
                DrawButtonMoreGlyph(e.Graphics, e.Panel.ButtonMoreBounds, e.Panel.ButtonMoreEnabled && e.Panel.Enabled);
            }
        }

        /// <summary>
        ///     Draws the image of the panel when collapsed
        /// </summary>
        /// <param name="e"></param>
        public void DrawPanelOverflowImage(RibbonPanelRenderEventArgs e)
        {
            var margin = 3;
            var imgSquareSize = new Size(32, 32);
            var imgSquareR = new Rectangle(new Point(
                e.Panel.Bounds.Left + (e.Panel.Bounds.Width - imgSquareSize.Width)/2,
                e.Panel.Bounds.Top + 5), imgSquareSize);

            var imgSquareBottomR = Rectangle.FromLTRB(
                imgSquareR.Left, imgSquareR.Bottom - 10, imgSquareR.Right, imgSquareR.Bottom);

            var textR = Rectangle.FromLTRB(
                e.Panel.Bounds.Left + margin,
                imgSquareR.Bottom + margin,
                e.Panel.Bounds.Right - margin,
                e.Panel.Bounds.Bottom - margin);

            using (var imgSq = RoundRectangle(imgSquareR, 5))
            {
                using (var imgSqB = RoundRectangle(imgSquareBottomR, 5, Corners.South))
                {
                    using (var b = new LinearGradientBrush(
                        imgSquareR, Theme.ColorTable.TabContentNorth, Theme.ColorTable.TabContentSouth, 90
                        ))
                    {
                        e.Graphics.FillPath(b, imgSq);
                    }

                    using (var b = new SolidBrush(Theme.ColorTable.PanelTextBackground))
                    {
                        e.Graphics.FillPath(b, imgSqB);
                    }

                    using (var p = new Pen(Theme.ColorTable.PanelDarkBorder))
                    {
                        e.Graphics.DrawPath(p, imgSq);
                    }

                    if (e.Panel.Image != null)
                    {
                        e.Graphics.DrawImage(e.Panel.Image,
                            imgSquareR.Left + (imgSquareR.Width - e.Panel.Image.Width)/2,
                            imgSquareR.Top + ((imgSquareR.Height - imgSquareBottomR.Height) - e.Panel.Image.Height)/2, e.Panel.Image.Width, e.Panel.Image.Height);
                    }

                    if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2007 | e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2010)
                    {
                        using (var b = new SolidBrush(GetTextColor(e.Panel.Enabled, Theme.ColorTable.Text)))
                        {
                            var sf = new StringFormat();
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Near;
                            sf.Trimming = StringTrimming.Character;

                            e.Graphics.DrawString(e.Panel.Text, e.Ribbon.Font, b, textR, sf);
                        }
                    }
                    else if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2013)
                    {
                        using (var b = new SolidBrush(GetTextColor(e.Panel.Enabled, Theme.ColorTable.RibbonItemText_2013)))
                        {
                            var sf = new StringFormat();
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Near;
                            sf.Trimming = StringTrimming.Character;

                            e.Graphics.DrawString(e.Panel.Text, e.Ribbon.Font, b, textR, sf);
                        }
                    }

                    if (e.Panel.Text != null)
                    {
                        var bounds = LargeButtonDropDownArrowBounds(e.Graphics, e.Panel.Owner.Font, e.Panel.Text, textR);

                        if (bounds.Right < e.Panel.Bounds.Right)
                        {
                            var boundsLight = bounds;
                            boundsLight.Offset(0, 1);
                            var lt = Theme.ColorTable.ArrowLight;
                            var dk = Theme.ColorTable.Arrow;

                            DrawArrow(e.Graphics, boundsLight, lt, RibbonArrowDirection.Down);
                            DrawArrow(e.Graphics, bounds, dk, RibbonArrowDirection.Down);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Draws an overflown panel in normal state
        /// </summary>
        /// <param name="e"></param>
        public void DrawPanelOverflowNormal(RibbonPanelRenderEventArgs e)
        {
            var darkBorder = Rectangle.FromLTRB(
                e.Panel.Bounds.Left,
                e.Panel.Bounds.Top,
                e.Panel.Bounds.Right,
                e.Panel.Bounds.Bottom);

            var lightBorder = Rectangle.FromLTRB(
                e.Panel.Bounds.Left + 1,
                e.Panel.Bounds.Top + 1,
                e.Panel.Bounds.Right - 1,
                e.Panel.Bounds.Bottom - 1);


            var dark = RoundRectangle(darkBorder, 3);
            var light = RoundRectangle(lightBorder, 3);

            using (var p = new Pen(Theme.ColorTable.PanelLightBorder))
            {
                e.Graphics.DrawPath(p, light);
            }

            using (var p = new Pen(Theme.ColorTable.PanelDarkBorder))
            {
                e.Graphics.DrawPath(p, dark);
            }

            DrawPanelOverflowImage(e);

            dark.Dispose();
            light.Dispose();
        }

        /// <summary>
        ///     Draws an overflown panel in pressed state
        /// </summary>
        /// <param name="e"></param>
        public void DrawPanelOverflowPressed(RibbonPanelRenderEventArgs e)
        {
            var darkBorder = Rectangle.FromLTRB(
                e.Panel.Bounds.Left,
                e.Panel.Bounds.Top,
                e.Panel.Bounds.Right,
                e.Panel.Bounds.Bottom);

            var lightBorder = Rectangle.FromLTRB(
                e.Panel.Bounds.Left + 1,
                e.Panel.Bounds.Top + 1,
                e.Panel.Bounds.Right - 1,
                e.Panel.Bounds.Bottom - 1);

            var glossy = Rectangle.FromLTRB(
                e.Panel.Bounds.Left,
                e.Panel.Bounds.Top,
                e.Panel.Bounds.Right,
                e.Panel.Bounds.Top + 17);


            var dark = RoundRectangle(darkBorder, 3);
            var light = RoundRectangle(lightBorder, 3);


            using (var b = new LinearGradientBrush(lightBorder,
                Theme.ColorTable.PanelOverflowBackgroundPressed,
                Theme.ColorTable.PanelOverflowBackgroundSelectedSouth, 90))
            {
                b.WrapMode = WrapMode.TileFlipXY;
                e.Graphics.FillPath(b, dark);
            }

            using (var path = RoundRectangle(glossy, 3, Corners.NorthEast | Corners.NorthWest))
            {
                using (var b = new LinearGradientBrush(
                    glossy,
                    Color.FromArgb(150, Color.White),
                    Color.FromArgb(50, Color.White), 90
                    ))
                {
                    b.WrapMode = WrapMode.TileFlipXY;
                    e.Graphics.FillPath(b, path);
                }
            }

            using (var p = new Pen(Color.FromArgb(40, Color.White)))
            {
                e.Graphics.DrawPath(p, light);
            }

            using (var p = new Pen(Theme.ColorTable.PanelDarkBorder))
            {
                e.Graphics.DrawPath(p, dark);
            }

            DrawPanelOverflowImage(e);

            DrawPressedShadow(e.Graphics, glossy);

            dark.Dispose();
            light.Dispose();
        }

        /// <summary>
        ///     Draws a panel in selected state
        /// </summary>
        /// <param name="e"></param>
        public void DrawPanelSelected(RibbonPanelRenderEventArgs e)
        {
            var darkBorder = Rectangle.FromLTRB(
                e.Panel.Bounds.Left,
                e.Panel.Bounds.Top,
                e.Panel.Bounds.Right,
                e.Panel.Bounds.Bottom);

            var lightBorder = Rectangle.FromLTRB(
                e.Panel.Bounds.Left + 1,
                e.Panel.Bounds.Top + 1,
                e.Panel.Bounds.Right - 1,
                e.Panel.Bounds.Bottom - 1);

            var textArea =
                Rectangle.FromLTRB(
                    e.Panel.Bounds.Left + 1,
                    e.Panel.ContentBounds.Bottom,
                    e.Panel.Bounds.Right - 1,
                    e.Panel.Bounds.Bottom - 1);

            var dark = RoundRectangle(darkBorder, 3);
            var light = RoundRectangle(lightBorder, 3);
            var txt = RoundRectangle(textArea, 3, Corners.SouthEast | Corners.SouthWest);

            using (var p = new Pen(Theme.ColorTable.PanelLightBorder))
            {
                e.Graphics.DrawPath(p, light);
            }

            using (var p = new Pen(Theme.ColorTable.PanelDarkBorder))
            {
                e.Graphics.DrawPath(p, dark);
            }

            using (var b = new SolidBrush(Theme.ColorTable.PanelBackgroundSelected))
            {
                e.Graphics.FillPath(b, light);
            }

            using (var b = new SolidBrush(Theme.ColorTable.PanelTextBackgroundSelected))
            {
                e.Graphics.FillPath(b, txt);
            }

            if (e.Panel.ButtonMoreVisible)
            {
                if (e.Panel.ButtonMorePressed)
                {
                    DrawButtonPressed(e.Graphics, e.Panel.ButtonMoreBounds, Corners.SouthEast, e.Ribbon);
                }
                else if (e.Panel.ButtonMoreSelected)
                {
                    DrawButtonSelected(e.Graphics, e.Panel.ButtonMoreBounds, Corners.SouthEast, e.Ribbon);
                }

                DrawButtonMoreGlyph(e.Graphics, e.Panel.ButtonMoreBounds, e.Panel.ButtonMoreEnabled && e.Panel.Enabled);
            }

            txt.Dispose();
            dark.Dispose();
            light.Dispose();
        }

        /// <summary>
        ///     Draws an overflown panel in selected state
        /// </summary>
        /// <param name="e"></param>
        public void DrawPannelOveflowSelected(RibbonPanelRenderEventArgs e)
        {
            var darkBorder = Rectangle.FromLTRB(
                e.Panel.Bounds.Left,
                e.Panel.Bounds.Top,
                e.Panel.Bounds.Right,
                e.Panel.Bounds.Bottom);

            var lightBorder = Rectangle.FromLTRB(
                e.Panel.Bounds.Left + 1,
                e.Panel.Bounds.Top + 1,
                e.Panel.Bounds.Right - 1,
                e.Panel.Bounds.Bottom - 1);


            var dark = RoundRectangle(darkBorder, 3);
            var light = RoundRectangle(lightBorder, 3);

            using (var p = new Pen(Theme.ColorTable.PanelLightBorder))
            {
                e.Graphics.DrawPath(p, light);
            }

            using (var p = new Pen(Theme.ColorTable.PanelDarkBorder))
            {
                e.Graphics.DrawPath(p, dark);
            }

            using (var b = new LinearGradientBrush(
                lightBorder, Theme.ColorTable.PanelOverflowBackgroundSelectedNorth, Color.Transparent, 90))
            {
                e.Graphics.FillPath(b, light);
            }

            DrawPanelOverflowImage(e);

            dark.Dispose();
            light.Dispose();
        }

        #endregion

        #region Button

        /// <summary>
        ///     Gets the corners to round on the specified button
        /// </summary>
        /// <param name="r"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        private Corners ButtonCorners(RibbonButton button)
        {
            if (!(button.OwnerItem is RibbonItemGroup))
            {
                return Corners.All;
            }
            var g = button.OwnerItem as RibbonItemGroup;
            var c = Corners.None;
            if (button == g.FirstItem)
            {
                c |= Corners.West;
            }

            if (button == g.LastItem)
            {
                c |= Corners.East;
            }

            return c;
        }

        /// <summary>
        ///     Determines button's dropDown corners
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        private Corners ButtonDdRounding(RibbonButton button)
        {
            if (!(button.OwnerItem is RibbonItemGroup))
            {
                if (button.SizeMode == RibbonElementSizeMode.Large)
                {
                    return Corners.South;
                }
                return Corners.East;
            }
            var c = Corners.None;
            var g = button.OwnerItem as RibbonItemGroup;
            if (button == g.LastItem)
            {
                c |= Corners.East;
            }

            return c;
        }

        /// <summary>
        ///     Determines buttonface corners
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        private Corners ButtonFaceRounding(RibbonButton button)
        {
            if (!(button.OwnerItem is RibbonItemGroup))
            {
                if (button.SizeMode == RibbonElementSizeMode.Large)
                {
                    return Corners.North;
                }
                return Corners.West;
            }
            var c = Corners.None;
            var g = button.OwnerItem as RibbonItemGroup;
            if (button == g.FirstItem)
            {
                c |= Corners.West;
            }

            return c;
        }

        /// <summary>
        ///     Draws a regular button in normal state
        /// </summary>
        /// <param name="e"></param>
        /// <param name="button"></param>
        public void DrawButton(Graphics g, Rectangle bounds, Corners corners)
        {
            if (bounds.Height <= 0 || bounds.Width <= 0) return;

            var outerR = Rectangle.FromLTRB(
                bounds.Left,
                bounds.Top,
                bounds.Right - 1,
                bounds.Bottom - 1);

            var innerR = Rectangle.FromLTRB(
                bounds.Left + 1,
                bounds.Top + 1,
                bounds.Right - 2,
                bounds.Bottom - 2);

            var glossyR = Rectangle.FromLTRB(
                bounds.Left + 1,
                bounds.Top + 1,
                bounds.Right - 2,
                bounds.Top + Convert.ToInt32(bounds.Height*.36));

            using (var boundsPath = RoundRectangle(outerR, 3, corners))
            {
                using (var brus = new SolidBrush(Theme.ColorTable.ButtonBgOut))
                {
                    g.FillPath(brus, boundsPath);
                }

                #region Main Bg

                using (var path = new GraphicsPath())
                {
                    path.AddEllipse(new Rectangle(bounds.Left, bounds.Top, bounds.Width, bounds.Height*2));
                    path.CloseFigure();
                    using (var gradient = new PathGradientBrush(path))
                    {
                        gradient.WrapMode = WrapMode.Clamp;
                        gradient.CenterPoint = new PointF(
                            Convert.ToSingle(bounds.Left + bounds.Width/2),
                            Convert.ToSingle(bounds.Bottom));
                        gradient.CenterColor = Theme.ColorTable.ButtonBgCenter;
                        gradient.SurroundColors = new[] {Theme.ColorTable.ButtonBgOut};

                        var blend = new Blend(3);
                        blend.Factors = new[] {0f, 0.8f, 0f};
                        blend.Positions = new[] {0f, 0.30f, 1f};


                        var lastClip = g.Clip;
                        var newClip = new Region(boundsPath);
                        newClip.Intersect(lastClip);
                        g.SetClip(newClip.GetBounds(g));
                        g.FillPath(gradient, path);
                        g.Clip = lastClip;
                    }
                }

                #endregion

                //Border
                using (var p = new Pen(Theme.ColorTable.ButtonBorderOut))
                {
                    g.DrawPath(p, boundsPath);
                }

                //Inner border
                using (var path = RoundRectangle(innerR, 3, corners))
                {
                    using (var p = new Pen(Theme.ColorTable.ButtonBorderIn))
                    {
                        g.DrawPath(p, path);
                    }
                }

                //Glossy effect
                using (var path = RoundRectangle(glossyR, 3, (corners & Corners.NorthWest) | (corners & Corners.NorthEast)))
                {
                    if (glossyR.Width > 0 && glossyR.Height > 0)
                        using (var b = new LinearGradientBrush(
                            glossyR, Theme.ColorTable.ButtonGlossyNorth, Theme.ColorTable.ButtonGlossySouth, 90))
                        {
                            b.WrapMode = WrapMode.TileFlipXY;
                            g.FillPath(b, path);
                        }
                }
            }
        }

        /// <summary>
        ///     Draws the button as Checked
        /// </summary>
        /// <param name="g"></param>
        /// <param name="button"></param>
        public void DrawButtonChecked(Graphics g, RibbonButton button)
        {
            DrawButtonChecked(g, button.Bounds, ButtonCorners(button));
        }

        /// <summary>
        ///     Draws the button as checked
        /// </summary>
        /// <param name="g"></param>
        /// <param name="button"></param>
        public void DrawButtonChecked(Graphics g, Rectangle bounds, Corners corners)
        {
            if (bounds.Height <= 0 || bounds.Width <= 0) return;

            var outerR = Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right - 1, bounds.Bottom - 1);
            var innerR = Rectangle.FromLTRB(bounds.Left + 1, bounds.Top + 1, bounds.Right - 2, bounds.Bottom - 2);
            var glossyR = Rectangle.FromLTRB(bounds.Left + 1, bounds.Top + 1, bounds.Right - 2, bounds.Top + Convert.ToInt32(bounds.Height*.36));

            using (var boundsPath = RoundRectangle(outerR, 3, corners))
            {
                using (var brus = new SolidBrush(Theme.ColorTable.ButtonCheckedBgOut))
                {
                    g.FillPath(brus, boundsPath);
                }

                #region Main Bg

                using (var path = new GraphicsPath())
                {
                    path.AddEllipse(new Rectangle(bounds.Left, bounds.Top, bounds.Width, bounds.Height*2));
                    path.CloseFigure();
                    using (var gradient = new PathGradientBrush(path))
                    {
                        gradient.WrapMode = WrapMode.Clamp;
                        gradient.CenterPoint = new PointF(
                            Convert.ToSingle(bounds.Left + bounds.Width/2),
                            Convert.ToSingle(bounds.Bottom));
                        gradient.CenterColor = Theme.ColorTable.ButtonCheckedBgCenter;
                        gradient.SurroundColors = new[] {Theme.ColorTable.ButtonCheckedBgOut};

                        var blend = new Blend(3);
                        blend.Factors = new[] {0f, 0.8f, 0f};
                        blend.Positions = new[] {0f, 0.30f, 1f};


                        var lastClip = g.Clip;
                        var newClip = new Region(boundsPath);
                        newClip.Intersect(lastClip);
                        g.SetClip(newClip.GetBounds(g));
                        g.FillPath(gradient, path);
                        g.Clip = lastClip;
                    }
                }

                #endregion

                //Border
                using (var p = new Pen(Theme.ColorTable.ButtonCheckedBorderOut))
                {
                    g.DrawPath(p, boundsPath);
                }

                //Inner border
                using (var path = RoundRectangle(innerR, 3, corners))
                {
                    using (var p = new Pen(Theme.ColorTable.ButtonCheckedBorderIn))
                    {
                        g.DrawPath(p, path);
                    }
                }

                //Glossy effect
                using (var path = RoundRectangle(glossyR, 3, (corners & Corners.NorthWest) | (corners & Corners.NorthEast)))
                {
                    using (var b = new LinearGradientBrush(
                        glossyR, Theme.ColorTable.ButtonCheckedGlossyNorth, Theme.ColorTable.ButtonCheckedGlossySouth, 90))
                    {
                        b.WrapMode = WrapMode.TileFlipXY;
                        g.FillPath(b, path);
                    }
                }
            }

            DrawPressedShadow(g, outerR);
        }

        /// <summary>
        ///     Draws a regular button in disabled state
        /// </summary>
        /// <param name="e"></param>
        /// <param name="button"></param>
        public void DrawButtonDisabled(Graphics g, Rectangle bounds, Corners corners)
        {
            if (bounds.Height <= 0 || bounds.Width <= 0) return;

            var outerR = Rectangle.FromLTRB(
                bounds.Left,
                bounds.Top,
                bounds.Right - 1,
                bounds.Bottom - 1);

            var innerR = Rectangle.FromLTRB(
                bounds.Left + 1,
                bounds.Top + 1,
                bounds.Right - 2,
                bounds.Bottom - 2);

            var glossyR = Rectangle.FromLTRB(
                bounds.Left + 1,
                bounds.Top + 1,
                bounds.Right - 2,
                bounds.Top + Convert.ToInt32(bounds.Height*.36));

            using (var boundsPath = RoundRectangle(outerR, 3, corners))
            {
                using (var brus = new SolidBrush(Theme.ColorTable.ButtonDisabledBgOut))
                {
                    g.FillPath(brus, boundsPath);
                }

                #region Main Bg

                using (var path = new GraphicsPath())
                {
                    path.AddEllipse(new Rectangle(bounds.Left, bounds.Top, bounds.Width, bounds.Height*2));
                    path.CloseFigure();
                    using (var gradient = new PathGradientBrush(path))
                    {
                        gradient.WrapMode = WrapMode.Clamp;
                        gradient.CenterPoint = new PointF(
                            Convert.ToSingle(bounds.Left + bounds.Width/2),
                            Convert.ToSingle(bounds.Bottom));
                        gradient.CenterColor = Theme.ColorTable.ButtonDisabledBgCenter;
                        gradient.SurroundColors = new[] {Theme.ColorTable.ButtonDisabledBgOut};

                        var blend = new Blend(3);
                        blend.Factors = new[] {0f, 0.8f, 0f};
                        blend.Positions = new[] {0f, 0.30f, 1f};


                        var lastClip = g.Clip;
                        var newClip = new Region(boundsPath);
                        newClip.Intersect(lastClip);
                        g.SetClip(newClip.GetBounds(g));
                        g.FillPath(gradient, path);
                        g.Clip = lastClip;
                    }
                }

                #endregion

                //Border
                using (var p = new Pen(Theme.ColorTable.ButtonDisabledBorderOut))
                {
                    g.DrawPath(p, boundsPath);
                }

                //Inner border
                using (var path = RoundRectangle(innerR, 3, corners))
                {
                    using (var p = new Pen(Theme.ColorTable.ButtonDisabledBorderIn))
                    {
                        g.DrawPath(p, path);
                    }
                }

                //Glossy effect
                using (var path = RoundRectangle(glossyR, 3, (corners & Corners.NorthWest) | (corners & Corners.NorthEast)))
                {
                    using (var b = new LinearGradientBrush(
                        glossyR, Theme.ColorTable.ButtonDisabledGlossyNorth, Theme.ColorTable.ButtonDisabledGlossySouth, 90))
                    {
                        b.WrapMode = WrapMode.TileFlipXY;
                        g.FillPath(b, path);
                    }
                }
            }
        }

        /// <summary>
        ///     Draws the arrow of buttons
        /// </summary>
        /// <param name="g"></param>
        /// <param name="button"></param>
        public void DrawButtonDropDownArrow(Graphics g, RibbonButton button, Rectangle textLayout)
        {
            var bounds = Rectangle.Empty;

            if (button.SizeMode == RibbonElementSizeMode.Large || button.SizeMode == RibbonElementSizeMode.Overflow)
            {
                bounds = LargeButtonDropDownArrowBounds(g, button.Owner.Font, button.Text, textLayout);
            }
            else
            {
                //bounds = new Rectangle(
                //    button.ButtonFaceBounds.Right + (button.DropDownBounds.Width - arrowSize.Width) / 2,
                //    button.Bounds.Top + (button.Bounds.Height - arrowSize.Height) / 2,
                //    arrowSize.Width, arrowSize.Height);
                bounds = textLayout;
            }

            DrawArrowShaded(g, bounds, button.DropDownArrowDirection, button.Enabled);
        }

        /// <summary>
        ///     Draws a regular button in pressed state
        /// </summary>
        /// <param name="e"></param>
        /// <param name="button"></param>
        public void DrawButtonPressed(Graphics g, Rectangle bounds, Corners corners, Ribbon ribbon)
        {
            //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
            if (ribbon.OrbStyle == RibbonOrbStyle.Office_2007 | ribbon.OrbStyle == RibbonOrbStyle.Office_2010)
            {
                var outerR = Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right - 1, bounds.Bottom - 1);

                using (var boundsPath = RoundRectangle(outerR, 3, corners))
                {
                    var innerR = Rectangle.FromLTRB(bounds.Left + 1, bounds.Top + 1, bounds.Right - 2, bounds.Bottom - 2);
                    var glossyR = Rectangle.FromLTRB(bounds.Left + 1, bounds.Top + 1, bounds.Right - 2, bounds.Top + Convert.ToInt32(bounds.Height*.36));

                    using (var brus = new SolidBrush(Theme.ColorTable.ButtonPressedBgOut))
                    {
                        g.FillPath(brus, boundsPath);
                    }

                    //Border
                    using (var p = new Pen(Theme.ColorTable.ButtonPressedBorderOut))
                    {
                        g.DrawPath(p, boundsPath);
                    }

                    //Inner border
                    using (var path = RoundRectangle(innerR, 3, corners))
                    {
                        using (var p = new Pen(Theme.ColorTable.ButtonPressedBorderIn))
                        {
                            g.DrawPath(p, path);
                        }
                    }

                    #region Main Bg

                    using (var path = new GraphicsPath())
                    {
                        path.AddEllipse(new Rectangle(bounds.Left, bounds.Top, bounds.Width, bounds.Height*2));
                        path.CloseFigure();
                        using (var gradient = new PathGradientBrush(path))
                        {
                            gradient.WrapMode = WrapMode.Clamp;
                            gradient.CenterPoint = new PointF(
                                Convert.ToSingle(bounds.Left + bounds.Width/2),
                                Convert.ToSingle(bounds.Bottom));
                            gradient.CenterColor = Theme.ColorTable.ButtonPressedBgCenter;
                            gradient.SurroundColors = new[] {Theme.ColorTable.ButtonPressedBgOut};

                            var blend = new Blend(3);
                            blend.Factors = new[] {0f, 0.8f, 0f};
                            blend.Positions = new[] {0f, 0.30f, 1f};


                            var lastClip = g.Clip;
                            var newClip = new Region(boundsPath);
                            newClip.Intersect(lastClip);
                            g.SetClip(newClip.GetBounds(g));
                            g.FillPath(gradient, path);
                            g.Clip = lastClip;
                        }
                    }

                    #endregion

                    //Glossy effect
                    using (var path = RoundRectangle(glossyR, 3, (corners & Corners.NorthWest) | (corners & Corners.NorthEast)))
                    {
                        using (var b = new LinearGradientBrush(glossyR, Theme.ColorTable.ButtonPressedGlossyNorth, Theme.ColorTable.ButtonPressedGlossySouth, 90))
                        {
                            b.WrapMode = WrapMode.TileFlipXY;
                            g.FillPath(b, path);
                        }
                    }

                    DrawPressedShadow(g, outerR);
                }
            }
            else if (ribbon.OrbStyle == RibbonOrbStyle.Office_2013)
            {
                //Flat Effect
                using (var path = FlatRectangle(bounds))
                {
                    using (var b = new SolidBrush(Theme.ColorTable.ButtonPressed_2013))
                    {
                        g.FillPath(b, path);
                    }
                }
            }
        }

        /// <summary>
        ///     Draws the button as pressed
        /// </summary>
        /// <param name="g"></param>
        /// <param name="button"></param>
        public void DrawButtonPressed(Graphics g, RibbonButton button, Ribbon ribbon)
        {
            DrawButtonPressed(g, button.Bounds, ButtonCorners(button), ribbon);
        }

        /// <summary>
        ///     Draws a regular buttton in selected state
        /// </summary>
        /// <param name="e"></param>
        /// <param name="button"></param>
        public void DrawButtonSelected(Graphics g, Rectangle bounds, Corners corners, Ribbon ribbon)
        {
            if (bounds.Height <= 0 || bounds.Width <= 0) return;

            //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
            if (ribbon.OrbStyle == RibbonOrbStyle.Office_2007 | ribbon.OrbStyle == RibbonOrbStyle.Office_2010)
            {
                var outerR = Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right - 1, bounds.Bottom - 1);

                using (var boundsPath = RoundRectangle(outerR, 3, corners))
                {
                    var innerR = Rectangle.FromLTRB(bounds.Left + 1, bounds.Top + 1, bounds.Right - 2, bounds.Bottom - 2);
                    var glossyR = Rectangle.FromLTRB(bounds.Left + 1, bounds.Top + 1, bounds.Right - 2, bounds.Top + Convert.ToInt32(bounds.Height*.36));

                    using (var brus = new SolidBrush(Theme.ColorTable.ButtonSelectedBgOut))
                    {
                        g.FillPath(brus, boundsPath);
                    }

                    //Border
                    using (var p = new Pen(Theme.ColorTable.ButtonSelectedBorderOut))
                    {
                        g.DrawPath(p, boundsPath);
                    }

                    //Inner border
                    using (var path = RoundRectangle(innerR, 3, corners))
                    {
                        using (var p = new Pen(Theme.ColorTable.ButtonSelectedBorderIn))
                        {
                            g.DrawPath(p, path);
                        }
                    }

                    #region Main Bg

                    using (var path = new GraphicsPath())
                    {
                        path.AddEllipse(new Rectangle(bounds.Left, bounds.Top, bounds.Width, bounds.Height*2));
                        path.CloseFigure();
                        using (var gradient = new PathGradientBrush(path))
                        {
                            gradient.WrapMode = WrapMode.Clamp;
                            gradient.CenterPoint = new PointF(
                                Convert.ToSingle(bounds.Left + bounds.Width/2),
                                Convert.ToSingle(bounds.Bottom));
                            gradient.CenterColor = Theme.ColorTable.ButtonSelectedBgCenter;
                            gradient.SurroundColors = new[] {Theme.ColorTable.ButtonSelectedBgOut};

                            var blend = new Blend(3);
                            blend.Factors = new[] {0f, 0.8f, 0f};
                            blend.Positions = new[] {0f, 0.30f, 1f};


                            var lastClip = g.Clip;
                            var newClip = new Region(boundsPath);
                            newClip.Intersect(lastClip);
                            g.SetClip(newClip.GetBounds(g));
                            g.FillPath(gradient, path);
                            g.Clip = lastClip;
                        }
                    }

                    #endregion

                    //Glossy effect
                    using (var path = RoundRectangle(glossyR, 3, (corners & Corners.NorthWest) | (corners & Corners.NorthEast)))
                    {
                        using (var b = new LinearGradientBrush(glossyR, Theme.ColorTable.ButtonSelectedGlossyNorth, Theme.ColorTable.ButtonSelectedGlossySouth, 90))
                        {
                            b.WrapMode = WrapMode.TileFlipXY;
                            g.FillPath(b, path);
                        }
                    }
                }
            }
            else if (ribbon.OrbStyle == RibbonOrbStyle.Office_2013)
            {
                //Flat Effect
                using (var path = FlatRectangle(bounds))
                {
                    using (var b = new SolidBrush(Theme.ColorTable.ButtonSelected_2013))
                    {
                        g.FillPath(b, path);
                    }
                }
            }
        }

        /// <summary>
        ///     Draws the button as a selected button
        /// </summary>
        /// <param name="g"></param>
        /// <param name="button"></param>
        public void DrawButtonSelected(Graphics g, RibbonButton button, Ribbon ribbon)
        {
            DrawButtonSelected(g, button.Bounds, ButtonCorners(button), ribbon);
        }

        /// <summary>
        ///     Draws the orb's option buttons background
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        public void DrawOrbOptionButton(Graphics g, Rectangle bounds)
        {
            bounds.Width -= 1;
            bounds.Height -= 1;

            using (var p = RoundRectangle(bounds, 3))
            {
                using (var b = new SolidBrush(Theme.ColorTable.OrbOptionBackground))
                {
                    g.FillPath(b, p);
                }

                GradientRect(g, Rectangle.FromLTRB(bounds.Left, bounds.Top + bounds.Height/2, bounds.Right, bounds.Bottom - 2),
                    Theme.ColorTable.OrbOptionShine, Theme.ColorTable.OrbOptionBackground);

                using (var pen = new Pen(Theme.ColorTable.OrbOptionBorder))
                {
                    g.DrawPath(pen, p);
                }
            }
        }

        /// <summary>
        ///     Draws a SplitDropDown button in normal state
        /// </summary>
        /// <param name="e"></param>
        /// <param name="button"></param>
        public void DrawSplitButton(RibbonItemRenderEventArgs e, RibbonButton button)
        {
        }

        /// <summary>
        ///     Draws a SplitDropDown button with the dropdown area pressed
        /// </summary>
        /// <param name="e"></param>
        /// <param name="button"></param>
        public void DrawSplitButtonDropDownPressed(RibbonItemRenderEventArgs e, RibbonButton button)
        {
        }

        /// <summary>
        ///     Draws a SplitDropDown button with the dropdown area selected
        /// </summary>
        /// <param name="e"></param>
        /// <param name="button"></param>
        public void DrawSplitButtonDropDownSelected(RibbonItemRenderEventArgs e, RibbonButton button)
        {
            var outerR = Rectangle.FromLTRB(
                button.DropDownBounds.Left,
                button.DropDownBounds.Top,
                button.DropDownBounds.Right - 1,
                button.DropDownBounds.Bottom - 1);

            var innerR = Rectangle.FromLTRB(
                outerR.Left + 1,
                outerR.Top + (button.SizeMode == RibbonElementSizeMode.Large ? 1 : 0),
                outerR.Right - 1,
                outerR.Bottom - 1);

            var faceOuterR = Rectangle.FromLTRB(
                button.ButtonFaceBounds.Left,
                button.ButtonFaceBounds.Top,
                button.ButtonFaceBounds.Right - 1,
                button.ButtonFaceBounds.Bottom - 1);

            var faceInnerR = Rectangle.FromLTRB(
                faceOuterR.Left + 1,
                faceOuterR.Top + 1,
                faceOuterR.Right + (button.SizeMode == RibbonElementSizeMode.Large ? -1 : 0),
                faceOuterR.Bottom + (button.SizeMode == RibbonElementSizeMode.Large ? 0 : -1));

            var faceCorners = ButtonFaceRounding(button);
            var ddCorners = ButtonDdRounding(button);

            var outer = RoundRectangle(outerR, 3, ddCorners);
            var inner = RoundRectangle(innerR, 2, ddCorners);
            var faceOuter = RoundRectangle(faceOuterR, 3, faceCorners);
            var faceInner = RoundRectangle(faceInnerR, 2, faceCorners);

            using (var b = new SolidBrush(Color.FromArgb(150, Color.White)))
            {
                e.Graphics.FillPath(b, faceInner);
            }

            using (var p = new Pen(button.Pressed && button.SizeMode != RibbonElementSizeMode.DropDown ? Theme.ColorTable.ButtonPressedBorderIn : Theme.ColorTable.ButtonSelectedBorderIn))
            {
                e.Graphics.DrawPath(p, faceInner);
            }

            using (var p = new Pen(button.Pressed && button.SizeMode != RibbonElementSizeMode.DropDown ? Theme.ColorTable.ButtonPressedBorderOut : Theme.ColorTable.ButtonSelectedBorderOut))
            {
                e.Graphics.DrawPath(p, faceOuter);
            }

            outer.Dispose();
            inner.Dispose();
            faceOuter.Dispose();
            faceInner.Dispose();
        }

        /// <summary>
        ///     Draws a SplitDropDown button in pressed state
        /// </summary>
        /// <param name="e"></param>
        /// <param name="button"></param>
        public void DrawSplitButtonPressed(RibbonItemRenderEventArgs e, RibbonButton button)
        {
        }

        /// <summary>
        ///     Draws a SplitDropDown button in selected state
        /// </summary>
        /// <param name="e"></param>
        /// <param name="button"></param>
        public void DrawSplitButtonSelected(RibbonItemRenderEventArgs e, RibbonButton button)
        {
            var outerR = Rectangle.FromLTRB(
                button.DropDownBounds.Left,
                button.DropDownBounds.Top,
                button.DropDownBounds.Right - 1,
                button.DropDownBounds.Bottom - 1);

            var innerR = Rectangle.FromLTRB(
                outerR.Left + 1,
                outerR.Top + 1,
                outerR.Right - 1,
                outerR.Bottom - 1);

            var faceOuterR = Rectangle.FromLTRB(
                button.ButtonFaceBounds.Left,
                button.ButtonFaceBounds.Top,
                button.ButtonFaceBounds.Right - 1,
                button.ButtonFaceBounds.Bottom - 1);

            var faceInnerR = Rectangle.FromLTRB(
                faceOuterR.Left + 1,
                faceOuterR.Top + 1,
                faceOuterR.Right + (button.SizeMode == RibbonElementSizeMode.Large ? -1 : 0),
                faceOuterR.Bottom + (button.SizeMode == RibbonElementSizeMode.Large ? 0 : -1));

            var faceCorners = ButtonFaceRounding(button);
            var ddCorners = ButtonDdRounding(button);

            var outer = RoundRectangle(outerR, 3, ddCorners);
            var inner = RoundRectangle(innerR, 2, ddCorners);
            var faceOuter = RoundRectangle(faceOuterR, 3, faceCorners);
            var faceInner = RoundRectangle(faceInnerR, 2, faceCorners);

            using (var b = new SolidBrush(Color.FromArgb(150, Color.White)))
            {
                e.Graphics.FillPath(b, inner);
            }


            using (var p = new Pen(button.Pressed && button.SizeMode != RibbonElementSizeMode.DropDown ? Theme.ColorTable.ButtonPressedBorderOut : Theme.ColorTable.ButtonSelectedBorderOut))
            {
                e.Graphics.DrawPath(p, outer);
            }

            using (var p = new Pen(button.Pressed && button.SizeMode != RibbonElementSizeMode.DropDown ? Theme.ColorTable.ButtonPressedBorderIn : Theme.ColorTable.ButtonSelectedBorderIn))
            {
                e.Graphics.DrawPath(p, faceInner);
            }


            outer.Dispose();
            inner.Dispose();
            faceOuter.Dispose();
            faceInner.Dispose();
        }

        public Rectangle LargeButtonDropDownArrowBounds(Graphics g, Font font, string text, Rectangle textLayout)
        {
            //Kevin- This function will throw an error if the text is null or empty.

            var bounds = Rectangle.Empty;

            var moreWords = text.Contains(" ");
            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = moreWords ? StringAlignment.Center : StringAlignment.Near;
            sf.Trimming = StringTrimming.EllipsisCharacter;

            sf.SetMeasurableCharacterRanges(new[] {new CharacterRange(0, text.Length)});
            var regions = g.MeasureCharacterRanges(text, font, textLayout, sf);

            var lastCharBounds = Rectangle.Round(regions[regions.Length - 1].GetBounds(g));
            if (moreWords)
            {
                return new Rectangle(lastCharBounds.Right + 3,
                    lastCharBounds.Top + (lastCharBounds.Height - arrowSize.Height)/2, arrowSize.Width, arrowSize.Height);
            }
            return new Rectangle(
                textLayout.Left + (textLayout.Width - arrowSize.Width)/2,
                lastCharBounds.Bottom + ((textLayout.Bottom - lastCharBounds.Bottom) - arrowSize.Height)/2, arrowSize.Width, arrowSize.Height);
        }

        #endregion

        #region Group

        /// <summary>
        ///     Draws the background of the specified  RibbonItemGroup
        /// </summary>
        /// <param name="e"></param>
        /// <param name="?"></param>
        public void DrawItemGroup(RibbonItemRenderEventArgs e, RibbonItemGroup grp)
        {
            var outerR = Rectangle.FromLTRB(
                grp.Bounds.Left,
                grp.Bounds.Top,
                grp.Bounds.Right - 1,
                grp.Bounds.Bottom - 1);

            var innerR = Rectangle.FromLTRB(
                outerR.Left + 1,
                outerR.Top + 1,
                outerR.Right - 1,
                outerR.Bottom - 1);

            var glossyR = Rectangle.FromLTRB(
                outerR.Left + 1,
                outerR.Top + outerR.Height/2 + 1,
                outerR.Right - 1,
                outerR.Bottom - 1);

            var outer = RoundRectangle(outerR, 2);
            var inner = RoundRectangle(innerR, 2);
            var glossy = RoundRectangle(glossyR, 2);

            using (var b = new LinearGradientBrush(
                innerR, Theme.ColorTable.ItemGroupBgNorth, Theme.ColorTable.ItemGroupBgSouth, 90))
            {
                e.Graphics.FillPath(b, inner);
            }

            using (var b = new LinearGradientBrush(
                glossyR, Theme.ColorTable.ItemGroupBgGlossy, Color.Transparent, 90))
            {
                e.Graphics.FillPath(b, glossy);
            }

            outer.Dispose();
            inner.Dispose();
        }

        /// <summary>
        ///     Draws the background of the specified  RibbonItemGroup
        /// </summary>
        /// <param name="e"></param>
        /// <param name="?"></param>
        public void DrawItemGroupBorder(RibbonItemRenderEventArgs e, RibbonItemGroup grp)
        {
            var outerR = Rectangle.FromLTRB(
                grp.Bounds.Left,
                grp.Bounds.Top,
                grp.Bounds.Right - 1,
                grp.Bounds.Bottom - 1);

            var innerR = Rectangle.FromLTRB(
                outerR.Left + 1,
                outerR.Top + 1,
                outerR.Right - 1,
                outerR.Bottom - 1);

            var outer = RoundRectangle(outerR, 2);
            var inner = RoundRectangle(innerR, 2);

            using (var dark = new Pen(Theme.ColorTable.ItemGroupSeparatorDark))
            {
                using (var light = new Pen(Theme.ColorTable.ItemGroupSeparatorLight))
                {
                    foreach (RibbonItem item in grp.Items)
                    {
                        if (item == grp.LastItem) break;

                        e.Graphics.DrawLine(dark,
                            new Point(item.Bounds.Right, item.Bounds.Top),
                            new Point(item.Bounds.Right, item.Bounds.Bottom));

                        e.Graphics.DrawLine(light,
                            new Point(item.Bounds.Right + 1, item.Bounds.Top),
                            new Point(item.Bounds.Right + 1, item.Bounds.Bottom));
                    }
                }
            }

            using (var p = new Pen(Theme.ColorTable.ItemGroupOuterBorder))
            {
                e.Graphics.DrawPath(p, outer);
            }

            using (var p = new Pen(Theme.ColorTable.ItemGroupInnerBorder))
            {
                e.Graphics.DrawPath(p, inner);
            }

            outer.Dispose();
            inner.Dispose();
        }

        #endregion

        #region ButtonList

        public void DrawButtonList(Graphics g, RibbonButtonList list, Ribbon ribbon)
        {
            var outerR = Rectangle.FromLTRB(
                list.Bounds.Left,
                list.Bounds.Top,
                list.Bounds.Right - 1,
                list.Bounds.Bottom);

            using (var path = RoundRectangle(outerR, 3, Corners.East))
            {
                var bgcolor = list.Selected ? Theme.ColorTable.ButtonListBgSelected : Theme.ColorTable.ButtonListBg;

                if (list.Canvas is RibbonDropDown) bgcolor = Theme.ColorTable.DropDownBg;

                using (var b = new SolidBrush(bgcolor))
                {
                    g.FillPath(b, path);
                }

                using (var p = new Pen(Theme.ColorTable.ButtonListBorder))
                {
                    g.DrawPath(p, path);
                }
            }

            if (list.ScrollType == RibbonButtonList.ListScrollType.Scrollbar && ScrollBarRenderer.IsSupported)
            {
                ScrollBarRenderer.DrawUpperVerticalTrack(g, list.ScrollBarBounds, ScrollBarState.Normal);

                if (list.ThumbPressed)
                {
                    ScrollBarRenderer.DrawVerticalThumb(g, list.ThumbBounds, ScrollBarState.Pressed);
                    ScrollBarRenderer.DrawVerticalThumbGrip(g, list.ThumbBounds, ScrollBarState.Pressed);
                }
                else if (list.ThumbSelected)
                {
                    ScrollBarRenderer.DrawVerticalThumb(g, list.ThumbBounds, ScrollBarState.Hot);
                    ScrollBarRenderer.DrawVerticalThumbGrip(g, list.ThumbBounds, ScrollBarState.Hot);
                }
                else
                {
                    ScrollBarRenderer.DrawVerticalThumb(g, list.ThumbBounds, ScrollBarState.Normal);
                    ScrollBarRenderer.DrawVerticalThumbGrip(g, list.ThumbBounds, ScrollBarState.Normal);
                }

                if (list.ButtonUpPressed)
                {
                    ScrollBarRenderer.DrawArrowButton(g, list.ButtonUpBounds, ScrollBarArrowButtonState.UpPressed);
                }
                else if (list.ButtonUpSelected)
                {
                    ScrollBarRenderer.DrawArrowButton(g, list.ButtonUpBounds, ScrollBarArrowButtonState.UpHot);
                }
                else
                {
                    ScrollBarRenderer.DrawArrowButton(g, list.ButtonUpBounds, ScrollBarArrowButtonState.UpNormal);
                }

                if (list.ButtonDownPressed)
                {
                    ScrollBarRenderer.DrawArrowButton(g, list.ButtonDownBounds, ScrollBarArrowButtonState.DownPressed);
                }
                else if (list.ButtonDownSelected)
                {
                    ScrollBarRenderer.DrawArrowButton(g, list.ButtonDownBounds, ScrollBarArrowButtonState.DownHot);
                }
                else
                {
                    ScrollBarRenderer.DrawArrowButton(g, list.ButtonDownBounds, ScrollBarArrowButtonState.DownNormal);
                }
            }
            else
            {
                #region Control Buttons

                if (list.ScrollType == RibbonButtonList.ListScrollType.Scrollbar)
                {
                    using (var b = new SolidBrush(Theme.ColorTable.ButtonGlossyNorth))
                    {
                        g.FillRectangle(b, list.ScrollBarBounds);
                    }
                }

                if (!list.ButtonDownEnabled)
                {
                    DrawButtonDisabled(g, list.ButtonDownBounds, list.ButtonDropDownPresent ? Corners.None : Corners.SouthEast);
                }
                else if (list.ButtonDownPressed)
                {
                    DrawButtonPressed(g, list.ButtonDownBounds, list.ButtonDropDownPresent ? Corners.None : Corners.SouthEast, ribbon);
                }
                else if (list.ButtonDownSelected)
                {
                    DrawButtonSelected(g, list.ButtonDownBounds, list.ButtonDropDownPresent ? Corners.None : Corners.SouthEast, ribbon);
                }
                else
                {
                    DrawButton(g, list.ButtonDownBounds, Corners.None);
                }

                if (!list.ButtonUpEnabled)
                {
                    DrawButtonDisabled(g, list.ButtonUpBounds, Corners.NorthEast);
                }
                else if (list.ButtonUpPressed)
                {
                    DrawButtonPressed(g, list.ButtonUpBounds, Corners.NorthEast, ribbon);
                }
                else if (list.ButtonUpSelected)
                {
                    DrawButtonSelected(g, list.ButtonUpBounds, Corners.NorthEast, ribbon);
                }
                else
                {
                    DrawButton(g, list.ButtonUpBounds, Corners.NorthEast);
                }

                if (list.ButtonDropDownPresent)
                {
                    if (list.ButtonDropDownPressed)
                    {
                        DrawButtonPressed(g, list.ButtonDropDownBounds, Corners.SouthEast, ribbon);
                    }
                    else if (list.ButtonDropDownSelected)
                    {
                        DrawButtonSelected(g, list.ButtonDropDownBounds, Corners.SouthEast, ribbon);
                    }
                    else
                    {
                        DrawButton(g, list.ButtonDropDownBounds, Corners.SouthEast);
                    }
                }

                if (list.ScrollType == RibbonButtonList.ListScrollType.Scrollbar && list.ScrollBarEnabled)
                {
                    if (list.ThumbPressed)
                    {
                        DrawButtonPressed(g, list.ThumbBounds, Corners.All, ribbon);
                    }
                    else if (list.ThumbSelected)
                    {
                        DrawButtonSelected(g, list.ThumbBounds, Corners.All, ribbon);
                    }
                    else
                    {
                        DrawButton(g, list.ThumbBounds, Corners.All);
                    }
                }

                var dk = Theme.ColorTable.Arrow;
                var lt = Theme.ColorTable.ArrowLight;
                var ds = Theme.ColorTable.ArrowDisabled;

                var arrUp = CenterOn(list.ButtonUpBounds, new Rectangle(Point.Empty, arrowSize));
                arrUp.Offset(0, 1);
                var arrD = CenterOn(list.ButtonDownBounds, new Rectangle(Point.Empty, arrowSize));
                arrD.Offset(0, 1);
                var arrdd = CenterOn(list.ButtonDropDownBounds, new Rectangle(Point.Empty, arrowSize));
                arrdd.Offset(0, 3);

                DrawArrow(g, arrUp, list.ButtonUpEnabled ? lt : Color.Transparent, RibbonArrowDirection.Up);
                arrUp.Offset(0, -1);
                DrawArrow(g, arrUp, list.ButtonUpEnabled ? dk : ds, RibbonArrowDirection.Up);

                DrawArrow(g, arrD, list.ButtonDownEnabled ? lt : Color.Transparent, RibbonArrowDirection.Down);
                arrD.Offset(0, -1);
                DrawArrow(g, arrD, list.ButtonDownEnabled ? dk : ds, RibbonArrowDirection.Down);

                if (list.ButtonDropDownPresent)
                {
                    using (var b = new SolidBrush(Theme.ColorTable.Arrow))
                    {
                        var sm = g.SmoothingMode;
                        g.SmoothingMode = SmoothingMode.None;
                        g.FillRectangle(b, new Rectangle(new Point(arrdd.Left - 1, arrdd.Top - 4), new Size(arrowSize.Width + 2, 1)));
                        g.SmoothingMode = sm;
                    }

                    DrawArrow(g, arrdd, lt, RibbonArrowDirection.Down);
                    arrdd.Offset(0, -1);
                    DrawArrow(g, arrdd, dk, RibbonArrowDirection.Down);
                }

                #endregion
            }
        }

        #endregion

        #region Separator

        public void DrawSeparator(Graphics g, RibbonSeparator separator)
        {
            if (separator.SizeMode == RibbonElementSizeMode.DropDown)
            {
                if (!string.IsNullOrEmpty(separator.Text))
                {
                    using (var b = new SolidBrush(Theme.ColorTable.SeparatorBg))
                    {
                        g.FillRectangle(b, separator.Bounds);
                    }

                    using (var p = new Pen(Theme.ColorTable.SeparatorLine))
                    {
                        g.DrawLine(p,
                            new Point(separator.Bounds.Left, separator.Bounds.Bottom),
                            new Point(separator.Bounds.Right, separator.Bounds.Bottom));
                    }
                }
                else
                {
                    using (var p = new Pen(Theme.ColorTable.DropDownImageSeparator))
                    {
                        g.DrawLine(p,
                            new Point(separator.Bounds.Left + 30, separator.Bounds.Top + 1),
                            new Point(separator.Bounds.Right, separator.Bounds.Top + 1));
                    }
                }
            }
            else
            {
                using (var p = new Pen(Theme.ColorTable.SeparatorDark))
                {
                    g.DrawLine(p,
                        new Point(separator.Bounds.Left, separator.Bounds.Top),
                        new Point(separator.Bounds.Left, separator.Bounds.Bottom));
                }

                using (var p = new Pen(Theme.ColorTable.SeparatorLight))
                {
                    g.DrawLine(p,
                        new Point(separator.Bounds.Left + 1, separator.Bounds.Top),
                        new Point(separator.Bounds.Left + 1, separator.Bounds.Bottom));
                }
            }
        }

        #endregion

        #region TextBox

        /// <summary>
        ///     Draws a disabled textbox
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        public void DrawTextBoxDisabled(Graphics g, Rectangle bounds)
        {
            using (var b = new SolidBrush(SystemColors.Control))
            {
                g.FillRectangle(b, bounds);
            }

            using (var p = new Pen(Theme.ColorTable.TextBoxBorder))
            {
                g.DrawRectangle(p, bounds);
            }
        }

        /// <summary>
        ///     Draws an unselected textbox
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        public void DrawTextBoxSelected(Graphics g, Rectangle bounds)
        {
            using (var path = RoundRectangle(bounds, 3))
            {
                using (var b = new SolidBrush(SystemColors.Window))
                {
                    //g.FillPath(b, path);
                    g.FillRectangle(b, bounds);
                }

                using (var p = new Pen(Theme.ColorTable.TextBoxBorder))
                {
                    //g.DrawPath(p, path);
                    g.DrawRectangle(p, bounds);
                }
            }
        }

        /// <summary>
        ///     Draws an unselected textbox
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        public void DrawTextBoxUnselected(Graphics g, Rectangle bounds)
        {
            using (var b = new SolidBrush(Theme.ColorTable.TextBoxUnselectedBg))
            {
                g.FillRectangle(b, bounds);
            }

            using (var p = new Pen(Theme.ColorTable.TextBoxBorder))
            {
                g.DrawRectangle(p, bounds);
            }
        }

        #endregion

        #region ComboBox

        public void DrawComboxDropDown(Graphics g, RibbonComboBox b, Ribbon ribbon)
        {
            if (b.DropDownButtonPressed)
            {
                DrawButtonPressed(g, b.DropDownButtonBounds, Corners.None, ribbon);
            }
            else if (b.DropDownButtonSelected)
            {
                DrawButtonSelected(g, b.DropDownButtonBounds, Corners.None, ribbon);
            }
            else if (b.Selected)
            {
                DrawButton(g, b.DropDownButtonBounds, Corners.None);
            }

            DrawArrowShaded(g, b.DropDownButtonBounds, RibbonArrowDirection.Down, true); //b.Enabled);
        }

        public void DrawUpDownButtons(Graphics g, RibbonUpDown b, Ribbon ribbon)
        {
            if (b.UpButtonPressed)
                DrawButtonPressed(g, b.UpButtonBounds, Corners.None, ribbon);
            else if (b.UpButtonSelected)
                DrawButtonSelected(g, b.UpButtonBounds, Corners.None, ribbon);
            else
                DrawButton(g, b.UpButtonBounds, Corners.None);

            if (b.DownButtonPressed)
                DrawButtonPressed(g, b.DownButtonBounds, Corners.None, ribbon);
            else if (b.DownButtonSelected)
                DrawButtonSelected(g, b.DownButtonBounds, Corners.None, ribbon);
            else
                DrawButton(g, b.DownButtonBounds, Corners.None);

            DrawArrowShaded(g, b.UpButtonBounds, RibbonArrowDirection.Up, true);
            DrawArrowShaded(g, b.DownButtonBounds, RibbonArrowDirection.Down, true);
        }

        #endregion

        #region Quick Access and Caption Bar

        private GraphicsPath CreateQuickAccessPath(Point a, Point b, Point c, Point d, Point e, Rectangle bounds, int offsetx, int offsety, Ribbon ribbon)
        {
            a.Offset(offsetx, offsety);
            b.Offset(offsetx, offsety);
            c.Offset(offsetx, offsety);
            d.Offset(offsetx, offsety);
            e.Offset(offsetx, offsety);

            var path = new GraphicsPath();
            if (ribbon.RightToLeft == RightToLeft.No)
            {
                path.AddLine(a, b);
                path.AddArc(new Rectangle(b.X - bounds.Height/2, b.Y, bounds.Height, bounds.Height), -90, 180);
                path.AddLine(d, c);
                if (ribbon.OrbVisible)
                {
                    path.AddCurve(new[] {c, e, a});
                }
                else
                {
                    path.AddArc(new Rectangle(a.X - bounds.Height/2, a.Y, bounds.Height, bounds.Height), 90, 180);
                }
            }
            else
            {
                ///   a-----b    a-----b
                ///  |     z    |       z
                ///   c---d      c-----d
                path.AddLine(d, c);
                path.AddArc(new Rectangle(a.X - bounds.Height/2, a.Y, bounds.Height, bounds.Height), 90, 180);
                path.AddLine(a, b);
                if (ribbon.OrbVisible)
                {
                    path.AddCurve(new[] {b, e, d});
                }
                else
                {
                    path.AddArc(new Rectangle(b.X - bounds.Height/2, b.Y, bounds.Height, bounds.Height), -90, 180);
                }
            }
            return path;
        }

        public void DrawCaptionBarBackground(Rectangle r, Graphics g)
        {
            var smbuff = g.SmoothingMode;
            var r1 = new Rectangle(r.Left, r.Top, r.Width, 4);
            var r2 = new Rectangle(r.Left, r1.Bottom, r.Width, 4);
            var r3 = new Rectangle(r.Left, r2.Bottom, r.Width, r.Height - 8);
            var r4 = new Rectangle(r.Left, r3.Bottom, r.Width, 1);

            Rectangle[] rects = {r1, r2, r3, r4};
            Color[,] colors =
            {
                {Theme.ColorTable.Caption1, Theme.ColorTable.Caption2},
                {Theme.ColorTable.Caption3, Theme.ColorTable.Caption4},
                {Theme.ColorTable.Caption5, Theme.ColorTable.Caption6},
                {Theme.ColorTable.Caption7, Theme.ColorTable.Caption7}
            };

            g.SmoothingMode = SmoothingMode.None;

            for (var i = 0; i < rects.Length; i++)
            {
                var grect = rects[i];
                grect.Height += 2;
                grect.Y -= 1;
                using (var b = new LinearGradientBrush(grect, colors[i, 0], colors[i, 1], 90))
                {
                    g.FillRectangle(b, rects[i]);
                }
            }

            g.SmoothingMode = smbuff;
        }

        private void DrawCaptionBarText(Rectangle captionBar, RibbonRenderEventArgs e)
        {
            Form f = e.Ribbon.FindForm();

            if (f == null)
                return;

            var sf = new StringFormat();
            sf.LineAlignment = sf.Alignment = StringAlignment.Center;
            sf.Trimming = StringTrimming.EllipsisCharacter;
            sf.FormatFlags |= StringFormatFlags.NoWrap;
            var ft = new Font(SystemFonts.CaptionFont, FontStyle.Regular);


            if (e.Ribbon.ActualBorderMode == RibbonWindowMode.NonClientAreaGlass)
            {
                WinApi.DrawTextOnGlass(e.Graphics, f.Text, SystemFonts.CaptionFont, captionBar, 10);
            }
            else if (e.Ribbon.ActualBorderMode == RibbonWindowMode.NonClientAreaCustomDrawn)
            {
                TextRenderer.DrawText(e.Graphics, f.Text, ft, captionBar, Theme.ColorTable.FormBorder);
            }
            //Console.WriteLine("capt " + DateTime.Now.Millisecond + e.ClipRectangle.ToString());
            //WinApi.FillForGlass(e.Graphics, captionBar);
            //WinApi.DrawTextOnGlass(e.Ribbon.Handle, f.Text, SystemFonts.CaptionFont, captionBar, 10);
        }

        #endregion

        #region Ribbon Orb

        /// <summary>
        ///     Draws the orb on the specified state
        /// </summary>
        /// <param name="g">Device to draw</param>
        /// <param name="ribbon">Ribbon that the orb belongs to</param>
        /// <param name="r">Layout rectangle for the orb</param>
        /// <param name="selected">Specifies if the orb should be drawn as selected</param>
        /// <param name="pressed">Specifies if the orb should be drawn as pressed</param>
        public void DrawOrb(Graphics g, Rectangle r, Image image, bool selected, bool pressed)
        {
            int sweep, start;
            Point p1, p2, p3;
            Color bgdark, bgmed, bglight, light;
            var rinner = r;
            rinner.Inflate(-1, -1);
            var shadow = r;
            shadow.Offset(1, 1);
            shadow.Inflate(2, 2);

            #region Color Selection

            if (pressed)
            {
                bgdark = Theme.ColorTable.OrbPressedBackgroundDark;
                bgmed = Theme.ColorTable.OrbPressedBackgroundMedium;
                bglight = Theme.ColorTable.OrbPressedBackgroundLight;
                light = Theme.ColorTable.OrbPressedLight;
            }
            else if (selected)
            {
                bgdark = Theme.ColorTable.OrbSelectedBackgroundDark;
                bgmed = Theme.ColorTable.OrbSelectedBackgroundDark;
                bglight = Theme.ColorTable.OrbSelectedBackgroundLight;
                light = Theme.ColorTable.OrbSelectedLight;
            }
            else
            {
                bgdark = Theme.ColorTable.OrbBackgroundDark;
                bgmed = Theme.ColorTable.OrbBackgroundMedium;
                bglight = Theme.ColorTable.OrbBackgroundLight;
                light = Theme.ColorTable.OrbLight;
            }

            #endregion

            #region Shadow

            using (var p = new GraphicsPath())
            {
                p.AddEllipse(shadow);

                using (var gradient = new PathGradientBrush(p))
                {
                    gradient.WrapMode = WrapMode.Clamp;
                    gradient.CenterPoint = new PointF(shadow.Left + shadow.Width/2, shadow.Top + shadow.Height/2);
                    gradient.CenterColor = Color.FromArgb(180, Color.Black);
                    gradient.SurroundColors = new[] {Color.Transparent};

                    var blend = new Blend(3);
                    blend.Factors = new[] {0f, 1f, 1f};
                    blend.Positions = new[] {0, 0.2f, 1f};
                    gradient.Blend = blend;

                    g.FillPath(gradient, p);
                }
            }

            #endregion

            #region Orb Background

            using (var p = new Pen(bgdark, 1))
            {
                g.DrawEllipse(p, r);
            }

            using (var p = new GraphicsPath())
            {
                p.AddEllipse(r);
                using (var gradient = new PathGradientBrush(p))
                {
                    gradient.WrapMode = WrapMode.Clamp;
                    gradient.CenterPoint = new PointF(Convert.ToSingle(r.Left + r.Width/2), Convert.ToSingle(r.Bottom));
                    gradient.CenterColor = bglight;
                    gradient.SurroundColors = new[] {bgmed};

                    var blend = new Blend(3);
                    blend.Factors = new[] {0f, .8f, 1f};
                    blend.Positions = new[] {0, 0.50f, 1f};
                    gradient.Blend = blend;


                    g.FillPath(gradient, p);
                }
            }

            #endregion

            #region Bottom round shine

            var bshine = new Rectangle(0, 0, r.Width/2, r.Height/2);
            bshine.X = r.X + (r.Width - bshine.Width)/2;
            bshine.Y = r.Y + r.Height/2;


            using (var p = new GraphicsPath())
            {
                p.AddEllipse(bshine);

                using (var gradient = new PathGradientBrush(p))
                {
                    gradient.WrapMode = WrapMode.Clamp;
                    gradient.CenterPoint = new PointF(Convert.ToSingle(r.Left + r.Width/2), Convert.ToSingle(r.Bottom));
                    gradient.CenterColor = Color.White;
                    gradient.SurroundColors = new[] {Color.Transparent};

                    g.FillPath(gradient, p);
                }
            }

            #endregion

            #region Upper Glossy

            using (var p = new GraphicsPath())
            {
                sweep = 160;
                start = 180 + (180 - sweep)/2;
                p.AddArc(rinner, start, sweep);

                p1 = Point.Round(p.PathData.Points[0]);
                p2 = Point.Round(p.PathData.Points[p.PathData.Points.Length - 1]);
                p3 = new Point(rinner.Left + rinner.Width/2, p2.Y - 3);
                p.AddCurve(new[] {p2, p3, p1});

                using (var gradient = new PathGradientBrush(p))
                {
                    gradient.WrapMode = WrapMode.Clamp;
                    gradient.CenterPoint = p3;
                    gradient.CenterColor = Color.Transparent;
                    gradient.SurroundColors = new[] {light};

                    var blend = new Blend(3);
                    blend.Factors = new[] {.3f, .8f, 1f};
                    blend.Positions = new[] {0, 0.50f, 1f};
                    gradient.Blend = blend;

                    g.FillPath(gradient, p);
                }

                using (var b = new LinearGradientBrush(new Point(r.Left, r.Top), new Point(r.Left, p1.Y), Color.White, Color.Transparent))
                {
                    var blend = new Blend(4);
                    blend.Factors = new[] {0f, .4f, .8f, 1f};
                    blend.Positions = new[] {0f, .3f, .4f, 1f};
                    b.Blend = blend;
                    g.FillPath(b, p);
                }
            }

            #endregion

            #region Upper Shine

            /////Lower gloss
            //using (GraphicsPath p = new GraphicsPath())
            //{
            //    sweep = 140;
            //    start = (180 - sweep) / 2;
            //    p.AddArc(rinner, start, sweep);

            //    p1 = Point.Round(p.PathData.Points[0]);
            //    p2 = Point.Round(p.PathData.Points[p.PathData.Points.Length - 1]);
            //    p3 = new Point(rinner.Left + rinner.Width / 2, p1.Y + 3);
            //    p.AddCurve(new Point[] { p2, p3, p1 });

            //    g.FillPath(Brushes.White, p);
            //}

            ///Upper shine
            using (var p = new GraphicsPath())
            {
                sweep = 160;
                start = 180 + (180 - sweep)/2;
                p.AddArc(rinner, start, sweep);

                using (var pen = new Pen(Color.White))
                {
                    g.DrawPath(pen, p);
                }
            }

            #endregion

            #region Lower Shine

            using (var p = new GraphicsPath())
            {
                sweep = 160;
                start = (180 - sweep)/2;
                p.AddArc(rinner, start, sweep);
                var pt = Point.Round(p.PathData.Points[0]);

                var rrinner = rinner;
                rrinner.Inflate(-1, -1);
                sweep = 160;
                start = (180 - sweep)/2;
                p.AddArc(rrinner, start, sweep);

                using (var b = new LinearGradientBrush(
                    new Point(rinner.Left, rinner.Bottom),
                    new Point(rinner.Left, pt.Y - 1),
                    light, Color.FromArgb(50, light)))
                {
                    g.FillPath(b, p);
                }

                //p1 = Point.Round(p.PathData.Points[0]);
                //p2 = Point.Round(p.PathData.Points[p.PathData.Points.Length - 1]);
                //p3 = new Point(rinner.Left + rinner.Width / 2, rinner.Bottom - 1);
                //p.AddCurve(new Point[] { p2, p3, p1 });

                //using (LinearGradientBrush b = new LinearGradientBrush(
                //    new Point(rinner.Left, rinner.Bottom + 1),
                //    new Point(rinner.Left, p1.Y),
                //    Color.FromArgb(200, Color.White), Color.Transparent))
                //{
                //    g.FillPath(b, p);
                //}
            }

            #endregion

            #region Orb Icon

            if (image != null)
            {
                var irect = new Rectangle(Point.Empty, image.Size);
                irect.X = r.X + (r.Width - irect.Width)/2;
                irect.Y = r.Y + (r.Height - irect.Height)/2;
                g.DrawImage(image, irect);
            }

            #endregion
        }

        /// <summary>
        ///     Draws the orb button in a normal state
        /// </summary>
        /// <param name="e"></param>
        /// <param name="button"></param>
        public void DrawOrbNormal(RibbonRenderEventArgs e)
        {
            //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
            if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2010)
            {
                using (var path = RoundRectangle(e.ClipRectangle, 2, Corners.North))
                {
                    e.Graphics.FillPath(new SolidBrush(Theme.ColorTable.OrbButtonBackground), path);

                    //Border
                    using (var pOut = new Pen(Theme.ColorTable.OrbButtonBorderDark))
                    {
                        e.Graphics.DrawPath(pOut, path);
                    }

                    //Inner border
                    var innerR = Rectangle.FromLTRB(e.ClipRectangle.Left + 1, e.ClipRectangle.Top + 1, e.ClipRectangle.Right - 1, e.ClipRectangle.Bottom);

                    using (var inpath = RoundRectangle(innerR, 2, Corners.North))
                    {
                        using (var pIn = new Pen(Theme.ColorTable.OrbButtonMedium))
                        {
                            e.Graphics.DrawPath(pIn, inpath);
                        }
                    }

                    var intCenter = e.ClipRectangle.Height/2;

                    var rec = Rectangle.FromLTRB(e.ClipRectangle.Left + 1, e.ClipRectangle.Top + intCenter, e.ClipRectangle.Right - 2, e.ClipRectangle.Bottom - 1);

                    var north = Theme.ColorTable.OrbButtonDark;
                    var south = Theme.ColorTable.OrbButtonLight;

                    using (var b = new LinearGradientBrush(new Point(0, e.ClipRectangle.Top + intCenter), new Point(0, e.ClipRectangle.Bottom), north, south))
                    {
                        b.WrapMode = WrapMode.TileFlipXY;
                        e.Graphics.FillRectangle(b, rec);
                    }
                }
            }
            else if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2013)
            {
                using (var path = FlatRectangle(e.ClipRectangle))
                {
                    e.Graphics.FillPath(new SolidBrush(Theme.ColorTable.OrbButton_2013), path);
                }
            }
        }

        /// <summary>
        ///     Draws the orb button in pressed state
        /// </summary>
        /// <param name="e"></param>
        /// <param name="button"></param>
        public void DrawOrbPressed(RibbonRenderEventArgs e)
        {
            //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
            if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2010)
            {
                using (var path = RoundRectangle(e.ClipRectangle, 2, Corners.North))
                {
                    e.Graphics.FillPath(new SolidBrush(Theme.ColorTable.ButtonPressedGlossySouth), path);

                    //Border
                    using (var p = new Pen(Theme.ColorTable.ButtonPressedBorderOut))
                    {
                        e.Graphics.DrawPath(p, path);
                    }

                    //Inner border
                    var innerR = Rectangle.FromLTRB(e.ClipRectangle.Left + 1, e.ClipRectangle.Top + 1, e.ClipRectangle.Right - 1, e.ClipRectangle.Bottom);

                    using (var inpath = RoundRectangle(innerR, 2, Corners.North))
                    {
                        using (var p = new Pen(Theme.ColorTable.ButtonPressedBorderIn))
                        {
                            e.Graphics.DrawPath(p, inpath);
                        }
                    }

                    var north = Theme.ColorTable.ButtonPressedGlossyNorth;
                    var south = Theme.ColorTable.ButtonPressedGlossySouth;

                    var intCenter = e.ClipRectangle.Height/2;
                    var rec = Rectangle.FromLTRB(e.ClipRectangle.Left + 1, e.ClipRectangle.Top + intCenter, e.ClipRectangle.Right - 2, e.ClipRectangle.Bottom - 1);

                    using (var b = new LinearGradientBrush(new Point(0, e.ClipRectangle.Top + intCenter), new Point(0, e.ClipRectangle.Bottom), north, south))
                    {
                        b.WrapMode = WrapMode.TileFlipXY;
                        e.Graphics.FillRectangle(b, rec);
                    }
                }
            }
            else if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2013)
            {
                using (var path = FlatRectangle(e.ClipRectangle))
                {
                    e.Graphics.FillPath(new SolidBrush(Theme.ColorTable.OrbButtonPressed_2013), path);
                }
            }
        }

        /// <summary>
        ///     Draws the orb button in a selected state
        /// </summary>
        /// <param name="e"></param>
        /// <param name="button"></param>
        public void DrawOrbSelected(RibbonRenderEventArgs e)
        {
            //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
            if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2010)
            {
                using (var path = RoundRectangle(e.ClipRectangle, 2, Corners.North))
                {
                    e.Graphics.FillPath(new SolidBrush(Theme.ColorTable.ButtonPressedGlossySouth), path);

                    //Border
                    using (var p = new Pen(Theme.ColorTable.ButtonPressedBorderOut))
                    {
                        e.Graphics.DrawPath(p, path);
                    }

                    //Inner border
                    var innerR = Rectangle.FromLTRB(e.ClipRectangle.Left + 1, e.ClipRectangle.Top + 1, e.ClipRectangle.Right - 1, e.ClipRectangle.Bottom);

                    using (var inpath = RoundRectangle(innerR, 2, Corners.North))
                    {
                        using (var p = new Pen(Theme.ColorTable.ButtonPressedBorderIn))
                        {
                            e.Graphics.DrawPath(p, inpath);
                        }
                    }

                    var north = Theme.ColorTable.ButtonSelectedGlossyNorth;
                    var south = Theme.ColorTable.ButtonSelectedGlossySouth;

                    var intCenter = e.ClipRectangle.Height/2;
                    var rec = Rectangle.FromLTRB(e.ClipRectangle.Left + 1, e.ClipRectangle.Top + intCenter, e.ClipRectangle.Right - 2, e.ClipRectangle.Bottom - 1);

                    using (var b = new LinearGradientBrush(new Point(0, e.ClipRectangle.Top + intCenter), new Point(0, e.ClipRectangle.Bottom), north, south))
                    {
                        b.WrapMode = WrapMode.TileFlipXY;
                        e.Graphics.FillRectangle(b, rec);
                    }
                }
            }
            else if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2013)
            {
                using (var path = FlatRectangle(e.ClipRectangle))
                {
                    e.Graphics.FillPath(new SolidBrush(Theme.ColorTable.OrbButtonSelected_2013), path);
                }
            }
        }

        #endregion

        #endregion

        #region Overrides

        public override void OnRenderDropDownBackground(RibbonCanvasEventArgs e)
        {
            var outerR = new Rectangle(0, 0, e.Bounds.Width - 1, e.Bounds.Height - 1);
            var imgsR = new Rectangle(0, 0, 26, e.Bounds.Height);
            var dd = e.Canvas as RibbonDropDown;

            using (var b = new SolidBrush(Theme.ColorTable.DropDownBg))
            {
                e.Graphics.Clear(Color.Transparent);
                var sbuff = e.Graphics.SmoothingMode;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillRectangle(b, outerR);
                e.Graphics.SmoothingMode = sbuff;
            }

            if (dd != null && dd.DrawIconsBar)
            {
                using (var b = new SolidBrush(Theme.ColorTable.DropDownImageBg))
                {
                    e.Graphics.FillRectangle(b, imgsR);
                }

                using (var p = new Pen(Theme.ColorTable.DropDownImageSeparator))
                {
                    e.Graphics.DrawLine(p,
                        new Point(imgsR.Right, imgsR.Top),
                        new Point(imgsR.Right, imgsR.Bottom));
                }
            }

            using (var p = new Pen(Theme.ColorTable.DropDownBorder))
            {
                if (dd != null)
                {
                    using (var r = RoundRectangle(new Rectangle(Point.Empty, new Size(dd.Size.Width - 1, dd.Size.Height - 1)), dd.BorderRoundness))
                    {
                        var smb = e.Graphics.SmoothingMode;
                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        e.Graphics.DrawPath(p, r);
                        e.Graphics.SmoothingMode = smb;
                    }
                }
                else
                {
                    e.Graphics.DrawRectangle(p, outerR);
                }
            }

            if (dd.ShowSizingGrip)
            {
                var gripArea = Rectangle.FromLTRB(
                    e.Bounds.Left + 1,
                    e.Bounds.Bottom - dd.SizingGripHeight,
                    e.Bounds.Right - 1,
                    e.Bounds.Bottom - 1);

                using (var b = new LinearGradientBrush(
                    gripArea, Theme.ColorTable.DropDownGripNorth, Theme.ColorTable.DropDownGripSouth, 90))
                {
                    e.Graphics.FillRectangle(b, gripArea);
                }

                using (var p = new Pen(Theme.ColorTable.DropDownGripBorder))
                {
                    e.Graphics.DrawLine(p,
                        gripArea.Location,
                        new Point(gripArea.Right - 1, gripArea.Top));
                }

                DrawGripDot(e.Graphics, new Point(gripArea.Right - 7, gripArea.Bottom - 3));
                DrawGripDot(e.Graphics, new Point(gripArea.Right - 3, gripArea.Bottom - 7));
                DrawGripDot(e.Graphics, new Point(gripArea.Right - 3, gripArea.Bottom - 3));
            }
        }

        public override void OnRenderOrbDropDownBackground(RibbonOrbDropDownEventArgs e)
        {
            var Width = e.RibbonOrbDropDown.Width;
            var Height = e.RibbonOrbDropDown.Height;

            var OrbDDContent = e.RibbonOrbDropDown.ContentBounds;
            var Bcontent = e.RibbonOrbDropDown.ContentButtonsBounds;
            var OuterRect = new Rectangle(0, 0, Width - 1, Height - 1);
            var InnerRect = new Rectangle(1, 1, Width - 3, Height - 3);
            var NorthNorthRect = new Rectangle(1, 1, Width - 3, OrbDDContent.Top/2);
            var northSouthRect = new Rectangle(1, NorthNorthRect.Bottom, NorthNorthRect.Width, OrbDDContent.Top/2);
            var southSouthRect = Rectangle.FromLTRB(1,
                (Height - OrbDDContent.Bottom)/2 + OrbDDContent.Bottom, Width - 1, Height - 1);

            var OrbDropDownDarkBorder = Theme.ColorTable.OrbDropDownDarkBorder;
            var OrbDropDownLightBorder = Theme.ColorTable.OrbDropDownLightBorder;
            var OrbDropDownBack = Theme.ColorTable.OrbDropDownBack;
            var OrbDropDownNorthA = Theme.ColorTable.OrbDropDownNorthA;
            var OrbDropDownNorthB = Theme.ColorTable.OrbDropDownNorthB;
            var OrbDropDownNorthC = Theme.ColorTable.OrbDropDownNorthC;
            var OrbDropDownNorthD = Theme.ColorTable.OrbDropDownNorthD;
            var OrbDropDownSouthC = Theme.ColorTable.OrbDropDownSouthC;
            var OrbDropDownSouthD = Theme.ColorTable.OrbDropDownSouthD;
            var OrbDropDownContentbg = Theme.ColorTable.OrbDropDownContentbg;
            var OrbDropDownContentbglight = Theme.ColorTable.OrbDropDownContentbglight;
            var OrbDropDownSeparatorlight = Theme.ColorTable.OrbDropDownSeparatorlight;
            var OrbDropDownSeparatordark = Theme.ColorTable.OrbDropDownSeparatordark;

            var innerPath = RoundRectangle(InnerRect, 6);
            var outerPath = RoundRectangle(OuterRect, 6);

            e.Graphics.SmoothingMode = SmoothingMode.None;

            using (Brush b = new SolidBrush(Color.FromArgb(0x8e, 0x8e, 0x8e)))
            {
                e.Graphics.FillRectangle(b, new Rectangle(Width - 10, Height - 10, 10, 10));
            }

            using (Brush b = new SolidBrush(OrbDropDownBack))
            {
                e.Graphics.FillPath(b, outerPath);
            }

            GradientRect(e.Graphics, NorthNorthRect, OrbDropDownNorthA, OrbDropDownNorthB);
            GradientRect(e.Graphics, northSouthRect, OrbDropDownNorthC, OrbDropDownNorthD);
            GradientRect(e.Graphics, southSouthRect, OrbDropDownSouthC, OrbDropDownSouthD);

            using (var p = new Pen(OrbDropDownDarkBorder))
            {
                e.Graphics.DrawPath(p, outerPath);
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (var p = new Pen(OrbDropDownLightBorder))
            {
                e.Graphics.DrawPath(p, innerPath);
            }

            innerPath.Dispose();
            outerPath.Dispose();

            #region Content

            InnerRect = OrbDDContent;
            InnerRect.Inflate(0, 0);
            OuterRect = OrbDDContent;
            OuterRect.Inflate(1, 1);

            using (var b = new SolidBrush(OrbDropDownContentbg))
            {
                e.Graphics.FillRectangle(b, OrbDDContent);
            }

            //Steve - Recent Items Caption
            if (e.RibbonOrbDropDown.ContentRecentItemsCaptionBounds.Height > 0)
            {
                var cb = e.RibbonOrbDropDown.ContentRecentItemsCaptionBounds;

                //draw the lines first since we need to adjust the bounds for the text portion
                var linePos = Convert.ToInt32(e.RibbonOrbDropDown.RecentItemsCaptionLineSpacing/2);

                using (var p = new Pen(OrbDropDownSeparatorlight))
                {
                    e.Graphics.DrawLine(p,
                        new Point(OrbDDContent.Left, cb.Bottom - linePos),
                        new Point(OrbDDContent.Right, cb.Bottom - linePos));
                }
                using (var p = new Pen(OrbDropDownSeparatordark))
                {
                    e.Graphics.DrawLine(p,
                        new Point(OrbDDContent.Left, cb.Bottom - linePos - 1),
                        new Point(OrbDDContent.Right, cb.Bottom - linePos - 1));
                }

                //adjust the bounds for the text margins and line height
                cb.X += e.Ribbon.ItemMargin.Left;
                cb.Width -= (e.Ribbon.ItemMargin.Left + e.Ribbon.ItemMargin.Right);
                cb.Height -= e.RibbonOrbDropDown.RecentItemsCaptionLineSpacing;

                var sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                if (e.Ribbon.RightToLeft == RightToLeft.Yes)
                {
                    sf.Alignment = StringAlignment.Far;
                }
                else
                {
                    sf.Alignment = StringAlignment.Near;
                }
                e.Graphics.DrawString(e.RibbonOrbDropDown.RecentItemsCaption, new Font(e.Ribbon.RibbonTabFont.FontFamily, e.Ribbon.RibbonTabFont.Size, FontStyle.Bold), Brushes.DarkBlue, cb, sf);
            }

            using (var b = new SolidBrush(OrbDropDownContentbglight))
            {
                //Menu items
                e.Graphics.FillRectangle(b, Bcontent);
            }

            using (var p = new Pen(OrbDropDownSeparatorlight))
            {
                e.Graphics.DrawLine(p, Bcontent.Right, Bcontent.Top, Bcontent.Right, Bcontent.Bottom);
            }

            using (var p = new Pen(OrbDropDownSeparatordark))
            {
                e.Graphics.DrawLine(p, Bcontent.Right - 1, Bcontent.Top, Bcontent.Right - 1, Bcontent.Bottom);
            }

            using (var p = new Pen(OrbDropDownLightBorder))
            {
                e.Graphics.DrawRectangle(p, OuterRect);
            }

            using (var p = new Pen(OrbDropDownDarkBorder))
            {
                e.Graphics.DrawRectangle(p, InnerRect);
            }

            #endregion

            #region Orb

            if (e.Ribbon.OrbVisible && e.Ribbon.CaptionBarVisible && e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2007)
            {
                var orbb = e.Ribbon.RectangleToScreen(e.Ribbon.OrbBounds);
                orbb = e.RibbonOrbDropDown.RectangleToClient(orbb);
                DrawOrb(e.Graphics, orbb, e.Ribbon.OrbImage, e.Ribbon.OrbSelected, e.Ribbon.OrbPressed);
            }

            #endregion
        }

        public override void OnRenderPanelPopupBackground(RibbonCanvasEventArgs e)
        {
            var pnl = e.RelatedObject as RibbonPanel;

            if (pnl == null) return;

            var darkBorder = Rectangle.FromLTRB(
                e.Bounds.Left,
                e.Bounds.Top,
                e.Bounds.Right,
                e.Bounds.Bottom);

            var lightBorder = Rectangle.FromLTRB(
                e.Bounds.Left + 1,
                e.Bounds.Top + 1,
                e.Bounds.Right - 1,
                e.Bounds.Bottom - 1);

            var textArea =
                Rectangle.FromLTRB(
                    e.Bounds.Left + 1,
                    pnl.ContentBounds.Bottom,
                    e.Bounds.Right - 1,
                    e.Bounds.Bottom - 1);

            var dark = RoundRectangle(darkBorder, 3);
            var light = RoundRectangle(lightBorder, 3);
            var txt = RoundRectangle(textArea, 3, Corners.SouthEast | Corners.SouthWest);

            using (var p = new Pen(Theme.ColorTable.PanelLightBorder))
            {
                e.Graphics.DrawPath(p, light);
            }

            using (var p = new Pen(Theme.ColorTable.PanelDarkBorder))
            {
                e.Graphics.DrawPath(p, dark);
            }

            using (var b = new SolidBrush(Theme.ColorTable.PanelBackgroundSelected))
            {
                e.Graphics.FillPath(b, light);
            }

            using (var b = new SolidBrush(Theme.ColorTable.PanelTextBackground))
            {
                e.Graphics.FillPath(b, txt);
            }

            txt.Dispose();
            dark.Dispose();
            light.Dispose();
        }

        public override void OnRenderRibbonBackground(RibbonRenderEventArgs e)
        {
            //if (e.Ribbon.ActualBorderMode == RibbonWindowMode.NonClientAreaGlass)
            //{
            //    e.Graphics.Clear(Color.Transparent);
            //    SmoothingMode sbuff = e.Graphics.SmoothingMode;
            //    e.Graphics.SmoothingMode = SmoothingMode.None;
            //    e.Graphics.FillRectangle(new SolidBrush(Theme.ColorTable.RibbonBackground), new Rectangle(0, e.Ribbon.CaptionBarSize + 1, e.Ribbon.Width, e.Ribbon.Height));
            //    e.Graphics.SmoothingMode = sbuff;
            //}
            //else
            //{
            //   e.Graphics.Clear(Theme.ColorTable.RibbonBackground);
            //}

            if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2007 | e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2010)
            {
                e.Graphics.Clear(Theme.ColorTable.RibbonBackground);
            }
            if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2013)
            {
                e.Graphics.Clear(Theme.ColorTable.RibbonBackground_2013);
            }

            if (e.Ribbon.ActualBorderMode == RibbonWindowMode.NonClientAreaGlass)
            {
                WinApi.FillForGlass(e.Graphics, new Rectangle(0, 0, e.Ribbon.Width, e.Ribbon.CaptionBarSize + 1));
            }
        }

        public override void OnRenderRibbonCaptionBar(RibbonRenderEventArgs e)
        {
            if (e.Ribbon.CaptionBarVisible)
            {
                var captionBar = new Rectangle(0, 0, e.Ribbon.Width, e.Ribbon.CaptionBarSize);
                if (!(e.Ribbon.ActualBorderMode == RibbonWindowMode.NonClientAreaGlass && RibbonDesigner.Current == null))
                {
                    //DrawCaptionBarBackground(captionBar, e.Graphics);
                }
                DrawCaptionBarText(e.Ribbon.CaptionTextBounds, e);
            }
        }

        public override void OnRenderRibbonItem(RibbonItemRenderEventArgs e)
        {
            if (e.Item is RibbonButton)
            {
                #region Button

                var b = e.Item as RibbonButton;

                if (b.Enabled)
                {
                    if (b.Style == RibbonButtonStyle.Normal)
                    {
                        if (b.Pressed && b.SizeMode != RibbonElementSizeMode.DropDown)
                        {
                            DrawButtonPressed(e.Graphics, b, e.Ribbon);
                        }
                        else if (b.Selected)
                        {
                            if (b.Checked)
                            {
                                DrawButtonPressed(e.Graphics, b, e.Ribbon);
                            }
                            else
                            {
                                DrawButtonSelected(e.Graphics, b, e.Ribbon);
                            }
                        }
                        else if (b.Checked)
                        {
                            DrawButtonChecked(e.Graphics, b);
                        }
                        else if (b is RibbonOrbOptionButton)
                        {
                            DrawOrbOptionButton(e.Graphics, b.Bounds);
                        }
                    }
                    else
                    {
                        if (b.Style == RibbonButtonStyle.DropDownListItem)
                        {
                            //clear out the drowdown background so we don't see the image area shading
                            using (var br = new SolidBrush(Theme.ColorTable.DropDownBg))
                            {
                                //e.Graphics.Clear(Color.Transparent);
                                var sbuff = e.Graphics.SmoothingMode;
                                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                                e.Graphics.FillRectangle(br, b.Bounds);
                                e.Graphics.SmoothingMode = sbuff;
                            }
                        }

                        if (b.DropDownPressed && b.SizeMode != RibbonElementSizeMode.DropDown)
                        {
                            DrawButtonPressed(e.Graphics, b, e.Ribbon);
                            DrawSplitButtonDropDownSelected(e, b);
                        }
                        else if (b.Pressed && b.SizeMode != RibbonElementSizeMode.DropDown)
                        {
                            DrawButtonPressed(e.Graphics, b, e.Ribbon);
                            DrawSplitButtonSelected(e, b);
                        }

                        else if (b.DropDownSelected)
                        {
                            DrawButtonSelected(e.Graphics, b, e.Ribbon);
                            DrawSplitButtonDropDownSelected(e, b);
                        }
                        else if (b.Selected)
                        {
                            DrawButtonSelected(e.Graphics, b, e.Ribbon);
                            DrawSplitButtonSelected(e, b);
                        }
                        else if (b.Checked)
                        {
                            DrawButtonChecked(e.Graphics, b);
                        }
                        else
                        {
                            DrawSplitButton(e, b);
                        }
                    }
                }


                if (b.Style != RibbonButtonStyle.Normal && !(b.Style == RibbonButtonStyle.DropDown && b.SizeMode == RibbonElementSizeMode.Large))
                {
                    if (b.Style == RibbonButtonStyle.DropDown)
                    {
                        DrawButtonDropDownArrow(e.Graphics, b, b.OnGetDropDownBounds(b.SizeMode, b.Bounds));
                    }
                    else
                    {
                        DrawButtonDropDownArrow(e.Graphics, b, b.DropDownBounds);
                    }
                }

                #endregion
            }
            else if (e.Item is RibbonItemGroup)
            {
                #region Group

                DrawItemGroup(e, e.Item as RibbonItemGroup);

                #endregion
            }
            else if (e.Item is RibbonButtonList)
            {
                #region ButtonList

                DrawButtonList(e.Graphics, e.Item as RibbonButtonList, e.Ribbon);

                #endregion
            }
            else if (e.Item is RibbonSeparator)
            {
                #region Separator

                if (e.Item.Visible)
                {
                    DrawSeparator(e.Graphics, e.Item as RibbonSeparator);
                }

                #endregion
            }
            else if (e.Item is RibbonUpDown)
            {
                #region UpDown

                var t = e.Item as RibbonUpDown;

                if (t.Enabled)
                {
                    if (t != null && (t.Selected || (t.Editing)))
                    {
                        DrawTextBoxSelected(e.Graphics, t.TextBoxBounds);
                    }
                    else
                    {
                        DrawTextBoxUnselected(e.Graphics, t.TextBoxBounds);
                    }
                }
                else
                {
                    DrawTextBoxDisabled(e.Graphics, t.TextBoxBounds);
                }

                DrawUpDownButtons(e.Graphics, t, e.Ribbon);

                #endregion
            }
            else if (e.Item is RibbonTextBox)
            {
                #region TextBox

                var t = e.Item as RibbonTextBox;

                if (t.Enabled)
                {
                    if (t != null && (t.Selected || (t.Editing)))
                    {
                        DrawTextBoxSelected(e.Graphics, t.TextBoxBounds);
                    }
                    else
                    {
                        DrawTextBoxUnselected(e.Graphics, t.TextBoxBounds);
                    }
                }
                else
                {
                    DrawTextBoxDisabled(e.Graphics, t.TextBoxBounds);
                }

                if (t is RibbonComboBox)
                {
                    DrawComboxDropDown(e.Graphics, t as RibbonComboBox, e.Ribbon);
                }

                #endregion
            }
        }

        public override void OnRenderRibbonItemBorder(RibbonItemRenderEventArgs e)
        {
            if (e.Item is RibbonItemGroup)
            {
                DrawItemGroupBorder(e, e.Item as RibbonItemGroup);
            }
        }

        public override void OnRenderRibbonItemImage(RibbonItemBoundsEventArgs e)
        {
            var img = e.Item.ShowFlashImage ? e.Item.FlashImage : e.Item.Image;

            if (e.Item is RibbonButton)
            {
                if (!(e.Item.SizeMode == RibbonElementSizeMode.Large || e.Item.SizeMode == RibbonElementSizeMode.Overflow))
                {
                    img = e.Item.ShowFlashImage ? (e.Item as RibbonButton).SmallImage : (e.Item as RibbonButton).SmallImage;
                }
            }

            if (img != null)
            {
                if (!e.Item.Enabled)
                    img = CreateDisabledImage(img);

                e.Graphics.DrawImage(img, e.Bounds);
            }
        }

        public override void OnRenderRibbonItemText(RibbonTextEventArgs e)
        {
            var foreColor = e.Color;
            var sf = e.Format;
            var f = e.Ribbon.Font;
            var embedded = false;

            if (e.Item is RibbonButton)
            {
                #region Button

                var b = e.Item as RibbonButton;

                if (b is RibbonCaptionButton)
                {
                    if (WinApi.IsWindows) f = new Font(RibbonCaptionButton.WindowsIconsFont, f.Size);
                    embedded = true;
                    foreColor = Theme.ColorTable.Arrow;
                }

                if (b.Style == RibbonButtonStyle.DropDown && b.SizeMode == RibbonElementSizeMode.Large)
                {
                    DrawButtonDropDownArrow(e.Graphics, b, e.Bounds);
                }

                #endregion
            }
            else if (e.Item is RibbonSeparator)
            {
                if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2007 | e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2010)
                    foreColor = GetTextColor(e.Item.Enabled, Theme.ColorTable.Text);
                else if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2013)
                    foreColor = GetTextColor(e.Item.Enabled, Theme.ColorTable.RibbonItemText_2013);
            }

            embedded = embedded || !e.Item.Enabled;

            if (embedded)
            {
                var cbr = e.Bounds;
                cbr.Y++;
                using (var b = new SolidBrush(Theme.ColorTable.ArrowLight))
                {
                    e.Graphics.DrawString(e.Text, new Font(f, e.Style), b, cbr, sf);
                }
            }

            if (foreColor.Equals(Color.Empty))
            {
                if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2007 | e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2010)
                    foreColor = GetTextColor(e.Item.Enabled, Theme.ColorTable.Text);
                else if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2013)
                    foreColor = GetTextColor(e.Item.Enabled, Theme.ColorTable.RibbonItemText_2013);
            }

            using (var b = new SolidBrush(foreColor))
            {
                e.Graphics.DrawString(e.Text, new Font(f, e.Style), b, e.Bounds, sf);
            }
        }

        public override void OnRenderRibbonOrb(RibbonRenderEventArgs e)
        {
            if (e.Ribbon.OrbVisible)
            {
                if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2007)
                {
                    if (e.Ribbon.CaptionBarVisible)
                        DrawOrb(e.Graphics, e.Ribbon.OrbBounds, e.Ribbon.OrbImage, e.Ribbon.OrbSelected, e.Ribbon.OrbPressed);
                }
                else if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2010 | e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2013) //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
                {
                    //draw 2010 style
                    var args = new RibbonRenderEventArgs(e.Ribbon, e.Graphics, e.Ribbon.OrbBounds);
                    if (e.Ribbon.OrbPressed)
                    {
                        DrawOrbPressed(args);
                    }
                    else if (e.Ribbon.OrbSelected)
                    {
                        DrawOrbSelected(args);
                    }
                    else
                    {
                        DrawOrbNormal(args);
                    }

                    if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2010)
                    {
                        if (e.Ribbon.OrbText != string.Empty)
                            TextRenderer.DrawText(e.Graphics, e.Ribbon.OrbText, e.Ribbon.RibbonTabFont, e.Ribbon.OrbBounds, Theme.ColorTable.OrbButtonText);
                    }
                    else if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2013)
                    {
                        if (e.Ribbon.OrbText != string.Empty)
                            TextRenderer.DrawText(e.Graphics, e.Ribbon.OrbText, e.Ribbon.RibbonTabFont, e.Ribbon.OrbBounds, Theme.ColorTable.OrbButtonText_2013);
                    }

                    if (e.Ribbon.OrbImage != null)
                    {
                        var irect = new Rectangle(Point.Empty, e.Ribbon.OrbImage.Size);
                        irect.X = e.Ribbon.OrbBounds.X + (e.Ribbon.OrbBounds.Width - irect.Width)/2;
                        irect.Y = e.Ribbon.OrbBounds.Y + (e.Ribbon.OrbBounds.Height - irect.Height)/2;
                        e.Graphics.DrawImage(e.Ribbon.OrbImage, irect);
                    }
                }
            }
        }

        public override void OnRenderRibbonPanelBackground(RibbonPanelRenderEventArgs e)
        {
            if (e.Panel.OverflowMode && !(e.Canvas is RibbonPanelPopup))
            {
                if (e.Panel.Pressed)
                {
                    DrawPanelOverflowPressed(e);
                }
                else if (e.Panel.Selected)
                {
                    DrawPannelOveflowSelected(e);
                }
                else
                {
                    DrawPanelOverflowNormal(e);
                }
            }
            else
            {
                if (e.Panel.Selected)
                {
                    if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2007 | e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2010) //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
                    {
                        DrawPanelSelected(e);
                    }
                    else if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2013) //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
                    {
                        //DO NOT DRAW ANYTHING
                    }
                }
                else
                {
                    DrawPanelNormal(e);
                }
            }
        }

        public override void OnRenderRibbonPanelText(RibbonPanelRenderEventArgs e)
        {
            if (e.Panel.OverflowMode && !(e.Canvas is RibbonPanelPopup))
            {
                return;
            }

            var textArea =
                Rectangle.FromLTRB(
                    e.Panel.Bounds.Left + 1,
                    e.Panel.ContentBounds.Bottom,
                    e.Panel.Bounds.Right - 1,
                    e.Panel.Bounds.Bottom - 1);

            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2007 | e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2010)
            {
                using (Brush b = new SolidBrush(GetTextColor(e.Panel.Enabled, Theme.ColorTable.PanelText)))
                {
                    e.Graphics.DrawString(e.Panel.Text, e.Ribbon.Font, b, textArea, sf);
                }
            }
            else if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2013)
            {
                using (Brush b = new SolidBrush(GetTextColor(e.Panel.Enabled, Theme.ColorTable.PanelText_2013)))
                {
                    e.Graphics.DrawString(e.Panel.Text, e.Ribbon.Font, b, textArea, sf);
                }
            }
        }

        public override void OnRenderRibbonQuickAccessToolbarBackground(RibbonRenderEventArgs e)
        {
            /// a-----b    a-----b
            ///  z    |   z       |
            ///   c---d    c-----d
            var bounds = e.Ribbon.QuickAcessToolbar.Bounds;
            Padding padding = e.Ribbon.QuickAcessToolbar.Padding;
            Padding margin = e.Ribbon.QuickAcessToolbar.Margin;
            var a = new Point(bounds.Left - (e.Ribbon.OrbVisible ? margin.Left : 0), bounds.Top);
            var b = new Point(bounds.Right + padding.Right, bounds.Top);
            var c = new Point(bounds.Left, bounds.Bottom);
            var d = new Point(b.X, c.Y);
            var z = new Point(c.X - 2, a.Y + bounds.Height/2 - 1);

            var aero = e.Ribbon.ActualBorderMode == RibbonWindowMode.NonClientAreaGlass && RibbonDesigner.Current == null;
            if (e.Ribbon.RightToLeft == RightToLeft.Yes)
            {
                ///   a-----b    a-----b
                ///  |     z    |       z
                ///   c---d      c-----d
                a = new Point(bounds.Left + padding.Left, bounds.Top);
                b = new Point(bounds.Right + (e.Ribbon.OrbVisible ? margin.Right : 0), bounds.Top);
                c = new Point(a.X, bounds.Bottom);
                d = new Point(bounds.Right, bounds.Bottom);
                z = new Point(d.X + 2, b.Y + (bounds.Height/2) - 1);
            }

            using (var path = CreateQuickAccessPath(a, b, c, d, z, bounds, 0, 0, e.Ribbon))
            {
                if (!aero)
                {
                    using (var p = new Pen(Theme.ColorTable.QuickAccessBorderLight, 3))
                    {
                        e.Graphics.DrawPath(p, path);
                    }
                }
                using (var p = new Pen(Theme.ColorTable.QuickAccessBorderDark, 1))
                {
                    if (aero) p.Color = Color.FromArgb(150, 150, 150);
                    e.Graphics.DrawPath(p, path);
                }
                if (e.Ribbon.RightToLeft == RightToLeft.Yes)
                {
                    b = a;
                    d = c;
                }
                if (!aero)
                {
                    using (var br = new LinearGradientBrush(
                        b, d, Color.FromArgb(150, Theme.ColorTable.QuickAccessUpper), Color.FromArgb(150, Theme.ColorTable.QuickAccessLower)
                        ))
                    {
                        e.Graphics.FillPath(br, path);
                    }
                }
                else
                {
                    using (var br = new LinearGradientBrush(
                        b, d,
                        Color.FromArgb(66, RibbonProfesionalRendererColorTable.ToGray(Theme.ColorTable.QuickAccessUpper)),
                        Color.FromArgb(66, RibbonProfesionalRendererColorTable.ToGray(Theme.ColorTable.QuickAccessLower))
                        ))
                    {
                        e.Graphics.FillPath(br, path);
                    }
                }
            }
        }

        public override void OnRenderRibbonTab(RibbonTabRenderEventArgs e)
        {
            if (e.Ribbon.Minimized && !e.Ribbon.Expanded)
            {
                DrawTabMinimized(e);
            }
            else if (e.Tab.Active)
            {
                DrawCompleteTab(e);
            }
            else if (e.Tab.Pressed)
            {
                //DrawTabPressed(e);
            }
            else if (e.Tab.Selected)
            {
                if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2010 | e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2013) //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
                    DrawTabMinimized(e);
                else if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2007)
                    DrawTabSelected(e);
            }
            else
            {
                DrawTabNormal(e);
            }
        }

        public override void OnRenderRibbonTabText(RibbonTabRenderEventArgs e)
        {
            var sf = new StringFormat();

            sf.Alignment = StringAlignment.Center;
            sf.Trimming = StringTrimming.EllipsisCharacter;
            sf.LineAlignment = StringAlignment.Center;
            sf.FormatFlags |= StringFormatFlags.NoWrap;

            var r = Rectangle.FromLTRB(e.Tab.TabBounds.Left + e.Ribbon.TabTextMargin.Left, e.Tab.TabBounds.Top + e.Ribbon.TabTextMargin.Top, e.Tab.TabBounds.Right - e.Ribbon.TabTextMargin.Right, e.Tab.TabBounds.Bottom - e.Ribbon.TabTextMargin.Bottom);

            if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2007 | e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2010) //Michael Spradlin - 05/03/2013 Office 2013 Style Changes
            {
                using (Brush b = new SolidBrush(GetTextColor(true, e.Tab.Active ? Theme.ColorTable.TabActiveText : Theme.ColorTable.TabText)))
                {
                    e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    e.Graphics.DrawString(e.Tab.Text, e.Ribbon.RibbonTabFont, b, r, sf);
                }
            }
            else if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2013)
            {
                using (Brush b = new SolidBrush(GetTextColor(true, e.Tab.Active | e.Tab.Selected ? Theme.ColorTable.TabText_2013 : Theme.ColorTable.TabTextSelected_2013)))
                {
                    e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    e.Graphics.DrawString(e.Tab.Text, e.Ribbon.RibbonTabFont, b, r, sf);
                }
            }
        }

        public override void OnRenderScrollbar(Graphics g, Control ctl, Ribbon ribbon)
        {
            var list = (RibbonDropDown) ctl;
            if (ScrollBarRenderer.IsSupported)
            {
                ScrollBarRenderer.DrawUpperVerticalTrack(g, list.ScrollBarBounds, ScrollBarState.Normal);

                if (list.ThumbPressed)
                {
                    ScrollBarRenderer.DrawVerticalThumb(g, list.ThumbBounds, ScrollBarState.Pressed);
                    ScrollBarRenderer.DrawVerticalThumbGrip(g, list.ThumbBounds, ScrollBarState.Pressed);
                }
                else if (list.ThumbSelected)
                {
                    ScrollBarRenderer.DrawVerticalThumb(g, list.ThumbBounds, ScrollBarState.Hot);
                    ScrollBarRenderer.DrawVerticalThumbGrip(g, list.ThumbBounds, ScrollBarState.Hot);
                }
                else
                {
                    ScrollBarRenderer.DrawVerticalThumb(g, list.ThumbBounds, ScrollBarState.Normal);
                    ScrollBarRenderer.DrawVerticalThumbGrip(g, list.ThumbBounds, ScrollBarState.Normal);
                }

                if (list.ButtonUpPressed)
                {
                    ScrollBarRenderer.DrawArrowButton(g, list.ButtonUpBounds, ScrollBarArrowButtonState.UpPressed);
                }
                else if (list.ButtonUpSelected)
                {
                    ScrollBarRenderer.DrawArrowButton(g, list.ButtonUpBounds, ScrollBarArrowButtonState.UpHot);
                }
                else
                {
                    ScrollBarRenderer.DrawArrowButton(g, list.ButtonUpBounds, ScrollBarArrowButtonState.UpNormal);
                }

                if (list.ButtonDownPressed)
                {
                    ScrollBarRenderer.DrawArrowButton(g, list.ButtonDownBounds, ScrollBarArrowButtonState.DownPressed);
                }
                else if (list.ButtonDownSelected)
                {
                    ScrollBarRenderer.DrawArrowButton(g, list.ButtonDownBounds, ScrollBarArrowButtonState.DownHot);
                }
                else
                {
                    ScrollBarRenderer.DrawArrowButton(g, list.ButtonDownBounds, ScrollBarArrowButtonState.DownNormal);
                }
            }
            else
            {
                #region Control Buttons

                using (var b = new SolidBrush(Theme.ColorTable.ButtonGlossyNorth))
                {
                    g.FillRectangle(b, list.ScrollBarBounds);
                }

                if (!list.ButtonDownEnabled)
                {
                    DrawButtonDisabled(g, list.ButtonDownBounds, Corners.SouthEast);
                }
                else if (list.ButtonDownPressed)
                {
                    DrawButtonPressed(g, list.ButtonDownBounds, Corners.SouthEast, ribbon);
                }
                else if (list.ButtonDownSelected)
                {
                    DrawButtonSelected(g, list.ButtonDownBounds, Corners.SouthEast, ribbon);
                }
                else
                {
                    DrawButton(g, list.ButtonDownBounds, Corners.None);
                }

                if (!list.ButtonUpEnabled)
                {
                    DrawButtonDisabled(g, list.ButtonUpBounds, Corners.NorthEast);
                }
                else if (list.ButtonUpPressed)
                {
                    DrawButtonPressed(g, list.ButtonUpBounds, Corners.NorthEast, ribbon);
                }
                else if (list.ButtonUpSelected)
                {
                    DrawButtonSelected(g, list.ButtonUpBounds, Corners.NorthEast, ribbon);
                }
                else
                {
                    DrawButton(g, list.ButtonUpBounds, Corners.NorthEast);
                }

                if (list.ScrollBarEnabled)
                {
                    if (list.ThumbPressed)
                    {
                        DrawButtonPressed(g, list.ThumbBounds, Corners.All, ribbon);
                    }
                    else if (list.ThumbSelected)
                    {
                        DrawButtonSelected(g, list.ThumbBounds, Corners.All, ribbon);
                    }
                    else
                    {
                        DrawButton(g, list.ThumbBounds, Corners.All);
                    }
                }

                var dk = Theme.ColorTable.Arrow;
                var lt = Theme.ColorTable.ArrowLight;
                var ds = Theme.ColorTable.ArrowDisabled;

                var arrUp = CenterOn(list.ButtonUpBounds, new Rectangle(Point.Empty, arrowSize));
                arrUp.Offset(0, 1);
                var arrD = CenterOn(list.ButtonDownBounds, new Rectangle(Point.Empty, arrowSize));
                arrD.Offset(0, 1);

                DrawArrow(g, arrUp, list.ButtonUpEnabled ? lt : Color.Transparent, RibbonArrowDirection.Up);
                arrUp.Offset(0, -1);
                DrawArrow(g, arrUp, list.ButtonUpEnabled ? dk : ds, RibbonArrowDirection.Up);

                DrawArrow(g, arrD, list.ButtonDownEnabled ? lt : Color.Transparent, RibbonArrowDirection.Down);
                arrD.Offset(0, -1);
                DrawArrow(g, arrD, list.ButtonDownEnabled ? dk : ds, RibbonArrowDirection.Down);

                #endregion
            }
        }

        public override void OnRenderTabScrollButtons(RibbonTabRenderEventArgs e)
        {
            if (e.Tab.ScrollLeftVisible)
            {
                if (e.Tab.ScrollLeftSelected)
                {
                    DrawButtonSelected(e.Graphics, e.Tab.ScrollLeftBounds, Corners.West, e.Ribbon);
                }
                else
                {
                    DrawButton(e.Graphics, e.Tab.ScrollLeftBounds, Corners.West);
                }

                DrawArrowShaded(e.Graphics, e.Tab.ScrollLeftBounds, RibbonArrowDirection.Right, true);
            }

            if (e.Tab.ScrollRightVisible)
            {
                if (e.Tab.ScrollRightSelected)
                {
                    DrawButtonSelected(e.Graphics, e.Tab.ScrollRightBounds, Corners.East, e.Ribbon);
                }
                else
                {
                    DrawButton(e.Graphics, e.Tab.ScrollRightBounds, Corners.East);
                }

                DrawArrowShaded(e.Graphics, e.Tab.ScrollRightBounds, RibbonArrowDirection.Left, true);
            }
        }

        public override void OnRenderToolTipBackground(RibbonToolTipRenderEventArgs e)
        {
            var darkBorder = Rectangle.FromLTRB(
                e.ClipRectangle.Left,
                e.ClipRectangle.Top,
                e.ClipRectangle.Right - 1,
                e.ClipRectangle.Bottom - 1);

            var lightBorder = Rectangle.FromLTRB(
                e.ClipRectangle.Left + 1,
                e.ClipRectangle.Top + 1,
                e.ClipRectangle.Right - 2,
                e.ClipRectangle.Bottom - 1);

            var dark = RoundRectangle(darkBorder, 3);
            var light = RoundRectangle(lightBorder, 3);

            //Draw the Drop Shadow
            var shadow = e.ClipRectangle;
            shadow.Offset(2, 1);
            using (var path = RoundRectangle(shadow, 3, Corners.All))
            {
                using (var b = new PathGradientBrush(path))
                {
                    b.WrapMode = WrapMode.Clamp;

                    var cb = new ColorBlend(3);
                    cb.Colors = new[]
                    {
                        Color.Transparent,
                        Color.FromArgb(50, Color.Black),
                        Color.FromArgb(100, Color.Black)
                    };
                    cb.Positions = new[] {0f, .1f, 1f};

                    b.InterpolationColors = cb;

                    e.Graphics.FillPath(b, path);
                }
            }

            //Fill the background
            //using (SolidBrush b = new SolidBrush(Theme.ColorTable.ToolTipContentSouth))
            //{
            //   e.Graphics.FillPath(b, dark);
            //}
            using (var b = new LinearGradientBrush(
                e.ClipRectangle, Theme.ColorTable.ToolTipContentNorth, Theme.ColorTable.ToolTipContentSouth, 90))
            {
                e.Graphics.FillPath(b, dark);
            }

            //Draw the borders
            using (var p = new Pen(Theme.ColorTable.ToolTipLightBorder))
            {
                e.Graphics.DrawPath(p, light);
            }
            using (var p = new Pen(Theme.ColorTable.ToolTipDarkBorder))
            {
                e.Graphics.DrawPath(p, dark);
            }

            dark.Dispose();
            light.Dispose();
        }

        public override void OnRenderToolTipImage(RibbonToolTipRenderEventArgs e)
        {
            e.Graphics.DrawImage(e.TipImage, e.ClipRectangle);
        }

        public override void OnRenderToolTipText(RibbonToolTipRenderEventArgs e)
        {
            if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2007 | e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2010)
            {
                using (Brush b = new SolidBrush(Theme.ColorTable.ToolTipText))
                {
                    e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    e.Graphics.DrawString(e.Text, e.Font, b, e.ClipRectangle, e.Format);
                }
            }
            else if (e.Ribbon.OrbStyle == RibbonOrbStyle.Office_2013)
            {
                using (Brush b = new SolidBrush(Theme.ColorTable.ToolTipText_2013))
                {
                    e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    e.Graphics.DrawString(e.Text, e.Font, b, e.ClipRectangle, e.Format);
                }
            }
        }

        #endregion
    }
}