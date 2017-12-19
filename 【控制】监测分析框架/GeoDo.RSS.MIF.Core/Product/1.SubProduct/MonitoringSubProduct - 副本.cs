using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Drawing;
using GeoDo.RasterProject;

namespace GeoDo.RSS.MIF.Core
{
    public abstract class MonitoringSubProduct : IMonitoringSubProduct, IDisposable
    {
        protected string _name;
        protected string _identify;
        protected bool _isBinary;
        protected SubProductDef _subProductDef;
        protected List<AlgorithmDef> _algorithmDefs;
        protected IArgumentProvider _argumentProvider;
        protected string _error = null;

        public MonitoringSubProduct()
        {
        }

        public MonitoringSubProduct(SubProductDef subProductDef)
        {
            _subProductDef = subProductDef;
            if (subProductDef == null)
                return;
            _name = subProductDef.Name;
            _identify = subProductDef.Identify;
            if (subProductDef.Algorithms == null || subProductDef.Algorithms.Length == 0)
                return;
            //通过默认算法设置参数提供者
            _algorithmDefs = new List<AlgorithmDef>(subProductDef.Algorithms);
            if (_algorithmDefs.Count > 0)
            {
                ExtractProductIdentify pid = new ExtractProductIdentify();
                pid.ThemeIdentify = subProductDef.ProductDef.Theme.Identify;
                pid.ProductIdentify = subProductDef.ProductDef.Identify;
                pid.SubProductIdentify = subProductDef.Identify;
                _argumentProvider = MonitoringThemeFactory.GetArgumentProvider(pid, _algorithmDefs[0].Identify);
                _argumentProvider.SetArg("AlgorithmName", _algorithmDefs[0].Identify);
            }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Identify
        {
            get { return _identify; }
        }

        public bool IsBinary
        {
            get { return _isBinary; }
        }

        public SubProductDef Definition
        {
            get { return _subProductDef; }
        }

        public List<AlgorithmDef> AlgorithmDefs
        {
            get { return _algorithmDefs; }
        }

        /// <summary>
        /// 参数提供者中可以指定算法标识,指定本次产品生成用的算法
        /// </summary>
        /// <param name="argProvider"></param>
        /// <param name="outputer"></param>
        /// <returns></returns>
        public abstract IExtractResult Make(Action<int, string> progressTracker);
        public virtual IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            return Make(progressTracker);
        }

        public virtual bool ArgumentsIsOK()
        {
            return false;
        }

        public virtual void Reset()
        {
        }

        public IArgumentProvider ArgumentProvider
        {
            get { return _argumentProvider; }
        }

        public void ResetArgumentProvider(string algIdentify)
        {
            ExtractProductIdentify pid = new ExtractProductIdentify();
            pid.ThemeIdentify = "CMA";
            pid.ProductIdentify = _subProductDef.ProductDef.Identify;
            pid.SubProductIdentify = _subProductDef.Identify;
            IArgumentProvider prd = MonitoringThemeFactory.GetArgumentProvider(pid, algIdentify);
            if (prd != null)
            {
                string[] argNames = prd.ArgNames;
                if (argNames != null)
                    foreach (string name in argNames)
                        _argumentProvider.SetArg(name, prd.GetArg(name));
            }
        }

        public void ResetArgumentProvider(string satellite, string sensor, params string[] args)
        {
            if (_argumentProvider == null)
                _argumentProvider = GetDefaultArgProvider(satellite, sensor, args);
            else
            {
                _argumentProvider.Reset();
                IArgumentProvider prd = GetDefaultArgProvider(satellite, sensor, args);
                if (prd != null)
                {
                    string[] argNames = prd.ArgNames;
                    if (argNames != null)
                        foreach (string name in argNames)
                            _argumentProvider.SetArg(name, prd.GetArg(name));
                }
            }
        }

