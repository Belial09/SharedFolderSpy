namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs
{
    public class RibbonItemEventArgs : System.EventArgs
    {
        public RibbonItemEventArgs(RibbonItem item)
        {
            Item = item;
        }

        public RibbonItem Item { get; set; }
    }
}