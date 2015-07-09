#region

using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Collections;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Designers
{
    internal class RibbonButtonDesigner
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

        protected override void AddButton(object sender, System.EventArgs e)
        {
            base.AddButton(sender, e);
        }
    }
}