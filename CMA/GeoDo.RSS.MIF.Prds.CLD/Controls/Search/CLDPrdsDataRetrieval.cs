using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeoDo.Tools;
using GeoDo.RSS.Core.DF;
using System.Drawing.Imaging;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public partial class CLDPrdsDataRetrieval : Form
    {
        private static string _dataBaseXml = ConnectMySqlCloud._dataBaseXml;
        private Dictionary<CheckBox, long> cbx2PrdID = new Dictionary<CheckBox, long>();
        ConnectMySqlCloud _con;
        private static UIControls _ui = UIControls.GetInstance();   //界面控件集合
        private static DBConnect _db=null ;   //数据库实例
        public delegate void DoubleClickRowHdrEventHandler(object source,DoubleClickRowHdrEventArgs args);
        public event DoubleClickRowHdrEventHandler _openEventhandler;
        DataReader dr = null;

        public CLDPrdsDataRetrieval()
        {
            InitInterface();
            _ui.SetUIControls(groupBoxDataSource, groupBoxProducts, groupBoxDataSet, groupBoxPrdsPeriod, groupBoxDayNight, groupBoxPrdsTime);
        }

        private void InitInterface()
        {
            InitializeComponent();
            _con = new ConnectMySqlCloud();
            ucRadioBoxListProducts.CheckedChanged += new EventHandler(ucRadioBoxList1_CheckedChanged);
        }

        private void CLDDataRetrieval_Load(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            radibtnMOD06.Checked = true;
            radibtnPrdDay.Checked = true;
            radibtnContinueTime.Checked = true;
        }

        #region 条件选择状态响应事件
        void ucRadioBoxList1_CheckedChanged(object sender, EventArgs e)
        {
            long[] ids;
            string[] setsNames;
            long radiID = ucRadioBoxListProducts.Checked;
            try
            {
                if (radiID != -1)
                {
                    setsNames = QueryPrds2dataset(radiID, out ids);
                    this.ucCheckBoxListDataSet.ResetContent(ids, setsNames);
                }
                else
                    this.ucCheckBoxListDataSet.ResetContent(null, null);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        #region 数据源与产品的切换
        /// <summary>
        /// 数据源选项响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadiBtnDataSource_click(object sender, EventArgs e)
        {
            radibtnPrdDay.Enabled = true;
            radibtnPrdMonth.Enabled = true;
            groupBoxDayNight.Enabled = true;
            //radibtnPrdTen.Enabled = false;
            radibtnPrdYear.Enabled = true;
            try
            {
                this.ucCheckBoxListDataSet.ResetContent(null, null);
                if (radibtnMOD06.Checked)
                    ChangeProducts("MODIS");
                else if (radibtnAIRS.Checked)
                    ChangeProducts("AIRS");
                else
                {
                    groupBoxDayNight.Enabled = false;
                    radibtnPrdDay.Enabled = false;
                    radibtnPrdMonth.Checked = true;
                    radibtnPrdYear.Enabled = false;
                    ChangeProducts("ISCCP");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ChangeProducts(string sensor)
        {
            long[] ids;
            string[] setsNames;
            if (_con.QueryPrdsInfo(sensor, out setsNames, out ids))
                this.ucRadioBoxListProducts.ResetContent(ids, setsNames);
        }
        #endregion

        /// <summary>
        /// 周期类型选项响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadiBtnPrdsPeriod_click(object sender, EventArgs e)
        {
            if (radibtnPrdYear.Checked)
                radibtnContinueTime.Checked = true;
            if (radibtnContinueTime.Checked)
            {
                dateTimePickerBegin.Format = DateTimePickerFormat.Custom;
                dateTimePickerBegin.CustomFormat = "yyyy-MM-dd";
                dateTimePickerEnd.CustomFormat = "yyyy-MM-dd";
                panelPrdsTimeMonth.Visible = false;
                panelPrdsTimeTen.Visible = false;
                if (radibtnPrdDay.Checked)
                {
                    dataGridViewPeriod.Visible = false;
                    dataGridViewDay.Visible = true;
                }
                else
                {
                    dataGridViewDay.Visible = false;
                    dataGridViewPeriod.Visible = true;
                    if (radibtnPrdYear.Checked)
                    {
                        dateTimePickerBegin.Format = DateTimePickerFormat.Custom;
                        dateTimePickerBegin.CustomFormat = "yyyy";
                        dateTimePickerEnd.CustomFormat = "yyyy";
                    }
                    else if (radibtnPrdMonth.Checked)
                    {
                        dateTimePickerBegin.Format = DateTimePickerFormat.Custom;
                        dateTimePickerBegin.CustomFormat = "yyyy-MM";
                        dateTimePickerEnd.CustomFormat = "yyyy-MM";
                    }
                }
            }
            else
            {
                if (radibtnPrdDay.Checked)
                {
                    panelPrdsTimeMonth.Visible = false;
                    panelPrdsTimeTen.Visible = false;
                    dataGridViewPeriod.Visible = false;
                    dataGridViewDay.Visible = true;
                    dateTimePickerBegin.Format = DateTimePickerFormat.Custom;
                    dateTimePickerBegin.CustomFormat = "yyyy-MM-dd";
                    dateTimePickerEnd.CustomFormat = "yyyy-MM-dd";
                }
                else
                {
                    dateTimePickerBegin.Format = DateTimePickerFormat.Custom;
                    dateTimePickerBegin.CustomFormat = "yyyy";
                    dateTimePickerEnd.CustomFormat = "yyyy";
                    if (radibtnPrdYear.Checked)
                    {
                        panelPrdsTimeMonth.Visible = false;
                        panelPrdsTimeTen.Visible = false;
                        radibtnSameTime.Enabled = false;
                    }
                    else if (radibtnPrdTen.Checked)
                    {
                        panelPrdsTimeMonth.Visible = true;
                        panelPrdsTimeTen.Visible = true;
                    }
                    else if (radibtnPrdMonth.Checked)
                    {
                        panelPrdsTimeMonth.Visible = true;
                        panelPrdsTimeTen.Visible = false;
                    }
                }
            }
        }

        /// <summary>
        /// 连续与同期选项响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadiBtnPrdsTime_click(object sender, EventArgs e)
        {
            dateTimePickerBegin.Format = DateTimePickerFormat.Custom;
            dateTimePickerBegin.CustomFormat = "yyyy-MM-dd";
            dateTimePickerEnd.CustomFormat = "yyyy-MM-dd";
            radibtnSameTime.Enabled = true;
            if (radibtnSameTime.Checked)
            {
                radibtnPrdDay.Enabled = false;
                //radibtnPrdTen.Enabled = true;
                radibtnPrdYear.Enabled = false;
                if (!radibtnPrdDay.Checked)
                {
                    dateTimePickerBegin.Format = DateTimePickerFormat.Custom;
                    dateTimePickerBegin.CustomFormat = "yyyy";
                    dateTimePickerEnd.CustomFormat = "yyyy";
                    if (radibtnPrdYear.Checked)
                    {
                        panelPrdsTimeMonth.Visible = false;
                        panelPrdsTimeTen.Visible = false;
                        radibtnSameTime.Enabled = false;
                    }
                    else if (radibtnPrdTen.Checked)
                    {
                        panelPrdsTimeMonth.Visible = true;
                        panelPrdsTimeTen.Visible = true;
                    }
                    else if (radibtnPrdMonth.Checked)
                    {
                        panelPrdsTimeMonth.Visible = true;
                        panelPrdsTimeTen.Visible = false;
                    }
                }
            }
            else
            {
                radibtnPrdDay.Enabled = true;
                radibtnPrdTen.Enabled = false;
                radibtnPrdYear.Enabled = true;
                panelPrdsTimeMonth.Visible = false;
                panelPrdsTimeTen.Visible = false;
            }
            if (radibtnISCCPD2.Checked)
            {
                radibtnPrdDay.Enabled = false;
                radibtnPrdMonth.Checked = true;
                radibtnPrdYear.Enabled = false;
            }
        }

        #endregion
        #region 按钮事件
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_db == null)
                return;
            if (tabControl1.SelectedIndex==0)
            {
                if (dr == null)
                    return;
                //DataReader dr = new DataReader(_ui, _db, dataGridViewDay, dataGridViewPeriod);
                dr.GetSelectedFilesFromGrid(_dataBaseXml);
            } 
            else
            {
                if (dr == null)
                    return;
                //DataReader dr = new DataReader(_ui, _db, dataGridViewCldSAT);
                dr.GetSelectedFilesFromGrid(_dataBaseXml,true);
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CheckArgsIsOK())
                    return;
                if (_db == null)
                    _db = DBConnect.GetInstance();//创建数据库连接
                ConcatSQL cSql = new ConcatSQL();
                cSql.ConcatSQLByUIControls(_ui);//根据界面条件生成相应的sql语句；
                dr = new DataReader(_ui, _db, dataGridViewDay, dataGridViewPeriod);//创建查询结果的识别语句，并把结果输出到gridview
                _db.OpenConnection();
                _db.Retrieval(cSql.Sql, dr.DataReaderImplementation);//执行查询
                _db.CloseConnection();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAddQueryCondtions_Click(object sender, EventArgs e)
        {

        }
        #region 双击gridview的行头时，尝试打开文件
        private void dataGridViewPeriod_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                int row = e.RowIndex;
                string fullname = GetFileFullName(dataGridViewPeriod, row);
                if (_openEventhandler != null)
                    _openEventhandler(this, new DoubleClickRowHdrEventArgs(fullname));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridViewDay_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                int row = e.RowIndex;
                string fullname = GetFileFullName(dataGridViewDay, row);
                if (_openEventhandler != null)
                    _openEventhandler(this, new DoubleClickRowHdrEventArgs(fullname));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string GetFileFullName(DataGridView dataGridView, int row)
        {
            string name = "文件路径";
            int column = 0;
            foreach (DataGridViewColumn cln in dataGridView.Columns)
            {
                if (cln.HeaderText == name)
                {
                    column = cln.DisplayIndex;
                    break;
                }
            }
            string localfname = dataGridView.Rows[row].Cells[column].Value.ToString();
            if (!string.IsNullOrEmpty(localfname))
            {
                string dir = GetRootPathFromName(localfname);
                return Path.Combine(dir, localfname.TrimStart('\\'));
            }
            return null;
        }
        #endregion

        public static string GetRootPathFromName(string localfname)
        {
            DataBaseArg dbargs = DataBaseArg.ParseXml(_dataBaseXml);
            //string dir = dbargs.OutputDir;
            string dir = null;
            if (Path.GetDirectoryName(localfname).ToUpper().Contains("MODIS"))
                dir = dbargs.OutputDir;
            else if (Path.GetDirectoryName(localfname).ToUpper().Contains("AIRS"))
                dir = dbargs.AIRSRootPath;
            else if (Path.GetDirectoryName(localfname).ToUpper().Contains("ISCCP"))
                dir = dbargs.ISCCPRootPath;
            else if (Path.GetDirectoryName(localfname).ToUpper().Contains("CLOUDSAT"))
                dir = dbargs.CloudSATRootPath;
            else
                dir = dbargs.OutputDir;
            return dir;
        }

        #region 双击文件路径单元格时显示完整路径
        private void dataGridViewPeriod_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int column = e.ColumnIndex;
            int row = e.RowIndex;
            FullFileNameCol(dataGridViewPeriod, column, row);
            dataGridViewPeriod.UpdateCellValue(column, row);
        }

        private void dataGridViewDay_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int column = e.ColumnIndex;
            int row = e.RowIndex;
            FullFileNameCol(dataGridViewDay, column, row);
            dataGridViewDay.UpdateCellValue(column, row);
        }

        private void FullFileNameCol(DataGridView dataGridView, int column, int row)
        {
            string name = "文件路径";
            if (dataGridView.Columns[column].HeaderText == name)
            {
                string localfname = dataGridView.Rows[row].Cells[column].Value.ToString();
                if (!string.IsNullOrEmpty(localfname))
                {
                    string dir = GetRootPathFromName(localfname);
                    dataGridView.Rows[row].Cells[column].Value = Path.Combine(dir, localfname.TrimStart('\\'));
                }
            }
        }
        #endregion

        #endregion

        private bool CheckArgsIsOK()
        {
            if (!_ui.CheckValidProductDataSource())
            {
                throw new ArgumentException("请选择产品数据源");
            }
            else if (!_ui.CheckValidProductType())
            {
                throw new ArgumentException("请选择产品类型");
            }
            else if (!_ui.CheckValidDataSet())
            {
                throw new ArgumentException("请选择数据集");
            }
            else if (!_ui.CheckValidProductPeriod())
            {
                throw new ArgumentException("请选择产品周期");
            }
            else if (!_ui.CheckValidDayNight())
            {
                throw new ArgumentException("请选择昼夜");
            }
            else if (!_ui.CheckValidProductTime())
            {
                return false;
            }
            return true;
        }

        private string[] QueryPrds2dataset(long prdsID, out long[] setIds)
        {
            string [] setNames;
            setIds = null;
            if (radibtnMOD06.Checked)
            {
                if (_con.QueryPrdsID2Datasets("MODIS",prdsID, out setNames, out setIds))
                    return setNames;
            }
            else if (radibtnAIRS.Checked)
            {
                if (_con.QueryPrdsID2Datasets("AIRS",prdsID, out setNames, out setIds))
                    return setNames;
            }
            else if(radibtnISCCPD2.Checked)
            {
                if (_con.QueryPrdsID2Datasets("ISCCPD2",prdsID, out setNames, out setIds))
                    return setNames;
            }
            setIds = null;
            return new string[]{};
        }

        #region  cloudsat查询
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex==1)
            {
                btnCloudSAToverview.Visible = true;
                radibtnCloudSAT.Checked = true;
            }
            else
            {
                btnCloudSAToverview.Visible = false;
                radibtnCloudSAT.Checked = false;
            }
        }

        private void radibtnCloudSAT_CheckedChanged(object sender, EventArgs e)
        {
            if (radibtnCloudSAT.Checked)
            {
                long[] ids;
                string[] setsNames;
                string sensor = "CloudSAT";
                if (_con.QueryCloudSATPrdsInfo(sensor, out setsNames, out ids))
                    this.ucRadioBoxListCLDSAT.ResetContent(ids, setsNames);
                else
                    this.ucRadioBoxListCLDSAT.ResetContent(null, null);
            }
        }

        private void btnQueryCloudsat_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CheckCloudSATArgsIsOK())
                    return;
                if (_db == null)
                    _db = DBConnect.GetInstance();//创建数据库连接
                string sql = ConcatCloudsatSQL();
                dr = new DataReader(_ui, _db,dataGridViewCldSAT);//创建查询结果的识别语句，并把结果输出到gridview
                _db.OpenConnection();
                _db.Retrieval(sql, dr.DataReaderImplementationCloudSAT);//执行查询
                _db.CloseConnection();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool CheckCloudSATArgsIsOK()
        {
            if (!_ui.CheckCloudSATValidProductType(groupBox5))
                throw new ArgumentException("请选择产品类型");
            return true;
        }

        private string ConcatCloudsatSQL()
        {
            string prds=ucRadioBoxListCLDSAT.CheckedName;
            if(prds ==null||string.IsNullOrEmpty(prds))
                throw new ArgumentException("参数有误，无法进行查询！");
            StringBuilder sql = new StringBuilder();
            sql.Append("select a.ReceiveTime as DataTime, a.GranuleNumber as GranuleNumber,b.FileAllPointsCount as AllPointsCounts,b.DRIDPointsCount as CoverPtCounts,b.DRIDCoverPct as CoverPtPct,c.UpLatitude as NorthLat,c.UpLongitude as NorthLon,c.DownLatitude as SouthLat,c.DownLongitude as SouthLon,a.ImageData as ImageData from cp_cloudsat_tb a join cp_cloudsat2region_tb b,cp_cloudsatinregionlatlon_tb c where ");
            //查询筛选关键字：时间【日】、产品名称
            sql.Append("a.ProductName = '" + prds + "' and ");
            sql.Append("DATE_FORMAT(a.ReceiveTime,'%Y-%m-%d')>='").Append(dateTimePicker2.Text).Append("'");
            sql.Append(" and ");
            sql.Append("DATE_FORMAT(a.ReceiveTime,'%Y-%m-%d')<='").Append(dateTimePicker1.Text).Append("'");
            sql.Append(" and ");
            sql.Append("a.CloudSatID = b.CloudSatID");
            sql.Append(" and ");
            sql.Append("a.GranuleNumber = c.GranuleNumber;");
            return sql.ToString();
        }

        private void btnCloudSAToverview_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewCldSAT.RowCount <= 0)
                    throw new ArgumentException("没有符合要求的文件记录！请重新查询！");
                if (dataGridViewCldSAT.SelectedRows.Count == 0)
                    throw new ArgumentException("未选择生成快视图的文件记录！请选择！");
                string northname = "最北点", southname = "最南点", granuleNOname = "轨道号";
                int ncol = -1, scol = -1, granuleNOcol = -1;
                foreach (DataGridViewColumn cln in dataGridViewCldSAT.Columns)
                {
                    if (cln.HeaderText.Contains(northname))
                        ncol = cln.DisplayIndex;
                    else if (cln.HeaderText.Contains(southname))
                        scol = cln.DisplayIndex;
                    else if (cln.HeaderText.Contains(granuleNOname))
                        granuleNOcol = cln.DisplayIndex;
                }
                if (ncol == -1 || scol == -1 || granuleNOcol == -1)
                    return;
                int granuleNO;
                Dictionary<int, List<string>> overviewPts = new Dictionary<int, List<string>>();
                string[] lonlat = null;
                List<string> nslonlat = null;
                foreach (DataGridViewRow r in dataGridViewCldSAT.SelectedRows)
                {
                    if (r.Cells[ncol].Value == null || r.Cells[scol].Value == null || r.Cells[granuleNOcol].Value == null)
                        continue;
                    //解析记录的轨道号；
                    string value = r.Cells[granuleNOcol].Value.ToString();
                    if (string.IsNullOrEmpty(value) || !int.TryParse(value, out  granuleNO))
                        continue;
                    nslonlat = new List<string>();
                    //从数据库查出轨道号对应的经纬度点；
                    lonlat = r.Cells[ncol].Value.ToString().Split(',');
                    if (lonlat.Length!=2)
                        continue;
                    nslonlat.AddRange(lonlat);
                    lonlat = r.Cells[scol].Value.ToString().Split(',');
                    if (lonlat.Length != 2)
                        continue;
                    nslonlat.AddRange(lonlat);
                    float llf ;
                    bool isok = true;
                    foreach (string ll in nslonlat)
                    {
                        if (!float.TryParse(ll,out llf))
                        {
                            isok = false;
                            break;
                        }
                    }
                    if (isok)
                        overviewPts.Add(granuleNO, nslonlat);
                }
                string overviewpng = genOverViewPng(overviewPts);
                if (overviewpng == null)
                    return;
                if (_openEventhandler != null)
                    _openEventhandler(this, new DoubleClickRowHdrEventArgs(overviewpng));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }
        private string genOverViewPng(Dictionary<int, List<string>> overviewPts)
        {
            //根据每个点的经纬度计算其在快视图中的位置，每一组点绘制为一条曲线；
            string maskpng = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\ProductArgs\CLD\Cloudsat_Overview_withcountry.png";
            if (!File.Exists(maskpng))
                throw new ArgumentException("未找到中国区域快视图背景文件！请检查！"); 
            string overviewpng = AppDomain.CurrentDomain.BaseDirectory + @"\TEMP";
            string tempName = Path.Combine(overviewpng, Guid.NewGuid().ToString() + ".png");
            string filename = OverView(maskpng, 800);
            Bitmap bmpOriginal = new Bitmap(filename);
            try
            {
                string blockNum = string.Empty;
                float minX = 65.0f, maxY = 60.0f, resl = 0.1f;
                float x1, y1, x2, y2;
                using (Graphics g = Graphics.FromImage(bmpOriginal))
                {
                    Pen drawpen = new Pen(Color.Red, 2);
                    drawpen.DashPattern = new float[] { 5, 5 };
                    float offset = 0.4f;
                    foreach (int number in overviewPts.Keys)
                    {
                        offset += 0.1f;
                        if (offset >= 0.95)
                            offset = 0.1f;
                        blockNum = number.ToString();
                        x1 = (float.Parse(overviewPts[number][0]) - minX) / resl;
                        y1 = (maxY - float.Parse(overviewPts[number][1])) / resl;
                        x2 = (float.Parse(overviewPts[number][2]) - minX) / resl;
                        y2 = (maxY - float.Parse(overviewPts[number][3])) / resl;
                        g.DrawLine(drawpen, x1, y1, x2, y2);
                        g.DrawString(blockNum, new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(Color.Black), new PointF((x1 + (x2-x1)* offset)  - 10, (y1 + (y2-y1) * offset)));
                    }
                }
                bmpOriginal.Save(tempName);
                return tempName;
            }
            finally
            {
                bmpOriginal.Dispose();
                File.Delete(filename);
            }
        }

        public static string OverView(string prdFilename, int maxSize)
        {
            try
            {
                using (IRasterDataProvider prd = GeoDataDriver.Open(prdFilename) as IRasterDataProvider)
                {
                    int[] bands =new int[] { 1, 2, 3 };
                    for (int i = 0; i < 3; i++)
                    {
                        if (bands[i] > prd.BandCount)
                        {
                            bands[i] = 1;
                        }
                    }
                    string overViewFilename = Path.ChangeExtension(prdFilename, ".overview.png");
                    using (Bitmap bmp = GenerateOverview(prd, bands, maxSize))
                    {
                        bmp.MakeTransparent(Color.Black);
                        bmp.Save(overViewFilename, ImageFormat.Png);
                        bmp.Dispose();
                    }
                    return overViewFilename;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("生成缩略图失败" + ex.Message);
                return null;
            }
        }

        private static Bitmap GenerateOverview(IRasterDataProvider prd, int[] bandNos, int maxSize)
        {
            CoordEnvelope env = prd.CoordEnvelope;
            IOverviewGenerator v = prd as IOverviewGenerator;
            Size size = v.ComputeSize(maxSize);//缩略图最大不超过的尺寸
            Bitmap bm = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
            v.Generate(bandNos, ref bm);
            return bm;
        }
        #endregion

    }
}
