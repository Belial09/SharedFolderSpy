using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SharedFolderSpy.Data.Entities
{
    public abstract class BaseItem
    {
        private string _path = "";
        public string ItemPath
        {
            get { return _path; }
            set { _path = value; }
        }

        private Image _icon;
        public Image Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

        private string _size = "";
        public string Size
        {
            get { return _size; }
            set { _size = value; }
        }

        private string _date = "";
        public string Date
        {
            get { return _date; }
            set { _date = value; }
        }

        public abstract string Name
        {
            get;
            set;
        }

        private BaseItem _parent;
        public BaseItem Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
    }
}
