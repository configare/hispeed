using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.BlockOper;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.LST
{
    public class SubProductCYCALST : CmaMonitoringSubProduct
    {
        /*
         *分卫星 传感器
         *日信息：卫星_传感器_年_季_月_旬_日_时间起_时间止
         *旬信息: 卫星_传感器_年_季_月_旬
         *月信息：卫星_传感器_年_季_月
         *季信息：卫星_传感器_年_季
         *年信息：卫星_传感器_年
         *
         *周信息：卫星_传感器_年_月_周
         */
        private IContextMessage _contextMessage = null;

        public SubProductCYCALST()
            : base()
        {
        }

        public SubProductCYCALST(SubProductDef subProductDef)
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
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            string algorith = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (algorith == "CYCAAlgorithm")
                return CYCAAlgorithm(progressTracker);
            return null;
        }

        private IExtractResult CYCAAlgorithm(Action<int, string> progressTracker)
        {
            string[] files = null;
            Dictionary<string, string[]> pathDic = _argumentProvider.GetArg("FileSelectType") as Dictionary<string, string[]>;
            if (pathDic == null || pathDic.Count == 0)
            {
                PrintInfo("请点击\"确定\"按钮,以确定文件参数设置完毕!");
                return null;
            }
            if (pathDic.Keys.Contains("DirectoryPath")) //选择局地文件夹路径
                files = GetFiles(pathDic["DirectoryPath"][0]);
            else if (pathDic.Keys.Contains("FileNames")) //选择多个文件进行计算
                files = pathDic["FileNames"];
            if (files == null || files.Length == 0)
            {
                PrintInfo("待合成的数据文件不存在,请检查路径或文件设置!");
                return null;
            }
            RegionArg args = _argumentProvider.GetArg("RegionArgs") as RegionArg;
            if (args == null)
            {
                PrintInfo("请点击\"确定\"按钮,以确定周期参数设置完毕!");
                return null;
            }
            if (progressTracker != null)
                progressTracker.Invoke(10, "正在整理文件列表,请稍后...");
            CYCAFileListInfo cycaFileInfo = GeneralCYCAFileList.GeneralFileList(files, args, _subProductDef, "DBLV");
            if (cycaFileInfo == null || cycaFileInfo.OutFilename.Count == 0)
                return null;
            UInt16[] cloudValues = GetNanValues("CloudyValue");
            UInt16[] waterValues = GetNanValues("WaterValue");
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;//
            int bandNo = TryGetBandNo(bandNameRaster, "LstBand");

            IExtractResultArray array = new ExtractResultArray("LST");
            if (cycaFileInfo.Days001Files != null && cycaFileInfo.Days001Files.Count != 0)
            {
                if (progressTracker != null)
                    progressTracker.Invoke(25, "正在处理日数据,请稍后...");
                foreach (string key in cycaFileInfo.Days001Files.Keys)
                    array.Add(LSTDataProcessor.DataProcessor(progressTracker, _contextMessage, args.CK001Days, cycaFileInfo.Days001Files[key].ToArray(), bandNo, _subProductDef.Identify, cloudValues, waterValues, cycaFileInfo.OutFilename[key], args.CycType) as FileExtractResult);
            }
            if (cycaFileInfo.Days007Files != null && cycaFileInfo.Days007Files.Count != 0)
            {
                if (progressTracker != null)
                    progressTracker.Invoke(40, "正在处理周数据,请稍后...");
                foreach (string key in cycaFileInfo.Days007Files.Keys)
                    array.Add(LSTDataProcessor.DataProcessor(progressTracker, _contextMessage, args.CK007Days, cycaFileInfo.Days007Files[key].ToArray(), bandNo, _subProductDef.Identify, cloudValues, waterValues, cycaFileInfo.OutFilename[key], args.CycType) as FileExtractResult);
            }
            if (cycaFileInfo.Days010Files != null && cycaFileInfo.Days010Files.Count != 0)
            {
                if (progressTracker != null)
                    progressTracker.Invoke(55, "正在处理旬数据,请稍后...");
                foreach (string key in cycaFileInfo.Days010Files.Keys)
                    array.Add(LSTDataProcessor.DataProcessor(progressTracker, _contextMessage, args.CK010Days, cycaFileInfo.Days010Files[key].ToArray(), bandNo, _subProductDef.Identify, cloudValues, waterValues, cycaFileInfo.OutFilename[key], args.CycType) as FileExtractResult);
            }
            if (cycaFileInfo.Days030Files != null && cycaFileInfo.Days030Files.Count != 0)
            {
                if (progressTracker != null)
                    progressTracker.Invoke(70, "正在处理月数据,请稍后...");
                foreach (string key in cycaFileInfo.Days030Files.Keys)
                    array.Add(LSTDataProcessor.DataProcessor(progressTracker, _contextMessage, args.CK030Days, cycaFileInfo.Days030Files[key].ToArray(), bandNo, _subProductDef.Identify, cloudValues, waterValues, cycaFileInfo.OutFilename[key], args.CycType) as FileExtractResult);
            }
            if (cycaFileInfo.Days090Files != null && cycaFileInfo.Days090Files.Count != 0)
            {
                if (progressTracker != null)
                    progressTracker.Invoke(80, "正在处理季数据,请稍后...");
                foreach (string key in cycaFileInfo.Days090Files.Keys)
                    array.Add(LSTDataProcessor.DataProcessor(progressTracker, _contextMessage, args.CK090Days, cycaFileInfo.Days090Files[key].ToArray(), bandNo, _subProductDef.Identify, cloudValues, waterValues, cycaFileInfo.OutFilename[key], args.CycType) as FileExtractResult);
            }
            if (cycaFileInfo.Days365Files != null && cycaFileInfo.Days365Files.Count != 0)
            {
                if (progressTracker != null)
                    progressTracker.Invoke(90, "正在处理年数据,请稍后...");
                foreach (string key in cycaFileInfo.Days365Files.Keys)
                    array.Add(LSTDataProcessor.DataProcessor(progressTracker, _contextMessage, args.CK365Days, cycaFileInfo.Days365Files[key].ToArray(), bandNo, _subProductDef.Identify, cloudValues, waterValues, cycaFileInfo.OutFilename[key], args.CycType) as FileExtractResult);
            } return array;
        }

        private string[] GetFiles(string dirPath)
        {
            ObritFileFinder off = new ObritFileFinder();
            string extInfo = null;
            string[] files = null;
            if (string.IsNullOrEmpty(dirPath))
                return null;
            string flag = _argumentProvider.GetArg("FileDataIdentify").ToString();
            files = off.Find(string.Empty, ref extInfo, "Dir=" + dirPath + ",Flag=" + flag + ",Sort=asc,ProFlag=true,FindRegion=all");
            if (files == null || files.Length == 0)
            {
                PrintInfo("合成陆表高温的文件未提供!");
                return null;
            }
            return files;
        }

        private string[] GetFilesList(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                return null;
            string flag = _argumentProvider.GetArg("FileDataIdentify").ToString();
            string[] dirList = Directory.GetDirectories(path, flag, SearchOption.TopDirectoryOnly);
            if (dirList == null || dirList.Length == 0)
                return null;
            List<string> fileList = new List<string>();
            foreach (string item in dirList)
            {
                try
                {
                    fileList.AddRange(Directory.GetFiles(item, flag, SearchOption.AllDirectories).Where(
                        file => file.IndexOf("周期产品") == -1).ToArray());
                }
                catch (Exception ex)
                {
                }
            }
            return fileList.Count == 0 ? null : fileList.ToArray();
        }

        private RasterIdentify GetRasterIdentifyID(string[] fileNames)
        {
            RasterIdentify rst = new RasterIdentify(fileNames);
            rst.ProductIdentify = _subProductDef.ProductDef.Identify;
            rst.SubProductIdentify = _subProductDef.Identify;

            object obj = _argumentProvider.GetArg("OutFileIdentify");
            if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
                rst.SubProductIdentify = obj.ToString();

            rst.IsOutput2WorkspaceDir = true;
            return rst;
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        private bool IsNanValue(ushort pixelValue, ushort[] nanValues)
        {
            if (nanValues == null || nanValues.Length < 1)
            {
                return false;
            }
            else
            {
                foreach (short value in nanValues)
                {
                    if (pixelValue == value)
                        return true;
                }
            }
            return false;
        }

        private UInt16[] GetNanValues(string argumentName)
        {
            string nanValuestring = _argumentProvider.GetArg(argumentName) as string;
            if (!string.IsNullOrEmpty(nanValuestring))
            {
                string[] valueStrings = nanValuestring.Split(new char[] { ',', '，' });
                if (valueStrings != null && valueStrings.Length > 0)
                {
                    List<ushort> values = new List<ushort>();
                    ushort value;
                    for (int i = 0; i < valueStrings.Length; i++)
                    {
                        if (UInt16.TryParse(valueStrings[i], out value))
                            values.Add(value);
                    }
                    if (values.Count > 0)
                    {
                        return values.ToArray();
                    }
                }
            }
            return null;
        }
    }
}
