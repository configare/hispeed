using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using GeoDo.MicroWaveSnow.SNWParaStat;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    public class TSANRetrieval
    {
        private Dictionary<string, object> _arguments = null;
        private static Dictionary<string, int> _periodTypesID = null;
        private string _periodType,_statType, _productType;
        private string[] _orbiTypes = null;
        private int _startYear, _endyear, _startMonth, _endMonth, _startTen, _endTen, _startPentad, _endPentad, _periodTypeID;
        private string[] _sensor = new string[] { "SSMI", "MWRI" };
        private List<string> _retrievFiles = null;
        public string SqlAppended { get; set; }
        private static SNWParaStat _snwParaStat = new SNWParaStat();
        private Dictionary<string, List<string>> _retrievAvgFiles = null;
        DBConnect db = new DBConnect();
        private bool _IsPeriodical = true;

        public TSANRetrieval(Dictionary<string, object> arguments)
        {
            if (arguments == null)
                return;
            _arguments = arguments;
            InitParas();
        }

        private void InitParas()
        {
            if (_periodTypesID != null)
                _periodTypesID.Clear();
            _periodTypesID = new Dictionary<string, int>();
            _periodTypesID.Add("pentad", 1);
            _periodTypesID.Add("ten", 2);
            _periodTypesID.Add("month", 3);
            _periodTypesID.Add("winter", 3);
            _productType = _arguments["prdTypes"] as string;
            _orbiTypes = _arguments["orbTypes"] as string[];
            _periodType = _arguments["periodTypes"] as string;
            string pt = _arguments["queryTypes"] as string;
            if (pt == "continue")
                _IsPeriodical = false;
            _periodTypeID = _periodTypesID[_periodType.ToLower()];
            _statType = _arguments["statTypes"] as string;
            ParseTimeParas();
        }

        private void ParseTimeParas()
        {
            string[] startEndyears = _arguments["startEndyears"] as string[];
            _startYear=int.Parse(startEndyears[0]);
            _endyear = int.Parse(startEndyears[1]);
            string[] startEndTime = _arguments["startEndTime"] as string[];
            string [] splittime =startEndTime[0].Split('_');
            _startMonth=int.Parse(splittime[0]);
             _startTen=int.Parse(splittime[1]);
             _startPentad = int.Parse(splittime[2]);
             splittime = startEndTime[1].Split('_');
             _endMonth = int.Parse(splittime[0]);
             _endTen = int.Parse(splittime[1]);
             _endPentad = int.Parse(splittime[2]);     
       
        }

        private StringBuilder GenerateSQL(string sensor, bool isWinter = false)
        {
            return ConcatPeriod(sensor,_periodType,_statType,_productType,isWinter);
        }

        //拼接周期产品sql语句
        public StringBuilder ConcatPeriod(string sensor, string periodType, string statType, string productType,bool isWinter=false)
        {
            string imagedata = "AImageData, DImageData";
            StringBuilder sql = new StringBuilder();
            sql.Append("select ").Append(imagedata).Append(" from sd_pstat_tb");
            sql.Append(" where ");
            AppendOr(ref sql, "PeriodType", _periodTypeID.ToString());
            sql.Append(" and ");
            AppendOr(ref sql, "Sensor",sensor);
            sql.Append(" and ");
            AppendOr(ref sql, "StatType", statType);
            sql.Append(" and ");
            AppendOr(ref sql, "ProductType", productType);
            sql.Append(" and ");
            if (!isWinter)
            {
                AppendYear(ref sql, _startYear, _endyear);
                sql.Append(" and ");
            }
            return sql;
        }

        public static void AppendOr(ref StringBuilder sql,string field, string value)
        {
            sql.Append(field).Append("=").Append("'").Append(value).Append("'");
        }

        public Dictionary<string, List<string>> DoRetrievalAndCompute()
        {
            #region 查询数据
            StringBuilder sqlhdr= null;
            if (!_IsPeriodical)
                return JustRetrieval();
            switch(_periodType.ToLower())
            {
                case "winter":
                    sqlhdr = GenerateSQL(_sensor[0],true);
                    DoRetrievalAndComputeWinter(sqlhdr);
                    break;
                case "month":
                    sqlhdr = GenerateSQL(_sensor[0]);
                    DoRetrievalAndComputeMonthly(sqlhdr);
                    break;
                case "pentad":
                    sqlhdr = GenerateSQL(_sensor[0]);
                    DoRetrievalAndCompute510Days(sqlhdr,"Hou");
                    break;
                case "ten":
                    sqlhdr = GenerateSQL(_sensor[0]);
                    DoRetrievalAndCompute510Days(sqlhdr,"Xun");
                    break;
            }
            return _retrievAvgFiles;
            #endregion
        }

        #region 查询+计算
        private void DoRetrievalAndComputeMonthly(StringBuilder sql)
        {
            StringBuilder monthsql = null;
            string outAvgRetrievFname = null;
            string outputdir = Path.Combine(db.Output, "MWS\\纵向同期_横向连续统计\\");
            if (!Directory.Exists(outputdir))
                Directory.CreateDirectory(outputdir);
            if (_retrievAvgFiles != null)
                _retrievAvgFiles.Clear();
            for (int fm = _startMonth; fm <= _endMonth;fm++ )
            {
                try
                {
                    monthsql = new StringBuilder(sql.ToString());
                    monthsql.Append("ExtractMonth=").Append(Convert.ToInt32(fm));
                    if (_retrievFiles != null)
                        _retrievFiles.Clear();
                    else
                        _retrievFiles = new List<string>();
                    db.Retrieval(monthsql.ToString(), SaveRetrieveHistory);
                    if (_retrievFiles == null || _retrievFiles.Count == 0)
                    {
                        monthsql = GenerateSQL(_sensor[1]);
                        monthsql.Append("ExtractMonth=").Append(Convert.ToInt32(fm));
                        if (_retrievFiles != null)
                            _retrievFiles.Clear();
                        else
                            _retrievFiles = new List<string>();
                        db.Retrieval(monthsql.ToString(), SaveRetrieveHistory);
                    }
                    //历史同期平均
                    if (_retrievFiles != null && _retrievFiles.Count > 0)
                    {
                        outAvgRetrievFname = Path.Combine(outputdir, CreateSamePeriodAvgOutName(_retrievFiles, "Month"));
                        _snwParaStat.SNWParaAvgStat(_retrievFiles.ToArray(), 0.1f, outAvgRetrievFname);
                    }
                    else
                        continue;
                    if (_retrievAvgFiles == null)
                    {
                        _retrievAvgFiles = new Dictionary<string, List<string>>();
                        if (File.Exists(outAvgRetrievFname))
                            _retrievAvgFiles.Add(outAvgRetrievFname, new List<string>(_retrievFiles.ToArray()));
                    }
                    else if (!_retrievAvgFiles.ContainsKey(outAvgRetrievFname))
                    {
                        if (File.Exists(outAvgRetrievFname))
                            _retrievAvgFiles.Add(outAvgRetrievFname, new List<string>(_retrievFiles.ToArray()));
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static void AppendYear(ref StringBuilder sql, int begin, int end)
        {
            sql.Append("ExtractYear>=").Append(Convert.ToInt32(begin));
            sql.Append(" and ");
            sql.Append("ExtractYear<=").Append(Convert.ToInt32(end));
        }

        private void SaveRetrieveHistory(MySqlDataReader dataReader)
        {
            string AImageData,DImageData;
            string dbdir = db.PeriodDayDir;
            while (dataReader.Read())
            {
				if(_orbiTypes.Length==2)
                {
                    AImageData = dbdir + dataReader["AImageData"].ToString();
                    if (AImageData != "" && !_retrievFiles.Contains(AImageData) && File.Exists(AImageData))
                        _retrievFiles.Add(AImageData);
                    DImageData = dbdir + dataReader["DImageData"].ToString();
                    if (DImageData != "" && !_retrievFiles.Contains(DImageData) && File.Exists(DImageData))
                        _retrievFiles.Add(DImageData);
                }
                else if (_orbiTypes[0] == "ASD")
                {
                    AImageData = dbdir + dataReader["AImageData"].ToString();
                    if (AImageData != "" && !_retrievFiles.Contains(AImageData) && File.Exists(AImageData))
                        _retrievFiles.Add(AImageData);
                }
                else if (_orbiTypes[0] == "DSD")
                {
                    DImageData = dbdir + dataReader["DImageData"].ToString();
                    if (DImageData != "" && !_retrievFiles.Contains(DImageData) && File.Exists(DImageData))
                        _retrievFiles.Add(DImageData);
                }
            }
        }

        /// <summary>
        /// 同期或者冬季统计文件名生成
        /// </summary>
        /// <param name="al"></param>
        /// <param name="periodType"></param>
        /// <returns></returns>
        public  string CreateSamePeriodAvgOutName(List<string> al,string periodType,int year =0,bool isSameYear=false)
        {
            StringBuilder outname = new StringBuilder();
            if (al.Count != 0)
            {
                if (al.Count == 1 && periodType!="Winter")
                {
                    //统计1个文件
                    //返回它本身的文件名，比如MWS_MWSD_China_Hou_0SD_D_1987_1_1_avg.dat
                    outname.Append(Path.GetFileName(al[0].ToString()));
                    return outname.ToString();
                }
                string orbitype = null;
                if (_orbiTypes.Length == 2)
                    orbitype = "AD";
                else if (_orbiTypes[0] == "ASD")
                    orbitype = "A";
                else if (_orbiTypes[0] == "DSD")
                    orbitype = "D";
                string[] splits1 = Path.GetFileName(al[0].ToString()).Split('_');
                string[] splits2 = Path.GetFileName(al[al.Count - 1].ToString()).Split('_');
                if (periodType == "Winter")
                    outname.Append(splits1[0]).Append("_").Append(splits1[1]).Append("_").Append(splits1[2]).Append("_").Append("Winter").Append("_").Append(splits1[4]).Append("_").Append(orbitype).Append("_");
                else
                    outname.Append(splits1[0]).Append("_").Append(splits1[1]).Append("_").Append(splits1[2]).Append("_").Append(splits1[3]).Append("_").Append(splits1[4]).Append("_").Append(orbitype).Append("_");
                StringBuilder postfix1 = new StringBuilder();
                StringBuilder postfix2 = new StringBuilder();
                switch (periodType)
                {
                    case "Hou":
                    case "Xun":
                        //统计2个文件,
                        //输入：MWS_MWSD_China_Hou_0SD_A_2011_1_1_min.dat
                        //如果前后文件名除了年之外周期一致则为同期统计，
                        //输出：MWS_MWSD_China_Hou_0SD_D_1987_2014_1_1_avg.dat，表示1987到2014年的1月1候的同期统计
                        postfix1.Append(splits1[7]).Append("_").Append(splits1[8]);//月、旬/侯
                        postfix2.Append(splits2[7]).Append("_").Append(splits2[8]);//月、旬/侯
                        if (postfix1.ToString() == postfix2.ToString())//仅年际变化
                        {
                            outname.Append(splits1[6]).Append("_").Append(splits2[6]).Append("_").Append(postfix1).Append("_").Append(splits1[9]);
                        }
                        else
                        {
                            outname.Append(splits1[6]).Append("_").Append(postfix1).Append("_").Append(splits2[6]).Append("_").Append(postfix2).Append("_").Append(splits1[9]);
                        }
                        break;
                    case "Month":
                        //统计2个文件,
                        //输入：MWS_MWSD_China_Month_0SD_A_2011_1_max.dat
                        //如果前后文件名除了年之外周期一致则为同期统计，
                        //输出：MWS_MSWE_China_Month_SWE_D_1987_2013_12_avg.dat，表示1987到2014年的12月的同期统计
                        postfix1.Append(splits1[7]);//月、季
                        postfix2.Append(splits2[7]);//月、季
                        if (postfix1.ToString() == postfix2.ToString())//仅年际变化
                        {
                            outname.Append(splits1[6]).Append("_").Append(splits2[6]).Append("_").Append(postfix1).Append("_").Append(splits1[8]);
                        }
                        else
                        {
                            outname.Append(splits1[6]).Append("_").Append(postfix1).Append("_").Append(splits2[6]).Append("_").Append(postfix2).Append("_").Append(splits1[8]);
                        }
                        break;
                    case "Winter":
                        //统计2个文件,
                        //输入：MWS_MWSD_China_Month_0SD_A_2011_1_max.dat
                        //如果前后文件名除了年之外周期一致则为同期统计，
                        //输出：MWS_MSWE_China_Winter_SWE_D_1987_11_1988_2_avg.dat	，表示1987年11月到2013年的2月的冬季统计
                        outname.Append(year).Append("_").Append(_startMonth).Append("_").Append(isSameYear?year:++year).Append("_").Append(_endMonth).Append("_").Append(splits1[8]);
                        break;
                }
            }
            return outname.ToString();
        }

        private void DoRetrievalAndComputeWinter(StringBuilder sql)
        {
            string outAvgRetrievFname = null;
            string outputdir = Path.Combine(db.Output, "MWS\\纵向同期_横向连续统计\\");
            if (!Directory.Exists(outputdir))
                Directory.CreateDirectory(outputdir);
            if (_retrievAvgFiles != null)
                _retrievAvgFiles.Clear();
            if (_startMonth <= _endMonth)
            {
                //同年情况
                for (int year = _startYear; year <= _endyear; ++year)
                {
                    try
                    {
                        StringBuilder newSql = new StringBuilder(sql.ToString());
                        newSql.Append(" ExtractYear=").Append(year).Append(" and (");
                        for (int month = _startMonth; month <= _endMonth; ++month)
                        {
                            newSql.Append("ExtractMonth=").Append(month);
                            if (month != _endMonth)
                                newSql.Append(" or ");
                        }
                        newSql.Append(");");
                        if (_retrievFiles != null)
                            _retrievFiles.Clear();
                        else
                            _retrievFiles = new List<string>();
                        db.Retrieval(newSql.ToString(), SaveRetrieveHistory);
                        //历史同期平均
                        if (_retrievFiles == null || _retrievFiles.Count <= 0)
                            continue;
                        outAvgRetrievFname = Path.Combine(outputdir, CreateSamePeriodAvgOutName(_retrievFiles, "Winter",year,true));
                        _snwParaStat.SNWParaAvgStat(_retrievFiles.ToArray(), 0.1f, outAvgRetrievFname);
                        if (File.Exists(outAvgRetrievFname))
                        {
                            if (_retrievAvgFiles == null)
                            {
                                _retrievAvgFiles = new Dictionary<string, List<string>>();
                                _retrievAvgFiles.Add("Winter", new List<string> { outAvgRetrievFname });
                            }
                            else if (_retrievAvgFiles.ContainsKey("Winter"))
                            {
                                _retrievAvgFiles["Winter"].Add(outAvgRetrievFname);
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            else
            {
                //跨年情况
                for (int year = _startYear; year <= _endyear; ++year)
                {
                    try
                    {
                        StringBuilder newSql1 = new StringBuilder(sql.ToString());
                        newSql1.Append(" ExtractYear=").Append(year).Append(" and (");

                        for (int month = _startMonth; month <= 12; ++month)
                        {
                            newSql1.Append("ExtractMonth=").Append(month);
                            if (month != 12)
                                newSql1.Append(" or ");
                        }
                        newSql1.Append(")");
                        StringBuilder newSql2 = new StringBuilder(sql.ToString());
                        newSql2.Append(" ExtractYear=").Append(year + 1).Append(" and (");
                        for (int month = 1; month <= _endMonth; ++month)
                        {
                            newSql2.Append("ExtractMonth=").Append(month);
                            if (month != _endMonth)
                                newSql2.Append(" or ");
                        }
                        newSql2.Append(")");
                        StringBuilder newSql = new StringBuilder();
                        newSql.Append(newSql1.ToString()).Append(" union all ").Append(newSql2.ToString()).Append(";");
                        if (_retrievFiles == null)
                            _retrievFiles = new List<string>();
                        db.Retrieval(newSql.ToString(), SaveRetrieveHistory);
                        //历史同期平均
                        if (_retrievFiles == null || _retrievFiles.Count <= 0)
                            continue;
                        outAvgRetrievFname = Path.Combine(outputdir, CreateSamePeriodAvgOutName(_retrievFiles, "Winter", year, false));
                        _snwParaStat.SNWParaAvgStat(_retrievFiles.ToArray(), 0.1f, outAvgRetrievFname);
                        if (File.Exists(outAvgRetrievFname))
                        {
                            if (_retrievAvgFiles == null)
                            {
                                _retrievAvgFiles = new Dictionary<string, List<string>>();
                                _retrievAvgFiles.Add("Winter", new List<string> { outAvgRetrievFname });
                            }
                            else if (_retrievAvgFiles.ContainsKey("Winter"))
                            {
                                _retrievAvgFiles["Winter"].Add(outAvgRetrievFname);
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        private void DoRetrievalAndCompute510Days(StringBuilder sql, string periodType)
        {
            StringBuilder monthsql = null;
            string outAvgRetrievFname = null;
            string outputdir = Path.Combine(db.Output, "MWS\\纵向同期_横向连续统计\\");
            if (!Directory.Exists(outputdir))
                Directory.CreateDirectory(outputdir);
            if (_retrievAvgFiles != null)
                _retrievAvgFiles.Clear();
            int start, end;
            string sqlAppend = null; 
            if (periodType=="Hou")
            {
                start = _startPentad;
                end = _endPentad;
                sqlAppend = "Extract5Day = ";
            }
            else
            {
                start = _startTen;
                end = _endTen;
                sqlAppend = "Extract10Day = ";
            }
            for (int fm = start; fm <= end; fm++)
            {
                try
                {
                    monthsql = new StringBuilder(sql.ToString());
                    monthsql.Append("ExtractMonth=").Append(Convert.ToInt32(_startMonth)).Append(" and ");
                    monthsql.Append(sqlAppend).Append(Convert.ToInt32(fm));
                    if (_retrievFiles != null)
                        _retrievFiles.Clear();
                    else
                        _retrievFiles = new List<string>();
                    db.Retrieval(monthsql.ToString(), SaveRetrieveHistory);
                    if (_retrievFiles == null || _retrievFiles.Count == 0)
                    {
                        monthsql = GenerateSQL(_sensor[1]);
                        monthsql.Append("ExtractMonth=").Append(Convert.ToInt32(_startMonth)).Append(" and ");
                        monthsql.Append(sqlAppend).Append(Convert.ToInt32(fm));
                        if (_retrievFiles != null)
                            _retrievFiles.Clear();
                        else
                            _retrievFiles = new List<string>();
                        db.Retrieval(monthsql.ToString(), SaveRetrieveHistory);
                    }
                    //历史同期平均
                    if (_retrievFiles != null && _retrievFiles.Count > 0)
                    {
                        outAvgRetrievFname = Path.Combine(outputdir, CreateSamePeriodAvgOutName(_retrievFiles, periodType));
                        _snwParaStat.SNWParaAvgStat(_retrievFiles.ToArray(), 0.1f, outAvgRetrievFname);
                    }
                    else
                        continue;
                    if (_retrievAvgFiles == null)
                    {
                        _retrievAvgFiles = new Dictionary<string, List<string>>();
                        if (File.Exists(outAvgRetrievFname))
                            _retrievAvgFiles.Add(outAvgRetrievFname, new List<string>(_retrievFiles.ToArray()));
                    }
                    else if (!_retrievAvgFiles.ContainsKey(outAvgRetrievFname))
                    {
                        if (File.Exists(outAvgRetrievFname))
                            _retrievAvgFiles.Add(outAvgRetrievFname, new List<string>(_retrievFiles.ToArray()));
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
#endregion
        #region 只查询不计算
        private Dictionary<string, List<string>> JustRetrieval()
        {
            StringBuilder sqlhdr = GenerateSQL(_sensor[0], true);
            List<string> sqlList = null;
            switch (_periodType.ToLower())
            {
                case "month":
                    sqlList = GenerateRetrievalSqlMonth(sqlhdr);
                    break;
                case "pentad":
                case "ten":
                    sqlList = GenerateRetrievalSql510day(sqlhdr);
                    break;
                default:
                    break;
            }
            if (sqlList == null || sqlList.Count == 0)
                return null;
            if (_retrievAvgFiles != null)
                _retrievAvgFiles.Clear();
            string outputdir = Path.Combine(db.Output, "MWS\\纵向同期_横向连续统计\\");
            if (!Directory.Exists(outputdir))
                Directory.CreateDirectory(outputdir);
            string outAvgRetrievFname=null;
            foreach (string subsql in sqlList)
            {
                try
                {
                    if (_retrievFiles != null)
                        _retrievFiles.Clear();
                    else
                        _retrievFiles = new List<string>();
                    db.Retrieval(subsql, SaveRetrieveHistory);
                    if (_retrievFiles == null || _retrievFiles.Count == 0)
                    {
                        string newsubsql = subsql.Replace(_sensor[0], _sensor[1]);
                        if (_retrievFiles != null)
                            _retrievFiles.Clear();
                        else
                            _retrievFiles = new List<string>();
                        db.Retrieval(newsubsql.ToString(), SaveRetrieveHistory);
                    }
                    if (_retrievFiles == null && _retrievFiles.Count == 0)
                        continue;
                    if (_orbiTypes.Length==1)
                    {
                        if (_retrievAvgFiles == null)
                        {
                            _retrievAvgFiles = new Dictionary<string, List<string>>();
                            _retrievAvgFiles.Add("continue", new List<string>(_retrievFiles.ToArray()));
                        }
                        else if (_retrievAvgFiles.ContainsKey("continue"))
                            _retrievAvgFiles["continue"].AddRange(_retrievFiles.ToArray());
                        else
                            _retrievAvgFiles.Add("continue", new List<string>(_retrievFiles.ToArray()));
                    }
                    else
                    {
                        //MWS_MWSD_China_Month_0SD_A_2011_1_max.dat
                        string dfile=null;
                        string[] ad = new string[] { "_A_", "_D_" };
                        foreach (string afile in _retrievFiles)
                        {
                            if (Path.GetFileName(afile).Contains("_A_"))
                            {
                                outAvgRetrievFname = Path.GetFileName(afile).Replace("_A_", "_AD_");
                                dfile = Path.Combine(Path.GetDirectoryName(afile).Replace("Ascend", "Descend"), Path.GetFileName(afile).Replace("_A_", "_D_"));
                            }
                            else if (Path.GetFileName(afile).Contains("_D_"))
                            {
                                outAvgRetrievFname = Path.GetFileName(afile).Replace("_D_", "_AD_");
                                dfile = Path.Combine(Path.GetDirectoryName(afile).Replace("Descend", "Ascend"), Path.GetFileName(afile).Replace("_D_", "_A_"));
                            }
                            else
                                continue;
                            outAvgRetrievFname = Path.Combine(outputdir, outAvgRetrievFname);
                            if (!File.Exists(outAvgRetrievFname))
                            {
                                if (_retrievFiles.Contains(dfile))
                                    _snwParaStat.SNWParaAvgStat(new string[] { afile, dfile }, 0.1f, outAvgRetrievFname);
                                else
                                    File.Copy(afile, outAvgRetrievFname);
                            }
                            AddRetrievAvgFiles("continue", outAvgRetrievFname);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return  _retrievAvgFiles;
        }

        private void AddRetrievAvgFiles(string key,string file)
        {
            if (_retrievAvgFiles == null)
            {
                _retrievAvgFiles = new Dictionary<string, List<string>>();
                _retrievAvgFiles.Add("continue", new List<string>(new string []{file}));
            }
            else if (_retrievAvgFiles.ContainsKey("continue"))
            {
                if (!_retrievAvgFiles["continue"].Contains(file))
                    _retrievAvgFiles["continue"].AddRange(new string[] { file });
            }
            else
                _retrievAvgFiles.Add("continue", new List<string>(new string[] { file }));

        }

        private List<string> GenerateRetrievalSql510day(StringBuilder sql)
        {
            int start, end;
            string sqlAppend = null;
            if (_periodType.ToLower() == "pentad")
            {
                start = _startPentad;
                end = _endPentad;
                sqlAppend = "Extract5Day ";
            }
            else
            {
                start = _startTen;
                end = _endTen;
                sqlAppend = "Extract10Day ";
            }
            List<string> sqlList = new List<string>();
            sql.Append("(ExtractYear>=").Append(_startYear).Append(" and ").Append("ExtractYear<=").Append(_endyear).Append(") ");
            sql.Append(Append510Day(_startMonth, _endMonth, sqlAppend, start, end));
            sqlList.Add(sql.ToString());
            return sqlList;
        }

        private string Append510Day(int monthstart,int monthend,string append,int start,int end)
        {
            StringBuilder sql = new StringBuilder();
            if (monthstart == monthend)
            {
                //同月，monthstart到monthend
                sql.Append(" and ExtractMonth=").Append(monthstart).Append(" and " + append + ">=").Append(start).Append(" and " + append + "<=").Append(end).Append(";");
            }
            else if (monthstart + 1 == monthend)
            {
                //两月，monthstart的start到末，以及monthend的1到end
                sql.Append("and ((ExtractMonth=").Append(monthstart).Append(" and " + append + ">=").Append(start).Append(") or ");
                StringBuilder endSQL = new StringBuilder();
                endSQL.Append("(ExtractMonth=").Append(monthend).Append(" and " + append + "<=").Append(end).Append("));");
                sql.Append(endSQL.ToString());
            }
            else if (monthstart + 1 < monthend)
            {
                //三月（含）以上，monthstart的start到末，中间的整月，以及monthend的1到end
                StringBuilder midSQL = new StringBuilder();
                midSQL.Append("(ExtractMonth>=").Append(monthstart + 1).Append(" and ").Append("ExtractMonth<=").Append(monthend - 1).Append(") or ");
                StringBuilder endSQL = new StringBuilder();
                endSQL.Append("(ExtractMonth=").Append(monthend).Append(" and " + append + "<=").Append(end).Append("));");
                sql.Append("and ((ExtractMonth=").Append(monthstart).Append(" and " + append + ">=").Append(start).Append(") or ");
                sql.Append(midSQL.ToString());
                sql.Append(endSQL.ToString());
            }
            return sql.ToString();
        }

        private List<string> GenerateRetrievalSqlMonth(StringBuilder sql)
        {
            if (_startYear == _endyear)
            {
                //同年，_startMonth到_endMonth
                sql.Append(" ExtractYear=").Append(_startYear).Append(" and ExtractMonth>=").Append(_startMonth).Append(" and ExtractMonth<=").Append(_endMonth).Append(";");
            }
            else 
            {
                sql.Append(" ExtractMonth>=").Append(_startMonth).Append(" and ExtractMonth<=").Append(_endMonth);
                sql.Append(" and (ExtractYear>=").Append(_startYear).Append(" and ").Append("ExtractYear<=").Append(_endyear).Append(");");
            }
            return new List<string> { sql.ToString() };
        }


        #endregion
    }
}
