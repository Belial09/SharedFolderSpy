using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Fesslersoft.SharedFolderSpy.Shared
{
    public static class Extensions
    {
        public static Icon ToIcon(this Image img, bool makeTransparent)
        {
            var bmp = (Bitmap)img;
            if (makeTransparent)
            {
                bmp.MakeTransparent(Color.White);
            }
            var icH = bmp.GetHicon();
            return Icon.FromHandle(icH);
        }
    }
}
