using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DF;
using OSGeo.GDAL;
using System.IO;
using GeoDo.HDF4;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;
using GeoDo.HDF;

namespace GeoDo.RSS.DF.GDAL.H4BandPrd
{
    /// <summary>
    /// ---------------------------------------------------
    /// 2014.01.25,修改内容如下：
    /// 支持NASA网站下载MODIS数据。
    /// 详情：
    /// NASA网站MODIS数据的命名规则是：
    /// 形如：MYD02HKM.A2014055.0850.005.2014055183303.hdf
    /// MOD021KM.AYYYYDDD.HHMM.VVV.YYYYDDDHHMMSS.hdf
    /// A=Data Date
    /// YYYYDDD=Data Year and Julian Date
    /// HHMM=Data Hour & Minute Start Time
    /// VVV = Collection Version
    /// YYYYDDDHHMMSS=Production Date& Time
    /// hdf = Suffix Denoting HDF file
    /// Note that:
    ///     all times are UTC time, not local time
    /// ---------------------------------------------------
    /// </summary>
    [Export(typeof(IBandProvider)), ExportMetadata("VERSION", "1")]
    public class BandProviderHDF4 : BandProvider
    {
        protected const string GDAL_SUBDATASETS_NAME = "SUBDATASETS";
        private const string EXP_EXTRACT_DSNAME = @"\]\s*(?<dsname>(\S*\s)*)\(";
        protected List<string> _datasetNames = new List<string>();
        protected List<string> _datasetFullNames = new List<string>();
        protected IRasterDataProvider _provider = null;
        protected Access _access = Access.GA_ReadOnly;
        private BandProviderDef _matchedBandProviderDef = null;
        private Regex[] MYD_AQUA_NAMES = new Regex[] { 
            new Regex(@"MYD021KM", RegexOptions.Compiled), 
            new Regex(@"MYD02QKM", RegexOptions.Compiled),
            new Regex(@"MYD02HKM", RegexOptions.Compiled), 
            new Regex(@"EOSA", RegexOptions.Compiled),
            new Regex(@"AQUA", RegexOptions.Compiled)
        };//通过文件名来确定AQUA卫星
        private Regex MYD_MOD_JULIAN_DATATIME = new Regex(@".A(?<yyyy>\d{4})(?<ddd>\d{3}).(?<hh>\d{2})(?<mm>\d{2})", RegexOptions.Compiled);//儒略日

        public BandProviderHDF4()
        {

        }

        public override void Init(string fname, enumDataProviderAccess access, IRasterDataProvider provider)
        {
            _provider = provider;
            _access = access == enumDataProviderAccess.ReadOnly ? Access.GA_ReadOnly : Access.GA_Update;
            Dictionary<string, string> subdatasets = _provider.Attributes.GetAttributeDomain(GDAL_SUBDATASETS_NAME);
            ExtractDatasetNames(subdatasets);
            TryGetBandProviderDefinition(fname, subdatasets);
        }

        private void TryGetBandProviderDefinition(string fname, Dictionary<string, string> subdatasets)
        {
            List<string> datasetNames = null;
            if (subdatasets != null)
                datasetNames = GetDatasetNames(subdatasets);
            BandProviderDef[] bandProviderDefs = null;
            //Console.WriteLine(this.GetType().Assembly.Location);
            string configfile = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "GeoDo.RSS.DF.GDAL.H4BandPrd.xml");
            using (H4BandProviderXmlParser xml = new H4BandProviderXmlParser(configfile))
            {
                bandProviderDefs = xml.GetBandProviderDefs();
            }
            if (bandProviderDefs == null)
                return;
            string dayornight = "";
            using (Hdf4Operator hdf = new Hdf4Operator(fname))
            {
                foreach (BandProviderDef prddef in bandProviderDefs)
                {
                    bool isMatched = true;
                    if (datasetNames != null)
                    {
                        foreach (DefaultBandDatasetDef bandDef in prddef.DefaultBandDatasetDefs)
                        {
                            if (!datasetNames.Contains(bandDef.Name))
                            {
                                isMatched = false;
                                break;
                            }
                        }
                        if (!isMatched)
                            continue;
                    }
                    foreach (IdentifyAttDef id in prddef.IdentifyAttDefs)
                    {
                        string attvalue = hdf.GetAttributeValue(id.Name);
                        if (attvalue != id.Value)
                        {
                            isMatched = false;
                            break;
                        }
                    }
                    //增加对网站下载MODIS数据的支持，数据未在属性中定义卫星、传感器信息
                    if (!isMatched)
                    {
                        DataIdentify di = DataIdentifyMatcher.Match(fname);
                        if (di != null && !string.IsNullOrEmpty(di.Satellite))
                            foreach (IdentifyAttDef id in prddef.IdentifyAttDefs)
                            {
                                if (di.Satellite.ToUpper() == id.Value.ToUpper())
                                {
                                    isMatched = true;
                                    break;
                                }
                            }
                    }
                    if (isMatched)
                    {
                        _matchedBandProviderDef = prddef;
                        break;
                    }
                }
                dayornight = TryGetDayOrnight(hdf);
            }
            if (_matchedBandProviderDef != null)
            {
                _dataIdentify = new DataIdentify(_matchedBandProviderDef.Satellite, _matchedBandProviderDef.Sensor);
                _dataIdentify.IsOrbit = true;
                TrySetIdentifyByName(fname);
                TryGetOrbitDirection(dayornight, _matchedBandProviderDef.Satellite);
            }
        }

