#region

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fesslersoft.SharedFolderSpy.Native.Entities;

#endregion

namespace Fesslersoft.SharedFolderSpy.Native
{
        public class Win32
        {
            public enum NetApiStatus : uint
            {
                NERR_Success = 0,
                NERR_InvalidComputer = 2351,
                NERR_NotPrimary = 2226,
                NERR_SpeGroupOp = 2234,
                NERR_LastAdmin = 2452,
                NERR_BadPassword = 2203,
                NERR_PasswordTooShort = 2245,
                NERR_UserNotFound = 2221,
                ERROR_ACCESS_DENIED = 5,
                ERROR_NOT_ENOUGH_MEMORY = 8,
                ERROR_INVALID_PARAMETER = 87,
                ERROR_INVALID_NAME = 123,
                ERROR_INVALID_LEVEL = 124,
                ERROR_MORE_DATA = 234,
                ERROR_SESSION_CREDENTIAL_CONFLICT = 1219,
                RPC_S_SERVER_UNAVAILABLE = 2147944122,
                RPC_E_REMOTE_DISABLED = 2147549468 
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
                        list.Add(
                            new NEtFileEnumResult
                            {
                                Id = fileInfo3.SessionID,
                                NumberOfLocks = fileInfo3.NumLocks,
                                Pathname = fileInfo3.PathName,
                                Permission = fileInfo3.Permission,
                                Username = fileInfo3.UserName
                            }
                            );
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
                var pBuffer = IntPtr.Zero;
                var status = NetSessionEnum(server, null, null, 502, out pBuffer, -1, out entriesRead, out totalEntries, ref resumeHandle);

                if (status == 0 & entriesRead > 0)
                {
                    var shareinfoType = typeof (SessionInfo502);
                    var offset = Marshal.SizeOf(shareinfoType);

                    for (int i = 0, item = pBuffer.ToInt32(); i < entriesRead; i++, item += offset)
                    {
                        var pItem = new IntPtr(item);

                        var sessionInfo502 = (SessionInfo502) Marshal.PtrToStructure(pItem, shareinfoType);
                        list.Add(
                            new NetSessionEnumResult
                            {
                                ClientType = sessionInfo502.ClientType,
                                ComputerName = sessionInfo502.ComputerName,
                                NumOpens = sessionInfo502.NumOpens,
                                SecondsActive = sessionInfo502.SecondsActive,
                                SecondsIdle = sessionInfo502.SecondsIdle,
                                Transport = sessionInfo502.Transport,
                                UserFlags = sessionInfo502.UserFlags,
                                UserName = sessionInfo502.UserName
                            }
                            );
                    }
                }
                NetApiBufferFree(pBuffer);
                return list;
            }

            public static List<NetShareInfoResult> BuildNetShareEnumList(string server)
            {
                //Note that this code will only work if run as administrator!!
                var list = new List<NetShareInfoResult>();

                int entriesRead;
                int totalEntries;
                var resumeHandle = 0;
                var pBuffer = IntPtr.Zero;
                var status = NetShareEnum(server, 2, out pBuffer, -1, out entriesRead, out totalEntries, ref resumeHandle);

                if (status == 0 & entriesRead > 0)
                {
                    var shareinfoType = typeof (ShareInfo2);
                    var offset = Marshal.SizeOf(shareinfoType);

                    for (int i = 0, item = pBuffer.ToInt32(); i < entriesRead; i++, item += offset)
                    {
                        var pItem = new IntPtr(item);

                        var shareInfo2 = (ShareInfo2) Marshal.PtrToStructure(pItem, shareinfoType);
                        list.Add(
                            new NetShareInfoResult
                            {
                                CurrentUsers = shareInfo2.CurrentUsers,
                                MaxUsers = shareInfo2.MaxUsers,
                                NetName = shareInfo2.NetName,
                                Password = shareInfo2.Password,
                                Path = shareInfo2.Path,
                                Permissions = shareInfo2.Permissions,
                                Remark = shareInfo2.Remark,
                                ShareType = shareInfo2.ShareType
                            }
                            );
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