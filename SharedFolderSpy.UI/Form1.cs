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
using SharedFolderSpy.WinApi.ph03n1x;

namespace SharedFolderSpy.UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


            





            



















            //var data = Management.RetrieveLocalShares();
            //var xxx = SharedFolderSpy.WinApi.WinApi.Check();

            //fileSystemWatcher1.Path = @"\\P724-MF\AMD-Catalyst-14-9-win7-win8.1-64Bit-dd-ccc-whql";
            ////var shares = Win32Share.GetAllShares();

            ////foreach (var win32Share in shares)
            ////{
            ////    var x = win32Share.AccessMask;
            ////}

            ////List<ConnectionShare> connections = ConnectionShare.BuildConnectionShareList();
            ////var abc = new ListBox();
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

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            while (true)
            {
                Application.DoEvents();

                List<NetAPI32.ShareInfo2> shares = NetAPI32.BuildNetShareEnumList(null);
                listBox1.Items.Clear();
                foreach (NetAPI32.ShareInfo2 share in shares)
                {
                    int maxAllowedUsers = share.MaxUsers;
                    int shareType = share.ShareType;
                    string shareName = share.NetName;
                    string sharePath = share.Path;
                    string shareRemark = share.Remark;
                    int currentConectedUsers = share.CurrentUsers;

                    listBox1.Items.Add(String.Format("NAME:={0};PATH:={1};REMARK:={2};SHARETYPE:={3};CURRUSERCOUNT:={4};MAXUSERS:={5}", shareName, sharePath, shareRemark, shareType, currentConectedUsers, maxAllowedUsers));
                }

                listBox2.Items.Clear();
                List<NetAPI32.FileInfo3> fileconnections = NetAPI32.BuildNetFileEnumList(null);
                foreach (NetAPI32.FileInfo3 fileconnection in fileconnections)
                {
                    
                    int remoteUserPrimition = fileconnection.Permission;
                    string remoteUsername = fileconnection.UserName;
                    string sharePath = fileconnection.PathName;
                    int shareID = fileconnection.SessionID;

                    if (sharePath == @"R:\Downloads\Entpackt\MOVIES\SERIEN\Person of Interest" && remoteUsername == "mf")
                    {
                        NetAPI32.RemoveFileAccess(null, shareID);
                    }

                    if (sharePath.Contains(".mkv"))
                    {
                        NetAPI32.RemoveFileAccess(null, shareID);
                        
                        //NetAPI32.RemoveSession(null, null, "MF");
                    }

                    listBox2.Items.Add(String.Format("SHAREID:={0};USER:={1};PERMISSION:={2};PATH:={3}", shareID, remoteUsername, remoteUserPrimition, sharePath));
                }

                listBox3.Items.Clear();
                List<NetAPI32.SessionInfo502> connectionsInfo = NetAPI32.BuildNetSessionEnumList(null);
                foreach (NetAPI32.SessionInfo502 connectionInfo in connectionsInfo)
                {
                    string remoteUsername = connectionInfo.UserName;
                    string remoteIP = connectionInfo.ComputerName;
                    int conectionTime = Convert.ToInt32(connectionInfo.SecondsActive);
                    int fileOpenCount = Convert.ToInt32(connectionInfo.NumOpens);
                    listBox3.Items.Add(String.Format("USERNAME:={0};IP:={1};TIME:={2};IDLETIME:={4};FILEOPENCOUNT:={3}", remoteUsername, remoteIP, conectionTime, fileOpenCount, connectionInfo.SecondsIdle));
                }



                System.Threading.Thread.Sleep(500);
            }
        }
    }


}
