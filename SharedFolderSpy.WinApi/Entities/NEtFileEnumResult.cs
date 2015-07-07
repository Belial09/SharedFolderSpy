using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SharedFolderSpy.WinApi.Entities
{
    public class NEtFileEnumResult
    {
        public Int32 Id { get; set; }
        public Int32 Index { get; set; }
        public Int32 Permission { get; set; }
        public Int32 NumberOfLocks { get; set; }
        public String Username { get; set; }
        public String Pathname { get; set; }
    }
}
