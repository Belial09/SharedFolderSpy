#region

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.AdvancedTreeview
{
    public static class TextHelper
    {
        public static StringAlignment TranslateAligment(HorizontalAlignment aligment)
        {
            if (aligment == HorizontalAlignment.Left)
                return StringAlignment.Near;
            if (aligment == HorizontalAlignment.Right)
                return StringAlignment.Far;
            return StringAlignment.Center;
        }
    }
}