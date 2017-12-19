using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public class MultiFileComputer
    {
        private IArgumentProvider _argumentProvider = null;
        private string _orbitFilesDirPath = null;
        private IContextMessage _contextMessage = null;
        private string _identify = null;
        private ComputeArgument _arg = null;

        public MultiFileComputer(IArgumentProvider argumentProvider, string defaultDirPath,
                                 IContextMessage contextMessage, string identify, ComputeArgument arg)
        {
            _argumentProvider = argumentProvider;
            _orbitFilesDirPath = defaultDirPath;
            _contextMessage = contextMessage;
            _identify = identify;
            _arg = arg;
        }

        //
        public IExtractResult ComputeByCurrentRaster(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null || _argumentProvider.DataProvider == null)
                return null;
            if (_arg == null)
                return null;
            int[] bandNos = _arg.BandNos;
            string express = _arg.Express;
            IRasterExtracter<UInt16, Int16> extracter = new SimpleRasterExtracter<UInt16, Int16>();

            extracter.Reset(_argumentProvider, bandNos, express);
            IRasterDataProvider prd = _argumentProvider.DataProvider;
            IPixelFeatureMapper<Int16> result = new MemPixelFeatureMapper<Int16>(_identify, prd.Width * prd.Height, new System.Drawing.Size(prd.Width, prd.Height), prd.CoordEnvelope, prd.SpatialRef);
            extracter.Extract(result);
            return result;
        }

        public IExtractResult ComputeByDirPath(string dirPath)
        {
            if (!Directory.Exists(dirPath))
                return null;
            string[] fnames = GetFilesFromDirPath(dirPath);
            if (fnames == null)
                fnames = GetFilesFromDirPath(_orbitFilesDirPath);
            if (fnames == null || fnames.Length == 0)
            {
                PrintInfo("请选择正确的局地文件路径进行NDVI计算。");
                return null;
            }
            return ComputeByFiles(fnames);
        }

        public IExtractResult ComputeByFiles(string[] fnames)
        {
            if (_arg == null)
                return null;
            IExtractResultArray resultAry = new ExtractResultArray(_identify);
            IRasterExtracter<UInt16, Int16> extracter = new SimpleRasterExtracter<UInt16, Int16>();

            foreach (string fname in fnames)
            {
                if (!IsSameTypeFile(fname))
                    continue;
                IExtractResultBase result = ComputeSingleFile(fname, extracter, _arg.BandNos, _arg.Express);
                if (result != null)
                    resultAry.Add(result);
            }
            return resultAry;
        }

        bool _isFirst = true;
        string _satellite = null;
        string _sensor = null;
        int _bandCount = 0;
        /// <summary>
        /// 对选中的多个文件进行限制，只对同类型的文件（卫星、传感器、波段数相同）的文件进行计算
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool IsSameTypeFile(string fileName)
        {
            using (IRasterDataProvider prd = GeoDataDriver.Open(fileName) as IRasterDataProvider)
            {
                if (_isFirst)
                {
                    _satellite = prd.DataIdentify.Satellite;
                    _sensor = prd.DataIdentify.Sensor;
                    _bandCount = prd.BandCount;
                    _isFirst = false;
                    return true;
                }
                else
                {
                    if (prd.DataIdentify.Satellite != _satellite || prd.DataIdentify.Sensor != _sensor
                     || prd.BandCount != _bandCount)
                        return false;
                    else
                        return true;
                }
            }
        }

        private IExtractResultBase ComputeSingleFile(string fname, IRasterExtracter<ushort, short> extracter, int[] bandNos, string express)
        {
            using (IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider)
            {
                if (prd == null)
                    return null;
                int bandCount = prd.BandCount;
                foreach (int band in bandNos)
                {
                    if (bandCount == 0 || bandCount == 1)
                    {
                        PrintInfo("请选择正确的局地文件进行计算。");
                        return null;
                    }
                    if (bandCount < band)
                    {
                        PrintInfo("获取波段序号失败，可能是波段映射表配置错误或判识算法波段参数配置错误！");
                        return null;
                    }
                }
                IArgumentProvider aPrd = new ArgumentProvider(prd, null);
                extracter.Reset(aPrd, bandNos, express);
                string filename = string.Empty;
                using (IPixelFeatureMapper<Int16> resultNDVI = new MemPixelFeatureMapper<Int16>(_identify, prd.Width * prd.Height, new System.Drawing.Size(prd.Width, prd.Height), prd.CoordEnvelope, prd.SpatialRef))
                {
                    extracter.Extract(resultNDVI);
                    RasterIdentify rid = GetRasterIdentify(fname);
                    using (InterestedRaster<Int16> iir = new InterestedRaster<Int16>(rid, new System.Drawing.Size(prd.Width, prd.Height), prd.CoordEnvelope))
                    {
                        iir.Put(resultNDVI);
                        filename = iir.FileName;
                    }
                    IFileExtractResult ndviResult = new FileExtractResult(_identify, filename);
                    ndviResult.SetDispaly(false);
                    return ndviResult;
                }
            }
        }

        private string[] GetFilesFromDirPath(string dir)
        {
            if (!Directory.Exists(dir))
                return null;
            string[] fnames = Directory.GetFiles(dir);
            if (fnames == null || fnames.Length == 0)
                return null;
            List<string> fnameList = new List<string>();
            foreach (string fname in fnames)
            {
                if (Path.GetExtension(fname).ToLower() == ".ldf" || Path.GetExtension(fname).ToLower() == ".ldff"
                 || Path.GetExtension(fname).ToLower() == ".ld2" || Path.GetExtension(fname).ToLower() == ".ld3")
                    fnameList.Add(fname);
            }
            return fnameList.Count != 0 ? fnameList.ToArray() : fnameList.ToArray();
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        private RasterIdentify GetRasterIdentify(string fname)
        {
            RasterIdentify result = new RasterIdentify(fname.ToUpper());
            result.ThemeIdentify = "CMA";
            result.ProductIdentify = "VGT";
            result.SubProductIdentify = _identify;
            result.IsOutput2WorkspaceDir = true;
            return result;
        }
    }
}
