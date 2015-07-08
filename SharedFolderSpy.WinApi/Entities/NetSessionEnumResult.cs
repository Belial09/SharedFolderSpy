namespace Fesslersoft.SharedFolderSpy.Native.Entities
{
    public sealed class NetSessionEnumResult
    {
        public string ComputerName { get; set; }
        public string UserName { get; set; }
        public uint NumOpens { get; set; }
        public uint SecondsActive { get; set; }
        public uint SecondsIdle { get; set; }
        public uint UserFlags { get; set; }
        public string ClientType { get; set; }
        public string Transport { get; set; }
    }
}