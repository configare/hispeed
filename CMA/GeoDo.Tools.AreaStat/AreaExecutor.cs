using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Drawing;

namespace GeoDo.Tools.AreaStat
{
    public class AreaExecutor
    {
        string _argFileName;
        Action<int, string> _progress;
        AreaStatArg _argument;

        public AreaExecutor(string argFileName, Action<int, string> progress)
        {
            _argFileName = argFileName;
            _progress = progress;
            _argument = new AreaStatArg(argFileName);
        }

        public void Execute()
        {
            CheckArg();
            //获取所有待统计数据文件
            string[] inputFiles = Directory.GetFiles(_argument.InputFileDir, "*.dat", SearchOption.AllDirectories);
            if (inputFiles == null || inputFiles.Length < 1)
                throw new FileNotFoundException("不存在符合条件的待统计文件!"); ;
            Compute(inputFiles, _argument.OutputFileName);
        }

        private void Compute(string[] inputFiles, string outFileName)
        {
            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            try
            {
                for (int i = 0; i < inputFiles.Length; i++)
                {
                    IRasterDataProvider inRaster = RasterDataDriver.Open(inputFiles[i]) as IRasterDataProvider;
                    if (inRaster.BandCount != 1)
                    {
                        inRaster.Dispose();
                        continue;
                    }
                    RasterMaper rm = new RasterMaper(inRaster, new int[] { 1 });
                    rms.Add(rm);
                }
                //输出文件准备（作为输入栅格并集处理）
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray(), _argument.OutResolution))
                {
                    if (_progress != null)
                        _progress(0, "开始进行面积统计");
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<short, short> rfr = null;
                    rfr = new RasterProcessModel<short, short>(_progress);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<short, short>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        foreach (RasterVirtualVistor<short> rv in rvInVistor)
                        {
                            short[] dt = rv.RasterBandsData[0];
                            if (dt != null)
                            {
                                for (int index = 0; index < dt.Length; index++)
                                {
                                    if (dt[index] == 1)
                                        rvOutVistor[0].RasterBandsData[0][index] = dt[index];
                                }
                            }
                        }
                    }));
                    //执行
                    rfr.Excute();
                    if (_progress != null)
                        _progress(100, "面积统计完成");
                }
            }
            finally
            {
                foreach (RasterMaper rm in rms)
                {
                    rm.Raster.Dispose();
                }
            }
        }

        private void CheckArg()
        {
            if (string.IsNullOrEmpty(_argument.InputFileDir))
                throw new ArgumentNullException("输入的文件夹路径为空!");
            if (!Directory.Exists(_argument.InputFileDir))
                throw new DirectoryNotFoundException("未发现指定输入路径" + _argument.InputFileDir);
            if (string.IsNullOrEmpty(_argument.OutputFileName))
                throw new ArgumentNullException("输出文件名为空!");
        }

        protected IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] inrasterMaper, float resolution)
        {
            string dir = Path.GetDirectoryName(outFileName);
            if (!string.IsNullOrEmpty(dir))
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Union(inRaster.Raster.CoordEnvelope);
            }
            float resX, resY;
            if (resolution != 0f)
            {
                resX = resolution;
                resY = resolution;
            }
            else
            {
                resX = inrasterMaper[0].Raster.ResolutionX;
                resY = inrasterMaper[0].Raster.ResolutionY;
                for(int i=1;i<inrasterMaper.Length;i++)
                {
                    if (resX > inrasterMaper[i].Raster.ResolutionX)
                        resX = inrasterMaper[i].Raster.ResolutionX;
                    if (resY > inrasterMaper[i].Raster.ResolutionY)
                        resY = inrasterMaper[i].Raster.ResolutionY;
                }
            }
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, enumDataType.Int16, mapInfo) as RasterDataProvider;
            return outRaster;
        }
    }
}
