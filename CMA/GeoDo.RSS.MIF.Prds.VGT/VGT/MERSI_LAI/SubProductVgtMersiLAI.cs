using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.FileProject;
using GeoDo.HDF5;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public class SubProductVgtMersiLAI: CmaMonitoringSubProduct
    {
        private static IContextMessage _contextMessage = null;
        private Action<int, string> _progressTracker = null;
        private ISmartSession _session = null;
        public static UInt16[][] output = null;
        public static UInt16 laiFillVlue = LAICalc.laiFillVlue;
        //static float[] bandslope = new float[3] { 1.0E-4f, 1.0E-4f, -2.2051458E38f };
        //static float[] bandInt = new float[3] { 0.0f, 0.0f, 1.0E-4f };
        static float[] bandslope = new float[3] { 1, 1,1 };
        static float[] bandInt = new float[3] { 0.0f, 0.0f, 0.0f };
        static float angleslope = 0.01f;
        static int bandfillValue = 65535;
        static int angleFillValue = 32767;
        static float latlonFillValue = 999.9f;
        private static int _prjbandfillvalue=0;
        private static int _prjCLDMSKxOffset = 0, _prjCLDMSKyOffset = 0;
        private static float _prjbandZoom = 1000.0f;

        public SubProductVgtMersiLAI(SubProductDef subProductDef)
            : base(subProductDef)
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            _progressTracker = progressTracker;
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _progressTracker = progressTracker;
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            //产品制作
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "LAISWIR")
            {
                return DoCalLAIfromHDF();
            }
            else if (_argumentProvider.GetArg("AlgorithmName").ToString() == "LAIPrj")
            {
                return DoCalLAIfromHDFPrj();
            }
            else
                return null;
        }

        #region 基于HDF轨道文件计算LAI
        public IExtractResult DoCalLAIfromHDF()
        {
            if (_progressTracker != null)
                _progressTracker(1, "开始计算LAI...");
            Dictionary<string, string[]> arguments = _argumentProvider.GetArg("IOFilesArguments") as Dictionary<string, string[]>;
            if (arguments == null || !arguments.ContainsKey("INPUTFILES"))
                return null;
            if (arguments["INPUTFILES"].Length != 3)// || arguments["OUTPARAS"].Length != 3)
                return null;
            foreach (string infile in arguments["INPUTFILES"])
            {
                if (!File.Exists(infile))
                    return null;
            }
            string L2LSRFile = arguments["INPUTFILES"][0];
            string L1GranulesFile = arguments["INPUTFILES"][1];
            string landCoverFile = arguments["INPUTFILES"][2];
            //string outpath = arguments["OUTPARAS"][0];
            output = new UInt16[1][];
            #region 计算LAI
            if (_progressTracker != null)
                _progressTracker(5, "开始读取数据文件...");
            IRasterDataProvider lsrRaster = null;
            IRasterDataProvider angleRaster = null;
            IRasterDataProvider landcoverRaster = null;
            int outwidth, outheight;
            try
            {
                #region
                if (_progressTracker != null)
                    _progressTracker(7, "正在读取LSR数据文件...");
                #region 读取xml获取波段配置信息
                RasterIdentify datid = new RasterIdentify(Path.GetFileName(L2LSRFile));
                if (!ParseLAIConfigXml.ParseXML(datid.Satellite,datid.Sensor,true))
                    return null;
                string lsrdataset = ParseLAIConfigXml.DataSet;
                int rno = ParseLAIConfigXml.RedNO;
                int irno = ParseLAIConfigXml.IRNO;
                int swirno = ParseLAIConfigXml.SWIRNO;
                string geodatastes = ParseLAIConfigXml.GeoDataSets;
                Hdf5Operator hdfic = new Hdf5Operator(L2LSRFile);
                string[] HDFdatasets = hdfic.GetDatasetNames;
                bool matched = false;
                for (int i = 0; i < HDFdatasets.Length; i++)
                {
                    if (HDFdatasets[i].ToUpper() == lsrdataset.ToUpper())
                    {
                        matched = true;
                        break;
                    }
                }
                if (!matched)
                    return null;
                Dictionary<string, string> dsAttrs = hdfic.GetAttributes(lsrdataset);
                if (!dsAttrs.ContainsKey("band_name"))
                    return null;
                string[] bandNO = dsAttrs["band_name"].Split(',');
                //if (!bandNO.Contains(rno.ToString()) || !bandNO.Contains(irno.ToString()) || !bandNO.Contains(swirno.ToString()))
                //    return null;
                int ridx=0, iridx=0, swiridx=0;
                for (int id = 0; id < bandNO.Length;id++ )
                {
                    if (bandNO[id] == rno.ToString())
                    {
                        ridx = id+1;
                    }
                    else if (bandNO[id] == irno.ToString())
                    {
                        iridx = id + 1;
                    }
                    else if (bandNO[id] == swirno.ToString())
                    {
                        swiridx = id + 1;
                    }
                }
                if (ridx==0|| iridx==0||swiridx==0)
                    return null;
                if (dsAttrs.ContainsKey("Intercept"))
                {
                    string[] bandIntstr = dsAttrs["Intercept"].Split(',');
                    if (bandIntstr.Length==1)
                    {
                        float bandintval = float.Parse(bandIntstr[0]);
                        bandInt = new float[] { bandintval, bandintval, bandintval };
                    }
                    else
                    {
                        bandInt[0] = float.Parse(bandIntstr[ridx - 1]);
                        bandInt[1] = float.Parse(bandIntstr[iridx - 1]);
                        bandInt[2] = float.Parse(bandIntstr[swiridx - 1]);
                    }
                }
                if (dsAttrs.ContainsKey("Slope"))
                {
                    string[] bandSlope = dsAttrs["Slope"].Split(',');
                    if (bandSlope.Length == 1)
                    {
                        float bandslpval = float.Parse(bandSlope[0]);
                        bandslope = new float[] { bandslpval, bandslpval, bandslpval };
                    }
                    else
                    {
                        bandslope[0] = float.Parse(bandSlope[ridx - 1]);
                        bandslope[1] = float.Parse(bandSlope[iridx - 1]);
                        bandslope[2] = float.Parse(bandSlope[swiridx - 1]);
                    }
                }
                #endregion
                string[] openArgs = new string[] { "datasets=" + lsrdataset };
                lsrRaster = RasterDataDriver.Open(L2LSRFile, openArgs) as IRasterDataProvider;
                if (lsrRaster == null || lsrRaster.BandCount != bandNO.Length)
                    return null;
                outwidth = lsrRaster.Width;
                outheight = lsrRaster.Height;
                UInt16[] reddata = GetDataValue<UInt16>(lsrRaster.GetRasterBand(ridx), 0, 0, outwidth, outheight);
                UInt16[] IRdata = GetDataValue<UInt16>(lsrRaster.GetRasterBand(iridx), 0, 0, outwidth, outheight);
                UInt16[] swirdata = GetDataValue<UInt16>(lsrRaster.GetRasterBand(swiridx), 0, 0, outwidth, outheight);
                string[] locationArgs = new string[] { "datasets = " + geodatastes };//SolarZenith,SolarAzimuth,SensorZenith,SensorAzimuth,Longitude,Latitude
                if (_progressTracker != null)
                    _progressTracker(9, "正在读取L1数据文件...");
                angleRaster = RasterDataDriver.Open(L1GranulesFile, locationArgs) as IRasterDataProvider;
                if (angleRaster == null || angleRaster.BandCount != 6)
                    return null;
                Int16[] sza = GetDataValue<Int16>(angleRaster.GetRasterBand(1), 0, 0, outwidth, outheight);
                Int16[] saa = GetDataValue<Int16>(angleRaster.GetRasterBand(2), 0, 0, outwidth, outheight);
                Int16[] vza = GetDataValue<Int16>(angleRaster.GetRasterBand(3), 0, 0, outwidth, outheight);
                Int16[] vaa = GetDataValue<Int16>(angleRaster.GetRasterBand(4), 0, 0, outwidth, outheight);
                float[] lonM = GetDataValue<float>(angleRaster.GetRasterBand(5), 0, 0, outwidth, outheight);
                float[] latM = GetDataValue<float>(angleRaster.GetRasterBand(6), 0, 0, outwidth, outheight);
                if (_progressTracker != null)
                    _progressTracker(11, "正在读取LandCover数据文件...");
                landcoverRaster = GeoDataDriver.Open(landCoverFile) as IRasterDataProvider;
                if (landcoverRaster == null || landcoverRaster.BandCount < 1)
                    return null;
                #endregion
                output[0]=new ushort[reddata.Length];
                //output[1]=new ushort[reddata.Length];
                #region 分块并行处理
                int parts = 7;
                int step = reddata.Length / parts;
                int[] istart = new int[parts];
                int[] iend = new int[parts];
                istart[0] = 0;
                iend[0] = 0 + step;
                for (int p = 1; p < parts; p++)
                {
                    istart[p] = istart[p - 1] + step;
                    iend[p] = iend[p - 1] + step;
                }
                iend[parts - 1] = reddata.Length;
                var tasks = new Action[] { 
                    () => CalLAIParts(istart[0], iend[0], outwidth,reddata, IRdata, swirdata,  landcoverRaster,sza,saa,vza,vza,latM,lonM) ,
                () => CalLAIParts(istart[1], iend[1],  outwidth,reddata, IRdata, swirdata,landcoverRaster,sza,saa,vza,vza,latM,lonM),
                () => CalLAIParts(istart[2], iend[2],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
                () => CalLAIParts(istart[3], iend[3],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
                () => CalLAIParts(istart[4], iend[4],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
                () => CalLAIParts(istart[5], iend[5],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
                () => CalLAIParts(istart[6], iend[6],  outwidth,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,latM,lonM),
                };
                if (_progressTracker != null)
                    _progressTracker(13, "正在逐点计算LAI,请稍候...");
                System.Threading.Tasks.Parallel.Invoke(tasks);
                #endregion
                #region 建立输出文件
                if (_progressTracker != null)
                    _progressTracker(75, "正在投影LAI数据,请稍候...");
                return OutputLAI(angleRaster, L2LSRFile);
                #endregion
            }
            catch (System.Exception ex)
            {
                //_progressTracker(0, ex.Message);
                PrintInfo(ex.Message);
                return null;
            }
            finally
            {
                if (lsrRaster != null)
                    lsrRaster.Dispose();
                if (angleRaster != null)
                    angleRaster.Dispose();
                if (landcoverRaster != null)
                    landcoverRaster.Dispose();
                if (output != null)
                    output = null;
            }
            #endregion 
        }

        public static unsafe T[] GetDataValue<T>(IRasterBand band,int xoffset=0,int yoffset=0,int width=1,int height =1)
        {
            enumDataType dataType = band.DataType;
            //width = band.Width;
            //height = band.Height;
            int length = width * height;
            switch (dataType)
            {
                case enumDataType.Float:
                    {
                        float[] buffer = new float[length];
                        fixed (float* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(xoffset, yoffset, width, height, bufferPtr, enumDataType.Float, width, height);
                        }
                        return buffer as T[];
                    }
                case enumDataType.UInt16:
                    {
                        UInt16[] srcBuffer = new UInt16[length];
                        fixed (UInt16* dataPtr = srcBuffer)
                        {
                            IntPtr srcBufferPtr = new IntPtr(dataPtr);
                            band.Read(0, 0, width, height, srcBufferPtr, dataType, width, height);//读一个波段的数据
                        }
                        return srcBuffer as T[];
                    }
                case enumDataType.Int16:
                    {
                        short[] buffer = new short[length];
                        fixed (short* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(xoffset, yoffset, width, height, bufferPtr, enumDataType.Int16, width, height);
                        }
                        return buffer as T[];
                    }
                case enumDataType.Byte:
                    {
                        Byte[] buffer = new Byte[width * height];
                        fixed (Byte* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(xoffset, yoffset, width, height, bufferPtr, enumDataType.Byte, width, height);
                        }
                        return buffer as T[];
                    }

            }
            return null;
        }

        public static void CalLAIParts(int istart,int iend, int outwidth,UInt16[] reddata, UInt16[] IRdata, UInt16[] swirdata, IRasterDataProvider landcoverRaster,Int16[] sza,Int16[] saa,Int16[] vza,Int16[] vaa,float [] latM,float [] lonM)
        {
            for (int i = istart; i < iend; i++)
            {
                try
                {
                    if (reddata[i] == bandfillValue || IRdata[i] == bandfillValue || swirdata[i] == bandfillValue)
                    {
                        output[0][i] = laiFillVlue;
                        //output[1][i] = laiFillVlue;
                        continue;
                    }
                    float[] b346 = new float[] { reddata[i] * bandslope[0] + bandInt[0], IRdata[i] * bandslope[1] + bandInt[1], swirdata[i] * bandslope[2] + bandInt[2] };
                    int y = i / outwidth;
                    int x = i % outwidth;
                    if (sza[i] == angleFillValue || saa[i] == angleFillValue || vza[i] == angleFillValue || vaa[i] == angleFillValue)
                    {
                        output[0][i] = laiFillVlue;
                        //output[1][i] = laiFillVlue;
                        continue;
                    }
                    float[] angles = new float[] { sza[i] * angleslope, saa[i] * angleslope, vza[i] * angleslope, vaa[i] * angleslope };
                    if (latM[i] == latlonFillValue || lonM[i] == latlonFillValue)
                    {
                        output[0][i] = laiFillVlue;
                        //output[1][i] = laiFillVlue;
                        continue;
                    }
                    float lctReslX = landcoverRaster.ResolutionX;
                    float lctReslY = landcoverRaster.ResolutionY;
                    double lctMinLon = landcoverRaster.CoordEnvelope.MinX;
                    double lctMaxLat = landcoverRaster.CoordEnvelope.MaxY;
                    float lat = latM[i], lon = lonM[i];
                    //根据像元的经纬度去LandCoverType文件中找相应的偏移量xoffset和yoffset，offset =xoffset+yoffset*landcoverRaster.Width;
                    int xoffset = (int)Math.Floor((lon - lctMinLon) / lctReslX);
                    int yoffset = (int)Math.Floor((lctMaxLat - lat) / lctReslY);
                    if (xoffset < 0 || yoffset < 0)
                    {
                        output[0][i] = laiFillVlue;
                        //output[1][i] = laiFillVlue;
                        continue;
                    }
                    byte[] lct = GetDataValue<byte>(landcoverRaster.GetRasterBand(1), xoffset, yoffset, 1, 1);
                    //int offset = xoffset + yoffset * lctWidth;
                    UInt16[] lelai = LAICalc.CalLAI(b346, angles, lct[0]);
                    output[0][i] = lelai[0];
                    //output[1][i] = lelai[1];
                }
                catch (System.Exception ex)
                {
                    PrintInfo(ex.Message);
                    output[0][i] = laiFillVlue;
                    continue;
                }
            }
        }

        #endregion 
        public  IExtractResult OutputLAI(IRasterDataProvider angleRaster, string L2LSRFile,bool isNeedPrj =true)
        {
            int outwidth = angleRaster.Width, outheight = angleRaster.Height;
            IRasterDataProvider mainRaster = null;
            try
            {
                #region 建立输出文件
                RasterIdentify datid = new RasterIdentify(Path.GetFileName(L2LSRFile));
                datid.ProductIdentify = "VGT";
                datid.SubProductIdentify = "0LAI";
                float outResolution = 0.01f;
                if (datid.Resolution == "1000M")
                    outResolution = 0.01f;
                else if (datid.Resolution == "5000M")
                    outResolution = 0.05f;
                string laiFileName = datid.ToWksFullFileName(".ldf");
                FileExtractResult result = null;
                if (isNeedPrj)
                {
                    mainRaster=new ArrayRasterDataProvider<UInt16>("Array", output, outwidth, outheight);
                    HDF4FilePrjSettings setting = new HDF4FilePrjSettings();
                    setting.LocationFile = angleRaster;
                    setting.OutFormat = "LDF";
                    setting.OutResolutionX = setting.OutResolutionY = outResolution;
                    Dictionary<string, double> exargs = new Dictionary<string, double>();
                    exargs.Add("FillValue", laiFillVlue);
                    setting.ExtArgs = new object[] { exargs };
                    HDF4FileProjector projector = new HDF4FileProjector();
                    GeoDo.RasterProject.PrjEnvelope mainPrj = null;
                    projector.ComputeDstEnvelope(angleRaster, GeoDo.Project.SpatialReference.GetDefault(), out mainPrj, null);
                    if (mainPrj != null)
                    {
                        setting.OutEnvelope = mainPrj;
                        setting.OutPathAndFileName = laiFileName;
                        projector.Project(mainRaster, setting, GeoDo.Project.SpatialReference.GetDefault(), null);
                        if (_progressTracker != null)
                            _progressTracker(100, "LAI数据投影完成！");
                    }
                    else
                        return null;
                } 
                else
                {
                    if(!CreateRaster(laiFileName, 1,enumDataType.UInt16, angleRaster))
                        return null;
                }
                string resultFile = Path.Combine(Path.GetDirectoryName(laiFileName), Path.GetFileNameWithoutExtension(laiFileName) + ".dat");
                if (File.Exists(laiFileName))
                {
                    if (File.Exists(resultFile))
                        File.Delete(resultFile);
                    File.Move(laiFileName, resultFile);
                    result = new FileExtractResult("0LAI", resultFile, true);
                }
                else
                    result = new FileExtractResult("0LAI", laiFileName, true);
                result.SetDispaly(true);
                return result;
                #endregion
            }
            catch (System.Exception ex)
            {
                //_progressTracker(0, ex.Message);
                PrintInfo(ex.Message);
                return null;
            }
            finally
            {
                if (mainRaster!=null)
                    mainRaster.Dispose();
            }
        }

        public static bool CreateRaster(string outFileName, int bandCount,enumDataType datatype,IRasterDataProvider referProvider)
        {
            CoordEnvelope env = referProvider.CoordEnvelope;
            float resX = referProvider.ResolutionX;
            float resY = referProvider.ResolutionY;
            int width = referProvider.Width;
            int height = referProvider.Height;
            Project.ISpatialReference spatialRef = referProvider.SpatialRef;
            List<string> options = new List<string>();
            options.Add("INTERLEAVE=BSQ");
            options.Add("VERSION=LDF");
            options.Add("WITHHDR=TRUE");
            options.Add("SPATIALREF=" + spatialRef.ToProj4String());
            options.Add("MAPINFO={" + 1 + "," + 1 + "}:{" + env.MinX + "," + env.MaxY + "}:{" + resX + "," + resY + "}"); //=env.ToMapInfoString(new Size(width, height));
            options.Add("BANDNAMES= " + "LAI");
            //string hdrfile = HdrFile.GetHdrFileName(referProvider.fileName);
            //if (!string.IsNullOrWhiteSpace(hdrfile) && File.Exists(hdrfile))
            //{
            //    HdrFile hdr = HdrFile.LoadFrom(hdrfile);
            //    if (hdr != null && hdr.BandNames != null)
            //        options.Add("BANDNAMES=" + string.Join(",", hdr.BandNames));
            //}
            //CheckAndCreateDir();
            if (!Directory.Exists(Path.GetDirectoryName(outFileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(outFileName));
            using (IRasterDataDriver raster = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver)
            {
                using (RasterDataProvider outRaster = raster.Create(outFileName, width, height, bandCount, datatype, options.ToArray()) as RasterDataProvider)
                {
                    unsafe
                    {
                        fixed (UInt16* ptr = output[0])
                        {
                            IntPtr buffer = new IntPtr(ptr);
                            outRaster.GetRasterBand(1).Write(0, 0, width, height, buffer, enumDataType.UInt16, width, height);
                        }
                    }
                    return true;
                }
            }
        }

        private static void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        #region 基于投影后的ldf文件的LAI计算
        public IExtractResult DoCalLAIfromHDFPrj()
        {
            if (_progressTracker != null)
                _progressTracker(1, "开始计算LAI...");
            Dictionary<string, string[]> arguments = _argumentProvider.GetArg("IOFilesArguments") as Dictionary<string, string[]>;
            if (arguments == null || !arguments.ContainsKey("INPUTFILES"))
                return null;
            if (arguments["INPUTFILES"].Length !=7)
                return null;
            foreach (string infile in arguments["INPUTFILES"])
            {
                if (!File.Exists(infile))
                    return null;
            }
            string L2LSRFile = arguments["INPUTFILES"][0];//LSR投影文件
            string _szafile= arguments["INPUTFILES"][1];
            string _saafile = arguments["INPUTFILES"][2];
            string _vzafile = arguments["INPUTFILES"][3];
            string _vaafile = arguments["INPUTFILES"][4];
            string L2CLDMaskFile = arguments["INPUTFILES"][5];//云检测文件
            string landCoverFile = arguments["INPUTFILES"][6];//土地覆盖类型文件
            #region 获取波段信息
            if (_argumentProvider.GetArg("Red") == null)
            {
                PrintInfo("参数\"Red\"为空。");
                return null;
            }
            int rno = (int)(_argumentProvider.GetArg("Red"));
            if (rno<=0)
            {
                PrintInfo("参数\"Red\"波段号无效。");
                return null;
            }
            if (_argumentProvider.GetArg("NearInfrared") == null)
            {
                PrintInfo("参数\"NearInfrared\"为空。");
                return null;
            }
            int irno = (int)(_argumentProvider.GetArg("NearInfrared"));
            if (irno <= 0)
            {
                PrintInfo("参数\"NearInfrared\"波段号无效。");
                return null;
            }
            if (_argumentProvider.GetArg("SWInfrared") == null)
            {
                PrintInfo("参数\"SWInfrared\"为空。");
                return null;
            }
            int swirno = (int)(_argumentProvider.GetArg("SWInfrared"));
            if (swirno <= 0)
            {
                PrintInfo("参数\"SWInfrared\"波段号无效。");
                return null;
            }
            #endregion
            int rnoCH = -1, irnoCH = -1, swirnoCH = -1;
            IBandNameRaster bandNameRaster =null;
            bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            using (IRasterDataProvider dr = GeoDataDriver.Open(L2LSRFile) as IRasterDataProvider)
            {
                if(dr!=null)
                    bandNameRaster = dr as IBandNameRaster;
            }            
            if (bandNameRaster == null)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            int newbandNo = -1;
            if (bandNameRaster.TryGetBandNoFromBandName(rno, out newbandNo))
                rnoCH = newbandNo;
            if (bandNameRaster.TryGetBandNoFromBandName(irno, out newbandNo))
                irnoCH = newbandNo;
            if (bandNameRaster.TryGetBandNoFromBandName(swirno, out newbandNo))
                swirnoCH = newbandNo;
            //double VisibleZoom = Obj2Double(_argumentProvider.GetArg("Visible_Zoom"));
            //double FarInfraredZoom = Obj2Double(_argumentProvider.GetArg("FarInfrared_Zoom"));
            //double NDSIVisibleZoom = Obj2Double(_argumentProvider.GetArg("NDSIVisible_Zoom"));
            if (rnoCH == -1 || irnoCH == -1 || swirnoCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            output = new UInt16[1][];
            #region 计算LAI
            if (_progressTracker != null)
                _progressTracker(5, "开始读取数据文件...");
            IRasterDataProvider lsrRaster = null;
            IRasterDataProvider angleRaster = null;
            IRasterDataProvider L2CLDMaskRaster = null;
            IRasterDataProvider landcoverRaster = null;
            int outwidth, outheight;
            try
            {
                #region 波段数据读取
                if (_progressTracker != null)
                    _progressTracker(7, "正在读取地表反射率数据文件...");
                lsrRaster = GeoDataDriver.Open(L2LSRFile) as IRasterDataProvider;
                if (lsrRaster == null || lsrRaster.BandCount <3)
                    return null;
                float prjReslX = lsrRaster.ResolutionX;
                float prjReslY = lsrRaster.ResolutionY;
                double prjMinLon = lsrRaster.CoordEnvelope.MinX;
                double prjMaxLat = lsrRaster.CoordEnvelope.MaxY;
                outwidth = lsrRaster.Width;
                outheight = lsrRaster.Height;
                UInt16[] reddata = GetDataValue<UInt16>(lsrRaster.GetRasterBand(rnoCH), 0, 0, outwidth, outheight);
                UInt16[] IRdata = GetDataValue<UInt16>(lsrRaster.GetRasterBand(irnoCH), 0, 0, outwidth, outheight);
                UInt16[] swirdata = GetDataValue<UInt16>(lsrRaster.GetRasterBand(swirnoCH), 0, 0, outwidth, outheight);
                if (_progressTracker != null)
                    _progressTracker(9, "正在读取角度数据文件...");
                Int16[] sza, saa, vza, vaa;
                angleRaster = GeoDataDriver.Open(_szafile) as IRasterDataProvider;
                if (angleRaster == null || angleRaster.BandCount <1)
                    return null;
                sza = GetDataValue<Int16>(angleRaster.GetRasterBand(1), 0, 0, outwidth, outheight);
                angleRaster = GeoDataDriver.Open(_saafile) as IRasterDataProvider;
                if (angleRaster == null || angleRaster.BandCount < 1)
                    return null;
                saa = GetDataValue<Int16>(angleRaster.GetRasterBand(1), 0, 0, outwidth, outheight);
                angleRaster = GeoDataDriver.Open(_vzafile) as IRasterDataProvider;
                if (angleRaster == null || angleRaster.BandCount < 1)
                    return null;
                vza = GetDataValue<Int16>(angleRaster.GetRasterBand(1), 0, 0, outwidth, outheight);
                angleRaster = GeoDataDriver.Open(_vaafile) as IRasterDataProvider;
                if (angleRaster == null || angleRaster.BandCount < 1)
                    return null;
                vaa = GetDataValue<Int16>(angleRaster.GetRasterBand(1), 0, 0, outwidth, outheight);
                L2CLDMaskRaster = GeoDataDriver.Open(L2CLDMaskFile) as IRasterDataProvider;
                if (L2CLDMaskRaster == null || L2CLDMaskRaster.BandCount < 1)
                    return null;
                #region 计算云检测文件的xy偏移量；
                double CLDMaskminx = L2CLDMaskRaster.CoordEnvelope.MinX, CLDMaskmaxy = L2CLDMaskRaster.CoordEnvelope.MaxY;
                _prjCLDMSKxOffset = (int)Math.Ceiling((prjMinLon - CLDMaskminx) / prjReslX);
                _prjCLDMSKyOffset = (int)Math.Ceiling((CLDMaskmaxy - prjMaxLat) / prjReslY);
                #endregion
                byte[] cldmask = GetDataValue<byte>(L2CLDMaskRaster.GetRasterBand(1), _prjCLDMSKxOffset, _prjCLDMSKyOffset, outwidth, outheight);
                if (_progressTracker != null)
                    _progressTracker(11, "正在读取LandCover数据文件...");
                landcoverRaster = GeoDataDriver.Open(landCoverFile) as IRasterDataProvider;
                if (landcoverRaster == null || landcoverRaster.BandCount < 1)
                    return null;
                #endregion
                output[0] = new ushort[reddata.Length];
                #region 分块并行处理
                int parts = 7;
                int step = reddata.Length / parts;
                int[] istart = new int[parts];
                int[] iend = new int[parts];
                istart[0] = 0;
                iend[0] = 0 + step;
                for (int p = 1; p < parts; p++)
                {
                    istart[p] = istart[p - 1] + step;
                    iend[p] = iend[p - 1] + step;
                }
                iend[parts - 1] = reddata.Length;
                var tasks = new Action[] { 
                    () => CalPrjLAIParts(istart[0], iend[0], outwidth,cldmask,reddata, IRdata, swirdata,  landcoverRaster,sza,saa,vza,vza,prjReslX,prjMinLon,prjMaxLat) ,
                () => CalPrjLAIParts(istart[1], iend[1],  outwidth,cldmask,reddata, IRdata, swirdata,landcoverRaster,sza,saa,vza,vza,prjReslX,prjMinLon,prjMaxLat),
                () => CalPrjLAIParts(istart[2], iend[2],  outwidth,cldmask,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,prjReslX,prjMinLon,prjMaxLat),
                () => CalPrjLAIParts(istart[3], iend[3],  outwidth,cldmask,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,prjReslX,prjMinLon,prjMaxLat),
                () => CalPrjLAIParts(istart[4], iend[4],  outwidth,cldmask,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,prjReslX,prjMinLon,prjMaxLat),
                () => CalPrjLAIParts(istart[5], iend[5],  outwidth,cldmask,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,prjReslX,prjMinLon,prjMaxLat),
                () => CalPrjLAIParts(istart[6], iend[6],  outwidth,cldmask,reddata, IRdata, swirdata, landcoverRaster,sza,saa,vza,vza,prjReslX,prjMinLon,prjMaxLat),
                };
                if (_progressTracker != null)
                    _progressTracker(13, "正在逐点计算LAI,请稍候...");
                System.Threading.Tasks.Parallel.Invoke(tasks);
                #endregion
                #region 建立输出文件
                if (_progressTracker != null)
                    _progressTracker(75, "正在输出LAI数据,请稍候...");
                return OutputLAI(angleRaster, L2LSRFile,false);
                #endregion
            }
            catch (System.Exception ex)
            {
                //_progressTracker(0, ex.Message);
                PrintInfo(ex.Message);
                return null;
            }
            finally
            {
                if (lsrRaster != null)
                    lsrRaster.Dispose();
                if (angleRaster != null)
                    angleRaster.Dispose();
                if (landcoverRaster != null)
                    landcoverRaster.Dispose();
                if (L2CLDMaskRaster != null)
                    L2CLDMaskRaster.Dispose();
                if (output != null)
                    output = null;
            }
            #endregion
        }

        public static void CalPrjLAIParts(int istart, int iend, int outwidth, byte [] cldmask,UInt16[] reddata, UInt16[] IRdata, UInt16[] swirdata, IRasterDataProvider landcoverRaster, Int16[] sza, Int16[] saa, Int16[] vza, Int16[] vaa,float prjresl,double minlon,double maxlat)
        {
            float pixelX, pixelY;
            for (int i = istart; i < iend; i++)
            {
                try
                {
                    if (cldmask[i] == 0 || (cldmask[i] != 255 && cldmask[i] != 253 && cldmask[i] != 191 && cldmask[i] != 127 && cldmask[i] != 125))
                    {
                        output[0][i] = laiFillVlue;
                        continue;
                    }
                    if (reddata[i] == _prjbandfillvalue || IRdata[i] == _prjbandfillvalue || swirdata[i] == _prjbandfillvalue)
                    {
                        output[0][i] = laiFillVlue;
                        //output[1][i] = laiFillVlue;
                        continue;
                    }
                    float[] b346 = new float[] { reddata[i] / _prjbandZoom, IRdata[i] / _prjbandZoom, swirdata[i] / _prjbandZoom };
                    //r,ir,swir的波段数据为辐射亮度而非反射率，应进行转换
                    //refl =pi*radiance*d^2/Es,ES为Solar Irradiance on RSB Detectors，每个波长的反射单元的太阳辐射量度；
                    //d为数据获取时间段内的平均日地距离/AU；
                    int y = i / outwidth;
                    int x = i % outwidth;
                    pixelX = (float)(minlon + x * prjresl);
                    pixelY = (float)(maxlat - y * prjresl);
                    if (sza[i] == _prjbandfillvalue || saa[i] == _prjbandfillvalue || vza[i] == _prjbandfillvalue || vaa[i] == _prjbandfillvalue)
                    {
                        output[0][i] = laiFillVlue;
                        //output[1][i] = laiFillVlue;
                        continue;
                    }
                    float[] angles = new float[] { sza[i] * angleslope, saa[i] * angleslope, vza[i] * angleslope, vaa[i] * angleslope };
                    float lctReslX = landcoverRaster.ResolutionX;
                    float lctReslY = landcoverRaster.ResolutionY;
                    double lctMinLon = landcoverRaster.CoordEnvelope.MinX;
                    double lctMaxLat = landcoverRaster.CoordEnvelope.MaxY;
                    //根据像元的经纬度去LandCoverType文件中找相应的偏移量xoffset和yoffset，offset =xoffset+yoffset*landcoverRaster.Width;
                    int xoffset = (int)Math.Floor((pixelX - lctMinLon) / lctReslX);
                    int yoffset = (int)Math.Floor((lctMaxLat - pixelY) / lctReslY);
                    if (xoffset < 0 || yoffset < 0)
                    {
                        output[0][i] = laiFillVlue;
                        //output[1][i] = laiFillVlue;
                        continue;
                    }
                    byte[] lct = GetDataValue<byte>(landcoverRaster.GetRasterBand(1), xoffset, yoffset, 1, 1);
                    //int offset = xoffset + yoffset * lctWidth;
                    UInt16[] lelai = LAICalc.CalLAI(b346, angles, lct[0]);
                    output[0][i] = lelai[0];
                    //output[1][i] = lelai[1];
                }
                catch (System.Exception ex)
                {
                    PrintInfo(ex.Message);
                    output[0][i] = laiFillVlue;
                    continue;
                }
            }
        }

        #endregion
    }
}
