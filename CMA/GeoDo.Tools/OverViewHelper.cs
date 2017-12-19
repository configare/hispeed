using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using GeoDo.FileProject;

namespace GeoDo.Tools
{
    public class OverViewHelper
    {
        public static string OverView(string prdFilename, int maxSize)
        {
            using (IRasterDataProvider prd = GeoDataDriver.Open(prdFilename) as IRasterDataProvider)
            {
                return OverView(prd, maxSize);
            }
        }

        public static string OverView(IRasterDataProvider prd, int maxSize)
        {
            try
            {
                int[] bands = prd.GetDefaultBands();
                if (bands == null || bands.Length == 0)
                    bands = new int[] { 1, 1, 1 };
                for (int i = 0; i < 3; i++)
                {
                    if (bands[i] > prd.BandCount)
                    {
                        bands[i] = 1;
                    }
                }
                //int[] newBandMaps;
                //PrjBand[] prjBands = BandNoToBand(bands, out newBandMaps);
                //bands = bands.Reverse().ToArray();
                string overViewFilename = OverFileName(prd.fileName);
                using (Bitmap bmp = GenerateOverview(prd, bands, maxSize))
                {
                    bmp.MakeTransparent(Color.Black);
                    bmp.Save(overViewFilename, ImageFormat.Png);
                    bmp.Dispose();
                }
                return overViewFilename;
            }
            catch (Exception ex)
            {
                Console.WriteLine("生成缩略图失败" + ex.Message);
                return null;
            }
        }

        public static string OverFileName(string filename)
        {
            return Path.ChangeExtension(filename, ".overview.png");
        }

        private static Bitmap GenerateOverview(IRasterDataProvider prd, int[] bandNos, int maxSize)
        {
            CoordEnvelope env = prd.CoordEnvelope;
            IOverviewGenerator v = prd as IOverviewGenerator;
            Size size = v.ComputeSize(maxSize);//缩略图最大不超过的尺寸
            Bitmap bm = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
            v.Generate(bandNos, ref bm);
            return bm;
        }

        //private PrjBand[] BandNoToBand(int[] bandNos, out int[] newBandMaps)
        //{
        //    List<PrjBand> sortList = new List<PrjBand>();
        //    int[] sortBandNos = bandNos.Distinct().OrderBy(x => x).ToArray();
        //    newBandMaps = new int[bandNos.Length];
        //    foreach (int band in sortBandNos)
        //    {
        //        sortList.Add(new PrjBand("", -1f, band.ToString(), -1, "", "", ""));
        //    }
        //    for (int i = 0; i < bandNos.Length; i++)
        //    {
        //        for (int j = 0; j < sortBandNos.Length; j++)
        //        {
        //            if (bandNos[i] == sortBandNos[j])
        //            {
        //                newBandMaps[i] = j + 1;
        //                break;
        //            }
        //        }
        //    }
        //    return sortList.ToArray();
        //}
    }
}
