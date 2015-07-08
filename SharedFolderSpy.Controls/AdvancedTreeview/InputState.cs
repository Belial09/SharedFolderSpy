#region

using System.Windows.Forms;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview
{
    internal abstract class InputState
    {
        private readonly TreeViewAdv _tree;

        public InputState(TreeViewAdv tree)
        {
            _tree = tree;
        }

        public TreeViewAdv Tree
        {
            get { return _tree; }
        }

        public abstract void KeyDown(KeyEventArgs args);
        public abstract void MouseDown(TreeNodeAdvMouseEventArgs args);

        /// <summary>
        ///     handle OnMouseMove event
        /// </summary>
        /// <param name="args"></param>
        /// <returns>true if event was handled and should be dispatched</returns>
        public virtual bool MouseMove(MouseEventArgs args)
        {
            return false;
        }

        public abstract void MouseUp(TreeNodeAdvMouseEventArgs args);
    }
}