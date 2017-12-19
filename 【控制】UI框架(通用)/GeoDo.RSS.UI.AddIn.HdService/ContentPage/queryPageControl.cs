using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using GeoDo.RSS.UI.AddIn.HdService.HdDataServer;
using GeoDo.RSS.Core.UI;
using System.IO;
using System.Configuration;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public partial class queryPageControl : UserControl
    {
        private string _rootPath;
        HdDataProvider _hdDataProvider = null;
        RadListView lv = new RadListView();

        public queryPageControl()
        {
            InitializeComponent();
            InitHdDataService();
            LoadListView();
        }

        private void InitHdDataService()
        {
            _rootPath = ConfigurationManager.AppSettings["DataServerRootPath"];
            _hdDataProvider = new HdDataProvider();
            _hdDataProvider.getProjectionsCompleted += new EventHandler<getProjectionsCompletedEventArgs>(Instance_getProjectionsCompleted);
            _hdDataProvider.getBlocksCompleted += new EventHandler<getBlocksCompletedEventArgs>(_hdDataProvider_getBlocksCompleted);
            _hdDataProvider.getMosaicsCompleted += new EventHandler<getMosaicsCompletedEventArgs>(_hdDataProvider_getMosaicsCompleted);
        }

        private void LoadListView()
        {
            lv.ShowColumnHeaders = true;
            lv.ShowCheckBoxes = true;
            lv.Dock = DockStyle.Fill;
            lv.ViewType = ListViewType.DetailsView;
            lv.AllowColumnReorder = true;
            lv.EnableColumnSort = true;
            lv.AllowEdit = false;
            lv.MouseClick += new MouseEventHandler(lv_MouseClick);
            lv.ItemMouseDoubleClick += new ListViewItemEventHandler(lv_ItemMouseDoubleClick);
            this.panel5.Controls.Add(lv);
        }

        void lv_MouseClick(object sender, MouseEventArgs e)
        {
            //if (e.Button == System.Windows.Forms.MouseButtons.Right)
            //{lv.
            //    ListViewDataItem item = (ListViewDataItem)lv(e.Location);
            //    string file = item.Tag as string;
            //    ContextMenuStrip menu = ListViewMenu(file);
            //}
        }

        private ContextMenuStrip ListViewMenu(string file)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem item = new ToolStripMenuItem("查看缩略图");
            item.Click += new EventHandler(item_Click);
            item.Tag = file;
            return menu;
        }

        void item_Click(object sender, EventArgs e)
        {
        }
        
        private void ShowProjectionsInfo(ProjectionInfo[] pjs)
        {
            lv.Items.Clear();
            lv.Columns.Clear();
            ListViewDetailColumn col1 = new ListViewDetailColumn("文件名");
            col1.Width = 320;
            lv.Columns.Add(col1);
            lv.Columns.Add("创建时间");
            lv.Columns.Add("大小(MB)");
            lv.Columns.Add("快视图");
            lv.Columns.Add("轨道时间");
            lv.Columns.Add("白天/晚上");
            lv.Columns.Add("卫星");
            lv.Columns.Add("传感器");
            if (pjs == null || pjs.Length == 0)
            {
                SendMessage("查询到满足条件的结果共0条。");
                return;
            }
            else
                SendMessage("查询到满足条件的结果共" + pjs.Length + "条。");
            lv.BeginInit();
            for (int i = 0; i < pjs.Length; i++)
            {
                ProjectionInfo pg = pjs[i];
                OrbitInfo o = pg.orbitInfo;
                lv.Items.Add(
                    Path.GetFileName(pg.datapath),
                    pg.createTime,
                    (pg.fileSize / 1024.0 / 1024).ToString("f2"),
                    pg.thumbnail,
                    o == null ? "" : o.observationdate + o.observationtime,
                    o == null ? "" : o.dayOrNight,
                    o == null ? "" : o.satellite,
                    o == null ? "" : o.sensor);
                lv.Items[i].Tag = pg.datapath;
            }
            lv.EndInit();
        }

        private void ShowBlocksInfo(BlockInfo[] infos)
        {
            lv.Items.Clear();
            lv.Columns.Clear();
            ListViewDetailColumn col1 = new ListViewDetailColumn("文件名");
            col1.Width = 320;
            lv.Columns.Add(col1);
            lv.Columns.Add("创建时间");
            lv.Columns.Add("大小(MB)");
            lv.Columns.Add("快视图");
            lv.Columns.Add("轨道时间");
            lv.Columns.Add("白天/晚上");
            lv.Columns.Add("卫星");
            lv.Columns.Add("传感器");
            if (infos == null || infos.Length == 0)
            {
                SendMessage("查询到满足条件的结果共0条。");
                return;
            }
            else
                SendMessage("查询到满足条件的结果共" + infos.Length + "条。");
            for (int i = 0; i < infos.Length; i++)
            {
                BlockInfo info = infos[i];
                if (info == null) continue;
                MosaicInfo o = info.mosaicInfo == null ? null : info.mosaicInfo;
                lv.Items.Add(
                    Path.GetFileName(info.datapath),
                    info.createTime,
                    (info.fileSize / 1024.0 / 1024).ToString("f2"),
                    info.thumbnail,
                    o == null ? "" : o.observationdate,
                    o == null ? "" : o.dayOrNight,
                    o == null ? "" : o.satellite,
                    o == null ? "" : o.sensor);
                lv.Items[i].Tag = info.datapath;
            }
        }

        private void ShowMosaicsInfo(MosaicInfo[] infos)
        {
            lv.Items.Clear();
            lv.Columns.Clear();
            ListViewDetailColumn col1 = new ListViewDetailColumn("文件名");
            col1.Width = 320;
            lv.Columns.Add(col1);
            lv.Columns.Add("创建时间");
            lv.Columns.Add("大小(MB)");
            lv.Columns.Add("快视图");
            lv.Columns.Add("轨道时间");
            lv.Columns.Add("白天/晚上");
            lv.Columns.Add("卫星");
            lv.Columns.Add("传感器");
            if (infos == null || infos.Length == 0)
            {
                SendMessage("查询到满足条件的结果共0条。");
                return;
            }
            else
                SendMessage("查询到满足条件的结果共" + infos.Length + "条。");
            for (int i = 0; i < infos.Length; i++)
            {
                MosaicInfo info = infos[i];
                lv.Items.Add(
                    Path.GetFileName(info.datapath),
                    info.createTime,
                    (info.fileSize / 1024.0 / 1024).ToString("f2"),
                    info.thumbnail,
                    info.observationdate,
                    info.dayOrNight,
                    info.satellite,
                    info.sensor);
                lv.Items[i].Tag = info.datapath;
            }
        }

        void lv_ItemMouseDoubleClick(object sender, ListViewItemEventArgs e)
        {
            string datapath = e.Item.Tag as string;
            string file = _rootPath + datapath;
            OpenFile(file);
        }

        private void OpenFile(string filename)
        {
            if (!File.Exists(filename))
                MsgBox.ShowInfo("文件不存在或无法访问" + filename);
            else
                OpenFileFactory.Open(filename);
        }
        
        private bool _isSearching = false;

        private void SendMessage(string message)
        {
            lbMessage.Text = message;
        }

        /// <summary>
        /// BeginTime=2012-09-09 0:00:00;EndTime=2012-09-09 23:59:59;Satellite=FY3A;Sensor=VIRR;DataType=Projection
        /// </summary>
        /// <param name="argument"></param>
        internal void SetQueryArgument(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
                return;
            string[] args = argument.Split(';');
            Dictionary<string, string> argDic = new Dictionary<string, string>();
            foreach (string arg in args)
            {
                string[] argKv = arg.Split('=');
                argDic.Add(argKv[0], argKv[1]);
            }
            DateTime dtBeginTime;
            DateTime dtEndTime;
            DateTime.TryParse(argDic["BeginTime"], out dtBeginTime);
            DateTime.TryParse(argDic["EndTime"], out dtEndTime);
            string Satellite = argDic["Satellite"];
            string Sensor = argDic["Sensor"];
            string DataType = argDic["DataType"];

            if (_isSearching)
                return;
            _isSearching = true;

            switch (DataType)
            {
                case "Projection":
                    _hdDataProvider.getProjectionsAsync(dtBeginTime, dtEndTime, Satellite, Sensor);
                    break;
                case "Block":
                    _hdDataProvider.getBlocksAsync(dtBeginTime, dtEndTime, Satellite, Sensor);
                    break;
                case "Mosaic":
                    _hdDataProvider.getMosaicsAsync(dtBeginTime, dtEndTime, Satellite, Sensor);
                    break;
                default:
                    break;
            }
        }

        void Instance_getProjectionsCompleted(object sender, getProjectionsCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                    return;
                if (e.Error != null)
                {
                    lbMessage.Text = e.Error.Message;
                    return;
                }
                HdDataServer.ProjectionInfo[] pjInfos = e.Result;
                ShowProjectionsInfo(pjInfos);
                _isSearching = false;
            }
            catch (Exception ex)
            {
                SendMessage(ex.Message);
                return;
            }
            finally
            {
                _isSearching = false;
            }
        }

        void _hdDataProvider_getBlocksCompleted(object sender, getBlocksCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                    return;
                if (e.Error != null)
                {
                    lbMessage.Text = e.Error.Message;
                    return;
                }
                HdDataServer.BlockInfo[] pjInfos = e.Result;
                ShowBlocksInfo(pjInfos);
                _isSearching = false;
            }
            catch (Exception ex)
            {
                SendMessage(ex.Message);
                return;
            }
            finally
            {
                _isSearching = false;
            }
        }

        void _hdDataProvider_getMosaicsCompleted(object sender, getMosaicsCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                    return;
                if (e.Error != null)
                {
                    lbMessage.Text = e.Error.Message;
                    return;
                }
                HdDataServer.MosaicInfo[] pjInfos = e.Result;
                ShowMosaicsInfo(pjInfos);
                _isSearching = false;
            }
            catch (Exception ex)
            {
                SendMessage(ex.Message);
                return;
            }
            finally
            {
                _isSearching = false;
            }
        }
    }
}
