using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using SharedFolderSpy.WinApi;

namespace SharedFolderSpy.UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var data = Management.RetrieveLocalShares();
            var xxx = SharedFolderSpy.WinApi.WinApi.Check();

            fileSystemWatcher1.Path = @"\\P724-MF\AMD-Catalyst-14-9-win7-win8.1-64Bit-dd-ccc-whql";
            //var shares = Win32Share.GetAllShares();

            //foreach (var win32Share in shares)
            //{
            //    var x = win32Share.AccessMask;
            //}

            //List<ConnectionShare> connections = ConnectionShare.BuildConnectionShareList();
            //var abc = new ListBox();
        }

        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            bool isTempFile = e.Name.StartsWith("~$") || e.Name.EndsWith(".tmp");
            if (!isTempFile)
            {
                var ipadress = new FileInfo(e.FullPath).GetAccessControl().GetOwner(typeof(NTAccount));
            }
        }

        private void fileSystemWatcher1_Created(object sender, FileSystemEventArgs e)
        {
            bool isTempFile = e.Name.StartsWith("~$") || e.Name.EndsWith(".tmp");
            IdentityReference ipadress = null;
            if (!isTempFile)
            {
                ipadress =new FileInfo(e.FullPath).GetAccessControl().GetOwner(typeof(NTAccount));
            }
            var x = ipadress;
        }
    }


}
