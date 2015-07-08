using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Text;

namespace SharedFolderSpy.WinApi
{

    namespace ph03n1x
    {
        public class NetAPI32
        {
            #region StructLayout
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct FileInfo3
            {
                public int SessionID;
                public int Permission;
                public int NumLocks;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string PathName;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string UserName;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct SessionInfo502
            {
                [MarshalAs(UnmanagedType.LPWStr)]
                public string ComputerName;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string UserName;
                public uint NumOpens;
                public uint SecondsActive;
                public uint SecondsIdle;
                public uint UserFlags;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string ClientType;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string Transport;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct ShareInfo2
            {
                [MarshalAs(UnmanagedType.LPWStr)]
                public string NetName;
                public int ShareType;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string Remark;
                public int Permissions;
                public int MaxUsers;
                public int CurrentUsers;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string Path;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string Password;
            }

            #endregion StructLayout

            #region DllImports

            [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            static extern int NetFileClose(
                string servername,
                int id);

            #region NetFileEnum
            [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            private static extern int NetFileEnum(
                string serverName,
                string basePath,
                string userName,
                int level,
                ref IntPtr buffer,
                int prefMaxLength,
                out int entriesRead,
                out int totalEntries,
                ref int resumeHandle
                );
            #endregion NetFileEnum

            #region NetShareEnum
            [DllImport("netapi32", CharSet = CharSet.Unicode)]
            protected static extern int NetShareEnum(
                string serverName,
                int level,
                out IntPtr buffer,
                int prefMaxLength,
                out int entriesRead,
                out int totalEntries,
                ref int resumeHandle
                );
            #endregion NetShareEnum

            #region NetSessionDel
            [DllImport("NetApi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            internal static extern uint NetSessionDel(
                 string serverName,
                 string uncClientName,
                 string userName);
            //eg. NetSessionDel(null, null, "USERNAMETODELETE")
            #endregion NetSessionDel

            #region NetShareDel
            [DllImport("netapi32.dll", SetLastError = true)]
            static extern uint NetShareDel(
                [MarshalAs(UnmanagedType.LPWStr)] 
            string serverName,
                [MarshalAs(UnmanagedType.LPWStr)] 
            string shareName,
                Int32 reserved //must be 0
                        );
            #endregion NetShareDel

            #region NetSessionEnum
            [DllImport("netapi32.dll", SetLastError = true)]
            public static extern int NetSessionEnum
                (
                [In, MarshalAs(UnmanagedType.LPWStr)]
            string serverName,
                [In, MarshalAs(UnmanagedType.LPWStr)]
            string uncClientName,
                [In, MarshalAs(UnmanagedType.LPWStr)]
            string userName,
                Int32 level,
                out IntPtr buffer,
                int prefmaxlen,
                out int entriesRead,
                out int totalEntries,
                ref int resumeHandle
                );
            #endregion NetSessionEnum

            #region NetApiBufferFree
            [DllImport("Netapi32.dll", SetLastError = true)]
            public static extern int NetApiBufferFree(IntPtr Buffer);
            #endregion NetApiBufferFree

            #endregion DllImports

            #region Methods
            public static List<SessionInfo502> BuildNetSessionEnumList(string server)
            {
                List<SessionInfo502> list = new List<SessionInfo502>();

                int entriesRead;
                int totalEntries;
                int resumeHandle = 0;
                IntPtr pBuffer = IntPtr.Zero;
                int status = NetSessionEnum(server, null, null, 502, out pBuffer, -1, out entriesRead, out totalEntries, ref resumeHandle);

                if (status == 0 & entriesRead > 0)
                {
                    Type shareinfoType = typeof(SessionInfo502);
                    int offset = Marshal.SizeOf(shareinfoType);

                    for (int i = 0, item = pBuffer.ToInt32(); i < entriesRead; i++, item += offset)
                    {
                        IntPtr pItem = new IntPtr(item);

                        SessionInfo502 sessionInfo502 = (SessionInfo502)Marshal.PtrToStructure(pItem, shareinfoType);
                        list.Add(sessionInfo502);
                    }
                }
                NetApiBufferFree(pBuffer);
                return list;
            }

            public static List<FileInfo3> BuildNetFileEnumList(string server)
            {
                List<FileInfo3> list = new List<FileInfo3>();

                int entriesRead;
                int totalEntries;
                int resumeHandle = 0;
                IntPtr pBuffer = IntPtr.Zero;
                int status = NetFileEnum(server, null, null, 3, ref pBuffer, -1, out entriesRead, out totalEntries, ref resumeHandle);

                if (status == 0 & entriesRead > 0)
                {
                    Type shareinfoType = typeof(FileInfo3);
                    int offset = Marshal.SizeOf(shareinfoType);

                    for (int i = 0, item = pBuffer.ToInt32(); i < entriesRead; i++, item += offset)
                    {
                        IntPtr pItem = new IntPtr(item);

                        FileInfo3 fileInfo3 = (FileInfo3)Marshal.PtrToStructure(pItem, shareinfoType);
                        list.Add(fileInfo3);
                    }
                    NetApiBufferFree(pBuffer);
                }
                return list;
            }

            public static bool RemoveFileAccess(string servername, int id)
            {
                return (WinApi.NET_API_STATUS)NetFileClose(servername, id) == WinApi.NET_API_STATUS.NERR_Success;
            }

            public static bool RemoveSession(string server, string client, string user)
            {
                return (WinApi.NET_API_STATUS)NetSessionDel(server, client, user) == WinApi.NET_API_STATUS.NERR_Success;
            }

            public static List<ShareInfo2> BuildNetShareEnumList(string server)
            {
                //Note that this code will only work if run as administrator!!
                List<ShareInfo2> list = new List<ShareInfo2>();

                int entriesRead;
                int totalEntries;
                int resumeHandle = 0;
                IntPtr pBuffer = IntPtr.Zero;
                int status = NetShareEnum(server, 2, out pBuffer, -1, out entriesRead, out totalEntries, ref resumeHandle);

                if (status == 0 & entriesRead > 0)
                {
                    Type shareinfoType = typeof(ShareInfo2);
                    int offset = Marshal.SizeOf(shareinfoType);

                    for (int i = 0, item = pBuffer.ToInt32(); i < entriesRead; i++, item += offset)
                    {
                        IntPtr pItem = new IntPtr(item);

                        ShareInfo2 shareInfo2 = (ShareInfo2)Marshal.PtrToStructure(pItem, shareinfoType);
                        list.Add(shareInfo2);
                    }
                }
                NetApiBufferFree(pBuffer);

                return list;
            }

            #endregion Methods
        }
    }
}
