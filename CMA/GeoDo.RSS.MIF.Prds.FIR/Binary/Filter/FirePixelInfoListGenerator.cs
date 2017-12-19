using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using GeoDo.RSS.Core.DF;
using System.IO;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public class FirePixelInfoListGenerator : IDisposable
    {
        private IRasterDictionaryTemplate<byte> _landTypeDictionary = null;
        private IRasterDictionaryTemplate<int> _xianJieDictionary = null;

        public FirePixelInfoListGenerator()
        {
            _xianJieDictionary = RasterDictionaryTemplateFactory.CreateXjRasterTemplate();
            _landTypeDictionary = RasterDictionaryTemplateFactory.CreateLandRasterTemplate();
        }

        internal IFileExtractResult Generator(IArgumentProvider argProvider, Dictionary<int, PixelFeature> features)
        {
            if (argProvider.DataProvider == null || features == null || features.Count == 0)
                return null;
            DateTime orbitDateTime = DateTime.MinValue;
            string saveFilename = GetPLSTFilename(argProvider, out orbitDateTime);
            CoordEnvelope envelope = argProvider.DataProvider.CoordEnvelope;
            float resolutionX = argProvider.DataProvider.ResolutionX;
            float resolutionY = argProvider.DataProvider.ResolutionY;
            int sample = argProvider.DataProvider.Width;
            int line = argProvider.DataProvider.Height;
            int row, col, index = 0;
            double longitude, latitude;
            using (StreamWriter sw = new StreamWriter(saveFilename, false, Encoding.Default))
            {

                foreach (int fet in features.Keys)
                {
                    if (features[fet].IsVertified)
                    {
                        row = features[fet].PixelIndex / sample;
                        col = features[fet].PixelIndex - row * sample;
                        longitude = envelope.MinX + col * resolutionX;
                        latitude = envelope.MaxY - row * resolutionY;
                        //0
                        sw.Write((index++).ToString().PadLeft(7, ' '));
                        sw.Write("\t");
                        //1
                        sw.Write(latitude.ToString("0.00").PadLeft(6, ' '));
                        sw.Write("\t");
                        //2
                        sw.Write(longitude.ToString("0.00").PadLeft(7, ' '));
                        sw.Write("\t");
                        //3
                        sw.Write(features[fet].FireAreaNum.ToString().PadLeft(7, ' '));
                        sw.Write("\t");
                        //4
                        int xianJie = _xianJieDictionary.GetCode(longitude, latitude);
                        if (xianJie != default(int))
                        {
                            sw.Write(xianJie.ToString().PadLeft(6, ' '));
                        }
                        else
                        {
                            //sw.Write("-9999".PadLeft(6, ' '));
                            sw.Write("0".PadLeft(6, ' '));
                        }
                        sw.Write("\t");
                        //5
                        byte landUse = _landTypeDictionary.GetCode(longitude, latitude);
                        if (landUse != default(byte))
                        {
                            sw.Write(landUse.ToString().PadLeft(12, ' '));
                        }
                        else
                        {
                            //sw.Write("-9999".PadLeft(12, ' '));
                            sw.Write("0".PadLeft(12, ' '));
                        }
                        sw.Write("\t");
                        //6
                        sw.Write(features[fet].PixelArea.ToString("#0.000000").PadLeft(10, ' '));
                        sw.Write("\t");
                        //7
                        if (features[fet].PixelArea == 0)
                            sw.Write("0.0000".PadLeft(6, ' '));
                        else
                            sw.Write((features[fet].SecondPixelArea / (features[fet].PixelArea * 100) * 100).ToString("#0.0000").PadLeft(6, ' '));
                        sw.Write("\t");
                        //8
                        sw.Write("750.00".PadLeft(8, ' '));
                        sw.Write("\t");
                        //9
                        sw.Write(orbitDateTime.ToString("yyyy-MM-dd HH:mm").PadLeft(16, ' '));
                        sw.Write("\t");
                        //10
                        sw.Write(features[fet].FireIntensityGrade.ToString().PadLeft(8, ' '));
                        sw.Write("\t");
                        //11
                        //sw.Write(features[fet].FirIntensity.ToString("#.########").PadLeft(16, ' '));
                        sw.Write(features[fet].FirIntensity.ToString("0.0000000").PadLeft(16, ' '));
                        sw.WriteLine();
                    }
                }
            }
            if (File.Exists(saveFilename))
            {
                IFileExtractResult result = new FileExtractResult("PLST", saveFilename);
                result.SetDispaly(false);
                return result;
            }
            return null;
        }

        internal IFileExtractResult GeneratorHB(IArgumentProvider argProvider, Dictionary<int, PixelFeature> features)
        {
            if (argProvider.DataProvider == null || features == null || features.Count == 0)
                return null;
            DateTime orbitDateTime = DateTime.MinValue;
            string saveFilename = GetPLSFFilename(argProvider, out orbitDateTime);
            CoordEnvelope envelope = argProvider.DataProvider.CoordEnvelope;
            float resolutionX = argProvider.DataProvider.ResolutionX;
            float resolutionY = argProvider.DataProvider.ResolutionY;
            int sample = argProvider.DataProvider.Width;
            int line = argProvider.DataProvider.Height;
            int row, col, index = 0;
            double longitude, latitude;
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
            using (StreamWriter sw = new StreamWriter(saveFilename, false, Encoding.Default))
            {
                foreach (int fet in features.Keys)
                {
                    if (features[fet].IsVertified)
                    {
                        row = features[fet].PixelIndex / sample;
                        col = features[fet].PixelIndex - row * sample;
                        longitude = envelope.MinX + col * resolutionX;
                        latitude = envelope.MaxY - row * resolutionY;
                        //0
                        sw.Write((index++).ToString().PadLeft(7, ' '));
                        sw.Write("\t");
                        //1
                        sw.Write(latitude.ToString("0.00").PadLeft(6, ' '));
                        sw.Write("\t");
                        //2
                        sw.Write(longitude.ToString("0.00").PadLeft(7, ' '));
                        sw.Write("\t");
                        //3
                        sw.Write(features[fet].FireAreaNum.ToString().PadLeft(7, ' '));
                        sw.Write("\t");
                        //4
                        int xianJie = _xianJieDictionary.GetCode(longitude, latitude);
                        if (xianJie != default(int))
                        {
                            sw.Write(xianJie.ToString().PadLeft(6, ' '));
                        }
                        else
                        {
                            //sw.Write("-9999".PadLeft(6, ' '));
                            sw.Write("0".PadLeft(6, ' '));
                        }
                        sw.Write("\t");
                        //5
                        byte landUse = _landTypeDictionary.GetCode(longitude, latitude);
                        if (landUse != default(byte))
                        {
                            sw.Write(landUse.ToString().PadLeft(12, ' '));
                        }
                        else
                        {
                            //sw.Write("-9999".PadLeft(12, ' '));
                            sw.Write("0".PadLeft(12, ' '));
                        }
                        sw.Write("\t");
                        //6
                        sw.Write(features[fet].PixelArea.ToString("#0.000000").PadLeft(10, ' '));
                        sw.Write("\t");
                        //7
                        if (features[fet].PixelArea == 0)
                            sw.Write("0.0000".PadLeft(6, ' '));
                        else
                            sw.Write((features[fet].SecondPixelArea / (features[fet].PixelArea * 100) * 100).ToString("#0.0000").PadLeft(6, ' '));
                        sw.Write("\t");
                        //8
                        sw.Write("750.00".PadLeft(8, ' '));
                        sw.Write("\t");
                        //9
                        sw.Write(orbitDateTime.ToString("yyyy-MM-dd HH:mm").PadLeft(16, ' '));
                        sw.Write("\t");
                        //10
                        sw.Write(features[fet].FireIntensityGrade.ToString().PadLeft(8, ' '));
                        sw.Write("\t");
                        //11
                        //sw.Write(features[fet].FirIntensity.ToString("#.########").PadLeft(16, ' '));
                        sw.Write(features[fet].FirIntensity.ToString("0.0000000").PadLeft(16, ' '));
                        if(inRegionIndex.Contains( features[fet].PixelIndex))
                            sw.Write("是".PadLeft(16, ' '));
                        else
                            sw.Write(" ".PadLeft(16, ' '));
                        sw.WriteLine();
                    }
                }
            }
            if (File.Exists(saveFilename))
            {
                IFileExtractResult result = new FileExtractResult("PLSF", saveFilename);
                result.SetDispaly(false);
                return result;
            }
            return null;
        }

        private string GetPLSTFilename(IArgumentProvider argProvider, out DateTime orbitDateTime)
        {
            RasterIdentify id = new RasterIdentify(argProvider.DataProvider.fileName);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FIR";
            id.SubProductIdentify = "PLST";
            id.IsOutput2WorkspaceDir = true;
            DataIdentify dataId = argProvider.DataProvider.DataIdentify;
            if (dataId == null)
                dataId = new DataIdentify();
            id.Satellite = dataId.Satellite ?? "Unknow";
            id.Sensor = dataId.Sensor ?? "Unknow";
            id.OrbitDateTime = dataId.OrbitDateTime;
            orbitDateTime = id.OrbitDateTime;
            return id.ToWksFullFileName(".txt");
        }

        private string GetPLSFFilename(IArgumentProvider argProvider, out DateTime orbitDateTime)
        {
            RasterIdentify id = new RasterIdentify(argProvider.DataProvider.fileName);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FIR";
            id.SubProductIdentify = "PLSF";
            id.IsOutput2WorkspaceDir = true;
            DataIdentify dataId = argProvider.DataProvider.DataIdentify;
            if (dataId == null)
                dataId = new DataIdentify();
            id.Satellite = dataId.Satellite ?? "Unknow";
            id.Sensor = dataId.Sensor ?? "Unknow";
            id.OrbitDateTime = dataId.OrbitDateTime;
            orbitDateTime = id.OrbitDateTime;
            return id.ToWksFullFileName(".txt");
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
    }
}
