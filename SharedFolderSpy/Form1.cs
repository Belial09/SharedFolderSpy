using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Fesslersoft.SharedFolderSpy.Native;
using Fesslersoft.SharedFolderSpy.Native.Entities;
using Fesslersoft.SharedFolderSpy.Shared;

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
            var bgWorker = new BackgroundWorker();
            bgWorker.DoWork += BgWorkerOnDoWork;
            bgWorker.RunWorkerAsync();

            var bgWorkerTaskbarIcon = new BackgroundWorker();
            bgWorkerTaskbarIcon.DoWork += BgWorkerTaskbarIconOnDoWork;
            bgWorkerTaskbarIcon.RunWorkerAsync();
        }

        private void BgWorkerTaskbarIconOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            try
            {
                var icons = new Icon[20];
                int Offset = 0;
                while (Offset <= 19)
                {
                    icons[Offset] = imageList1.Images[Offset].ToIcon(false);
                    Offset++;
                }

                Offset = 0;
                while (true)
                {
                    BeginInvoke((Action)delegate
                    {
                        notifyIcon1.Visible = true;
                        notifyIcon1.Icon = icons[Offset];
                        Offset++;
                        if (Offset > 19) Offset = 0;
                    });
                    Thread.Sleep(50);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void BgWorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            while (true)
            {
                var shares = Win32.BuildNetShareEnumList(null);
                BeginInvoke((Action)(() => listBox1.Items.Clear()));
                foreach (var share in shares)
                {
                    NetShareInfoResult share1 = share;
                    BeginInvoke((Action)(() => listBox1.Items.Add(String.Format("NAME:={0};PATH:={1};REMARK:={2};SHARETYPE:={3};CURRUSERCOUNT:={4};MAXUSERS:={5}", share1.NetName,  share1.Path, share1.Remark,  share1.ShareType, share1.CurrentUsers, share1.MaxUsers))));
                }

                var fileconnections = Win32.BuildNetFileEnumList(null);
                BeginInvoke((Action)(() => listBox2.Items.Clear()));
                foreach (var fileconnection in fileconnections)
                {
                    NEtFileEnumResult fileconnection1 = fileconnection;
                    BeginInvoke((Action)(() => listBox2.Items.Add(String.Format("SHAREID:={0};USER:={1};PERMISSION:={2};PATH:={3}", fileconnection1.Id, fileconnection1.Username, fileconnection1.Permission, fileconnection1.Pathname))));
                }

                var connectionsInfo = Win32.BuildNetSessionEnumList(null);
                BeginInvoke((Action)(() => listBox3.Items.Clear()));
                foreach (var connectionInfo in connectionsInfo)
                {
                    NetSessionEnumResult info = connectionInfo;
                    BeginInvoke((Action)(() =>listBox3.Items.Add(String.Format("USERNAME:={0};IP:={1};TIME:={2};IDLETIME:={4};FILEOPENCOUNT:={3}", info.UserName, info.ComputerName, info.SecondsActive, info.NumOpens, info.SecondsIdle))));
                }
                System.Threading.Thread.Sleep(500);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Maps.Init();
        }
    }


}
