using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;
using CodeCell.AgileMap.Core;
using System.Drawing.Imaging;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public class SubProductRasterFldFlood : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        //泛滥缩小水体
        public SubProductRasterFldFlood(SubProductDef subProductDef)
            : base(subProductDef)
        {
            _identify = subProductDef.Identify;
            _name = subProductDef.Name;
            if (subProductDef.Algorithms != null)
            {
                _algorithmDefs = subProductDef.Algorithms.ToList();
            }
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker,IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "FLOD")
            {
                if (_argumentProvider.GetArg("mainfiles") == null)
                    return null;
                string binWater = _argumentProvider.GetArg("mainfiles").ToString();
                if (_argumentProvider.GetArg("backWaterFile") == null)
                    return null;
                string backWaterPath = _argumentProvider.GetArg("backWaterFile").ToString();
                if (!File.Exists(binWater) || !File.Exists(backWaterPath))
                    return null;
                if (string.IsNullOrEmpty(backWaterPath)||string.Equals(binWater,backWaterPath))
                    return null;
                //背景水体可能为DAT/MVG/SHP格式文件
                switch (Path.GetExtension(backWaterPath).ToUpper())
                {
                    case ".DAT":
                        return CompareDATFile(backWaterPath,binWater);
                    case ".SHP":
                        return CompareSHPFile(backWaterPath,binWater);
                    case ".MVG":
                        return null;
                }       
            }
            return null;
        }

        private IExtractResult CompareSHPFile(string backWaterPath, string binWater)
        {
            string shpPrimaryField=null;
            Feature[] features = GetFeatures(backWaterPath,out shpPrimaryField);
            if (features == null || features.Length == 0)
                return null;
            //矢量栅格化
            Dictionary<string, Color> nameColors = new Dictionary<string, Color>();
            IRasterDataProvider dataPrd = GeoDataDriver.Open(binWater) as IRasterDataProvider;
            IPixelFeatureMapper<Int16> result = new MemPixelFeatureMapper<Int16>("FLOD", 1000, new Size(dataPrd.Width, dataPrd.Height), dataPrd.CoordEnvelope, dataPrd.SpatialRef);
            using (Bitmap bitmap = VectorsToBitmap(dataPrd, features, shpPrimaryField, out nameColors))
            {
                int[] aoi;
                Color color;
                string name;
                List<int> items = new List<int>();
                foreach (Feature fea in features)
                {
                    name = fea.GetFieldValue(shpPrimaryField);
                    if (String.IsNullOrEmpty(name))
                        continue;
                    color = nameColors[name];
                    aoi = GetAOIByFeature(bitmap, color);
                    if (aoi != null)
                        items.AddRange(aoi); 
                }
                //分情况处理
                //无aoi区域
                if (items.Count == 0)
                {
                    ArgumentProvider ap = new ArgumentProvider(dataPrd, null);
                    RasterPixelsVisitor<Int16> rpVisitor = new RasterPixelsVisitor<Int16>(ap);
                    rpVisitor.VisitPixel(new int[] {1},
                    (idx, values) =>
                    {
                        if (values[0] == 1)
                            result.Put(idx, 1);
                    });
                }
                //有aoi区域
                else
                {
                    ArgumentProvider ap = new ArgumentProvider(dataPrd, null);
                    RasterPixelsVisitor<Int16> rpVisitor = new RasterPixelsVisitor<Int16>(ap);
                    rpVisitor.VisitPixel(new int[] { 1 },
                    (idx, values) =>
                    {
                        if (values[0] == 1)
                            result.Put(idx, 4);
                    });
                    Size size = new Size(dataPrd.Width, dataPrd.Height);
                    Rectangle aoiRect = AOIHelper.ComputeAOIRect(items.ToArray(), size);
                    rpVisitor.VisitPixel(aoiRect,items.ToArray(),new int[] { 1 },
                    (idx, values) =>
                    {
                        if (values[0] == 1)
                        {
                            result.Put(idx, 1);
                        }
                        else if(values[0]==0)
                        {
                            result.Put(idx, 5);
                        }
                    });
                }
            }
            return result;
        }

        private IExtractResult CompareDATFile(string backWaterPath,string binWater)
        {
            Dictionary<string, FilePrdMap> filePrdMap = new Dictionary<string, FilePrdMap>();
            filePrdMap.Add("backWaterPath", new FilePrdMap(backWaterPath, 1, new VaildPra(Int16.MinValue, Int16.MaxValue), new int[] { 1 }));
            filePrdMap.Add("binWater", new FilePrdMap(binWater, 1, new VaildPra(Int16.MinValue, Int16.MaxValue), new int[] { 1 }));
            ITryCreateVirtualPrd tryVPrd = new TryCreateVirtualPrdByMultiFile();
            IVirtualRasterDataProvider vrd = tryVPrd.CreateVirtualRasterPRD(ref filePrdMap);
            if (vrd == null)
            {
                PrintInfo("数据间无相交部分,无法进行泛滥缩小水体计算!");
                if (filePrdMap != null && filePrdMap.Count > 0)
                {
                    foreach (FilePrdMap value in filePrdMap.Values)
                    {
                        if (value.Prd != null)
                            value.Prd.Dispose();
                    }
                }
                return null;
            }
            try
            {
                ArgumentProvider ap = new ArgumentProvider(vrd, null);
                RasterPixelsVisitor<float> rpVisitor = new RasterPixelsVisitor<float>(ap);
                IPixelFeatureMapper<Int16> result = new MemPixelFeatureMapper<Int16>("FLOD", 1000, new Size(vrd.Width, vrd.Height), vrd.CoordEnvelope, vrd.SpatialRef);
                rpVisitor.VisitPixel(new int[] { filePrdMap["backWaterPath"].StartBand,
                                                 filePrdMap["binWater"].StartBand},
                    (idx, values) =>
                    {
                        if (values[0] == 1 && values[1] == 1)
                            result.Put(idx, 1);
                        else if (values[0] == 1 && values[1] == 0)
                            result.Put(idx, 5);
                        else if (values[0] == 0 && values[1] == 1)
                            result.Put(idx, 4);
                    });
                RasterIdentify rid = new RasterIdentify(new string[]{backWaterPath, binWater});
                rid.ProductIdentify = _subProductDef.ProductDef.Identify;
                rid.SubProductIdentify = _identify;
                rid.IsOutput2WorkspaceDir = true;
                IInterestedRaster<Int16> iir = new InterestedRaster<Int16>(rid, result.Size, result.CoordEnvelope, result.SpatialRef);
                iir.Put(result);
                iir.Dispose();
                return new FileExtractResult("扩大缩小水体", iir.FileName);
            }
            finally
            {
                vrd.Dispose();
            }
        }

        private Feature[] GetFeatures(string fname, out string primaryField)
        {
            primaryField = string.Empty;
            if (fname == null)
                return null;
            IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(fname) as IVectorFeatureDataReader;
            if (dr == null)
                return null;
            primaryField = dr.Fields[0];
            try
            {
                Feature[] fets = dr.FetchFeatures();
                if (fets == null || fets.Length == 0)
                    return null;
                List<Feature> features = new List<Feature>();
                foreach (Feature fet in fets)
                {
                    if (fet.Geometry is ShapePolygon)
                        features.Add(fet);
                }
                return features != null ? features.ToArray() : null;
            }
            finally
            {
                dr.Dispose();
            }
        }

        private Bitmap VectorsToBitmap(IRasterDataProvider prd, CodeCell.AgileMap.Core.Feature[] features, string shpPrimaryField, out Dictionary<string, Color> nameColors)
        {
            using (IVector2BitmapConverter c = new Vector2BitmapConverter())
            {
                Dictionary<ShapePolygon, Color> vectors = GetVectorColors(features, shpPrimaryField, out nameColors);
                Bitmap bmp = new Bitmap(prd.Width, prd.Height, PixelFormat.Format24bppRgb);
                Envelope envelop = GetEnvelop(prd);
                c.ToBitmap(vectors, Color.Black, envelop, new Size(prd.Width, prd.Height), ref bmp);
                return bmp;
            }
        }

        private Dictionary<ShapePolygon, Color> GetVectorColors(Feature[] features, string shpPrimaryField, out Dictionary<string, Color> nameColors)
        {
            Dictionary<ShapePolygon, Color> vectorColors = new Dictionary<ShapePolygon, Color>();
            nameColors = new Dictionary<string, Color>();
            int count = features.Count();
            Random random = new Random(1);
            Color color;
            for (int i = 0; i < count; i++)
            {
                color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
                if (features[i].GetFieldValue(shpPrimaryField) == null)
                    continue;
                if (!nameColors.Keys.Contains(features[i].GetFieldValue(shpPrimaryField))
                  && !String.IsNullOrEmpty(features[i].GetFieldValue(shpPrimaryField)))
                    nameColors.Add(features[i].GetFieldValue(shpPrimaryField), color);
                if (!vectorColors.Keys.Contains(features[i].Geometry as ShapePolygon))
                    vectorColors.Add(features[i].Geometry as ShapePolygon, color);
            }
            return vectorColors;
        }

        private Envelope GetEnvelop(IRasterDataProvider prd)
        {
            Size size = new System.Drawing.Size();
            size.Width = prd.Width;
            size.Height = prd.Height;
            return new Envelope(prd.CoordEnvelope.MinX, prd.CoordEnvelope.MinY, prd.CoordEnvelope.MaxX, prd.CoordEnvelope.MaxY);
        }

        private int[] GetAOIByFeature(Bitmap bitmap, Color color)
        {
            using (IBitmap2RasterConverter c = new Bitmap2RasterConverter())
            {
                return c.ToRaster(bitmap, color);
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
