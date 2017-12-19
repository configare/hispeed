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
        //private string _rootPath;
        private string _rootPathProjetion;
        private string _rootPathMosaic;
        private string _rootPathBlock;
        private string _rootStatDat;

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
            _rootPathProjetion = ConfigurationManager.AppSettings["DataServerProjetionPath"];
            _rootPathMosaic = ConfigurationManager.AppSettings["DataServerMosaicPath"];
            _rootPathBlock = ConfigurationManager.AppSettings["DataServerBlockPath"];
            _rootStatDat = ConfigurationManager.AppSettings["DataServerStatPath"];

            _hdDataProvider = new HdDataProvider();
            _hdDataProvider.getProjectionsCompleted += new EventHandler<getProjectionsCompletedEventArgs>(Instance_getProjectionsCompleted);
            _hdDataProvider.getBlocksCompleted += new EventHandler<getBlocksCompletedEventArgs>(_hdDataProvider_getBlocksCompleted);
            _hdDataProvider.getMosaicsCompleted += new EventHandler<getMosaicsCompletedEventArgs>(_hdDataProvider_getMosaicsCompleted);
            _hdDataProvider.getRasterDatsCompleted += new EventHandler<getRasterDatsCompletedEventArgs>(_hdDataProvider_getRasterDatsCompleted);
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

        private void lv_MouseClick(object sender, MouseEventArgs e)
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

        private void item_Click(object sender, EventArgs e)
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
                lv.Items[i].Tag = _rootPathProjetion + pg.datapath;
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
                lv.Items[i].Tag = _rootPathBlock + info.datapath;
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
                lv.Items[i].Tag = _rootPathMosaic + info.datapath;
            }
        }

        private void lv_ItemMouseDoubleClick(object sender, ListViewItemEventArgs e)
        {
            string datapath = e.Item.Tag as string;
            string file = datapath;
            OpenFile(file);
        }

        private void OpenFile(string filename)
        {
            if (!System.IO.File.Exists(filename))
                MsgBox.ShowInfo("文件不存在或无法访问" + filename);
            else
                OpenFileFactory.Open(filename);
        }
        
        private bool _isSearching = false;

        /// <summary>
        /// BeginTime=2012-09-09 0:00:00;EndTime=2012-09-09 23:59:59;Satellite=FY3A;Sensor=VIRR;DataType=Projection
        /// DataType=RasterData;
        /// </summary>
        /// <param name="argument"></param>
        internal void SetQueryArgument(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
                return;
            if (_isSearching)
                return;
            try
            {
                _isSearching = true;
                string[] args = argument.Split(';');
                Dictionary<string, string> argDic = new Dictionary<string, string>();
                foreach (string arg in args)
                {
                    string[] argKv = arg.Split('=');
                    argDic.Add(argKv[0], argKv[1]);
                }
                string DataType = argDic["DataType"];
                switch (DataType)
                {
                    case "Projection":
                        SearchProjection(argDic);
                        break;
                    case "Block":
                        SearchBlock(argDic);
                        break;
                    case "Mosaic":
                        SearchMosaic(argDic);
                        break;
                    case "RasterData":
                        SearchRasterData(argDic);
                        break;
                    default:
                        break;
                }
            }
            finally
            {
                SendMessage("已启动查询，正在等待服务器响应[" + argument + "]");
            }
        }

        #region 批量统计数据查询

        private void SearchRasterData(Dictionary<string, string> argDic)
        {
            string satellite = argDic["satellite"];
            string sensor = argDic["sensor"];
            string resolution = argDic["resolution"];
            string productName = argDic["productName"];
            string countPeroid = argDic["countPeroid"];
            string countIdentify = argDic["countIdentify"];
            string beginDate = argDic["beginDate"];
            string endDate = argDic["endDate"];
            _hdDataProvider.getRasterDatsAsync(satellite, sensor, resolution, productName, countPeroid, countIdentify, beginDate, endDate);
        }

        private void _hdDataProvider_getRasterDatsCompleted(object sender, getRasterDatsCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                    return;
                if (e.Error != null)
                {
                    SendMessage(e.Error.Message);
                    return;
                }
                HdDataServer.ArrayOfString result = e.Result;
                ShowStatDatData(result);
                _isSearching = false;
                UpdateDBMessage("从数据库查询到" + result.Count + "条记录");
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

        private void ShowStatDatData(HdDataServer.ArrayOfString files)
        {
            lv.Items.Clear();
            lv.Columns.Clear();
            ListViewDetailColumn col1 = new ListViewDetailColumn("文件名");
            col1.Width = 320;
            lv.Columns.Add(col1);
            if (files == null || files.Count == 0)
            {
                SendMessage("查询到满足条件的结果共0条。");
                return;
            }
            else
                SendMessage("查询到满足条件的结果共" + files.Count + "条。");
            for (int i = 0; i < files.Count; i++)
            {
                string info = files[i];
                string filename = Path.GetFileName(info);
                lv.Items.Add(filename, filename);
                lv.Items[i].Tag = _rootStatDat + info;
            }
        }
        #endregion 

        private void SearchMosaic(Dictionary<string, string> argDic)
        {
            DateTime dtBeginTime;
            DateTime dtEndTime;
            DateTime.TryParse(argDic["BeginTime"], out dtBeginTime);
            DateTime.TryParse(argDic["EndTime"], out dtEndTime);
            string Satellite = argDic["Satellite"];
            string Sensor = argDic["Sensor"];
            _hdDataProvider.getMosaicsAsync(dtBeginTime, dtEndTime, Satellite, Sensor);
        }

        private void SearchBlock(Dictionary<string, string> argDic)
        {
            DateTime dtBeginTime;
            DateTime dtEndTime;
            DateTime.TryParse(argDic["BeginTime"], out dtBeginTime);
            DateTime.TryParse(argDic["EndTime"], out dtEndTime);
            string Satellite = argDic["Satellite"];
            string Sensor = argDic["Sensor"];
            _hdDataProvider.getBlocksAsync(dtBeginTime, dtEndTime, Satellite, Sensor);
        }

        private void SearchProjection(Dictionary<string, string> argDic)
        {
            DateTime dtBeginTime;
            DateTime dtEndTime;
            DateTime.TryParse(argDic["BeginTime"], out dtBeginTime);
            DateTime.TryParse(argDic["EndTime"], out dtEndTime);
            string Satellite = argDic["Satellite"];
            string Sensor = argDic["Sensor"];
            _hdDataProvider.getProjectionsAsync(dtBeginTime, dtEndTime, Satellite, Sensor);
        }

        private void Instance_getProjectionsCompleted(object sender, getProjectionsCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                    return;
                if (e.Error != null)
                {
                    SendMessage(e.Error.Message);
                    return;
                }
                HdDataServer.ProjectionInfo[] result = e.Result;
                ShowProjectionsInfo(result);
                _isSearching = false;
                UpdateDBMessage("从数据库查询到" + result.Length + "条记录");
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

        private void _hdDataProvider_getBlocksCompleted(object sender, getBlocksCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                    return;
                if (e.Error != null)
                {
                    SendMessage(e.Error.Message);
                    return;
                }
                HdDataServer.BlockInfo[] result = e.Result;
                ShowBlocksInfo(result);
                _isSearching = false;
                UpdateDBMessage("从数据库查询到" + result.Length + "条记录");
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

        private void _hdDataProvider_getMosaicsCompleted(object sender, getMosaicsCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                    return;
                if (e.Error != null)
                {
                    SendMessage(e.Error.Message);
                    return;
                }
                HdDataServer.MosaicInfo[] result = e.Result;
                ShowMosaicsInfo(result);
                _isSearching = false;
                UpdateDBMessage("从数据库查询到" + result.Length + "条记录");
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

        private void UpdateDBMessage(string msg)
        {
            SendMessage("最新一次访问数据库时间" + DateTime.Now.ToLongTimeString() + msg);
        }

        private void SendMessage(string obj)
        {
            try
            {
                if (obj != null && obj.Length != 0)
                {
                    int ind = obj.IndexOf("\r\n");
                    if (ind > 0)
                        obj = obj.Substring(0, ind);
                }
                if (this.InvokeRequired)
                    this.Invoke(new Action<string>((val) => { this.lbMessage.Text = val; }), obj);
                else
                    this.lbMessage.Text = obj;
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine("queryPageControl:OnMessageSend:" + ex.Message);
            }
        }
    }
}
