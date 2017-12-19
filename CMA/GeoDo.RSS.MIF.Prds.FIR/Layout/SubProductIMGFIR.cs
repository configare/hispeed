using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.Layout;
using CodeCell.AgileMap.Core;
using System.Xml.Linq;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public class SubProductIMGFIR : CmaMonitoringSubProduct
    {
        private StringBuilder _firInfos = new StringBuilder();
        private string _firFile = null;
        private bool _addFirInfo = false;

        public SubProductIMGFIR(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "0IMGAlgorithm")
            {
                IExtractResult er= IMGAlgorithm();
                if (_firFile != null)
                {
                    string gxdFileName = (er as FileExtractResult).FileName;
                    string shpFileName = GenaratePoints(gxdFileName);
                    if (!string.IsNullOrEmpty(shpFileName) && File.Exists(shpFileName))
                        AddShpToGxd(shpFileName, gxdFileName);
                }
                return er;
            }
            return null;
        }

        private IExtractResult IMGAlgorithm()
        {
            return ThemeGraphyResult(null);
        }

        protected override void ApplyAttributesOfLayoutTemplate(ILayoutTemplate template)
        {
            _addFirInfo = false;
            _firFile = null;
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            if (instanceIdentify == "FPGI" || instanceIdentify == "0SDI")
            {
                foreach (IElement item in template.Layout.Elements)
                {
                    if (item is MultlineTextElement)
                    {
                        MultlineTextElement txt = item as MultlineTextElement;
                        if (txt.Text.Contains("{ControlPoints}"))
                        {
                            _addFirInfo = true;
                            break;
                        }
                    }
                }
                if (_addFirInfo)
                {
                    string[] frilFileList = FindFRILFiles();
                    if (frilFileList == null || frilFileList.Length < 1)
                    {
                        _firFile = null;
                        return;
                    }
                    SetFireInfos(frilFileList[0]);
                    _firFile = frilFileList[0];
                    Dictionary<string, string> vars = new Dictionary<string, string>();
                    vars.Add("{ControlPoints}", _firInfos.ToString());
                    template.ApplyVars(vars);
                }

            }
        }

        private string GenaratePoints(string gxdFileName)
        {
            string[] lines = File.ReadAllLines(_firFile);
            if (lines == null || lines.Length < 2)
                return null;
            List<Feature> features = new List<Feature>();
            ShapePoint pt;
            string[] fieldValues;
            string[] fieldNames = new string[]{"火区号","经度","纬度","像元个数","像元覆盖面积","明火面积"};
            for (int i = 1; i < lines.Length; i++)
            {
               fieldValues = lines[i].Replace(" ", "").Split(new char[] { '\t' },6, StringSplitOptions.RemoveEmptyEntries);
               if (fieldValues != null)
               {
                   //经纬度坐标度分秒转十进制
                   double x=GetAngleFromString(fieldValues[1]);
                   double y=GetAngleFromString(fieldValues[2]);
                   pt = new ShapePoint(x, y);
                   Feature f = new Feature(i - 1, pt, fieldNames, fieldValues, null);
                   features.Add(f);
               }
            }
            if (features.Count < 1)
                return null;
            string shpFileName = Path.Combine(Path.GetDirectoryName(gxdFileName), Path.GetFileNameWithoutExtension(gxdFileName) + ".shp");
            EsriShapeFilesWriterII w = new EsriShapeFilesWriterII(shpFileName, enumShapeType.Point);
            w.BeginWrite();
            w.Write(features.ToArray());
            w.EndWriter();
            //mcd文件
            CreateMcd(shpFileName);
            return shpFileName;
        }

        private double GetAngleFromString(string angleString)
        {
            string[] values = angleString.Trim().Split(new char[] { '°', '′', '″' },StringSplitOptions.RemoveEmptyEntries);
            if (values == null || values.Length != 3)
                return 0;
            double angle = double.Parse(values[0]) + double.Parse(values[1]) / 60 + double.Parse(values[2]) / 60 / 60;
            return Math.Round(angle,2);
        }

        private void SetFireInfos(string frilFile)
        {
            _firInfos.Clear();
            string[] lines = File.ReadAllLines(frilFile);
            if (lines == null || lines.Length < 2)
                return;
            _firInfos.AppendLine(string.Format("{0}\t{1}\t{2}\t{3}\t", "火区号", "像元个数", "像元覆盖面积", "明火面积"));
            int length = "火区号\t".Length;
            //i从1开始，第一行为列名称
            for (int i = 1; i < lines.Length;i++ )
            {
                string[] infos = lines[i].Replace(" ", "").Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (infos != null && infos.Length == 11)
                {
                    _firInfos.AppendLine(string.Format("{0}\t{1}\t{2}\t{3}\t", infos[0].Trim().PadRight(11), infos[3].Trim().PadRight(12),
                        infos[4].Trim().PadRight(17), infos[5].Trim().PadRight(16)));
                }
            }
        }

        private string[] FindFRILFiles()
        {
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            List<string> plstList = new List<string>();
            foreach (string file in files)
            {
                string dir = Path.GetDirectoryName(file);
                string plstDir = null;
                if (!dir.Contains("栅格产品"))
                    return null;
                plstDir = Path.Combine(Directory.GetParent(dir).ToString(), "信息列表");
                if (!Directory.Exists(plstDir))
                    return null;
                string inputFileName = Path.GetFileNameWithoutExtension(file);
                string plstName=null;
                if(inputFileName.Contains("DBLV"))
                   plstName = Path.GetFileNameWithoutExtension(file).Replace("DBLV", "FRIL");
                else if(inputFileName.Contains("0FPG"))
                    plstName = Path.GetFileNameWithoutExtension(file).Replace("0FPG", "FRIL");
                plstName = Path.Combine(plstDir, plstName + ".txt");
                if (!File.Exists(plstName))
                    return null;
                plstList.Add(plstName);
            }
            return plstList.ToArray();
        }

        private void AddShpToGxd(string shpFileName,string gxdFileName)
        {
            try
            {
                string shpMcd = Path.ChangeExtension(shpFileName, ".mcd");
                XElement xShpMcd = XElement.Load(shpMcd);
                XElement xShpLayer = xShpMcd.Element("Layers").Element("Layer");
                xShpLayer.Element("FeatureClass").Element("DataSource").SetAttributeValue("fileurl", shpFileName);
                XElement xGxd = XElement.Load(gxdFileName);// gxdDataFrame.GxdVectorHost.McdFileContent.ToString());     //
                XElement xLayers = xGxd.Element("GxdDataFrames").Element("GxdDataFrame").Element("GxdVectorHost").Element("Map").Element("Layers");
                xLayers.Add(xShpLayer);
                xGxd.Save(gxdFileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void CreateMcd(string shpFile)
        {
            try
            {
                //1.文件复制
                string sourceFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SystemData\ProductArgs\火情监测专题图模版.mcd");
                if (!File.Exists(sourceFileName))
                    return;
                string shpName = Path.GetFileNameWithoutExtension(shpFile);
                string newFileName = Path.Combine(Path.GetDirectoryName(shpFile), shpName + ".mcd");
                File.Copy(sourceFileName, newFileName, true);
                //2.修改属性
                XDocument doc = XDocument.Load(newFileName);
                XElement layerElement = doc.Element("Map").Element("Layers").Element("Layer");
                if (layerElement == null)
                    return;
                layerElement.Attribute("name").Value = shpName;//
                XElement dataSourceEle = layerElement.Element("FeatureClass").Element("DataSource");
                if (dataSourceEle == null)
                    return;
                dataSourceEle.Attribute("name").Value = shpName;//
                dataSourceEle.Attribute("fileurl").Value = ".\\" + Path.GetFileName(shpFile);//
                doc.Save(newFileName);
            }
            catch
            {
                Console.WriteLine("创建mcd失败。");
            }
        }
    }
}
