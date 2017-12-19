using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.Comm;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using GeoDo.RasterProject;
using System.Drawing;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.GDIPlus;
using GeoDo.RSS.Layout.DataFrm;
using System.Xml.Linq;
using System.ComponentModel;
using GeoDo.RSS.UI.AddIn.Theme;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using GeoDo.FileProject;
using CodeCell.AgileMap.Core;
using GeoDo.ProjectDefine;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    public class CmaMonitoringSubProduct : MonitoringSubProduct
    {
        public CmaMonitoringSubProduct()
            : base()
        { }

        public CmaMonitoringSubProduct(SubProductDef def)
            : base(def)
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            throw new NotImplementedException();
        }

        public override IExtractResult MakeExtProduct(IPixelIndexMapper piexd, Action<int, string> progressTracker)
        {
            return null;
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

        public string GetStringArgument(string arugmentName)
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

        protected string GetColorTableName(string attrubileName)
        {
            if (string.IsNullOrEmpty(attrubileName))
                return null;
            object obj = _argumentProvider.GetArg(attrubileName);
            if (obj == null)
                return null;
            return obj.ToString();
        }

        public DataIdentify GetDataIdentify()
        {
            object obj = _argumentProvider.GetArg("CurrentDataIdentify");
            if (obj == null)
                return null;
            return obj as DataIdentify;
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

        public RasterIdentify CreatRasterIndetifyId(string[] files, string productIdentify, string subProductindentify, DataIdentify di, string format, string cycFlag, string extinfo)
        {
            RasterIdentify id = new RasterIdentify(files);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = productIdentify;
            id.SubProductIdentify = subProductindentify;
            id.CYCFlag = cycFlag;
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

        public RasterIdentify CreatRasterIndetifyId(string[] files, string productIdentify, string subProductindentify, DataIdentify di, string format, string extinfo, string baseFile, out string replaceStr)
        {
            RasterIdentify id = new RasterIdentify(files);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = productIdentify;
            id.SubProductIdentify = subProductindentify;
            id.ExtInfos = extinfo;
            id.Format = format;
            RasterIdentify idBase = new RasterIdentify(baseFile);
            id.Satellite = idBase.Satellite;
            id.Sensor = idBase.Sensor;
            if (id.MinOrbitDate == idBase.OrbitDateTime)
                replaceStr = idBase.OrbitDateTime.ToString(_datFileDatestr) + "_" + id.MaxOrbitDate.ToString(_datFileDatestr);
            else if (id.MaxOrbitDate == idBase.OrbitDateTime)
                replaceStr = idBase.OrbitDateTime.ToString(_datFileDatestr) + "_" + id.MinOrbitDate.ToString(_datFileDatestr);
            else
                replaceStr = idBase.OrbitDateTime.ToString(_datFileDatestr);
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

        public IExtractResult ThemeGraphyResult(string extInfos)
        {
            #region 专题图生产机制 new//专题图产品定义在Instance节点中的
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            if (!string.IsNullOrWhiteSpace(instanceIdentify))
            {
                if (instanceIdentify == "ISOI" || instanceIdentify == "AEDG")//海冰冰面温度等值线专题图
                {
                    return VectoryThemeGraphy(extInfos);
                }
                SubProductInstanceDef instance = FindSubProductInstanceDefs(instanceIdentify);
                if (instance != null)
                {
                    if (instance.Name.Contains("多通道合成图"))
                        return ThemeGraphyMCSI(instance);
                    return ThemeGraphyByInstance(instance);
                }
            }
            #endregion
            return ThemeGraphy(extInfos);
        }

        protected SubProductInstanceDef GetSubProductInstanceByOutIdentify(string outIdentify)
        {
            if (string.IsNullOrEmpty(outIdentify))
                outIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            if (!string.IsNullOrWhiteSpace(outIdentify))
            {
                SubProductInstanceDef instance = FindSubProductInstanceDefs(outIdentify);
                if (instance != null)
                    return instance;
            }
            return null;
        }

        protected SubProductInstanceDef FindSubProductInstanceDefs(string instanceIdentify)
        {
            if (_subProductDef == null || _subProductDef.SubProductInstanceDefs == null)
                return null;
            foreach (SubProductInstanceDef instance in _subProductDef.SubProductInstanceDefs)
            {
                if (instance.OutFileIdentify == instanceIdentify)
                    return instance;
            }
            return null;
        }

        private IExtractResult ThemeGraphy(string extInfos)
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
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            string templatName = GetStringArgument("ThemeGraphTemplateName");
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
            {
                TrySetSelectedPrimaryFiles(ref files);
            }
            if (files == null || files.Length == 0)
                return null;
            UpdateFilesByInstance(ref files, outFileIdentify);
            string colorTabelName = GetColorTableName("colortablename");
            if (string.IsNullOrEmpty(extInfos))
                extInfos = GetStringArgument("extinfo");
            string isFixImageRegion = GetStringArgument("FixImageRegion");
            if (isFixImageRegion == "true")
            {
                (tgg as CmaThemeGraphGenerator).IsFitToTemplateWidth = false;
                PrjEnvelope useRegionGeo = LoadThemplateEnvelope(templatName);
                (tgg as CmaThemeGraphGenerator).SetEnvelope(new Layout.GxdEnvelope(useRegionGeo.MinX, useRegionGeo.MaxX, useRegionGeo.MinY, useRegionGeo.MaxY));
            }
            else
            {
                PrjEnvelope useRegionGeo = GetArgToPrjEnvelope("UseRegion");
                if (useRegionGeo != null)  //需要限定输出范围
                {
                    (tgg as CmaThemeGraphGenerator).SetEnvelope(new Layout.GxdEnvelope(useRegionGeo.MinX, useRegionGeo.MaxX, useRegionGeo.MinY, useRegionGeo.MaxY));
                }
            }
            // by chennan 专题图增加云、烟信息
            string dstFile = AddClmOrSmok(files[0]);

            tgg.Generate(dstFile, templatName, MasicAOI(aoi, ref extInfos), extInfos, outFileIdentify, colorTabelName);
            string resultFilename = tgg.Save();

            if (string.IsNullOrEmpty(resultFilename))
                return null;
            return new FileExtractResult(outFileIdentify, resultFilename);
        }

        private void UpdateFilesByInstance(ref string[] files, string outFileIdentify)
        {
            SubProductInstanceDef def = GetSubProductInstanceByOutIdentify(outFileIdentify);
            if (def == null)
                return;
            int startIndex = def.FileProvider.IndexOf(":") + 1;
            string subPrcIdentify = def.FileProvider.Substring(startIndex);
            MIFCommAnalysis.UpdateFilesByDataIdentify(ref files, subPrcIdentify);
        }

        protected bool TrySetSelectedPrimaryFiles(ref string[] files)
        {
            ISmartSession session = _argumentProvider.GetArg("SmartSession") as ISmartSession;
            if (session == null)
                return false;
            IWorkspace wks = (session.MonitoringSession as IMonitoringSession).Workspace;
            if (wks == null)
                return false;
            ICatalog catalog = wks.ActiveCatalog;
            if (catalog == null)
                return false;
            string[] fnames = catalog.GetSelectedFiles();
            if (fnames == null || fnames.Length == 0)
                return false;
            files = fnames;
            _argumentProvider.SetArg("SelectedPrimaryFiles", fnames);
            return true;
        }

        /// <summary>
        /// 根据Instance生成专题图
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        protected IExtractResult ThemeGraphyByInstance(SubProductInstanceDef instance)
        {
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            string templatName = instance.LayoutName;
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
            {
                TrySetSelectedPrimaryFiles(ref files);
            }
            if (files == null || files.Length == 0)
                return null;
            UpdateFilesByInstance(ref files, outFileIdentify);
            // by chennan 专题图增加云、烟信息
            string dstFile = AddClmOrSmok(files[0], instance.isExtMethod);

            //by chennan 20150512 新增产品投影转换
            if (!string.IsNullOrEmpty(instance.DefaultProj))
            {
                GeoDo.Project.ISpatialReference[] customSpatialReferences = SpatialReferenceSelection.CustomSpatialReferences;
                string[] outfiles = null;
                foreach (GeoDo.Project.ISpatialReference item in customSpatialReferences)
                {
                    if (item.Name == instance.DefaultProj)
                    {
                        outfiles = ProjectTrance(dstFile, item);
                        break;
                    }
                }
                if (outfiles != null && outfiles.Length == 1)
                    dstFile = outfiles[0];
            }
            //

            string resultFilename = GenerateRasterThemeGraphy(dstFile, instance);
            if (string.IsNullOrEmpty(resultFilename))
                return null;
            return new FileExtractResult(outFileIdentify, resultFilename);
        }

        private string[] ProjectTrance(string fileName, GeoDo.Project.ISpatialReference proj)
        {
            string errorMsg;
            string[] outFiles = null;
            ProjectionFactory quick = new ProjectionFactory();
            string prjFlag = GenericFilename.GetProjectionIdentify(proj.ProjectionCoordSystem.Name.Name);
            string outdir = Path.Combine(MifEnvironment.GetTempDir(), Path.GetFileNameWithoutExtension(fileName).Replace("_" + prjFlag, "")
                            + "_" + prjFlag + Path.GetExtension(fileName));
            PrjOutArg prjOutArg = new PrjOutArg(proj, null, 0, 0, outdir);
            outFiles = quick.Project(fileName, prjOutArg, new Action<int, string>(OnProgress), out errorMsg);
            return outFiles == null ? null : outFiles;
        }

        private void OnProgress(int progerss, string text)
        {

        }

        private string GenerateRasterThemeGraphy(string dstFile, SubProductInstanceDef instance)
        {
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            string templatName = instance.LayoutName;
            string aoiTemplateName = string.Empty;
            Dictionary<string, int[]> aoi = null;
            GetAOI(out aoiTemplateName, out aoi);
            string extInfos = GetStringArgument("extinfo");
            int[] themeAoi = MasicAOI(aoi, ref extInfos);
            string colorTabelName = null;
            if (!string.IsNullOrEmpty(instance.ColorTableName))
                colorTabelName = "colortablename=" + instance.ColorTableName;
            ILayoutTemplate t = GetTemplateByArg(templatName);
            if (t == null)
                return null;
            ILayout layout = t.Layout;
            int width, height;
            GeoDo.RSS.Core.DF.CoordEnvelope envelope = null;
            string fileOpenArgs = GetStringArgument("fileOpenArgs");
            string SpatialRef = null;
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(dstFile, fileOpenArgs) as IRasterDataProvider)
            {
                width = dataPrd.Width;
                height = dataPrd.Height;
                envelope = dataPrd.CoordEnvelope;
                SpatialRef = dataPrd.SpatialRef.ToWKTString();
            }
            if (instance.isOriginal)//原始分辨率
                ChangeTemplateSize(layout, envelope, width, height);
            else
                FitSizeToTemplateWidth(layout, width, height);
            CreateRasterIdentifyCopy(dstFile);
            //生成文档，并应用变量
            TryApplyVars(t);
            ApplyAttributesOfLayoutTemplate(t);

            IGxdDocument gxdDoc = CreateDocument(t, string.IsNullOrEmpty(colorTabelName) ? null : colorTabelName);
            IGxdDataFrame gxdDf = gxdDoc.DataFrames.Count > 0 ? gxdDoc.DataFrames[0] : null;
            if (gxdDf != null)
            {
                string[] arguments = new string[] { string.IsNullOrEmpty(colorTabelName) ? null : colorTabelName, fileOpenArgs };
                IGxdRasterItem rst = new GxdRasterItem(dstFile, colorTabelName, arguments, colorTabelName);//这里传具体的色标定义标识
                gxdDf.GxdRasterItems.Add(rst);
                gxdDf.SpatialRef = SpatialRef;
                TryGeneratreAOISecondaryFile(themeAoi, rst);
            }
            TrySetAttributesOfElements(gxdDoc);
            string docFname = GetOutputGxdFileName(_rstIdentifyCopy, _subProductDef.ProductDef.Identify, outFileIdentify);
            gxdDoc.SaveAs(docFname);
            return docFname;
        }

        private string AddClmOrSmok(string srcFile)
        {
            return AddClmOrSmok(srcFile, true);
        }

        private string AddClmOrSmok(string srcFile, bool isExtMethod)
        {
            if (!isExtMethod)
                return srcFile;
            RasterIdentify rid = new RasterIdentify(srcFile);
            if (rid == null || string.IsNullOrEmpty(rid.SubProductIdentify))
                return srcFile;
            string identify = rid.SubProductIdentify;
            string clmFile = srcFile.ToUpper().Replace("_" + identify.ToUpper() + "_", "_0CLM_");
            string smokeFile = srcFile.ToUpper().Replace("_" + identify.ToUpper() + "_", "_SMOK_");
            string firGFile = srcFile.ToUpper().Replace("_" + identify.ToUpper() + "_", "_FIRG_");
            string dstDir = srcFile.Substring(0, Path.GetDirectoryName(srcFile).LastIndexOf("\\")) + "\\云烟数据\\";
            if (!File.Exists(clmFile) && !File.Exists(smokeFile) && !File.Exists(firGFile))
            {
                return srcFile;
            }
            else
            {
                if (!Directory.Exists(dstDir))
                    Directory.CreateDirectory(dstDir);
                string dstFile = Path.Combine(dstDir, Path.GetFileName(srcFile));
                using (IRasterDataProvider provider = GeoDataDriver.Open(srcFile) as IRasterDataProvider)
                {
                    if (provider.DataType == enumDataType.Int16)
                        CreateCLMSMOKFileInt16(srcFile, clmFile, smokeFile, firGFile, ref dstFile, provider, 1);
                    else if (provider.DataType == enumDataType.UInt16)
                        CreateCLMSMOKFileUInt16(srcFile, clmFile, smokeFile, firGFile, ref dstFile, provider, 1);
                    else if (provider.DataType == enumDataType.Float)
                        CreateCLMSMOKFileFloat(clmFile, smokeFile, firGFile, ref dstFile, provider, 1);
                    else
                        return srcFile;
                }
                return dstFile;
            }
        }

        private void CreateCLMSMOKFileInt16(string srcFile, string clmFile, string smokeFile, string firGFile, ref string dstFile, IRasterDataProvider provider, int filenameIndex)
        {
            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            int index = -1;
            Dictionary<int, Int16> maperBandDic = new Dictionary<int, Int16>();
            try
            {
                IRasterDataProvider srcRaster = RasterDataDriver.Open(srcFile) as IRasterDataProvider;
                RasterMaper brm = new RasterMaper(srcRaster, new int[] { 1 });
                rms.Add(brm);
                maperBandDic.Add(++index, 1);
                if (File.Exists(clmFile))
                {
                    IRasterDataProvider clmRaster = RasterDataDriver.Open(clmFile) as IRasterDataProvider;
                    RasterMaper rm = new RasterMaper(clmRaster, new int[] { 1 });
                    rms.Add(rm);
                    maperBandDic.Add(++index, 9999);
                }
                if (File.Exists(smokeFile))
                {
                    IRasterDataProvider smokeRaster = RasterDataDriver.Open(smokeFile) as IRasterDataProvider;
                    RasterMaper rm = new RasterMaper(smokeRaster, new int[] { 1 });
                    rms.Add(rm);
                    maperBandDic.Add(++index, 10000);
                }
                if (File.Exists(firGFile))
                {
                    IRasterDataProvider fireGRaster = RasterDataDriver.Open(firGFile) as IRasterDataProvider;
                    RasterMaper rm = new RasterMaper(fireGRaster, new int[] { 1 });
                    rms.Add(rm);
                    maperBandDic.Add(++index, 10001);
                }
                using (IRasterDataProvider outRaster = CreateOutRaster(dstFile, rms.ToArray(), enumDataType.Int16))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<Int16, Int16> rfr = null;
                    rfr = new RasterProcessModel<Int16, Int16>();
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        int y = rvInVistor[0].IndexY * rvInVistor[0].Raster.Width;
                        int count = fileIns.Length;
                        for (int dataIndex = 0; dataIndex < dataLength; dataIndex++)
                        {
                            rvOutVistor[0].RasterBandsData[0][dataIndex] = rvInVistor[0].RasterBandsData[0][dataIndex];
                            for (int i = 1; i < count; i++)
                            {
                                if (rvInVistor[i].RasterBandsData[0][dataIndex] != 0)
                                    rvOutVistor[0].RasterBandsData[0][dataIndex] = maperBandDic[i];
                            }
                        }
                    }));
                    //执行
                    rfr.Excute(0);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("正由另一进程使用，因此该进程无法访问此文件") == -1)
                {
                    string usedfile = Path.GetFileNameWithoutExtension(dstFile);
                    filenameIndex++;
                    dstFile = Path.Combine(Path.GetDirectoryName(dstFile), (usedfile.IndexOf("(") != -1 ? usedfile.Replace("(" + (filenameIndex - 1) + ")", "(" + filenameIndex + ")") : usedfile + "(" + filenameIndex + ")") + Path.GetExtension(dstFile));
                    CreateCLMSMOKFileInt16(srcFile, clmFile, smokeFile, firGFile, ref dstFile, provider, filenameIndex);
                    //throw new Exception(ex.Message);
                }
            }
            finally
            {
                foreach (RasterMaper rm in rms)
                {
                    rm.Raster.Dispose();
                }
            }
        }

        private void CreateCLMSMOKFileUInt16(string srcFile, string clmFile, string smokeFile, string firGFile, ref string dstFile, IRasterDataProvider provider, int filenameIndex)
        {
            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            int index = -1;
            Dictionary<int, UInt16> maperBandDic = new Dictionary<int, UInt16>();
            try
            {
                IRasterDataProvider srcRaster = RasterDataDriver.Open(srcFile) as IRasterDataProvider;
                RasterMaper brm = new RasterMaper(srcRaster, new int[] { 1 });
                rms.Add(brm);
                maperBandDic.Add(++index, 1);
                if (File.Exists(clmFile))
                {
                    IRasterDataProvider clmRaster = RasterDataDriver.Open(clmFile) as IRasterDataProvider;
                    RasterMaper rm = new RasterMaper(clmRaster, new int[] { 1 });
                    rms.Add(rm);
                    maperBandDic.Add(++index, 9999);
                }
                if (File.Exists(smokeFile))
                {
                    IRasterDataProvider smokeRaster = RasterDataDriver.Open(smokeFile) as IRasterDataProvider;
                    RasterMaper rm = new RasterMaper(smokeRaster, new int[] { 1 });
                    rms.Add(rm);
                    maperBandDic.Add(++index, 10000);
                }
                if (File.Exists(firGFile))
                {
                    IRasterDataProvider fireGRaster = RasterDataDriver.Open(firGFile) as IRasterDataProvider;
                    RasterMaper rm = new RasterMaper(fireGRaster, new int[] { 1 });
                    rms.Add(rm);
                    maperBandDic.Add(++index, 10001);
                }
                using (IRasterDataProvider outRaster = CreateOutRaster(dstFile, rms.ToArray(), enumDataType.UInt16))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<UInt16, UInt16> rfr = null;
                    rfr = new RasterProcessModel<UInt16, UInt16>();
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<UInt16, UInt16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        int y = rvInVistor[0].IndexY * rvInVistor[0].Raster.Width;
                        int count = fileIns.Length;
                        for (int dataIndex = 0; dataIndex < dataLength; dataIndex++)
                        {
                            rvOutVistor[0].RasterBandsData[0][dataIndex] = rvInVistor[0].RasterBandsData[0][dataIndex];
                            for (int i = 1; i < count; i++)
                            {
                                if (rvInVistor[i].RasterBandsData[0][dataIndex] != 0)
                                    rvOutVistor[0].RasterBandsData[0][dataIndex] = maperBandDic[i];
                            }
                        }
                    }));
                    //执行
                    rfr.Excute(0);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("正由另一进程使用，因此该进程无法访问此文件") == -1)
                {
                    string usedfile = Path.GetFileNameWithoutExtension(dstFile);
                    filenameIndex++;
                    dstFile = Path.Combine(Path.GetDirectoryName(dstFile), (usedfile.IndexOf("(") != -1 ? usedfile.Replace("(" + (filenameIndex - 1) + ")", "(" + filenameIndex + ")") : usedfile + "(" + filenameIndex + ")") + Path.GetExtension(dstFile));
                    CreateCLMSMOKFileUInt16(srcFile, clmFile, smokeFile, firGFile, ref dstFile, provider, filenameIndex);
                }
            }
            finally
            {
                foreach (RasterMaper rm in rms)
                {
                    rm.Raster.Dispose();
                }
            }
        }

        private void CreateCLMSMOKFileFloat(string clmFile, string smokeFile, string firGFile, ref string dstFile, IRasterDataProvider provider, int filenameIndex)
        {
            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            int index = -1;
            Dictionary<int, float> maperBandDic = new Dictionary<int, float>();
            try
            {
                if (File.Exists(clmFile))
                {
                    IRasterDataProvider clmRaster = RasterDataDriver.Open(clmFile) as IRasterDataProvider;
                    RasterMaper rm = new RasterMaper(clmRaster, new int[] { 1 });
                    rms.Add(rm);
                    maperBandDic.Add(++index, 9999f);
                }
                if (File.Exists(smokeFile))
                {
                    IRasterDataProvider smokeRaster = RasterDataDriver.Open(smokeFile) as IRasterDataProvider;
                    RasterMaper rm = new RasterMaper(smokeRaster, new int[] { 1 });
                    rms.Add(rm);
                    maperBandDic.Add(++index, 10000f);
                }
                if (File.Exists(firGFile))
                {
                    IRasterDataProvider fireGRaster = RasterDataDriver.Open(firGFile) as IRasterDataProvider;
                    RasterMaper rm = new RasterMaper(fireGRaster, new int[] { 1 });
                    rms.Add(rm);
                    maperBandDic.Add(++index, 10001f);
                }
                using (IRasterDataProvider outRaster = CreateOutRaster(dstFile, rms.ToArray(), enumDataType.Float))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<Int16, float> rfr = null;
                    rfr = new RasterProcessModel<Int16, float>();
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, float>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        int y = rvInVistor[0].IndexY * rvInVistor[0].Raster.Width;
                        int count = fileIns.Length;
                        for (int dataIndex = 0; dataIndex < dataLength; dataIndex++)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                if (rvInVistor[i].RasterBandsData[0][dataIndex] != 0)
                                    rvOutVistor[0].RasterBandsData[0][dataIndex] = maperBandDic[i];
                            }
                        }
                    }));
                    //执行
                    rfr.Excute(0);
                }
                dstFile = AddValue(provider, dstFile);
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("正由另一进程使用，因此该进程无法访问此文件") == -1)
                {
                    string usedfile = Path.GetFileNameWithoutExtension(dstFile);
                    filenameIndex++;
                    dstFile = Path.Combine(Path.GetDirectoryName(dstFile), (usedfile.IndexOf("(") != -1 ? usedfile.Replace("(" + (filenameIndex - 1) + ")", "(" + filenameIndex + ")") : usedfile + "(" + filenameIndex + ")") + Path.GetExtension(dstFile));
                    CreateCLMSMOKFileFloat(clmFile, smokeFile, firGFile, ref dstFile, provider, filenameIndex);
                }

            }
            finally
            {
                foreach (RasterMaper rm in rms)
                {
                    rm.Raster.Dispose();
                }
            }
        }

        public IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] inrasterMaper, enumDataType dataType)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            GeoDo.RSS.Core.DF.CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Union(inRaster.Raster.CoordEnvelope);
            }
            float resX = inrasterMaper[0].Raster.ResolutionX;
            float resY = inrasterMaper[0].Raster.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, dataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        //private void CreateCLMSMOKFileInt16(string srcFile, string clmFile, string smokeFile, string firGFile, string dstFile, IRasterDataProvider provider)
        //{
        //    try
        //    {
        //        IInterestedRaster<Int16> iir = new InterestedRaster<Int16>(dstFile, new Size(provider.Width, provider.Height), provider.CoordEnvelope, provider.SpatialRef);
        //        ArgumentProvider ap = new ArgumentProvider(provider, null);
        //        AddValue<Int16>(provider, srcFile, (index, value) =>
        //        {
        //            if ((Int16)value != 0)
        //                iir.Put(index, value);
        //        });
        //        if (File.Exists(clmFile))
        //        {
        //            AddValue<Int16>(provider, clmFile, (index, value) =>
        //            {
        //                if ((Int16)value == 1)
        //                    iir.Put(index, 9999);
        //            });
        //        }
        //        if (File.Exists(smokeFile))
        //        {
        //            AddValue<Int16>(provider, smokeFile, (index, value) =>
        //            {
        //                if ((Int16)value == 1)
        //                    iir.Put(index, 10000);
        //            });
        //        }
        //        if (File.Exists(firGFile))
        //        {
        //            AddValue<Int16>(provider, firGFile, (index, value) =>
        //            {
        //                if ((Int16)value == 1)
        //                    iir.Put(index, 10001);
        //            });
        //        }
        //        provider.Dispose();
        //        //AddValue<Int16>(provider, srcFile, (index, value) =>
        //        //{
        //        //    if ((Int16)value != 0)
        //        //        iir.Put(index, value);
        //        //});
        //        iir.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.Message.IndexOf("正由另一进程使用，因此该进程无法访问此文件") == -1)
        //            throw new Exception(ex.Message);
        //    }
        //}

        //private void CreateCLMSMOKFileUInt16(string srcFile, string clmFile, string smokeFile, string firGFile, string dstFile, IRasterDataProvider provider)
        //{
        //    try
        //    {
        //        IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(dstFile, new Size(provider.Width, provider.Height), provider.CoordEnvelope, provider.SpatialRef);
        //        ArgumentProvider ap = new ArgumentProvider(provider, null);
        //        AddValue<UInt16>(provider, srcFile, (index, value) =>
        //        {
        //            if ((UInt16)value != 0)
        //                iir.Put(index, value);
        //        });
        //        if (File.Exists(clmFile))
        //        {
        //            AddValue<UInt16>(provider, clmFile, (index, value) =>
        //            {
        //                if ((UInt16)value == 1)
        //                    iir.Put(index, 9999);
        //            });
        //        }
        //        if (File.Exists(smokeFile))
        //        {
        //            AddValue<UInt16>(provider, smokeFile, (index, value) =>
        //            {
        //                if ((UInt16)value == 1)
        //                    iir.Put(index, 10000);
        //            });
        //        }
        //        if (File.Exists(firGFile))
        //        {
        //            AddValue<UInt16>(provider, firGFile, (index, value) =>
        //            {
        //                if ((UInt16)value == 1)
        //                    iir.Put(index, 10001);
        //            });
        //        }
        //        provider.Dispose();
        //        iir.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.Message.IndexOf("正由另一进程使用，因此该进程无法访问此文件") == -1)
        //            throw new Exception(ex.Message);
        //    }
        //}

        //private void CreateCLMSMOKFileFloat(string clmFile, string smokeFile, string firGFile, ref string dstFile, IRasterDataProvider provider)
        //{
        //    try
        //    {
        //        IInterestedRaster<float> iir = new InterestedRaster<float>(dstFile, new Size(provider.Width, provider.Height), provider.CoordEnvelope, provider.SpatialRef);
        //        ArgumentProvider ap = new ArgumentProvider(provider, null);
        //        if (File.Exists(clmFile))
        //            AddValue<Int16>(provider, clmFile, (index, value) =>
        //            {
        //                if ((Int16)value == 1)
        //                    iir.Put(index, 9999f);
        //            });
        //        if (File.Exists(smokeFile))
        //            AddValue<Int16>(provider, smokeFile, (index, value) =>
        //            {
        //                if ((Int16)value == 1)
        //                    iir.Put(index, 10000f);
        //            });
        //        if (File.Exists(firGFile))
        //            AddValue<Int16>(provider, firGFile, (index, value) =>
        //            {
        //                if ((Int16)value == 1)
        //                    iir.Put(index, 10001f);
        //            });
        //        //修改以适应蓝藻覆盖度（无效值为-9999，而不为0）
        //        //AddValue<float>(provider.fileName, (index, value) =>
        //        //{
        //        //    if ((float)value != 0)
        //        //        iir.Put(index, value);
        //        //});
        //        iir.Dispose();
        //        dstFile = AddValue(provider, dstFile);
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.Message.IndexOf("正由另一进程使用，因此该进程无法访问此文件") == -1)
        //            throw new Exception(ex.Message);
        //    }
        //}

        private void AddValue<T>(IRasterDataProvider srcProvider, string clmFile, Action<int, T> where)
        {
            using (IRasterDataProvider provider = GeoDataDriver.Open(clmFile) as IRasterDataProvider)
            {
                //临时使用 文件含有AOI信息后可以不用
                GeoDo.RSS.Core.DF.CoordEnvelope envelope = srcProvider.CoordEnvelope.Intersect(provider.CoordEnvelope);
                if (envelope == null || (envelope.Width / srcProvider.ResolutionX < 5 && envelope.Height / srcProvider.ResolutionY < 5))
                    return;
                //
                ArgumentProvider ap = new ArgumentProvider(provider, null);
                RasterPixelsVisitor<T> visitor = new RasterPixelsVisitor<T>(ap);
                visitor.VisitPixel(new int[] { 1 }, (index, values) =>
                {
                    where(index, values[0]);
                });
            }
        }

        private string AddValue(IRasterDataProvider srcFile, string dstFile)
        {
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(dstFile) as IRasterDataProvider)
            {
                RasterMaper[] rasterInputMaps = new RasterMaper[] { new RasterMaper(srcFile, new int[] { 1 }) ,
                                                                    new RasterMaper(dataPrd,new int[]{1})};
                string fileName = Path.Combine(Path.GetDirectoryName(dstFile), Path.GetFileNameWithoutExtension(dstFile) + "_temp.dat");
                IRasterDataProvider outPrd = null;
                IRasterDataDriver driver = null;
                try
                {
                    driver = GeoDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
                    string mapInfo = dataPrd.CoordEnvelope.ToMapInfoString(new Size(dataPrd.Width, dataPrd.Height));
                    outPrd = driver.Create(fileName, dataPrd.Width, dataPrd.Height, 1, enumDataType.Float, mapInfo);
                    RasterMaper[] rasterOutputMaps = new RasterMaper[] { new RasterMaper(outPrd, new int[] { 1 }) };
                    RasterProcessModel<float, float> rfr = new RasterProcessModel<float, float>(null);
                    rfr.SetRaster(rasterInputMaps, rasterOutputMaps);
                    rfr.RegisterCalcModel(new RasterCalcHandler<float, float>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        if (rvInVistor[0].RasterBandsData == null || rvInVistor[1].RasterBandsData == null ||
                            rvInVistor[0].RasterBandsData[0] == null || rvInVistor[1].RasterBandsData[0] == null)
                            return;
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        for (int index = 0; index < dataLength; index++)
                        {
                            float srcvalue = rvInVistor[0].RasterBandsData[0][index];
                            float inValue = rvInVistor[1].RasterBandsData[0][index];
                            if ((srcvalue != 0 && srcvalue != -9999))
                                rvOutVistor[0].RasterBandsData[0][index] = srcvalue;
                            else if ((srcvalue == -9999 || srcvalue == 0) && inValue != 0)
                                rvOutVistor[0].RasterBandsData[0][index] = inValue;
                            else
                                rvOutVistor[0].RasterBandsData[0][index] = srcvalue;
                        }
                    }));
                    //执行
                    rfr.Excute();
                    return fileName;
                }
                finally
                {
                    if (outPrd != null)
                        outPrd.Dispose();
                    if (driver != null)
                        driver.Dispose();
                }
            }
        }

        private PrjEnvelope LoadThemplateEnvelope(string templatName)
        {
            string template = CmaThemeGraphGenerator.FindTemplate(templatName);
            if (string.IsNullOrWhiteSpace(template))
                return null;
            string xml = Path.ChangeExtension(template, ".xml");
            if (!File.Exists(xml))
                return null;
            return ParseEnvelope(xml);
        }

        private static PrjEnvelope ParseEnvelope(string xml)
        {
            XElement ele = XElement.Load(xml);
            if (ele == null)
                return null;
            XElement xElement = ele.Element("Envelope");
            if (xElement == null || xElement.Value == null)
                return null;
            return new PrjEnvelope(double.Parse(xElement.Attribute("minx").Value)
                                             , double.Parse(xElement.Attribute("maxx").Value)
                                             , double.Parse(xElement.Attribute("miny").Value)
                                             , double.Parse(xElement.Attribute("maxy").Value));
        }

        #region 监测示意图（原始影像+判识结果）
        public virtual IExtractResult ThemeGraphyMCSIDBLV(SubProductInstanceDef instance)
        {
            try
            {
                string templatName = instance.LayoutName;
                string instanceIdentify = instance.OutFileIdentify;
                string instanceName = instance.Name;
                bool isOrigial = instance.isOriginal;
                bool isFixTempleSize = instance.isFixTempleSize;
                bool isCurrentView = instance.isCurrentView;
                string productIdentify = _subProductDef.ProductDef.Identify;
                string colortablename = instance.ColorTableName;
                string outdir = instance.OutDir;
                IRasterDrawing rasterDrawing = GetRasterDrawingArugment();  //多通道合成图 需要获取当前打开的图像。
                if (rasterDrawing == null)
                    return null;
                string fname = CreateMonitorBitmapAddExtructResult(rasterDrawing, templatName, productIdentify, instanceIdentify, isOrigial, isFixTempleSize, isCurrentView, colortablename, outdir);
                IExtractResult er = new FileExtractResult(instanceIdentify, fname) as IExtractResult;
                return er;
            }
            finally
            {
            }

        }

        private string CreateMonitorBitmapAddExtructResult(IRasterDrawing rasterDrawing, string templatName, string productIdentify, string instanceIdentify, bool isOrigial, bool isFixTempleSize, bool isCurrentView, string colortablename)
        {
            //创建用于生成多通道合成图的Bitmap，及其地理信息
            string bitmapFileName = null;
            int width = 0;
            int height = 0;
            bool isOk = GetOrginBmpAndSave(rasterDrawing, out bitmapFileName, out width, out height, isCurrentView);
            CreatWorldFile(bitmapFileName, rasterDrawing, width, height);
            //加载模版
            ILayoutTemplate temp = GetTemplateByArg(templatName);
            if (temp == null)
                return null;
            ILayout layout = temp.Layout;
            if (isOrigial)
                ChangeTemplateSize(layout, rasterDrawing.DataProvider.CoordEnvelope, width, height);
            else
            {
                FitSizeToTemplateWidth(layout, width, height, isFixTempleSize);
            }
            //生成文档，并应用变量
            IGxdDocument gxdDoc = GetDocument(bitmapFileName, temp);
            TryApplyVars(gxdDoc.GxdTemplateHost.LayoutTemplate, rasterDrawing);
            TrySetAttributesOfElements(gxdDoc);
            string[] dstFile = GetStringArray("SelectedPrimaryFiles");
            if (dstFile != null && dstFile.Length >= 1)
            {
                IGxdDataFrame gxdDf = gxdDoc.DataFrames.Count > 0 ? gxdDoc.DataFrames[0] : null;
                if (gxdDf != null)
                {
                    IGxdRasterItem rst = new GxdRasterItem(dstFile[0], string.IsNullOrEmpty(colortablename) ? null : "colortablename=" + colortablename);//这里传具体的色标定义标识
                    gxdDf.GxdRasterItems.Add(rst);
                }
            }
            string rasterFname = rasterDrawing.FileName;
            string fname = GetOutputGxdFileName(new RasterIdentify(rasterFname), productIdentify, instanceIdentify);
            gxdDoc.SaveAs(fname);
            return fname;
        }
        /// <summary>
        /// 重载增加文件路径判断
        /// </summary>
        private string CreateMonitorBitmapAddExtructResult(IRasterDrawing rasterDrawing, string templatName, string productIdentify, string instanceIdentify, bool isOrigial, bool isFixTempleSize, bool isCurrentView, string colortablename, string outdir)
        {
            //创建用于生成多通道合成图的Bitmap，及其地理信息
            string bitmapFileName = null;
            int width = 0;
            int height = 0;
            bool isOk = GetOrginBmpAndSave(rasterDrawing, out bitmapFileName, out width, out height, isCurrentView);
            CreatWorldFile(bitmapFileName, rasterDrawing, width, height);
            string[] dstFile = GetStringArray("SelectedPrimaryFiles");
            //加载模版
            ILayoutTemplate temp = GetTemplateByArg(templatName);
            if (temp == null)
                return null;
            ILayout layout = temp.Layout;
            if (isOrigial)
                ChangeTemplateSize(layout, rasterDrawing.DataProvider.CoordEnvelope, width, height);
            else
            {
                FitSizeToTemplateWidth(layout, width, height, isFixTempleSize);
                ApplyAttributesOfLayoutTemplate(temp);
            }
            //生成文档，并应用变量
            IGxdDocument gxdDoc = GetDocument(bitmapFileName, temp);
            TryApplyVars(gxdDoc.GxdTemplateHost.LayoutTemplate, rasterDrawing, dstFile);

            ThemeGraphRegion grapRegion = null;

            grapRegion = new ThemeGraphRegion();
            grapRegion.PrjEnvelopeItems = new PrjEnvelopeItem[] { new PrjEnvelopeItem("", new PrjEnvelope(rasterDrawing.DataProvider.CoordEnvelope.MinX, rasterDrawing.DataProvider.CoordEnvelope.MaxX, rasterDrawing.DataProvider.CoordEnvelope.MinY, rasterDrawing.DataProvider.CoordEnvelope.MaxY)) };
            grapRegion.ProductIdentify = productIdentify;
            grapRegion.Enable = true;
            grapRegion.SelectedIndex = 0;


            List<string> infoContents = new List<string>();
            infoContents.Add(string.Format("影像范围：{0},{1},{2},{3}", rasterDrawing.DataProvider.CoordEnvelope.MinX, rasterDrawing.DataProvider.CoordEnvelope.MaxX, rasterDrawing.DataProvider.CoordEnvelope.MinY, rasterDrawing.DataProvider.CoordEnvelope.MaxY));

            if (isFixTempleSize)
                grapRegion = null;
            TrySetAttributesOfElements(gxdDoc, grapRegion, infoContents);

            if (dstFile != null && dstFile.Length >= 1)
            {
                IGxdDataFrame gxdDf = gxdDoc.DataFrames.Count > 0 ? gxdDoc.DataFrames[0] : null;
                if (gxdDf != null)
                {
                    IGxdRasterItem rst = new GxdRasterItem(dstFile[0], string.IsNullOrEmpty(colortablename) ? null : "colortablename=" + colortablename);//这里传具体的色标定义标识
                    gxdDf.GxdRasterItems.Add(rst);
                    IRasterDataProvider rd = GeoDataDriver.Open(dstFile[0]) as IRasterDataProvider;
                    gxdDf.SpatialRef = rd.SpatialRef.ToWKTString();
                    if (rd != null)
                    {
                        infoContents.Add(string.Format("数据范围：{0},{1},{2},{3}", rd.CoordEnvelope.MinX, rd.CoordEnvelope.MaxX, rd.CoordEnvelope.MinY, rd.CoordEnvelope.MaxY));
                        rd.Dispose();
                    }
                }
            }
            RasterIdentify rid = new RasterIdentify(rasterDrawing.DataProvider);
            string fname = GetOutputGxdFileName(rid, productIdentify, instanceIdentify);

            //修改路径前缀
            if (!string.IsNullOrEmpty(outdir))
            {
                string olddir = fname.Substring(0, fname.LastIndexOf('\\'));

                string temprelslution = string.Empty;

                if (instanceIdentify == "TNCI")
                {
                    temprelslution = "9999";//固定产品使用固定分辨率字符
                }
                else
                {
                    temprelslution = rid.Resolution.Replace("M", "");
                }
                if (!string.IsNullOrEmpty(rid.Resolution))
                {

                    if (int.Parse(rid.Resolution.Replace("M", "")) >= 1000 && (instanceIdentify != "TNCI"))
                    {
                        temprelslution = "1000";
                    }
                }
                string filename = fname.Substring(fname.LastIndexOf('\\') + 1, fname.LastIndexOf('.') - fname.LastIndexOf('\\') - 1);
                string newfilename = string.Format("SEVP_NSMC_{0}_{1}_{2}_AR2_L00_PY{3}_{4}00000", GetFileNamePartOne(productIdentify), rid.Satellite, GetFileNamePartTwo(productIdentify), temprelslution, rid.OrbitDateTime.AddHours(8).ToString("yyyyMMddHHmm"));
                if (Directory.Exists(outdir))
                {
                    //更换路径
                    fname = fname.Replace(olddir, outdir);

                }
                //更换文件名
                fname = fname.Replace(filename, newfilename);
            }
            //重命名文件名，命名规则参照 环境气象频道产品命名规范-大雾沙尘.docx
            gxdDoc.SaveAs(fname);
            SaveInfoText(fname, infoContents);
            return fname;
        }

        private void SaveInfoText(string fname, List<string> infoContents)
        {
            string infoFilename = Path.Combine(Path.GetDirectoryName(fname), Path.GetFileNameWithoutExtension(fname) + ".infoTxt");
            if (infoContents == null || infoContents.Count == 0)
                return;
            File.WriteAllLines(infoFilename, infoContents.ToArray(), Encoding.Default);
        }

        /// 重载 方法CreateMonitorBitmapAddExtructResult 子方法用以获取文件名称组成部分
        private string GetFileNamePartOne(string productIdentify)
        {
            if (string.IsNullOrEmpty(productIdentify))
            {
                return "Null";
            }
            else
            {
                switch (productIdentify)
                {
                    case "FOG": return "WXFO";
                    case "DST": return "WXSA";
                    default: return "Null";
                }
            }
        }
        private string GetFileNamePartTwo(string productIdentify)
        {

            if (string.IsNullOrEmpty(productIdentify))
            {
                return "Null";
            }
            else
            {
                switch (productIdentify)
                {
                    case "FOG": return "EFG";
                    case "DST": return "EDST";
                    default: return "Null";
                }
            }
        }

        private string SearchCurrentExtractResult(string rasterFileName)
        {
            RasterIdentify rasterId = new RasterIdentify(rasterFileName);
            rasterId.ProductIdentify = _subProductDef.ProductDef.Identify;
            rasterId.SubProductIdentify = "DBLV";
            string dblvFname = rasterId.ToWksFullFileName(".dat");
            return dblvFname;
        }


        #endregion

        #region 多通道合成图专题图
        public virtual IExtractResult ThemeGraphyMCSI(SubProductInstanceDef instance)
        {
            try
            {
                string templatName = instance.LayoutName;
                string instanceIdentify = instance.OutFileIdentify;
                string instanceName = instance.Name;
                bool isOrigial = instance.isOriginal;
                bool isCurrentView = instance.isCurrentView;
                string productIdentify = _subProductDef.ProductDef.Identify;
                //string colortablename = GetStringArugment("colortablename");
                //string extinfo = GetStringArugment("extinfo");
                IRasterDrawing rasterDrawing = GetRasterDrawingArugment();  //多通道合成图 需要获取当前打开的图像。
                if (rasterDrawing == null)
                    return null;
                string fname = CreateMonitorBitmap(rasterDrawing, templatName, productIdentify, instanceIdentify, isOrigial, isCurrentView);
                IExtractResult er = new FileExtractResult(instanceIdentify, fname) as IExtractResult;
                return er;
            }
            finally
            {
            }
        }

        private IRasterDrawing GetRasterDrawingArugment()
        {
            object obj = _argumentProvider.GetArg("SmartSession");
            if (obj == null)
                return null;
            GeoDo.RSS.Core.UI.ISmartSession session = obj as GeoDo.RSS.Core.UI.ISmartSession;
            if (session == null)
                return null;
            ICanvasViewer viewer = null;
            viewer = GetAutoGenRasterDrawing(session);
            if (viewer == null)
                viewer = session.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return null;
            ICanvas canvas = viewer.Canvas;
            if (canvas == null)
                return null;
            return canvas.PrimaryDrawObject as IRasterDrawing;
        }

        public string GetRasterFilenameFromDrawing(out float resolution)
        {
            resolution = 0f;
            IRasterDrawing rasterDrawing = GetRasterDrawingArugment();
            if (rasterDrawing != null)
            {
                resolution = rasterDrawing.DataProvider.ResolutionX;
                return rasterDrawing.FileName;
            }
            return null;
        }

        public string GetRasterFilenameFromDrawing(out float resolution, out int width, out int height)
        {
            resolution = 0f;
            width = 0;
            height = 0;
            IRasterDrawing rasterDrawing = GetRasterDrawingArugment();
            if (rasterDrawing != null)
            {
                resolution = rasterDrawing.DataProvider.ResolutionX;
                width = rasterDrawing.DataProvider.Width;
                height = rasterDrawing.DataProvider.Height;
                return rasterDrawing.FileName;
            }
            return null;
        }

        private ICanvasViewer GetAutoGenRasterDrawing(GeoDo.RSS.Core.UI.ISmartSession session)
        {
            ISmartWindow[] smartWindows = null;
            ICanvasViewer viewer = null;
            if (AutoGeneratorSettings.CurrentSettings != null && GetStringArray("SelectedPrimaryFiles") != null)
            {
                string rasterFile = GetStringArray("SelectedPrimaryFiles")[0];
                smartWindows = session.SmartWindowManager.GetSmartWindows((w) =>
                {
                    if (w is ICanvasViewer)
                        return (w as ICanvasViewer).Title.ToUpper().Contains(Path.GetFileName(rasterFile).ToUpper());
                    return false;
                });
                if (smartWindows != null && smartWindows.Length != 0)
                    viewer = smartWindows[0] as ICanvasViewer;
            }
            return viewer;
        }

        private string CreateMonitorBitmap(IRasterDrawing rasterDrawing, string templateName, string productIdentify, string instanceIdentify, bool isOrginal, bool isCurrentView)
        {
            //创建用于生成多通道合成图的Bitmap，及其地理信息
            string bitmapFileName = null;
            int width = 0, height = 0;
            GetOrginBmpAndSave(rasterDrawing, out bitmapFileName, out width, out height, isCurrentView);
            CreatWorldFile(bitmapFileName, rasterDrawing, width, height);
            //加载模版
            ILayoutTemplate temp = GetTemplateByArg(templateName);
            if (temp == null)
                return null;
            ILayout layout = temp.Layout;
            if (isOrginal)
            {
                ChangeTemplateSize(layout, rasterDrawing.DataProvider.CoordEnvelope, width, height);
            }
            else
            {
                FitSizeToTemplateWidth(layout, width, height);
            }
            //生成文档，并应用变量
            IGxdDocument gxdDoc = GetDocument(bitmapFileName, temp);
            TryApplyVars(gxdDoc.GxdTemplateHost.LayoutTemplate, rasterDrawing);
            TrySetAttributesOfElements(gxdDoc);

            string rasterFname = rasterDrawing.FileName;
            string fname = GetOutputGxdFileName(new RasterIdentify(rasterFname), productIdentify, instanceIdentify);
            gxdDoc.SaveAs(fname);
            return fname;
        }

        private void ChangeTemplateSize(ILayout layout, GeoDo.RSS.Core.DF.CoordEnvelope envelope, int width, int height)
        {
            //查看是否应用区域
            ThemeGraphRegion region = ThemeGraphRegionSetting.GetThemeGraphRegion(_subProductDef.ProductDef.Identify);
            //数据框大小
            int newWidth = width;
            int newHeight = height;
            if (region != null && region.SelectedItem != null)
            {
                PrjEnvelope env = null;
                env = region.SelectedItem.PrjEnvelope;
                double widthRate = env.Width / envelope.Width;
                double hightRate = env.Height / envelope.Height;
                newWidth = (int)(widthRate * width);
                newHeight = (int)(hightRate * height);
            }
            IElement[] dfs = layout.QueryElements((e) => { return e is IDataFrame; });
            if (dfs == null || dfs.Length == 0)
                return;
            IDataFrame df = (dfs[0] as IDataFrame);
            if (df.Size.Width == newWidth && df.Size.Height == newHeight)
                return;
            IElement[] bords = layout.QueryElements((e) => { return e is Border; });
            if (bords == null || bords.Length == 0)
                return;
            Border bord = (bords[0] as Border);
            bool isMoveElements = true;
            if (df.Size.Width == bord.Size.Width && df.Size.Height == bord.Size.Height)
                isMoveElements = false;
            float yOffset = newHeight - df.Size.Height;
            float xOffset = newWidth - df.Size.Width;
            df.IsLocked = false;
            df.ApplySize(xOffset, yOffset);
            df.IsLocked = true;
            bord.ApplySize(xOffset, yOffset);
            List<IElement> eles = layout.Elements;
            for (int i = 0; i < eles.Count; i++)
            {
                if (eles[i] is IBorder ||
                    eles[i] is IDataFrame)
                    continue;
                if (eles[i] is ISizableElement)
                {
                    if (eles[i].Name == "标题" ||
                        eles[i].Name.Contains("Time") ||
                        eles[i].Name.Contains("Date"))
                    {
                        if (isMoveElements)
                            (eles[i] as ISizableElement).ApplyLocation(-xOffset / 2, -yOffset);
                        else
                            continue;
                    }
                    //if (eles[i].Name.Contains("标注") ||
                    //    eles[i].Name.Contains("文本 图") ||
                    //    eles[i].Name.Contains("文本 例")||
                    //    eles[i] is ISizableElementGroup||
                    //    eles[i] is FeatureDrawingElement)
                    //{
                    //    if (isMoveElements)
                    //        (eles[i] as ISizableElement).ApplyLocation(-xOffset, 0);
                    //}
                    (eles[i] as ISizableElement).ApplyLocation(xOffset, yOffset);
                }
            }
        }

        private void ChangeTemplateSize(ILayout layout, int width, int height)
        {
            IElement[] dfs = layout.QueryElements((e) => { return e is IDataFrame; });
            if (dfs == null || dfs.Length == 0)
                return;
            IDataFrame df = (dfs[0] as IDataFrame);
            if (df.Size.Width == width && df.Size.Height == height)
                return;
            float yOffset = height - df.Size.Height;
            float xOffset = width - df.Size.Width;
            df.IsLocked = false;
            df.ApplySize(xOffset, yOffset);
            df.IsLocked = true;
            layout.Size = new System.Drawing.SizeF(width, height);
            List<IElement> eles = layout.Elements;
            for (int i = 0; i < eles.Count; i++)
            {
                if (eles[i].Name == "标题" ||
                    eles[i].Name.Contains("Time") ||
                    eles[i].Name.Contains("Date"))
                    continue;
                if (eles[i] is IBorder ||
                    eles[i] is IDataFrame)
                    continue;
                if (eles[i] is ISizableElement)
                {
                    (eles[i] as ISizableElement).ApplyLocation(xOffset, yOffset);
                }
            }
        }

        private bool GetOrginBmpAndSave(IRasterDrawing rasterDrawing, out string fileName, out int width, out int height, bool isCurrentView)
        {
            fileName = null;
            width = 0;
            height = 0;
            System.Drawing.Bitmap bitmap = null;
            try
            {
                if (isCurrentView)
                    bitmap = rasterDrawing.GetBitmap(null);
                else
                    bitmap = rasterDrawing.GetBitmapUseOriginResolution();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (bitmap == null)
                return false;
            string ofilename = rasterDrawing.FileName;
            fileName = MifEnvironment.GetFullFileName(Guid.NewGuid().ToString() + ".bmp");
            width = bitmap.Width;
            height = bitmap.Height;
            if (bitmap == null)
                return false;
            bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
            return true;
        }

        private void CreatWorldFile(string fileName, IRasterDrawing rasterDrawing, int width, int height)
        {
            if (string.IsNullOrEmpty(fileName))
                return;
            if (width == 0 || height == 0)
                return;
            WorldFile wf = new WorldFile();
            if (rasterDrawing.OriginalEnvelope == null)
                return;
            if (rasterDrawing == null)
                return;
            if (rasterDrawing.DataProvider == null)
                return;
            IRasterDataProvider prd = rasterDrawing.DataProviderCopy;
            if (prd.SpatialRef == null)
                return;
            double bmpResolutionX = (prd.CoordEnvelope.MaxX - prd.CoordEnvelope.MinX) / width;
            double bmpResolutionY = (prd.CoordEnvelope.MaxY - prd.CoordEnvelope.MinY) / height;
            wf.CreatWorldFile(bmpResolutionX, -bmpResolutionY, prd.CoordEnvelope.MinX, prd.CoordEnvelope.MaxY, fileName);
            wf.CreatXmlFile(prd.SpatialRef, fileName);
        }

        private ILayoutTemplate GetTemplateByArg(string templateName)
        {
            ILayoutTemplate t = LayoutTemplate.FindTemplate(templateName);
            if (t == null)
                t = LayoutTemplate.FindTemplate("缺省二值图模版");
            return t;
        }

        private void TryGeneratreAOISecondaryFile(int[] aoi, IGxdRasterItem rst)
        {
            string fname = Path.Combine(Path.GetDirectoryName(rst.FileName), Path.GetFileNameWithoutExtension(rst.FileName) + ".aoi");
            if (File.Exists(fname))
                File.Delete(fname);
            if (aoi == null || aoi.Length == 0)
                return;
            using (FileStream sw = new FileStream(fname, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(sw))
                {
                    for (int i = 0; i < aoi.Length; i++)
                        bw.Write(aoi[i]);
                }
            }
        }

        /// <summary>
        /// 适应模版宽度，等比例缩放图像，高度空白区域自动填充。
        /// </summary>
        /// <param name="temp">专题模版</param>
        /// <param name="width">原图像宽度</param>
        /// <param name="height">原图像高度</param>
        private void FitSizeToTemplateWidth(ILayout layout, int width, int height, bool isFixTempleSize = false)
        {
            FitSizeToTemplateWidth(layout, (float)width, (float)height, isFixTempleSize);
        }

        protected void FitSizeToTemplateWidth(ILayout layout, float width, float height, bool isFixTempleSize = false)
        {
            if (isFixTempleSize)
                return;
            IElement[] dfs = layout.QueryElements((e) => { return e is IDataFrame; });
            if (dfs == null || dfs.Length == 0)
                return;
            IDataFrame df = (dfs[0] as IDataFrame);
            if (df.Size.Width == width && df.Size.Height == height)
                return;
            float dfNewW = df.Size.Width;
            float sc = width / dfNewW;
            float dfNewH = height / sc;
            float yOffset = dfNewH - df.Size.Height;
            df.IsLocked = false;
            df.ApplySize(0f, yOffset);
            df.IsLocked = true;
            layout.Size = new System.Drawing.SizeF(layout.Size.Width, layout.Size.Height + yOffset);
            List<IElement> eles = layout.Elements;
            for (int i = 0; i < eles.Count; i++)
            {
                if (eles[i].Name == "标题" ||
                    eles[i].Name.Contains("Time") ||
                    eles[i].Name.Contains("Date"))
                    continue;
                if (eles[i] is IBorder ||
                    eles[i] is IDataFrame)
                    continue;
                if (eles[i] is ISizableElement)
                {
                    (eles[i] as ISizableElement).ApplyLocation(0f, yOffset);
                }
            }
        }

        private void TryApplyVars(ILayoutTemplate temp, IRasterDrawing rasterDrawing, string[] dstFile)
        {
            RasterIdentify rst = new RasterIdentify(rasterDrawing.FileName);
            Dictionary<string, string> vars = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(rst.Satellite))
            {
                string sate = rst.Satellite.ToUpper();
                if (sate.Contains("FY3"))
                    sate = sate.Replace("FY3", "FY-3");
                else if (sate.Contains("FY2"))
                    sate = sate.Replace("FY2", "FY-2");
                vars.Add("{Satellite}", sate);
            }
            if (!string.IsNullOrEmpty(rst.Sensor))
                vars.Add("{Sensor}", rst.Sensor);
            if (rst.GenerateDateTime != DateTime.MinValue)
            {
                string timezone = _argumentProvider.GetArg("TimeZone").ToString();
                if (string.IsNullOrEmpty(timezone))
                    vars.Add("{OrbitDateTime}", rst.OrbitDateTime.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " (北京时)");
                else
                    vars.Add("{OrbitDateTime}", UpdateTimeByTimeZone(rst.OrbitDateTime, timezone));
            }
            IRasterDataProvider dataProvider = rasterDrawing.DataProviderCopy;// GeoDataDriver.Open(fileName) as IRasterDataProvider; 
            if (dataProvider != null)
            {
                string resolutionStr = Math.Round(dataProvider.ResolutionX, 4).ToString();
                vars.Add("{resolution}", resolutionStr);
                if (dataProvider.SpatialRef == null)
                {
                    vars["{resolution}"] += "度";
                    vars.Add("{projection}", "等经纬度");
                }
                else if (dataProvider.SpatialRef.GeographicsCoordSystem == null)
                    vars.Add("{projection}", "");
                else if (dataProvider.SpatialRef.ProjectionCoordSystem == null)
                {
                    vars["{resolution}"] += "度";
                    vars.Add("{projection}", "等经纬度");
                }
                else
                {
                    string targatName = string.Empty;
                    string projectName = dataProvider.SpatialRef.ProjectionCoordSystem.Name.Name;
                    GetProjectName(projectName, out targatName);
                    vars.Add("{projection}", targatName);
                }
                int[] channels = rasterDrawing.SelectedBandNos;// dataProvider.GetDefaultBands();
                List<string> channelList = new List<string>();
                if (channels != null && channels.Count() > 0)
                {
                    for (int i = 0; i < channels.Length; i++)
                    {
                        channelList.Add(channels[i].ToString());
                        channelList.Add(",");
                    }
                    channelList.RemoveAt(channelList.Count - 1);
                }
                string channelStr = null;
                foreach (string item in channelList)
                    channelStr += item;
                vars.Add("{channel}", channelStr);
            }
            //增加监测示意图监测产品时间变量
            if (dstFile != null)
            {
                rst = new RasterIdentify(dstFile);
                if (rst.GenerateDateTime != DateTime.MinValue)
                {
                    string timezone = _argumentProvider.GetArg("TimeZone").ToString();
                    if (string.IsNullOrEmpty(timezone))
                    {
                        if (vars.ContainsKey("{Satellite}") && vars["{Satellite}"] == "FY-3B")
                            vars.Add("{ProOrbitDateTime}", rst.OrbitDateTime.AddHours(8).ToString("yyyy年MM月dd日") + "  下午");
                        else if (vars.ContainsKey("{Satellite}") && (vars["{Satellite}"] == "FY-3A" || vars["{Satellite}"] == "FY-3C"))
                            vars.Add("{ProOrbitDateTime}", rst.OrbitDateTime.AddHours(8).ToString("yyyy年MM月dd日") + "  上午");
                        else
                            vars.Add("{ProOrbitDateTime}", rst.OrbitDateTime.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " (北京时)");
                    }
                    else
                        vars.Add("{ProOrbitDateTime}", UpdateTimeByTimeZone(rst.OrbitDateTime, timezone));
                }
            }
            temp.ApplyVars(vars);
        }

        private void TryApplyVars(ILayoutTemplate temp, IRasterDrawing rasterDrawing)
        {
            TryApplyVars(temp, rasterDrawing, null);
        }

        private string UpdateTimeByTimeZone(DateTime obritDatetime, string timezone)
        {
            Regex regex = new Regex(@"\(UTC(?<TimeSpan>\S*)\)(?<TimeZone>\S*)");
            if (regex.IsMatch(timezone.ToUpper()))
            {
                Match m = regex.Match(timezone.ToUpper());
                TimeSpan ts = TimeSpan.Parse(m.Groups["TimeSpan"].Value);
                return obritDatetime.Add(ts).ToString("yyyy年MM月dd日 HH:mm") + " " + m.Groups["TimeZone"].Value;
            }
            return obritDatetime.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " (北京时)";
        }

        private void GetProjectName(string projectName, out string targatName)
        {
            switch (projectName)
            {
                case "Polar Stereographic": targatName = "极射赤面投影";
                    break;
                case "Albers Conical Equal Area": targatName = "阿尔伯斯等面积投影";
                    break;
                case "Lambert Conformal Conic": targatName = "兰伯托";
                    break;
                case "Mercator": targatName = "墨卡托";
                    break;
                case "Hammer": targatName = "Hammer";
                    break;
                default: targatName = "";
                    break;
            }
        }

        private IGxdDocument GetDocument(string rasterFileName, ILayoutTemplate temp, params object[] arguments)
        {
            IGxdDocument doc = new GxdDocument(new GxdTemplateHost(temp));
            IGxdDataFrame gxdDf = doc.DataFrames.Count > 0 ? doc.DataFrames[0] : null;
            if (string.IsNullOrEmpty(rasterFileName))
                return null;
            if (gxdDf != null)
            {
                IGxdRasterItem rst = new GxdRasterItem(rasterFileName, null);//这里传具体的色标定义标识
                gxdDf.GxdRasterItems.Add(rst);
            }
            return doc;
        }

        private string GetOutputGxdFileName(RasterIdentify rstIdentifyCopy, string productIdentify, string instanceIdentify)
        {
            RasterIdentify rstIdentify = rstIdentifyCopy;
            if (rstIdentifyCopy == null)
                rstIdentify = new RasterIdentify();
            else
                rstIdentify = rstIdentifyCopy;
            rstIdentify.ProductIdentify = productIdentify;
            rstIdentify.SubProductIdentify = instanceIdentify;
            return rstIdentify.ToWksFullFileName(".gxd");
        }
        #endregion

        #region 专题图制作
        private RasterIdentify _rstIdentifyCopy = null;

        //基于矢量的专题图
        protected IExtractResult VectoryThemeGraphy(string extInfos)
        {
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            string templatName = GetStringArgument("ThemeGraphTemplateName");
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            string file = files[0];
            string docFilename = GenerateVectoryThemeGraphy(file, templatName, extInfos, outFileIdentify);
            if (string.IsNullOrWhiteSpace(docFilename))
                return null;
            else
            {
                IExtractResult er = new FileExtractResult(outFileIdentify, docFilename) as IExtractResult;
                return er;
            }
        }

        private string GenerateVectoryThemeGraphy(string shpPrdFileName, string templateName, string extInfos, string outIdentify, params object[] options)
        {
            ILayoutTemplate template = GetTemplateByArg(templateName);
            if (template == null)
                return null;
            //生成文档，并应用变量
            CreateRasterIdentifyCopy(shpPrdFileName);

            TryApplyVars(template);
            ApplyAttributesOfLayoutTemplate(template);

            IGxdDocument gxdDoc = CreateDocument(template, options != null && options.Length > 0 ? options[0] : null);

            TrySetAttributesOfElements(gxdDoc);

            string docFname = GetOutputGxdFileName(_rstIdentifyCopy, _subProductDef.ProductDef.Identify, outIdentify);
            gxdDoc.SaveAs(docFname);
            return docFname;
        }

        private IGxdDocument CreateDocument(ILayoutTemplate template, object p)
        {
            IGxdDocument doc = new GxdDocument(new GxdTemplateHost(template));
            IGxdDataFrame gxdDf = doc.DataFrames.Count > 0 ? doc.DataFrames[0] : null;
            return doc;
        }

        private void CreateRasterIdentifyCopy(string shpPrdFileName)
        {
            _rstIdentifyCopy = new RasterIdentify(shpPrdFileName);
        }

        private void TrySetAttributesOfElements(IGxdDocument doc)
        {
            TrySetAttributesOfElements(doc, null, null);
        }

        private void TrySetAttributesOfElements(IGxdDocument doc, ThemeGraphRegion region, List<string> infoContents)
        {
            ILayout layout = doc.GxdTemplateHost.LayoutTemplate.Layout;
            //图廓
            IBorder border = layout.GetBorder();
            ApplyAttributesOfLayoutBorder(border);
            //数据框
            IElement[] dfs = layout.QueryElements((e) => { return e is IDataFrame; });
            ApplyAttributesOfDataFrame(doc.DataFrames, dfs, layout, region, infoContents);
            //图列
            ApplyAttributesOfElement(layout.QueryElements((e) => { return e != null; }));
        }

        private void ApplyAttributesOfElement(IElement[] eles)
        {
            if (eles == null || eles.Length == 0)
                return;
            foreach (IElement e in eles)
                ApplyAttributesOfElement(e.Name, e);
        }

        private void ApplyAttributesOfDataFrame(List<IGxdDataFrame> list, IElement[] dfs, ILayout layout)
        {
            ApplyAttributesOfDataFrame(list, dfs, layout, null, null);
        }

        private void ApplyAttributesOfDataFrame(List<IGxdDataFrame> list, IElement[] dfs, ILayout layout, ThemeGraphRegion region, List<string> infoContents)
        {
            if (list == null || dfs == null || list.Count == 0 || dfs.Length == 0 || list.Count != dfs.Length)
                return;
            if (region == null && infoContents == null)
                for (int i = 0; i < dfs.Length; i++)
                    ApplyAttributesOfDataFrame(list[i], dfs[i] as IDataFrame, layout);
            else
                for (int i = 0; i < dfs.Length; i++)
                    ApplyAttributesOfDataFrame(list[i], dfs[i] as IDataFrame, layout, region, infoContents);
        }

        protected virtual void ApplyAttributesOfLayoutTemplate(ILayoutTemplate template)
        {
        }

        protected virtual void ApplyAttributesOfElement(string name, IElement ele)
        {
        }

        protected virtual void ApplyAttributesOfDataFrame(IGxdDataFrame gxdDataFrame, IDataFrame dataFrame, ILayout layout)
        {
            ApplyAttributesOfDataFrame(gxdDataFrame, dataFrame, layout, null, null);
        }

        protected virtual void ApplyAttributesOfDataFrame(IGxdDataFrame gxdDataFrame, IDataFrame dataFrame, ILayout layout, ThemeGraphRegion region, List<string> infoContents)
        {
            TrySetDataFrameEnvelope(gxdDataFrame, dataFrame, layout, region, infoContents);
        }

        #region 尝试设置数据框范围
        private void TrySetDataFrameEnvelope(IGxdDataFrame gxdDataFrame, IDataFrame dataFrame, ILayout layout)
        {
            TrySetDataFrameEnvelope(gxdDataFrame, dataFrame, layout, null, null);
        }

        private void TrySetDataFrameEnvelope(IGxdDataFrame gxdDataFrame, IDataFrame dataFrame, ILayout layout, ThemeGraphRegion region, List<string> infoContents)
        {
            try
            {
                //在什么条件下设置??
                if (gxdDataFrame.Envelope == null)
                {
                    ThemeGraphRegion regionDef = ThemeGraphRegionSetting.GetThemeGraphRegion(_subProductDef.ProductDef.Identify);
                    if (regionDef != null && regionDef.SelectedItem != null)
                        region = regionDef;
                    if (region != null && region.SelectedItem != null)
                    {
                        PrjEnvelope env = null;
                        env = region.SelectedItem.PrjEnvelope;
                        if (infoContents == null)
                            infoContents = new List<string>();
                        infoContents.Add(string.Format("数据框范围：{0},{1},{2},{3}", env.MinX, env.MaxX, env.MinY, env.MaxY));
                        Layout.GxdEnvelope gxdEnv = ToPrjEnvelope(env, gxdDataFrame, dataFrame);
                        if (gxdEnv != null)
                        {
                            FitSizeToTemplateWidth(layout, (float)(gxdEnv.MaxX - gxdEnv.MinX), (float)(gxdEnv.MaxY - gxdEnv.MinY));
                            gxdDataFrame.Envelope = gxdEnv;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //PrintInfo(ex.Message);
            }
        }

        protected Layout.GxdEnvelope ToPrjEnvelope(PrjEnvelope env, Layout.IGxdDataFrame gxdDataFrame, Layout.IDataFrame dataFrame)
        {
            if (env == null)
                return null;
            if (GeoDo.Project.SpatialReference.GetDefault().ToWKTString() == gxdDataFrame.SpatialRef)
                gxdDataFrame.SpatialRef = null;
            GeoDo.Project.IProjectionTransform tran = GetProjectionTransform(gxdDataFrame.SpatialRef);
            if (tran == null)
                return null;
            double[] xs = new double[] { env.MinX, env.MaxX };
            double[] ys = new double[] { env.MaxY, env.MinY };
            try
            {
                tran.Transform(xs, ys);
                return new Layout.GxdEnvelope(xs[0], xs[1], ys[1], ys[0]);
            }
            catch
            {
                return null;
            }
        }

        private Project.IProjectionTransform GetProjectionTransform(string spatialRef)
        {
            if (spatialRef == null || !spatialRef.Contains("PROJCS"))
                return Project.ProjectionTransformFactory.GetDefault();
            else
                return Project.ProjectionTransformFactory.GetProjectionTransform(GeoDo.Project.SpatialReference.GetDefault(), GeoDo.Project.SpatialReferenceFactory.GetSpatialReferenceByWKT(spatialRef, Project.enumWKTSource.EsriPrjFile));
        }
        #endregion

        protected virtual void ApplyAttributesOfLayoutBorder(IBorder border)
        {
        }

        private void TryApplyVars(ILayoutTemplate temp)
        {
            if (_rstIdentifyCopy == null)
                return;
            Dictionary<string, string> vars = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(_rstIdentifyCopy.Satellite))
            {
                string sate = _rstIdentifyCopy.Satellite.ToUpper();
                if (sate.Contains("FY3"))
                    sate = sate.Replace("FY3", "FY-3");
                else if (sate.Contains("FY2"))
                    sate = sate.Replace("FY2", "FY-2");
                vars.Add("{Satellite}", sate);
            }
            if (!string.IsNullOrEmpty(_rstIdentifyCopy.Sensor))
                vars.Add("{Sensor}", _rstIdentifyCopy.Sensor);
            if (!string.IsNullOrEmpty(_rstIdentifyCopy.ProductName))
                vars.Add("{Product}", _rstIdentifyCopy.ProductName);
            if (_rstIdentifyCopy.OrbitDateTime != DateTime.MinValue)
            {
                vars.Add("{OrbitDateTime}", _rstIdentifyCopy.OrbitDateTime.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " (北京时)");
                vars.Add("{OrbitDateDay}", _rstIdentifyCopy.OrbitDateTime.AddHours(8).ToString("yyyy年MM月dd日"));
            }
            if (_rstIdentifyCopy.MaxOrbitDate != DateTime.MaxValue && _rstIdentifyCopy.MinOrbitDate != DateTime.MinValue)
            {
                DateTime minTime = _rstIdentifyCopy.MinOrbitDate.AddHours(8);
                DateTime maxTime = _rstIdentifyCopy.MaxOrbitDate.AddHours(8);
                vars.Add("{MinOrbitDateTime:MaxOrbitDateTime}", minTime.ToString("yyyy年MM月dd日 HH:mm") + ":" + maxTime.ToString("yyyy年MM月dd日 HH:mm"));
                vars.Add("{MinOrbitDateDay:MaxOrbitDateTime}", minTime.ToString("yyyy年MM月dd日") + " vs " + maxTime.ToString("yyyy年MM月dd日"));
                vars.Add("{MaxOrbitDateDay:MinOrbitDateTime}", maxTime.ToString("yyyy年MM月dd日") + " vs " + minTime.ToString("yyyy年MM月dd日"));
                vars.Add("{MinOrbitDateDay:MaxOrbitDateDay:Time}", minTime.ToString("yyyy年MM月dd日") + " — " + maxTime.ToString("MM月dd日") + "合成 " + minTime.ToString("t") + "(北京时)");
                vars.Add("{MinOrbitDateDay~MaxOrbitDateDay}", minTime.ToString("yyyy年MM月dd日") + "-" + maxTime.ToString("yyyy年MM月dd日"));
            }
            if (_rstIdentifyCopy != null)
            {
                string filename = _rstIdentifyCopy.OriFileName[0];
                if (!IsVector(filename))
                {
                    using (RasterDataProvider rdd = GeoDataDriver.Open(filename) as RasterDataProvider)
                    {
                        string resolutionStr = Math.Round(rdd.ResolutionX, 4).ToString();
                        vars.Add("{resolution}", resolutionStr);
                        if (rdd.SpatialRef == null)
                        {
                            vars["{resolution}"] += "度";
                            vars.Add("{projection}", "等经纬度");
                        }
                        else if (rdd.SpatialRef.GeographicsCoordSystem == null)
                            vars.Add("{projection}", "");
                        else if (rdd.SpatialRef.ProjectionCoordSystem == null)
                        {
                            vars["{resolution}"] += "度";
                            vars.Add("{projection}", "等经纬度");
                        }
                        else
                        {
                            string targatName = string.Empty;
                            string projectName = rdd.SpatialRef.ProjectionCoordSystem.Name.Name;
                            GetProjectName(projectName, out targatName);
                            vars.Add("{projection}", targatName);
                        }
                    }
                }
            }
            temp.ApplyVars(vars);
        }

        private bool IsVector(string filename)
        {
            string[] vectorExt = new string[] { ".SHP" };
            if (vectorExt.Contains(Path.GetExtension(filename).ToUpper()))
                return true;
            return false;
        }

        protected PrjEnvelope GetArgToPrjEnvelope(string arg)
        {
            object obj = _argumentProvider.GetArg(arg);
            if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                return null;
            else
                return (PrjEnvelope)obj;
        }
        #endregion

        public IStatResult StatResultToIStatResult(string[] files, StatResultItem[] resultItems, string productIdentify, string outputIdentify, string title, string extinfo, int zoom)
        {
            return StatResultItemToIStatResult.ItemsToResults(resultItems, files);
        }

        public string StatResultToFile(string[] files, StatResultItem[] resultItems, string productIdentify, string outputIdentify, string title, string extinfo)
        {
            IStatResult results = StatResultItemToIStatResult.ItemsToResults(resultItems, files);
            return StatResultToFile(files, results, productIdentify, outputIdentify, title, extinfo, 1, true, (byte)1);
        }

        public string StatResultToFile(string[] files, StatResultItem[] resultItems, string productIdentify, string outputIdentify, string title, string extinfo, int zoom)
        {
            IStatResult results = StatResultItemToIStatResult.ItemsToResults(resultItems, files);
            return StatResultToFile(files, results, productIdentify, outputIdentify, title, extinfo, zoom, true, (byte)1);
        }

        public string StatResultToFile(string[] files, IStatResult results, string productIdentify, string outputIdentify, string title, string extinfo, int zoom, bool isTotal)
        {
            return StatResultToFile(files, results, productIdentify, outputIdentify, title, extinfo, zoom, isTotal, 1);
        }

        public string StatResultToFile(string[] files, IStatResult results, string productIdentify, string outputIdentify, string title, string extinfo, int zoom, bool isTotal, byte baseCol)
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
                    excelControl.Zoom = zoom;
                    //excelControl.Add(true, title, results, isTotal, 1);
                    excelControl.Add(true, title, results, results.Columns.Length - 2, isTotal, 1, 1, baseCol);
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

        public string StatResultToFile(string[] files, IStatResult results, string productIdentify, string outputIdentify, string title, string extinfo, int zoom, int startCol, int displayCol, bool isTotal, byte baseCol, int charType)
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
                    excelControl.Zoom = zoom;
                    excelControl.Add(true, title, results, startCol, displayCol, isTotal, 1, 1, baseCol, charType);
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


        public string StatResultToFile(string[] files, IStatResult results, string productIdentify, string outputIdentify, string title, string extinfo, int zoom, List<RowDisplayDef> rowRulers, bool displayDataLable, bool isTotal, int startCol, int displayCol, int uionBCol, byte baseCol)
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
                    excelControl.Zoom = zoom;
                    excelControl.Add(false, title, results, startCol, displayCol, displayDataLable, isTotal, 1, rowRulers, uionBCol, baseCol);
                    if (!filename.ToUpper().EndsWith(".XLSX"))
                        filename += ".XLSX";
                    excelControl.SaveFile(filename);
                }
            }
            catch (Exception ex)
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

        public string StatResultToFile(string[] files, IStatResult results, string productIdentify, string outputIdentify, string title, string extinfo, int zoom, List<RowDisplayDef> rowRulers, bool displayDataLable, bool isTotal, int startCol, int displayCol, byte baseCol)
        {
            return StatResultToFile(files, results, productIdentify, outputIdentify, title, extinfo, zoom, rowRulers, displayDataLable, isTotal, startCol, displayCol, 0, 1);
        }
        public string StatResultToFile(string[] files, IStatResult results, string productIdentify, string outputIdentify, string title, string extinfo, int zoom, List<RowDisplayDef> rowRulers, bool displayDataLable, bool isTotal, int startCol, int displayCol)
        {
            return StatResultToFile(files, results, productIdentify, outputIdentify, title, extinfo, zoom, rowRulers, displayDataLable, isTotal, startCol, displayCol, 1);
        }

        public string StatResultToFile(string[] files, IStatResult results, string productIdentify, string outputIdentify, string title, string extinfo, int zoom, List<RowDisplayDef> rowRulers, bool displayDataLable, int startCol, int displayCol)
        {
            return StatResultToFile(files, results, productIdentify, outputIdentify, title, extinfo, zoom, rowRulers, displayDataLable, false, startCol, displayCol);
        }

        public string StatResultToFile(string[] files, IStatResult results, string productIdentify, string outputIdentify, string title, string extinfo, int zoom, List<RowDisplayDef> rowRulers, int startCol, int displayCol)
        {
            return StatResultToFile(files, results, productIdentify, outputIdentify, title, extinfo, zoom, rowRulers, true, startCol, displayCol);
        }

        public string StatResultToFile(string[] files, IStatResult results, string productIdentify, string outputIdentify, string title, string extinfo, int zoom, bool isTotal, int statImage)
        {
            return StatResultToFile(files, results, productIdentify, outputIdentify, title, extinfo, zoom, isTotal, statImage, 1);
        }

        public string StatResultToFile(string[] files, IStatResult results, string productIdentify, string outputIdentify, string title, string extinfo, int zoom, bool isTotal, int statImage, int notStatCol)
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
                    excelControl.Zoom = zoom;
                    //excelControl.Add(true, title, results, isTotal, 1);
                    excelControl.Add(true, title, results, results.Columns.Length - notStatCol, isTotal, 1, statImage);
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

        public string GetFileName(string[] files, string productIdentify, string outputIdentify, string fomart, string cycFlag, string extinfo)
        {
            IFileNameGenerator fng = GetFileNameGenerator();
            if (fng == null)
                return string.Empty;
            DataIdentify di = GetDataIdentify();
            return fng.NewFileName(CreatRasterIndetifyId(files, productIdentify, outputIdentify, di, fomart, cycFlag, extinfo));
        }

        private string _datFileDatestr = "yyyyMMddHHmmss";
        public string GetFileNameDefTime(string[] files, string productIdentify, string outputIdentify, string fomart, string extinfo, string baseFile)
        {
            IFileNameGenerator fng = GetFileNameGenerator();
            if (fng == null)
                return string.Empty;
            DataIdentify di = GetDataIdentify();
            string replaceStr = null;
            RasterIdentify rid = CreatRasterIndetifyId(files, productIdentify, outputIdentify, di, fomart, extinfo, baseFile, out replaceStr);
            string result = fng.NewFileName(rid);
            return result.Replace(rid.MinOrbitDate.ToString(_datFileDatestr) + "_" + rid.MaxOrbitDate.ToString(_datFileDatestr), replaceStr);
        }

        private IFileNameGenerator GetFileNameGenerator()
        {
            object obj = _argumentProvider.GetArg("FileNameGenerator");
            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                return FileNameGeneratorDefault.GetFileNameGenerator();
            return obj as IFileNameGenerator;
        }

        #region 统计分析 省市县
        /// <summary>
        /// 分段条件统计
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="filter"></param>
        /// <param name="progressTracker"></param>
        /// <returns></returns>
        public IExtractResult StatRaster<T>(SubProductInstanceDef instance, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker) where T : struct, IConvertible
        {
            return StatRaster<T>(instance, filters, progressTracker, false, 1);
        }

        public IExtractResult StatRaster<T>(SubProductInstanceDef instance, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker, bool weight, float weightZoom) where T : struct, IConvertible
        {
            string aoiTemplateName = instance.AOIProvider;
            if (aoiTemplateName.IndexOf(":") != -1)
                aoiTemplateName = aoiTemplateName.Substring(0, aoiTemplateName.IndexOf(":"));
            switch (aoiTemplateName)
            {
                case "当前区域":
                    return StatRasterByVector<T>(instance.Name, null, filters, progressTracker, weight, weightZoom);
                case "省市县行政区划":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("三级行政区划");
                        return StatRasterXJ<T>(_subProductDef.ProductDef.Name, _subProductDef.ProductDef.Identify, instance.Name, instance.AOIProvider, statString, instance.OutFileIdentify, filters, progressTracker, weight, weightZoom);
                    }
                case "省级行政区+土地利用类型":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("三级行政区划");
                        return StatRasterXJ<T>(_subProductDef.ProductDef.Name, _subProductDef.ProductDef.Identify, instance.Name, instance.AOIProvider, statString, instance.OutFileIdentify, filters, progressTracker, weight, weightZoom);
                    }
                case "省级行政区划":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("行政区划");
                        return StatRasterByVector<T>(instance.Name, statString, filters, progressTracker, weight, weightZoom);
                    }
                case "土地利用类型":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("土地利用类型");
                        return StatRasterByVector<T>(instance.Name, statString, filters, progressTracker, weight, weightZoom);
                    }
                case "自定义区域"://
                default:
                    break;
            }
            return null;
        }

        public IExtractResult StatRaster<T>(SubProductInstanceDef instance, string[] files, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker) where T : struct, IConvertible
        {
            return StatRaster<T>(instance, files, filters, progressTracker, false, 1);
        }

        public IExtractResult StatRaster<T>(SubProductInstanceDef instance, string[] files, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker, bool weight, float weightZoom) where T : struct, IConvertible
        {
            _argumentProvider.SetArg("SelectedPrimaryFiles", files);
            string aoiTemplateName = instance.AOIProvider;
            if (aoiTemplateName.IndexOf(":") != -1)
                aoiTemplateName = aoiTemplateName.Substring(0, aoiTemplateName.IndexOf(":"));
            switch (aoiTemplateName)
            {
                case "当前区域":
                    return StatRasterByVector<T>(instance.Name, null, filters, progressTracker, weight, weightZoom);
                case "省市县行政区划":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("三级行政区划");
                        return StatRasterXJ<T>(_subProductDef.ProductDef.Name, _subProductDef.ProductDef.Identify, instance.Name, instance.AOIProvider, statString, instance.OutFileIdentify, filters, progressTracker);
                    }
                case "省级行政区+土地利用类型":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("三级行政区划");
                        return StatRasterXJ<T>(_subProductDef.ProductDef.Name, _subProductDef.ProductDef.Identify, instance.Name, instance.AOIProvider, statString, instance.OutFileIdentify, filters, progressTracker);
                    }
                case "省级行政区划":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("行政区划");
                        return StatRasterByVector<T>(instance.Name, statString, filters, progressTracker, weight, weightZoom);
                    }
                case "土地利用类型":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("土地利用类型");
                        return StatRasterByVector<T>(instance.Name, statString, filters, progressTracker, weight, weightZoom);
                    }
                case "自定义区域"://
                default:
                    break;
            }
            return null;
        }

        public IStatResult StatRasterToStatResult<T>(SubProductInstanceDef instance, string[] files, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker) where T : struct, IConvertible
        {
            return StatRasterToStatResult<T>(instance, files, filters, progressTracker, false, 1);
        }

        public IStatResult StatRasterToStatResult<T>(SubProductInstanceDef instance, string[] files, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker, bool weight, float weightZoom) where T : struct, IConvertible
        {
            _argumentProvider.SetArg("SelectedPrimaryFiles", files);
            string aoiTemplateName = instance.AOIProvider;
            if (aoiTemplateName.IndexOf(":") != -1)
                aoiTemplateName = aoiTemplateName.Substring(0, aoiTemplateName.IndexOf(":"));
            switch (aoiTemplateName)
            {
                case "当前区域":
                    return StatRasterByVectorToStatResult<T>(instance.Name, null, filters, progressTracker, weight, weightZoom);
                case "省市县行政区划":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("三级行政区划");
                        return StatRasterXJToStatResult<T>(_subProductDef.ProductDef.Name, _subProductDef.ProductDef.Identify, instance.Name, instance.AOIProvider, statString, instance.OutFileIdentify, filters, progressTracker, weight, weightZoom);
                    }
                case "省级行政区+土地利用类型":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("三级行政区划");
                        return StatRasterXJToStatResult<T>(_subProductDef.ProductDef.Name, _subProductDef.ProductDef.Identify, instance.Name, instance.AOIProvider, statString, instance.OutFileIdentify, filters, progressTracker, weight, weightZoom);
                    }
                case "省级行政区划":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("行政区划");
                        return StatRasterByVectorToStatResult<T>(instance.Name, statString, filters, progressTracker, weight, weightZoom);
                    }
                case "土地利用类型":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("土地利用类型");
                        return StatRasterByVectorToStatResult<T>(instance.Name, statString, filters, progressTracker, weight, weightZoom);
                    }
                case "自定义区域"://
                default:
                    break;
            }
            return null;
        }

        private IStatResult StatRasterXJToStatResult<T>(string productName, string productIdentify, string instanceName, string instanceAOI, string aoiTemplate, string outFileIdentify, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker) where T : struct, IConvertible
        {
            return StatRasterXJToStatResult<T>(productName, productIdentify, instanceName, instanceAOI, aoiTemplate, outFileIdentify, filters, progressTracker, false, 1);
        }

        private IStatResult StatRasterXJToStatResult<T>(string productName, string productIdentify, string instanceName, string instanceAOI, string aoiTemplate, string outFileIdentify, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker, bool weight, float weightZoom) where T : struct, IConvertible
        {
            object aioObj = _argumentProvider.GetArg("AOI");
            string title = GetStringArgument("title");
            string[] files = GetStringArray("SelectedPrimaryFiles");
            string extInfos = GetStringArgument("extinfo");
            string statname = GetStringArgument("statname");
            Dictionary<string, SortedDictionary<string, double>> dic = null;
            if (files == null || files.Length == 0)
                return null;
            dic = RasterStatFactory.Stat(files[0], aoiTemplate, filters, progressTracker, weight, weightZoom);
            if (dic == null || dic.Count == 0)
                return null;
            if (string.IsNullOrEmpty(statname))
                title = productName + instanceName;
            else
                title = statname + instanceName;
            SortedDictionary<string, string> keyNmaeDic = GetRasterStatDictionary(aoiTemplate);
            string subTitle = GetSubTitle(files);
            IStatResult results = SSXDicToStatResult(dic, filters.Keys.ToArray(), subTitle, instanceAOI, keyNmaeDic);
            return results;
        }

        private IExtractResult StatRasterXJ<T>(string productName, string productIdentify, string instanceName, string instanceAOI, string aoiTemplate, string outFileIdentify, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker) where T : struct, IConvertible
        {
            return StatRasterXJ<T>(productName, productIdentify, instanceName, instanceAOI, aoiTemplate, outFileIdentify, filters, progressTracker, false, 1);
        }

        private IExtractResult StatRasterXJ<T>(string productName, string productIdentify, string instanceName, string instanceAOI, string aoiTemplate, string outFileIdentify, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker, bool weight, float weightZoom) where T : struct, IConvertible
        {
            object aioObj = _argumentProvider.GetArg("AOI");
            string title = GetStringArgument("title");
            string[] files = GetStringArray("SelectedPrimaryFiles");
            string extInfos = GetStringArgument("extinfo");
            string statname = GetStringArgument("statname");
            Dictionary<string, SortedDictionary<string, double>> dic = null;
            if (files == null || files.Length == 0)
                return null;
            if (instanceAOI.Contains("+"))
            {
                dic = RasterStatFactory.Stat(files[0], aoiTemplate, filters, progressTracker, weight, weightZoom, instanceAOI.Substring(instanceAOI.IndexOf("+") + 1));
            }
            else
                dic = RasterStatFactory.Stat(files[0], aoiTemplate, filters, progressTracker, weight, weightZoom);
            if (dic == null || dic.Count == 0)
                return null;
            if (string.IsNullOrEmpty(statname))
                title = productName + instanceName;
            else
                title = statname + instanceName;
            SortedDictionary<string, string> keyNmaeDic = GetRasterStatDictionary(aoiTemplate);
            string subTitle = GetSubTitle(files);
            IStatResult results = SSXDicToStatResult(dic, filters.Keys.ToArray(), subTitle, instanceAOI, keyNmaeDic);
            if (results == null)
                return null;
            string filename = StatResultToFile(files, results, productIdentify, outFileIdentify, title, extInfos, 1, false, 0);
            return new FileExtractResult(outFileIdentify, filename);
        }

        private IStatResult StatRasterByVectorToStatResult<T>(string instanceName, string aoiTemplate, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker) where T : struct, IConvertible
        {
            return StatRasterByVectorToStatResult<T>(instanceName, aoiTemplate, filters, progressTracker, false, 1);
        }

        private IStatResult StatRasterByVectorToStatResult<T>(string instanceName, string aoiTemplate, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker, bool weight, float weightZoom) where T : struct, IConvertible
        {
            object aioObj = _argumentProvider.GetArg("AOI");
            string title = GetStringArgument("title");
            string[] files = GetStringArray("SelectedPrimaryFiles");
            string extInfos = GetStringArgument("extinfo");
            string outFileIdentify = GetStringArgument("OutFileIdentify");//outfileidentify
            string statname = GetStringArgument("statname");
            string productName = _subProductDef.ProductDef.Name;
            string productIdentify = _subProductDef.ProductDef.Identify;
            Dictionary<string, SortedDictionary<string, double>> dic = null;
            if (files == null || files.Length == 0)
                return null;
            foreach (string file in files)
            {
                dic = RasterStatFactory.Stat(files[0], aoiTemplate, filters, progressTracker, weight, weightZoom);
            }
            dic = RasterStatFactory.Stat(files[0], aoiTemplate, filters, progressTracker, weight, weightZoom);
            if (dic == null || dic.Count == 0)
                return null;
            title = productName + statname + instanceName;
            string subTitle = GetSubTitle(files);
            IStatResult results = DicToStatResult(dic, filters.Keys.ToArray(), subTitle);
            return results;
        }

        private IExtractResult StatRasterByVector<T>(string instanceName, string aoiTemplate, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker, bool weight, float weightZoom) where T : struct, IConvertible
        {
            object aioObj = _argumentProvider.GetArg("AOI");
            string title = GetStringArgument("title");
            string[] files = GetStringArray("SelectedPrimaryFiles");
            string extInfos = GetStringArgument("extinfo");
            string outFileIdentify = GetStringArgument("OutFileIdentify");//outfileidentify
            string statname = GetStringArgument("statname");
            string productName = _subProductDef.ProductDef.Name;
            string productIdentify = _subProductDef.ProductDef.Identify;
            Dictionary<string, SortedDictionary<string, double>> dic = null;
            if (files == null || files.Length == 0)
                return null;
            foreach (string file in files)
            {
                dic = RasterStatFactory.Stat(files[0], aoiTemplate, filters, progressTracker, weight, weightZoom);
            }
            dic = RasterStatFactory.Stat(files[0], aoiTemplate, filters, progressTracker, weight, weightZoom);
            if (dic == null || dic.Count == 0)
                return null;
            title = productName + statname + instanceName;
            string subTitle = GetSubTitle(files);
            IStatResult results = DicToStatResult(dic, filters.Keys.ToArray(), subTitle);
            if (results == null)
                return null;
            string filename = StatResultToFile(files, results, productIdentify, outFileIdentify, title, extInfos, 1, true, 1);
            return new FileExtractResult(outFileIdentify, filename);
        }

        private IStatResult DicToStatResult(Dictionary<string, SortedDictionary<string, double>> areaResultDic, string[] filterKeys, string subTitle)
        {
            string[] rowKeys = areaResultDic.Keys.ToArray(); //行
            List<string> cols = new List<string>();          //列
            cols.Add("统计分类");
            cols.AddRange(filterKeys);
            string[] columns = cols.ToArray();
            List<string[]> rows = new List<string[]>();
            for (int i = 0; i < rowKeys.Length; i++)
            {
                string type = rowKeys[i];
                SortedDictionary<string, double> rowStat = areaResultDic[type];
                string[] row = new string[1 + filterKeys.Length];
                row[0] = type;
                for (int j = 0; j < filterKeys.Length; j++)
                {
                    string key = filterKeys[j];
                    if (rowStat.ContainsKey(key))
                        row[j + 1] = rowStat[key].ToString();
                    else
                        row[j + 1] = "0";
                }
                rows.Add(row);
            }
            if (rows == null || rows.Count == 0)
                return null;
            else
                return new StatResult(subTitle, columns, rows.ToArray());
        }

        private IStatResult SSXDicToStatResult(Dictionary<string, SortedDictionary<string, double>> areaResultDic, string[] filterKeys, string subTitle, string templateName, SortedDictionary<string, string> keyNmaeDic)
        {
            string[] rowKeys = areaResultDic.Keys.ToArray(); //行
            List<string> cols = new List<string>();
            string[] columnName = GetRasterStatColumnName("三级行政区划");
            if (columnName == null || columnName.Length != 3)//列
                cols.AddRange(new string[] { "省/直辖市", "市/区", "县" });
            else
                cols.AddRange(columnName);
            List<string[]> rows = new List<string[]>();
            List<string> countys = new List<string>();
            List<string> extends = new List<string>();
            Dictionary<string, SortedDictionary<string, double>> newDic = new Dictionary<string, SortedDictionary<string, double>>();
            SortedDictionary<string, double> newResult = new SortedDictionary<string, double>();
            string[] extendColName = null;
            int extendColCount = 0;
            //求一个最大省市县集合
            foreach (string key in areaResultDic.Keys)
            {
                newResult = ToSSXLevel(areaResultDic[key]);
                if (newResult != null)
                {
                    newDic.Add(key, newResult);
                    foreach (string item in newResult.Keys)
                    {
                        if (!countys.Contains(item))
                        {
                            if (item.Contains(":"))
                            {
                                extends.Add(item);
                                string subStr = item.Substring(item.IndexOf(":") + 1);
                                extendColName = subStr.Split(new char[] { ';' });
                                int currExtendColCount = extendColName.Length;
                                if (extendColCount != 0 && extendColCount != currExtendColCount)
                                    extendColCount = 0;
                                else
                                    extendColCount = currExtendColCount;
                            }
                            else
                                countys.Add(item);
                        }
                    }
                }
            }
            countys.Sort();
            extends.Sort();
            if (extendColCount != 0)
            {
                for (int i = 0; i < filterKeys.Length; i++)
                {
                    cols.Add(filterKeys[i]);
                    cols.AddRange(extendColName);
                }
            }
            else
                cols.AddRange(filterKeys);
            string[] columns = cols.ToArray();

            //
            string name = null;
            try
            {
                for (int i = 0; i < countys.Count; i++)
                {
                    string[] row = new string[3 + filterKeys.Length + extendColCount * filterKeys.Length];
                    if (keyNmaeDic != null)
                        keyNmaeDic.TryGetValue(countys[i], out name);
                    string keyString = countys[i].ToString();
                    if (keyString.Length != 6)//未知区域或者海外。
                    {
                        row[0] = string.IsNullOrWhiteSpace(name) ? keyString : name;
                    }
                    else if (keyString.Substring(2, 4) == "0000")
                    {
                        row[0] = string.IsNullOrWhiteSpace(name) ? keyString : name;
                    }
                    else if (keyString.Substring(4, 2) == "00")//市
                    {
                        if (!string.IsNullOrWhiteSpace(name))//代替省字符
                        {
                            string sj = keyString.Substring(0, 2) + "0000";
                            string sjName = "";
                            if (keyNmaeDic != null)
                                keyNmaeDic.TryGetValue(sj, out sjName);
                            if (!string.IsNullOrWhiteSpace(sjName))
                                name = name.Replace(sjName, "");
                        }
                        row[1] = string.IsNullOrWhiteSpace(name) ? keyString : name;
                    }
                    else if (keyString.Substring(4, 2) != "00")//县
                    {
                        if (!string.IsNullOrWhiteSpace(name))//代替省市字符
                        {
                            string sj = keyString.Substring(0, 4) + "00";
                            string sjName = "";
                            if (keyNmaeDic != null)
                                keyNmaeDic.TryGetValue(sj, out sjName);
                            if (!string.IsNullOrWhiteSpace(sjName))
                                name = name.Replace(sjName, "");
                        }
                        row[2] = string.IsNullOrWhiteSpace(name) ? keyString : name;
                    }
                    string key = countys[i];
                    string extendKey = null;
                    for (int j = 0; j < filterKeys.Length; j++)
                    {
                        string filter = filterKeys[j];
                        if (newDic[filter].Keys.Contains(countys[i]))
                            row[3 + j * (extendColCount + 1)] = newDic[filter][key].ToString();
                        else
                            row[3 + j * (extendColCount + 1)] = "0";
                        for (int exetend = 0; exetend < extendColCount; exetend++)
                        {
                            for (int extendColIndex = 1; extendColIndex <= extendColCount; extendColIndex++)
                            {
                                if (extends.Contains(extendKey = key + ":" + extendColName[extendColIndex - 1]))
                                {
                                    if (newDic[filter].ContainsKey(extendKey))
                                        row[3 + j * (extendColCount + 1) + extendColIndex] = newDic[filter][extendKey].ToString();
                                    else
                                        row[3 + j * (extendColCount + 1) + extendColIndex] = "0";
                                }
                            }
                        }
                    }
                    rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                string exstr = ex.Message;
            }
            if (rows == null || rows.Count == 0)
                return null;
            else
                return new StatResult(subTitle, columns, rows.ToArray());
        }

        private SortedDictionary<string, double> ToSSXLevel(SortedDictionary<string, double> statResult)
        {
            SortedDictionary<string, double> newDic = new SortedDictionary<string, double>();
            string[] keys = statResult.Keys.ToArray();
            for (int i = 0; i < statResult.Count; i++)
            {
                string key = keys[i];
                if (key == "0")
                    continue;
                string keyString = key.ToString();
                if (keyString.Length != 6)
                {
                    AddUpToDic(newDic, key, statResult[key]);//省级单位
                }
                else if (keyString.Substring(2, 4) == "0000")
                {
                    AddUpToDic(newDic, key, statResult[key]);
                }
                else if (keyString.Substring(4, 2) == "00")
                {
                    AddUpToDic(newDic, key, statResult[key]);//市级单位
                    //将该值加至省级单位
                    string sj = keyString.Substring(0, 2) + "0000";
                    AddUpToDic(newDic, sj, statResult[key]);
                }
                else if (keyString.Substring(4, 2) != "00")
                {
                    AddUpToDic(newDic, key, statResult[key]);//县级级单位
                    //将该值加至市级单位
                    string shiJi = keyString.Substring(0, 4) + "00";
                    AddUpToDic(newDic, shiJi, statResult[key]);
                    //将该值加至省级单位
                    string shengJi = keyString.Substring(0, 2) + "0000";
                    AddUpToDic(newDic, shengJi, statResult[key]);
                }
            }
            return newDic;
        }

        private void AddUpToDic(IDictionary<string, double> newDic, string key, double addValue)
        {
            if (newDic.ContainsKey(key))
                newDic[key] += addValue;
            else
                newDic.Add(key, addValue);
        }

        /// <summary>
        /// 单条件统计
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="filter"></param>
        /// <param name="progressTracker"></param>
        /// <returns></returns>
        public IExtractResult StatRaster<T>(SubProductInstanceDef instance, Func<T, bool> filter, Action<int, string> progressTracker)
        {
            return StatRaster<T>(instance, filter, false, progressTracker, false);
        }

        public IExtractResult StatRaster<T>(SubProductInstanceDef instance, Func<T, bool> filter, bool isStatAreaPersent, Action<int, string> progressTracker)
        {
            return StatRaster<T>(instance, filter, isStatAreaPersent, progressTracker, false);
        }

        public IExtractResult StatRaster<T>(SubProductInstanceDef instance, Func<T, bool> filter, bool isStatAreaPersent, Action<int, string> progressTracker, bool isCombinSameDay)
        {
            switch (instance.AOIProvider)
            {
                case "省市县行政区划":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("三级行政区划");
                        return StatRasterXJ<T>(_subProductDef.ProductDef.Name, _subProductDef.ProductDef.Identify, instance.Name, "三级行政区划", statString, instance.OutFileIdentify, filter, progressTracker, isCombinSameDay);
                    }
                case "省级行政区划":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("行政区划");
                        return StatRasterByVector<T>(instance.Name, statString, filter, isStatAreaPersent, progressTracker, isCombinSameDay);
                    }
                case "土地利用类型":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("土地利用类型");
                        return StatRasterByVector<T>(instance.Name, statString, filter, isStatAreaPersent, progressTracker, isCombinSameDay);
                    }
                case "土地利用类型(栅格)":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("土地利用类型(栅格)");
                        return StatRasterXJ<T>(_subProductDef.ProductDef.Name, _subProductDef.ProductDef.Identify, instance.Name, instance.AOIProvider, statString, instance.OutFileIdentify, filter, progressTracker, isCombinSameDay);
                    }
                default:
                    break;
            }
            return null;
        }

        public IStatResult StatRasterToStatResult<T>(SubProductInstanceDef instance, Func<T, bool> filter, Action<int, string> progressTracker)
        {
            return StatRasterToStatResult<T>(instance, filter, false, progressTracker, false);
        }

        public IStatResult StatRasterToStatResult<T>(SubProductInstanceDef instance, Func<T, bool> filter, bool isStatAreaPersent, Action<int, string> progressTracker)
        {
            return StatRasterToStatResult<T>(instance, filter, isStatAreaPersent, progressTracker, false);
        }

        public IStatResult StatRasterToStatResult<T>(SubProductInstanceDef instance, Func<T, bool> filter, bool isStatAreaPersent, Action<int, string> progressTracker, bool isCombinSameDay)
        {
            switch (instance.AOIProvider)
            {
                case "省市县行政区划":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("三级行政区划");
                        return StatRasterXJToStatResult<T>(_subProductDef.ProductDef.Name, _subProductDef.ProductDef.Identify, instance.Name, "三级行政区划", statString, instance.OutFileIdentify, filter, progressTracker, isCombinSameDay);
                    }
                case "省级行政区划":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("行政区划");
                        return StatRasterByVectorToStatResult<T>(instance.Name, statString, filter, isStatAreaPersent, progressTracker, isCombinSameDay);
                    }
                case "土地利用类型":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("土地利用类型");
                        return StatRasterByVectorToStatResult<T>(instance.Name, statString, filter, isStatAreaPersent, progressTracker, isCombinSameDay);
                    }
                case "土地利用类型(栅格)":
                    {
                        string statString = AreaStatProvider.GetAreaStatItemFileName("土地利用类型(栅格)");
                        return StatRasterXJToStatResult<T>(_subProductDef.ProductDef.Name, _subProductDef.ProductDef.Identify, instance.Name, instance.AOIProvider, statString, instance.OutFileIdentify, filter, progressTracker, isCombinSameDay);
                    }
                default:
                    break;
            }
            return null;
        }

        private IStatResult StatRasterByVectorToStatResult<T>(string instanceName, string aoiTemplate, Func<T, bool> filter, bool isStatAreaPersent, Action<int, string> progressTracker, bool isCombinSameDay)
        {
            object aioObj = _argumentProvider.GetArg("AOI");
            string[] files = GetStringArray("SelectedPrimaryFiles");
            string outFileIdentify = GetStringArgument("OutFileIdentify"); //outfileidentify
            SortedDictionary<string, StatAreaItem> dic = null;
            if (files == null || files.Length == 0)
                return null;
            dic = RasterStatFactory.Stat(files, aoiTemplate, filter, progressTracker, isCombinSameDay);
            if (dic == null || dic.Count == 0)
                return null;
            Dictionary<string, StatAreaItem> sortDic = OrderStatDic(dic);  //按照覆盖面积
            string subTitle = GetSubTitle(files);
            IStatResult results;
            if (files.Length == 1)
                results = DicToStatResultSingleFile(sortDic, subTitle);
            else
            {
                results = DicToStatResult(sortDic, subTitle);
                if (isStatAreaPersent)
                {
                    Dictionary<string, double[]> statPercent = RasterStatFactory.StatPercent(files, aoiTemplate, filter, progressTracker);
                    if (statPercent != null && statPercent.Count != 0)
                    {
                        results = CombinStatResult(results, statPercent);
                    }
                }
            }
            if (results == null)
                return null;
            return results;
        }

        private IExtractResult StatRasterByVector<T>(string instanceName, string aoiTemplate, Func<T, bool> filter, bool isStatAreaPersent, Action<int, string> progressTracker, bool isCombinSameDay)
        {
            //object aioObj = _argumentProvider.GetArg("AOI");
            //string title = GetStringArgument("title");
            //string[] files = GetStringArray("SelectedPrimaryFiles");
            //string extInfos = GetStringArgument("extinfo");
            //string outFileIdentify = GetStringArgument("OutFileIdentify"); //outfileidentify
            //string productName = _subProductDef.ProductDef.Name.Replace(" ", "").Trim();
            //string productIdentify = _subProductDef.ProductDef.Identify;
            //SortedDictionary<string, StatAreaItem> dic = null;
            //if (files == null || files.Length == 0)
            //    return null;
            //dic = RasterStatFactory.Stat(files, aoiTemplate, filter, progressTracker, isCombinSameDay);
            //if (dic == null || dic.Count == 0)
            //    return null;
            //Dictionary<string, StatAreaItem> sortDic = OrderStatDic(dic);  //按照覆盖面积
            //title = productName + instanceName;
            //string subTitle = GetSubTitle(files);
            //IStatResult results;
            //if (files.Length == 1)
            //    results = DicToStatResultSingleFile(sortDic, subTitle);
            //else
            //{
            //    results = DicToStatResult(sortDic, subTitle);
            //    if (isStatAreaPersent)
            //    {
            //        Dictionary<string, double[]> statPercent = RasterStatFactory.StatPercent(files, aoiTemplate, filter, progressTracker);
            //        if (statPercent != null && statPercent.Count != 0)
            //        {
            //            results = CombinStatResult(results, statPercent);
            //        }
            //    }
            //}
            //if (results == null)
            //    return null;
            //string filename = StatResultToFile(files, results, productIdentify, outFileIdentify, title, extInfos, 1, false, 1, 1);
            //return new FileExtractResult(outFileIdentify, filename);

            string title = GetStringArgument("title");
            string[] files = GetStringArray("SelectedPrimaryFiles");
            string extInfos = GetStringArgument("extinfo");
            string outFileIdentify = GetStringArgument("OutFileIdentify"); //outfileidentify
            string productName = _subProductDef.ProductDef.Name.Replace(" ", "").Trim();
            string productIdentify = _subProductDef.ProductDef.Identify;
            if (files == null || files.Length == 0)
                return null;
            title = productName + instanceName;
            string subTitle = GetSubTitle(files);
            IStatResult results = StatRasterByVectorToStatResult<T>(instanceName, aoiTemplate, filter, isStatAreaPersent, progressTracker, isCombinSameDay);
            if (results == null)
                return null;
            string filename = StatResultToFile(files, results, productIdentify, outFileIdentify, title, extInfos, 1, false, 1, 1);
            return new FileExtractResult(outFileIdentify, filename);
        }

        private IStatResult CombinStatResult(IStatResult results, Dictionary<string, double[]> statPercent)
        {
            List<string[]> newRows = new List<string[]>();
            foreach (string[] row in results.Rows)
            {
                List<string> newRow = new List<string>();
                newRow.AddRange(row);
                string key = row[0];
                string[] newValuesString = new string[2];
                if (statPercent.ContainsKey(key))
                {
                    double[] newValues = statPercent[key];
                    newValuesString[0] = newValues[newValues.Length - 2].ToString();
                    newValuesString[1] = newValues[newValues.Length - 1].ToString();
                }
                newRow.AddRange(newValuesString);
                newRows.Add(newRow.ToArray());
            }
            List<string> newColumnTitles = new List<string>();
            newColumnTitles.AddRange(results.Columns);
            newColumnTitles.AddRange(new string[] { "最大面积值(平方公里)", "最大面积百分比(%)" });

            return new StatResult(results.Title, newColumnTitles.ToArray(), newRows.ToArray());
        }

        private Dictionary<string, StatAreaItem> OrderStatDic(SortedDictionary<string, StatAreaItem> dic)
        {
            IOrderedEnumerable<KeyValuePair<string, StatAreaItem>> items = dic.OrderByDescending((t) => { return t.Value.Cover; });
            Dictionary<string, StatAreaItem> sortDic = new Dictionary<string, StatAreaItem>();
            foreach (KeyValuePair<string, StatAreaItem> item in items)
            {
                sortDic.Add(item.Key, item.Value);
            }
            return sortDic;
        }

        private IStatResult DicToStatResult(Dictionary<string, StatAreaItem> dic, string subTitle)
        {
            string[] columns = new string[] { "统计分类", "覆盖面积(平方公里)", "累计面积(平方公里)" };
            string[] keys = dic.Keys.ToArray();
            List<string[]> rows = new List<string[]>();
            for (int i = 0; i < keys.Length; i++)
            {
                string key = keys[i];
                string[] row = new string[3];
                row[0] = key;
                row[1] = dic[key].Cover.ToString();
                row[2] = dic[key].GrandTotal.ToString();
                rows.Add(row);
            }
            if (rows == null || rows.Count == 0)
                return null;
            else
                return new StatResult(subTitle, columns, rows.ToArray());
        }

        private IStatResult DicToStatResultSingleFile(Dictionary<string, StatAreaItem> dic, string subTitle)
        {
            string[] columns = new string[] { "统计分类", "覆盖面积(平方公里)" };
            string[] keys = dic.Keys.ToArray();
            List<string[]> rows = new List<string[]>();
            for (int i = 0; i < keys.Length; i++)
            {
                string key = keys[i];
                string[] row = new string[2];
                row[0] = key;
                row[1] = dic[key].Cover.ToString();
                rows.Add(row);
            }
            if (rows == null || rows.Count == 0)
                return null;
            else
                return new StatResult(subTitle, columns, rows.ToArray());
        }

        private IStatResult StatRasterXJToStatResult<T>(string productName, string productIdentify, string instanceName, string instanceAOI, string aoiTemplate, string outFileIdentify, Func<T, bool> filter, Action<int, string> progressTracker, bool isCombinSameDay)
        {
            object aioObj = _argumentProvider.GetArg("AOI");
            string[] files = GetStringArray("SelectedPrimaryFiles");
            SortedDictionary<string, StatAreaItem> dic = null;
            if (files == null || files.Length == 0)
                return null;
            dic = RasterStatFactory.Stat<T>(files, aoiTemplate, filter, progressTracker, isCombinSameDay);
            if (dic == null || dic.Count == 0)
                return null;
            SortedDictionary<string, string> keyNmaeDic = GetRasterStatDictionary(aoiTemplate);
            string subTitle = GetSubTitle(files);
            IStatResult results = SSXDicToStatResult(dic, subTitle, instanceAOI, keyNmaeDic);
            if (results == null)
                return null;
            return results;
        }

        private IExtractResult StatRasterXJ<T>(string productName, string productIdentify, string instanceName, string instanceAOI, string aoiTemplate, string outFileIdentify, Func<T, bool> filter, Action<int, string> progressTracker, bool isCombinSameDay)
        {
            //object aioObj = _argumentProvider.GetArg("AOI");
            //string title = GetStringArgument("title");
            //string[] files = GetStringArray("SelectedPrimaryFiles");
            //string extInfos = GetStringArgument("extinfo");
            //SortedDictionary<string, StatAreaItem> dic = null;
            //if (files == null || files.Length == 0)
            //    return null;
            //dic = RasterStatFactory.Stat<T>(files, aoiTemplate, filter, progressTracker, isCombinSameDay);
            //if (dic == null || dic.Count == 0)
            //    return null;
            //title = productName.Replace(" ", "").Trim() + instanceName;
            //SortedDictionary<string, string> keyNmaeDic = GetRasterStatDictionary(aoiTemplate);
            //string subTitle = GetSubTitle(files);
            //IStatResult results = SSXDicToStatResult(dic, subTitle, instanceAOI, keyNmaeDic);
            //if (results == null)
            //    return null;
            //string filename = StatResultToFile(files, results, productIdentify, outFileIdentify, title, extInfos, 1, false, 0);
            //return new FileExtractResult(outFileIdentify, filename);


            string title = GetStringArgument("title");
            string[] files = GetStringArray("SelectedPrimaryFiles");
            string extInfos = GetStringArgument("extinfo");
            if (files == null || files.Length == 0)
                return null;
            title = productName.Replace(" ", "").Trim() + instanceName;
            IStatResult results = StatRasterXJToStatResult<T>(_subProductDef.ProductDef.Name, _subProductDef.ProductDef.Identify, instanceName, "三级行政区划", aoiTemplate, outFileIdentify, filter, progressTracker, isCombinSameDay);
            if (results == null)
                return null;
            string filename = StatResultToFile(files, results, productIdentify, outFileIdentify, title, extInfos, 1, false, 0);
            return new FileExtractResult(outFileIdentify, filename);
        }

        private IStatResult SSXDicToStatResult(SortedDictionary<string, StatAreaItem> dic, string subTitle, string templateName, SortedDictionary<string, string> keyNmaeDic)
        {
            //获取列名称
            string[] columns;
            string[] columnName = GetRasterStatColumnName(templateName);
            if (columnName == null)
                columns = new string[] { "省/直辖市", "市/区", "县", "覆盖面积(平方公里)", "累计面积(平方公里)" };
            else
            {
                columns = new string[columnName.Length + 2];
                for (int i = 0; i < columnName.Length; i++)
                {
                    columns[i] = columnName[i];
                }
                columns[columnName.Length] = "覆盖面积(平方公里)";
                columns[columnName.Length + 1] = "累计面积(平方公里)";
            }
            if (templateName == "三级行政区划")
            {
                string[] keys = dic.Keys.ToArray();
                List<string[]> rows = new List<string[]>();
                for (int i = 0; i < keys.Length; i++)
                {
                    string[] row = new string[5];
                    string key = keys[i];
                    string name = null;
                    if (keyNmaeDic != null)
                        keyNmaeDic.TryGetValue(key, out name);
                    string keyString = key.ToString();
                    if (keyString.Length != 6)//未知区域或者海外。
                    {
                        row[0] = string.IsNullOrWhiteSpace(name) ? keyString : name;
                    }
                    else if (keyString.Substring(2, 4) == "0000")
                    {
                        row[0] = string.IsNullOrWhiteSpace(name) ? keyString : name;
                    }
                    else if (keyString.Substring(4, 2) == "00")//市
                    {
                        if (!string.IsNullOrWhiteSpace(name))//代替省字符
                        {
                            string sj = keyString.Substring(0, 2) + "0000";
                            string sjName = "";
                            if (keyNmaeDic != null)
                                keyNmaeDic.TryGetValue(sj, out sjName);
                            if (!string.IsNullOrWhiteSpace(sjName))
                                name = name.Replace(sjName, "");
                        }
                        row[1] = string.IsNullOrWhiteSpace(name) ? keyString : name;
                    }
                    else if (keyString.Substring(4, 2) != "00")//县
                    {
                        if (!string.IsNullOrWhiteSpace(name))//代替省市字符
                        {
                            string sj = keyString.Substring(0, 4) + "00";
                            string sjName = "";
                            if (keyNmaeDic != null)
                                keyNmaeDic.TryGetValue(sj, out sjName);
                            if (!string.IsNullOrWhiteSpace(sjName))
                                name = name.Replace(sjName, "");
                        }
                        row[2] = string.IsNullOrWhiteSpace(name) ? keyString : name;
                    }
                    row[3] = dic[key].Cover.ToString();
                    row[4] = dic[key].GrandTotal.ToString();
                    rows.Add(row);
                }
                if (rows == null || rows.Count == 0)
                    return null;
                else
                    return new StatResult(subTitle, columns, rows.ToArray());
            }
            else
            {
                string[] keys = dic.Keys.ToArray();
                List<string[]> rows = new List<string[]>();
                string name;
                for (int i = 0; i < keys.Length; i++)
                {
                    string[] row = new string[3];
                    keyNmaeDic.TryGetValue(keys[i], out name);
                    if (string.IsNullOrWhiteSpace(name))
                        continue;
                    row[0] = name;
                    row[1] = dic[keys[i]].Cover.ToString();
                    row[2] = dic[keys[i]].GrandTotal.ToString();
                    rows.Add(row);
                }
                if (rows == null || rows.Count == 0)
                    return null;
                else
                {
                    string[] row = new string[3];
                    row[0] = "合计";
                    double cover = 0;
                    double grandTotal = 0;
                    foreach (string[] item in rows)
                    {
                        cover += double.Parse(item[1]);
                        grandTotal += double.Parse(item[2]);
                    }
                    row[1] = cover.ToString();
                    row[2] = grandTotal.ToString();
                    rows.Add(row);
                    return new StatResult(subTitle, columns, rows.ToArray());
                }
            }
        }

        private string[] GetRasterStatColumnName(string templateName)
        {
            if (string.IsNullOrEmpty(templateName))
                return null;
            string[] columnNames = AreaStatProvider.GetColumnNameArrayByName(templateName);
            return columnNames;
        }

        private static string GetSubTitle(string[] files)
        {
            string subTitle = "统计日期：" + DateTime.Now.ToShortDateString();
            string orbitTimes = string.Empty;
            foreach (string item in files)
            {
                if (!File.Exists(item))
                    break;
                RasterIdentify rasterId = new RasterIdentify(item);
                orbitTimes += rasterId.OrbitDateTime.ToShortDateString() + " ";
            }
            return subTitle += "\n" + "轨道时间：" + orbitTimes;
        }

        private SortedDictionary<string, string> GetRasterStatDictionary(string templateName)
        {
            if (string.IsNullOrEmpty(templateName))
                return null;
            string[] split = templateName.Split(':');
            if (split.Length != 3)
                return null;
            string codeFileName = split[2];
            string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "SystemData\\RasterTemplate\\", "*.txt");
            foreach (string file in files)
            {
                if (Path.GetFileName(file) == codeFileName)
                {
                    codeFileName = file;
                    break;
                }
            }
            if (!File.Exists(codeFileName))
                return null;
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
            string[] lines = File.ReadAllLines(codeFileName, Encoding.Default);
            string[] parts = null;
            foreach (string lne in lines)
            {
                parts = lne.Split(new char[] { '\t', '=' });
                dic.Add(parts[0], parts[1]);
            }
            return dic;
        }
        #endregion

        #region 相邻时次面积统计
        public IExtractResult StatCompareArea<T1, T>(SubProductInstanceDef instance, string productIdentify, string title, Action<int, string> progressTracker)
            where T : struct, IComparable, IConvertible
            where T1 : IComparable
        {
            if (instance == null)
                return null;
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            //文件列表排序
            string[] dstFiles = SortFileName(files);
            TypeConverter t1Converter = TypeDescriptor.GetConverter(typeof(T1));
            T1 ot1 = (T1)t1Converter.ConvertFromString("0");
            TypeConverter tConverter = TypeDescriptor.GetConverter(typeof(T));
            T1 it1 = (T1)t1Converter.ConvertFromString("1");
            T ot = (T)tConverter.ConvertFromString("0");
            T it = (T)tConverter.ConvertFromString("5"); //缩小
            T et = (T)tConverter.ConvertFromString("1"); //不变
            T tt = (T)tConverter.ConvertFromString("4"); //扩大
            Func<T1, T1, T> function =
            (fstFileValue, sedFileValue) =>
            {
                //
                if (fstFileValue.CompareTo(sedFileValue) == 0)
                {
                    if (fstFileValue.CompareTo(ot1) == 0)
                        return ot;
                    else
                        return et;
                }
                //
                else
                {
                    if (fstFileValue.CompareTo(sedFileValue) > 0)
                        return it;
                    else
                        return tt;
                }
            };
            //生成多个对比文件
            IInterestedRaster<T> iir = null;
            List<string> compareFile = new List<string>();
            compareFile.Add(dstFiles[0]);
            for (int i = 0; i < dstFiles.Length - 1; i++)
            {
                IPixelFeatureMapper<T> rasterResult = MakeCompareRaster<T1, T>(productIdentify, dstFiles[i], dstFiles[i + 1], function, false);
                RasterIdentify id = new RasterIdentify(dstFiles[i]);
                id.ThemeIdentify = "CMA";
                id.ProductIdentify = _subProductDef.ProductDef.Identify;
                id.SubProductIdentify = _subProductDef.Identify;
                iir = new InterestedRaster<T>(id, rasterResult.Size, rasterResult.CoordEnvelope.Clone());
                iir.Put(rasterResult);
                compareFile.Add(iir.FileName);
                if (iir != null)
                    iir.Dispose();
            }
            Dictionary<string, Func<T, bool>> dic = new Dictionary<string, Func<T, bool>>();
            dic.Add("新增面积（平方公里）", (v) => { return (v.CompareTo(tt) == 0); });
            dic.Add("持续面积（平方公里）", (v) => { return (v.CompareTo(et) == 0); });
            dic.Add("减少面积（平方公里）", (v) => { return (v.CompareTo(it) == 0); });
            return StatRaster<T>(instance, compareFile.ToArray(), dic, progressTracker);
        }

        #endregion

        public IStatResult AreaStatResultToStatResult<T>(string filename, string productName, string productIdentify, Func<T, bool> filter)
        {
            object aioObj = _argumentProvider.GetArg("AOI");
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            string[] files = new string[] { filename };
            string extInfos = GetStringArgument("extinfo");
            if (files == null || files.Length == 0)
                return null;
            string title = string.Empty;
            string[] dstFiles = SortFileName(files);
            StatResultItem[] areaResult = GetStatResultsForMultiFiles<T>(productName, files, ref title, aioObj, filter);
            if (areaResult == null)
                return null;
            return StatResultToIStatResult(files, areaResult, productIdentify, outFileIdentify, title, extInfos, 1);
        }

        public IExtractResult AreaStatResult<T>(string productName, string productIdentify, Func<T, bool> filter)
        {
            object aioObj = _argumentProvider.GetArg("AOI");
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            string[] files = GetStringArray("SelectedPrimaryFiles");
            string extInfos = GetStringArgument("extinfo");
            if (files == null || files.Length == 0)
                return null;
            string title = string.Empty;
            string[] dstFiles = SortFileName(files);
            StatResultItem[] areaResult = GetStatResultsForMultiFiles<T>(productName, files, ref title, aioObj, filter);
            if (areaResult == null)
                return null;
            string filename = StatResultToFile(files, areaResult, productIdentify, outFileIdentify, title, extInfos, 1);
            return new FileExtractResult(outFileIdentify, filename);
        }

        private StatResultItem[] GetStatResultsForMultiFiles<T>(string productName, string[] statFiles, ref string title, object aoiObj, Func<T, bool> filter)
        {
            StatResultItem[] areaResult = null;
            if (statFiles == null || statFiles.Length < 1)
                return areaResult;
            if (statFiles.Length == 1)
            {
                areaResult = CommProductStat.AreaStat<T>(productName, statFiles[0], ref title, aoiObj, filter);
            }
            else
            {
                List<StatResultItem> resultList = new List<StatResultItem>();
                for (int i = 0; i < statFiles.Length; i++)
                {
                    areaResult = CommProductStat.AreaStat<T>(productName, statFiles[i], ref title, aoiObj, filter);

                    if (areaResult != null && areaResult.Length > 0)
                    {
                        RasterIdentify id = new RasterIdentify(statFiles[i]);
                        foreach (StatResultItem item in areaResult)
                        {
                            item.Code = id.OrbitDateTime.ToString();
                        }
                        resultList.AddRange(areaResult);
                    }
                }
                if (resultList.Count <= 0)
                    return null;
                areaResult = resultList.ToArray();
            }
            return areaResult;
        }

        public IExtractResult AreaStatResult<T>(string productName, string productIdentify, Func<T, int, int> weight, int zoom)
        {
            object aioObj = _argumentProvider.GetArg("AOI");
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            string[] files = GetStringArray("SelectedPrimaryFiles");
            string extInfos = GetStringArgument("extinfo");
            if (files == null || files.Length == 0)
                return null;
            string title = string.Empty;
            StatResultItem[] areaResult = CommProductStat.AreaStat<T>(productName, files[0], ref title, aioObj, weight);
            if (areaResult == null)
                return null;
            string filename = StatResultToFile(files, areaResult, productIdentify, outFileIdentify, title, extInfos, zoom);
            return new FileExtractResult(outFileIdentify, filename);
        }

        public IStatResult AreaStatResultToStatResult<T>(string filename, string productName, string productIdentify, Func<T, int, int> weight, int zoom)
        {
            object aioObj = _argumentProvider.GetArg("AOI");
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            string[] files = new string[] { filename };
            string extInfos = GetStringArgument("extinfo");
            if (files == null || files.Length == 0)
                return null;
            string title = string.Empty;
            StatResultItem[] areaResult = CommProductStat.AreaStat<T>(productName, files[0], ref title, aioObj, weight);
            if (areaResult == null)
                return null;
            return StatResultToIStatResult(files, areaResult, productIdentify, outFileIdentify, title, extInfos, zoom);
        }


        public IExtractResult TimesStatAnalysisByPixel<T>(string productName, string productIdentify, string extInfos,
            Func<T, T, T> function, Action<int, string> progressTracker)
        {
            IExtractResult extractResult = null;

            IRasterOperator<T> roper = new RasterOperator<T>();
            IInterestedRaster<T> timeResult = null;
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            if (string.IsNullOrEmpty(extInfos))
                extInfos = GetStringArgument("extinfo");
            DataIdentify di = GetDataIdentify();
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            RasterIdentify identify = CreatRasterIndetifyId(files, productIdentify, outFileIdentify, di, null, extInfos);


            string dateString = "";
            if (files.Length > 1 && identify.MaxOrbitDate != DateTime.MaxValue &&
                identify.MinOrbitDate != DateTime.MinValue)
                dateString = identify.MinOrbitDate.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " ~ " +
                             identify.MaxOrbitDate.AddHours(8).ToString("yyyy年MM月dd日 HH:mm");
            else
                dateString = identify.OrbitDateTime.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " (北京时)";
            string timesCount = "";
            if (identify.ObritTiems != null)
                timesCount = identify.ObritTiems.Length.ToString();

            timeResult = roper.Times(files, identify, progressTracker, function);

            if (timeResult == null)
                return null;
            object obj = _argumentProvider.GetArg("ThemeGraphyGenerator");

            if (obj == null)
                return timeResult;
            IThemeGraphGenerator tgg = obj as IThemeGraphGenerator;
            if (tgg == null)
                return timeResult;

            timeResult.Dispose();
            if (!(tgg is CmaThemeGraphGenerator))
            {
                ISmartSession session = _argumentProvider.GetArg("SmartSession") as ISmartSession;
                if (session != null)
                {
                    tgg = new GeoDo.RSS.MIF.Prds.Comm.CmaThemeGraphGenerator(session);
                }
                else
                    return null;
            }
            string aoiTemplateName = string.Empty;
            Dictionary<string, int[]> aoi = null;
            GetAOI(out aoiTemplateName, out aoi);
            string templatName = GetStringArgument("ThemeGraphTemplateName");
            string colorTabelName = GetColorTableName("colortablename");
            if (_argumentProvider.GetArg("ThemeEnvelope") != null)
            {
                PrjEnvelope useRegionGeo = GetEnvelopeFromArg("ThemeEnvelope");
                if (useRegionGeo != null)
                    (tgg as CmaThemeGraphGenerator).SetEnvelope(new Layout.GxdEnvelope(useRegionGeo.MinX,
                        useRegionGeo.MaxX, useRegionGeo.MinY, useRegionGeo.MaxY));
            }
            (tgg as CmaThemeGraphGenerator).DateString = dateString;
            (tgg as CmaThemeGraphGenerator).TimesCount = timesCount;

            int[] aois = MasicAOI(aoi, ref extInfos);
            tgg.Generate(timeResult.FileName, templatName, aois, extInfos, outFileIdentify, colorTabelName);

            string resultFilename = tgg.Save();
            if (string.IsNullOrEmpty(resultFilename))
                return timeResult;

            extractResult = new FileExtractResult(outFileIdentify, resultFilename);

            return extractResult;
        }

        public IExtractResult TimesStatAnalysisByRasterMap<T>(string productName, string productIdentify, string extInfos, float outResolution,
            Action<int, string> progressTracker)
        {
            IExtractResult extractResult = null;

            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            if (string.IsNullOrEmpty(extInfos))
                extInfos = GetStringArgument("extinfo");
            DataIdentify di = GetDataIdentify();
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            RasterIdentify identify = CreatRasterIndetifyId(files, productIdentify, outFileIdentify, di, null, extInfos);


            string dateString = "";
            if (files.Length > 1 && identify.MaxOrbitDate != DateTime.MaxValue &&
                identify.MinOrbitDate != DateTime.MinValue)
                dateString = identify.MinOrbitDate.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " ~ " +
                             identify.MaxOrbitDate.AddHours(8).ToString("yyyy年MM月dd日 HH:mm");
            else
                dateString = identify.OrbitDateTime.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " (北京时)";
            string timesCount = "";
            if (identify.ObritTiems != null)
                timesCount = identify.ObritTiems.Length.ToString();

            //日最大合成
            files = MaxDayMosaic(files, outResolution);
            string filename;
            //输入文件准备
            int bandNo = 1;
            List<RasterMaper> rms = new List<RasterMaper>();
            try
            {
                for (int i = 0; i < files.Length; i++)
                {
                    IRasterDataProvider inRaster = RasterDataDriver.Open(files[i]) as IRasterDataProvider;
                    if (inRaster.BandCount < bandNo)
                        continue;
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { bandNo });
                    rms.Add(rm);
                }
                Feature[] fets = GetAOIFeatues();
                //输出文件准备（作为输入栅格并集处理）
                filename = identify.ToWksFullFileName(".dat");
                using (IRasterDataProvider outRaster = CreateOutRaster(filename, rms.ToArray(), outResolution))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<Int16, Int16> rfr = null;
                    rfr = new RasterProcessModel<Int16, Int16>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    if (fets != null && fets.Length != 0)
                        rfr.SetFeatureAOI(fets);
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        short[] maxData = new short[dataLength];
                        if (fets == null || fets.Length == 0)
                            foreach (RasterVirtualVistor<Int16> rv in rvInVistor)
                            {
                                Int16[] dt = rv.RasterBandsData[0];
                                if (dt != null)
                                {
                                    for (int index = 0; index < dataLength; index++)
                                    {
                                        if (dt[index] != 0)
                                            rvOutVistor[0].RasterBandsData[0][index]++;
                                    }
                                }
                            }
                        else if (fets != null && fets.Length != 0 && aoi != null && aoi.Length != 0)
                            foreach (RasterVirtualVistor<Int16> rv in rvInVistor)
                            {
                                Int16[] dt = rv.RasterBandsData[0];
                                if (dt != null)
                                {
                                    for (int index = 0; index < aoi.Length; index++)
                                    {
                                        if (dt[aoi[index]] != 0)
                                            rvOutVistor[0].RasterBandsData[0][aoi[index]]++;
                                    }
                                }
                            }
                    }));
                    //执行
                    rfr.Excute();
                }
            }
            finally
            {
                foreach (RasterMaper rm in rms)
                {
                    rm.Raster.Dispose();
                }
            }

            object obj = _argumentProvider.GetArg("ThemeGraphyGenerator");
            if (obj == null)
                return new FileExtractResult(outFileIdentify, filename);
            IThemeGraphGenerator tgg = obj as IThemeGraphGenerator;
            if (tgg == null)
                return new FileExtractResult(outFileIdentify, filename);
            if (!(tgg is CmaThemeGraphGenerator))
            {
                ISmartSession session = _argumentProvider.GetArg("SmartSession") as ISmartSession;
                if (session != null)
                {
                    tgg = new GeoDo.RSS.MIF.Prds.Comm.CmaThemeGraphGenerator(session);
                }
                else
                    return null;
            }
            string templatName = GetStringArgument("ThemeGraphTemplateName");
            string colorTabelName = GetColorTableName("colortablename");
            if (_argumentProvider.GetArg("ThemeEnvelope") != null)
            {
                PrjEnvelope useRegionGeo = GetEnvelopeFromArg("ThemeEnvelope");
                if (useRegionGeo != null)
                    (tgg as CmaThemeGraphGenerator).SetEnvelope(new Layout.GxdEnvelope(useRegionGeo.MinX,
                        useRegionGeo.MaxX, useRegionGeo.MinY, useRegionGeo.MaxY));
            }
            (tgg as CmaThemeGraphGenerator).DateString = dateString;
            (tgg as CmaThemeGraphGenerator).TimesCount = timesCount;
            tgg.Generate(filename, templatName, null, extInfos, outFileIdentify, colorTabelName);

            string resultFilename = tgg.Save();
            if (string.IsNullOrEmpty(resultFilename))
                return new FileExtractResult(outFileIdentify, filename);

            extractResult = new FileExtractResult(outFileIdentify, resultFilename);

            return extractResult;
        }

        private Feature[] GetAOIFeatues()
        {
            object obj = _argumentProvider.GetArg("AOI");
            if (obj == null)
                return null;
            obj = _argumentProvider.GetArg("AOIFeatures");
            if (obj != null)
                return obj as Feature[];
            return null;
        }

        private string[] MaxDayMosaic(string[] files, float outResolution)
        {
            Dictionary<string, List<string>> sameDayFiles = new Dictionary<string, List<string>>();
            RasterIdentify rid = null;
            string dateStr = null;
            foreach (string file in files)
            {
                rid = new RasterIdentify(file);
                dateStr = rid.OrbitDateTime.ToString("yyyyMMdd");
                if (sameDayFiles.ContainsKey(dateStr))
                    sameDayFiles[dateStr].Add(file);
                else
                {
                    sameDayFiles[dateStr] = new List<string>();
                    sameDayFiles[dateStr].Add(file);
                }
            }
            List<string> resultFiles = new List<string>();
            foreach (string key in sameDayFiles.Keys)
            {
                if (sameDayFiles[key].Count == 1)
                    resultFiles.Add(sameDayFiles[key][0]);
                else
                {
                    //输入文件准备
                    int bandNo = 1;
                    List<RasterMaper> rms = new List<RasterMaper>();
                    try
                    {
                        int length = sameDayFiles[key].Count;
                        for (int i = 0; i < length; i++)
                        {
                            IRasterDataProvider inRaster = RasterDataDriver.Open(sameDayFiles[key][i]) as IRasterDataProvider;
                            if (inRaster.BandCount < bandNo)
                                continue;
                            RasterMaper rm = new RasterMaper(inRaster, new int[] { bandNo });
                            rms.Add(rm);
                        }
                        //输出文件准备（作为输入栅格并集处理）
                        RasterIdentify identify = new RasterIdentify(sameDayFiles[key][0]);
                        string filename = Path.Combine(MifEnvironment.GetTempDir(), identify.ToWksFileName(".dat"));
                        using (IRasterDataProvider outRaster = CreateOutRaster(filename, rms.ToArray(), outResolution))
                        {
                            //栅格数据映射
                            RasterMaper[] fileIns = rms.ToArray();
                            RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                            //创建处理模型
                            RasterProcessModel<Int16, Int16> rfr = null;
                            rfr = new RasterProcessModel<Int16, Int16>();
                            rfr.SetRaster(fileIns, fileOuts);
                            rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                            {
                                int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                                short[] maxData = new short[dataLength];
                                foreach (RasterVirtualVistor<Int16> rv in rvInVistor)
                                {
                                    Int16[] dt = rv.RasterBandsData[0];
                                    if (dt != null)
                                    {
                                        for (int index = 0; index < dataLength; index++)
                                            if (dt[index] != 0)
                                                rvOutVistor[0].RasterBandsData[0][index] = dt[index];
                                    }
                                }
                            }));
                            //执行
                            rfr.Excute();
                            if (File.Exists(filename))
                                resultFiles.Add(filename);
                        }
                    }
                    finally
                    {
                        foreach (RasterMaper rm in rms)
                        {
                            rm.Raster.Dispose();
                        }
                    }
                }
            }
            return resultFiles.Count == 0 ? null : resultFiles.ToArray();
        }

        public IExtractResult TimesStatAnalysisByPixel_tmp<T>(string productName, string productIdentify, string extInfos, Func<T, T, T> function, Action<int, string> progressTracker)
        {
            IRasterOperator<T> roper = new RasterOperator<T>();
            IInterestedRaster<T> timeResult = null;
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            if (string.IsNullOrEmpty(extInfos))
                extInfos = GetStringArgument("extinfo");
            DataIdentify di = GetDataIdentify();
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            RasterIdentify identify = CreatRasterIndetifyId(files, productIdentify, outFileIdentify, di, null, extInfos);
            string dateString = "";
            if (files.Length > 1 && identify.MaxOrbitDate != DateTime.MaxValue && identify.MinOrbitDate != DateTime.MinValue)
                dateString = identify.MinOrbitDate.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " ~ " + identify.MaxOrbitDate.AddHours(8).ToString("yyyy年MM月dd日 HH:mm");
            else
                dateString = identify.OrbitDateTime.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " (北京时)";
            string timesCount = "";
            if (identify.ObritTiems != null)
                timesCount = identify.ObritTiems.Length.ToString();

            timeResult = roper.Times(files, identify, progressTracker, function);
            if (timeResult == null)
                return null;
            object obj = _argumentProvider.GetArg("ThemeGraphyGenerator");
            if (obj == null)
                return timeResult;
            IThemeGraphGenerator tgg = obj as IThemeGraphGenerator;
            if (tgg == null)
                return timeResult;
            timeResult.Dispose();
            if (!(tgg is CmaThemeGraphGenerator))
            {
                ISmartSession session = _argumentProvider.GetArg("SmartSession") as ISmartSession;
                if (session != null)
                    tgg = new GeoDo.RSS.MIF.Prds.Comm.CmaThemeGraphGenerator(session);
                else
                    return null;
            }
            string aoiTemplateName = string.Empty;
            Dictionary<string, int[]> aoi = null;
            GetAOI(out aoiTemplateName, out aoi);
            string templatName = GetStringArgument("ThemeGraphTemplateName");
            string colorTabelName = GetColorTableName("colortablename");
            if (_argumentProvider.GetArg("ThemeEnvelope") != null)
            {
                PrjEnvelope useRegionGeo = GetEnvelopeFromArg("ThemeEnvelope");
                if (useRegionGeo != null)
                    (tgg as CmaThemeGraphGenerator).SetEnvelope(new Layout.GxdEnvelope(useRegionGeo.MinX, useRegionGeo.MaxX, useRegionGeo.MinY, useRegionGeo.MaxY));
            }
            (tgg as CmaThemeGraphGenerator).DateString = dateString;
            (tgg as CmaThemeGraphGenerator).TimesCount = timesCount;
            int[] aois = MasicAOI(aoi, ref extInfos);
            tgg.Generate(timeResult.FileName, templatName, aois, extInfos, outFileIdentify, colorTabelName);
            string resultFilename = tgg.Save();
            if (string.IsNullOrEmpty(resultFilename))
                return timeResult;
            return new FileExtractResult(outFileIdentify, resultFilename);
        }

        public IFileExtractResult TimesStatAnalysisByPixel<T>(string productIdentify, string extInfos, Func<T, T, T> function, Action<int, string> progressTracker)
        {
            IRasterOperator<T> roper = new RasterOperator<T>();
            IInterestedRaster<T> timeResult = null;
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            if (string.IsNullOrEmpty(extInfos))
                extInfos = GetStringArgument("extinfo");
            DataIdentify di = GetDataIdentify();
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            RasterIdentify identify = CreatRasterIndetifyId(files, productIdentify, outFileIdentify, di, null, extInfos);
            string dateString = "";
            if (files.Length > 1 && identify.MaxOrbitDate != DateTime.MaxValue && identify.MinOrbitDate != DateTime.MinValue)
                dateString = identify.MinOrbitDate.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " ~ " + identify.MaxOrbitDate.AddHours(8).ToString("yyyy年MM月dd日 HH:mm");
            else
                dateString = identify.OrbitDateTime.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " (北京时)";
            string timesCount = "";
            if (identify.ObritTiems != null)
                timesCount = identify.ObritTiems.Length.ToString();

            timeResult = roper.Times(files, identify, progressTracker, function);
            if (timeResult == null)
                return null;
            return new FileExtractResult(productIdentify, timeResult.FileName);
        }

        private PrjEnvelope GetEnvelopeFromArg(string argumentName)
        {
            string env = _argumentProvider.GetArg(argumentName) as string;
            if (string.IsNullOrEmpty(env))
                return null;
            else
            {
                bool withBlank = false;
                if (env.ToLower().IndexOf("withblank:") != -1)
                {
                    env = env.Substring(env.IndexOf(":") + 1);
                    withBlank = true;
                }
                string[] latlon = env.Split(',');
                if (latlon == null || latlon.Length < 4)
                    return null;
                double[] geoRange;
                if (!withBlank && !IsSetRange(latlon, out geoRange))
                    return null;
                return new PrjEnvelope(double.Parse(latlon[0]), double.Parse(latlon[1]), double.Parse(latlon[2]), double.Parse(latlon[3]));
            }
        }

        public bool IsSetRange(string[] latlon, out double[] geoRange)
        {
            geoRange = new double[4];
            if (latlon == null || latlon.Length != 4)
                return false;
            double num;
            for (int i = 0; i < latlon.Length; i++)
            {
                if (double.TryParse(latlon[i], out num))
                    geoRange[i] = num;
                else
                    return false;
            }
            string[] fnames = _argumentProvider.GetArg("SelectedPrimaryFiles") as string[];
            if (fnames == null || fnames.Length == 0)
                return false;
            try
            {
                foreach (string name in fnames)
                {
                    using (IRasterDataProvider dataPrd = GeoDataDriver.Open(name) as IRasterDataProvider)
                    {
                        if (dataPrd.CoordEnvelope.MinX > geoRange[0] || dataPrd.CoordEnvelope.MaxX < geoRange[1] ||
                        dataPrd.CoordEnvelope.MinY > geoRange[0] || dataPrd.CoordEnvelope.MaxY < geoRange[3])
                            return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public IExtractResult CycleTimeStatAnalysisByPixel<T>(string productName, string productIdentify, string extInfos, Func<int, T, T, T> function, Action<int, string> progress)
        {
            IRasterOperator<T> roper = new RasterOperator<T>();
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            if (string.IsNullOrEmpty(extInfos))
                extInfos = GetStringArgument("extinfo");
            DataIdentify di = GetDataIdentify();
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            IThemeGraphGenerator tgg = null;
            object theme = _argumentProvider.GetArg("ThemeGraphyGenerator");
            if (theme != null)
                tgg = theme as IThemeGraphGenerator;
            RasterIdentify identify = CreatRasterIndetifyId(files, productIdentify, outFileIdentify, di, null, extInfos);
            string dateString = "";
            if (files.Length > 1 && identify.MaxOrbitDate != DateTime.MaxValue && identify.MinOrbitDate != DateTime.MinValue)
                dateString = identify.MinOrbitDate.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " ~ " + identify.MaxOrbitDate.AddHours(8).ToString("yyyy年MM月dd日 HH:mm");
            else
                dateString = identify.OrbitDateTime.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " (北京时)";
            string filename = null;
            using (IInterestedRaster<T> cycleIimeResult = roper.CycleTimes(files, identify, function, progress))
            {
                if (cycleIimeResult == null)
                    return null;
                filename = cycleIimeResult.FileName;
                if (tgg == null)
                    return cycleIimeResult;
                cycleIimeResult.Dispose();
            }
            string aoiTemplateName = string.Empty;
            Dictionary<string, int[]> aoi = null;
            GetAOI(out aoiTemplateName, out aoi);
            string templatName = GetStringArgument("ThemeGraphTemplateName");
            string colorTabelName = GetColorTableName("colortablename");
            string[] obtTimes = GetOrbitTimes(files);
            //专题图范围
            PrjEnvelope env = null;
            ThemeGraphRegion region = ThemeGraphRegionSetting.GetThemeGraphRegion(_subProductDef.ProductDef.Identify);
            if (region != null && region.SelectedItem != null)
            {
                env = region.SelectedItem.PrjEnvelope;
            }
            ThematicGraphHelper thematic = new ThematicGraphHelper(null, templatName);
            thematic.AOI = MasicAOI(aoi, ref extInfos);
            thematic.SetOrbitTimes(obtTimes);
            thematic.DateString = dateString;
            string resultFilename = thematic.CreateThematicGraphic(filename, outFileIdentify, extInfos, new object[] { colorTabelName, env });
            return new FileExtractResult(outFileIdentify, resultFilename);
        }

        public IExtractResult CycleTimeStatAnalysisByRasterMap<T>(string productName, string productIdentify, string extInfos, float outResolution, Action<int, string> progress)
        {
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            if (string.IsNullOrEmpty(extInfos))
                extInfos = GetStringArgument("extinfo");
            DataIdentify di = GetDataIdentify();
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            IThemeGraphGenerator tgg = null;
            object theme = _argumentProvider.GetArg("ThemeGraphyGenerator");
            if (theme != null)
                tgg = theme as IThemeGraphGenerator;
            RasterIdentify identify = CreatRasterIndetifyId(files, productIdentify, outFileIdentify, di, null, extInfos);
            string dateString = "";
            if (files.Length > 1 && identify.MaxOrbitDate != DateTime.MaxValue && identify.MinOrbitDate != DateTime.MinValue)
                dateString = identify.MinOrbitDate.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " ~ " + identify.MaxOrbitDate.AddHours(8).ToString("yyyy年MM月dd日 HH:mm");
            else
                dateString = identify.OrbitDateTime.AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " (北京时)";
            string filename = null;
            //RasterMap生产周期合成数据
            //输入文件准备
            int bandNo = 1;
            List<RasterMaper> rms = new List<RasterMaper>();
            try
            {
                for (int i = 0; i < files.Length; i++)
                {
                    IRasterDataProvider inRaster = RasterDataDriver.Open(files[i]) as IRasterDataProvider;
                    if (inRaster.BandCount < bandNo)
                        continue;
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { bandNo });
                    rms.Add(rm);
                }
                //输出文件准备（作为输入栅格并集处理）
                filename = identify.ToWksFullFileName(".dat");
                Feature[] fets = GetAOIFeatues();
                using (IRasterDataProvider outRaster = CreateOutRaster(filename, rms.ToArray(), outResolution))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<Int16, Int16> rfr = null;
                    rfr = new RasterProcessModel<Int16, Int16>(progress);
                    rfr.SetRaster(fileIns, fileOuts);
                    if (fets != null && fets.Length != 0)
                        rfr.SetFeatureAOI(fets);
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                        short[] maxData = new short[dataLength];
                        if (fets == null || fets.Length == 0)
                            foreach (RasterVirtualVistor<Int16> rv in rvInVistor)
                            {
                                Int16[] dt = rv.RasterBandsData[0];
                                if (dt != null)
                                {
                                    for (int index = 0; index < dataLength; index++)
                                    {
                                        if (dt[index] != 0)
                                            rvOutVistor[0].RasterBandsData[0][index] = dt[index];
                                    }
                                }
                            }
                        else if (fets != null && fets.Length != 0 && aoi != null && aoi.Length != 0)
                        {
                            foreach (RasterVirtualVistor<Int16> rv in rvInVistor)
                            {
                                Int16[] dt = rv.RasterBandsData[0];
                                if (dt != null)
                                {
                                    for (int index = 0; index < aoi.Length; index++)
                                    {
                                        if (dt[aoi[index]] != 0)
                                            rvOutVistor[0].RasterBandsData[0][aoi[index]] = dt[aoi[index]];
                                    }
                                }
                            }
                        }
                    }));
                    //执行
                    rfr.Excute();
                }
            }
            finally
            {
                foreach (RasterMaper rm in rms)
                {
                    rm.Raster.Dispose();
                }
            }
            string aoiTemplateName = string.Empty;
            string templatName = GetStringArgument("ThemeGraphTemplateName");
            string colorTabelName = GetColorTableName("colortablename");
            string[] obtTimes = GetOrbitTimes(files);
            //专题图范围
            PrjEnvelope env = null;
            ThemeGraphRegion region = ThemeGraphRegionSetting.GetThemeGraphRegion(_subProductDef.ProductDef.Identify);
            if (region != null && region.SelectedItem != null)
            {
                env = region.SelectedItem.PrjEnvelope;
            }
            ThematicGraphHelper thematic = new ThematicGraphHelper(null, templatName);
            thematic.SetOrbitTimes(obtTimes);
            thematic.DateString = dateString;
            string resultFilename = thematic.CreateThematicGraphic(filename, outFileIdentify, extInfos, new object[] { colorTabelName, env });
            return new FileExtractResult(outFileIdentify, resultFilename);
        }

        protected IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] inrasterMaper, float resolution)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            GeoDo.RSS.Core.DF.CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Union(inRaster.Raster.CoordEnvelope);
            }
            float resX, resY;
            if (resolution != 0f)
            {
                resX = resolution;
                resY = resolution;
            }
            else
            {
                resX = inrasterMaper[0].Raster.ResolutionX;
                resY = inrasterMaper[0].Raster.ResolutionY;
            }
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, enumDataType.Int16, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        private string[] GetOrbitTimes(string[] files)
        {
            if (files == null || files.Length == 0)
                return null;
            List<string> orbitName = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                RasterIdentify rd = new RasterIdentify(file);
                orbitName.Add(string.Format("{0}）{1}", i + 1, rd.OrbitDateTime.ToString("yyyy-MM-dd HH:mm")));
            }
            return orbitName.ToArray();
        }

        public IExtractResult CompareAnalysisByPixel<T1, T>(string productName, string productIdentify, string extInfos, Func<T1, T1, T> function)
            where T : IComparable
            where T1 : IComparable
        {
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0 || files.Length == 1)
                return null;
            //文件列表排序
            string[] dstFiles = SortFileName(files);
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            object obj = _argumentProvider.GetArg("ThemeGraphyGenerator");
            if (obj == null)
                return null;
            IThemeGraphGenerator tgg = obj as IThemeGraphGenerator;
            if (tgg == null)
                return null;
            string templatName = GetStringArgument("ThemeGraphTemplateName");
            string colorTabelName = GetColorTableName("colortablename");
            string aoiTemplateName = string.Empty;
            Dictionary<string, int[]> aoi = null;
            GetAOI(out aoiTemplateName, out aoi);
            ExtractResultArray results = new ExtractResultArray(productIdentify + "_COMP");
            for (int i = 0; i < dstFiles.Length - 1; i++)
            {
                //生成专题图
                IPixelFeatureMapper<T> rasterResult = MakeCompareRaster<T1, T>(productIdentify, dstFiles[i], dstFiles[i + 1], function, true);
                if (rasterResult == null)
                    continue;
                RasterIdentify rid = new RasterIdentify(dstFiles[i]);
                IInterestedRaster<T> iir = new InterestedRaster<T>(rid, rasterResult.Size, rasterResult.CoordEnvelope, rasterResult.SpatialRef);
                iir.Put(rasterResult);
                iir.Dispose();
                tgg.Generate(iir.FileName, templatName, MasicAOI(aoi, ref extInfos), extInfos, outFileIdentify, colorTabelName);
                string resultFilename = tgg.Save();
                if (string.IsNullOrEmpty(resultFilename))
                    return null;
                FileExtractResult result = new FileExtractResult(outFileIdentify, resultFilename);
                if (result != null)
                    results.Add(result);
            }
            return results;
        }

        protected string[] SortFileName(string[] files)
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

        //计算生成两文件的对比文件
        protected IPixelFeatureMapper<T> MakeCompareRaster<T1, T>(string productIdentify, string fstFileName, string sedFileName, Func<T1, T1, T> function, bool isAddClound)
            where T : IComparable
            where T1 : IComparable
        {
            Dictionary<string, FilePrdMap> filePrdMap = new Dictionary<string, FilePrdMap>();
            filePrdMap.Add("backWaterPath", new FilePrdMap(fstFileName, 1, new VaildPra(float.MinValue, float.MaxValue), new int[] { 1 }));
            filePrdMap.Add("binWater", new FilePrdMap(sedFileName, 1, new VaildPra(float.MinValue, float.MaxValue), new int[] { 1 }));
            string[] clmFiles = null;
            //查找两文件对应的云结果文件，若有则添加进来
            if (isAddClound)
            {
                clmFiles = FindClmFils(new string[] { fstFileName, sedFileName });
            }
            if (clmFiles != null && clmFiles.Length > 0)
            {
                for (int i = 0; i < clmFiles.Length; i++)
                    filePrdMap.Add("clmFile" + (i + 1), new FilePrdMap(clmFiles[i], 1, new VaildPra(float.MinValue, float.MaxValue), new int[] { 1 }));
            }
            ITryCreateVirtualPrd tryVPrd = new TryCreateVirtualPrdByMultiFile();
            IVirtualRasterDataProvider vrd = tryVPrd.CreateVirtualRasterPRD(ref filePrdMap);
            if (vrd == null)
            {
                if (filePrdMap != null && filePrdMap.Count() > 0)
                {
                    foreach (FilePrdMap item in filePrdMap.Values)
                    {
                        if (item.Prd != null)
                            item.Prd.Dispose();
                    }
                }
                return null;
            }
            if (filePrdMap.Count == 3 || filePrdMap.Count == 4)
            {
                try
                {
                    ArgumentProvider ap = new ArgumentProvider(vrd, null);
                    RasterPixelsVisitor<T1> rpVisitor = new RasterPixelsVisitor<T1>(ap);
                    IPixelFeatureMapper<T> result = new MemPixelFeatureMapper<T>(productIdentify, 1000, new Size(vrd.Width, vrd.Height), vrd.CoordEnvelope, vrd.SpatialRef);
                    TypeConverter tConverter = TypeDescriptor.GetConverter(typeof(T));
                    TypeConverter t1Converter = TypeDescriptor.GetConverter(typeof(T1));
                    T1 ot = (T1)t1Converter.ConvertFromString("1");
                    T it = (T)tConverter.ConvertFromString("0");
                    if (filePrdMap.Count == 4)
                    {
                        rpVisitor.VisitPixel(new int[] { filePrdMap["backWaterPath"].StartBand, filePrdMap["binWater"].StartBand, filePrdMap["clmFile1"].StartBand, filePrdMap["clmFile2"].StartBand },
                            (idx, values) =>
                            {
                                T value = function(values[0], values[1]);
                                if (values[2].CompareTo(ot) == 0 || values[3].CompareTo(ot) == 0)
                                {
                                    result.Put(idx, (T)tConverter.ConvertFromString("9999"));
                                }
                                else
                                    result.Put(idx, value);
                            });
                    }
                    else
                    {
                        rpVisitor.VisitPixel(new int[] { filePrdMap["backWaterPath"].StartBand, filePrdMap["binWater"].StartBand, filePrdMap["clmFile1"].StartBand },
                            (idx, values) =>
                            {
                                T value = function(values[0], values[1]);
                                if (values[2].CompareTo(ot) == 0)
                                {
                                    result.Put(idx, (T)tConverter.ConvertFromString("9999"));
                                }
                                else
                                    result.Put(idx, value);
                            });
                    }
                    return result;
                }
                finally
                {
                    vrd.Dispose();
                    if (filePrdMap != null && filePrdMap.Count() > 0)
                    {
                        foreach (FilePrdMap item in filePrdMap.Values)
                        {
                            if (item.Prd != null)
                                item.Prd.Dispose();
                        }
                    }
                }
            }
            else
            {
                try
                {
                    ArgumentProvider ap = new ArgumentProvider(vrd, null);
                    RasterPixelsVisitor<T1> rpVisitor = new RasterPixelsVisitor<T1>(ap);
                    IPixelFeatureMapper<T> result = new MemPixelFeatureMapper<T>(productIdentify, 1000, new Size(vrd.Width, vrd.Height), vrd.CoordEnvelope, vrd.SpatialRef);
                    rpVisitor.VisitPixel(new int[] { filePrdMap["backWaterPath"].StartBand, filePrdMap["binWater"].StartBand },
                        (idx, values) =>
                        {
                            result.Put(idx, function(values[0], values[1]));
                        });
                    return result;
                }
                finally
                {
                    vrd.Dispose();
                }
            }
        }

        //查找栅格产品文件对应的云判识结果文件
        private string[] FindClmFils(string[] rasterFiles)
        {
            int length = rasterFiles.Length;
            RasterIdentify rid = new RasterIdentify(rasterFiles[0]);
            if (rid == null || string.IsNullOrEmpty(rid.SubProductIdentify))
                return null;
            string identify = rid.SubProductIdentify;
            List<string> clmList = new List<string>();
            for (int i = 0; i < length; i++)
            {
                string clmFile = rasterFiles[i].ToUpper().Replace("_" + identify.ToUpper() + "_", "_0CLM_");
                if (File.Exists(clmFile))
                    clmList.Add(clmFile);
            }
            return clmList.ToArray();
        }

        public int[] GetAOI()
        {
            List<int> aoiResult = new List<int>();
            object obj = _argumentProvider.GetArg("AOI");
            if (obj != null)
                if (obj as Dictionary<string, int[]> != null)
                {
                    Dictionary<string, int[]> aoi = obj as Dictionary<string, int[]>;
                    foreach (string item in aoi.Keys)
                        aoiResult.AddRange(aoi[item]);
                    return aoiResult.ToArray();
                }
            return null;
        }

        protected int TryGetBandNo(IBandNameRaster bandNameRaster, string argName)
        {
            int bandNo = (int)_argumentProvider.GetArg(argName);
            if (bandNameRaster != null)
            {
                int newbandNo = -1;
                if (bandNameRaster.TryGetBandNoFromBandName(bandNo, out newbandNo))
                    bandNo = newbandNo;
            }
            return bandNo;
        }

        protected IStatResult TransposeRowCol(IStatResult dstResult)
        {

            List<string> dstColums = new List<string>();
            List<string[]> dstRowTemp = new List<string[]>();
            List<string> dstColTemp = new List<string>();
            for (int col = 1; col < dstResult.Columns.Length; col++)
            {
                dstColTemp.Add(dstResult.Columns[col]);
                for (int row = 0; row < dstResult.Rows.Length; row++)
                    dstColTemp.Add(dstResult.Rows[row][col]);
                dstRowTemp.Add(dstColTemp.ToArray());
                dstColTemp.Clear();
            }
            dstColums.Add(dstResult.Columns[0]);
            for (int row = 0; row < dstResult.Rows.Length; row++)
                dstColums.Add(dstResult.Rows[row][0]);
            dstResult = new StatResult(dstResult.Title, dstColums.ToArray(), dstRowTemp.ToArray());
            return dstResult;
        }

        #region
        public IExtractResult CHAZStatRasterByVector<T>(string instanceName, string outFileIdentify, string aoiTemplate, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker) where T : struct, IConvertible
        {
            return CHAZStatRasterByVector<T>(instanceName, outFileIdentify, aoiTemplate, filters, progressTracker, false, 1);
        }

        public IExtractResult CHAZStatRasterByVector<T>(string instanceName, string outFileIdentify, string aoiTemplate, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker, bool weight, float weightZoom) where T : struct, IConvertible
        {
            object aioObj = _argumentProvider.GetArg("AOI");
            string title = GetStringArgument("title");
            string[] bjsfiles = GetStringArray("mainfiles");
            string[] jsfiles = GetStringArray("jianshu");
            string extInfos = GetStringArgument("extinfo");
            //string outFileIdentify = GetStringArgument("OutFileIdentify");//outfileidentify
            string statname = GetStringArgument("statname");
            string productName = _subProductDef.ProductDef.Name;
            string productIdentify = _subProductDef.ProductDef.Identify;
            Dictionary<string, SortedDictionary<string, double>> bjsdic, jsdic, dic = null;//<矢量面名称，<分级字段，统计值>>
            if (bjsfiles == null || bjsfiles.Length == 0 || jsfiles == null || jsfiles.Length == 0)
                return null;
            bjsdic = RasterStatFactory.Stat(bjsfiles[0], aoiTemplate, filters, progressTracker, weight, weightZoom);
            if (bjsdic == null || bjsdic.Count == 0)
                return null;
            jsdic = RasterStatFactory.Stat(jsfiles[0], aoiTemplate, filters, progressTracker, weight, weightZoom);
            if (jsdic == null || jsdic.Count == 0)
                return null;
            if (bjsdic.Count != jsdic.Count)
                return null;
            //bool isident = true;
            //foreach (string key in bjsdic.Keys)
            //{
            //    if (!jsdic.ContainsKey(key))
            //    {
            //        isident = false;
            //        break;
            //    }
            //}
            //if (!isident)
            //    return null;
            ////<矢量面名称，<分级字段，统计值>>
            //foreach (string key in jsdic.Keys)
            //{
            //    if (!bjsdic.ContainsKey(key))
            //    {
            //        isident = false;
            //        break;
            //    }
            //}
            //if (!isident)
            //    return null;
            dic = new Dictionary<string, SortedDictionary<string, double>>();
            double chazvalue = 0;
            foreach (string key in bjsdic.Keys)
            {
                if (!dic.ContainsKey(key))
                    dic.Add(key, new SortedDictionary<string, double>());
                foreach (string keyvalue in bjsdic[key].Keys)
                {
                    if (jsdic.ContainsKey(keyvalue))
                    {
                        chazvalue = bjsdic[key][keyvalue] - jsdic[key][keyvalue];
                        if (!dic[key].ContainsKey(keyvalue))
                            dic[key].Add(keyvalue, chazvalue);
                    }
                }
            }
            if (dic.Count <= 0)
                return null;
            title = productName + statname + instanceName;
            string subTitle = GetCHAZSubTitle(bjsfiles[0], jsfiles[0]);
            IStatResult results = DicToStatResult(dic, filters.Keys.ToArray(), subTitle);
            if (results == null)
                return null;
            string filename = StatResultToFile(new string[] { bjsfiles[0], jsfiles[0] }, results, productIdentify, outFileIdentify, title, extInfos, 1, true, 1);
            return new FileExtractResult(outFileIdentify, filename);
        }

        public IExtractResult CHAZStatRasterXJ<T>(string productName, string productIdentify, string instanceName, string instanceAOI, string aoiTemplate, string outFileIdentify, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker) where T : struct, IConvertible
        {
            return CHAZStatRasterXJ<T>(productName, productIdentify, instanceName, instanceAOI, aoiTemplate, outFileIdentify, filters, progressTracker, false, 1);
        }

        public IExtractResult CHAZStatRasterXJ<T>(string productName, string productIdentify, string instanceName, string instanceAOI, string aoiTemplate, string outFileIdentify, Dictionary<string, Func<T, bool>> filters, Action<int, string> progressTracker, bool weight, float weightZoom) where T : struct, IConvertible
        {
            object aioObj = _argumentProvider.GetArg("AOI");
            string title = GetStringArgument("title");
            string[] bjsfiles = GetStringArray("mainfiles");
            string[] jsfiles = GetStringArray("jianshu");
            string extInfos = GetStringArgument("extinfo");
            string statname = GetStringArgument("statname");
            Dictionary<string, SortedDictionary<string, double>> bjsdic, jsdic, dic = null;
            if (bjsfiles == null || bjsfiles.Length == 0 || jsfiles == null || jsfiles.Length == 0)
                return null;
            bjsdic = RasterStatFactory.Stat(bjsfiles[0], aoiTemplate, filters, progressTracker, weight, weightZoom);
            if (bjsdic == null || bjsdic.Count == 0)
                return null;
            jsdic = RasterStatFactory.Stat(jsfiles[0], aoiTemplate, filters, progressTracker, weight, weightZoom);
            if (jsdic == null || jsdic.Count == 0)
                return null;
            //bool isident = true;
            //foreach (string key in bjsdic.Keys)
            //{
            //    if (!jsdic.ContainsKey(key))
            //    {
            //        isident = false;
            //        break;
            //    }
            //}
            //if (!isident)
            //    return null;
            ////<矢量面名称，<分级字段，统计值>>
            //foreach (string key in jsdic.Keys)
            //{
            //    if (!bjsdic.ContainsKey(key))
            //    {
            //        isident = false;
            //        break;
            //    }
            //}
            //if (!isident)
            //    return null;
            dic = new Dictionary<string, SortedDictionary<string, double>>();
            SortedDictionary<string, double> dicSortedDic = new SortedDictionary<string, double>();
            double chazvalue = 0;
            foreach (string key in bjsdic.Keys)
            {
                foreach (string keyvalue in bjsdic[key].Keys)
                {
                    chazvalue = bjsdic[key][keyvalue] - jsdic[key][keyvalue];
                    if (!dicSortedDic.ContainsKey(keyvalue))
                        dicSortedDic.Add(keyvalue, chazvalue);
                }
                if (!dic.ContainsKey(key))
                    dic.Add(key, dicSortedDic);
                dicSortedDic.Clear();
            }
            if (dic.Count <= 0)
                return null;

            if (string.IsNullOrEmpty(statname))
                title = productName + instanceName;
            else
                title = statname + instanceName;
            SortedDictionary<string, string> keyNmaeDic = GetRasterStatDictionary(aoiTemplate);
            string subTitle = GetCHAZSubTitle(bjsfiles[0], jsfiles[0]);
            IStatResult results = SSXDicToStatResult(dic, filters.Keys.ToArray(), subTitle, instanceAOI, keyNmaeDic);
            if (results == null)
                return null;
            string filename = StatResultToFile(bjsfiles, results, productIdentify, outFileIdentify, title, extInfos, 1, false, 0);
            return new FileExtractResult(outFileIdentify, filename);
            throw new NotImplementedException();
        }

        private static string GetCHAZSubTitle(string bjsfile, string jsfile)
        {
            string subTitle = "统计日期：" + DateTime.Now.ToShortDateString();
            string orbitTimes = string.Empty;
            RasterIdentify rasterId = new RasterIdentify(bjsfile);
            orbitTimes += rasterId.OrbitDateTime.ToShortDateString() + "减去";
            rasterId = new RasterIdentify(jsfile);
            orbitTimes += rasterId.OrbitDateTime.ToShortDateString() + " ";
            return subTitle += "\n" + "轨道时间：" + orbitTimes;
        }

        #endregion

    }
}
