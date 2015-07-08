#region

using System.Collections.Generic;
using System.ComponentModel;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Interfaces
{
    /// <summary>
    ///     Used to extract all child components from RibbonItem objects
    /// </summary>
    public interface IContainsRibbonComponents
    {
        IEnumerable<Component> GetAllChildComponents();
    }
}