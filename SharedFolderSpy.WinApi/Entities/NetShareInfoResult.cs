namespace Fesslersoft.SharedFolderSpy.Native.Entities
{
    public sealed class NetShareInfoResult
    {
        public string NetName { get; set; }
        public int ShareType { get; set; }
        public string Remark { get; set; }
        public int Permissions { get; set; }
        public int MaxUsers { get; set; }
        public int CurrentUsers { get; set; }
        public string Path { get; set; }
        public string Password { get; set; }
    }
}
