using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.HDF;
using GeoDo.HDF4;
using GeoDo.RSS.Core.DF;
using OSGeo.GDAL;
using GeoDo.Project;
using System.IO;
using GeoDo.HDF5;

namespace GeoDo.RSS.DF.GDAL.HDF5GEO
{
    public class FY3HDFL2ProductProvider : RasterDataProvider
    {
        private object[] _args = null;
        private Dictionary<string, string> _fileAttributes = new Dictionary<string, string>();
        protected string[] _allGdalSubDatasets = null;
        private string[] _selectedsets = null;
        private Dictionary<L2ProductDefind, List<string>> _selectedSetsDic = null;
        private string _prodscArg = "l2prodsc=";
        private string _prodatasetArg = "datasets=";

        public FY3HDFL2ProductProvider(string fileName, byte[] header1024, IGeoDataDriver driver, params object[] args)
            : base(fileName, driver)
        {
            L2ProductDefind[] l2Pros = L2ProductDefindParser.GetL2ProductDefs(Path.GetFileName(_fileName));
            if (l2Pros == null)
                return;
            _fileName = fileName;
            _args = args;
            L2ProductDefind verifyL2Pro = l2Pros.Length == 1 ? l2Pros[0] : VerifyL2ProDef(l2Pros);
            if (verifyL2Pro == null)
                return;
            using (Dataset dataset = Gdal.Open(fileName, Access.GA_ReadOnly))
            {
                GDALHelper.GetDatasetAttributes(dataset, _attributes);
            }
            Dictionary<string, string> allGdalSubDatasets = this.Attributes.GetAttributeDomain("SUBDATASETS");
            _allGdalSubDatasets = RecordAllSubDatasetNames(allGdalSubDatasets);
            _coordEnvelope = TrySetGeoInfo(verifyL2Pro);
            _selectedsets = TryGetSelectedSets(verifyL2Pro);
            TryCreateBandProvider();
            _bandCount = _rasterBands.Count;
            _dataType = _rasterBands[0].DataType;
            _width = _rasterBands[0].Width;
            _height = _rasterBands[0].Height;
            _coordType = _spatialRef.ProjectionCoordSystem != null ? enumCoordType.PrjCoord : enumCoordType.GeoCoord;
            _resolutionX = (float)(_coordEnvelope.Width / (_width));
            _resolutionY = (float)(_coordEnvelope.Height / (_height));
            if (_dataIdentify != null && _dataIdentify.OrbitDateTime == DateTime.MinValue)
            {
                _dataIdentify.OrbitDateTime = IceConDataProvider.TryGetFileDate(Path.GetFileName(fileName));
            }
        }

        private L2ProductDefind VerifyL2ProDef(L2ProductDefind[] l2Pros)
        {
            if (_args == null || _args.Length == 0)
                throw new Exception("有多个相同匹配定义的产品信息,无法确认具体产品描述!");
            string argStr;
            _selectedSetsDic = new Dictionary<L2ProductDefind, List<string>>();
            for (int i = 0; i < _args.Length; i++)
            {
                if (_args[i] == null)
                    continue;
                argStr = _args[i].ToString();
                if (!string.IsNullOrEmpty(argStr))
                {
                    if (argStr.Contains(_prodscArg))
                    {
                        for (int j = 0; j < l2Pros.Length; j++)
                        {
                            if (string.IsNullOrEmpty(l2Pros[j].Desc))
                                continue;
                            if (l2Pros[j].Desc.ToUpper().Trim() == argStr.Replace(_prodscArg, "").ToUpper().Trim())
                                return l2Pros[j];
                        }
                    }
                    else if (argStr.Contains(_prodatasetArg))
                    {
                        string[] datasetTemp = argStr.Replace(_prodatasetArg, "").Replace(" ", "_").Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
                        if (datasetTemp == null || datasetTemp.Length == 0)
                            return null;
                        List<string> dataSetsName = null;
                        for (int j = 0; j < datasetTemp.Length; j++)
                        {
                            foreach (L2ProductDefind item in l2Pros)
                            {
                                if (!_selectedSetsDic.ContainsKey(item))
                                {
                                    dataSetsName = new List<string>();
                                    _selectedSetsDic.Add(item, dataSetsName);
                                }
                                if (item.ProInfo == null || string.IsNullOrEmpty(item.ProInfo.ProDatasets))
                                    _selectedSetsDic[item].Add(datasetTemp[j]);
                                else if (item.ProInfo != null && !string.IsNullOrEmpty(item.ProInfo.ProDatasets) && item.ProInfo.ProDatasets.Contains(datasetTemp[j]))
                                    _selectedSetsDic[item].Add(datasetTemp[j]);
                            }
                        }
                        return GetL2ProDefFromDic();
                    }
                }
            }
            return null;
        }

