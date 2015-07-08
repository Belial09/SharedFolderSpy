using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;
using Fesslersoft.SharedFolderSpy.Native;

namespace Fesslersoft.SharedFolderSpy.UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            while (true)
            {
                Application.DoEvents();

                var shares = Win32.BuildNetShareEnumList(null);
                listBox1.Items.Clear();
                foreach (var share in shares)
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
                var fileconnections = Win32.BuildNetFileEnumList(null);
                foreach (var fileconnection in fileconnections)
                {
                    
                    int remoteUserPrimition = fileconnection.Permission;
                    string remoteUsername = fileconnection.Username;
                    string sharePath = fileconnection.Pathname;
                    int shareID = fileconnection.Id;
                    

                    if (sharePath == @"R:\Downloads\Entpackt\MOVIES\SERIEN\Person of Interest" && remoteUsername == "mf")
                    {
                        Win32.RemoveFileAccess(null, shareID);
                    }

                    if (sharePath.Contains(".mkv"))
                    {
                        Win32.RemoveFileAccess(null, shareID);
                        
                        //NetAPI32.RemoveSession(null, null, "MF");
                    }

                    listBox2.Items.Add(String.Format("SHAREID:={0};USER:={1};PERMISSION:={2};PATH:={3}", shareID, remoteUsername, remoteUserPrimition, sharePath));
                }

                listBox3.Items.Clear();
                var connectionsInfo = Win32.BuildNetSessionEnumList(null);
                foreach (var connectionInfo in connectionsInfo)
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
