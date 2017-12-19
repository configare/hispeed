using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using GeoDo.FileProject;
using GeoDo.Project;
using GeoDo.RasterProject;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.UI.AddIn.DataPro
{   
    public class MosaicProjectItem:IDisposable
    {
        private ISpatialReference _spatialRef;
        private string _prjFilename = "";
        private string _errorMsg = "";

        public MosaicProjectItem(IRasterDataProvider mainFile)
        {
            try
            {
                MainFile = mainFile;
                string dir = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, ".prjChche", Path.GetFileName(mainFile.fileName));
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                _prjFilename = Path.Combine(dir, Path.GetFileNameWithoutExtension(mainFile.fileName) + ".OVERVIEW.LDF");
                //
                DataIdentify dataIdentify = mainFile.DataIdentify;
                bool isDay = dataIdentify.DayOrNight == enumDayOrNight.Day || dataIdentify.DayOrNight == enumDayOrNight.Null;
                int[] bandNos = RgbStretcherFactory.GetDefaultBands(dataIdentify.Satellite, dataIdentify.Sensor, false, isDay);
                CreateOverviewBmp(null, bandNos);
            }
            catch (Exception ex)
            {
                _errorMsg = "获取范围或者生成缩略图失败：" + ex.Message;
                throw new Exception(_errorMsg, ex);
            }
        }

        public string ErrorMsg
        {
            get
            {
                return _errorMsg;
            }
        }
        
        /// <summary>
        /// 用于拼接投影的一个主轨道文件
        /// </summary>
        public IRasterDataProvider MainFile;
        /// <summary>
        /// 有效投影范围
        /// </summary>
        public PrjEnvelope Envelope;

        /// <summary>
        /// 用于生成缩略图的投影后数据
        /// </summary>
        public IRasterDataProvider OverViewFile;

        /// <summary>
        /// 缩略图
        /// </summary>
        public Bitmap OverViewBmp;

        private void CreateOverviewBmp(ISpatialReference spatialRef, int[] bandNos)
        {
            if (spatialRef == null)
                spatialRef = SpatialReference.GetDefault();
            if (_spatialRef == null || !_spatialRef.IsSame(spatialRef))
                _spatialRef = spatialRef;
            Bitmap bmp;
            GetOverview(MainFile, _spatialRef, bandNos, out bmp, out _errorMsg);
            OverViewBmp = bmp;
        }

        private void GetOverview(IRasterDataProvider file, ISpatialReference spatialRef, int[] bandNos, out Bitmap bmp ,out string errorMessage)
        {
            try
            {
                errorMessage = "";
                if (bandNos == null)
                    bandNos = new int[] { 1 };
                if (!File.Exists(_prjFilename))
                {
                    float resolution = 0.05f;
                    if (file.DataIdentify != null && file.DataIdentify.Sensor == "VISSR")
                        resolution = 0.1f;
                    if (file.DataIdentify != null && (file.DataIdentify.Satellite == "FY1D"))
                        resolution = 0.1f;
                    PrjOutArg arg = new PrjOutArg(spatialRef, null, resolution, resolution, _prjFilename);
                    arg.SelectedBands = bandNos.OrderBy((i) => { return i; }).ToArray();
                    ProjectionFactory proj = new ProjectionFactory();
                    string[] files = proj.Project(file, arg, null, out errorMessage);
                    if (files == null || files.Length == 0 || files[0] == null)
                    {
                        bmp = null;
                        if (string.IsNullOrWhiteSpace(errorMessage))
                            errorMessage = "投影缩略图文件失败";
                        return;
                    }
                    _prjFilename = files[0];
                }
                using (IRasterDataProvider prd = GeoDataDriver.Open(_prjFilename) as IRasterDataProvider)
                {
                    if (bandNos == null || bandNos.Length == 0)
                        bandNos = prd.GetDefaultBands();
                    if (bandNos == null || bandNos.Length == 0)
                    {
                        bmp = null;
                        if (string.IsNullOrWhiteSpace(errorMessage))
                            errorMessage = "获取缩略图显示波段列表为空";
                        return;
                    }
                    //bandNos = new int[] { 1, 1, 1 };
                    int[] orderBandMaps;
                    PrjBand[] prjBands = BandNoToBand(bandNos, out orderBandMaps);
                    bmp = GenerateOverview(prd, orderBandMaps);
                }
            }
            catch
            {
                bmp = null;
                throw;
            }
        }

        private PrjBand[] BandNoToBand(int[] bandNos, out int[] newBandMaps)
        {
            List<PrjBand> sortList = new List<PrjBand>();
            int[] sortBandNos = bandNos.Distinct().OrderBy(x => x).ToArray();
            newBandMaps = new int[bandNos.Length];
            foreach (int band in sortBandNos)
            {
                sortList.Add(new PrjBand("", -1f, band.ToString(), -1, "", "", ""));
            }
            for (int i = 0; i < bandNos.Length; i++)
            {
                for (int j = 0; j < sortBandNos.Length; j++)
                {
                    if (bandNos[i] == sortBandNos[j])
                    {
                        newBandMaps[i] = j + 1;
                        break;
                    }
                }
            }
            return sortList.ToArray();
        }

        private Bitmap GenerateOverview(IRasterDataProvider prd, int[] bandNos)
        {
            CoordEnvelope env = prd.CoordEnvelope;
            if (env != null)
                Envelope = new PrjEnvelope(env.MinX, env.MaxX, env.MinY, env.MaxY);
            IOverviewGenerator v = prd as IOverviewGenerator;
            Size size = v.ComputeSize(1000);//缩略图最大不超过的尺寸
            Bitmap bm = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
            v.Generate(bandNos, ref bm);
            return bm;
        }

        public void Dispose()
        {
            MainFile = null;
            Envelope = null;
            OverViewFile = null;
            if (OverViewBmp != null)
            {
                OverViewBmp.Dispose();
                OverViewBmp = null;
            }
        }
    }
}
