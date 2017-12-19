using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using GeoDo.RSS.MIF.Prds.Comm;
using System.Text.RegularExpressions;
using System.IO;
namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 积雪判识
    /// </summary>
    public class SubProductBinarySnw : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private static Regex DataReg = new Regex(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})", RegexOptions.Compiled);
        public SubProductBinarySnw(SubProductDef productDef)
            : base(productDef)
        {
            _identify = productDef.Identify;
            _isBinary = true;
            _algorithmDefs = new List<AlgorithmDef>(productDef.Algorithms);
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;
            if (_argumentProvider == null || _argumentProvider.DataProvider == null)
                return null;
            string algname = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (string.IsNullOrEmpty(algname))
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            //光学数据积雪判识
            if (algname == "SNWExtract")
            {
                int visiBandNo = (int)_argumentProvider.GetArg("Visible");
                int sIBandNo = (int)_argumentProvider.GetArg("ShortInfrared");
                int fIBandNo = (int)_argumentProvider.GetArg("FarInfrared");
                double visiBandZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
                double siBandZoom = (double)_argumentProvider.GetArg("ShortInfrared_Zoom");
                double fiBandZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");
                if (visiBandNo <= 0 || sIBandNo <= 0 || fIBandNo <= 0 || visiBandZoom <= 0 || siBandZoom <= 0 || fiBandZoom <= 0)
                {
                    PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                    return null;
                }
                string express = string.Format(@"(band{1}/{4}f > var_ShortInfraredMin) && (band{1}/{4}f< var_ShortInfraredMax) && 
                                (band{2}/{5}f< var_FarInfraredMax) && (band{2}/{5}f > var_FarInfraredMin) && (band{0}/{3}f> var_VisibleMin) && 
                                ((float)(band{0}/{3}f-band{1}/{4}f)/(band{0}/{3}f+band{1}/{4}f)> var_NDSIMin) && 
                                ((float)(band{0}/{3}f-band{1}/{4}f)/(band{0}/{3}f+band{1}/{4}f)< var_NDSIMax)&&
                                (band{0}/{3}f< var_VisibleMax)", visiBandNo, sIBandNo, fIBandNo, visiBandZoom, siBandZoom, fiBandZoom);
                int[] bandNos = new int[] { visiBandNo, sIBandNo, fIBandNo };
                IThresholdExtracter<UInt16> extracter = new SimpleThresholdExtracter<UInt16>();
                extracter.Reset(_argumentProvider, bandNos, express);
                int width = _argumentProvider.DataProvider.Width;
                int height = _argumentProvider.DataProvider.Height;
                IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("MWS", width, height, _argumentProvider.DataProvider.CoordEnvelope, _argumentProvider.DataProvider.SpatialRef);
                result.Tag = SnwDisplayInfo.GetDisplayInfo(_argumentProvider, visiBandNo, sIBandNo, fIBandNo);
                extracter.Extract(result);
                return result;
            }
            else
            {
                float[] extrParas = (float[])_argumentProvider.GetArg("Arguments");
                string filename = _argumentProvider.DataProvider.fileName;
                //解析文件时间，确定波段顺序
                Match m = DataReg.Match(Path.GetFileName(filename));
                string filedate = "";
                if (m.Success)
                {
                    filedate = m.Value;
                }
                Int32 filedateDig = Convert.ToInt32(filedate);
                int ch18vBandNo = 3;  //默认为现在使用的波段顺序
                int ch23vBandNo = 5;
                int ch36vBandNo = 7;
                int ch89vBandNo = 9;
                if (algname == "MSIdentify")
                {
                    if (filedateDig < 20110412)
                    {
                        ch18vBandNo = 7;
                        ch23vBandNo = 5;
                        ch36vBandNo = 3;
                        ch89vBandNo = 1;
                    }
                    if (ch18vBandNo <= 0 ||ch23vBandNo <= 0 || ch36vBandNo <= 0 || ch89vBandNo <= 0)
                    {
                        PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                        return null;
                    }
                    //string express = string.Format(@"(((((band{1}-band{3})*0.01>=var_23V89Vmin)||((band{0}-band{2})*0.01>= var_18V36Vmin))&&((band{1}*0.01+327.68)<=var_23Vmax)&&((band{0}-band{2})*0.01>=var_18Vsec36Vmin)&&((band{1}-band{3}-band{0}+band{2})*0.01>=var_si1si2thickmin))||((((band{1}-band{3})*0.01>var_23V89Vmin)||((band{0}-band{2})*0.01> var_18V36Vmin))&&((band{1}*0.01)+327.68<=var_23Vmax)&&((band{0}-band{2})*0.01<var_18Vsec36Vmin)&&((band{1}-band{3}-band{0}+band{2})*0.01>=var_si1thinsi2min)))", ch18vBandNo, ch23vBandNo, ch36vBandNo, ch89vBandNo);
                    string express = string.Format(@"(((((band{1}-band{3})*0.01>={4})||((band{0}-band{2})*0.01>= {5}))&&((band{1}*0.01+327.68)<={6})&&((band{0}-band{2})*0.01>={7})&&((band{1}-band{3}-band{0}+band{2})*0.01>={8}))||((((band{1}-band{3})*0.01>{4})||((band{0}-band{2})*0.01> {5}))&&((band{1}*0.01)+327.68<={6})&&((band{0}-band{2})*0.01<{7})&&((band{1}-band{3}-band{0}+band{2})*0.01>={9})))", ch18vBandNo, ch23vBandNo, ch36vBandNo, ch89vBandNo, extrParas[0], extrParas[1], extrParas[2], extrParas[3], extrParas[4], extrParas[5]);
                    int[] bandNos = new int[] { ch18vBandNo,ch23vBandNo, ch36vBandNo, ch89vBandNo };
                    IThresholdExtracter<Int16> extracter = new SimpleThresholdExtracter<Int16>();
                    extracter.Reset(_argumentProvider, bandNos, express);
                    int width = _argumentProvider.DataProvider.Width;
                    int height = _argumentProvider.DataProvider.Height;
                    IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("MWS", width, height, _argumentProvider.DataProvider.CoordEnvelope, _argumentProvider.DataProvider.SpatialRef);
                    //由于当数据过大（如全球范围数据时），分块缓存不足，因此注释掉。
                    //result.Tag = SnwDisplayInfo.GetDisplayInfoMS(_argumentProvider, ch18vBandNo, ch23vBandNo, ch36vBandNo, ch89vBandNo);
                    extracter.Extract(result);
                    return result;
                }
                else
                {
                    return null;
                }
            }
            
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }
}
