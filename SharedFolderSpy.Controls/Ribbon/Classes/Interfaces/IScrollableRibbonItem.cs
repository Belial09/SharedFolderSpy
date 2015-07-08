#region

using System.Drawing;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Interfaces
{
    /// <summary>
    ///     Implemented by Ribbon items that has scrollable content
    /// </summary>
    public interface IScrollableRibbonItem
    {
        /// <summary>
        ///     Gets the bounds of the content (without scrolling controls)
        /// </summary>
        Rectangle ContentBounds { get; }

        /// <summary>
        ///     Scrolls the content down
        /// </summary>
        void ScrollDown();

        /// <summary>
        ///     Scrolls the content up
        /// </summary>
        void ScrollUp();
    }
}