using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public class ElevationCorrections
    {
        public static bool DoElevationCorrections(TVDIUCArgs ucArgs, ref string error)
        {
            if (string.IsNullOrEmpty(ucArgs.LSTFile) || string.IsNullOrEmpty(ucArgs.DEMFile) || ucArgs.TVDIParas == null || ucArgs.TVDIParas.LstFile == null)
            {
                error = "陆表高温高程订正所需数据或参数设置不全.";
                return false;
            }

            Dictionary<string, FilePrdMap> filePrdMap = new Dictionary<string, FilePrdMap>();
            filePrdMap.Add("LSTFile", new FilePrdMap(ucArgs.LSTFile, ucArgs.TVDIParas.LstFile.Zoom, new VaildPra(ucArgs.TVDIParas.LstFile.Min, ucArgs.TVDIParas.LstFile.Max), new int[] { ucArgs.TVDIParas.LstFile.Band }));
            filePrdMap.Add("DemFile", new FilePrdMap(ucArgs.DEMFile, 1, new VaildPra(float.MinValue, float.MaxValue), new int[] { 1 }));

            ITryCreateVirtualPrd tryVPrd = new TryCreateVirtualPrdByMultiFile();
            IVirtualRasterDataProvider vrd = null;

            IInterestedRaster<float> iir = null;
            try
            {
                vrd = tryVPrd.CreateVirtualRasterPRD(ref filePrdMap);
                if (vrd == null)
                    throw new Exception("数据间无相交部分,无法创建虚拟数据提供者!");

                ArgumentProvider ap = new ArgumentProvider(vrd, null);
                RasterPixelsVisitor<float> rpVisitor = new RasterPixelsVisitor<float>(ap);
                IPixelFeatureMapper<float> _result = new MemPixelFeatureMapper<float>("0LEC", 1000, new Size(vrd.Width, vrd.Height), vrd.CoordEnvelope, vrd.SpatialRef);
                ArgumentItem ai = ucArgs.TVDIParas.LstFile;
                rpVisitor.VisitPixel(new int[] { filePrdMap["LSTFile"].StartBand,
                                                 filePrdMap["DemFile"].StartBand},
                    (index, values) =>
                    {
                        if (values[1] == -9999)
                        {
                            _result.Put(index, 9999);//海洋
                        }
                        else if (values[1] == -9000)
                        {
                            _result.Put(index, 9000);//非中国区域陆地
                        }
                        else if (values[1] >= 6000)
                        {
                            _result.Put(index, 0); //6000之内的LST数据
                        }
                        else if (values[0] == ucArgs.TVDIParas.LstFile.Cloudy)
                        {
                            _result.Put(index, 9998); //云区
                        }
                        else if (values[0] == 12)
                        {
                            _result.Put(index, 9997);//无数据区域
                        }
                        else if (values[0] == 0)
                            _result.Put(index, 0);
                        else if (values[1] == 0)
                            _result.Put(index, 0);
                        else
                        {
                            _result.Put(index, (float)(Math.Round((values[0] - 273f + 0.006f * values[1]) * ai.Zoom, 0)));
                        }
                    });
                iir = new InterestedRaster<float>(CreateRID(ucArgs.LSTFile), new Size(vrd.Width, vrd.Height), vrd.CoordEnvelope, vrd.SpatialRef);
                iir.Put(_result);
                ucArgs.ECLstFile = iir.FileName;
                return true;
            }
            finally
            {
                if (iir != null)
                    iir.Dispose();
                vrd.Dispose();
            }
        }

        private static RasterIdentify CreateRID(string lstFile)
        {
            RasterIdentify rid = new RasterIdentify(lstFile);
            rid.ThemeIdentify = "CMA";
            rid.SubProductIdentify = "0LEC";
            rid.ProductIdentify = "DRT";
            return rid;
        }
    }
}
