using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace SharedFolderSpy.WinApi
{
    public class Management
    {
        public static List<String> RetrieveLocalShares()
        {
            var retVal = new List<String>();
            using (var shares = new ManagementClass("Win32_Share"))
            {
                retVal.AddRange(from ManagementObject share in shares.GetInstances() select share["Name"].ToString());
            }
            return retVal;
        }
    }
}
