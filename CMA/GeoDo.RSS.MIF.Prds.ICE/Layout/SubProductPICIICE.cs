using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.UI;
using GeoDo.Project;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    /// <summary>
    /// 风三微波成像仪南北极海冰(覆盖度)产品专题图制作
    /// </summary>
    public class SubProductPICIICE : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private Action<int, string> _progressTracker = null;
        private ISmartSession _session = null;
        private string _fileName = "";
        private string _fileOpenArgs = "";
        private string _outFileIdentify = "";//SICI|NICI当前处理的是南极还是北极产品
        //以下参数用于：通过AOI清理误判的海冰
        private string _southAOIName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\ICE\\MWRI_SIC_SOUTHAOI.ldf");
        private string _northAOIName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\ICE\\MWRI_SIC_NORTHAOI.ldf");
        private bool _isUseAOIToClear = false;
        private string _aoiFileName = "";

        public SubProductPICIICE(SubProductDef subProductDef)
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
            //Initialize Argument
            string[] arguments = _argumentProvider.GetArg("CoverageArguments") as string[];
            if (arguments == null || arguments.Length < 2)
                return null;
            if (!File.Exists(arguments[0]) || string.IsNullOrEmpty(arguments[1]))
                return null;
            _fileName = arguments[0];
            _fileOpenArgs = arguments[1];
            if (arguments[1].Contains("south"))
            {
                _outFileIdentify = "SICI";
                _aoiFileName = _southAOIName;
            }
            else
            {
                _outFileIdentify = "NICI";
                _aoiFileName = _northAOIName;
            }
            //产品制作
            IExtractResult result = null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "IceConverageAlgorithm")
            {
                _isUseAOIToClear = (bool)_argumentProvider.GetArg("BoolCustomAOI");
                if (_isUseAOIToClear)
                    TryCreateNewAoiFile();
                if (_isUseAOIToClear && AOIFileExist())
                {
                    result = IceConverageAlgorithmWithAOI(progressTracker);
                }
                else
                {
                    result = IceConverageAlgorithm(progressTracker);
                }
            }
            return result;
        }

        #region 使用AOI消除误判的海冰
        private IExtractResult IceConverageAlgorithmWithAOI(Action<int, string> progressTracker)
        {
            enumDataType dataType;
            int xSize,ySize,length;
            short[] nicDataBuffer = null;
            string[] options = null;
            using (IRasterDataProvider selectRaster = GeoDataDriver.Open(_fileName, _fileOpenArgs) as IRasterDataProvider)
            {
                dataType = selectRaster.DataType;
                xSize = selectRaster.Width;
                ySize = selectRaster.Height;
                length = xSize * ySize;
                nicDataBuffer = new short[xSize * ySize];
                using (IRasterDataProvider aoiRaster = GeoDataDriver.Open(_aoiFileName) as IRasterDataProvider)
                {
                    if (aoiRaster.Width != selectRaster.Width || aoiRaster.Height != selectRaster.Height)
                        return null;
                    byte[] aoi = new byte[xSize * ySize];
                    GCHandle aoiHandle = GCHandle.Alloc(aoi, GCHandleType.Pinned);
                    GCHandle dataHandle = GCHandle.Alloc(nicDataBuffer, GCHandleType.Pinned);
                    try
                    {
                        aoiRaster.GetRasterBand(1).Read(0, 0, xSize, ySize, aoiHandle.AddrOfPinnedObject(), enumDataType.Byte, xSize, ySize);
                        selectRaster.GetRasterBand(1).Read(0, 0, xSize, ySize, dataHandle.AddrOfPinnedObject(), enumDataType.Int16, xSize, ySize);
                        for (int i = 0; i < length; i++)
                        {
                            if (aoi[i] == 0 && nicDataBuffer[i] > 0 && nicDataBuffer[i] <= 100)//移除感兴趣区域外的海冰
                                nicDataBuffer[i] = 0;//陆地是120，海洋0，目前是将误判的海冰设置为海洋
                            else if (aoi[i] == 1 && nicDataBuffer[i] == 110)
                                nicDataBuffer[i] = 100;//感兴趣区域内的无效区域填充为冰
                        }
                    }
                    finally
                    {
                        aoiHandle.Free();
                        dataHandle.Free();
                    }
                }
                //生成新的海冰覆盖度文件。
                options = CreateLDFOptions(selectRaster);
            }
            string newfile = MifEnvironment.GetFullFileName(Path.GetFileNameWithoutExtension(_fileName) + Guid.NewGuid().ToString() + ".ldf");//系统配置的Temp目录
            IRasterDataDriver driver = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            using (IRasterDataProvider newRaster = driver.Create(newfile, xSize, ySize, 1, dataType, options) as IRasterDataProvider)
            {
                GCHandle handle = GCHandle.Alloc(nicDataBuffer, GCHandleType.Pinned);
                try
                {
                    newRaster.GetRasterBand(1).Write(0, 0, xSize, ySize, handle.AddrOfPinnedObject(), dataType, xSize, ySize);
                }
                finally
                {
                    handle.Free();
                }
                _fileName = newfile;
                _fileOpenArgs = "";
            }
            return IceConverageAlgorithm(progressTracker);
        }

        private bool AOIFileExist()
        {
            return (!string.IsNullOrWhiteSpace(_aoiFileName)) && File.Exists(_aoiFileName);
        }

        /// <summary>
        /// 根据当前绘制的AOI生成新的南极或北极海冰AOI文件。
        /// 如果当前绘制AOI的文件和当前要出专题图的文件区域一致时候（都是北极或都是南极），才生成新的AOI文件
        /// </summary>
        private void TryCreateNewAoiFile()
        {
            //获取当前视窗AOI以及Raster
            _session = _argumentProvider.GetArg("SmartSession") as ISmartSession;
            ICanvasViewer cv = _session.SmartWindowManager.ActiveCanvasViewer;
            if (cv == null)
                return;
            IRasterDrawing drawing = cv.ActiveObject as IRasterDrawing;
            if (drawing == null || drawing.DataProvider == null)
                return;
            IRasterDataProvider canvasRaster = drawing.DataProvider;
            int[] drawedAOI = cv.AOIProvider.GetIndexes();
            if (drawedAOI == null || drawedAOI.Length == 0)
                return;
            //如果当前算法面板设置的海冰覆盖度 和 当前打开的不匹配，则不生成或更新AOI文件
            using (IRasterDataProvider selectedRaster = GeoDataDriver.Open(_fileName, _fileOpenArgs) as IRasterDataProvider)
            {
                if (selectedRaster == null)
                    return;
                if (selectedRaster.Width != canvasRaster.Width || selectedRaster.Height != canvasRaster.Height)
                    return;
            }
            if (File.Exists(_aoiFileName))//这里存疑，不能简单的这么删除，应当待后面的新文件生成后再删除该文件
                File.Delete(_aoiFileName);
            int xSize = canvasRaster.Width;
            int ySize = canvasRaster.Height;
            ISpatialReference spatialReference = canvasRaster.SpatialRef;
            CoordEnvelope env = canvasRaster.CoordEnvelope;
            //创建Aoi文件
            IRasterDataDriver driver = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            string[] options = CreateLDFOptions(canvasRaster);
            using (IRasterDataProvider aoiRaster = driver.Create(_aoiFileName, xSize, ySize, 1, enumDataType.Byte, options) as IRasterDataProvider)
            {
                byte[] buffer = new byte[xSize * ySize];
                foreach (int i in drawedAOI)
                {
                    buffer[i] = 1;
                }
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                try
                {
                    aoiRaster.GetRasterBand(1).Write(0, 0, xSize, ySize, handle.AddrOfPinnedObject(), enumDataType.Byte, xSize, ySize);
                }
                finally
                {
                    handle.Free();
                }
            }
        }

        private string[] CreateLDFOptions(IRasterDataProvider inRaster)
        {
            CoordEnvelope outEnv = inRaster.CoordEnvelope;
            float resX = inRaster.ResolutionX;
            float resY = inRaster.ResolutionY;
            int width = inRaster.Width;
            int height = inRaster.Height;
            ISpatialReference spatialRef = inRaster.SpatialRef;
            string bandNames = "";
            string[] options = new string[]{
                            "INTERLEAVE=BSQ",
                            "VERSION=LDF",
                            "WITHHDR=TRUE",
                            "SPATIALREF=" + spatialRef.ToProj4String(),
                            "MAPINFO={" + 1 + "," + 1 + "}:{" + outEnv.MinX + "," + outEnv.MaxY + "}:{" + resX + "," + resY + "}",
                            "BANDNAMES="+ bandNames
                        };
            return options;
        }

        #endregion

        private IExtractResult IceConverageAlgorithm(Action<int, string> progressTracker)
        {
            _argumentProvider.SetArg("OutFileIdentify", _outFileIdentify);
            _argumentProvider.SetArg("SelectedPrimaryFiles", _fileName);
            _argumentProvider.SetArg("fileOpenArgs", _fileOpenArgs);
            return ThemeGraphyResult(null);
        }
    }
}
