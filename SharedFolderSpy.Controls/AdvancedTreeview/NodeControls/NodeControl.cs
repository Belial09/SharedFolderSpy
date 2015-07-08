#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview.NodeControls
{
    [DesignTimeVisible(false), ToolboxItem(false)]
    public abstract class NodeControl : Component
    {
        #region Properties

        private int _column;
        private TreeViewAdv _parent;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TreeViewAdv Parent
        {
            get { return _parent; }
            set
            {
                if (value != _parent)
                {
                    if (_parent != null)
                        _parent.NodeControls.Remove(this);

                    if (value != null)
                        value.NodeControls.Add(this);
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IToolTipProvider ToolTipProvider { get; set; }

        [DefaultValue(0)]
        public int Column
        {
            get { return _column; }
            set
            {
                if (_column < 0)
                    throw new ArgumentOutOfRangeException("value");

                _column = value;
                if (_parent != null)
                    _parent.FullUpdate();
            }
        }

        #endregion

        internal void AssignParent(TreeViewAdv parent)
        {
            _parent = parent;
        }

        public abstract void Draw(TreeNodeAdv node, DrawContext context);

        public virtual string GetToolTip(TreeNodeAdv node)
        {
            if (ToolTipProvider != null)
                return ToolTipProvider.GetToolTip(node);
            return string.Empty;
        }

        public virtual void KeyDown(KeyEventArgs args)
        {
        }

        public virtual void KeyUp(KeyEventArgs args)
        {
        }

        public abstract Size MeasureSize(TreeNodeAdv node);

        public virtual void MouseDoubleClick(TreeNodeAdvMouseEventArgs args)
        {
        }

        public virtual void MouseDown(TreeNodeAdvMouseEventArgs args)
        {
        }

        public virtual void MouseUp(TreeNodeAdvMouseEventArgs args)
        {
        }
    }
}