        #region 从文件属性获取经纬度
        private CoordEnvelope TrySetGeoInfo(L2ProductDefind l2Pro)
        {
            _spatialRef = SpatialReference.FromProj4(l2Pro.GeoInfo.Proj4Str);
            if (l2Pro.GeoInfo.GeoAtrrs == null && l2Pro.GeoInfo.GeoDef != null)
                return new CoordEnvelope(l2Pro.GeoInfo.GeoDef.LeftTopLon, l2Pro.GeoInfo.GeoDef.RightBottomLon, l2Pro.GeoInfo.GeoDef.RightBottomLat, l2Pro.GeoInfo.GeoDef.LeftTopLat);
            else if (l2Pro.GeoInfo.GeoAtrrs != null)
            {
                return GetCoordEnvelope(l2Pro.GeoInfo.GeoAtrrs);
            }
            return null;
        }

        private IHdfOperator HdfOperatorFactory(string filename)
        {
            if (HDF5Helper.IsHdf5(filename)) return new Hdf5Operator(filename);
            if (HDF4Helper.IsHdf4(filename)) return new Hdf4Operator(filename);
            return null;
        }

        private CoordEnvelope GetCoordEnvelope(GeoAtrributes geoAtrrs)
        {
            double lulat = 0.0, lulon = 0.0, rdlat = 0.0, rdlon = 0.0;
            string lulatstr = geoAtrrs.LeftTopLatAtrrName;
            string lulonstr = geoAtrrs.LeftTopLonAtrrName;
            string rdlatstr = geoAtrrs.RightBottomLatAtrrName;
            string rdlonstr = geoAtrrs.RightBottomLonAtrrName;
            string unitstr = geoAtrrs.Unit;
            using (IHdfOperator oper = HdfOperatorFactory(_fileName))
            {
                if (geoAtrrs.Location == enumGeoAttType.File)
                {
                    _fileAttributes = oper.GetAttributes();
                    if (_fileAttributes == null)
                        return null;
                    if (!GetExtentPoints(lulatstr, lulonstr, rdlatstr, rdlonstr, out lulat, out lulon, out rdlat, out rdlon))
                        return null;

                    if (!string.IsNullOrEmpty(unitstr) && _fileAttributes.ContainsKey(unitstr))
                    {
                        string unit = _fileAttributes[unitstr];
                        if (!string.IsNullOrEmpty(unit))
                            UpdateRDLonLat(unit, ref lulat, ref lulon, ref rdlat, ref rdlon);
                    }
                }
                else
                {
                    string geoDataset = geoAtrrs.GeoDataset;
                    lulat = GetDoubleAtrr(oper, geoDataset, lulatstr);
                    lulon = GetDoubleAtrr(oper, geoDataset, lulonstr);
                    rdlat = GetDoubleAtrr(oper, geoDataset, rdlatstr);
                    rdlon = GetDoubleAtrr(oper, geoDataset, rdlonstr);
                    if (Math.Abs(lulat - double.MinValue) < double.Epsilon ||
                        Math.Abs(lulon - double.MinValue) < double.Epsilon ||
                        Math.Abs(rdlat - double.MinValue) < double.Epsilon ||
                        Math.Abs(rdlon - double.MinValue) < double.Epsilon)
                        return null;
                }
                return new CoordEnvelope(lulon, rdlon, rdlat, lulat);
            }
        }

