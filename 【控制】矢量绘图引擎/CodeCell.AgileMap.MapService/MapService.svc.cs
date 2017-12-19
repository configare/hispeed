using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Drawing;
using CodeCell.AgileMap.Core;
using System.Drawing.Imaging;
using CodeCell.Bricks.Runtime;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;

namespace CodeCell.AgileMap.MapService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class MapService : IMapService
    {
        private IMapImageGenerator _gen = null;
        private string _outputdir = null;
        private string _outputUrlBase = null;
        private ITileSystemHelper _tileSystemHelper = null;
        private int _mintuesOfHostCache = 5;
        private string _outputImageExtname = ".jpg";
        private ImageFormat _outputImageFormat = ImageFormat.Jpeg;

        public MapService()
        {
            try
            {
                RectangleF fullRect = new RectangleF(-20037510f, -19993420, 40075020f, 39986840f);
                _tileSystemHelper = new TileSystemHelper(fullRect, new Size(256, 256), 20);
                //
                _gen = new MapImageGeneratorDefault();
                try
                {
                    _mintuesOfHostCache = int.Parse(System.Configuration.ConfigurationManager.AppSettings["MintuesOfHostCache"]);
                    _outputImageExtname = System.Configuration.ConfigurationManager.AppSettings["OutputImageFormat"];
                    switch (_outputImageExtname.ToUpper())
                    {
                        case ".JPG":
                            _outputImageFormat = ImageFormat.Jpeg;
                            break;
                        case ".PNG":
                            _outputImageFormat = ImageFormat.Png;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log.WriterException(ex);
                }
                _outputdir = System.Configuration.ConfigurationManager.AppSettings["MapImageOutputDir"];
                _outputUrlBase = System.Configuration.ConfigurationManager.AppSettings["MapImageUrlBaseDir"];
                string mcd = System.Configuration.ConfigurationManager.AppSettings["MapConfigFile"];
                IMap map = MapFactory.LoadMapFrom(mcd);
                _gen.ApplyMap(map);
            }
            catch (Exception ex)
            {
                Log.WriterException(ex);
            }
        }

        public MapInfo GetMapInfo()
        {
            string spatialRef = _gen.SpatialReference != null ? _gen.SpatialReference.ToWKTString() : string.Empty;
            return new MapInfo(_gen.Map.Name, spatialRef);
        }

        public LayerInfo[] GetLayerInfos()
        {
            ILayerContainer c = _gen.Map.LayerContainer;
            if (c.Layers == null || c.Layers.Length == 0)
                return null;
            List<LayerInfo> infos = new List<LayerInfo>();
            foreach (IFeatureLayer lyr in c.Layers)
            {
                IFeatureClass fetclass = lyr.Class as IFeatureClass;
                LayerInfo info = new LayerInfo(lyr.Name, lyr.Id, fetclass.ShapeType);
                infos.Add(info);
            }
            return infos.ToArray();
        }

        public FeatureInfo[] Query(string layerId, string geometry, string keywords)
        {
            return new QueryExecutor(_gen).Query(layerId, geometry, keywords);
        }

        public string GeoEnvelope2PrjEnvelope(double minX, double maxX, double minY, double maxY)
        {
            RectangleF rect = _gen.GeoEnvelope2Viewport(new Envelope(minX, minY, maxX, maxY));
            return rect.X.ToString() + "," +
                rect.Y.ToString() + "," +
                rect.Width.ToString() + "," +
                rect.Height.ToString();
        }

        public PointF[] Geo2Prj(PointF[] geoPts)
        {
            _gen.ProjectionTransform.Transform(geoPts);
            return geoPts;
        }

        public PointF[] Prj2Geo(PointF[] prjPts)
        {
            _gen.ProjectionTransform.InverTransform(prjPts);
            return prjPts;
        }

        //public static int ReqTimes = 0;
        public MapImage GetMapImage(double x,
            double y,
            double width,
            double height,
            int targetWidth,
            int targetHeight, string[] insvisibleLayerIds)
        {
            //int btime = Environment.TickCount;
            try
            {
                //ReqTimes++;
                //Log.WriterWarning(ReqTimes.ToString());                   
                return GetMapImageInternal(null, x, y, width, height, targetWidth, targetHeight, insvisibleLayerIds);

            }
            finally
            {
                //Log.WriterWarning((Environment.TickCount - btime).ToString());
                WaitCallback cb = new WaitCallback(ClearCache);
                ThreadPool.QueueUserWorkItem(cb);
            }
        }

        private void ClearCache(object sender)
        {
            string[] files = System.IO.Directory.GetFiles(_outputdir, "*" + _outputImageExtname);
            if (files == null || files.Length == 0)
                return;
            for (int i = files.Length - 1; i >= 0; i--)
            {
                try
                {
                    string f = files[i];
                    FileInfo finfo = new FileInfo(f);
                    if (DateTime.Now - finfo.CreationTime > TimeSpan.FromMinutes(_mintuesOfHostCache))
                        File.Delete(f);
                }
                catch (Exception ex)
                {
                    Log.WriterException(ex);
                }
            }
        }

        private object lockObj = new object();
        public MapImage GetMapImageByQuadkey(string quadkey,
            double x,
            double y,
            double width,
            double height,
            int targetWidth,
            int targetHeight)
        {

            lock (lockObj)
            {

                try
                {
                    string fname = System.IO.Path.Combine(_outputdir, quadkey + _outputImageExtname);
                    if (System.IO.File.Exists(fname))
                        return new MapImage(new RectangleF((float)x, (float)y, (float)width, (float)height), _outputUrlBase + quadkey + ".png");
                    else
                    {
                        return GetMapImageInternal(fname, x, y, width, height, targetWidth, targetHeight, null);
                    }
                }
                catch (Exception ex)
                {
                    Log.WriterException(ex);
                    return null;
                }
            }
        }

        private MapImage GetMapImageInternal(string fname, double x,
           double y,
           double width,
           double height,
           int targetWidth,
           int targetHeight, string[] invisibleLayerIds)
        {
            try
            {
                Image img = null;
                RectangleF rect = RectangleF.Empty;
                string retfname = null;
                GetMapImageInternal(fname, x, y, width, height, targetWidth, targetHeight, invisibleLayerIds, out rect, out img, out retfname);
                if (img != null && retfname != null)
                {
                    img.Save(retfname, _outputImageFormat);
                    return new MapImage(rect, _outputUrlBase + System.IO.Path.GetFileName(retfname));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.WriterException(ex);
                return null;
            }
        }

        private void GetMapImageInternal(string fname, double x,
           double y,
           double width,
           double height,
           int targetWidth,
           int targetHeight,
           string[] invisibleLayerIds,
           out RectangleF retRectangleF, out Image retImage, out string retFilename)
        {
            try
            {
                /*
                 * 转换投影坐标的最小值为-Max 
                 */
                y = -(y + height);
                if (string.IsNullOrEmpty(fname))
                    fname = System.IO.Path.Combine(_outputdir, Guid.NewGuid().ToString() + _outputImageExtname);
                RectangleF vf = new RectangleF((float)x, (float)y, (float)width, (float)height);
                Size size = new Size(targetWidth, targetHeight);
                retImage = new Bitmap(size.Width, size.Height);
                List<ILayerDrawable> disLayers = TrySetInvisableOfLayers(invisibleLayerIds);
                try
                {
                    retRectangleF = _gen.GetMapImage(vf, size, ref retImage);
                    float y1 = -retRectangleF.Y;
                    float y2 = -(retRectangleF.Y + retRectangleF.Height);
                    retRectangleF = new RectangleF((float)retRectangleF.X, Math.Min(y1, y2), retRectangleF.Width, retRectangleF.Height);
                    retFilename = fname;
                }
                finally
                {
                    TryResetVisible(disLayers);
                }
            }
            catch (Exception ex)
            {
                retRectangleF = RectangleF.Empty;
                retImage = null;
                retFilename = null;
                Log.WriterException(ex);
            }
        }

        private void TryResetVisible(List<ILayerDrawable> disLayers)
        {
            if (disLayers != null && disLayers.Count > 0)
                foreach (ILayerDrawable lyr in disLayers)
                    lyr.Visible = true;
        }

        private List<ILayerDrawable> TrySetInvisableOfLayers(string[] invisibleLayerIds)
        {
            if (invisibleLayerIds == null || invisibleLayerIds.Length == 0)
                return null;
            List<ILayerDrawable> invisableLayers = new List<ILayerDrawable>();
            foreach (string lyrId in invisibleLayerIds)
            {
                IFeatureLayer lyr = _gen.Map.LayerContainer.GetLayerById(lyrId) as IFeatureLayer;
                if (lyr != null)
                {
                    (lyr as ILayerDrawable).Visible = false;
                    invisableLayers.Add(lyr as ILayerDrawable);
                }
            }
            return invisableLayers;
        }

        public FeatureInfo[] Identify(string[] layerIds, PointF prjPt, double tolerance)
        {
            try
            {
                IFeatureLayer[] layers = GetLayerByIds(layerIds);
                if (layers == null || layers.Length == 0)
                    return null;
                List<Feature> retFets = new List<Feature>();
                foreach (IFeatureLayer lyr in layers)
                {
                    IFeatureClass fetclass = lyr.Class as IFeatureClass;
                    if (fetclass.ShapeType == enumShapeType.Polyline)
                        continue;
                    Feature[] fets = lyr.Identify(new ShapePoint(prjPt.X, prjPt.Y), tolerance);
                    if (fets != null)
                        retFets.AddRange(fets);
                }
                return QueryExecutor.FeaturesToFeatureInfo(retFets.ToArray());
            }
            catch (Exception ex)
            {
                Log.WriterException(ex);
                return null;
            }
        }

        private IFeatureLayer[] GetLayerByIds(string[] layerIds)
        {
            if (layerIds == null)
                return _gen.Map.LayerContainer.FeatureLayers;
            else if (layerIds.Length == 0)
                return null;
            List<IFeatureLayer> layers = new List<IFeatureLayer>();
            foreach (string id in layerIds)
            {
                IFeatureLayer lyr = _gen.Map.LayerContainer.GetLayerById(id) as IFeatureLayer;
                if (lyr != null)
                    layers.Add(lyr);
            }
            return layers.ToArray();
        }
    }

}
