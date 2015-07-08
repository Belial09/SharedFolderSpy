using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SharedFolderSpy.WinApi.Entities;

namespace SharedFolderSpy.WinApi
{
    public class WinApi
    {
        public enum NET_API_STATUS : uint
        {
            NERR_Success = 0,
            /// <summary>
            /// This computer name is invalid.
            /// </summary>
            NERR_InvalidComputer = 2351,
            /// <summary>
            /// This operation is only allowed on the primary domain controller of the domain.
            /// </summary>
            NERR_NotPrimary = 2226,
            /// <summary>
            /// This operation is not allowed on this special group.
            /// </summary>
            NERR_SpeGroupOp = 2234,
            /// <summary>
            /// This operation is not allowed on the last administrative account.
            /// </summary>
            NERR_LastAdmin = 2452,
            /// <summary>
            /// The password parameter is invalid.
            /// </summary>
            NERR_BadPassword = 2203,
            /// <summary>
            /// The password does not meet the password policy requirements. 
            /// Check the minimum password length, password complexity and password history requirements.
            /// </summary>
            NERR_PasswordTooShort = 2245,
            /// <summary>
            /// The user name could not be found.
            /// </summary>
            NERR_UserNotFound = 2221,
            ERROR_ACCESS_DENIED = 5,
            ERROR_NOT_ENOUGH_MEMORY = 8,
            ERROR_INVALID_PARAMETER = 87,
            ERROR_INVALID_NAME = 123,
            ERROR_INVALID_LEVEL = 124,
            ERROR_MORE_DATA = 234,
            ERROR_SESSION_CREDENTIAL_CONFLICT = 1219,
            /// <summary>
            /// The RPC server is not available. This error is returned if a remote computer was specified in
            /// the lpServer parameter and the RPC server is not available.
            /// </summary>
            RPC_S_SERVER_UNAVAILABLE = 2147944122, // 0x800706BA
            /// <summary>
            /// Remote calls are not allowed for this process. This error is returned if a remote computer was 
            /// specified in the lpServer parameter and remote calls are not allowed for this process.
            /// </summary>
            RPC_E_REMOTE_DISABLED = 2147549468 // 0x8001011C
        }
        public enum NETFILEPERMISSIONS
        {
            PERM_FILE_NONE = 0,
            PERM_FILE_READ = 1,
            PERM_FILE_WRITE = 2,
            PERM_FILE_CREATE = 4,
            PERM_FILE_EXECUTE = 8
        }

        [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern int NetFileEnum(
             string servername,
             string basepath,
             string username,
             int level,
             ref IntPtr bufptr,
             int prefmaxlen,
             out int entriesread,
             out int totalentries,
             IntPtr resume_handle
        );

        [DllImport("Netapi32.dll", SetLastError = true)]
        static extern int NetApiBufferFree(IntPtr Buffer);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct FILE_INFO_3
        {
            public int fi3_id;
            public int fi3_permission;
            public int fi3_num_locks;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string fi3_pathname;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string fi3_username;
        }
        const int MAX_PREFERRED_LENGTH = -1;

        public static List<NEtFileEnumResult> Check()
        {
            var returnValue = new List<NEtFileEnumResult>();
            int dwReadEntries;
            int dwTotalEntries;
            IntPtr pBuffer = IntPtr.Zero;
            var pCurrent = new FILE_INFO_3();

            int dwStatus = NetFileEnum("localhost", null, null, 3, ref pBuffer, MAX_PREFERRED_LENGTH, out dwReadEntries, out dwTotalEntries, IntPtr.Zero);

            if ((NET_API_STATUS)dwStatus == NET_API_STATUS.NERR_Success)
            {
                for (var dwIndex = 0; dwIndex < dwReadEntries; dwIndex++)
                {
                    var iPtr = new IntPtr(pBuffer.ToInt32() + (dwIndex * Marshal.SizeOf(pCurrent)));
                    pCurrent = (FILE_INFO_3)Marshal.PtrToStructure(iPtr, typeof(FILE_INFO_3));
                    var nEtFileEnumResult = new NEtFileEnumResult
                    {
                        Index = dwIndex, 
                        Id = pCurrent.fi3_id, 
                        NumberOfLocks = pCurrent.fi3_num_locks, 
                        Pathname = pCurrent.fi3_pathname, 
                        Permission = pCurrent.fi3_permission, 
                        Username = pCurrent.fi3_username
                    };
                    returnValue.Add(nEtFileEnumResult);
                }
                NetApiBufferFree(pBuffer);
            }
            return returnValue;
        }
    }
}