        public void ResetArgumentProvider(string algIdentify, string satellite, string sensor, params string[] args)
        {
            if (_argumentProvider == null)
                _argumentProvider = GetDefaultArgProvider(algIdentify, satellite, sensor, args);
            else
            {
                _argumentProvider.Reset();
                IArgumentProvider prd = GetDefaultArgProvider(algIdentify, satellite, sensor, args);
                if (prd != null)
                {
                    string[] argNames = prd.ArgNames;
                    if (argNames != null)
                        foreach (string name in argNames)
                            _argumentProvider.SetArg(name, prd.GetArg(name));
                }
            }
        }

        private IArgumentProvider GetDefaultArgProvider(string algIdentify, string satellite, string sensor, string[] args)
        {
            ExtractProductIdentify id1 = new ExtractProductIdentify();
            id1.ProductIdentify = _subProductDef.ProductDef.Identify;
            id1.SubProductIdentify = _subProductDef.Identify;
            id1.ThemeIdentify = "CMA";
            return MonitoringThemeFactory.GetArgumentProvider(id1, algIdentify, satellite, sensor);
        }

        private IArgumentProvider GetDefaultArgProvider(string satellite, string sensor, string[] args)
        {
            ExtractProductIdentify id1 = new ExtractProductIdentify();
            id1.ProductIdentify = _subProductDef.ProductDef.Identify;
            id1.SubProductIdentify = _subProductDef.Identify;
            id1.ThemeIdentify = "CMA";
            //
            ExtractAlgorithmIdentify id2 = new ExtractAlgorithmIdentify();
            id2.Satellite = satellite;
            id2.Sensor = sensor;
            id2.CustomIdentify = (args != null && args.Length > 0) ? args[0] : null;
            //
            return MonitoringThemeFactory.GetArgumentProvider(id1, id2);
        }

        public virtual void SetExtHeader(IExtHeaderSetter setter, object header)
        {
        }

        public AlgorithmDef UseDefaultAlgorithm(string productIdentify)
        {
            if (_algorithmDefs == null || _algorithmDefs.Count == 0)
                return null;
            ExtractProductIdentify pid = new ExtractProductIdentify();
            pid.ProductIdentify = productIdentify;
            pid.SubProductIdentify = _identify;
            pid.ThemeIdentify = "CMA";
            _argumentProvider = MonitoringThemeFactory.GetArgumentProvider(pid, _algorithmDefs[0].Identify);
            return _algorithmDefs[0];
        }

        public void Dispose()
        {
            if (_algorithmDefs != null)
            {
                _algorithmDefs.Clear();
                _algorithmDefs = null;
            }
            _subProductDef = null;
        }

        public string[] GetStringArray(string arugmentName)
        {
            object obj = _argumentProvider.GetArg(arugmentName);
            if (obj == null)
                return null;
            string[] resultArray = obj as string[];
            if (arugmentName == "SelectedPrimaryFiles" && resultArray != null)
                return resultArray;
            //
            if (arugmentName == "mainfiles" && resultArray != null)
                return resultArray;
            else if (resultArray != null && obj.ToString() == "System.String[]")
                return resultArray;
            else
            {
                //如果obj为数组，则obj.tostring() = System.String[];
                string[] tempSplit = obj.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (tempSplit == null || tempSplit.Length == 0)
                    return null;
                List<string> result = new List<string>();
                for (int i = 0; i < tempSplit.Length; i++)
                {
                    if (File.Exists(tempSplit[i]))
                        result.Add(tempSplit[i]);
                }
                return result.Count == 0 ? null : result.ToArray();
            }
        }

        public string GetStringArugment(string arugmentName)
        {
            object obj = _argumentProvider.GetArg(arugmentName);
            if (obj == null)
                return null;
            return obj.ToString();
        }

        public void GetAOI(out string aoiTemplateName, out Dictionary<string, int[]> aoi)
        {
            aoi = null;
            aoiTemplateName = string.Empty;
            object obj = _argumentProvider.GetArg("AOI");
            if (obj != null)
                if (obj as Dictionary<string, int[]> != null)
                    aoi = obj as Dictionary<string, int[]>;
                else
                    aoiTemplateName = obj.ToString();
        }

