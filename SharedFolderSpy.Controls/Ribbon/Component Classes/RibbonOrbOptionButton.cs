#region

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon
{
    public class RibbonOrbOptionButton
        : RibbonButton
    {
        #region Ctors

        public RibbonOrbOptionButton()
        {
        }

        public RibbonOrbOptionButton(string text)
            : this()
        {
            Text = text;
        }

        #endregion

        #region Props

        public override Image Image
        {
            get { return base.Image; }
            set
            {
                base.Image = value;

                SmallImage = value;
            }
        }

        [Browsable(false)]
        public override Image SmallImage
        {
            get { return base.SmallImage; }
            set { base.SmallImage = value; }
        }

        #endregion
    }
}