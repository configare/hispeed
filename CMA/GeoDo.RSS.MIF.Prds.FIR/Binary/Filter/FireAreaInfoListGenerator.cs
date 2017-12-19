using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public class FireAreaInfoListGenerator : IDisposable
    {
        private IRasterDictionaryTemplate<byte> _landTypeDictionary = null;
        private IRasterDictionaryTemplate<int> _xianJieDictionary = null;

        public FireAreaInfoListGenerator()
        {
            _xianJieDictionary = RasterDictionaryTemplateFactory.CreateXjRasterTemplate();
            _landTypeDictionary = RasterDictionaryTemplateFactory.CreateLandRasterTemplate();
        }

        internal Dictionary<int, FireAreaFeature> GetFireArea(IArgumentProvider argProvider, IPixelIndexMapper pixelMapper, Dictionary<int, PixelFeature> features)
        {
            RasterIdentify rid = new RasterIdentify();
            Size size = new Size(argProvider.DataProvider.Width, argProvider.DataProvider.Height);
            string iir = string.Empty;
            using (InterestedRaster<int> result = new InterestedRaster<int>(rid, size, argProvider.DataProvider.CoordEnvelope))
            {
                foreach (int index in pixelMapper.Indexes)
                    result.Put(index, index);
                iir = result.FileName;
            }
            List<int> aoiIndexs = new List<int>();
            foreach (int keys in features.Keys)
            {
                if (features[keys].IsVertified)
                    aoiIndexs.Add(features[keys].PixelIndex);
            }
            int[] aoi = aoiIndexs.ToArray();
            Rectangle aoiRect = AOIHelper.ComputeAOIRect(aoi, size);
            IRasterDataProvider dataProvider = GeoDataDriver.Open(iir) as IRasterDataProvider;
            PixelWndAccessor<int> wndVisitor = new PixelWndAccessor<int>(dataProvider);
            wndVisitor.Reset(new int[] { 1 }, aoiRect);
            int fAreaNum = -1;
            Dictionary<int, FireAreaFeature> fireAreaFeatures = new Dictionary<int, FireAreaFeature>();
            foreach (int index in aoi)
            {
                if (features[index].FireAreaNum == -1)
                {
                    fAreaNum++;
                    features[index].FireAreaNum = fAreaNum;
                    fireAreaFeatures.Add(fAreaNum, GetFireAreaInfo(features[index], fAreaNum, argProvider.DataProvider));
                    GetFireAreaNum(wndVisitor, index, features, fAreaNum, fireAreaFeatures, argProvider.DataProvider);
                }
            }
            return fireAreaFeatures;
        }

        private void GetFireAreaNum(PixelWndAccessor<int> wndVisitor, int index, Dictionary<int, PixelFeature> features, int fAreaNum,
            Dictionary<int, FireAreaFeature> fireAreaFeatures, IRasterDataProvider prd)
        {
            int[][] wndBuffers = new int[1][];
            wndBuffers[0] = new int[9];
            if (wndVisitor.ReadWndPixels(index, 3, wndBuffers))
            {
                for (int i = 0; i < wndBuffers[0].Length; i++)
                {
                    if (wndBuffers[0][i] != 0)
                    {
                        if (features[wndBuffers[0][i]].FireAreaNum != -1)
                            continue;
                        else
                        {
                            features[wndBuffers[0][i]].FireAreaNum = features[index].FireAreaNum;
                            fireAreaFeatures[features[index].FireAreaNum] = UpdateFireAreaFeature(fireAreaFeatures[features[index].FireAreaNum], features[index], prd);
                            GetFireAreaNum(wndVisitor, wndBuffers[0][i], features, fAreaNum, fireAreaFeatures, prd);
                        }
                    }
                }
            }
        }

        private FireAreaFeature UpdateFireAreaFeature(FireAreaFeature faf, PixelFeature pixelFeature, IRasterDataProvider prd)
        {
            int row = pixelFeature.PixelIndex / prd.Width;
            int col = pixelFeature.PixelIndex - row * prd.Width;
            //faf.FireReaIndex = pixelFeature.FireAreaNum;
            //faf.Longitude = (float)(prd.CoordEnvelope.MinX + col * prd.ResolutionX);
            //faf.Latitude = (float)(prd.CoordEnvelope.MaxY - row * prd.ResolutionY);
            faf.FireArea += pixelFeature.PixelArea;
            faf.SecondryFireArea += pixelFeature.SecondPixelArea;
            //faf.XJName = _xianJieDictionary.GetPixelName(faf.Longitude, faf.Latitude);
            faf.FireCount += 1;
            string solidType = _landTypeDictionary.GetPixelName(faf.Longitude, faf.Latitude);
            if (solidType.IndexOf("草地") != -1)
                faf.GrasslandCount += 1;
            else if (solidType.IndexOf("林地") != -1)
                faf.WoodlandCount += 1;
            else if (solidType.IndexOf("耕地") != -1)
                faf.FarmlandCount += 1;
            else
                faf.OtherCount += 1;
            faf.FarmlandPercent = (float)faf.FarmlandCount / faf.FireCount;
            faf.WoodlandPercent = (float)faf.WoodlandCount / faf.FireCount;
            faf.GrasslandPercent = (float)faf.GrasslandCount / faf.FireCount;
            faf.OtherPercent = (float)faf.OtherCount / faf.FireCount;
            //环保部新增字段
            if (faf.FireIndeies == null)
                faf.FireIndeies = new List<int>();
            if (!faf.FireIndeies.Contains(pixelFeature.PixelIndex))
                faf.FireIndeies.Add(pixelFeature.PixelIndex);
            int code = _xianJieDictionary.GetCode(faf.Longitude, faf.Latitude);
            faf.SJName = _xianJieDictionary.GetPixelName((int)(Math.Floor(code / 10000f) * 10000));
            faf.ShiName = _xianJieDictionary.GetPixelName((int)(Math.Floor(code / 100f) * 100));
            if (!string.IsNullOrEmpty(faf.ShiName))
                faf.ShiName = string.IsNullOrEmpty(faf.SJName) ? faf.ShiName : faf.ShiName.Replace(faf.SJName, ""); return faf;
        }

        private FireAreaFeature GetFireAreaInfo(PixelFeature pixelFeature, int fAreaNum, IRasterDataProvider prd)
        {
            FireAreaFeature faf = new FireAreaFeature();
            int row = pixelFeature.PixelIndex / prd.Width;
            int col = pixelFeature.PixelIndex - row * prd.Width;
            faf.FireReaIndex = fAreaNum;
            faf.Longitude = (float)(prd.CoordEnvelope.MinX + col * prd.ResolutionX);
            faf.Latitude = (float)(prd.CoordEnvelope.MaxY - row * prd.ResolutionY);
            faf.FireArea = pixelFeature.PixelArea;
            faf.SecondryFireArea = pixelFeature.SecondPixelArea;
            faf.XJName = _xianJieDictionary.GetPixelName(faf.Longitude, faf.Latitude);
            faf.FireCount = 1;
            string solidType = _landTypeDictionary.GetPixelName(faf.Longitude, faf.Latitude);
            if (solidType.IndexOf("草地") != -1)
                faf.GrasslandCount = 1;
            else if (solidType.IndexOf("林地") != -1)
                faf.WoodlandCount = 1;
            else if (solidType.IndexOf("耕地") != -1)
                faf.FarmlandCount = 1;
            else
                faf.OtherCount = 1;
            faf.FarmlandPercent = (float)faf.FarmlandCount / faf.FireCount;
            faf.WoodlandPercent = (float)faf.WoodlandCount / faf.FireCount;
            faf.GrasslandPercent = (float)faf.GrasslandCount / faf.FireCount;
            faf.OtherPercent = (float)faf.OtherCount / faf.FireCount;
            //环保部新增字段
            if (faf.FireIndeies == null)
                faf.FireIndeies = new List<int>();
            if (!faf.FireIndeies.Contains(pixelFeature.PixelIndex))
                faf.FireIndeies.Add(pixelFeature.PixelIndex);
            int code = _xianJieDictionary.GetCode(faf.Longitude, faf.Latitude);
            faf.SJName = _xianJieDictionary.GetPixelName((int)(Math.Floor(code / 10000f) * 10000));
            faf.ShiName = _xianJieDictionary.GetPixelName((int)(Math.Floor(code / 100f) * 100));
            if (!string.IsNullOrEmpty(faf.ShiName))
                faf.ShiName = string.IsNullOrEmpty(faf.SJName) ? faf.ShiName : faf.ShiName.Replace(faf.SJName, "");
            return faf;
        }

        /// <summary>
        /// 生成Txt火区信息列表
        /// </summary>
        /// <param name="argProvider"></param>
        /// <param name="features"></param>
        /// <returns></returns>
        /*
        internal IFileExtractResult Generator(IArgumentProvider argProvider, Dictionary<int, FireAreaFeature> features)
        {
            DateTime orbitDateTime = DateTime.MinValue;
            string saveFilename = GetFALTFilename(argProvider, out orbitDateTime);
            using (StreamWriter sw = new StreamWriter(saveFilename, false, Encoding.UTF8))
            {
                foreach (FireSubareaColumnHeader item in FireSubareaColumnHeaderDef.FireSubarea)
                {
                    sw.Write(item.Caption);
                    sw.Write("\t");
                }
                sw.WriteLine();
                for (int i = 0; i < features.Count; i++)
                {
                    sw.Write(features[i].FireReaIndex);
                    sw.Write("\t");
                    sw.Write(DegreeConvert(features[i].Longitude));
                    sw.Write("\t");
                    sw.Write(DegreeConvert(features[i].Latitude));
                    sw.Write("\t");
                    sw.Write(features[i].FireCount.ToString("#").PadRight(10));
                    sw.Write("\t");
                    sw.Write(features[i].FireArea.ToString("#0.000").PadRight(10));
                    sw.Write("\t");
                    sw.Write(features[i].SecondryFireArea.ToString("#0.000").PadRight(10));
                    sw.Write("\t");
                    sw.Write(string.IsNullOrEmpty(features[i].XJName) ? @"\".PadRight(30) : features[i].XJName.PadRight(30));
                    sw.Write("\t");
                    sw.Write(features[i].WoodlandPercent == 0 ? @"\".PadRight(20) : (features[i].WoodlandPercent * 100 + "%").PadRight(10));
                    sw.Write("\t");
                    sw.Write(features[i].GrasslandPercent == 0 ? @"\".PadRight(20) : (features[i].GrasslandPercent * 100 + "%").PadRight(10));
                    sw.Write("\t");
                    sw.Write(features[i].FarmlandPercent == 0 ? @"\".PadRight(20) : (features[i].FarmlandPercent * 100 + "%").PadRight(10));
                    sw.Write("\t");
                    sw.Write(features[i].OtherPercent == 0 ? @"\".PadRight(20) : (features[i].OtherPercent * 100 + "%").PadRight(10));
                    sw.Write("\t");
                    sw.WriteLine();
                }
            }
            IFileExtractResult result = new FileExtractResult("FRIL", saveFilename);
            result.SetDispaly(false);
            return result;
        }
        */

        /// <summary>
        /// 生成Excel火区信息列表
        /// </summary>
        /// <param name="argProvider"></param>
        /// <param name="features"></param>
        /// <returns></returns>
        internal IFileExtractResult Generator(IArgumentProvider argProvider, Dictionary<int, FireAreaFeature> features)
        {
            DateTime orbitDateTime = DateTime.MinValue;
            string saveFilename = GetFALTFilename(argProvider, out orbitDateTime);
            List<string[]> excelInfos = new List<string[]>();

            string[] columns = new string[] { "火区号", "中心经度", "中心纬度", "火点像元个数", "像元覆盖面积（平方公里）", "明火面积(公顷)", "省地县", "林地", "草地", "农田", "其他" };
            List<string> listTemp = new List<string>();
            for (int i = 0; i < features.Count; i++)
            {
                listTemp.Add((features[i].FireReaIndex + 1).ToString());
                listTemp.Add(features[i].Longitude.ToString());
                listTemp.Add(features[i].Latitude.ToString());
                listTemp.Add(features[i].FireCount.ToString("#"));
                listTemp.Add(features[i].FireArea.ToString("#0.000"));
                listTemp.Add(features[i].SecondryFireArea.ToString("#0.000"));
                listTemp.Add(string.IsNullOrEmpty(features[i].XJName) ? @"\" : features[i].XJName);
                listTemp.Add(features[i].WoodlandPercent == 0 ? @"\" : (features[i].WoodlandPercent * 100 + "%"));
                listTemp.Add(features[i].GrasslandPercent == 0 ? @"\" : (features[i].GrasslandPercent * 100 + "%"));
                listTemp.Add(features[i].FarmlandPercent == 0 ? @"\" : (features[i].FarmlandPercent * 100 + "%"));
                listTemp.Add(features[i].OtherPercent == 0 ? @"\" : (features[i].OtherPercent * 100 + "%"));
                excelInfos.Add(listTemp.ToArray());
                listTemp.Clear();
            }
            IStatResult result = new StatResult("火区信息统计结果", columns, excelInfos.ToArray());
            using (StatResultToExcelFile excelControl = new StatResultToExcelFile())
            {
                excelControl.Init();
                excelControl.Add(true, "火区信息数据统计", result, false, 0);
                excelControl.SaveFile(saveFilename);
            }
            IFileExtractResult resultFile = new FileExtractResult("FRIL", saveFilename);
            resultFile.SetDispaly(false);
            return resultFile;
        }
        internal IFileExtractResult GeneratorKB(IArgumentProvider argProvider, Dictionary<int, FireAreaFeature> features)
        {
            DateTime orbitDateTime = DateTime.MinValue;
            string saveFilename = GetFALTFilenameKB(argProvider, out orbitDateTime);
            List<string[]> excelInfos = new List<string[]>();

            string[] columns = new string[] { "火区号", "中心经度", "中心纬度", "火点像元个数", "像元覆盖面积（平方公里）", "明火面积(公顷)", "省地县", "林地", "草地", "农田", "其他" };
            List<string> listTemp = new List<string>();
            for (int i = 0; i < features.Count; i++)
            {
                listTemp.Add((features[i].FireReaIndex + 1).ToString());
                listTemp.Add(features[i].Longitude.ToString());
                listTemp.Add(features[i].Latitude.ToString());
                listTemp.Add(features[i].FireCount.ToString("#"));
                listTemp.Add(features[i].FireArea.ToString("#0.000"));
                listTemp.Add(features[i].SecondryFireArea.ToString("#0.000"));
                listTemp.Add(string.IsNullOrEmpty(features[i].XJName) ? @"\" : features[i].XJName);
                listTemp.Add(features[i].WoodlandPercent == 0 ? @"\" : (features[i].WoodlandPercent * 100 + "%"));
                listTemp.Add(features[i].GrasslandPercent == 0 ? @"\" : (features[i].GrasslandPercent * 100 + "%"));
                listTemp.Add(features[i].FarmlandPercent == 0 ? @"\" : (features[i].FarmlandPercent * 100 + "%"));
                listTemp.Add(features[i].OtherPercent == 0 ? @"\" : (features[i].OtherPercent * 100 + "%"));
                excelInfos.Add(listTemp.ToArray());
                listTemp.Clear();
            }
            IStatResult result = new StatResult("火区信息统计结果", columns, excelInfos.ToArray());
            using (StatResultToExcelFile excelControl = new StatResultToExcelFile())
            {
                excelControl.Init();
                excelControl.Add(true, "火区信息数据统计", result, false, 0);
                excelControl.WinExcelControl.SetCellValue(2, 2, 2, 12, 0, masExcelAlignType.Center, "火情信息快报", null);
                string timestring = string.Format("时间：{0}（北京时）", orbitDateTime.AddHours(8).ToString("yyyy年MM月dd日 HH:mm"));
                excelControl.WinExcelControl.SetCellValue(3, 2, 3, 6, 0, masExcelAlignType.Left, timestring, null);
                excelControl.WinExcelControl.SetCellValue(3, 7, 3, 12, 0, masExcelAlignType.Right, "国家卫星气象中心", null);
                excelControl.WinExcelControl.SetCellValue(4, 2, 4, 12, 1, masExcelAlignType.Center, "火区信息数据统计", null);
                excelControl.SaveFile(saveFilename);
            }
            IFileExtractResult resultFile = new FileExtractResult("FRIK", saveFilename);
            resultFile.SetDispaly(false);
            return resultFile;
        }

        private string DegreeConvert(float degree)
        {
            int number = (int)degree;
            float fraction = degree - number;
            int minute = (int)(fraction * 60);
            fraction = fraction * 60 - minute;
            int second = (int)(fraction * 60);
            return number.ToString() + "\u00B0" + minute.ToString() + "\u2032" + second.ToString() + "\u2033";
        }

        private string GetFALTFilename(IArgumentProvider argProvider, out DateTime orbitDateTime)
        {
            RasterIdentify id = new RasterIdentify(argProvider.DataProvider.fileName);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FIR";
            id.SubProductIdentify = "FRIL";
            id.IsOutput2WorkspaceDir = true;
            DataIdentify dataId = argProvider.DataProvider.DataIdentify;
            if (dataId == null)
                dataId = new DataIdentify();
            id.Satellite = dataId.Satellite ?? "Unknow";
            id.Sensor = dataId.Sensor ?? "Unknow";
            id.OrbitDateTime = dataId.OrbitDateTime;
            orbitDateTime = id.OrbitDateTime;
            return id.ToWksFullFileName(".xlsx");
        }

        public IFileExtractResult ExportILSTToExcel(string saveFileName)
        {
            string[] lineInfos = File.ReadAllLines(saveFileName, Encoding.Default);
            List<string[]> excelInfos = new List<string[]>();

            string[] columns = new string[] { "火区号", "中心经度", "中心纬度", "火点像元个数", "像元覆盖面积（平方公里）", "明火面积(公顷)", "省地县", "林地", "草地", "农田", "其他" };
            for (int i = 1; i < lineInfos.Length; i++)
            {
                excelInfos.Add(lineInfos[i].Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries));
            }
            IStatResult result = new StatResult("火区信息统计结果", columns, excelInfos.ToArray());
            using (StatResultToExcelFile excelControl = new StatResultToExcelFile())
            {
                excelControl.Init();
                excelControl.Add(true, "火区信息数据统计", result, false, 0);
                saveFileName = Path.Combine(Path.GetDirectoryName(saveFileName), Path.GetFileNameWithoutExtension(saveFileName) + ".XLSX");
                excelControl.SaveFile(saveFileName);
            }
            IFileExtractResult resultFile = new FileExtractResult("FRIL", saveFileName);
            resultFile.SetDispaly(false);
            return resultFile;
        }

        public void Dispose()
        {
            if (_landTypeDictionary != null)
            {
                _landTypeDictionary.Dispose();
                _landTypeDictionary = null;
                _xianJieDictionary.Dispose();
                _xianJieDictionary = null;
            }
        }

        /// <summary>
        /// 生成Excel环保部信息列表
        /// </summary>
        /// <param name="argProvider"></param>
        /// <param name="features"></param>
        /// <returns></returns>
        internal IFileExtractResult GeneratorHB(IArgumentProvider argProvider, Dictionary<int, FireAreaFeature> features)
        {
            DateTime orbitDateTime = DateTime.MinValue;
            string saveFilename = GetFALTFilenameHB(argProvider, out orbitDateTime);
            List<string[]> excelInfos = new List<string[]>();

            string[] columns = new string[] { "省", "地区", "县", "中心纬度", "中心经度", "火点像素数", "禁烧区", "明火面积(公顷)" };
            List<string> listTemp = new List<string>();

            SortedDictionary<string, StatAreaItem> statResult;
            RasterStatByVector<Int16> stat = new RasterStatByVector<Int16>(null);
            List<int> inRegionIndex = new List<int>();
            statResult = stat.CountByVector(argProvider.DataProvider, "禁烧区.shp", "JS",
                                    (cur, idx, cursum) =>
                                    {
                                        if (cur != 0)
                                            inRegionIndex.Add(idx);
                                        return cursum += cur;
                                    });
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].FarmlandPercent == 0)
                    continue;
                listTemp.Add(string.IsNullOrEmpty(features[i].SJName) ? @"\" : features[i].SJName);
                listTemp.Add(string.IsNullOrEmpty(features[i].ShiName) ? @"\" : features[i].ShiName);
                if (string.IsNullOrEmpty(features[i].XJName))

                    listTemp.Add(@"\");
                else if (!string.IsNullOrEmpty(features[i].SJName + features[i].ShiName))
                    listTemp.Add(features[i].XJName.Replace(features[i].SJName + features[i].ShiName, ""));
                else
                    listTemp.Add(features[i].XJName);
                listTemp.Add(features[i].Latitude.ToString());
                listTemp.Add(features[i].Longitude.ToString());
                listTemp.Add(features[i].FireCount.ToString("#"));
                listTemp.Add(GetHBInfos(features[i].FireIndeies, inRegionIndex));
                listTemp.Add(features[i].SecondryFireArea.ToString("#0.000"));
                excelInfos.Add(listTemp.ToArray());
                listTemp.Clear();
            }
            IStatResult result = new StatResult("环保部信息统计结果", columns, excelInfos.ToArray());
            using (StatResultToExcelFile excelControl = new StatResultToExcelFile())
            {
                excelControl.Init();
                excelControl.Add(true, "环保部信息数据统计", result, false, 0);
                excelControl.SaveFile(saveFilename);
            }
            IFileExtractResult resultFile = new FileExtractResult("FRIH", saveFilename);
            resultFile.SetDispaly(false);
            return resultFile;
        }

        private string GetHBInfos(List<int> fireIndeies, List<int> inRegionIndex)
        {
            foreach (int item in fireIndeies)
            {
                if (inRegionIndex.Contains(item))
                    return "是";
            }
            return "否";
        }

        private string GetFALTFilenameHB(IArgumentProvider argProvider, out DateTime orbitDateTime)
        {
            RasterIdentify id = new RasterIdentify(argProvider.DataProvider.fileName);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FIR";
            id.SubProductIdentify = "FRIH";
            id.IsOutput2WorkspaceDir = true;
            DataIdentify dataId = argProvider.DataProvider.DataIdentify;
            if (dataId == null)
                dataId = new DataIdentify();
            id.Satellite = dataId.Satellite ?? "Unknow";
            id.Sensor = dataId.Sensor ?? "Unknow";
            id.OrbitDateTime = dataId.OrbitDateTime;
            orbitDateTime = id.OrbitDateTime;
            return id.ToWksFullFileName(".xlsx");
        }

        private string GetFALTFilenameKB(IArgumentProvider argProvider, out DateTime orbitDateTime)
        {
            RasterIdentify id = new RasterIdentify(argProvider.DataProvider.fileName);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FIR";
            id.SubProductIdentify = "FRIK";
            id.IsOutput2WorkspaceDir = true;
            DataIdentify dataId = argProvider.DataProvider.DataIdentify;
            if (dataId == null)
                dataId = new DataIdentify();
            id.Satellite = dataId.Satellite ?? "Unknow";
            id.Sensor = dataId.Sensor ?? "Unknow";
            id.OrbitDateTime = dataId.OrbitDateTime;
            orbitDateTime = id.OrbitDateTime;
            return id.ToWksFullFileName(".xlsx");
        }
    }
}
