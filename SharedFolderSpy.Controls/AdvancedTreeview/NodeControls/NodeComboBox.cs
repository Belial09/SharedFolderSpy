#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview.NodeControls
{
    public class NodeComboBox : BaseTextControl
    {
        private object _selectedItem;

        #region Properties

        private int _editorWidth = 100;

        [DefaultValue(100)]
        public int EditorWidth
        {
            get { return _editorWidth; }
            set { _editorWidth = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object[] DropDownItems { get; set; }

        #endregion

        protected override Size CalculateEditorSize(EditorContext context)
        {
            if (Parent.UseColumns)
                return context.Bounds.Size;
            return new Size(EditorWidth, context.Bounds.Height);
        }

        protected override Control CreateEditor(TreeNodeAdv node)
        {
            var comboBox = new ComboBox();
            if (DropDownItems != null)
                comboBox.Items.AddRange(DropDownItems);
            _selectedItem = GetValue(node);
            comboBox.SelectedItem = _selectedItem;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.KeyDown += EditorKeyDown;
            comboBox.SelectedIndexChanged += EditorSelectedIndexChanged;
            comboBox.Disposed += EditorDisposed;
            return comboBox;
        }

        protected override void DoApplyChanges(TreeNodeAdv node)
        {
            SetValue(node, _selectedItem);
        }

        private void EditorDisposed(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            comboBox.KeyDown -= EditorKeyDown;
            comboBox.SelectedIndexChanged -= EditorSelectedIndexChanged;
            comboBox.Disposed -= EditorDisposed;
        }

        private void EditorKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                EndEdit(true);
        }

        private void EditorSelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedItem = (sender as ComboBox).SelectedItem;
            Parent.HideEditor();
        }

        public override void UpdateEditor(Control control)
        {
            (control as ComboBox).DroppedDown = true;
        }
    }
}