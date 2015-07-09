#region

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AutoMapper;
using Fesslersoft.SharedFolderSpy.Native.Entities;

#endregion

namespace Fesslersoft.SharedFolderSpy.Native
{
        public class Win32
        {
            public enum NetApiStatus : uint
            {
                NerrSuccess = 0,
                NerrInvalidComputer = 2351,
                NerrNotPrimary = 2226,
                NerrSpeGroupOp = 2234,
                NerrLastAdmin = 2452,
                NerrBadPassword = 2203,
                NerrPasswordTooShort = 2245,
                NerrUserNotFound = 2221,
                ErrorAccessDenied = 5,
                ErrorNotEnoughMemory = 8,
                ErrorInvalidParameter = 87,
                ErrorInvalidName = 123,
                ErrorInvalidLevel = 124,
                ErrorMoreData = 234,
                ErrorSessionCredentialConflict = 1219,
                RpcSServerUnavailable = 2147944122,
                RpcERemoteDisabled = 2147549468 
            }

            #region StructLayout

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct FileInfo3
            {
                public int SessionID;
                public int Permission;
                public int NumLocks;
                [MarshalAs(UnmanagedType.LPWStr)] public string PathName;
                [MarshalAs(UnmanagedType.LPWStr)] public string UserName;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct SessionInfo502
            {
                [MarshalAs(UnmanagedType.LPWStr)] public string ComputerName;
                [MarshalAs(UnmanagedType.LPWStr)] public string UserName;
                public uint NumOpens;
                public uint SecondsActive;
                public uint SecondsIdle;
                public uint UserFlags;
                [MarshalAs(UnmanagedType.LPWStr)] public string ClientType;
                [MarshalAs(UnmanagedType.LPWStr)] public string Transport;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct ShareInfo2
            {
                [MarshalAs(UnmanagedType.LPWStr)] public string NetName;
                public int ShareType;
                [MarshalAs(UnmanagedType.LPWStr)] public string Remark;
                public int Permissions;
                public int MaxUsers;
                public int CurrentUsers;
                [MarshalAs(UnmanagedType.LPWStr)] public string Path;
                [MarshalAs(UnmanagedType.LPWStr)] public string Password;
            }

            #endregion StructLayout

            #region DllImports

            [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            private static extern int NetFileClose(
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
            private static extern uint NetShareDel(
                [MarshalAs(UnmanagedType.LPWStr)] string serverName,
                [MarshalAs(UnmanagedType.LPWStr)] string shareName,
                Int32 reserved //must be 0
                );

            #endregion NetShareDel

            #region NetSessionEnum

            [DllImport("netapi32.dll", SetLastError = true)]
            public static extern int NetSessionEnum
                (
                [In, MarshalAs(UnmanagedType.LPWStr)] string serverName,
                [In, MarshalAs(UnmanagedType.LPWStr)] string uncClientName,
                [In, MarshalAs(UnmanagedType.LPWStr)] string userName,
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


            public static List<NEtFileEnumResult> BuildNetFileEnumList(string server)
            {
                var list = new List<NEtFileEnumResult>();
                int entriesRead;
                int totalEntries;
                var resumeHandle = 0;
                var pBuffer = IntPtr.Zero;
                var status = NetFileEnum(server, null, null, 3, ref pBuffer, -1, out entriesRead, out totalEntries, ref resumeHandle);
                if (status == 0 & entriesRead > 0)
                {
                    var shareinfoType = typeof (FileInfo3);
                    var offset = Marshal.SizeOf(shareinfoType);
                    for (int i = 0, item = pBuffer.ToInt32(); i < entriesRead; i++, item += offset)
                    {
                        var pItem = new IntPtr(item);
                        var fileInfo3 = (FileInfo3) Marshal.PtrToStructure(pItem, shareinfoType);
                        var nEtFileEnumResult = Mapper.Map<FileInfo3, NEtFileEnumResult>(fileInfo3);
                        list.Add(nEtFileEnumResult);
                    }
                    NetApiBufferFree(pBuffer);
                }
                return list;
            }

            public static List<NetSessionEnumResult> BuildNetSessionEnumList(string server)
            {
                var list = new List<NetSessionEnumResult>();
                int entriesRead;
                int totalEntries;
                var resumeHandle = 0;
                IntPtr pBuffer;
                var status = NetSessionEnum(server, null, null, 502, out pBuffer, -1, out entriesRead, out totalEntries, ref resumeHandle);
                if (status == 0 & entriesRead > 0)
                {
                    var shareinfoType = typeof (SessionInfo502);
                    var offset = Marshal.SizeOf(shareinfoType);
                    for (int i = 0, item = pBuffer.ToInt32(); i < entriesRead; i++, item += offset)
                    {
                        var pItem = new IntPtr(item);
                        var sessionInfo502 = (SessionInfo502) Marshal.PtrToStructure(pItem, shareinfoType);
                        var netSessionEnumResult = Mapper.Map<SessionInfo502, NetSessionEnumResult>(sessionInfo502);
                        list.Add(netSessionEnumResult);
                    }
                }
                NetApiBufferFree(pBuffer);
                return list;
            }

            public static List<NetShareInfoResult> BuildNetShareEnumList(string server)
            {
                var list = new List<NetShareInfoResult>();
                int entriesRead;
                int totalEntries;
                var resumeHandle = 0;
                IntPtr pBuffer;
                var status = NetShareEnum(server, 2, out pBuffer, -1, out entriesRead, out totalEntries, ref resumeHandle);
                if (status == 0 & entriesRead > 0)
                {
                    var shareinfoType = typeof (ShareInfo2);
                    var offset = Marshal.SizeOf(shareinfoType);
                    for (int i = 0, item = pBuffer.ToInt32(); i < entriesRead; i++, item += offset)
                    {
                        var pItem = new IntPtr(item);
                        var shareInfo2 = (ShareInfo2) Marshal.PtrToStructure(pItem, shareinfoType);
                        var netShareInfoResult = Mapper.Map<ShareInfo2, NetShareInfoResult>(shareInfo2);
                        list.Add(netShareInfoResult);
                    }
                }
                NetApiBufferFree(pBuffer);
                return list;
            }

            public static NetApiStatus RemoveFileAccess(string servername, int id)
            {
                return (NetApiStatus) NetFileClose(servername, id);
            }

            public static NetApiStatus RemoveSession(string server, string client, string user)
            {
                return (NetApiStatus) NetSessionDel(server, client, user);
            }

            #endregion Methods
        }
    }