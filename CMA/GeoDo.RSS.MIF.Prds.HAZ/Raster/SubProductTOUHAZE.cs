using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;
using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.UI.AddIn.TouMImportSmart;
using GeoDo.RSS.UI.AddIn.Theme;
using System.Runtime.InteropServices;
using GeoDo.RSS.DF.MEM;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.MIF.Prds.HAZ
{
    public class SubProductTOUHAZE : CmaMonitoringSubProduct
    {
        private Action<int, string> _progressTracker = null;
        private IContextMessage _contextMessage = null;

        public SubProductTOUHAZE(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _progressTracker = progressTracker;
            _contextMessage = contextMessage;
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "HAZEAlgorithm")
            {
                return HAZEAlgorithm();
            }
            return null;
        }

        private IExtractResult HAZEAlgorithm()
        {
            float lonMin = float.Parse(_argumentProvider.GetArg("LonMin").ToString());
            float lonMax = float.Parse(_argumentProvider.GetArg("LonMax").ToString());
            float latMin = float.Parse(_argumentProvider.GetArg("LatMin").ToString());
            float latMax = float.Parse(_argumentProvider.GetArg("LatMax").ToString());
            float invaild = float.Parse(_argumentProvider.GetArg("Invaild").ToString());
            float zoom = (float)_argumentProvider.GetArg("Zoom");
            float touResolution = (float)_argumentProvider.GetArg("TouResolution");
            int width = (int)_argumentProvider.GetArg("Width");
            int height = (int)_argumentProvider.GetArg("Height");
            string touFile = Obj2String(_argumentProvider.GetArg("TOUTxt"));
            bool IsComputerMid = (bool)_argumentProvider.GetArg("IsComputerMid");
            bool IsBilinear = (bool)_argumentProvider.GetArg("IsBilinear");
            float BilinearRes = float.Parse(_argumentProvider.GetArg("BilinearRes").ToString());
            if (string.IsNullOrEmpty(touFile) || !File.Exists(touFile))
                return null;
            Dictionary<string, string> dic = Obj2Dic(_argumentProvider.GetArg("OutEnvelopeSetting"));
            float outlonMin = float.Parse(dic["outlonMin"]);
            float outlonMax = float.Parse(dic["outlonMax"]);
            float outlatMin = float.Parse(dic["outlatMin"]);
            float outlatMax = float.Parse(dic["outlatMax"]);

            UpdateOutEnvelope(ref outlonMin, ref outlonMax, ref outlatMin, ref outlatMax, touResolution);
            IMonitoringSession ms = null;

            #region 中国区域裁切

            bool isChina = (bool)_argumentProvider.GetArg("isChina");
            string outFilename = MifEnvironment.GetFullFileName(Path.GetFileName(touFile));
            string chinaMask = string.Empty;
            if (isChina)
                chinaMask = AppDomain.CurrentDomain.BaseDirectory + "\\SystemData\\ProductArgs\\FOG\\TOUChinaMask\\china_mask.txt";
            TouProcessor.ProcessTouFile(touFile, ref outFilename, chinaMask);
            touFile = outFilename;
            if (string.IsNullOrEmpty(touFile) || !File.Exists(touFile))
                return null;

            #endregion

            #region 数据格式转换

            FY3TouImportSMART import = new FY3TouImportSMART(new Size(width, height), new CoordEnvelope(lonMin, lonMax, latMin, latMax), zoom, invaild, _progressTracker);
            string error;

            ISmartSession session = null;
            object obj = _argumentProvider.GetArg("SmartSession");
            if (obj != null)
                session = obj as ISmartSession;
            bool isBackGround = _argumentProvider.DataProvider != null && session != null && session.SmartWindowManager.ActiveCanvasViewer != null ? true : false;
            float rasterResulotion = !isBackGround ? touResolution : _argumentProvider.DataProvider.ResolutionX;
            rasterResulotion = IsBilinear ? (BilinearRes == -1 ? rasterResulotion : BilinearRes) : touResolution;
            RasterIdentify rid = new RasterIdentify(!isBackGround ? touFile : _argumentProvider.DataProvider.fileName);
            //中间计算用临时文件，最终结果保存为dstFilename
            string dstFilename = GetDstFilename(new RasterIdentify(touFile).OrbitDateTime, rid.Satellite, rid.Sensor, rasterResulotion);
            string tempFilename = dstFilename.Insert(dstFilename.LastIndexOf('.'), "temp");
            if (_progressTracker != null)
                _progressTracker.Invoke(10, "正在转换指数数据到栅格数据....");
            if (!import.ConvertTextToDat(touFile, tempFilename, new CoordEnvelope(outlonMin, outlonMax, outlatMin, outlatMax), out error))
            {
                if (_contextMessage != null && !string.IsNullOrEmpty(error))
                    _contextMessage.PrintMessage(_error);
                return null;
            }

            #endregion

            else
            {
                #region 输出结果插值处理

                //插值处理的放大 倍数
                Int16 intervalZoom = (Int16)Math.Floor(touResolution / rasterResulotion);
                string tempMidFilename = tempFilename.Insert(tempFilename.LastIndexOf('.'), "_mid");
                try
                {
                    GeoDo.RSS.MIF.Prds.Comm.Raster.BiliNearAndSmoothHelper bilinear = new Comm.Raster.BiliNearAndSmoothHelper();
                    //中值滤波
                    if (IsComputerMid)
                    {
                        if (_progressTracker != null)
                            _progressTracker.Invoke(30, "正在进行中值滤波....");
                        bilinear.SmoothComputer(tempFilename, 5, tempMidFilename);
                    }
                    else
                        tempMidFilename = tempFilename;
                    //插值
                    if (IsBilinear)
                    {
                        bilinear.AndBiliNear(tempMidFilename, intervalZoom, dstFilename);
                        if (_progressTracker != null)
                            _progressTracker.Invoke(60, "正在进行插值处理....");
                    }
                    else
                    {
                        if (File.Exists(dstFilename))
                            File.Delete(dstFilename);
                        File.Copy(tempMidFilename, dstFilename);
                    }

                #endregion

                    #region 专题产品生产

                    if (_progressTracker != null)
                        _progressTracker.Invoke(80, "正在生成专题产品....");

                    ms = _argumentProvider.EnvironmentVarProvider as IMonitoringSession;
                    ms.ChangeActiveSubProduct("0IMG");
                    ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("IsBackGround", isBackGround);
                    bool isOriginal = (bool)_argumentProvider.GetArg("isOriginal");
                    if (isOriginal)
                        ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "OHAI");
                    else
                        ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "HAEI");
                    ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("SelectedPrimaryFiles", new string[] { dstFilename });
                    ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("isSpecifyFiles", true);
                    ms.DoAutoExtract(false);

                    #endregion
                }
                finally
                {
                    //删除临时文件
                    if (File.Exists(tempFilename))
                        DelteAboutFile(tempFilename);
                    if (File.Exists(tempMidFilename))
                        DelteAboutFile(tempMidFilename);
                }
            }
            if (File.Exists(dstFilename))
            {
                DisplayResultClass.TrySaveFileToWorkspace(ms.ActiveMonitoringSubProduct, ms, dstFilename, new FileExtractResult("HAZE", dstFilename));
                WriteAboutFile(dstFilename);
            }
            return null;
        }

        private void WriteAboutFile(string dstFilename)
        {
            //写出dat文件的相关信息
            IRasterDataProvider rd = GeoDataDriver.Open(dstFilename) as IRasterDataProvider;
            try
            {
                if (rd != null)
                {
                    MemoryRasterDataProvider mrd = rd as MemoryRasterDataProvider;
                    if (mrd == null)
                        return;
                    HdrFile hdr = mrd.ToHdrFile();
                    if (hdr != null)
                    {
                        string hdrfile = Path.Combine(Path.GetDirectoryName(dstFilename), Path.GetFileNameWithoutExtension(dstFilename) + ".hdr");
                        HdrFile.SaveTo(hdrfile, hdr);
                    }
                }
            }
            finally
            {
                if (rd != null)
                    rd.Dispose();
            }
        }

        private void UpdateOutEnvelope(ref float outlonMin, ref float outlonMax, ref float outlatMin, ref float outlatMax, float touResolution)
        {
            float offset = touResolution / 2;
            outlonMin += offset;
            outlonMax += offset;
        }

        private Dictionary<string, string> Obj2Dic(object dicObj)
        {
            if (dicObj == null || string.IsNullOrEmpty(dicObj.ToString()))
                return null;
            return dicObj as Dictionary<string, string>;
        }

        private void DelteAboutFile(string Filename)
        {
            string[] aboutFiles = Directory.GetFiles(Path.GetDirectoryName(Filename), "*" + Path.GetFileNameWithoutExtension(Filename) + "*", SearchOption.TopDirectoryOnly);
            if (aboutFiles != null && aboutFiles.Length != 0)
            {
                foreach (string file in aboutFiles)
                    File.Delete(file);
            }
        }

        private string GetDstFilename(DateTime orbitTime, string satellite, string sensor, float resoultion)
        {
            RasterIdentify id = CreatRasterIndetifyId(orbitTime, satellite, sensor, resoultion);
            return id.ToWksFullFileName(".dat");
        }

        private RasterIdentify CreatRasterIndetifyId(DateTime orbitTime, string satellite, string sensor, float resoultion)
        {
            RasterIdentify id = new RasterIdentify();
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "HAZ";
            id.SubProductIdentify = "HAZE";
            id.Satellite = satellite;
            id.Sensor = sensor;
            float dstResolution = resoultion * 100000;
            if (dstResolution > 9999)
                id.Resolution = (int)(dstResolution / 1000) + "KM";
            else
                id.Resolution = dstResolution + "M";
            id.OrbitDateTime = orbitTime;
            id.GenerateDateTime = DateTime.Now;
            return id;
        }

        private string Obj2String(object argument)
        {
            if (argument == null)
                return null;
            return argument.ToString();
        }
    }
}
