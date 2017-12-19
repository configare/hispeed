using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.DST
{
    public class SubProductRasterDst : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductRasterDst(SubProductDef subProduct)
            : base(subProduct)
        {
            _name = subProduct.Name;
            _identify = subProduct.Identify;
            _isBinary = false;
            _algorithmDefs = new List<AlgorithmDef>();
            _algorithmDefs.AddRange(subProduct.Algorithms);
        }

        public override void Reset()
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;

            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            string algorith = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (algorith == "Visibility")
                return ComputeVisibility();
            PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
            return null;
        }

        private IExtractResult ComputeVisibility()
        {
            if (_argumentProvider.DataProvider == null)
            {
                PrintInfo("请提供沙尘影像数据！");
                return null;
            }
            IRasterDataProvider prd = _argumentProvider.DataProvider;
            int[] aois = GetAOIByBinaryFile();
            if (aois != null && aois.Length != 0)
                _argumentProvider.AOI = aois;
            int visiNo = -1, nearNo = -1, shortNo = -1, farNo = -1;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            visiNo = TryGetBandNo(bandNameRaster, "Visible");//(int)_argumentProvider.GetArg("Visible");
            nearNo = TryGetBandNo(bandNameRaster, "NearInfrared");//(int)_argumentProvider.GetArg("NearInfrared");
            shortNo = TryGetBandNo(bandNameRaster, "ShortInfrared");//(int)_argumentProvider.GetArg("ShortInfrared");
            farNo = TryGetBandNo(bandNameRaster, "FarInfrared"); //(int)_argumentProvider.GetArg("FarInfrared");
            double visiZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double nearZoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
            double shortZoom = (double)_argumentProvider.GetArg("ShortInfrared_Zoom");
            double farZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");

            if (visiNo == -1 || nearNo == -1 || shortNo == -1 || farNo == -1 || visiZoom == -1 || farZoom == -1 || shortZoom == -1)
            {
                PrintInfo("获取波段序号失败，可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            int bandCount = prd.BandCount;
            if (visiNo > bandCount)
            {
                PrintInfo("可见光波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return null;
            }
            if (nearNo > bandCount)
            {
                PrintInfo("近红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return null;
            }
            if (shortNo > bandCount)
            {
                PrintInfo("短波红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return null;
            }
            if (farNo > bandCount)
            {
                PrintInfo("远红外波段序号超过数据的波段范围，可能是影像数据选择不正确。");
                return null;
            }
            int[] bandNos = new int[] { visiNo, nearNo, shortNo, farNo };
            string express = "(UInt16)Math.Round(1000 * Math.Pow(Math.E,(var_VisibilityA + var_VisibilityB * band"
                            + visiNo + " / " + visiZoom + "f + var_VisibilityC * band" + nearNo + "/" + nearZoom + "f + var_VisibilityD * band"
                            + shortNo + "/" + shortZoom + "f + var_VisibilityE * band" + farNo + "/" + farZoom + "f + var_VisibilityF *(band"
                            + shortNo + "/" + shortZoom + "f - band" + farNo + "/" + farZoom + "f + var_ShortFarVar))),0)";
            IRasterExtracter<UInt16, UInt16> extracter = new SimpleRasterExtracter<UInt16, UInt16>();
            extracter.Reset(_argumentProvider, bandNos, express);
            IPixelFeatureMapper<UInt16> result = new MemPixelFeatureMapper<UInt16>("Visibility", 1000, new Size(prd.Width, prd.Height), prd.CoordEnvelope, prd.SpatialRef);
            extracter.Extract(result);
            return result;
        }

        private int[] GetAOIByBinaryFile()
        {
            string fname = _argumentProvider.GetArg("BinaryFile").ToString();
            if (String.IsNullOrEmpty(fname))
                return null;
            IRasterDataProvider prd = null;
            try
            {
                prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
                Dictionary<string, object> args = new Dictionary<string, object>();
                IArgumentProvider argPrd = new ArgumentProvider(prd, args);
                RasterPixelsVisitor<UInt16> raster = new RasterPixelsVisitor<UInt16>(argPrd);
                List<int> idxs = new List<int>();
                raster.VisitPixel(new int[] { 1 }, (index, value) =>
                {
                    if (value[0] != 0)
                        idxs.Add(index);
                });
                return idxs.ToArray();
            }
            catch
            {
                throw new ArgumentException("请选择正确的二值图文件！");
            }
            finally
            {
                if (prd != null)
                {
                    prd.Dispose();
                    prd = null;
                }
            }
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }
}
