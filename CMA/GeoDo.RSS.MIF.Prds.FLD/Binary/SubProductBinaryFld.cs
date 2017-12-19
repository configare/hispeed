using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public class SubProductBinaryFld : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductBinaryFld(SubProductDef subProductDef)
            : base(subProductDef)
        {
            _identify = subProductDef.Identify;
            _name = subProductDef.Name;
            if (subProductDef.Algorithms != null)
            {
                _algorithmDefs = subProductDef.Algorithms.ToList();
            }
        }
        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;
            string express = "";
            List<int> bandNos = new List<int>();
            if (_argumentProvider == null || _argumentProvider.DataProvider == null)
                return null;
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            object obj = _argumentProvider.GetArg("AlgorithmName");
            string argName = obj == null ? null : obj.ToString();
            if (string.IsNullOrEmpty(argName))
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            switch (argName)
            {
                case "SunDay1":
                    {
                        int bandNo = TryGetBandNo(bandNameRaster, "NearInfrared");
                        double nearInfraredZoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
                        //配置文件中设置为float类型，但有时为float类型，有时为double型
                        //double shortInfraredMin = (double)_argumentProvider.GetArg("ShortInfraredMin");
                        //double shortInfraredMax = (double)_argumentProvider.GetArg("ShortInfraredMax");
                        if (bandNo <= 0 || nearInfraredZoom == 0)
                        {
                            PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                            return null;
                        }
                        bandNos.Add(bandNo);
                        express = "(band" + bandNo + "/" + nearInfraredZoom + ">" + _argumentProvider.GetArg("ShortInfraredMin") + ")&&(band" + bandNo + "/" + nearInfraredZoom + "<" + _argumentProvider.GetArg("ShortInfraredMax") + ")";
                        break;
                    }
                case "SunDay2":
                    {
                        int visibleNo = TryGetBandNo(bandNameRaster, "Visible");
                        int nearInfraredNo = TryGetBandNo(bandNameRaster, "NearInfrared");
                        double visibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
                        double nearInfraredZoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
                        if (visibleNo <= 0 || nearInfraredNo <= 0 || visibleZoom == 0 || nearInfraredZoom == 0)
                        {
                            PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                            return null;
                        }
                        bandNos.Add(visibleNo);
                        bandNos.Add(nearInfraredNo);
                        express = "((band" + nearInfraredNo + "/" + nearInfraredZoom + "-band" + visibleNo + "/" + visibleZoom + ")<" + _argumentProvider.GetArg("NearInfraredVisibleMax")
                             + " && (band" + nearInfraredNo + "/" + nearInfraredZoom + ")<" + _argumentProvider.GetArg("NearInfraredMax")
                             + " && (band" + visibleNo + "/" + visibleZoom + ")<" + _argumentProvider.GetArg("VisibleMax")
                            + ")";
                        break;
                    }
                case "Night":
                    {
                        int nightFarInfraredNo = TryGetBandNo(bandNameRaster, "FarInfrared");
                        double nightFarInfraredZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");
                        if (nightFarInfraredNo <= 0)
                        {
                            PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                            return null;
                        }
                        bandNos.Add(nightFarInfraredNo);
                        express = "((band" + nightFarInfraredNo + "/" + nightFarInfraredZoom + ")>" + _argumentProvider.GetArg("FarInfraredMin") + @" &&
                                    (band" + nightFarInfraredNo + "/" + nightFarInfraredZoom + ")<" + _argumentProvider.GetArg("FarInfraredMax") + ")";
                        break;
                    }
                case "ThinCloud":
                    {
                        int visibleNo = TryGetBandNo(bandNameRaster, "Visible");
                        int nearInfraredNo = TryGetBandNo(bandNameRaster, "NearInfrared");
                        double visibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
                        double nearInfraredZoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
                        if (visibleNo <= 0 || nearInfraredNo <= 0 || visibleZoom == 0 || nearInfraredZoom == 0)
                        {
                            PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                            return null;
                        }
                        bandNos.Add(visibleNo);
                        bandNos.Add(nearInfraredNo);
                        express = "(band" + visibleNo + "==0)?false:((((float)band" + nearInfraredNo + "/" + nearInfraredZoom + ")/(band" + visibleNo + "/" + visibleZoom + ")>" + _argumentProvider.GetArg("NearInfraredVisibleMin") + ")&&(((float)band" + nearInfraredNo + "/" + nearInfraredZoom + ")/(band" + visibleNo + "/" + visibleZoom + ")<" + _argumentProvider.GetArg("NearInfraredVisibleMax") + "))";
                        break;
                    }
                case "Fog":
                    {
                        int middleInfraredNo = TryGetBandNo(bandNameRaster, "MiddleInfrared");
                        int farInfraredNo = TryGetBandNo(bandNameRaster, "FarInfrared");
                        double fogMiddleInfraredZoom = (double)_argumentProvider.GetArg("MiddleInfrared_Zoom");
                        double fogFarInfraredZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");
                        if (middleInfraredNo <= 0 || farInfraredNo <= 0 || fogFarInfraredZoom == 0 || fogMiddleInfraredZoom == 0)
                        {
                            PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                            return null;
                        }
                        object midFar = _argumentProvider.GetArg("MiddleInfraredFarInfrared");
                        float value = float.MinValue;
                        if (!float.TryParse(midFar.ToString(), out value))
                        {
                            PrintInfo("亮温差不在合法范围内,请重新获取。");
                            return null;
                        }
                        bandNos.Add(middleInfraredNo);
                        bandNos.Add(farInfraredNo);
                        express = "(band" + middleInfraredNo + "/" + fogMiddleInfraredZoom + "-band" + farInfraredNo + "/" + fogFarInfraredZoom + ")>" + value;
                        break;
                    }
                case "NDVI":
                    {
                        int visibleNo = TryGetBandNo(bandNameRaster, "Visible");
                        int nearInfraredNo = TryGetBandNo(bandNameRaster, "NearInfrared");
                        double ndviVisibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
                        double ndvitcNearInfraredZoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
                        if (visibleNo <= 0 || nearInfraredNo <= 0 || ndvitcNearInfraredZoom == 0 || ndvitcNearInfraredZoom == 0)
                        {
                            PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                            return null;
                        }
                        bandNos.Add(visibleNo);
                        bandNos.Add(nearInfraredNo);
                        express = "(band" + nearInfraredNo + "/" + ndvitcNearInfraredZoom + "+band" + visibleNo + "/" + ndviVisibleZoom + ")==0?false:((band" + nearInfraredNo + "/" + ndvitcNearInfraredZoom + "-band" + visibleNo + "/" + ndviVisibleZoom + ")" + " /(float)(band" + nearInfraredNo + "/" + ndvitcNearInfraredZoom + "+band" + visibleNo + "/" + ndviVisibleZoom + ")>" + _argumentProvider.GetArg("VisibleNearInfraredMin") + ")&&((band" + nearInfraredNo + "/" + ndvitcNearInfraredZoom + "-band" + visibleNo + "/" + ndviVisibleZoom + ")" + "/(float)(band" + nearInfraredNo + "/" + ndvitcNearInfraredZoom + "+band" + visibleNo + "/" + ndviVisibleZoom + ")<" + _argumentProvider.GetArg("VisibleNearInfraredMax") + ")";
                        break;
                    }
                case "GFProcess":
                    {
                        int bandNo = TryGetBandNo(bandNameRaster, "band");
                        double bandZoom = (double)_argumentProvider.GetArg("band_Zoom");
                        if (bandNo <= 0 || bandZoom <= 0)
                        {
                            PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                            return null;
                        }
                        bandNos.Add(bandNo);
                        express = "((band" + bandNo + "/" + bandZoom + ")>=" + _argumentProvider.GetArg("band1Min") + ")&&((band" + bandNo + "/" + bandZoom + ")<" + _argumentProvider.GetArg("band1Max") + ")";
                        break;
                    }
                default:
                    {
                        PrintInfo("指定的算法\"" + argName + "\"没有实现。");
                        return null;
                    }

            }
            IThresholdExtracter<UInt16> extracter = new SimpleThresholdExtracter<UInt16>();
            extracter.Reset(_argumentProvider, bandNos.ToArray(), express);
            IRasterDataProvider prd = _argumentProvider.DataProvider;
            IPixelIndexMapper resultFLD = PixelIndexMapperFactory.CreatePixelIndexMapper("FLD", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
            extracter.Extract(resultFLD);

            CreateStageAreaInfo();
            return resultFLD;
        }

        private void CreateStageAreaInfo()
        {
            RasterIdentify rs = new RasterIdentify(_argumentProvider.DataProvider.fileName);
            rs.SubProductIdentify = _identify;
            rs.ProductIdentify = _subProductDef.ProductDef.Identify;
            string outfilename = rs.ToWksFullFileName(".dat").Replace(".dat", ".txt");
            string[] Stage2Area = GetStringArray("Stage2Area");
            if (Stage2Area == null || Stage2Area.Length == 0)
                return;
            File.WriteAllLines(outfilename, Stage2Area, Encoding.Default);
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
