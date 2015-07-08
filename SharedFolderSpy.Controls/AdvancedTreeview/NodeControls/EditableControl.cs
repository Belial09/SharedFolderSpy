#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview.NodeControls
{
    public abstract class EditableControl : BindableControl
    {
        private readonly Timer _timer;
        private bool _discardChanges;
        private bool _editFlag;
        private TreeNodeAdv _editNode;

        protected EditableControl()
        {
            EditEnabled = false;
            _timer = new Timer();
            _timer.Interval = 500;
            _timer.Tick += TimerTick;
        }

        [DefaultValue(false)]
        public bool EditEnabled { get; set; }

        private void ApplyChanges(TreeNodeAdv node)
        {
            try
            {
                DoApplyChanges(node);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Value is not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void BeginEdit()
        {
            if (EditEnabled && Parent.CurrentNode != null && CanEdit(Parent.CurrentNode))
            {
                var args = new CancelEventArgs();
                OnEditorShowing(args);
                if (!args.Cancel)
                {
                    _discardChanges = false;
                    var control = CreateEditor(Parent.CurrentNode);
                    _editNode = Parent.CurrentNode;
                    control.Disposed += EditorDisposed;
                    Parent.DisplayEditor(control, this);
                }
            }
        }

        protected abstract Size CalculateEditorSize(EditorContext context);

        protected virtual bool CanEdit(TreeNodeAdv node)
        {
            return (node.Tag != null);
        }

        protected abstract Control CreateEditor(TreeNodeAdv node);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                _timer.Dispose();
        }

        protected abstract void DoApplyChanges(TreeNodeAdv node);

        private void EditorDisposed(object sender, EventArgs e)
        {
            OnEditorHided();
            if (!_discardChanges && _editNode != null)
                ApplyChanges(_editNode);
            _editNode = null;
        }

        public void EndEdit(bool cancel)
        {
            _discardChanges = cancel;
            Parent.HideEditor();
        }

        public override void MouseDoubleClick(TreeNodeAdvMouseEventArgs args)
        {
            _timer.Stop();
            _editFlag = false;
            if (Parent.UseColumns)
            {
                args.Handled = true;
                BeginEdit();
            }
        }

        public override void MouseDown(TreeNodeAdvMouseEventArgs args)
        {
            _editFlag = (!Parent.UseColumns && args.Button == MouseButtons.Left
                         && args.ModifierKeys == Keys.None && args.Node.IsSelected);
        }

        public override void MouseUp(TreeNodeAdvMouseEventArgs args)
        {
            if (_editFlag && args.Node.IsSelected)
                _timer.Start();
        }

        public void SetEditorBounds(EditorContext context)
        {
            var size = CalculateEditorSize(context);
            context.Editor.Bounds = new Rectangle(context.Bounds.X, context.Bounds.Y,
                Math.Min(size.Width, context.Bounds.Width), context.Bounds.Height);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            if (_editFlag)
                BeginEdit();
            _editFlag = false;
        }

        public virtual void UpdateEditor(Control control)
        {
        }

        #region Events

        protected void OnEditorHided()
        {
            if (EditorHided != null)
                EditorHided(this, EventArgs.Empty);
        }

        protected void OnEditorShowing(CancelEventArgs args)
        {
            if (EditorShowing != null)
                EditorShowing(this, args);
        }

        public event CancelEventHandler EditorShowing;

        public event EventHandler EditorHided;

        #endregion
    }
}