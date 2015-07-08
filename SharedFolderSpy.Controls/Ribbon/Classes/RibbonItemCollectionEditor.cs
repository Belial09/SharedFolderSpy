#region

using System;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Collections;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes
{
    public class RibbonItemCollectionEditor
        : CollectionEditor
    {
        public RibbonItemCollectionEditor()
            : base(typeof (RibbonItemCollection))
        {
        }

        protected override Type CreateCollectionItemType()
        {
            return typeof (RibbonButton);
        }

        protected override Type[] CreateNewItemTypes()
        {
            return new[]
            {
                typeof (RibbonButton),
                typeof (RibbonButtonList),
                typeof (RibbonItemGroup),
                typeof (RibbonComboBox),
                typeof (RibbonSeparator),
                typeof (RibbonTextBox),
                typeof (RibbonColorChooser),
                typeof (RibbonCheckBox),
                typeof (RibbonUpDown),
                typeof (RibbonLabel),
                typeof (RibbonHost)
            };
        }
    }
}