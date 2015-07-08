#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview
{
    public class TreeColumn : IDisposable
    {
        private readonly StringFormat _headerFormat;

        #region Properties

        private string _header;
        private HorizontalAlignment _textAlign = HorizontalAlignment.Left;
        private bool _visible = true;
        private int _width;
        internal TreeViewAdv TreeView { get; set; }

        [Browsable(false)]
        public int Index { get; internal set; }

        [Localizable(true)]
        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                if (TreeView != null)
                    TreeView.UpdateHeaders();
            }
        }

        [DefaultValue(50), Localizable(true)]
        public int Width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    if (value < 0)
                        throw new ArgumentOutOfRangeException("value");

                    _width = value;
                    if (TreeView != null)
                        TreeView.ChangeColumnWidth(this);
                }
            }
        }

        [DefaultValue(true)]
        public bool IsVisible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                if (TreeView != null)
                    TreeView.FullUpdate();
            }
        }

        [DefaultValue(HorizontalAlignment.Left)]
        public HorizontalAlignment TextAlign
        {
            get { return _textAlign; }
            set { _textAlign = value; }
        }

        #endregion

        public TreeColumn() :
            this(string.Empty, 50)
        {
        }

        public TreeColumn(string header, int width)
        {
            _header = header;
            _width = width;

            _headerFormat = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces);
            _headerFormat.LineAlignment = StringAlignment.Center;
            _headerFormat.Trimming = StringTrimming.EllipsisCharacter;
        }

        public void Draw(Graphics gr, Rectangle bounds, Font font)
        {
            _headerFormat.Alignment = TextHelper.TranslateAligment(TextAlign);
            gr.DrawString(Header + " ", font, SystemBrushes.WindowText, bounds, _headerFormat);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Header))
                return GetType().Name;
            return Header;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _headerFormat.Dispose();
        }

        #endregion
    }
}