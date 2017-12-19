using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class DataReader
    {
        private UIControls _ui;                                             //界面控件集合
        private DBConnect _db;                                              //数据库操作实例
        private DataGridView _gridDay;                                      //日表格控件
        private DataGridView _gridPeriod;                                   //周期表格控件
        private DataGridView _gridclousat =null;                                   //clousat表格控件
        private static List<string> _fileSelectedList = new List<string>(); //表格中被选中的文件路径
       /// <summary>
        /// 表格中被选中的文件路径
       /// </summary>
        public static List<string> FileSelectedList { get { return _fileSelectedList; } }

        private DataReader() { }

        public DataReader(UIControls ui, DBConnect db, DataGridView gridDay, DataGridView gridPeriod)
        {
            _ui = ui;
            _db = db;
            _gridDay = gridDay;
            _gridPeriod = gridPeriod;
        }

        public DataReader(UIControls ui, DBConnect db, DataGridView gridCloudsat)
        {
            _ui = ui;
            _db = db;
            _gridclousat = gridCloudsat;
        }

        /// <summary>
        /// 根据数据库返回的结果显示到图形界面
        /// </summary>
        /// <param name="reader"></param>
        public void DataReaderImplementation(MySqlDataReader reader)
        {
            foreach (Control c in _ui.GroupBoxProductPeriod.Controls)
            {
                RadioButton rb = c as RadioButton;
                if (rb != null && rb.Checked)
                {
                    switch (rb.Text)
                    {
                        case "日产品"://观测日期、区域、分辨率、数据集名、数据有效性、分钟段个数、分钟段时间、文件路径
                            {//select DataTime, KeyWords,Resolution,  ValidValuePercent, GranuleCounts, GranuleTimes, ImageData
                                _gridDay.Rows.Clear();
                                while (reader.Read())
                                {
                                    //string productID = reader["ProductID"].ToString();
                                    string dataTime = reader["DataTime"].ToString();
                                    //string sensorType = reader["SensorType"].ToString();
                                    string resolution = reader["Resolution"].ToString();
                                    string regionID = reader["KeyWords"].ToString();
                                    //string dayNightLabel = reader["DayNightLabel"].ToString();
                                    string dataQuality = reader["ValidValuePercent"].ToString();
                                    string granuleCounts = reader["GranuleCounts"].ToString();
                                    string granuleTimes = reader["GranuleTimes"].ToString();
                                    //string dataSet = reader["DataSetID"].ToString();
                                    string imageData = reader["ImageData"].ToString();
                                    _gridDay.Rows.Add(new string[] { dataTime, regionID,resolution,dataQuality, granuleCounts, granuleTimes, imageData });
                                }
                                break;
                            }
                        case "旬产品":
                        case "月产品":
                        case "年产品"://年、月、旬、区域、分辨率、数据集名、文件路径[、统计类型、有效百分比]；
                            {//select KeyWords,Resolution,   Year, Month, 10Day,ImageData, ValidValuePercent, statTypeID
                                _gridPeriod.Rows.Clear();
                                while (reader.Read())
                                {
                                    //string productID = reader["ProductID"].ToString();
                                    //string inputTime = reader["InputTime"].ToString();
                                    //string sensorType = reader["SensorType"].ToString();
                                    string year = reader["DataYear"].ToString();
                                    string month = reader["DataMonth"].ToString();                                    
                                    string tenDay = reader["Data10Day"].ToString();
                                    string resolution = reader["Resolution"].ToString();
                                    string regionID = reader["KeyWords"].ToString();
                                    //string dayNightLabel = reader["DayNightLabel"].ToString();
                                    //string dataQuality = reader["ValidValuePercent"].ToString();
                                    //string dataSet = reader["DataSetID"].ToString();
                                    string imageData = reader["ImageData"].ToString();
                                    string statTypeID = reader["statTypeID"].ToString();
                                    _gridPeriod.Rows.Add(new string[] { year, month, tenDay,regionID, resolution, imageData,   
                                        (statTypeID == "3" ? "AVG" : (statTypeID == "1" ? "MAX" : "MIN")) });
                                }
                                break;
                            }
                    }
                    break;
                }
            }
        }

        public void DataReaderImplementationCloudSAT(MySqlDataReader reader)
        {
            _gridclousat.Rows.Clear();
            while (reader.Read())
            {
                string dataTime = reader["DataTime"].ToString();
                string GranuleNumber = reader["GranuleNumber"].ToString();
                string AllPointsCounts = reader["AllPointsCounts"].ToString();
                string CoverPtCounts = reader["CoverPtCounts"].ToString();
                string CoverPtPct = reader["CoverPtPct"].ToString();
                string imageData = reader["ImageData"].ToString();
                string northlat = float.Parse(reader["NorthLat"].ToString()).ToString("f4");
                string northlon = float.Parse(reader["NorthLon"].ToString()).ToString("f4");
                string southlat = float.Parse(reader["SouthLat"].ToString()).ToString("f4");
                string southlon = float.Parse(reader["SouthLon"].ToString()).ToString("f4");
                //string northpt = "(" + northlon + "," + northlat + ")";
                //string southpt = "(" + southlon + "," + southlat + ")";
                string northpt =  northlon + "," + northlat ;
                string southpt = southlon + "," + southlat  ;
                _gridclousat.Rows.Add(new string[] { dataTime, GranuleNumber, AllPointsCounts, CoverPtCounts, CoverPtPct, northpt,southpt,imageData });
            }
        }

        //输出界面控件中选中的行的文件名
        public void GetSelectedFilesFromGrid(string xml,bool iscloudsat=false)
        {
            if (iscloudsat)
            {
                GetSelectFiles(_gridclousat, xml);
                return;
            }
            foreach (Control c in _ui.GroupBoxProductPeriod.Controls)
            {
                RadioButton rb = c as RadioButton;
                if (rb != null && rb.Checked)
                {
                    switch (rb.Text)
                    {
                        case "日产品":
                            {
                                GetSelectFiles(_gridDay,  xml);//true,
                                break;
                            }
                        case "旬产品":
                        case "月产品":
                        case "年产品":
                            {
                                GetSelectFiles(_gridPeriod,  xml); //false,                        
                                break;
                            }
                    }
                    break;
                }
            }
        }

        private void GetSelectFiles(DataGridView gridRows,string xml)//bool isDayFile,
        {
            _fileSelectedList.Clear();
            //string dir = null;// dbargs.OutputDir;
            //int col;
            //if (isDayFile)
            //    col = 4;
            //else
            //    col = 7;
            string name = "文件路径", dir;
            int col = -1;
            foreach (DataGridViewColumn cln in gridRows.Columns)
            {
                if (cln.HeaderText == name)
                {
                    col = cln.DisplayIndex;
                    break;
                }
            }
            if (col == -1)
                return;
            string localfname,fname;
            foreach (DataGridViewRow r in gridRows.Rows)
            {
                if (r.Selected)
                {
                    localfname=r.Cells[col].Value.ToString();
                    if (!string.IsNullOrEmpty(localfname))
                    {
                        dir = CLDDataRetrieval.GetRootPathFromName(localfname);
                        fname = Path.Combine(dir, localfname.TrimStart('\\'));
                        _fileSelectedList.Add(fname);
                    }
                }
            }
        }

    }
}
