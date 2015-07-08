using System;

namespace Fesslersoft.SharedFolderSpy.Native.Entities
{
    public sealed class NEtFileEnumResult
    {
        public Int32 Id { get; set; }
        public Int32 Permission { get; set; }
        public Int32 NumberOfLocks { get; set; }
        public String Username { get; set; }
        public String Pathname { get; set; }
    }
}
