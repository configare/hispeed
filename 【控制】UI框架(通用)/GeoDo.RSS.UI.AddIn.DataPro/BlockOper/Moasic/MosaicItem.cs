using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public class MosaicItem : IDisposable
    {
        public MosaicItem(IRasterDataProvider mainFile)
        {
            try
            {
                MainFile = mainFile;
                string dir = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, ".prjChche", Path.GetFileName(mainFile.fileName));
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                CreateOverviewBmp(null);
            }
            catch (Exception ex)
            {
                throw new Exception("获取范围或者生成缩略图失败：" + ex.Message, ex);
            }
        }
        
        /// <summary>
        /// 用于拼接的主文件
        /// </summary>
        public IRasterDataProvider MainFile;
        /// <summary>
        /// 拼接范围
        /// </summary>
        public CoordEnvelope Envelope;
        /// <summary>
        /// 缩略图
        /// </summary>
        public Bitmap OverViewBmp;

        private void CreateOverviewBmp(int[] bandNos)
        {
            Bitmap bmp;
            GetOverview(MainFile,bandNos, out bmp);
            OverViewBmp = bmp;
        }

        private void GetOverview(IRasterDataProvider file,int[] bandNos, out Bitmap bmp)
        {
            try
            {
                if (bandNos == null || bandNos.Length == 0)
                    bandNos = file.GetDefaultBands();
                if (bandNos == null || bandNos.Length == 0)
                    bandNos = new int[] { 1, 1, 1 };
               bmp = GenerateOverview(file, bandNos); 
            }
            catch
            {
                bmp = null;
            }
        }

        private Bitmap GenerateOverview(IRasterDataProvider dataProvider, int[] bandNos)
        {
            CoordEnvelope env = dataProvider.CoordEnvelope;
            if (env != null)
                Envelope = env;
            IOverviewGenerator v = dataProvider as IOverviewGenerator;
            Size size = v.ComputeSize(1000);//缩略图最大不超过的尺寸
            Bitmap bm = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
            v.Generate(bandNos, ref bm);
            return bm;
        }

        public void Dispose()
        {
            MainFile = null;
            Envelope = null;
            if (OverViewBmp != null)
            {
                OverViewBmp.Dispose();
                OverViewBmp = null;
            }
        }
    }
}
