using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 举例使用栅格数据计算模型的方法。
    /// </summary>
    public class RasterNdvi
    {
        public void Calc()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Action<int, string> progres = new Action<int, string>(
                (progress, text) =>
                {
                    //this.progressBar1.Value = progress;
                    //this.Text = text;
                    //Application.DoEvents();
                    Console.WriteLine(progress);
                }
                );
            //举例计算NDVI
            RasterProcessModel<ushort, short> rfr = null;
            RasterMaper[] fileIns = null;
            RasterMaper[] fileOuts = null;
            try
            {
                string fin = @"E:\Smart\MAS\ldf\FY3A_MERSI_GBAL_L1_20120517_0235_0250M_MS_GLL_DXX.ldf";
                //fin = @"E:\Smart\ldf\FY3A_MERSI_海南省_GLL_L1_20120911_D_0250M_MS.ldf";
                fin = @"E:\Smart\MAS\ldf\FY3B_MERSI_20120510_0640.LDF";
                RasterDataProvider or = RasterDataDriver.Open(fin) as RasterDataProvider;
                //初始化输出数据，这里输出为dat,可根据实际需求，自行决定输出文件格式
                IRasterDataDriver dd = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
                string mapInfo = or.CoordEnvelope.ToMapInfoString(new Size(or.Width * 2, or.Height * 2));
                //"MAPINFO={X,Y}:{Col,Row}:{ResolutionX,ResolutionY}";
                //mapInfo = "MAPINFO={1,1}:{108.0904,20.1572}:{0.0025,0.0025}";
                //输出数据和输入数据的范围可以不一致
                RasterDataProvider tr = dd.Create(@"E:\Smart\ldf\FY3A_MERSI_海南省_GLL_L1_20120911_D_0250M_MS_NDVI.dat", or.Width, or.Height, 1, enumDataType.Int16, mapInfo) as RasterDataProvider;
                tr.GetRasterBand(1).Fill(-999);

                RasterMaper fileIn = new RasterMaper(or, new int[] { 3, 4 });
                RasterMaper fileOut = new RasterMaper(tr, new int[] { 1 });
                fileIns = new RasterMaper[] { fileIn };
                fileOuts = new RasterMaper[] { fileOut };
                //创建处理器
                rfr = new RasterProcessModel<ushort, short>(progres);
                rfr.SetRaster(fileIns, fileOuts);
                rfr.SetTemplateAOI("vector:海陆模版");
                rfr.RegisterCalcModel(new RasterCalcHandler<ushort, short>((rvInVistor, rvOutVistor, aoiIndex) =>
                {
                    if (rvInVistor[0].RasterBandsData != null)
                    {
                        if (rvInVistor == null)
                            return;
                        ushort[] inBand0 = rvInVistor[0].RasterBandsData[0];//第1个输入文件的第1个波段的各像素值
                        ushort[] inBand1 = rvInVistor[0].RasterBandsData[1];//第1个输入文件的第2个波段的各像素值
                        short[] ndvi = new short[inBand0.Length];
                        if (aoiIndex != null)
                        {
                            int index;
                            for (int i = 0; i < aoiIndex.Length; i++)
                            {
                                index = aoiIndex[i];
                                rvOutVistor[0].RasterBandsData[0][index] = (short)((inBand1[index] - inBand0[index]) * 1000f / (inBand1[index] + inBand0[index]));
                            }
                        }
                        //else
                        //{
                        //    for (int i = 0; i < inBand0.Length; i++)
                        //    {
                        //        //第1个输出文件的第1个波段存储NDVI值
                        //        rvOutVistor[0].RasterBandsData[0][i] = (short)((inBand1[i] - inBand0[i]) * 1000f / (inBand1[i] + inBand0[i]));
                        //        //通道1和通道2的差值
                        //        //fileOutVistor[0].RasterBandsData[1][p] = (short)(inBand0[p] - inBand1[p]);
                        //    }
                        //}
                    }
                }));
                rfr.Excute();
            }
            finally
            {
                if (fileIns != null)
                {
                    for (int i = 0; i < fileIns.Length; i++)
                    {
                        fileIns[i].Raster.Dispose();
                    }
                }
                if (fileOuts != null)
                {
                    for (int i = 0; i < fileOuts.Length; i++)
                    {
                        fileOuts[i].Raster.Dispose();
                    }
                }
            }
            sw.Stop();
            Console.WriteLine((sw.ElapsedMilliseconds / 1000f).ToString() + "秒");
        }
    }
}
