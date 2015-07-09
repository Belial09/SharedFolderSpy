#region

using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Collections;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Designers
{
    internal class RibbonPanelDesigner
        : RibbonElementWithItemCollectionDesigner
    {
        public override Ribbon Ribbon
        {
            get
            {
                if (Component is RibbonPanel)
                {
                    return (Component as RibbonPanel).Owner;
                }
                return null;
            }
        }

        public override RibbonItemCollection Collection
        {
            get
            {
                if (Component is RibbonPanel)
                {
                    return (Component as RibbonPanel).Items;
                }
                return null;
            }
        }
    }
}