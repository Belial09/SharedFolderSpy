using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedFolderSpy.Data.Entities
{
    class UserItem : BaseItem
    {
        #region Overrides of BaseItem
        private string _name = "";
        public override string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion
    }
}
