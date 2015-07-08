#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Fesslersoft.SharedFolderSpy.Controls.Properties;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview.NodeControls
{
    public class NodeCheckBox : BindableControl
    {
        public const int ImageSize = 13;
        public const int Width = 13;
        private readonly Bitmap _check;
        private readonly Bitmap _uncheck;
        private readonly Bitmap _unknown;

        #region Properties

        private bool _editEnabled = true;

        [DefaultValue(false)]
        public bool ThreeState { get; set; }

        [DefaultValue(true)]
        public bool EditEnabled
        {
            get { return _editEnabled; }
            set { _editEnabled = value; }
        }

        #endregion

        public NodeCheckBox()
            : this(string.Empty)
        {
        }

        public NodeCheckBox(string propertyName)
        {
            _check = Resources.check;
            _uncheck = Resources.uncheck;
            _unknown = Resources.unknown;
            DataPropertyName = propertyName;
        }

        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            var r = context.Bounds;
            var dy = (int) Math.Round((float) (r.Height - ImageSize)/2);
            var state = GetCheckState(node);
            if (Application.RenderWithVisualStyles)
            {
                VisualStyleRenderer renderer;
                if (state == CheckState.Indeterminate)
                    renderer = new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.MixedNormal);
                else if (state == CheckState.Checked)
                    renderer = new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.CheckedNormal);
                else
                    renderer = new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.UncheckedNormal);
                renderer.DrawBackground(context.Graphics, new Rectangle(r.X, r.Y + dy, ImageSize, ImageSize));
            }
            else
            {
                Image img;
                if (state == CheckState.Indeterminate)
                    img = _unknown;
                else if (state == CheckState.Checked)
                    img = _check;
                else
                    img = _uncheck;
                context.Graphics.DrawImage(img, new Point(r.X, r.Y + dy));
                //ControlPaint.DrawCheckBox(context.Graphics, r, state2);
            }
        }

        protected virtual CheckState GetCheckState(TreeNodeAdv node)
        {
            var obj = GetValue(node);
            if (obj is CheckState)
                return (CheckState) obj;
            if (obj is bool)
                return (bool) obj ? CheckState.Checked : CheckState.Unchecked;
            return CheckState.Unchecked;
        }

        private CheckState GetNewState(CheckState state)
        {
            if (state == CheckState.Indeterminate)
                return CheckState.Unchecked;
            if (state == CheckState.Unchecked)
                return CheckState.Checked;
            return ThreeState ? CheckState.Indeterminate : CheckState.Unchecked;
        }

        public override void KeyDown(KeyEventArgs args)
        {
            if (args.KeyCode == Keys.Space && EditEnabled)
            {
                Parent.BeginUpdate();
                try
                {
                    if (Parent.CurrentNode != null)
                    {
                        var value = GetNewState(GetCheckState(Parent.CurrentNode));
                        foreach (var node in Parent.Selection)
                            SetCheckState(node, value);
                    }
                }
                finally
                {
                    Parent.EndUpdate();
                }
                args.Handled = true;
            }
        }

        public override Size MeasureSize(TreeNodeAdv node)
        {
            return new Size(Width, Width);
        }

        public override void MouseDoubleClick(TreeNodeAdvMouseEventArgs args)
        {
            args.Handled = true;
        }

        public override void MouseDown(TreeNodeAdvMouseEventArgs args)
        {
            if (args.Button == MouseButtons.Left && EditEnabled)
            {
                var state = GetCheckState(args.Node);
                state = GetNewState(state);
                SetCheckState(args.Node, state);
                args.Handled = true;
            }
        }

        protected void OnCheckStateChanged(TreePathEventArgs args)
        {
            if (CheckStateChanged != null)
                CheckStateChanged(this, args);
        }

        protected void OnCheckStateChanged(TreeNodeAdv node)
        {
            var path = Parent.GetPath(node);
            OnCheckStateChanged(new TreePathEventArgs(path));
        }

        protected virtual void SetCheckState(TreeNodeAdv node, CheckState value)
        {
            var type = GetPropertyType(node);
            if (type == typeof (CheckState))
            {
                SetValue(node, value);
                OnCheckStateChanged(node);
            }
            else if (type == typeof (bool))
            {
                SetValue(node, value != CheckState.Unchecked);
                OnCheckStateChanged(node);
            }
        }

        public event EventHandler<TreePathEventArgs> CheckStateChanged;
    }
}