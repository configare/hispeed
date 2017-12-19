using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.RasterDrawing;
using System.Drawing;
using GeoDo.RSS.UI.AddIn.CanvasViewer;
using System.IO;
using System.Drawing.Imaging;
using GeoDo.RSS.RasterTools;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    class ResultObject
    {
        public IPixelIndexMapper BinaryValues;
        public IBinaryBitampLayer BinaryLayer;
        public ResultObject(IPixelIndexMapper binaryValues, IBinaryBitampLayer binaryLayer)
        {
            BinaryValues = binaryValues;
            BinaryLayer = binaryLayer;
        }
    }

    internal class ExtractingSession : IExtractingSession, ICursorInfoProvider
    {
        private IMonitoringProduct _currentProduct;
        private IMonitoringSubProduct _currentSubProduct;
        private ICanvasViewer _canvasViewer;
        private Dictionary<string, ResultObject> _resultObjects = new Dictionary<string, ResultObject>(1);
        private bool _isUpdated = false;
        private bool _isActived = false;
        //
        private string _featureCollectionName;
        private ICursorDisplayedFeatures _features;
        private Size _featuresSize;
        //
        private ISmartSession _session;

        public ExtractingSession(ISmartSession session)
        {
            _session = session;
        }

        public IMonitoringProduct CurrentProduct
        {
            get { return _currentProduct; }
        }

        public IMonitoringSubProduct CurrentSubProduct
        {
            get { return _currentSubProduct; }
        }

        public bool IsActive
        {
            get { return _isActived; }
        }

        public bool IsNeedSave()
        {
            return _isUpdated;
        }

        public void Saved()
        {
            _isUpdated = false;
        }

        public bool Start(ICanvasViewer viewer, IMonitoringProduct product, IMonitoringSubProduct subProduct)
        {
            _canvasViewer = viewer;
            if (_canvasViewer == null)
                return false;
            _canvasViewer.OnWindowClosed += new EventHandler((sender, e) => { Stop(); });
            _currentProduct = product;
            _currentSubProduct = subProduct;
            ResultObject resultObj = GetResultObject(viewer, product, subProduct);
            string name = GetName(product, subProduct);
            if (!_resultObjects.ContainsKey(name))
                _resultObjects.Add(name, resultObj);
            else
            {
                _resultObjects[name].BinaryLayer.Dispose();
                _resultObjects[name].BinaryValues.Dispose();
                _resultObjects[name] = resultObj;
            }
            AttachCanvasViewerMenuHandlers();
            _isActived = true;
            _isUpdated = false;
            return true;
        }

        private void AttachCanvasViewerMenuHandlers()
        {
            ICanvasViewerMenuHandlerManager mgr = _canvasViewer;
            mgr.Register(_currentProduct.Identify, enumCanvasViewerMenu.Erase.ToString(), (result, args) => { HandleErase(result); }, null);
            mgr.Register(_currentProduct.Identify, enumCanvasViewerMenu.Adsorb.ToString(), (result, args) => { HandleAdsorb(result); }, null);
            mgr.Register(_currentProduct.Identify, enumCanvasViewerMenu.RemoveAll.ToString(), (result, args) => { HandleRemoveAll(); }, null);
            mgr.Register(_currentProduct.Identify, enumCanvasViewerMenu.Flash.ToString(), (result, args) => { HandleFalsh(); }, null);
            mgr.Register(_currentProduct.Identify, enumCanvasViewerMenu.MagicWand.ToString(), (result, args) => { HandleMagicWand(result, args); }, "GeoDo.RSS.UI.AddIn.Theme.dll:GeoDo.RSS.UI.AddIn.Theme.UCMagicWandArg");
        }

        //闪烁判识结果
        private void HandleFalsh()
        {
            string name = GetName(_currentProduct, _currentSubProduct);
            if (string.IsNullOrWhiteSpace(name))
                return;
            IBinaryBitampLayer layer = _resultObjects[name].BinaryLayer;
            LayerFlashControler c = new LayerFlashControler(_canvasViewer.Canvas, layer);
            c.Flash();
        }

        //清除所有判识结果
        private void HandleRemoveAll()
        {
            string name = GetName(_currentProduct, _currentSubProduct);
            IPixelIndexMapper mapper = _resultObjects[name].BinaryValues;
            mapper.Reset();
            UpdateLayer(_resultObjects[name]);
        }

        //魔术棒
        private void HandleAdsorb(object result)
        {
            GeometryOfDrawed geometry = result as GeometryOfDrawed;
            IRasterDataProvider prd = GetRasterDataProvider(_canvasViewer);
            using (IVectorAOIGenerator gen = new VectorAOIGenerator())
            {
                int[] aoi = gen.GetAOI(geometry.RasterPoints.Clone() as PointF[], geometry.Types, new Size(prd.Width, prd.Height));
                string name = GetName(_currentProduct, _currentSubProduct);
                IPixelIndexMapper mapper = _resultObjects[name].BinaryValues;
                mapper.Put(aoi);
                UpdateLayer(_resultObjects[name]);
            }
        }

        //魔术棒判识
        private void HandleMagicWand(object result, Dictionary<string, object> args)
        {
            GeometryOfDrawed geometry = result as GeometryOfDrawed;
            IRasterDrawing drawing = GetRasterDrawing(_canvasViewer);
            if (drawing == null)
                return;
            double prjX = 0, prjY = 0;
            ICoordinateTransform tran = _canvasViewer.Canvas.CoordTransform;
            //如果鼠标没有点中图像有效区域则直接退出
            tran.Raster2Prj(geometry.RasterPoints[0].X, geometry.RasterPoints[0].Y, out prjX, out prjY);
            if (prjX < drawing.OriginalEnvelope.MinX ||
                prjX > drawing.OriginalEnvelope.MaxX ||
                prjY < drawing.OriginalEnvelope.MinY ||
                prjY > drawing.OriginalEnvelope.MaxY)
                return;
            //计算鼠标在当前视窗位图中的位置
            int x = (int)((prjX - drawing.Envelope.MinX) / (drawing.Envelope.Width / (float)drawing.Bitmap.Width));
            int y = (int)((drawing.Envelope.MaxY - prjY) / (drawing.Envelope.Height / (float)drawing.Bitmap.Height));
            using (IBitmapMagicWand magicWand = new BitmapMagicWand())
            {
                Bitmap crtBitmap = drawing.Bitmap;
                BitmapData pdata = crtBitmap.LockBits(new Rectangle(0, 0, crtBitmap.Width, crtBitmap.Height), ImageLockMode.ReadWrite, crtBitmap.PixelFormat);
                try
                {
                    byte tolerance = 32;
                    bool isContinued = true;
                    if (args != null && args.Count == 2)
                    {
                        tolerance = byte.Parse(args["tolerance"].ToString());
                        isContinued = bool.Parse(args["iscontinued"].ToString());
                    }
                    /*
                     * 魔术棒提取
                     * ScanLineSegment为像素坐标扫描线三元组(Row,BeginCol,EndCol)
                     */
                    ScanLineSegment[] segs = magicWand.ExtractSnappedPixels(pdata, new Point(x, y), tolerance, isContinued);
                    /*
                     * 将扫描线数组转换为判识结果
                     */
                    MagicWandResult2ExtractReulst(segs, drawing);
                }
                finally
                {
                    crtBitmap.UnlockBits(pdata);
                }
                _canvasViewer.Canvas.Refresh(enumRefreshType.All);
            }
        }

        private void MagicWandResult2ExtractReulst(ScanLineSegment[] segs, IRasterDrawing drawing)
        {
            //获取判识结果对象
            string name = GetName(_currentProduct, _currentSubProduct);
            IPixelIndexMapper mapper = _resultObjects[name].BinaryValues;
            //计算当前视窗分辨率和缩放比
            double resX = drawing.Envelope.Width / drawing.Bitmap.Width;
            double resY = drawing.Envelope.Height / drawing.Bitmap.Height;
            float scaleX = (float)(resX / drawing.OriginalResolutionX);
            float scaleY = (float)(resY / (drawing.OriginalEnvelope.Height / drawing.DataProvider.Height));
            //计算当前视窗位图和原始栅格坐标交集
            Core.DrawEngine.CoordEnvelope evp = drawing.Envelope.Intersect(drawing.OriginalEnvelope);
            /*
             * 计算当前视窗位图中有效影像区域
             */
            int bRow = (int)((drawing.Envelope.MaxY - evp.MaxY) / resY);
            int bCol = (int)((evp.MinX - drawing.Envelope.MinX) / resX);
            int eRow = (int)(bRow + evp.Height / resY);
            int eCol = (int)(bCol + evp.Width / resX);
            /*
             * 因为瓦片为中心对齐方式，因此有无效黑边
             * row0,col0为实际数据行相对于全栅格图像的偏移
             */
            int row0 = (int)((1 / scaleY) * (drawing.OriginalEnvelope.MaxY - evp.MaxY) / drawing.OriginalResolutionX);
            int col0 = (int)((1 / scaleX) * (evp.MinX - drawing.OriginalEnvelope.MinX) / drawing.OriginalResolutionX);
            /*
             * 将扫面线转换到原始图像区域
             */
            List<ScanLineSegment> retSegs = new List<ScanLineSegment>();
            for (int i = 0; i < segs.Length; i++)
            {
                //无效黑边行
                if (segs[i].Row < bRow || segs[i].Row >= eRow)
                    continue;
                segs[i].Row = segs[i].Row - bRow + row0;
                //Math.Max,Max.Min去除无效黑边列
                segs[i].BeginCol = Math.Max(segs[i].BeginCol, bCol) - bCol + col0;
                segs[i].EndCol = Math.Min(segs[i].EndCol, eCol) - bCol + col0;
                retSegs.Add(segs[i]);
            }
            if (retSegs.Count == 0)
                return;
            // by chennan 应用感兴趣区域
            int[] aoi = GetAOI();
            //将扫描线重采样为原始分辨率(1:1)并生成二值图
            IBinaryResampler resampler = new BinaryResampler();
            resampler.Resample(retSegs.ToArray(), scaleX, scaleY, mapper, aoi);
            //更新图层
            UpdateLayer(_resultObjects[name]);
        }

        private int[] GetAOI()
        {
            if (_canvasViewer == null || _canvasViewer.AOIProvider == null)
                return null;
            return _canvasViewer.AOIProvider.GetIndexes();
        }

        //橡皮檫
        private void HandleErase(object result)
        {
            if (_canvasViewer == null)
                return;
            GeometryOfDrawed geometry = result as GeometryOfDrawed;
            IRasterDataProvider prd = GetRasterDataProvider(_canvasViewer);
            using (IVectorAOIGenerator gen = new VectorAOIGenerator())
            {
                int[] aoi = gen.GetAOI(geometry.RasterPoints.Clone() as PointF[], geometry.Types, new Size(prd.Width, prd.Height));
                string name = GetName(_currentProduct, _currentSubProduct);
                IPixelIndexMapper mapper = _resultObjects[name].BinaryValues;
                mapper.Remove(aoi);
                UpdateLayer(_resultObjects[name]);
            }
        }

        private ResultObject GetResultObject(ICanvasViewer viewer, IMonitoringProduct product, IMonitoringSubProduct subProduct)
        {
            IRasterDataProvider prd = GetRasterDataProvider(viewer);
            IPixelIndexMapper binaryValues = GetBinaryValuesMapper(prd, product, subProduct);
            IBinaryBitampLayer binaryLayer = GetBinaryLayer(viewer, prd, product, subProduct);
            return new ResultObject(binaryValues, binaryLayer);
        }

        private IBinaryBitampLayer GetBinaryLayer(ICanvasViewer cv, IRasterDataProvider dataProvider, IMonitoringProduct product, IMonitoringSubProduct subProduct)
        {
            string name = GetName(product, subProduct);
            Size bmSize = new Size(dataProvider.Width, dataProvider.Height);
            Color productColor = GetBinaryColor(subProduct);
            using (IBinaryBitmapBuilder builder = new BinaryBitmapBuilder())
            {
                Bitmap bitmap = builder.CreateBinaryBitmap(bmSize, productColor, Color.Transparent);
                IBinaryBitampLayer layer = cv.Canvas.LayerContainer.GetByName(name) as IBinaryBitampLayer;
                if (layer != null)
                {
                    cv.Canvas.LayerContainer.Layers.Remove(layer);
                    layer.Dispose();
                }
                layer = new BinaryBitmapLayer(name, bitmap, GetCoordEnvelope(dataProvider), dataProvider.CoordType == enumCoordType.GeoCoord);
                cv.Canvas.LayerContainer.Layers.Add(layer);
                return layer;
            }
        }

        private Color GetBinaryColor(IMonitoringSubProduct subProduct)
        {
            if (subProduct == null)
                return Color.Red;
            //可使用productColorTableName为产品标识，进行产品判识结果颜色的定义 by chennan 20130930
            ProductColorTable colorTable = ProductColorTableFactory.GetColorTable(subProduct.Definition.ProductDef.Identify);
            if (colorTable != null)
                return colorTable.GetColor(1f).Color;
            return subProduct.Definition.Color;
        }

        private Core.DrawEngine.CoordEnvelope GetCoordEnvelope(IRasterDataProvider dataProvider)
        {
            Core.DrawEngine.CoordEnvelope evp = new Core.DrawEngine.CoordEnvelope(
                dataProvider.CoordEnvelope.MinX,
                dataProvider.CoordEnvelope.MaxX,
                dataProvider.CoordEnvelope.MinY,
                dataProvider.CoordEnvelope.MaxY);
            return evp;
        }

        private IPixelIndexMapper GetBinaryValuesMapper(IRasterDataProvider prd, IMonitoringProduct product, IMonitoringSubProduct subProduct)
        {
            string name = GetName(product, subProduct);
            return PixelIndexMapperFactory.CreatePixelIndexMapper(name, prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
        }

        public IPixelIndexMapper GetBinaryValuesMapper(string productIdentify, string subProductIdentify)
        {
            string name = productIdentify + "_" + subProductIdentify;
            if (_resultObjects.ContainsKey(name))
                return _resultObjects[name].BinaryValues;
            else
                return null;
        }

        private IRasterDrawing GetRasterDrawing(ICanvasViewer viewer)
        {
            if (viewer == null || viewer.ActiveObject == null)
                return null;
            return viewer.ActiveObject as IRasterDrawing;
        }

        private IRasterDataProvider GetRasterDataProvider(ICanvasViewer viewer)
        {
            if (viewer == null || viewer.ActiveObject == null)
                return null;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            return drawing.DataProvider;
        }

        private string GetName(IMonitoringProduct product, IMonitoringSubProduct subProduct)
        {
            if (product == null || subProduct == null)
                return string.Empty;
            return product.Identify + "_" + subProduct.Identify; ;
        }

        public void Stop()
        {
            TryUnregisterCursorInfoProvider();
            if (_canvasViewer == null)
                return;
            foreach (string name in _resultObjects.Keys)
            {
                _canvasViewer.Canvas.LayerContainer.Layers.RemoveAll((lyr) => { return lyr.Name == name; });
                _resultObjects[name].BinaryLayer.Dispose();
                _resultObjects[name].BinaryValues.Dispose();
            }
            _resultObjects.Clear();
            if (_canvasViewer != null && _currentProduct != null)
                _canvasViewer.UnRegister(_currentProduct.Identify);
            _currentProduct = null;
            _currentSubProduct = null;
            _isActived = false;
            _isUpdated = false;
            _canvasViewer = null;
        }

        public void ChangeSubProduct(IMonitoringSubProduct subProduct)
        {
            if (subProduct == null || _currentSubProduct.Identify == subProduct.Identify)
                return;
            _currentSubProduct = subProduct;
            string name = GetName(_currentProduct, subProduct);
            ResultObject resultObj = GetResultObject(_canvasViewer, _currentProduct, _currentSubProduct);
            _resultObjects.Add(name, resultObj);
        }

        public void ApplyResult(IPixelIndexMapper dltResult)
        {
            if (dltResult == null)
                return;
            TryExtCursorInfoForFeaturesDisplay(dltResult);
            //
            IArgumentProvider argprd = _currentSubProduct.ArgumentProvider;
            string name = GetName(_currentProduct, _currentSubProduct);
            IPixelIndexMapper result = _resultObjects[name].BinaryValues;
            if (argprd != null && argprd.AOI != null)
            {
                result.Remove(argprd.AOI);
                foreach (int idx in dltResult.Indexes)
                    result.Put(idx);
            }
            else
            {
                result.Reset();
                foreach (int idx in dltResult.Indexes)
                    result.Put(idx);
            }
            UpdateLayer(_resultObjects[name]);
            _isUpdated = true;
        }

        private void TryExtCursorInfoForFeaturesDisplay(IPixelIndexMapper dltResult)
        {
            //光标信息窗口没有显示
            if (!_session.SmartWindowManager.SmartToolWindowFactory.IsDisplayed(9000))
                return;
            _featuresSize = dltResult.Size;
            if (dltResult.Tag != null && dltResult.Tag is ICursorDisplayedFeatures)
            {
                ICursorInfoDisplayer dis = _session.SmartWindowManager.CursorInfoDisplayer;
                if (dis != null)
                    dis.RegisterProvider(this as ICursorInfoProvider);
                _featureCollectionName = (dltResult.Tag as ICursorDisplayedFeatures).Name;
                _features = dltResult.Tag as ICursorDisplayedFeatures;
            }
        }

        private void TryUnregisterCursorInfoProvider()
        {
            //光标信息窗口没有显示
            if (!_session.SmartWindowManager.SmartToolWindowFactory.IsDisplayed(9000))
                return;
            ICursorInfoDisplayer dis = _session.SmartWindowManager.CursorInfoDisplayer;
            dis.UnregisterProvider(this);
        }

        private void UpdateLayer(ResultObject resultObject)
        {
            IPixelIndexMapper binaryValues = resultObject.BinaryValues;
            IBinaryBitampLayer binaryLayer = resultObject.BinaryLayer;
            Bitmap bitmap = binaryLayer.Bitmap;
            using (IBinaryBitmapBuilder builder = new BinaryBitmapBuilder())
            {
                builder.Reset(bitmap.Size, ref bitmap);
                builder.Fill(binaryValues.Indexes.ToArray(), bitmap.Size, ref bitmap);
            }
            _canvasViewer.Canvas.Refresh(enumRefreshType.All);
        }

        public void ApplyErase(int[] aoi)
        {
            string name = GetName(_currentProduct, _currentSubProduct);
            IPixelIndexMapper result = _resultObjects[name].BinaryValues;
            result.Remove(aoi);
            UpdateLayer(_resultObjects[name]);
            _isUpdated = true;
        }

        public void ApplyAdsorb(int[] aoi)
        {
            string name = GetName(_currentProduct, _currentSubProduct);
            IPixelIndexMapper result = _resultObjects[name].BinaryValues;
            result.Put(aoi);
            UpdateLayer(_resultObjects[name]);
            _isUpdated = true;
        }

        public string AddToWorkspace(IWorkspace wks)
        {
            if (_currentProduct == null || _currentSubProduct == null || wks == null)
                return null;
            ICatalog c = wks.GetCatalog("CurrentExtracting");
            string name = GetName(_currentProduct, _currentSubProduct);
            IPixelIndexMapper result = _resultObjects[name].BinaryValues;
            IRasterDataProvider prd = GetRasterDataProvider(_canvasViewer);
            if (prd == null)
                return null;
            RasterIdentify rstIdentify = GetRasterIdentify(prd);
            string fname = InterestedRaster<Int16>.GetWorkspaceFileName(rstIdentify);
            if (File.Exists(fname))
            {
                //GeoDo.RSS.UI.AddIn.Theme.AutoGeneratorSettings.enumActionOfExisted action = GetActionOfFileIsExisted(fname);
                //if (action == AutoGeneratorSettings.enumActionOfExisted.Overide)
                //{
                try
                {
                    fname = WriteInterestedRaster(result, prd, rstIdentify, fname);
                }
                catch (Exception e)
                {
                    MsgBox.ShowError(e.Message);
                }
                //}
                //else if (action == AutoGeneratorSettings.enumActionOfExisted.ReName)
                //{
                //    fname = fname.Replace(".dat", DateTime.Now.ToString("_yyyyMMddHHmmss") + ".dat");
                //    using (IInterestedRaster<Int16> rst = new InterestedRaster<Int16>(fname,
                //        new Size(prd.Width, prd.Height), prd.CoordEnvelope.Clone()))
                //    {
                //        rst.Put(result.Indexes.ToArray(), 1);
                //    }
                //}
                //else if (action == AutoGeneratorSettings.enumActionOfExisted.Skip)
                //{
                //}
            }
            else
                fname = WriteInterestedRaster(result, prd, rstIdentify, fname);
            //
            c.AddItem(new CatalogItem(fname, wks.GetCatalogByIdentify(rstIdentify.SubProductIdentify).Definition as SubProductCatalogDef));
            return fname;
        }

        private GeoDo.RSS.UI.AddIn.Theme.AutoGeneratorSettings.enumActionOfExisted GetActionOfFileIsExisted(string fname)
        {
            if (AutoGeneratorSettings.CurrentSettings == null)
            {
                if (MsgBox.ShowQuestionYesNo("文件\"" + fname + "\"已经存在,要覆盖吗？\n\n按【是】覆盖,按【否】重命名。")
                        == System.Windows.Forms.DialogResult.Yes)
                {
                    return AutoGeneratorSettings.enumActionOfExisted.Overide;
                }
                else
                    return AutoGeneratorSettings.enumActionOfExisted.ReName;
            }
            return AutoGeneratorSettings.CurrentSettings.ActionOfExisted;
        }

        private static string WriteInterestedRaster(IPixelIndexMapper result, IRasterDataProvider prd, RasterIdentify rstIdentify, string fname)
        {
            using (IInterestedRaster<Int16> rst = new InterestedRaster<Int16>(rstIdentify,
                new Size(prd.Width, prd.Height), prd.CoordEnvelope.Clone(), prd.SpatialRef))
            {
                rst.Put(result.Indexes.ToArray(), 1);
                fname = rst.FileName;
            }
            return fname;
        }

        private RasterIdentify GetRasterIdentify(IRasterDataProvider prd)
        {
            RasterIdentify rst = new RasterIdentify(prd.fileName);
            DataIdentify id = prd.DataIdentify;
            if (id != null)
            {
                rst.Satellite = id.Satellite;
                rst.Sensor = id.Sensor;
                if (id.OrbitDateTime != DateTime.MinValue)
                    rst.OrbitDateTime = id.OrbitDateTime;
            }
            rst.ThemeIdentify = "CMA";
            rst.ProductIdentify = _currentProduct.Identify;
            rst.SubProductIdentify = _currentSubProduct.Identify;
            rst.IsOutput2WorkspaceDir = true;
            return rst;
        }

        public string Name
        {
            get { return _featureCollectionName ?? string.Empty; }
        }

        public string GetInfo(int row, int col)
        {
            if (_features == null)
                return string.Empty;
            if (row < 0 || col < 0 || row >= _featuresSize.Height || col >= _featuresSize.Width)
                return string.Empty;
            return _features.GetCursorInfo(row * _featuresSize.Width + col);
        }
    }
}
