#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-2-20 9:32:59
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Drawing;
using GeoDo.RSS.BlockOper;
using CodeCell.AgileMap.Core;
using System.Windows.Forms;
using GeoDo.RSS.DF.MEM;
using GeoDo.RSS.Core.UI;
namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 类名：ClipSNWParaData
    /// 属性描述：
    /// 创建者：Li Xj   创建日期：2014-2-20 9:32:59
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
     public class ClipSNWParaData
    {
        Envelope evp = new Envelope();
        private CoordEnvelope _fileEnvelope = null;
        private Size _fileSize = Size.Empty;
        private double _resolutionX = 0;
        private double _resolutionY = 0;
        private CoordEnvelope _outsizeRegion = null;
        //private bool canUpateGeoRangeBySize = true;
        private Feature _vectorFeature;
        private Project.ISpatialReference _activeSpatialRef;
        private enumCoordType _activeCoordType;
        private string _blockName = "DXX";
        private string OutDir;
        private string inputFileName;
        private BlockDefWithAOI[] envelopes;
        private int[] aoiIndex = null;
        //private List<BlockDefWithAOI> blockList = new List<BlockDefWithAOI>();
        private string url = null;
        public IFileExtractResult ClipSNWResult(string filename, string area)
        {
            inputFileName = filename;
            using (IRasterDataProvider raster = GeoDataDriver.Open(filename) as IRasterDataProvider)
            {
                _resolutionX = raster.ResolutionX;
                _resolutionY = raster.ResolutionY;
                _fileSize = new Size(raster.Width, raster.Height);
                _fileEnvelope = raster.CoordEnvelope;
                _activeSpatialRef = raster.SpatialRef;
                _activeCoordType = raster.CoordType;
                raster.Dispose();
            }
            _outsizeRegion = new CoordEnvelope(_fileEnvelope.MinX, _fileEnvelope.MaxX, _fileEnvelope.MinY, _fileEnvelope.MaxY);
           OutDir = Path.GetDirectoryName(filename);
           
           string hdrfile = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".hdr");
            List<Feature> fets = new List<Feature>();
            IVectorFeatureDataReader dr = null;
            if(area == "中国区")
                url = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "数据引用\\基础矢量\\行政区划\\面\\中国边界.shp"); 
            else
                url = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "数据引用\\基础矢量\\行政区划\\面\\省级行政区域_面.shp");
            dr = VectorDataReaderFactory.GetUniversalDataReader(url) as IVectorFeatureDataReader;
            Feature[] temp = null;
            temp = dr.FetchFeatures();
            fets.AddRange(temp);
            Feature[] features = null;
            features = fets.ToArray();
            _blockName = area;
            int index = GetFeatureIndx(area);
            _vectorFeature = features[index];//_selectFeature
            //string fieldValue = _vectorFeature.GetFieldValue(fieldIndex);
            evp = GetMaskEnvelope(_vectorFeature.Geometry as ShapePolygon);
            BlockDefWithAOI outEnvelope;
            Size size;
            List<BlockDefWithAOI> blockList = new List<BlockDefWithAOI>();
            GetArgs(out outEnvelope, out size, out OutDir,out filename);
            envelopes = new BlockDefWithAOI[] { outEnvelope };
            aoiIndex = GetFeatureAOIIndex();
            blockList.AddRange(envelopes);
            blockList[0].AOIIndexes = aoiIndex;
            string[] put = RasterClipT(inputFileName,blockList.ToArray(),OutDir,"Cut");
            //put[0] = OutDir + put[0];
            string getpath = Path.GetDirectoryName(put[0]);
            IFileExtractResult res = new FileExtractResult("MWS", put[0], true);
            res.SetDispaly(false);
            return res;

        }
         /// <summary>
         /// 类似查找表，由省区名确定是省级行政区划shp文件的第几个Feature. 共有34个Feature。
         /// </summary>
         /// <param name="urlShp"></param>
         /// <param name="areaName"></param>
         /// <returns></returns>
        private int GetFeatureIndx(string areaName)
        {
            int i =0;
            if (areaName == "北京市")
                i = 1;
            if (areaName == "天津市")
                i = 2;
            if (areaName == "河北省")
                i = 3;
            if (areaName == "山西省")
                i = 4;
            if (areaName == "内蒙古自治区")
                i = 5;
            if (areaName == "辽宁省")
                i = 6;
            if (areaName == "吉林省")
                i = 7;
            if (areaName == "黑龙江省")
                i = 8;
            if (areaName == "上海市")
                i = 9;
            if (areaName == "江苏省")
                i = 10;
            if (areaName == "浙江省")
                i = 11;
            if (areaName == "安徽省")
                i = 12;
            if (areaName == "福建省")
                i = 13;
            if (areaName == "江西省")
                i = 14;
            if (areaName == "山东省")
                i = 15;
            if (areaName == "河南省")
                i = 16;
            if (areaName == "湖北省")
                i = 17;
            if (areaName == "湖南省")
                i = 18;
            if (areaName == "广东省")
                i = 19;
            if (areaName == "广西壮族自治区")
                i = 20;
            if (areaName == "海南省")
                i = 21;
            if (areaName == "重庆市")
                i = 22;
            if (areaName == "四川省")
                i = 23;
            if (areaName == "贵州省")
                i = 24;
            if (areaName == "云南省")
                i = 25;
            if (areaName == "西藏自治区")
                i = 26;
            if (areaName == "陕西省")
                i = 27;
            if (areaName == "甘肃省")
                i = 28;
            if (areaName == "青海省")
                i = 29;
            if (areaName == "宁夏回族自治区")
                i = 30;
            if (areaName == "新疆唯吾尔自治区")
                i = 31;
            if (areaName == "台湾省")
                i = 32;
            if (areaName == "香港特别行政区")
                i = 33;
            return i;
        }
        private int[] GetFeatureAOIIndex()
        {
            if (_vectorFeature == null || _outsizeRegion == null)
                return null;
            VectorAOIGenerator vg = new VectorAOIGenerator();
            int[] aoi = null;
            aoi = vg.GetAOI(new ShapePolygon[] { _vectorFeature.Geometry as ShapePolygon },
                            new Envelope(_fileEnvelope.MinX, _fileEnvelope.MinY, _fileEnvelope.MaxX, _fileEnvelope.MaxY),
                            _fileSize);
            return aoi;
        }
        private void GetArgs(out BlockDefWithAOI envelope, out Size size, out string dir, out string inputFilename)
        {
            envelope = new BlockDefWithAOI(_blockName, evp.MinX, evp.MinY, evp.MaxX, evp.MaxY);
            Size aoiSize = new Size((int)GetInteger(((evp.MaxX - evp.MinX)/_resolutionX),_resolutionX),
                 (int)GetInteger(((evp.MaxY-evp.MinY)/_resolutionY),_resolutionY));
            size = new Size((int)aoiSize.Width, aoiSize.Height);
            dir = OutDir;
            inputFilename = inputFileName;
        }
        protected int GetInteger(double fWidth, double res)
        {
            int v = (int)Math.Round(fWidth);
            if (fWidth - v > res)
                v++;
            return v;
        }
        private Envelope GetMaskEnvelope(ShapePolygon shapePolygon)
        {
            if (shapePolygon == null)
                return null;
            Envelope envelope = shapePolygon.Envelope;
            if (_activeCoordType == enumCoordType.GeoCoord)
                return envelope;
            else if (_activeCoordType == enumCoordType.PrjCoord)
            {
                GeoDo.Project.ISpatialReference srcProj = GeoDo.Project.SpatialReference.GetDefault();
                GeoDo.Project.ISpatialReference dstProj = _activeSpatialRef;
                GeoDo.Project.IProjectionTransform prj = GeoDo.Project.ProjectionTransformFactory.GetProjectionTransform(srcProj, dstProj);
                double[] xs = new double[] { shapePolygon.Rings[0].Points[0].X };
                double[] ys = new double[] { shapePolygon.Rings[0].Points[0].Y };
                prj.Transform(xs, ys);
                double minx = xs[0];
                double maxx = xs[0];
                double miny = ys[0];
                double maxy = ys[0];
                ShapePoint pt;
                foreach (ShapeRing ring in shapePolygon.Rings)
                {
                    for (int pti = 0; pti < ring.Points.Length; pti++)
                    {
                        pt = ring.Points[pti];
                        xs = new double[] { pt.X };
                        ys = new double[] { pt.Y };
                        prj.Transform(xs, ys);
                        minx = Math.Min(xs[0], minx);
                        maxx = Math.Max(xs[0], maxx);
                        miny = Math.Min(ys[0], miny);
                        maxy = Math.Max(ys[0], maxy);
                    }
                }
                Envelope corEnvelope = new Envelope(minx, miny, maxx, maxy);
                return corEnvelope;
            }
            else
                return null;
        }
        public string[] RasterClipT(string infName, BlockDefWithAOI[] blocks, string outDir, string type)
        {
            IRasterDataProvider prd = GeoDataDriver.Open(infName) as IRasterDataProvider;
            try
            {
                if (type.Equals("Cut"))
                {
                    RasterCutProcesser cut = new RasterCutProcesser();
                    IRasterDataProvider result = null;
                    if (prd is MemoryRasterDataProvider)
                        result = cut.Cut(prd, blocks[0], 100, "MEM", outDir,
                        new Action<int, string>((int progerss, string text) =>
                        {
                        }));
                    else
                        result = cut.Cut(prd, blocks[0], 100, "LDF", outDir,
                       new Action<int, string>((int progerss, string text) =>
                       {
                       }));
                    List<string> files = new List<string>();
                    if (result != null)
                    {
                        files.Add(result.fileName);
                        result.Dispose(); 
                    }
                    if (prd != null)
                        prd.Dispose();
                    return files.ToArray();
                }
                else
                    return null;
            }
            finally
            {
                
            }
        }
        public float[] GetEvelope(string filename, string area)
        {
            List<Feature> fets = new List<Feature>();
            IVectorFeatureDataReader dr = null;
            int index = 0;
            if (area == "中国区")
            {
                url = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "数据引用\\基础矢量\\行政区划\\面\\中国边界.shp");
                index = 0;
            }
            else
            {
                url = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "数据引用\\基础矢量\\行政区划\\面\\省级行政区域_面.shp");
                index = GetFeatureIndx(area);
            }
            dr = VectorDataReaderFactory.GetUniversalDataReader(url) as IVectorFeatureDataReader;
            Feature[] temp = null;
            temp = dr.FetchFeatures();
            fets.AddRange(temp);
            fets.ToArray();
            Feature[] features = null;
            features = fets.ToArray();
            _blockName = area;
            _vectorFeature = features[index];//_selectFeature
            //string fieldValue = _vectorFeature.GetFieldValue(fieldIndex);
            Envelope evp1 = new Envelope();
            evp1 = GetMaskEnvelopeHDF(_vectorFeature.Geometry as ShapePolygon);
            float[] envelope = new float[] { Convert.ToSingle(evp1.MinX), Convert.ToSingle(evp1.MaxX), Convert.ToSingle(evp1.MinY), Convert.ToSingle(evp1.MaxY)};
            return envelope;
        }
        private Envelope GetMaskEnvelopeHDF(ShapePolygon shapePolygon)
        {
            if (shapePolygon == null)
                return null;
            Envelope envelope = shapePolygon.Envelope;
            GeoDo.Project.ISpatialReference srcProj = GeoDo.Project.SpatialReference.GetDefault();
            GeoDo.Project.ISpatialReference dstProj = GeoDo.Project.SpatialReference.GetDefault();//_activeSpatialRef;
            GeoDo.Project.IProjectionTransform prj = GeoDo.Project.ProjectionTransformFactory.GetProjectionTransform(srcProj, dstProj);
            double[] xs = new double[] { shapePolygon.Rings[0].Points[0].X };
            double[] ys = new double[] { shapePolygon.Rings[0].Points[0].Y };
            prj.Transform(xs, ys);
            double minx = xs[0];
            double maxx = xs[0];
            double miny = ys[0];
            double maxy = ys[0];
            ShapePoint pt;
            foreach (ShapeRing ring in shapePolygon.Rings)
            {
                for (int pti = 0; pti < ring.Points.Length; pti++)
                {
                    pt = ring.Points[pti];
                    xs = new double[] { pt.X };
                    ys = new double[] { pt.Y };
                    prj.Transform(xs, ys);
                    minx = Math.Min(xs[0], minx);
                    maxx = Math.Max(xs[0], maxx);
                    miny = Math.Min(ys[0], miny);
                    maxy = Math.Max(ys[0], maxy);
                }
            }
            Envelope corEnvelope = new Envelope(minx, miny, maxx, maxy);
            return corEnvelope;

        }
    }
}

