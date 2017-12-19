using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public partial class CLDDataRetrieval : Form
    {
        private static string _dataBaseXml = ConnectMySqlCloud._dataBaseXml;
        private Dictionary<CheckBox, long> cbx2PrdID = new Dictionary<CheckBox, long>();
        ConnectMySqlCloud _con;
        private static UIControls _ui = UIControls.GetInstance();   //界面控件集合
        private static DBConnect _db=null ;   //数据库实例
        public delegate void DoubleClickRowHdrEventHandler(object source,DoubleClickRowHdrEventArgs args);
        public event DoubleClickRowHdrEventHandler _openEventhandler;
        private DataReader dr = null;

        public CLDDataRetrieval()
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
            if (_db==null)
                return;
            if (dr == null)
                return;
            //DataReader dr = new DataReader(_ui, _db, dataGridViewDay, dataGridViewPeriod);
            dr.GetSelectedFilesFromGrid(_dataBaseXml);
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
                if (dr==null)
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
                MessageBox.Show("请选择产品数据源");
                return false;
            }
            else if (!_ui.CheckValidProductType())
            {
                MessageBox.Show("请选择产品类型");
                return false;
            }
            else if (!_ui.CheckValidDataSet())
            {
                MessageBox.Show("请选择数据集");
                return false;
            }
            else if (!_ui.CheckValidProductPeriod())
            {
                MessageBox.Show("请选择产品周期");
                return false;
            }
            else if (!_ui.CheckValidDayNight())
            {
                MessageBox.Show("请选择昼夜");
                return false;
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


    }
}