        private int[] MasicAOI(Dictionary<string, int[]> aoiDic, ref string extInfos)
        {
            if (aoiDic == null || aoiDic.Count == 0)
                return null;
            List<int> result = new List<int>();
            if (string.IsNullOrEmpty(extInfos))
                extInfos += "_";
            foreach (string key in aoiDic.Keys)
            {
                result.AddRange(aoiDic[key]);
                extInfos += key;
            }
            return result.Count == 0 ? null : result.ToArray();
        }

        public RasterIdentify CreatRasterIndetifyId(string[] files, string productIdentify, string subProductindentify, DataIdentify di, string format, string extinfo)
        {
            RasterIdentify id = new RasterIdentify(files);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = productIdentify;
            id.SubProductIdentify = subProductindentify;
            id.ExtInfos = extinfo;
            id.Format = format;
            if (di == null)
                return id;
            if (string.IsNullOrEmpty(id.Satellite) || id.Satellite == "NUL")
                id.Satellite = di.Satellite;
            if (string.IsNullOrEmpty(id.Sensor) || id.Sensor == "NUL")
                id.Sensor = di.Sensor;
            if (id.OrbitDateTime == DateTime.MinValue)
                id.OrbitDateTime = di.OrbitDateTime;
            return id;
        }

        private IFileNameGenerator GetFileNameGenerator()
        {
            object obj = _argumentProvider.GetArg("FileNameGenerator");
            if (obj == null)
                return null;
            return obj as IFileNameGenerator;
        }

        public DataIdentify GetDataIdentify()
        {
            object obj = _argumentProvider.GetArg("CurrentDataIdentify");
            if (obj == null)
                return null;
            return obj as DataIdentify;
        }

        public string StatResultToFile(string[] files, StatResultItem[] resultItems, string productIdentify, string outputIdentify, string title, string extinfo)
        {
            IStatResult results = StatResultItemToIStatResult.ItemsToResults(resultItems);
            return StatResultToFile(files, results, productIdentify, outputIdentify, title, extinfo);
        }

        public string StatResultToFile(string[] files, IStatResult results, string productIdentify, string outputIdentify, string title, string extinfo)
        {
            string filename = GetFileName(files, productIdentify, outputIdentify, ".XLSX", extinfo);
            if (string.IsNullOrEmpty(filename))
                return string.Empty;
            try
            {
                if (File.Exists(filename))
                    File.Delete(filename);
                using (StatResultToExcelFile excelControl = new StatResultToExcelFile())
                {
                    excelControl.Init();
                    excelControl.Add(true, title, results, true, 1);
                    if (!filename.ToUpper().EndsWith(".XLSX"))
                        filename += ".XLSX";
                    excelControl.SaveFile(filename);
                }
            }
            catch (Exception)
            {
                using (StatResultToTxtFile txtControl = new StatResultToTxtFile())
                {
                    if (!filename.ToUpper().EndsWith(".TXT"))
                        filename += ".TXT";
                    txtControl.WriteResultToTxt(title + "\n");
                    txtControl.WriteResultToTxt("统计日期：" + DateTime.Today.Date.ToShortDateString() + "\n");
                    txtControl.WriteResultToTxt(results);
                    bool isSave = txtControl.SaveFile(filename);
                    if (!isSave)
                        return String.Empty;
                }
            }
            return filename;
        }

        public string GetFileName(string[] files, string productIdentify, string outputIdentify, string fomart, string extinfo)
        {
            IFileNameGenerator fng = GetFileNameGenerator();
            if (fng == null)
                return string.Empty;
            DataIdentify di = GetDataIdentify();
            return fng.NewFileName(CreatRasterIndetifyId(files, productIdentify, outputIdentify, di, fomart, extinfo));
        }

