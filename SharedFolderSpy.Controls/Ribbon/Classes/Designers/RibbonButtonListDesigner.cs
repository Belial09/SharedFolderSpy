#region

using System.ComponentModel;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Collections;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Designers
{
    internal class RibbonButtonListDesigner
        : RibbonElementWithItemCollectionDesigner
    {
        public override Ribbon Ribbon
        {
            get
            {
                if (Component is RibbonButtonList)
                {
                    return (Component as RibbonButtonList).Owner;
                }
                return null;
            }
        }

        public override RibbonItemCollection Collection
        {
            get
            {
                if (Component is RibbonButtonList)
                {
                    return (Component as RibbonButtonList).Buttons;
                }
                return null;
            }
        }
    }
}