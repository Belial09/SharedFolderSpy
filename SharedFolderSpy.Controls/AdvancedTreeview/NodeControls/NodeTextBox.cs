#region

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview.NodeControls
{
    public class NodeTextBox : BaseTextControl
    {
        private const int MinTextBoxWidth = 30;
        private string _label;
        private TextBox _textBox;

        protected override Size CalculateEditorSize(EditorContext context)
        {
            if (Parent.UseColumns)
                return context.Bounds.Size;
            var textBox = context.Editor as TextBox;
            var size = GetLabelSize(textBox.Text);
            var width = Math.Max(size.Width + Font.Height, MinTextBoxWidth); // reserve a place for new typed character
            return new Size(width, size.Height);
        }

        public void Copy()
        {
            if (_textBox != null)
                _textBox.Copy();
        }

        protected override Control CreateEditor(TreeNodeAdv node)
        {
            var textBox = new TextBox();
            textBox.TextAlign = TextAlign;
            textBox.Text = GetLabel(node);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.KeyDown += textBox_KeyDown;
            textBox.Disposed += textBox_Disposed;
            textBox.TextChanged += textBox_TextChanged;
            _label = textBox.Text;
            _textBox = textBox;
            return textBox;
        }

        public void Cut()
        {
            if (_textBox != null)
                _textBox.Cut();
        }

        public void Delete()
        {
            if (_textBox != null)
            {
                var len = Math.Max(_textBox.SelectionLength, 1);
                if (_textBox.SelectionStart < _textBox.Text.Length)
                {
                    var start = _textBox.SelectionStart;
                    _textBox.Text = _textBox.Text.Remove(_textBox.SelectionStart, len);
                    _textBox.SelectionStart = start;
                }
            }
        }

        protected override void DoApplyChanges(TreeNodeAdv node)
        {
            var oldLabel = GetLabel(node);
            if (oldLabel != _label)
            {
                SetLabel(node, _label);
                OnLabelChanged();
            }
        }

        public override void KeyDown(KeyEventArgs args)
        {
            if (args.KeyCode == Keys.F2 && Parent.CurrentNode != null)
            {
                args.Handled = true;
                BeginEdit();
            }
        }

        protected void OnLabelChanged()
        {
            if (LabelChanged != null)
                LabelChanged(this, EventArgs.Empty);
        }

        public void Paste()
        {
            if (_textBox != null)
                _textBox.Paste();
        }

        private void textBox_Disposed(object sender, EventArgs e)
        {
            _textBox.KeyDown -= textBox_KeyDown;
            _textBox.Disposed -= textBox_Disposed;
            _textBox.TextChanged -= textBox_TextChanged;
            _textBox = null;
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                EndEdit(true);
            else if (e.KeyCode == Keys.Enter)
                EndEdit(false);
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            _label = _textBox.Text;
            Parent.UpdateEditorBounds();
        }

        public event EventHandler LabelChanged;
    }
}