        public IExtractResult ThemeGraphyResult(string extInfos)
        {
            object obj = _argumentProvider.GetArg("ThemeGraphyGenerator");
            if (obj == null)
                return null;
            IThemeGraphGenerator tgg = obj as IThemeGraphGenerator;
            if (tgg == null)
                return null;
            string aoiTemplateName = string.Empty;
            Dictionary<string, int[]> aoi = null;
            GetAOI(out aoiTemplateName, out aoi);
            string outFileIdentify = GetStringArugment("OutFileIdentify");
            string templatName = GetStringArugment("ThemeGraphTemplateName");
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            PrjEnvelope useRegion = GetArgToPrjEnvelope("UseRegion");
            if (useRegion != null)  //需要限定输出范围
            {
            }
            string colorTabelName = GetColorTableName("colortablename");
            if (string.IsNullOrEmpty(extInfos))
                extInfos = GetStringArugment("extinfo");
            string resultFilename = tgg.Generate(files[0], templatName, MasicAOI(aoi, ref extInfos), extInfos, outFileIdentify, colorTabelName);
            if (string.IsNullOrEmpty(resultFilename))
                return null;
            return new FileExtractResult(outFileIdentify, resultFilename);
        }

        protected PrjEnvelope GetArgToPrjEnvelope(string arg)
        {
            object obj = _argumentProvider.GetArg(arg);
            if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                return null;
            else
                return (PrjEnvelope)obj;
        }

        protected string GetColorTableName(string attrubileName)
        {
            if (string.IsNullOrEmpty(attrubileName))
                return null;
            object obj = _argumentProvider.GetArg(attrubileName);
            if (obj == null)
                return null;
            return obj.ToString();
        }

        public IExtractResult AreaStatResult<T>(string productName, string productIdentify, Func<T, bool> filter)
        {
            object aioObj = _argumentProvider.GetArg("AOI");
            string outFileIdentify = GetStringArugment("OutFileIdentify");
            string[] files = GetStringArray("SelectedPrimaryFiles");
            string extInfos = GetStringArugment("extinfo");
            if (files == null || files.Length == 0)
                return null;
            string title = string.Empty;
            StatResultItem[] areaResult = CommProductStat.AreaStat<T>(productName, files[0], ref title, aioObj, filter);
            if (areaResult == null)
                return null;
            string filename = StatResultToFile(files, areaResult, productIdentify, outFileIdentify, title, extInfos);
            return new FileExtractResult(outFileIdentify, filename);
        }

        public IExtractResult TimesStatAnalysisByPixel<T>(string productName, string productIdentify, string extInfos, Func<T, T, T> function)
        {
            IRasterOperator<T> roper = new RasterOperator<T>();
            IInterestedRaster<T> timeResult = null;
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            if (string.IsNullOrEmpty(extInfos))
                extInfos = GetStringArugment("extinfo");
            DataIdentify di = GetDataIdentify();
            string outFileIdentify = GetStringArugment("OutFileIdentify");
            timeResult = roper.Times(files, CreatRasterIndetifyId(files, productIdentify, outFileIdentify, di, null, extInfos), function);
            if (timeResult == null)
                return null;
            object obj = _argumentProvider.GetArg("ThemeGraphyGenerator");
            if (obj == null)
                return timeResult;
            IThemeGraphGenerator tgg = obj as IThemeGraphGenerator;
            if (tgg == null)
                return timeResult;
            timeResult.Dispose();
            string aoiTemplateName = string.Empty;
            Dictionary<string, int[]> aoi = null;
            GetAOI(out aoiTemplateName, out aoi);
            string templatName = GetStringArugment("ThemeGraphTemplateName");
            string colorTabelName = GetColorTableName("colortablename");
            string resultFilename = tgg.Generate(timeResult.FileName, templatName, MasicAOI(aoi, ref extInfos), extInfos, outFileIdentify, colorTabelName);
            if (string.IsNullOrEmpty(resultFilename))
                return timeResult;
            return new FileExtractResult(outFileIdentify, resultFilename);
        }

