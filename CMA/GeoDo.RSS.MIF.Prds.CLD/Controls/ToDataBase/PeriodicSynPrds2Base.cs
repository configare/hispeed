using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Collections;
using GeoDo.RSS.Core.DF;


namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class PeriodicSynPrds2Base
    {
        private static string _inputDIR;
        public string _tableName = "cp_periodicsynthesis_tb";
        public Dictionary<string, int> _statName2ID = new Dictionary<string, int>();
        public Dictionary<string, int> _periodTypeID = new Dictionary<string, int>();
        private Dictionary<string, int> _regionID = new Dictionary<string, int>();
        string _logfName;
        private static ConnectMySqlCloud _dbConnect;

        public PeriodicSynPrds2Base(string dir, string logfName, ConnectMySqlCloud dbConnect)
        {
            _inputDIR = dir;
            _logfName = logfName;
            //_dbxml = dbxml;
            _dbConnect = dbConnect;
            _statName2ID.Add("MAX", 1);
            _statName2ID.Add("MIN", 2);
            _statName2ID.Add("AVG", 3);
            _periodTypeID.Add("day", 2);
            _periodTypeID.Add("ten", 3);
            _periodTypeID.Add("month", 4);
            _periodTypeID.Add("year", 5);
        }

        public void StartComp()
        {
            try
            {
                //CloudTopTemperature-MOD06-china-Ten-2011_1_1-AVG.LDF
                //CloudTopTemperature-MOD06-china-Month-2011_1-AVG.LDF
                //CloudTopTemperature-MOD06-china-Year-2011-AVG.LDF
                List<string> periodprdsfiles = Directory.GetFiles(_inputDIR, "*.ldf", SearchOption.AllDirectories).ToList();
                CheckFilesValid(ref periodprdsfiles);
                if (periodprdsfiles.Count > 0)
                {
                    foreach (string file in periodprdsfiles)
                    {
                        TryCreatePeriodRecord(file);
                    }
                }
                return;
            }
            catch (System.Exception ex)
            {
                LogFactory.WriteLine(_logfName, ex.Message);
            }
            finally
            {
            }
        }

        private void CheckFilesValid(ref List<string> fileList)
        {
            foreach (string file in fileList)
            {
                //CloudTopTemperature-MOD06-china-Ten-2011_1_1-AVG.LDF
                //CloudTopTemperature-MOD06-china-Month-2011_1-AVG.LDF
                //CloudTopTemperature-MOD06-china-Year-2011-AVG.LDF
                if (Path.GetFileNameWithoutExtension(file).Split('_').Length != 6)
                {
                    fileList.Remove(file);
                }
            }
        }

        public void TryCreatePeriodRecord(string file,Action<int, string> progressCallback=null)
        {
            string fname, localfname, region, sensor = "", statName, datasetname, period;
            string[] parts;
            long prdID = 0, peridID, staticsID, datasetID;
            int year = 0, month = 0, xun = 0, regionID;
            //double validPercent;
            //CloudTopTemperature_MOD06_china_Ten_2011-11-2_AVG_day_0.05.LDF
            //CloudTopTemperature_MOD06_china_Month_2011-11_AVG_day_0.05.LDF
            //CloudTopTemperature_MOD06_china_Year_2011_AVG_day_0.05.LDF
            if (progressCallback != null)
                progressCallback(-1, "\t\t\t\t入库开始...," + file);
            fname = Path.GetFileNameWithoutExtension(file);
            localfname = file.Replace(_inputDIR, "");
            parts = fname.Split('_');
            period = parts[3].ToLower();
            if (period != "ten" && period != "month" && period != "year")
                return;
            if (!_periodTypeID.ContainsKey(period))
                return;
            peridID = _periodTypeID[period];//周期类型
            string sta = parts[5].ToUpper();
            if (sta != "MIN" && sta != "MAX" && sta != "AVG")
                return;
            statName = sta;
            staticsID = _statName2ID[sta];//统计类型
            string label = parts[1].ToUpper();
            if (label == "MOD06" || label == "MYD06" || label == "AIRS")
                sensor = label;
            else
                return;
            datasetname = parts[0];
            if (_dbConnect.QueryDatasetsID(sensor, datasetname, out prdID, out datasetID))
            {
                region = parts[2];
                if (!_regionID.Keys.Contains(region))
                {
                    if (!_dbConnect.QueryRegionID(region.ToLower(), out regionID))
                        return;
                    _regionID.Add(region, regionID);
                }
                else
                    regionID = _regionID[region];
                if (regionID == 0)
                    regionID = 1;
                if (period == "ten")
                {
                    Regex XunDateReg = new Regex(@"(?<year>\d{4})-(?<month>\d{2})-(?<Xun>\d{1})", RegexOptions.Compiled);
                    Match match = XunDateReg.Match(parts[4]);
                    if (!match.Success)
                        return;
                    year = Int16.Parse(match.Groups["year"].Value);
                    month = Int16.Parse(match.Groups["month"].Value);
                    xun = Int16.Parse(match.Groups["Xun"].Value);
                }
                else if (period == "month")
                {
                    Regex MonthDateReg = new Regex(@"(?<year>\d{4})-(?<month>\d{2})", RegexOptions.Compiled);
                    Match match = MonthDateReg.Match(parts[4]);
                    if (!match.Success)
                        return;
                    year = Int16.Parse(match.Groups["year"].Value);
                    month = Int16.Parse(match.Groups["month"].Value);
                }
                else//period == "year"
                {
                    year = Int16.Parse(parts[4]);
                }
                //GetRsEnvolop(file, out resol, out validPercent);
                float resol = float.Parse(parts[parts.Length - 1]);
                string datasource = "";
                if (parts[parts.Length-2].ToLower() == "day")
                    datasource = "D";
                else
                    datasource = "N";
                if (!_dbConnect.IshasRecord(_tableName, "ImageName", Path.GetFileName(file)))
                {
                    _dbConnect.InsertPeriodicSynProductsTable(prdID, datasetID, localfname, regionID, region, sensor, resol, peridID, year, month, xun, _statName2ID[statName],datasource);
                }
                else
                {
                    _dbConnect.DeleteCLDParatableRecord(_tableName, "ImageName", Path.GetFileName(file));
                    _dbConnect.InsertPeriodicSynProductsTable(prdID, datasetID, localfname, regionID, region, sensor, resol, peridID, year, month, xun, _statName2ID[statName], datasource);
                }
            }
        }

        public static void GetRsEnvolop(string fname, out float resl, out double validPercent)
        {
            validPercent = 0;
            using (IRasterDataProvider dataprd = GeoDataDriver.Open(fname) as IRasterDataProvider)
            {
                resl = dataprd.ResolutionX;
                return;
            }
        }
    }
}
