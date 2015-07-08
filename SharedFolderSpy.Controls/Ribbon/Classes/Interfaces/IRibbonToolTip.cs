#region

using System.Drawing;
using System.Windows.Forms;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Interfaces
{
    public interface IRibbonToolTip
    {
        /// <summary>
        ///     Gets or Sets the ToolTip Text
        /// </summary>
        string ToolTip { get; set; }

        /// <summary>
        ///     Gets or Sets the ToolTip Title
        /// </summary>
        string ToolTipTitle { get; set; }

        /// <summary>
        ///     Gets or Sets the ToolTip Image
        /// </summary>
        Image ToolTipImage { get; set; }

        /// <summary>
        ///     Gets or Sets the stock ToolTip Icon
        /// </summary>
        ToolTipIcon ToolTipIcon { get; set; }

        /// <summary>
        ///     Occurs before a ToolTip is initially displayed.
        ///     <remarks>Use this event to change the ToolTip or Cancel it at all.</remarks>
        /// </summary>
        event RibbonElementPopupEventHandler ToolTipPopUp;
    }
}