        public IExtractResult CycleTimeStatAnalysisByPixel<T>(string productName, string productIdentify, string extInfos, Func<int, T, T, T> function)
        {
            IRasterOperator<T> roper = new RasterOperator<T>();
            IInterestedRaster<T> cycleIimeResult = null;
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            if (string.IsNullOrEmpty(extInfos))
                extInfos = GetStringArugment("extinfo");
            DataIdentify di = GetDataIdentify();
            string outFileIdentify = GetStringArugment("OutFileIdentify");
            cycleIimeResult = roper.CycleTimes(files, CreatRasterIndetifyId(files, productIdentify, outFileIdentify, di, null, extInfos), function);
            if (cycleIimeResult == null)
                return null;
            object obj = _argumentProvider.GetArg("ThemeGraphyGenerator");
            if (obj == null)
                return cycleIimeResult;
            IThemeGraphGenerator tgg = obj as IThemeGraphGenerator;
            if (tgg == null)
                return cycleIimeResult;
            cycleIimeResult.Dispose();
            string aoiTemplateName = string.Empty;
            Dictionary<string, int[]> aoi = null;
            GetAOI(out aoiTemplateName, out aoi);
            string templatName = GetStringArugment("ThemeGraphTemplateName");
            string colorTabelName = GetColorTableName("colortablename");
            string resultFilename = tgg.Generate(cycleIimeResult.FileName, templatName, MasicAOI(aoi, ref extInfos), extInfos, outFileIdentify, colorTabelName);
            if (string.IsNullOrEmpty(resultFilename))
                return cycleIimeResult;
            return new FileExtractResult(outFileIdentify, resultFilename);
        }

        public IExtractResult CompareAnalysisByPixel<T1,T>(string productName, string productIdentify, string extInfos, Func<T1, T1, T> function)
        {
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0 || files.Length == 1)
                return null;
            //文件列表排序
            string[] dstFiles = SortFileName(files);
            string outFileIdentify = GetStringArugment("OutFileIdentify");
            object obj = _argumentProvider.GetArg("ThemeGraphyGenerator");
            if (obj == null)
                return null;
            IThemeGraphGenerator tgg = obj as IThemeGraphGenerator;
            if (tgg == null)
                return null;
            string templatName = GetStringArugment("ThemeGraphTemplateName");
            string colorTabelName = GetColorTableName("colortablename");
            ExtractResultArray results = new ExtractResultArray(productIdentify + "_COMP");
            for (int i = 0; i < dstFiles.Length - 1; i++)
            {
                //生成专题图
                IPixelFeatureMapper<T> rasterResult = MakeCompareRaster<T1,T>(productIdentify, dstFiles[i], dstFiles[i + 1], function);
                string aoiTemplateName = string.Empty;
                Dictionary<string, int[]> aoi = null;
                GetAOI(out aoiTemplateName, out aoi);
                if (rasterResult == null)
                    continue;
                RasterIdentify rid = new RasterIdentify(dstFiles[i]);
                IInterestedRaster<T> iir = new InterestedRaster<T>(rid, rasterResult.Size, rasterResult.CoordEnvelope, rasterResult.SpatialRef);
                iir.Put(rasterResult);
                iir.Dispose();
                string resultFilename = tgg.Generate(iir.FileName, templatName, MasicAOI(aoi, ref extInfos), extInfos, outFileIdentify, colorTabelName);
                if (string.IsNullOrEmpty(resultFilename))
                    return null;
                FileExtractResult result = new FileExtractResult(outFileIdentify, resultFilename);
                if (result != null)
                    results.Add(result);
            }
            return results;
        }

        public bool CanDo
        {
            get
            {
                return ArgsIsFull();
            }
        }

