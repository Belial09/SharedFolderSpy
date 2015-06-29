using System;
using System.Runtime.InteropServices;
namespace Interop.NetAPI32
{
    public class WinApi
    {
        [DllImport("Netapi32.dll", SetLastError = true)]
        static extern int NetApiBufferFree(IntPtr Buffer);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        struct FILE_INFO_3
        {
            public int fi3_id;
            public int fi3_permission;
            public int fi3_num_locks;
            public string fi3_pathname;
            public string fi3_username;
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

        [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern int NetFileGetInfo(
          string servername,
          int fileid,
          int level,
          ref IntPtr bufptr
        );

        private int GetFileIdFromPath(string filePath)
        {
            const int MAX_PREFERRED_LENGTH = -1;

            int dwReadEntries;
            int dwTotalEntries;
            IntPtr pBuffer = IntPtr.Zero;
            FILE_INFO_3 pCurrent = new FILE_INFO_3();

            int dwStatus = NetFileEnum(null, filePath, null, 3, ref pBuffer, MAX_PREFERRED_LENGTH, out dwReadEntries, out dwTotalEntries, IntPtr.Zero);

            if (dwStatus == 0)
            {
                for (int dwIndex = 0; dwIndex < dwReadEntries; dwIndex++)
                {

                    IntPtr iPtr = new IntPtr(pBuffer.ToInt32() + (dwIndex * Marshal.SizeOf(pCurrent)));
                    pCurrent = (FILE_INFO_3)Marshal.PtrToStructure(iPtr, typeof(FILE_INFO_3));

                    int fileId = pCurrent.fi3_id;

                    //because of the path filter in the NetFileEnum function call, the first (and hopefully only) entry should be the correct one
                    NetApiBufferFree(pBuffer);
                    return fileId;
                }
            }

            NetApiBufferFree(pBuffer);
            return -1;  //should probably do something else here like throw an error
        }


        private string GetUsernameHandlingFile(int fileId)
        {
            string defaultValue = "[Unknown User]";

            if (fileId == -1)
            {
                return defaultValue;
            }

            IntPtr pBuffer_Info = IntPtr.Zero;
            int dwStatus_Info = NetFileGetInfo(null, fileId, 3, ref pBuffer_Info);

            if (dwStatus_Info == 0)
            {
                IntPtr iPtr_Info = new IntPtr(pBuffer_Info.ToInt32());
                FILE_INFO_3 pCurrent_Info = (FILE_INFO_3)Marshal.PtrToStructure(iPtr_Info, typeof(FILE_INFO_3));
                NetApiBufferFree(pBuffer_Info);
                return pCurrent_Info.fi3_username;
            }

            NetApiBufferFree(pBuffer_Info);
            return defaultValue;  //default if not successfull above
        }

        private string GetUsernameHandlingFile(string filePath)
        {
            int fileId = GetFileIdFromPath(filePath);
            return GetUsernameHandlingFile(fileId);
        }
    }
}