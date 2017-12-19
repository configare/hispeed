using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing.Imaging;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.SNW
{
    public class SubProductSNWDegree : CmaMonitoringSubProduct
    {
        private int _dayCount = -1;

        public SubProductSNWDegree(SubProductDef productDef)
            : base(productDef)
        {
            _identify = productDef.Identify;
            _isBinary = false;
            _algorithmDefs = new List<AlgorithmDef>(productDef.Algorithms);
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            string algname = _argumentProvider.GetArg("AlgorithmName").ToString();
            string[] statFileNames = GetStringArray("SelectedPrimaryFiles");
            string extinfo = GetStringArgument("extinfo");
            if (string.IsNullOrEmpty(algname) || statFileNames == null || statFileNames.Count() == 0)
                return null;
            foreach (string fileName in statFileNames)
                if (!File.Exists(fileName))
                    return null;
            if (algname == "0SDC")
            {
                RasterIdentify rid = new RasterIdentify(statFileNames[0]);
                if (rid != null)
                {
                    _dayCount = GetDayCount(rid.OrbitDateTime.Month, rid.OrbitDateTime.Year);
                }
                else
                    _dayCount = 30;
                string degreeFileName = GetSNWDegree(statFileNames);
                object aioObj = _argumentProvider.GetArg("AOI");
                string title = null;
                StatResultItem[] result = DegreeStat(degreeFileName, aioObj, ref title);
                if (File.Exists(degreeFileName))
                    File.Delete(degreeFileName);
                FileExtractResult fileResult = null;
                string filename = StatResultToFile(statFileNames, result, "SNW", outFileIdentify, title, extinfo);
                fileResult = new FileExtractResult("0SDC", filename);
                return fileResult;
            }
            return null;
        }

        private int GetDayCount(int month, int year)
        {
            switch (month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    return 31;
                case 2: return GetDaysOfMonth(year);
                case 4:
                case 6:
                case 9:
                case 11: return 30;
                default: return 0;
            }
        }

        private int GetDaysOfMonth(int year)
        {
            if (DateTime.IsLeapYear(year))
                return 29;
            else
                return 28;
        }

        private string GetSNWDegree(string[] files)
        {
            double pixelArea = 0;
            IInterestedRaster<Int16> timeResult = null;
            IRasterOperator<Int16> roper = new RasterOperator<Int16>();
            DataIdentify di = GetDataIdentify();
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            string extinfo = GetStringArgument("extinfo");
            timeResult = roper.Times(files, CreatRasterIndetifyId(files, "SNW", outFileIdentify, di, null, extinfo),
                (dstValue, srcValue) =>
                {
                    if (srcValue == 0)
                        return dstValue;
                    else
                        return ++dstValue;
                });
            IRasterDataProvider dataProvider = timeResult.HostDataProvider;
            if (dataProvider != null)
            {
                ArgumentProvider ap = new ArgumentProvider(dataProvider, null);
                RasterPixelsVisitor<Int16> visitor = new RasterPixelsVisitor<Int16>(ap);
                IPixelFeatureMapper<double> memresult = new MemPixelFeatureMapper<double>("0SDC", 1000, new Size(dataProvider.Width, dataProvider.Height), dataProvider.CoordEnvelope, dataProvider.SpatialRef);
                IInterestedRaster<double> iir = null;
                try
                {
                    visitor.VisitPixel(new int[] { 1 }, (index, values) =>
                    {
                        if (values[0] == 0)
                            memresult.Put(index, 0);
                        else
                        {
                            pixelArea = RasterOperator<Int16>.ComputePixelArea(index / dataProvider.Width, dataProvider.CoordEnvelope.MaxY, dataProvider.ResolutionY);
                            memresult.Put(index, pixelArea * values[0]);
                        }
                    });
                    RasterIdentify id = new RasterIdentify();
                    id.ThemeIdentify = "CMA";
                    id.ProductIdentify = "SNW";
                    id.SubProductIdentify = "0SDC";
                    id.GenerateDateTime = DateTime.Now;
                    iir = new InterestedRaster<double>(id, new Size(dataProvider.Width, dataProvider.Height), dataProvider.CoordEnvelope.Clone());
                    iir.Put(memresult);
                    return iir.FileName;
                }
                finally
                {
                    if (iir != null)
                        iir.Dispose();
                    if (visitor != null)
                        visitor.Dispose();
                    if (timeResult != null)
                        timeResult.Dispose();
                    if (File.Exists(timeResult.FileName))
                        File.Delete(timeResult.FileName);
                }
            }
            return null;
        }

        private StatResultItem[] DegreeStat(string fname, object aoiObj, ref string title)
        {
            if (string.IsNullOrEmpty(fname))
                return null;
            if (aoiObj == null)
            {
                title = "积雪程度指数" + "按当前区域统计";
                return DegreeStatByType(fname, "当前区域", title);
            }
            else
            {
                if (aoiObj as Dictionary<string, int[]> != null)
                {
                    Dictionary<string, int[]> aoi = aoiObj as Dictionary<string, int[]>;
                    title = "积雪" + "自定义程度指数统计";
                    return AreaStatCustom(fname, title, aoi);
                }
                else if (!string.IsNullOrEmpty(aoiObj.ToString()))
                {
                    title = "积雪" + "按" + aoiObj.ToString() + "程度指数统计";
                    return DegreeStatByType(fname, aoiObj.ToString(), title);
                }
            }
            return null;
        }

        private StatResultItem[] DegreeStatByType(string fname, string statType, string title)
        {
            if (string.IsNullOrEmpty(title))
                title = "积雪按" + statType + "程度指数统计";
            if (string.IsNullOrEmpty(fname))
                return null;
            StatResultItem[] items = null;
            using (IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider)
            {
                if (statType == "当前区域")
                    items = DegreeStatCurrentRegion(prd, title);
                else
                    items = StatArea(prd, statType);
                return items;
            }
        }

        private StatResultItem[] DegreeStatCurrentRegion(IRasterDataProvider prd, string title)
        {
            double totalArea = 0;
            using (IArgumentProvider argPrd = new ArgumentProvider(prd, null))
            {
                using (IRasterPixelsVisitor<double> visitor = new RasterPixelsVisitor<double>(argPrd))
                {
                    visitor.VisitPixel(new int[] { 1 },
                        (index, values) =>
                        {
                            totalArea += values[0];
                        });
                    StatResultItem sri = new StatResultItem();
                    sri.Name = "当前区域";
                    sri.Value = (totalArea / _dayCount);
                    return new StatResultItem[] { sri };
                }
            }
        }

        public StatResultItem[] StatArea(IRasterDataProvider raster, string templateName)
        {
            StatResultItem[] resultItems = null;
            switch (templateName)
            {
                case "分级行政区划":
                    resultItems = ApplyStatByRasterTemplate(raster, "China_XjRaster");
                    break;
                case "省级行政区划":
                    resultItems = ResultsWithoutZero(ApplyStatByVectorTemplate(raster, "省级行政区域_面.shp", "NAME"));
                    break;
                case "土地利用类型":
                    resultItems = ResultsWithoutZero(ApplyStatByVectorTemplate(raster, "土地利用类型_合并.shp", "NAME"));
                    break;
            }
            if (resultItems == null || resultItems.Length == 0)
                return null;
            return resultItems;
        }

        private StatResultItem[] ApplyStatByVectorTemplate(IRasterDataProvider raster, string shpTemplateName, string primaryFieldName)
        {
            string shpFullname = VectorAOITemplate.FindVectorFullname(shpTemplateName);
            if (String.IsNullOrEmpty(shpFullname))
                return null;
            CodeCell.AgileMap.Core.Feature[] features = null;
            //step2:读取矢量
            try
            {
                features = GetFeatures(shpFullname);
                if (features == null || features.Length == 0)
                    return null;
                //step3:矢量栅格化
                Dictionary<string, Color> nameColors = new Dictionary<string, Color>();
                using (Bitmap bitmap = VectorsToBitmap(raster, features, primaryFieldName, out nameColors))
                {
                    int[] aoi;
                    Color color;
                    double totalArea;
                    string name;
                    List<StatResultItem> items = new List<StatResultItem>();
                    foreach (Feature fea in features)
                    {
                        name = fea.GetFieldValue(primaryFieldName);
                        if (String.IsNullOrEmpty(name))
                            continue;
                        color = nameColors[name];
                        aoi = GetAOIByFeature(bitmap, color);
                        if (aoi == null)
                            totalArea = 0;
                        else
                            totalArea = GetTotalArea(raster, aoi);
                        StatResultItem it = new StatResultItem();
                        it.Name = name;
                        it.Code = fea.OID.ToString();
                        it.Value = totalArea / _dayCount;
                        items.Add(it);
                    }
                    return items != null ? items.ToArray() : null;

                }
            }
            finally
            {
                if (features != null && features.Length > 0)
                {
                    foreach (Feature item in features)
                        item.Dispose();
                }
            }
        }

        private StatResultItem[] ApplyStatByRasterTemplate(IRasterDataProvider raster, string templateName)
        {
            switch (templateName)
            {
                case "China_XjRaster":
                    return ResultsWithoutZero(CountByXJRaster(raster));
                case "China_LandRaster":
                    return ResultsWithoutZero(CountByLandRaster(raster));
            }
            return null;
        }

        private StatResultItem[] CountByXJRaster(IRasterDataProvider raster)
        {
            using (IRasterDictionaryTemplate<int> temp = RasterDictionaryTemplateFactory.CreateXjRasterTemplate())
            {
                Dictionary<int, string> paris = temp.CodeNameParis;
                if (paris == null)
                    return null;
                List<StatResultItem> items = new List<StatResultItem>();
                int[] aoi;
                double totalArea = 0;
                foreach (string value in paris.Values)
                {
                    aoi = temp.GetAOI(value, raster.CoordEnvelope.MinX, raster.CoordEnvelope.MaxX, raster.CoordEnvelope.MinY, raster.CoordEnvelope.MaxY, new Size(raster.Width, raster.Height));
                    if (aoi == null)
                        continue;
                    totalArea = GetTotalArea(raster, aoi);
                    if (totalArea == 0)
                        continue;
                    StatResultItem it = new StatResultItem();
                    it.Name = value;
                    it.Value = totalArea / _dayCount;
                    items.Add(it);
                }
                return items != null ? items.ToArray() : null;
            }
        }

        private StatResultItem[] CountByLandRaster(IRasterDataProvider raster)
        {
            using (IRasterDictionaryTemplate<byte> temp = RasterDictionaryTemplateFactory.CreateLandRasterTemplate())
            {
                Dictionary<byte, string> paris = temp.CodeNameParis;
                if (paris == null)
                    return null;
                List<StatResultItem> items = new List<StatResultItem>();
                int[] aoi;
                double totalArea;
                foreach (string value in paris.Values)
                {
                    aoi = temp.GetAOI(value, raster.CoordEnvelope.MinX, raster.CoordEnvelope.MaxX, raster.CoordEnvelope.MinY, raster.CoordEnvelope.MaxY, new Size(raster.Width, raster.Height));
                    if (aoi == null)
                        continue;
                    totalArea = GetTotalArea(raster, aoi);
                    if (totalArea == 0)
                        continue;
                    StatResultItem it = new StatResultItem();
                    it.Name = value;
                    it.Value = totalArea / _dayCount;
                    items.Add(it);
                }
                return items != null ? items.ToArray() : null;
            }
        }

        private StatResultItem[] AreaStatCustom(string fname, string title, Dictionary<string, int[]> aoi)
        {
            if (string.IsNullOrEmpty(title))
                title = "积雪程度指数统计";
            if (string.IsNullOrEmpty(fname))
                return null;
            StatResultItem[] items = null;
            using (IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider)
            {
                if (aoi == null || aoi.Count == 0)
                    return null;
                items = StatAreaCustom(prd, aoi);
                return items;
            }
        }

        private StatResultItem[] StatAreaCustom(IRasterDataProvider prd, Dictionary<string, int[]> aoi)
        {
            StatResultItem[] results = null;
            results = ResultsWithoutZero(ApplyStatByVectorFile(prd, aoi));
            if (results == null || results.Length == 0)
                return null;
            List<StatResultItem> primaryItems = new List<StatResultItem>();
            StatResultItem item = null;
            foreach (string name in aoi.Keys)
            {
                item = MatchItemInResults(name, results);
                if (item != null)
                    primaryItems.Add(item);
            }
            if (primaryItems.Count == 0)
                return null;
            return primaryItems.ToArray();
        }

        public Feature[] GetFeatures(string fname)
        {
            if (fname == null)
                return null;
            IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(fname) as IVectorFeatureDataReader;
            if (dr == null)
                return null;
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

        private double GetTotalArea(IRasterDataProvider raster, int[] aoi)
        {
            IArgumentProvider argPrd = new ArgumentProvider(raster, null);
            argPrd.AOI = aoi;
            using (IRasterPixelsVisitor<double> visitor = new RasterPixelsVisitor<double>(argPrd))
            {
                double totalArea = 0;
                visitor.VisitPixel(new int[] { 1 },
                    (idx, values) =>
                    {
                        if (values[0] >= 0)
                            totalArea += values[0];
                    });
                return totalArea;
            }
        }

        private StatResultItem[] ApplyStatByVectorFile(IRasterDataProvider prd, Dictionary<string, int[]> aoi)
        {
            if (aoi == null || aoi.Count == 0)
                return null;
            double totalArea = 0;
            List<StatResultItem> items = new List<StatResultItem>();
            foreach (string fea in aoi.Keys)
            {
                if (aoi[fea] == null)
                    totalArea = 0;
                else
                    totalArea = GetTotalArea(prd, aoi[fea]);
                StatResultItem it = new StatResultItem();
                it.Name = fea;
                it.Value = totalArea;
                items.Add(it);
            }
            return items != null ? items.ToArray() : null;
        }

        public Bitmap VectorsToBitmap(IRasterDataProvider prd, CodeCell.AgileMap.Core.Feature[] features, string shpPrimaryField, out Dictionary<string, Color> nameColors)
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

        private Envelope GetEnvelop(IRasterDataProvider prd)
        {
            Size size = new System.Drawing.Size();
            size.Width = prd.Width;
            size.Height = prd.Height;
            return new Envelope(prd.CoordEnvelope.MinX, prd.CoordEnvelope.MinY, prd.CoordEnvelope.MaxX, prd.CoordEnvelope.MaxY);
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

        public int[] GetAOIByFeature(Bitmap bitmap, Color color)
        {
            using (IBitmap2RasterConverter c = new Bitmap2RasterConverter())
            {
                return c.ToRaster(bitmap, color);
            }
        }

        private StatResultItem MatchItemInResults(string name, StatResultItem[] results)
        {
            StatResultItem result = null;
            foreach (StatResultItem item in results)
            {
                if (name == item.Name)
                    return item;
            }
            result = new StatResultItem();
            result.Name = name;
            result.Value = 0;
            return result;
        }

        private StatResultItem[] ResultsWithoutZero(StatResultItem[] items)
        {
            if (items == null || items.Length == 0)
                return null;
            List<StatResultItem> results = new List<StatResultItem>();
            foreach (StatResultItem item in items)
            {
                if (item.Value != 0)
                    results.Add(item);
            }
            if (results == null || results.Count == 0)
                return null;
            return results.ToArray();
        }

    }
}