        private bool ArgsIsFull()
        {
            if (_argumentProvider == null)
            {
                _error = "未提供[" + _subProductDef.Name + "]子产品参数提供者.";
                return false;
            }
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                _error = "未提供[" + _subProductDef.Name + "]子产品所用算法名.";
                return false;
            }
            string algorithmName = _argumentProvider.GetArg("AlgorithmName").ToString();
            AlgorithmDef algorithmDef = _subProductDef.GetAlgorithmDefByIdentify(algorithmName);
            if (algorithmDef == null)
            {
                _error = "当前[" + _subProductDef.Name + "]子产品不包含\"" + algorithmName + "\"算法.";
                return false;
            }
            if (algorithmDef != null)
            {
                object bandArg = null;
                foreach (BandDef arg in algorithmDef.Bands)
                {
                    bandArg = _argumentProvider.GetArg(arg.Identify);
                    if (bandArg == null)
                    {
                        _error = "未提供波段[" + arg.Identify + "]信息.";
                        return false;
                    }
                    if (bandArg.ToString() == "-1")
                    {
                        _error = "波段[" + arg.Identify + "]未找到对应的波段映射.";
                        return false;
                    }
                }
            }
            ArgumentPair argPair = null;
            ArgumentDef argDef = null;
            foreach (ArgumentBase arg in algorithmDef.Arguments)
            {
                if (arg is ArgumentPair)
                {
                    argPair = arg as ArgumentPair;
                    if (argPair.ArgumentMax != null && _argumentProvider.GetArg(argPair.ArgumentMax.Name) == null)
                    {
                        _error = "参数[" + argPair.ArgumentMax.Name + "]未提供.";
                        return false;
                    }
                    if (argPair.ArgumentMin != null && _argumentProvider.GetArg(argPair.ArgumentMin.Name) == null)
                    {
                        _error = "参数[" + argPair.ArgumentMin.Name + "]未提供.";
                        return false;
                    }
                }
                else if (arg is ArgumentDef)
                {
                    argDef = arg as ArgumentDef;
                    if (_argumentProvider.GetArg(argDef.Name) == null)
                    {
                        _error = "参数[" + argDef.Name + "]未提供.";
                        return false;
                    }

                    if (argDef.RefType == "file")
                    {
                        string filename = _argumentProvider.GetArg(argDef.Name).ToString();
                        if (!File.Exists(filename))
                        {
                            _error = "参数文件[" + Path.GetFileName(filename) + "]不存在.";
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private string[] SortFileName(string[] files)
        {
            List<RasterIdentify> idList = new List<RasterIdentify>();
            foreach (string fileName in files)
            {
                RasterIdentify id = new RasterIdentify(fileName);
                idList.Add(id);
            }
            idList.Sort(SortByOrbitTime);
            List<string> fileList = new List<string>();
            foreach (RasterIdentify id in idList)
            {
                fileList.Add(id.OriFileName[0]);
            }
            return fileList.ToArray();
        }

        private static int SortByOrbitTime(RasterIdentify fstId, RasterIdentify SedId)
        {
            if (fstId.OrbitDateTime.Equals(SedId.OrbitDateTime))
                return 0;
            else if (fstId.OrbitDateTime > SedId.OrbitDateTime)
                return 1;
            else
                return -1;
        }

        private IPixelFeatureMapper<T> MakeCompareRaster<T1,T>(string productIdentify, string fstFileName, string sedFileName, Func<T1, T1, T> function)
        {
            Dictionary<string, FilePrdMap> filePrdMap = new Dictionary<string, FilePrdMap>();
            filePrdMap.Add("backWaterPath", new FilePrdMap(fstFileName, 1, new VaildPra(float.MinValue, float.MaxValue), new int[] { 1 }));
            filePrdMap.Add("binWater", new FilePrdMap(sedFileName, 1, new VaildPra(float.MinValue, float.MaxValue), new int[] { 1 }));
            ITryCreateVirtualPrd tryVPrd = new TryCreateVirtualPrdByMultiFile();
            IVirtualRasterDataProvider vrd = tryVPrd.CreateVirtualRasterPRD(ref filePrdMap);
            if (vrd == null)
                throw new Exception("数据间无相交部分,无法创建虚拟数据提供者!");
            try
            {
                ArgumentProvider ap = new ArgumentProvider(vrd, null);
                RasterPixelsVisitor<T1> rpVisitor = new RasterPixelsVisitor<T1>(ap);
                IPixelFeatureMapper<T> result = new MemPixelFeatureMapper<T>(productIdentify, 1000, new Size(vrd.Width, vrd.Height), vrd.CoordEnvelope, vrd.SpatialRef);
                rpVisitor.VisitPixel(new int[] { filePrdMap["backWaterPath"].StartBand, filePrdMap["binWater"].StartBand},
                    (idx, values) =>
                    {
                        result.Put(idx, function(values[0],  values[1]));
                    });
                return result;
            }
            finally
            {
                vrd.Dispose();
            }
        }
    }
}