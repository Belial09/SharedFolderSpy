namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview
{
    public class TreeViewAdvCancelEventArgs : TreeViewAdvEventArgs
    {
        public TreeViewAdvCancelEventArgs(TreeNodeAdv node)
            : base(node)
        {
        }

        public bool Cancel { get; set; }
    }
}