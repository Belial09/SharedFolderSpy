using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Fesslersoft.SharedFolderSpy.Native;
using Fesslersoft.SharedFolderSpy.Native.Entities;
using Fesslersoft.SharedFolderSpy.Shared;
using Peter;

namespace Fesslersoft.SharedFolderSpy.UI
{
    public partial class Form1 : Form
    {
        private readonly ImageList _dirImageList = new ImageList();

        public Form1()
        {
            InitializeComponent();
            _dirImageList.ImageSize = new System.Drawing.Size(32, 32);
            _dirImageList.ColorDepth = ColorDepth.Depth32Bit;
            _dirImageList.Images.Add(Native.IconExtractor.GetFolderIcon(IconExtractor.IconSize.Large, IconExtractor.FolderType.Open).ToBitmap());
            listView2.LargeImageList = _dirImageList;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

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
                var icons = new Icon[16];
                int Offset = 0;
                while (Offset <= (icons.Length-1))
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
                        if (Offset > (icons.Length - 1)) Offset = 0;
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
            var shares = Win32.BuildNetShareEnumList(null);
            BeginInvoke((Action)(() => listView2.Items.Clear()));
            
            foreach (var share in shares.Where(x => String.IsNullOrEmpty(x.Remark)))
            {
                BeginInvoke((Action) (() => listView2.Items.Add( share.NetName,0  )));
            }
            comboBox1.Items.AddRange(shares.Where(x=>x.Remark==String.Empty).Select(x=>x.NetName).ToArray());

            //while (true)
            //{
                

            //    var fileconnections = Win32.BuildNetFileEnumList(null);
            //    BeginInvoke((Action)(() => listBox2.Items.Clear()));
            //    foreach (var fileconnection in fileconnections)
            //    {
            //        NEtFileEnumResult fileconnection1 = fileconnection;
            //        BeginInvoke((Action)(() => listBox2.Items.Add(String.Format("SHAREID:={0};USER:={1};PERMISSION:={2};PATH:={3}", fileconnection1.Id, fileconnection1.Username, fileconnection1.Permission, fileconnection1.Pathname))));
            //    }

            //    var connectionsInfo = Win32.BuildNetSessionEnumList(null);
            //    BeginInvoke((Action)(() => listBox3.Items.Clear()));
            //    foreach (var connectionInfo in connectionsInfo)
            //    {
            //        NetSessionEnumResult info = connectionInfo;
            //        BeginInvoke((Action)(() =>listBox3.Items.Add(String.Format("USERNAME:={0};IP:={1};TIME:={2};IDLETIME:={4};FILEOPENCOUNT:={3}", info.UserName, info.ComputerName, info.SecondsActive, info.NumOpens, info.SecondsIdle))));
            //    }
            //    System.Threading.Thread.Sleep(500);
            //}
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Maps.Init();

            
        }

        private void listView2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView2.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                        ListViewItem item = listView2.GetItemAt(e.X, e.Y);
                        if (item != null)
                        {
                            item.Selected = true;
                        }

                }
            }
        }

        private void listView2_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView2.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    ShellContextMenu ctxMnu = new ShellContextMenu();
                    ctxMnu.ShowContextMenu(new DirectoryInfo[] { (new DirectoryInfo(@"C:\Windows")) }, this.PointToScreen(new Point(e.X, e.Y)));
                }
            }
        }
    }


}
