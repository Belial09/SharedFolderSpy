#region

using System.Drawing;
using System.Windows.Forms;
using Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.Interfaces;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.Ribbon.Classes.EventArgs
{
    public delegate void RibbonElementPopupEventHandler(object sender, RibbonElementPopupEventArgs e);

    public class RibbonElementPopupEventArgs : PopupEventArgs
    {
        private readonly PopupEventArgs _args;
        private readonly IRibbonElement _ribbonElement;

        public RibbonElementPopupEventArgs(IRibbonElement item)
            : base(item.Owner, item.Owner, false, new Size(-1, -1))
        {
            _ribbonElement = item;
        }

        public RibbonElementPopupEventArgs(IRibbonElement item, PopupEventArgs args)
            : base(args.AssociatedWindow, args.AssociatedControl, args.IsBalloon, args.ToolTipSize)
        {
            _ribbonElement = item;
            _args = args;
        }

        public IRibbonElement AssociatedRibbonElement
        {
            get { return _ribbonElement; }
        }


        public new bool Cancel
        {
            get { return _args == null ? base.Cancel : _args.Cancel; }
            set
            {
                if (_args != null)
                    _args.Cancel = value;
                base.Cancel = value;
            }
        }
    }
}