        private bool GetExtentPoints(string lulatstr, string lulonstr, string rdlatstr, string rdlonstr,
            out double lulat, out double lulon, out double rdlat, out double rdlon)
        {
            lulat = lulon = rdlat = rdlon = double.MinValue;
            foreach (KeyValuePair<string, string> fileAttribute in _fileAttributes)
            {
                if (fileAttribute.Key == lulatstr)
                {
                    if (!double.TryParse(fileAttribute.Value, out lulat))
                        return false;
                }
                else if (fileAttribute.Key == lulonstr)
                {
                    if (!double.TryParse(fileAttribute.Value, out lulon))
                        return false;
                }
                else if (fileAttribute.Key == rdlatstr)
                {
                    if (!double.TryParse(fileAttribute.Value, out rdlat))
                        return false;
                }
                else if (fileAttribute.Key == rdlonstr)
                {
                    if (!double.TryParse(fileAttribute.Value, out rdlon))
                        return false;
                }
            }
            if (Math.Abs(lulat - double.MinValue) < double.Epsilon && lulatstr.Contains(":"))
            {
                try
                {
                    string metaName = GetValueFromStringColon(lulatstr, true);
                    string metaValue = _fileAttributes[metaName];
                    string upperLeftKey = GetValueFromStringColon(lulatstr, false);
                    string rightbottomKey = GetValueFromStringColon(rdlatstr, false);

                    string upperLeftValue = null;
                    string rightbottomValue = null;

                    string[] strs = metaValue.Split(new[] { "\n", "\t", "\0" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var str in strs)
                    {
                        string strKey = GetValueFromStringEqual(str, true);
                        if (strKey == upperLeftKey)
                            upperLeftValue = GetValueFromStringEqual(str, false);
                        else if (strKey == rightbottomKey)
                            rightbottomValue = GetValueFromStringEqual(str, false);

                        if (upperLeftValue != null && rightbottomValue != null)
                            break;
                    }

                    if (upperLeftValue != null && rightbottomValue != null)
                    {
                        string[] strsUL = upperLeftValue.Split(new[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        double xmin = 0.0;
                        if (double.TryParse(strsUL[0], out xmin))
                            lulon = xmin;
                        double ymax = 0.0;
                        if (double.TryParse(strsUL[1], out ymax))
                            lulat = ymax;
                        string[] strsLR = rightbottomValue.Split(new[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        double xmax = 0.0;
                        if (double.TryParse(strsLR[0], out xmax))
                            rdlon = xmax;
                        double ymin = 0.0;
                        if (double.TryParse(strsLR[1], out ymin))
                            rdlat = ymin;
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        private string GetValueFromStringEqual(string source, bool isKey)
        {
            return GetValueFromString(source, isKey, '=');
        }
        private string GetValueFromStringColon(string source, bool isKey)
        {
            return GetValueFromString(source, isKey, ':');
        }
        private string GetValueFromString(string source, bool isKey, char split)
        {
            var strs = source.Split(new[] { split });
            return isKey ? strs[0].Trim() : strs[1].Trim();
        }

        private void UpdateRDLonLat(string unit, ref double lulat, ref double lulon, ref double rdlat, ref double rdlon)
        {
            if (_spatialRef.ProjectionCoordSystem != null && _spatialRef.ProjectionCoordSystem.Unit.Name == "Meter")
                switch (unit.ToLower())
                {
                    case "km":
                    case "公里":
                        lulat *= 1000;
                        lulon *= 1000;
                        rdlat *= 1000;
                        rdlon *= 1000;
                        break;
                }
        }

        private double GetDoubleAtrr(IHdfOperator oper, string dataSet, string atrribute)
        {
            double result = double.MinValue;
            string attrTemp = oper.GetAttributeValue(dataSet, atrribute);
            if (!string.IsNullOrEmpty(attrTemp))
                if (double.TryParse(attrTemp, out result))
                    return result;
            return double.MinValue;
        }
        #endregion

        #region 设置波段
        private void TryCreateBandProvider()
        {
            foreach (string dsName in _selectedsets)
            {
                string dsPath = GetDatasetFullPath(dsName);
                Dataset dataset = Gdal.Open(dsPath, Access.GA_ReadOnly);
                IRasterBand[] gdalDatasets = ReadBandsFromDataset(dsName, dataset, this);
                _rasterBands.AddRange(gdalDatasets);
            }
        }

        private string[] RecordAllSubDatasetNames(Dictionary<string, string> subdatasets)
        {
            List<string> dss = new List<string>();
            int idx = 0;
            foreach (string key in subdatasets.Keys)
                if (idx++ % 2 == 0)
                    dss.Add(subdatasets[key]);
            return dss.ToArray();
        }

        private string GetDatasetFullPath(string datasetName)
        {
            for (int i = 0; i < _allGdalSubDatasets.Length; i++)
            {
                string shortGdalDatasetName = GetDatasetShortName(_allGdalSubDatasets[i]);
                if (shortGdalDatasetName == datasetName)
                    return _allGdalSubDatasets[i];
            }
            return null;
        }

        private string GetDatasetShortName(string datasetName)
        {
            string shortDatasetName = null;
            int groupIndex = datasetName.LastIndexOf("/");
            if (groupIndex == -1)
            {
                groupIndex = datasetName.LastIndexOf(":");
                if (groupIndex == -1)
                {
                    shortDatasetName = datasetName;
                    return shortDatasetName;
                }
            }
            
            shortDatasetName = datasetName.Substring(groupIndex + 1);
            return shortDatasetName;
        }

        private IRasterBand[] ReadBandsFromDataset(string dsname, Dataset ds, IRasterDataProvider provider)
        {
            int bandNo = 1;
            IRasterBand[] bands = new IRasterBand[ds.RasterCount];
            for (int i = 1; i <= ds.RasterCount; i++)
            {
                bands[i - 1] = new GDALRasterBand(provider, ds.GetRasterBand(i), new GDALDataset(ds));
                bands[i - 1].BandNo = bandNo++;
                bands[i - 1].Description = dsname;
            }
            return bands;
        }

        #endregion

        public override void AddBand(enumDataType dataType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据卫星传感器，获取默认显示的波段。
        /// </summary>
        /// <returns>返回的是映射后的bandNo</returns>
        public override int[] GetDefaultBands()
        {
            return new int[] { 1, 1, 1 };
        }

        private string[] TryGetSelectedSets()
        {
            List<string> allsets = new List<string>();
            for (int i = 0; i < _allGdalSubDatasets.Length; i++)
            {
                string shortGdalDatasetName = GetDatasetShortName(_allGdalSubDatasets[i]);
                allsets.Add(shortGdalDatasetName);
            }
            return allsets.ToArray();
        }

        private string[] TryGetSelectedSets(L2ProductDefind l2Pro)
        {
            List<string> allsets = new List<string>();
            string[] allsetTemp = GetTempSetsFromArgs(l2Pro);
            if (allsetTemp != null)
            {
                for (int i = 0; i < allsetTemp.Length; i++)
                {
                    for (int j = 0; j < _allGdalSubDatasets.Length; j++)
                    {
                        if (GetDatasetShortName(_allGdalSubDatasets[j]).ToUpper().Trim() == allsetTemp[i].ToUpper().Trim())
                        {
                            allsets.Add(allsetTemp[i]);
                            break;
                        }
                    }
                }
            }
            return allsets.Count == 0 ? TryGetSelectedSets() : allsets.ToArray();
        }

        private string[] GetTempSetsFromArgs(L2ProductDefind l2Pro)
        {
            if (_args == null || _args.Length == 0)
            {
                if (l2Pro.ProInfo != null)
                {
                    if (!string.IsNullOrEmpty(l2Pro.ProInfo.ProDatasets))
                        return l2Pro.ProInfo.ProDatasets.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
            else
            {
                if (_selectedSetsDic != null && _selectedSetsDic.Count != 0)
                    return _selectedSetsDic[l2Pro].ToArray();
                string argStr;
                for (int i = 0; i < _args.Length; i++)
                {
                    if (_args[i] == null)
                        continue;
                    argStr = _args[i].ToString();
                    if (!string.IsNullOrEmpty(argStr) && argStr.Contains(_prodatasetArg))
                        return argStr.Replace(_prodatasetArg, "").Replace(" ", "_").Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
                    if (!string.IsNullOrEmpty(argStr) && argStr.Contains(_prodscArg) && l2Pro.ProInfo != null)
                    {
                        if (!string.IsNullOrEmpty(l2Pro.ProInfo.ProDatasets))
                            return l2Pro.ProInfo.ProDatasets.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
                    }
                }
            }
            return null;
        }

        private L2ProductDefind GetL2ProDefFromDic()
        {
            L2ProductDefind result = null;
            int maxCount = 0;
            foreach (L2ProductDefind item in _selectedSetsDic.Keys)
            {
                if (_selectedSetsDic[item].Count > maxCount)
                {
                    result = item;
                    maxCount = _selectedSetsDic[item].Count;
                }
            }
            return maxCount == 0 ? null : result;
        }
    }

}
