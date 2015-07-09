#region

using System.ComponentModel.Design;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Collections;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Designers
{
    internal class RibbonOrbMenuItemDesigner
        : RibbonElementWithItemCollectionDesigner
    {
        public override Ribbon Ribbon
        {
            get
            {
                if (Component is RibbonButton)
                {
                    return (Component as RibbonButton).Owner;
                }
                return null;
            }
        }

        public override RibbonItemCollection Collection
        {
            get
            {
                if (Component is RibbonButton)
                {
                    return (Component as RibbonButton).DropDownItems;
                }
                return null;
            }
        }

        protected override DesignerVerbCollection OnGetVerbs()
        {
            return new DesignerVerbCollection(new[]
            {
                new DesignerVerb("Add DescriptionMenuItem", AddDescriptionMenuItem),
                new DesignerVerb("Add Separator", AddSeparator)
            });
        }
    }
}