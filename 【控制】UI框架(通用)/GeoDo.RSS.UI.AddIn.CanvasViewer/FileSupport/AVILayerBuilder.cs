using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;
using GeoDo.RSS.Core.RasterDrawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    public static class AVILayerBuilder
    {
        public static IAVILayer CreatAVILayer(string[] fnames, ICanvas canvas, int maxSize, out Size dataSize, 
                                              out GeoDo.RSS.Core.DrawEngine.CoordEnvelope viewPrjEvp,
                                              out GeoDo.RSS.Core.DF.CoordEnvelope viewGeoEvp,
                                              out float resolution)
        {
            Size bmpSize = Size.Empty;
            dataSize = Size.Empty;
            viewPrjEvp = null;
            viewGeoEvp = null;
            resolution = 0;
            BitmapObject[] bobjs = GetBitmapObjects(fnames, maxSize, canvas, out bmpSize, out dataSize, out viewPrjEvp,out viewGeoEvp, out resolution);
            if (bobjs == null || bobjs.Length == 0)
                return null;
            IAVILayer aviLyr = new AVILayer(bobjs, 500);
            aviLyr.IsRunning = true;
            return aviLyr;
        }

        private static BitmapObject[] GetBitmapObjects(string[] fnames, int maxSize, ICanvas canvas, out Size bmpSize,
                                                       out Size dataSize, out GeoDo.RSS.Core.DrawEngine.CoordEnvelope viewPrjEvp,
                                                       out GeoDo.RSS.Core.DF.CoordEnvelope viewGeoEvp,
                                                       out float resolution)
        {
            bmpSize = Size.Empty;
            dataSize = new Size();
            viewPrjEvp = null;
            viewGeoEvp = null;
            resolution = 0;
            List<BitmapObject> bmpObjs = new List<BitmapObject>();
            BitmapObject bmpO;
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope drawEvp = null;
            bool isFirst = true;
            foreach (string fname in fnames)
            {
                if (!File.Exists(fname))
                    continue;
                using (IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider)
                {
                    IOverviewGenerator ov = prd as IOverviewGenerator;
                    bmpSize = ov.ComputeSize(maxSize);
                    Bitmap bmp = new Bitmap(bmpSize.Width, bmpSize.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    RasterIdentify identify = new RasterIdentify(fname);
                    object[] sts = GetColorTableFunc(prd, identify.ProductIdentify, identify.SubProductIdentify);
                    if (sts != null)
                        ov.Generate(new int[] { 1, 1, 1 }, sts, ref bmp);
                    else
                    {
                        int[] bands = prd.GetDefaultBands();
                        if (bands == null || bands.Length == 0)
                            return null;
                        ov.Generate(bands, ref bmp);
                    }
                    drawEvp = GetDrawingEnvelop(canvas, prd);
                    if (isFirst)
                    {
                        viewPrjEvp = drawEvp;
                        ICoordinateTransform tans = canvas.CoordTransform;
                        viewGeoEvp = PrjToGeoCoordEvp(viewPrjEvp, tans);
                        resolution = prd.ResolutionX;
                        dataSize = GetMaxDataSize(canvas, viewPrjEvp, prd, out viewGeoEvp);
                        isFirst = false;
                    }
                    else
                    {
                        viewPrjEvp = viewPrjEvp.Union(drawEvp);
                        dataSize = GetMaxDataSize(canvas, viewPrjEvp, prd,out viewGeoEvp);
                        //如果分辩率不相等则取最大的分辨率
                        if (resolution < prd.ResolutionX)
                            resolution = prd.ResolutionX;
                    }
                    bmpO = new BitmapObject(bmp, drawEvp);
                    bmpObjs.Add(bmpO);
                }
            }
            return bmpObjs.Count != 0 ? bmpObjs.ToArray() : null;
        }

        private static object[] GetColorTableFunc(IRasterDataProvider prd, string productIdentify, string subIdentify)
        {
            if (string.IsNullOrEmpty(productIdentify) || string.IsNullOrEmpty(subIdentify))
                return null;
            ProductColorTable ct = ProductColorTableFactory.GetColorTable(productIdentify+subIdentify);
            Type dataType = DataTypeHelper.Enum2DataType(prd.DataType);
            return GetStretcher(dataType, ct);
        }

        private static object[] GetStretcher(Type dataType, ProductColorTable ct)
        {
            if (dataType.Equals(typeof(byte)))
                return ProductColorTableFactory.GetStretcher<byte>(ct);
            else if (dataType.Equals(typeof(Int16)))
                return ProductColorTableFactory.GetStretcher<Int16>(ct);
            else if (dataType.Equals(typeof(UInt16)))
                return ProductColorTableFactory.GetStretcher<UInt16>(ct);
            else if (dataType.Equals(typeof(Int32)))
                return ProductColorTableFactory.GetStretcher<Int32>(ct);
            else if (dataType.Equals(typeof(UInt32)))
                return ProductColorTableFactory.GetStretcher<UInt32>(ct);
            else if (dataType.Equals(typeof(Int64)))
                return ProductColorTableFactory.GetStretcher<Int64>(ct);
            else if (dataType.Equals(typeof(UInt64)))
                return ProductColorTableFactory.GetStretcher<UInt64>(ct);
            else if (dataType.Equals(typeof(float)))
                return ProductColorTableFactory.GetStretcher<float>(ct);
            else if (dataType.Equals(typeof(double)))
                return ProductColorTableFactory.GetStretcher<double>(ct);
            return null;
        }

        private static Core.DrawEngine.CoordEnvelope GetDrawingEnvelop(ICanvas canvas, IRasterDataProvider prd)
        {
            if (prd == null)
                return null;
            GeoDo.RSS.Core.DF.CoordEnvelope cop = prd.CoordEnvelope.Clone();
            ICoordinateTransform tans = canvas.CoordTransform;
            if (prd.CoordType == enumCoordType.PrjCoord)
                return new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(cop.MinX, cop.MaxX, cop.MinY, cop.MaxY);
            else
            {
                GeoDo.RSS.Core.DrawEngine.CoordEnvelope prjEvp = new Core.DrawEngine.CoordEnvelope(cop.MinX, cop.MaxX, cop.MinY, cop.MaxY);
                tans.Geo2Prj(prjEvp);
                return prjEvp;
            }
        }

        private static Size GetMaxDataSize(ICanvas canvas, Core.DrawEngine.CoordEnvelope viewPrjEvp, IRasterDataProvider prd,
                                           out GeoDo.RSS.Core.DF.CoordEnvelope viewGeoEvp)
        {
            viewGeoEvp = null;
            if (prd == null)
                return Size.Empty;
            viewGeoEvp = null;
            ICoordinateTransform tans = canvas.CoordTransform;
            viewGeoEvp = PrjToGeoCoordEvp(viewPrjEvp, tans);
            if (prd.CoordType == enumCoordType.PrjCoord)
                return new Size((int)(viewPrjEvp.Width / prd.ResolutionX), (int)(viewPrjEvp.Height / prd.ResolutionY));
            else
            {
                double wid = 0;
                double hei = 0;
                tans.Prj2Geo(viewPrjEvp.Width, viewPrjEvp.Height, out wid, out hei);
                return new Size((int)(wid / prd.ResolutionX), (int)(hei / prd.ResolutionY));
            }
        }

        private static GeoDo.RSS.Core.DF.CoordEnvelope PrjToGeoCoordEvp(Core.DrawEngine.CoordEnvelope viewPrjEvp, ICoordinateTransform tans)
        {
            double minX = 0;
            double minY = 0;
            double maxX = 0;
            double maxY = 0;
            tans.Prj2Geo(viewPrjEvp.MinX, viewPrjEvp.MinY, out minX, out minY);
            tans.Prj2Geo(viewPrjEvp.MaxX, viewPrjEvp.MaxY, out maxX, out maxY);
            return new GeoDo.RSS.Core.DF.CoordEnvelope(minX, maxX, minY, maxY);
        }
    }
}
