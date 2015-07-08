#region

using System.Drawing;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Collections;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Interfaces
{
    public interface IDropDownRibbonItem
    {
        RibbonItemCollection DropDownItems { get; }

        Rectangle DropDownButtonBounds { get; }

        bool DropDownButtonVisible { get; }

        bool DropDownButtonSelected { get; }

        bool DropDownButtonPressed { get; }
    }
}