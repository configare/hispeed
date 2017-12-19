#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Lixijia     时间：2013-9-29 10:00:45
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.IO;
using GeoDo.FileProject;
using GeoDo.RasterProject;
using System.Windows.Forms;
using System.Threading;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    /// <summary>
    /// 类名：ConnectionMySql
    /// 属性描述：
    /// 创建者：Lixijia   创建日期：2013-9-29 10:00:45
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
     public class ConnectMySqlCloud
    {
        public static string _dataBaseXml = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\ProductArgs\CLD\CPDataBaseArgs.xml";
        private MySqlConnection conn=null;
        private string _server;
        private string _database;
        private string _uid;
        private string _password;
        private Dictionary<string, String> _CloudSATRawPrdsComments = new Dictionary<string, string>();
        private Dictionary<string, MySqlDbType> _TabFieldName2Types = new Dictionary<string, MySqlDbType>();
        //private MySqlDataReader rdr = null;
        private static Object _obj = new Object();

        public MySqlConnection Connection
        {
            get { return conn; }
        }

        public ConnectMySqlCloud()
        {
            ConnectDB(_dataBaseXml);
            Initialization();
            //InitializeCloudSATrawPrdsComments();
        }

        public ConnectMySqlCloud(string xml)
        {
            ConnectDB(xml);
            Initialization();
            //InitializeCloudSATrawPrdsComments();
        }

        private void ConnectDB(string xml)
        {
            DataBaseArg arg = DataBaseArg.ParseXml(xml);
            _server = arg.ServerName;
            _database = arg.DatabaseName;
            _uid = arg.UID;
            _password = arg.Passwords;
        }

         public ConnectMySqlCloud(string server,string database,string uid,string passwords)
        {
            _server = server;
            _database = database;
            _uid = uid;
            _password = passwords;
            Initialization();
        }

        private void Initialization()
        {
            //string connString;
            StringBuilder connectionString = new StringBuilder();
            connectionString.Append("SERVER=").Append(_server).Append(";");
            connectionString.Append("DATABASE=").Append(_database).Append(";");
            connectionString.Append("UID=").Append(_uid).Append(";");
            connectionString.Append("PASSWORD=").Append(_password).Append(";");
            //connectionString.Append("MultipleActiveResultSets = true").Append(";");
            //connString = "server=" + _server + ";" + "database=" + _database + ";" + "uid=" + _uid + ";" + "password=" + _password + ";";
            conn = new MySqlConnection(connectionString.ToString());
            //TabFieldName2Types();
        }

        private void InitializeCloudSATrawPrdsComments()
        {
            _CloudSATRawPrdsComments.Add("1B-CPR", "Level 1B Received echo powers");
            _CloudSATRawPrdsComments.Add("2B-GEOPROF", "Cloud mask and radar reflectivities");
            _CloudSATRawPrdsComments.Add("2B-CLDCLASS", "Cloud Classification");
            _CloudSATRawPrdsComments.Add("2B-CWC-RO", "Radar-only liquid/ice water content");
            _CloudSATRawPrdsComments.Add("2B-TAU", "Cloud optical depth");
            _CloudSATRawPrdsComments.Add("2B-CWC-RVOD", "Radar + visible optical depth liquid/ice water content");
            _CloudSATRawPrdsComments.Add("2B-FLXHR", "Radiative fluxes and heating rates");
            _CloudSATRawPrdsComments.Add("2B-GEOPROF-Lidar".ToUpper(), "CloudSat CPR + CALIPSO Lidar Cloud mask");
            _CloudSATRawPrdsComments.Add("MODIS-AUX", "MODIS Auxiliary Data(Data from 22 MODIS channels)");
            _CloudSATRawPrdsComments.Add("ECMWF-AUX", "ECMWF Auxiliary Data(State Variables)");
            _CloudSATRawPrdsComments.Add("2C-PRECIP-COLUMN", "Precipitation Column precipitation");
        }

        private void ExceptionCatch(MySqlException ex)
        {
            switch (ex.Number)
            {
                case 0:
                    throw new Exception("Cannot connect to server.");
                case 1045:
                    throw new Exception("Invalid username/password, please try again");
                case 1042:
                    throw new Exception("MySQL服务未启动！Unable to connect to any of the specified MySQL hosts.");
                default:
                    throw ex;
            }
        }

         public bool ConnOpend()
        {
            if (conn.State == System.Data.ConnectionState.Open)
             {
                 return true;
             } 
                 return false;
        }

        public bool OpenConn()
        {
            try
            {
                conn.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                ExceptionCatch(ex);
                return false;
            }
        }

        public bool Closeconn()
        {
            try
            {
                conn.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.Write(ex.Message);
                return false;
            }
        }

        public bool UpdateRawCLDParatable(string tableName, string imagenameValue,string imgDataValue)
        {
            lock (_obj)
            {
                if (this.ConnOpend() || this.OpenConn())
                {
                    try
                    {
                        //string imagedata = GetRecordImageData(tableName, imagenameValue);
                        //if (imagedata != imgDataValue)
                        {
                            string queryf = "update {0} set ImageData = @imgdatavalue where ImageName = @keyfieldValue;";
                            string query = string.Format(queryf, tableName);
                            MySqlParameter[] param = new MySqlParameter[2];
                            param[0] = new MySqlParameter("@imgdatavalue", MySqlDbType.String);
                            param[0].Value = imgDataValue;
                            param[1] = new MySqlParameter("@keyfieldValue", MySqlDbType.String);
                            param[1].Value = imagenameValue;
                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.Add(param[0]);
                            cmd.Parameters.Add(param[1]);
                            cmd.ExecuteNonQuery();
                            //this.Closeconn();
                            return true;
                        }
                        //return false;
                    }
                    catch (MySqlException ex)
                    {
                        ExceptionCatch(ex);
                        return false;
                    }
                    finally
                    {
                        if (this.ConnOpend())
                            this.Closeconn();
                    }
                }
                return false;
            }
        }

        public string GetRecordImageData(string tableName, string imagenameValue)
        {
            lock (_obj)
            {
                string imagedata = null;
                //if (this.OpenConn() == true)
                {
                    string query = "SELECT ImageData From " + tableName + " where ImageName = @ImageName ;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("?ImageName", imagenameValue);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        imagedata = rdr.GetString(0);
                        break;
                    }
                    rdr.Close();
                    //conn.Close();
                }
                return imagedata;
            }
        }

        #region  数据入库
        #region 各种数据的数据集/波段与产品表的对应
        public void InsertISCCPbands2PrdsTable(int BandNOvalue, string BandName)
        {
            lock (_obj)
            {
                if (this.OpenConn() == true)
                {
                    string tableName = "cp_cldprds2isccpbands_tb";
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = string.Format("INSERT {0}(ISCCPbandNO,ISCCPbandName) VALUES(@ISCCPbandNO,@ISCCPbandName)", tableName);
                    cmd.Parameters.AddWithValue("?ISCCPbandNO", BandNOvalue);
                    cmd.Parameters.AddWithValue("?ISCCPbandName", BandName);
                    cmd.ExecuteNonQuery();
                    this.Closeconn();
                }
            }
        }

        public void InsertMOD06sets2PrdsTable(string setName, int prdsID)
        {
            lock (_obj)
            {
                if (this.OpenConn() == true)
                {
                    string tableName = "cp_cldprds2mod06sets_tb";
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = string.Format("INSERT {0}(MOD06DataSetName,ProductsID) VALUES(@MOD06DataSetName,@ProductsID)", tableName);
                    cmd.Parameters.AddWithValue("?MOD06DataSetName", setName);
                    cmd.Parameters.AddWithValue("?ProductsID", prdsID);
                    cmd.ExecuteNonQuery();
                    this.Closeconn();
                }
            }
        }

        public void InsertAIRSsets2PrdsTable(string setName, int prdsID)
        {
            lock (_obj)
            {
                if (this.OpenConn() == true)
                {
                    string tableName = "cp_cldprds2airssets_tb";
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = string.Format("INSERT {0}(AIRSDataSetName,ProductsID,FillValue) VALUES(@AIRSDataSetName,@ProductsID,@FillValue)", tableName);
                    cmd.Parameters.AddWithValue("?AIRSDataSetName", setName);
                    cmd.Parameters.AddWithValue("?ProductsID", prdsID);
                    cmd.Parameters.AddWithValue("?FillValue", -9999);
                    cmd.ExecuteNonQuery();
                    this.Closeconn();
                }
            }
        }

        public void UpdateAIRSsets2PrdsTable()
        {
            lock (_obj)
            {
                string tableName = "cp_cldprds2airssets_tb";
                int fillValue = -9999;
                int productsID = 0;
                string query = "UPDATE " + tableName + " SET FillValue = " + "@fillValue" + " where ProductsID" + "= @productsID";
                //Open connection
                if (this.OpenConn() == true)
                {
                    MySqlParameter[] param = new MySqlParameter[2];
                    param[0] = new MySqlParameter("@fillValue", MySqlDbType.Int32);
                    param[0].Value = fillValue;
                    param[1] = new MySqlParameter("@productsID", MySqlDbType.Int16);
                    param[1].Value = productsID;
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.Add(param[0]);
                    cmd.Parameters.Add(param[1]);
                    cmd.ExecuteNonQuery();
                    this.Closeconn();
                }
            }
        }
        #endregion

        #region 插入原始数据记录
        /// <summary>
        /// MOD06云参数原始数据入库
        /// </summary>
        /// <param name="idpara"></param>
        /// <param name="dataTime">数据时间</param>
        /// <param name="localfName">相对路径</param>
        /// <param name="fName"> 文件名称</param>
        /// <returns></returns>
        public void InsertRawMOD06Table(string tableName, DateTime dataTime, string fName, string localfName, string sensor)
        {
            lock (_obj)
            {
                if (this.ConnOpend() || this.OpenConn())
                {
                    string sat = "AQUA", projectionType = "NA";// keywords = "GLOBAL";
                    if (sensor == "MODIS")
                        sat = "TERRA";
                    string dataSource = "Copy";//, dataQuality = "A", remark = "Good";
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = string.Format("INSERT {0}(InputTime,ReceiveTime,ImageName,ImageData,Satellite,SensorType,DataSource,ProjectionType) VALUES(@InputTime,@ReceiveTime,@ImageName,@ImageData,@Satellite,@SensorType,@DataSource,@ProjectionType)", tableName);//,DataQuality,Remark//,@DataQuality,@remark
                    cmd.Parameters.AddWithValue("?InputTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("?ReceiveTime", dataTime);
                    cmd.Parameters.AddWithValue("?ImageName", fName);
                    cmd.Parameters.AddWithValue("?ImageData", localfName);
                    cmd.Parameters.AddWithValue("?Satellite", sat);//sat
                    cmd.Parameters.AddWithValue("?SensorType", sensor);
                    cmd.Parameters.AddWithValue("?DataSource", dataSource);
                    cmd.Parameters.AddWithValue("?ProjectionType", projectionType);
                    //cmd.Parameters.AddWithValue("?DataQuality", dataQuality);
                    //cmd.Parameters.AddWithValue("?Remark", remark);
                    cmd.ExecuteNonQuery();
                    if (this.ConnOpend())
                        this.Closeconn();
                }
            }
        }

        /// <summary>
        /// ISCCP云参数原始数据入库
        /// </summary>
        /// <param name="idpara"></param>
        /// <param name="datatype">类型</param>
        /// <param name="inputime">入库时间</param>
        /// <param name="relapathfile">相对路径</param>
        /// <param name="filename"> 文件名称</param>
        /// <returns></returns>
        public void InsertISCCP2Table(string tableName, DateTime dataTime, int UTCTime, string fName, string localfName)
        {
            lock (_obj)
            {
                if (this.ConnOpend() || this.OpenConn())
                {
                    string sat = "ISCCP", sensor = "D2", projectionType = "EqualArea", keywords = "GLOBAL";
                    float resol = 2.5f;
                    double lulat = 90.0, lulon = -180.0, rdlat = -90.0, rdlon = 180.0;
                    string dataSource = "Copy", dataQuality = "A";
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;//@ISCCPID,
                    //cmd.CommandText = string.Format("INSERT {0}(InputTime,ReceiveTime,ReceiveUTCTime,ImageName,ImageData,KeyWord,Satellite,SensorType,DataSource,ProjectionType,Resolution,LTLongtitude,LTLatitude,RDLongitude,RDLatitude,DataQuality) VALUES(@InputTime,@ReceiveTime,@ReceiveUTCTime,@ImageName,@ImageData,@KeyWord,@Satellite,@SensorType,@DataSource,@ProjectionType,@Resolution,@LTLongtitude,@LTLatitude,@RDLongitude,@RDLatitude,@DataQuality)", tableName);
                    cmd.CommandText = string.Format("INSERT {0}(InputTime,ReceiveTime,ReceiveUTCTime,ImageName,ImageData,KeyWord,Satellite,SensorType,DataSource,ProjectionType,Resolution,LTLongtitude,LTLatitude,RDLongitude,RDLatitude,DataQuality) VALUES(@InputTime,@ReceiveTime,@ReceiveUTCTime,@ImageName,@ImageData,@KeyWord,@Satellite,@SensorType,@DataSource,@ProjectionType,@Resolution,@LTLongtitude,@LTLatitude,@RDLongitude,@RDLatitude,@DataQuality)", tableName);
                    //cmd.Parameters.AddWithValue("?ISCCPID", no);
                    cmd.Parameters.AddWithValue("?InputTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("?ReceiveTime", dataTime);
                    cmd.Parameters.AddWithValue("?ReceiveUTCTime", UTCTime);
                    cmd.Parameters.AddWithValue("?ImageName", fName);
                    cmd.Parameters.AddWithValue("?ImageData", localfName);
                    cmd.Parameters.AddWithValue("?KeyWord", keywords);
                    cmd.Parameters.AddWithValue("?Satellite", sat);
                    cmd.Parameters.AddWithValue("?SensorType", sensor);
                    cmd.Parameters.AddWithValue("?DataSource", dataSource);
                    cmd.Parameters.AddWithValue("?ProjectionType", projectionType);
                    cmd.Parameters.AddWithValue("?Resolution", resol);
                    cmd.Parameters.AddWithValue("?LTLongtitude", lulon);
                    cmd.Parameters.AddWithValue("?LTLatitude", lulat);
                    cmd.Parameters.AddWithValue("?RDLongitude", rdlon);
                    cmd.Parameters.AddWithValue("?RDLatitude", rdlat);
                    cmd.Parameters.AddWithValue("?DataQuality", dataQuality);
                    cmd.ExecuteNonQuery();
                    if (this.ConnOpend())
                        this.Closeconn();
                }
            }
        }
        #endregion

        #region 日拼接产品入库
        public int QueryDayMergeGranuleCounts(string ImageName)
        {
            lock (_obj)
            {
                int counts = 0;
                if (this.ConnOpend() || this.OpenConn())
                {
                    string tablename = "CP_DayMergeProducts_TB";//, fieldName = "GranuleCounts";
                    string sql = "SELECT GranuleCounts  FROM {0} where ImageName =@ImageName";// +" where AIRSDataSetName = @setName";
                    sql = string.Format(sql, tablename);
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("?ImageName", ImageName);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        counts = rdr.GetInt32(0);
                        break;
                    }
                    rdr.Close();
                    if (this.ConnOpend())
                        conn.Close();
                }
                return counts;
            }
        }

        public void InsertNDayMergeProductsTable(DateTime dataTime, long prdID,long datasetID, string localfName,long regionID, string region, string sensor, float resol, int validPercent, int granuleCounts, string granuleTimes,string dataSource)
        {
            lock (_obj)
            {
                string tableName = "CP_DayMergeProducts_TB";
                if (this.ConnOpend() || this.OpenConn())
                {
                    int dayNightLabel = 0;
                    string fname = Path.GetFileName(localfName);
                    if (fname.Split('_')[0].ToUpper().Contains("DAY"))
                    {
                        dayNightLabel = 1;
                    }
                    else if (fname.Split('_')[0].ToUpper().Contains("NIGHT"))
                    {
                        dayNightLabel = 2;
                    }
                    string sat = "AQUA";
                    if (sensor == "MODIS")
                        sat = "TERRA";
                    int projectionTypeID = 1;// keywords = "GLOBAL";
                    string dataQuality = "A";
                    string sql = "INSERT " + tableName + " (InputTime,DataTime,ProductID,DataSetID,ImageName,ImageData,DayNightLabel,regionID,KeyWords,Satellite,SensorType," +
                    "ProjectionTypeID,Resolution,ValidValuePercent,GranuleCounts,GranuleTimes,DataQuality,DataSource) " +
                    "VALUES(@InputTime,@DataTime,@ProductID,@DataSetID,@ImageName,@ImageData,@DayNightLabel,@regionID,@KeyWords,@Satellite,@SensorType,@ProjectionTypeID,@" +
                    "Resolution,@ValidValuePercent,@GranuleCounts,@GranuleTimes,@DataQuality,@DataSource)";
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("?InputTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("?DataTime", dataTime);
                    cmd.Parameters.AddWithValue("?ProductID", prdID);
                    cmd.Parameters.AddWithValue("?DataSetID", datasetID);
                    cmd.Parameters.AddWithValue("?ImageName", fname);
                    cmd.Parameters.AddWithValue("?ImageData", localfName);
                    cmd.Parameters.AddWithValue("?DayNightLabel", dayNightLabel);
                    cmd.Parameters.AddWithValue("?regionID", regionID);
                    cmd.Parameters.AddWithValue("?KeyWords", region);
                    cmd.Parameters.AddWithValue("?Satellite", sat);
                    cmd.Parameters.AddWithValue("?SensorType", sensor);
                    cmd.Parameters.AddWithValue("?ProjectionTypeID", projectionTypeID);
                    cmd.Parameters.AddWithValue("?Resolution", resol);
                    cmd.Parameters.AddWithValue("?ValidValuePercent", validPercent);
                    cmd.Parameters.AddWithValue("?GranuleCounts", granuleCounts);
                    cmd.Parameters.AddWithValue("?GranuleTimes", granuleTimes);
                    cmd.Parameters.AddWithValue("?DataQuality", dataQuality);
                    cmd.Parameters.AddWithValue("?DataSource", dataSource);
                    cmd.ExecuteNonQuery();
                    if (this.ConnOpend())
                        this.Closeconn();
                }
            }
        }
        #endregion

        #region 周期合成产品入库
        public void InsertPeriodicSynProductsTable(long prdID, long setID,string localfName, int regionID,string region, string sensor, float resol, long periodTypeID, int year,int month, int xun, int statTypeID,string dataSource)
        {
            lock (_obj)
            {
                string tableName = "cp_periodicsynthesis_tb";
                if (this.ConnOpend() || this.OpenConn())
                {
                    int dayNightLabel = 0;
                    string label = Path.GetFileName(localfName).ToUpper().Split('_')[0];
                    if (label.Contains("DAY"))
                        dayNightLabel = 1;
                    else if (label.Contains("NIGHT"))
                        dayNightLabel = 2;
                    string sat = "AQUA";
                    if (sensor == "MOD06")
                    {
                        sat = "TERRA";
                        sensor = "MODIS";
                    }
                    else if (sensor == "MYD06")
                    {
                        sat = "AQUA";
                        sensor = "MODIS";
                    }
                    int projectionTypeID = 1;// keywords = "GLOBAL";
                    string dataQuality = "A";
                    string sql = "INSERT " + tableName + " (InputTime,ProductID,DataSetID,ImageName,ImageData,DayNightLabel,regionID,KeyWords" +
                        ",Resolution,Satellite,SensorType,ProjectionTypeID," +
                        "PeriodTypeID,DataYear,DataMonth,Data10Day,StatTypeID,DataQuality,DataSource) " +
                    "VALUES(@InputTime,@ProductID,@DataSetID,@ImageName,@ImageData,@DayNightLabel,@regionID,@KeyWords" +
                    ",@Resolution,@Satellite,@SensorType,@ProjectionTypeID,@" +
                        "PeriodTypeID,@DataYear,@DataMonth,@Data10Day,@StatTypeID,@DataQuality,@DataSource) ";
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("?InputTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("?ProductID", prdID);
                    cmd.Parameters.AddWithValue("?DataSetID", setID);
                    cmd.Parameters.AddWithValue("?ImageName", Path.GetFileName(localfName));
                    cmd.Parameters.AddWithValue("?ImageData", localfName);
                    cmd.Parameters.AddWithValue("?DayNightLabel", dayNightLabel);
                    cmd.Parameters.AddWithValue("?regionID", regionID);
                    cmd.Parameters.AddWithValue("?KeyWords", region);
                    cmd.Parameters.AddWithValue("?Resolution", resol);
                    cmd.Parameters.AddWithValue("?Satellite", sat);
                    cmd.Parameters.AddWithValue("?SensorType", sensor);
                    cmd.Parameters.AddWithValue("?ProjectionTypeID", projectionTypeID);
                    cmd.Parameters.AddWithValue("?PeriodTypeID", periodTypeID);
                    cmd.Parameters.AddWithValue("?DataYear", year);
                    cmd.Parameters.AddWithValue("?DataMonth", month);
                    cmd.Parameters.AddWithValue("?Data10Day", xun);
                    cmd.Parameters.AddWithValue("?StatTypeID", statTypeID);
                    cmd.Parameters.AddWithValue("?DataQuality", dataQuality);
                    cmd.Parameters.AddWithValue("?DataSource", dataSource);
                    cmd.ExecuteNonQuery();
                    if (this.ConnOpend())
                        this.Closeconn();
                }
            }
        }
        #endregion

        #region 插入数据时间连续性检测记录
        public void InsertMOD06DataTimeContinutyTable(DateTime dataTime,string keywords)
        {
            lock (_obj)
            {
                if (this.OpenConn() == true)
                {
                    string tableName = "cp_mod06rawdatatimecontinuty_tb";
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = string.Format("INSERT {0}(InputTime,LostDate,Keywords) VALUES(@InputTime,@LostDate,@Keywords)", tableName);
                    cmd.Parameters.AddWithValue("?InputTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("?LostDate", dataTime);
                    cmd.Parameters.AddWithValue("?Keywords", keywords);
                    cmd.ExecuteNonQuery();
                    this.Closeconn();
                }
            }
        }

        public void InsertISCCPDataTimeContinutyTable(int year, int month, string utcTime, bool isAllDayLostTip)
        {
            lock (_obj)
            {
                if (this.OpenConn() == true)
                {
                    string tableName = "cp_isccpdatatimecontinuty_tb";
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = string.Format("INSERT {0}(InputTime,Lostyear,Lostmonth,LostUTCTime,AllDayLostTip,Remark) VALUES(@InputTime,@Lostyear,@Lostmonth,@LostUTCTime,@AllDayLostTip,@Remark)", tableName);
                    cmd.Parameters.AddWithValue("?InputTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("?Lostyear", year);
                    cmd.Parameters.AddWithValue("?Lostmonth", month);
                    cmd.Parameters.AddWithValue("?LostUTCTime", utcTime);
                    cmd.Parameters.AddWithValue("?AllDayLostTip", (isAllDayLostTip ? "TRUE" : "FALSE"));
                    cmd.Parameters.AddWithValue("?Remark", "");
                    cmd.ExecuteNonQuery();
                    this.Closeconn();
                }
            }
        }

        public void InsertAIRSDataTimeContinutyTable(DateTime dataTime, int lostNOinDate, bool isAllDayLostTip)
        {
            lock (_obj)
            {
                if (this.OpenConn() == true)
                {
                    string tableName = "cp_airsrawdatatimecontinuty_tb";
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = string.Format("INSERT {0}(InputTime,LostDateTime,LostNOinDate,AllDayLostTip,Remark) VALUES(@InputTime,@LostDateTime,@LostNOinDate,@AllDayLostTip,@Remark)", tableName);
                    cmd.Parameters.AddWithValue("?InputTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("?LostDateTime", dataTime);
                    cmd.Parameters.AddWithValue("?LostNOinDate", lostNOinDate);
                    cmd.Parameters.AddWithValue("?AllDayLostTip", (isAllDayLostTip ? "TRUE" : "FALSE"));
                    cmd.Parameters.AddWithValue("?Remark", "");
                    cmd.ExecuteNonQuery();
                    this.Closeconn();
                }
            }
        }

        public void InsertPrdsDataTimeContinutyTable(long prdID, string setName,DateTime dataTime, string satellite, string sensorType, string keywords)
        {
            lock (_obj)
            {
                if (this.OpenConn() == true)
                {
                    string tableName = "cp_dmprdsuncontinuty_tb";
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = string.Format("INSERT {0}(InputTime,ProductID,DatasetName,LostDate,Satellite,SensorType,KeyWords) VALUES(@InputTime,@ProductID,@DatasetName,@LostDate,@Satellite,@SensorType,@KeyWords)", tableName);
                    cmd.Parameters.AddWithValue("?InputTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("?ProductID", prdID);
                    cmd.Parameters.AddWithValue("?DatasetName", setName);
                    cmd.Parameters.AddWithValue("?LostDate", dataTime);
                    cmd.Parameters.AddWithValue("?Satellite", satellite);
                    cmd.Parameters.AddWithValue("?SensorType", sensorType);
                    cmd.Parameters.AddWithValue("?KeyWords", keywords);
                    cmd.ExecuteNonQuery();
                    this.Closeconn();
                }
            }
        }
        #endregion
        #endregion

        #region 查询
        public string[] QueryDatasetsWithPrdsID(string sensor)
        {
            lock (_obj)
            {
                //Thread.Sleep(15000);
                List<string> setsLists = new List<string>();
                if (this.ConnOpend() || this.OpenConn())
                {
                    string sql;
                    if (sensor == "MOD06")
                        sql = "SELECT MOD06DataSetDisplayName FROM cp_cldprds2mod06sets_tb where ProductsID != 0";
                    else if (sensor == "MYD06")
                        sql = "SELECT MOD06DataSetDisplayName FROM cp_cldprds2myd06sets_tb where ProductsID != 0";
                    else if (sensor == "AIRS")
                        sql = "SELECT AIRSDataSetDisplayName FROM cp_cldprds2airssets_tb where ProductsID != 0";
                    else
                        return null;
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        setsLists.Add(rdr.GetString(0));
                    }
                    rdr.Close();
                    if (this.ConnOpend())
                        conn.Close();
                }
                return setsLists.ToArray();
            }
        }

        public bool QueryPrdID(string sat, string setName, out long prdID)
        {
            lock (_obj)
            {
                prdID = 0;
                if (this.ConnOpend() || this.OpenConn())
                {
                    string tablename = "", fieldName = "";
                    if (sat.ToUpper() == "MODIS")
                    {
                        tablename = "cp_cldprds2mod06sets_tb";
                        fieldName = "MOD06DataSetName";
                    }
                    else if (sat.ToUpper() == "AIRS")
                    {
                        tablename = "cp_cldprds2airssets_tb";
                        fieldName = "AIRSDataSetName";
                    }
                    else
                        return false;
                    string sql = "SELECT {0},ProductsID  FROM {1}";// +" where AIRSDataSetName = @setName";
                    sql = string.Format(sql, fieldName, tablename);
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        if (rdr.GetString(0) == setName)
                        {
                            prdID = rdr.GetInt64(1);
                            break;
                        }
                    }
                    rdr.Close();
                    if (this.ConnOpend())
                        conn.Close();
                }
                if (prdID != 0)
                    return true;
                return false;
            }
        }

        public bool QueryDatasetsInvalidValue(string sat, string setName, out double setFillValue, out double DayFillValue,out double DayInvalidValue)
        {
            lock (_obj)
            {
                setFillValue = 0;
                DayFillValue = 0;
                DayInvalidValue = 0;
                if (this.ConnOpend() || this.OpenConn())
                {
                    string tablename = "", fieldName = "";

                    if (sat.ToUpper() == "MOD06")
                    {
                        tablename = "cp_cldprds2mod06sets_tb";
                        fieldName = "MOD06DataSetName";
                    }
                    else if (sat.ToUpper() == "MYD06")
                    {
                        tablename = "cp_cldprds2myd06sets_tb";
                        fieldName = "MOD06DataSetName";
                    }
                    else if (sat.ToUpper() == "AIRS")
                    {
                        tablename = "cp_cldprds2airssets_tb";
                        fieldName = "AIRSDataSetName";
                    }
                    else
                        return false;
                    string sql = "SELECT {0},FillValue,DayFillValue,DayInvalidValue  FROM {1}";// +" where AIRSDataSetName = @setName";
                    sql = string.Format(sql, fieldName, tablename);
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        if (rdr.GetString(0) == setName)
                        {
                            setFillValue = double.Parse(rdr.GetString(1));
                            DayFillValue = double.Parse(rdr.GetString(2));
                            DayInvalidValue = double.Parse(rdr.GetString(3));
                            break;
                        }
                    }
                    rdr.Close();
                    if (this.ConnOpend())
                        conn.Close();
                }
                if (DayFillValue != 0 && DayInvalidValue != 0)
                    return true;
                return false;
            }
        }

        public string[] QueryMOD06setsWithPrdsID()
        {
            lock (_obj)
            {
                List<string> setsLists = new List<string>();
                if (this.OpenConn() == true)
                {
                    string tablename = "cp_cldprds2mod06sets_tb";
                    string sql = "SELECT MOD06DataSetDisplayName FROM " + tablename + " where ProductsID != 0";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        //if (rdr.GetInt64(1)!=0)
                        //{
                        setsLists.Add(rdr.GetString(0));
                        //}
                    }
                    rdr.Close();
                    conn.Close();
                }
                return setsLists.ToArray();
            }
        }

        public string[] QueryMYD06setsWithPrdsID()
        {
            lock (_obj)
            {
                List<string> setsLists = new List<string>();
                if (this.OpenConn() == true)
                {
                    string tablename = "cp_cldprds2myd06sets_tb";
                    string sql = "SELECT MOD06DataSetDisplayName FROM " + tablename + " where ProductsID != 0";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        setsLists.Add(rdr.GetString(0));
                    }
                    rdr.Close();
                    conn.Close();
                }
                return setsLists.ToArray();
            }
        }


        public string[] QueryAIRSsetsWithPrdsID()
        {
            lock (_obj)
            {
                List<string> setsLists = new List<string>();
                if (this.OpenConn() == true)
                {
                    string tablename = "cp_cldprds2airssets_tb";
                    string sql = "SELECT AIRSDataSetDisplayName FROM " + tablename + " where ProductsID != 0";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        setsLists.Add(rdr.GetString(0));
                    }
                    rdr.Close();
                    conn.Close();
                }
                return setsLists.ToArray();
            }
        }

        public bool QueryPrdsID2Datasets(string sensor,long prdID, out string[] setNames, out long[] setID)
        {
            lock (_obj)
            {
                setNames = new string[] { };
                setID = new long[] { };
                if (this.ConnOpend() || this.OpenConn())
                {
                    string sql;
                    if (sensor == "MODIS")
                        sql = "SELECT  Prds2MOD06ID,MOD06SetsLabel FROM cp_cldprds2mod06sets_tb where ProductsID =@prdID";
                    else if (sensor == "AIRS")
                        sql = "SELECT  Prds2AIRSID,AIRSSetsLabel,ProductsID FROM cp_cldprds2airssets_tb where ProductsID =@prdID";
                    else if (sensor == "ISCCPD2")
                        sql = "SELECT  Prds2ISCCPID,ISCCPbandlabel,ProductsID FROM cp_cldprds2isccpbands_tb where ProductsID =@prdID";
                    else
                        return false;
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("?prdID", prdID);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    List<string> setNameList = new List<string>();
                    List<long> setIDList = new List<long>();
                    while (rdr.Read())
                    {
                        if (!string.IsNullOrEmpty(rdr.GetString(1)))
                        {
                            setIDList.Add(rdr.GetInt64(0));
                            setNameList.Add(rdr.GetString(1));
                        }
                    }
                    rdr.Close();
                    if (this.ConnOpend())
                        conn.Close();
                    setNames = setNameList.ToArray();
                    setID = setIDList.ToArray();
                }
                if (setNames.Length != 0)
                    return true;
                return false;
            }
        }

        public bool QueryPrdsInfo(string sensorName, out string[] setNames, out long[] setID)
        {
            lock (_obj)
            {
                setNames = new string[] { };
                setID = new long[] { };
                if (this.ConnOpend() || this.OpenConn())
                {
                    List<string> setNameList = new List<string>();
                    List<long> setIDList = new List<long>();
                    string tablename1 = "cp_prds2sensor_view";
                    string sql = "SELECT  ProductsID,ProductsComments FROM " + tablename1 + " where sensorName =@sensorName";// +" where AIRSDataSetName = @setName";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("?sensorName", sensorName);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        setIDList.Add(rdr.GetInt64(0));
                        setNameList.Add(rdr.GetString(1));
                    }
                    rdr.Close();
                    if (this.ConnOpend())
                        conn.Close();
                    setNames = setNameList.ToArray();
                    setID = setIDList.ToArray();
                }
                if (setNames.Length != 0)
                    return true;
                return false;
            }
        }

        public bool QueryDatasetsID(string mode, string setName, out long prdID, out long setID)
        {
            lock(_obj)
            {
            setID = 0;
            prdID = 0;
            if (this.ConnOpend() || this.OpenConn())
            {
                string tablename1;
                string sql;
                if (mode.ToUpper().Contains("MOD"))
                {
                    tablename1 = "cp_cldprds2mod06sets_tb";
                    sql = "SELECT  Prds2MOD06ID,ProductsID FROM " + tablename1 + " where MOD06DataSetName =@setName";
                }
                else if (mode.ToUpper().Contains("MYD"))
                {
                    tablename1 = "cp_cldprds2myd06sets_tb";
                    sql = "SELECT  Prds2MOD06ID,ProductsID FROM " + tablename1 + " where MOD06DataSetName =@setName";
                }
                else if (mode.ToUpper().Contains("AIRS"))
                {
                    tablename1 = "cp_cldprds2airssets_tb";
                    sql = "SELECT  Prds2AIRSID,ProductsID FROM " + tablename1 + " where AIRSDataSetName =@setName";
                }
                else
                    return false;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("?setName", setName.Replace("_",""));
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    setID = rdr.GetInt64(0);
                    prdID = rdr.GetInt64(1);
                }
                rdr.Close();
                if (this.ConnOpend())
                    conn.Close();
            }
            if (setID != 0 && prdID != 0)
                return true;
            return false;
            }
        }
         
        #endregion
        public void DeleteCLDParatableRecord(string tableName, string keyfieldName, string keyfieldValue)
        {
            try
            {
                lock (_obj)
                {
                    if (this.ConnOpend() || this.OpenConn())
                    {
                        string query = @"delete  from " + tableName + " where {0}" + "= ?{1}";
                        query = string.Format(query, keyfieldName, "keyfieldValue");
                        MySqlParameter[] param = new MySqlParameter[1];
                        param[0] = new MySqlParameter("?keyfieldValue", MySqlDbType.String);
                        param[0].Value = keyfieldValue;
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.Add(param[0]);
                        int updateRowCount = cmd.ExecuteNonQuery();
                        if (this.ConnOpend())
                            this.Closeconn();
                    }
                }
            }
            catch (MySqlException ex)
            {
                ExceptionCatch(ex);
            }
        }

        public bool IshasRecord(string tableName, string selectfieldName, string fieldValue)
        {
            lock (_obj)
            {
                int count = -1;
                if (this.ConnOpend() || this.OpenConn())
                {
                    string query = "SELECT COUNT(*) From " + tableName + " where " + selectfieldName + "= @fieldValue";
                    MySqlParameter[] param = new MySqlParameter[1];
                    param[0] = new MySqlParameter("@fieldValue", MySqlDbType.String);
                    param[0].Value = fieldValue;
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.Add(param[0]);
                    count = int.Parse(cmd.ExecuteScalar() + "");
                    if (this.ConnOpend())
                        this.Closeconn();
                    if (count == 0)
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
        }

        public bool IshasRecord(string tableName, string selectfieldName, long  fieldValue)
        {
            lock (_obj)
            {
                int count = -1;
                if (this.ConnOpend() || this.OpenConn())
                {
                    string query = "SELECT COUNT(*) From " + tableName + " where " + selectfieldName + "= @fieldValue";
                    MySqlParameter[] param = new MySqlParameter[1];
                    param[0] = new MySqlParameter("@fieldValue", MySqlDbType.Int64);
                    param[0].Value = fieldValue;
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.Add(param[0]);
                    count = int.Parse(cmd.ExecuteScalar() + "");
                    if (this.ConnOpend())
                        this.Closeconn();
                    if (count == 0)
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
        }
         
        public string[] QueryAllDayMergeFiles(string sensor,out string[] fdate, out int[] validPcts, out int[] granulescounts, out string[] granuleTimes,out string [] datasource)
        {
            lock (_obj)
            {
                fdate = null; validPcts = null; granulescounts = null; granuleTimes = null; datasource = null;
                string table = "cp_daymergeproducts_tb";
                List<string> fdatelist = new List<string>();
                List<string> datasourcel = new List<string>();
                List<string> fileList = new List<string>();
                List<int> validPctlist = new List<int>();
                List<int> granulescountslst = new List<int>();
                List<string> granuleTimeslst = new List<string>();
                if (this.OpenConn() == true)
                {
                    string sql = "select DataTime,ImageName,ValidValuePercent,GranuleCounts,GranuleTimes,DataSource From {0} where SensorType =@sensor and to_days(InputTime) = to_days(now()) order by ValidValuePercent asc,substring_index(ImageName,'_',1)  asc";// where to_days(InputTime) = to_days(now())  
                    sql = string.Format(sql, table);
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("?sensor", sensor);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        fdatelist.Add(rdr.GetDateTime(0).ToShortDateString());
                        fileList.Add(rdr.GetString(1));
                        validPctlist.Add(rdr.GetInt32(2));
                        granulescountslst.Add(rdr.GetInt16(3));
                        granuleTimeslst.Add(rdr.GetString(4));
                        datasourcel.Add(rdr.GetString(5));
                    }
                    rdr.Close();
                    conn.Close();
                    fdate = fdatelist.ToArray();
                    validPcts = validPctlist.ToArray();
                    granulescounts = granulescountslst.ToArray();
                    granuleTimes = granuleTimeslst.ToArray();
                    datasource = datasourcel.ToArray();
                }
                if (fileList.Count != 0)
                {
                    return fileList.ToArray();
                }
                return new string[] { };
            }
        }

        public struct DayMergeLine
        {
            public DateTime Datatime;
            public int ValidPct;
            public int GranuleCounts;
            public string GranuleTimes;
            public string ImageName;
         }

        public List<DayMergeLine> QureyDayMergeMonthly(string relaDir)
        {
            lock(_obj)
            {
                if (this.ConnOpend() || this.OpenConn())
                {
                    string set, sat,sensor, dnlabel;
                    int year, month;
                    float resl;
                    string[] parts = null;
                    List<string> partl = new List<string>();
                    parts = relaDir.Split('\\');
                    foreach (string pp in parts)
                    {
                        if (string.IsNullOrEmpty(pp) || string.IsNullOrWhiteSpace(pp))
                            continue;
                        else
                            partl.Add(pp);
                    }
                    parts = partl.ToArray();
                    set = parts[1];
                    sat = parts[2];
                    sensor = parts[3];
                    year = int.Parse(parts[4]);
                    month = int.Parse(parts[5]);
                    dnlabel = parts[6];
                    if (dnlabel.ToUpper() == "DAY")
                        dnlabel = "D";
                    else
                        dnlabel = "N";
                    resl = float.Parse(parts[7]);
                    string mod06prdtable = "cp_cldprds2mod06sets_tb", myd06prdtable = "cp_cldprds2myd06sets_tb", mod06pid = "Prds2MOD06ID", mod06setname = "MOD06DataSetName";
                    string airsprdtable = "cp_cldprds2airssets_tb", airspid = "Prds2AIRSID", airssetname = "AIRSDataSetName";
                    string sql = "select a.DataTime,a.ValidValuePercent,a.GranuleCounts,a.GranuleTimes,a.ImageName From (cp_daymergeproducts_tb a join {0} b )where a.DataSetID=b.{1} AND b.{2} =@set AND YEAR(a.DataTime)=@year AND MONTH(a.DataTime)=@month AND  a.DataSource=@dnlabel and abs(a.Resolution-@resl)<0.001 order by a.DataTime asc";//AND a.Resolution =@resl // +" where AIRSDataSetName = @setName";
                    if (sensor.ToUpper().Contains("MOD"))
                    {
                        if (sat.ToUpper()=="AQUA")
                            sql = string.Format(sql, myd06prdtable, mod06pid, mod06setname);
                        else
                            sql = string.Format(sql, mod06prdtable, mod06pid, mod06setname);
                    }
                    else
                        sql = string.Format(sql, airsprdtable, airspid, airssetname);
                    sql = string.Format(sql, relaDir);
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("?year", year);
                    cmd.Parameters.AddWithValue("?month", month);
                    cmd.Parameters.AddWithValue("?set", set);
                    cmd.Parameters.AddWithValue("?resl", resl);
                    cmd.Parameters.AddWithValue("?dnlabel", dnlabel);
                    MySqlDataReader rdr = null;
                    try
                    {
                        if (rdr == null)
                            rdr = cmd.ExecuteReader();
                        else if (rdr.IsClosed == false)
                        {
                            rdr.Close();
                            rdr = cmd.ExecuteReader();
                        }
                        List<DayMergeLine> lineValues = new List<DayMergeLine>();
                        DayMergeLine linevalue = new DayMergeLine();
                        while (rdr.Read())
                        {
                            linevalue.Datatime = rdr.GetDateTime(0);
                            linevalue.ValidPct = rdr.GetInt16(1);
                            linevalue.GranuleCounts = rdr.GetInt16(2);
                            linevalue.GranuleTimes = rdr.GetString(3);
                            linevalue.ImageName = rdr.GetString(4);
                            lineValues.Add(linevalue);
                        }
                        rdr.Close();
                        rdr = null;
                        if (this.ConnOpend() || this.OpenConn())
                            conn.Close();
                        return lineValues;
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (rdr != null)
                        {
                            if (!rdr.IsClosed)
                                rdr.Close();
                            rdr = null;
                        }
                    }
                }
                return null;
            }            
        }
         
        public void UpdateMOD06setstable(string [] names)
        {
            lock (_obj)
            {
                if (this.ConnOpend() || this.OpenConn())
                {
                    try
                    {
                        string tableName = "cp_cldprds2mod06sets_tb";
                        foreach (string set in names)
                        {
                            if (!IshasthisRecord(tableName, "MOD06DataSetName", set.Replace("_", "")))
                            {
                                MySqlCommand cmd = new MySqlCommand();
                                cmd.Connection = conn;
                                cmd.CommandText = string.Format("INSERT {0}(MOD06DataSetName,MOD06DataSetDisplayName) VALUES(@MOD06DataSetName,@MOD06DataSetDisplayName)", tableName);
                                cmd.Parameters.AddWithValue("?MOD06DataSetName", set.Replace("_", ""));
                                cmd.Parameters.AddWithValue("?MOD06DataSetDisplayName", set);
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                string queryf = "update cp_cldprds2mod06sets_tb set MOD06DataSetDisplayName = @imgdatavalue where MOD06DataSetName = @keyfieldValue;";
                                MySqlParameter[] param = new MySqlParameter[2];
                                param[0] = new MySqlParameter("@imgdatavalue", MySqlDbType.String);
                                param[0].Value = set;
                                param[1] = new MySqlParameter("@keyfieldValue", MySqlDbType.String);
                                param[1].Value = set.Replace("_", "");
                                MySqlCommand cmd = new MySqlCommand(queryf, conn);
                                cmd.Parameters.Add(param[0]);
                                cmd.Parameters.Add(param[1]);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        ExceptionCatch(ex);
                        return;
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        //if(this.OpenConn())
                        this.Closeconn();
                    }
                }
            }
        }

        public bool IshasthisRecord(string tableName, string selectfieldName, string fieldValue)
        {
            lock (_obj)
            {
                int count = -1;
                string query = "SELECT COUNT(*) From " + tableName + " where " + selectfieldName + "= @fieldValue";
                MySqlParameter[] param = new MySqlParameter[1];
                param[0] = new MySqlParameter("@fieldValue", MySqlDbType.String);
                param[0].Value = fieldValue;
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.Add(param[0]);
                count = int.Parse(cmd.ExecuteScalar() + "");
                //this.Closeconn();
                if (count == 0)
                    return false;
                else
                    return true;
            }
        }

        #region cloudsat入库及查询
        /// <summary>
        /// CloudSAT云参数原始数据入库
        /// </summary>
        /// <param name="tableName"></param>数据库表名
        /// <param name="dataTime"></param>数据日期
        /// <param name="fName"></param>数据文件名
        /// <param name="localfName"></param>数据相对路径文件名
        /// <param name="granuleNO"></param>轨道数据分块号
        /// <param name="ProductsName"></param>数据产品名
        public long InsertCloudSAT2Table(string tableName, DateTime dataTime, string fName, string localfName, int granuleNO, string ProductName, string datatype)
        {
            lock (_obj)
            {
                long cloudsatID = 0;
                if (this.ConnOpend() || this.OpenConn())
                {
                    string sat = "CloudSAT";
                    //string dataSource = "Copy", dataQuality = "A";
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = string.Format("INSERT {0}(InputTime,ReceiveTime,ImageName,ImageData,Satellite,SensorType,GranuleNumber,ProductName,DataType,ProductsComments,DataSource,DataQuality) VALUES(@InputTime,@ReceiveTime,@ImageName,@ImageData,@Satellite,@SensorType,@GranuleNumber,@ProductName,@DataType,@ProductsComments,@DataSource,@DataQuality);SELECT LAST_INSERT_ID();", tableName);
                    cmd.Parameters.AddWithValue("?InputTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("?ReceiveTime", dataTime);
                    cmd.Parameters.AddWithValue("?ImageName", fName);
                    cmd.Parameters.AddWithValue("?ImageData", localfName);
                    cmd.Parameters.AddWithValue("?Satellite", sat);
                    cmd.Parameters.AddWithValue("?SensorType", "NA");
                    cmd.Parameters.AddWithValue("?GranuleNumber", granuleNO);
                    cmd.Parameters.AddWithValue("?ProductName", ProductName);
                    cmd.Parameters.AddWithValue("?DataType", datatype);
                    cmd.Parameters.AddWithValue("?ProductsComments", "");// _CloudSATRawPrdsComments[ProductName]);
                    cmd.Parameters.AddWithValue("?DataSource", "");//dataSource);
                    cmd.Parameters.AddWithValue("?DataQuality", "");//dataQuality);
                    cmd.ExecuteNonQuery();//CloudSatID
                    cloudsatID = cmd.LastInsertedId;
                    if (this.ConnOpend())
                        this.Closeconn();
                }
                return cloudsatID;
            }
        }
         
        #region cloudSAT与区域关联
        public void InsertCloudsat2RegionTable(long cloudSATID, long fileAllPointsCount, long regionID, double regionIDCoverPct)
        {
            lock (_obj)
            {
                //string tableName = "cp_cloudsat2region_tb";
                string sql;
                if (this.ConnOpend() || this.OpenConn())
                {
                    sql = "INSERT cp_cloudsat2region_tb (CloudSatID,FileAllPointsCount,DRID,DRIDPointsCount,DRIDCoverPct,InputTime) "
                        + " VALUES(@CloudSatID,@FileAllPointsCount,@DRID,@DRIDPointsCount,@DRIDCoverPct,@InputTime)";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("?CloudSatID", cloudSATID);
                    cmd.Parameters.AddWithValue("?FileAllPointsCount", fileAllPointsCount);
                    cmd.Parameters.AddWithValue("?DRID", regionID);
                    cmd.Parameters.AddWithValue("?DRIDPointsCount", (Int32)fileAllPointsCount * regionIDCoverPct / 100);
                    cmd.Parameters.AddWithValue("?DRIDCoverPct", regionIDCoverPct);
                    cmd.Parameters.AddWithValue("?InputTime", DateTime.Now);
                    cmd.ExecuteNonQuery();
                    if (this.ConnOpend())
                        this.Closeconn();
                }
            }
        }

        public void InsertCloudsatGranule2Region(int granuleNO, long regionID, float uplat, float uplon, float downlat, float downlon, int firstptIdx, int lastptIdx)
        {
            //granuleNO,regionid,uplat,uplon,downlat,downlon,firstpt,lastpt;
            lock (_obj)
            {
                string sql;
                if (this.ConnOpend() || this.OpenConn())
                {
                    sql = "INSERT cp_cloudsatinregionlatlon_tb (GranuleNumber,DRID,UpLatitude,UpLongitude,DownLatitude,DownLongitude,FirstInPtIndex,LastInPtIndex) "
                        + " VALUES(@GranuleNumber,@DRID,@UpLatitude,@UpLongitude,@DownLatitude,@DownLongitude,@FirstInPtIndex,@LastInPtIndex)";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("?GranuleNumber", granuleNO);
                    cmd.Parameters.AddWithValue("?DRID", regionID);
                    cmd.Parameters.AddWithValue("?UpLatitude", uplat);
                    cmd.Parameters.AddWithValue("?UpLongitude", uplon);
                    cmd.Parameters.AddWithValue("?DownLatitude", downlat);
                    cmd.Parameters.AddWithValue("?DownLongitude", downlon);
                    cmd.Parameters.AddWithValue("?FirstInPtIndex", firstptIdx);
                    cmd.Parameters.AddWithValue("?LastInPtIndex", lastptIdx);
                    cmd.ExecuteNonQuery();
                    if (this.ConnOpend())
                        this.Closeconn();
                }
            }
        }

        #endregion

        public void UpdateLink()
        {
            lock (_obj)
            {
            try
            {
                string tableName1 = "cp_cloudsat_tb", tableName2 = "cp_cloudsat2region_tb";
                string keyfieldName = "CloudSatID";
                if (this.ConnOpend() || this.OpenConn())
                {
                    string query = @"DELETE {0} FROM {0} LEFT JOIN {1} ON {0}.{2}={1}.{2} WHERE {1}.{2} IS NULL";
                    query = string.Format(query, tableName2, tableName1, keyfieldName);
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    if (this.ConnOpend())
                        this.Closeconn();
                }
            }
            catch (MySqlException ex)
            {
                ExceptionCatch(ex);
            }
            }
        }
         
        public bool QueryCloudSATPrdsInfo(string sensorName, out string[] setNames, out long[] setID)
        {
            lock (_obj)
            {
                setNames = new string[] { };
                setID = new long[] { };
                if (this.ConnOpend() || this.OpenConn())
                {
                    List<string> setNameList = new List<string>();
                    List<long> setIDList = new List<long>();
                    string tablename1 = "cp_cldprds2cloudsatbands_tb";
                    string sql = "SELECT  ProductsID,CloudSATDataPrds FROM " + tablename1 + " where ProductsID != 0";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        setIDList.Add(rdr.GetInt64(0));
                        setNameList.Add(rdr.GetString(1));
                    }
                    rdr.Close();
                    if (this.ConnOpend())
                        conn.Close();
                    setNames = setNameList.ToArray();
                    setID = setIDList.ToArray();
                }
                if (setNames.Length != 0)
                    return true;
                return false;
            }
        }        
        #endregion
        #region 投影范围入库与查询
        public long InsertRegionTable(string regionname, double lulon, double rdlon, double rdlat, double lulat)
        {
            lock(_obj)
            {
           //minx=lulon,maxx=rdlon,miny=rdlat,maxy=lulat;
            string sql;
            if (this.ConnOpend() || this.OpenConn())
            {
                sql = "INSERT cp_dataregion_tb (regionName,ULLatitude,ULLontitude,RDLatitude,RDLontitude,InputTime) VALUES(@regionName,@ULLatitude,@ULLontitude,@RDLatitude,@RDLontitude,@InputTime)";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("?regionName", regionname);
                cmd.Parameters.AddWithValue("?ULLatitude", lulat);
                cmd.Parameters.AddWithValue("?ULLontitude", lulon);
                cmd.Parameters.AddWithValue("?RDLatitude", rdlat);
                cmd.Parameters.AddWithValue("?RDLontitude", rdlon);
                cmd.Parameters.AddWithValue("?InputTime", DateTime.Now);
                object delId = cmd.ExecuteScalar();
                if (this.ConnOpend())
                    this.Closeconn();
                if (delId == null)
                    return 0;
                else
                    return long.Parse(delId + "");
            }
            return 0;
            }
        }

        public bool IshasDataRegionRecord(PrjEnvelopeItem prjItem, out long regionID)
        {
            lock (_obj)
            {
                regionID = 0;
                if (this.ConnOpend() || this.OpenConn())
                {
                    string tableName = "cp_dataregion_tb";
                    string query = "SELECT DRID From " + tableName + " where regionName = @fieldValue";
                    MySqlParameter[] param = new MySqlParameter[1];
                    param[0] = new MySqlParameter("@fieldValue", MySqlDbType.String);
                    param[0].Value = prjItem.Name.ToLower();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.Add(param[0]);
                    object delId = cmd.ExecuteScalar();
                    if (this.ConnOpend())
                        this.Closeconn();
                    if (delId == null)
                        return false;
                    else
                    {
                        regionID = long.Parse(delId + "");
                        return true;
                    }
                }
                else
                    return false;
            }
        }

        public bool QueryRegionEnvolop(string fieldValue, out double lulat, out double lulon, out double rdlat, out double rdlon)
        {
            lock (_obj)
            {
            lulon = 0; lulat = 0; rdlon = 0; rdlat = 0;
            if (this.ConnOpend() || this.OpenConn())
            {
                string tablename1 = "cp_dataregion_tb";
                //string sql = "SELECT Prds2AIRSID,AIRSDataSetName,AIRSDataSetComments,ProductsID,FillValue  FROM " + tablename;// +" where AIRSDataSetName = @setName";
                string sql = "SELECT regionName,ULLatitude,ULLontitude,RDLatitude,RDLontitude FROM " + tablename1;// + " where {0}= @fieldValue";// +" where AIRSDataSetName = @setName";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr.GetString(0) == fieldValue)
                    {
                        lulat = rdr.GetDouble(1);
                        lulon = rdr.GetDouble(2);
                        rdlat = rdr.GetDouble(3);
                        rdlon = rdr.GetDouble(4);
                    }
                }
                rdr.Close();
                if (this.ConnOpend())
                    conn.Close();
            }
            if (lulat != 0)
                return true;
            return false;
            }
        }

        public bool QueryRegionID(string regionname, out int drID)
        {
            lock (_obj)
            {
                drID = 0;
                if (this.ConnOpend() || this.OpenConn())
                {
                    string tablename1 = "cp_dataregion_tb";
                    //string sql = "SELECT Prds2AIRSID,AIRSDataSetName,AIRSDataSetComments,ProductsID,FillValue  FROM " + tablename;// +" where AIRSDataSetName = @setName";
                    string sql = "SELECT DRID  FROM " + tablename1 + " where RegionName= @regionName";// +" where AIRSDataSetName = @setName";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("?regionName", regionname);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        drID = rdr.GetInt32(0);
                        break;
                    }
                    rdr.Close();
                    if (this.ConnOpend())
                        conn.Close();
                }
                if (drID != 0)
                    return true;
                return false;
            }
        }

        public Dictionary<long, PrjEnvelopeItem> GetAllRegion()
        {
            lock (_obj)
            {
                Dictionary<long, PrjEnvelopeItem> allRegions = new Dictionary<long, PrjEnvelopeItem>();
                if (this.ConnOpend() || this.OpenConn())
                {
                    double lulat, lulon, rdlat, rdlon;
                    string tablename1 = "cp_dataregion_tb";
                    //string sql = "SELECT Prds2AIRSID,AIRSDataSetName,AIRSDataSetComments,ProductsID,FillValue  FROM " + tablename;// +" where AIRSDataSetName = @setName";
                    string sql = "SELECT DRID,regionName,ULLatitude,ULLontitude,RDLatitude,RDLontitude FROM " + tablename1;// + " where {0}= @fieldValue";// +" where AIRSDataSetName = @setName";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        lulat = rdr.GetDouble(2);
                        lulon = rdr.GetDouble(3);
                        rdlat = rdr.GetDouble(4);
                        rdlon = rdr.GetDouble(5);
                        PrjEnvelopeItem item = new PrjEnvelopeItem(rdr.GetString(1), new PrjEnvelope(lulon, rdlon, rdlat, lulat));
                        allRegions.Add(rdr.GetInt64(0), item);
                    }
                    rdr.Close();
                    if (this.ConnOpend())
                        conn.Close();
                }
                return allRegions;
            }
        }
        #endregion

    }
}