        private void TrySetIdentifyByName(string fname)
        {
            string filename = Path.GetFileName(fname);
            bool isAqua = false;
            foreach (Regex reg in MYD_AQUA_NAMES)
            {
                Match match = reg.Match(filename);
                if (match.Success)
                {
                    if (_dataIdentify == null)
                        _dataIdentify = new DataIdentify();
                    _dataIdentify.Satellite = "AQUA";//EOSA
                    _dataIdentify.Sensor = "MODIS";
                    isAqua = true;
                    break;
                }
            }
            if (!isAqua)
            {
                _dataIdentify.Satellite = "TERRA";//EOST
                _dataIdentify.Sensor = "MODIS";
            }
            if (_dataIdentify.OrbitDateTime == DateTime.MinValue)//匹配儒略日形式的日期。NASA网站下载的MODIS数据。
            {
                Match match = MYD_MOD_JULIAN_DATATIME.Match(filename);
                DateTime dt;
                if (match.Success)
                {
                    if (TryGetDate(match, out dt))
                        _dataIdentify.OrbitDateTime = dt;
                }
            }
        }

        private bool TryGetDate(Match match, out DateTime dt)
        {
            dt = DateTime.MinValue;
            int yyyy, ddd, hh, mm;
            if (match.Groups["yyyy"].Success && match.Groups["ddd"].Success
                && match.Groups["hh"].Success && match.Groups["mm"].Success)
            {
                int.TryParse(match.Groups["yyyy"].Value, out yyyy);
                int.TryParse(match.Groups["ddd"].Value, out ddd);
                int.TryParse(match.Groups["hh"].Value, out hh);
                int.TryParse(match.Groups["mm"].Value, out mm);
                dt = new DateTime(yyyy, 1, 1).AddDays(ddd - 1).AddHours(hh).AddMinutes(mm);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Terra (EOS AM) and Aqua (EOS PM) satellites
        /// 上午星(Terra)，白天是升轨，晚上是降轨(不知道这个只适用于中国还是全部数据都适用)
        /// 下午星(Aqua)反之
        /// </summary>
        /// <param name="hdf"></param>
        private void TryGetOrbitDirection(string dayOrNight, string satellite)
        {
            if (string.IsNullOrWhiteSpace(dayOrNight) || string.IsNullOrWhiteSpace(satellite))
                return;
            if (satellite == "TERRA" || satellite == "EOST")
                _dataIdentify.IsAscOrbitDirection = (dayOrNight == "Day" ? false : true);
            else if (satellite == "AQUA" || satellite == "EOSA")
                _dataIdentify.IsAscOrbitDirection = (dayOrNight == "Day" ? true : false);
        }

        private string TryGetDayOrnight(Hdf4Operator hdf)
        {
            string dayornight = hdf.GetAttributeValue("DAYNIGHTFLAG");
            if (string.IsNullOrWhiteSpace(dayornight))
                return null;
            dayornight = dayornight.Trim();
            string up = dayornight.ToUpper();
            if (up == "DAY" || up == "D")
                return "Day";
            else if (up == "NIGHT" || up == "N")
                return "Night";
            else if (up == "BOTH")
                return "Night";
            else
                return dayornight;//默认为白天
        }

        private List<string> GetDatasetNames(Dictionary<string, string> subdatasets)
        {
            List<string> datasetNames = new List<string>();
            int var = 0;
            foreach (string key in subdatasets.Keys)
            {
                if (var++ % 2 != 0)
                {
                    Match m = Regex.Match(subdatasets[key], EXP_EXTRACT_DSNAME);
                    if (m.Success)
                    {
                        datasetNames.Add(m.Groups["dsname"].Value.Trim());
                    }
                }
            }
            return datasetNames;
        }

        private void ExtractDatasetNames(Dictionary<string, string> subdatasets)
        {
            int var = 0;
            foreach (string key in subdatasets.Keys)
            {
                if (var++ % 2 != 0)
                {
                    Match m = Regex.Match(subdatasets[key], EXP_EXTRACT_DSNAME);
                    if (m.Success)
                    {
                        _datasetNames.Add(m.Groups["dsname"].Value.Trim());
                        _datasetFullNames.Add(subdatasets[subdatasets.Keys.ElementAt<string>(var - 2)]);
                    }
                }
            }
        }

        public override void Reset()
        {
            if (_datasetNames != null)
            {
                _datasetNames.Clear();
                _datasetNames = null;
            }
            _provider = null;
            _matchedBandProviderDef = null;
        }

        public override IRasterBand[] GetDefaultBands()
        {
            if (_matchedBandProviderDef == null || _matchedBandProviderDef.DefaultBandDatasetDefs == null || _matchedBandProviderDef.DefaultBandDatasetDefs.Count == 0)
                return null;
            IBandNameParser bandNameParser = new DefaultBandNameParser();
            using (Hdf4Operator hdf4 = new Hdf4Operator(_provider.fileName))
            {
                List<IRasterBand> rasterBands = new List<IRasterBand>();
                foreach (DefaultBandDatasetDef dsdef in _matchedBandProviderDef.DefaultBandDatasetDefs)
                {
                    string bandNos = hdf4.GetAttributeValue(dsdef.Name, dsdef.BandNoAttribute);
                    BandName[] bNames = bandNameParser.Parse(bandNos);
                    Dataset ds = Gdal.Open(ToDatasetFullName(dsdef.Name), _access);
                    IRasterBand[] rBands = ReadBandsFromDataset(ds, _provider);
                    rasterBands.AddRange(rBands);
                    for (int i = 0; i < rBands.Length; i++)
                    {
                        rBands[i].Description = bNames[i].Name;
                        rBands[i].BandNo = bNames[i].Index;
                    }

                }
                rasterBands.Sort();
                return rasterBands.Count > 0 ? rasterBands.ToArray() : null;
            }
        }

        //SUBDATASET_1_NAME=HDF4_SDS:UNKNOWN:"f:\TERRA_2010_03_25_03_09_GZ.MOD021KM.hdf":0
        private string ToDatasetFullName(string datasetName)
        {
            int idx = _datasetNames.IndexOf(datasetName);
            //string dsName = string.Format("HDF4_SDS:UNKNOWN:\"{0}\":{1}",  _provider.fileName, idx);
            string dsName = _datasetFullNames[idx];     //这个idx不一定就是dsName字符串中的序号。
            return dsName;
        }

        public override IRasterBand[] GetBands(string datasetName)
        {
            if (string.IsNullOrEmpty(datasetName))
                return null;
            return GetBandsByFullName(datasetName);
        }

        private IRasterBand[] GetBandsByFullName(string datasetName)
        {
            string datasetFullName = ToDatasetFullName(datasetName);
            if (_datasetNames == null || _datasetNames.Count == 0 || string.IsNullOrEmpty(datasetFullName))
                return null;
            foreach (string dsname in _datasetNames)
            {
                if (string.IsNullOrEmpty(dsname))
                    continue;
                if (dsname.Contains(datasetName))
                {
                    Dataset ds = Gdal.Open(datasetFullName, _access);
                    return ReadBandsFromDataset(ds, _provider);
                }
            }
            return null;
        }

        private IRasterBand[] ReadBandsFromDataset(Dataset ds, IRasterDataProvider provider)
        {
            if (ds == null || ds.RasterCount == 0)
                return null;
            IRasterBand[] bands = new IRasterBand[ds.RasterCount];
            for (int i = 1; i <= ds.RasterCount; i++)
                bands[i - 1] = new GDALRasterBand(provider, ds.GetRasterBand(i), new GDALDataset(ds));
            return bands;
        }

        public override string[] GetDatasetNames()
        {
            return _datasetNames.Count > 0 ? _datasetNames.ToArray() : null;
        }

        public override Dictionary<string, string> GetAttributes()
        {
            using (IHdfOperator hdf = new Hdf4Operator(_provider.fileName))
            {
                return hdf.GetAttributes();
            }
        }

        public override Dictionary<string, string> GetDatasetAttributes(string datasetName)
        {
            using (IHdfOperator hdf = new Hdf4Operator(_provider.fileName))
            {
                return hdf.GetAttributes(datasetName);
            }
        }

        public override bool IsSupport(string fname, byte[] header1024, Dictionary<string, string> datasetNames)
        {
            string ext = Path.GetExtension(fname).ToUpper();
            if (ext != ".HDF" || !HDF4Helper.IsHdf4(header1024))
                return false;
            TryGetBandProviderDefinition(fname, datasetNames);
            return _matchedBandProviderDef != null;
        }
    }
}
