using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Layout.Elements;
using GeoDo.RSS.Layout;
using System.Xml.Linq;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.Prds.ICE;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    /// <summary>
    /// 海冰覆盖度产品专题图
    /// </summary>
    public class SubProductDEGRICE : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductDEGRICE(SubProductDef subProductDef)
            : base(subProductDef)
        {

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
            return IceDegree();
        }

        private IExtractResult IceDegree()
        {
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            SubProductInstanceDef instatnce = GetSubProductInstanceByOutIdentify(outFileIdentify);
            if(instatnce!=null)
                outFileIdentify = instatnce.OutFileIdentify;

            //string templatName = GetStringArgument("ThemeGraphTemplateName");
            string xIntervalStr = GetStringArgument("XInterval");
            string yIntervalStr = GetStringArgument("YInterval");
            float xInterval = 0, yInterval = 0;
            if (string.IsNullOrWhiteSpace(xIntervalStr) || string.IsNullOrWhiteSpace(yIntervalStr))
                return null;
            if (!float.TryParse(xIntervalStr, out xInterval) || !float.TryParse(yIntervalStr, out yInterval))
                return null;
            string outidentify = _subProductDef.Identify;
            dblvFile = GetIceDblv();
            Feature[] features = GetIceDegreeShp(dblvFile, xInterval, yInterval);

            string gxd = GenOutFiename(dblvFile, outFileIdentify, ".gxd");
            string shpFilename = Path.ChangeExtension(gxd, ".shp");
            SaveToShp(shpFilename, features);
            ApplyMcdToGxd(shpFilename);

            if (string.IsNullOrWhiteSpace(shpFilename) || !File.Exists(shpFilename))
                return null;
            CreaterShpLayers(shpFilename);

            IExtractResult result = null;
            if (instatnce != null)
                result = ThemeGraphyByInstance(instatnce);
            else
                result = VectoryThemeGraphy(null);
            string gxdFile = (result as FileExtractResult).FileName;
            AddShpLayerToGxd(gxdFile);
            (result as FileExtractResult).Add2Workspace = true;
            return result as IExtractResult;
        }

        private void AddShpLayerToGxd(string gxdFile)
        {
            try
            {
                XElement xGxdDoc = XElement.Load(gxdFile);
                XElement xLayers = xGxdDoc.Element("GxdDataFrames").Element("GxdDataFrame").Element("GxdVectorHost").Element("Map").Element("Layers");
                for (int i = 0; i < _shplayers.Count; i++)
                {
                    PrintInfo("Layer" + i + ":" + _shplayers[i].Attribute("name").Value);
                    xLayers.Add(_shplayers[i]);
                }
                xGxdDoc.Save(gxdFile);
            }
            catch (Exception ex)
            {
                PrintInfo(ex.Message);
            }
        }

        private void CreaterShpLayers(string shpFile)
        {
            _shplayers.Clear();
            //1.文件复制
            string mcdTempFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SystemData\ProductArgs\海冰覆盖度矢量模版.mcd");
            //2.修改属性
            XDocument doc = XDocument.Load(mcdTempFileName);
            //3、拷贝属性
            IEnumerable<XElement> eles = doc.Element("Map").Element("Layers").Elements("Layer");
            XElement layerElement = eles.Single((ele) => { return ele.Attribute("name").Value == "DEGR"; });
            if (layerElement != null)
            {
                ApplyLayer(shpFile, layerElement);
                _shplayers.Add(layerElement);
            }
        }

        private List<XElement> _shplayers = new List<XElement>(); //加载的shpfile层
        private string shpFilename = "";
        private string dblvFile = "";

        /// <summary>
        /// 设置数据层
        /// 栅格数据
        /// </summary>
        /// <param name="gxdDataFrame"></param>
        /// <param name="dataFrame"></param>
        protected override void ApplyAttributesOfDataFrame(IGxdDataFrame gxdDataFrame, IDataFrame dataFrame, ILayout layout)
        {
            string colorTabelName = null;
            //if (!string.IsNullOrEmpty(instance.ColorTableName))
            //    colorTabelName = "colortablename=" + instance.ColorTableName;

            IGxdRasterItem rst = new GeoDo.RSS.Layout.GDIPlus.GxdRasterItem(dblvFile, colorTabelName);//这里传具体的色标定义标识
            gxdDataFrame.GxdRasterItems.Add(rst);

            base.ApplyAttributesOfDataFrame(gxdDataFrame, dataFrame, layout);
        }

        #region CreateShp and Mcd，计算海冰覆盖度，生成shp，生成显示mcd
        private Feature[] GetIceDegreeShp(string dblvFile, float xInterval, float yInterval)
        {
            Envelope env;
            Size size;
            short[] data;
            using (IRasterDataProvider dblv = RasterDataDriver.Open(dblvFile) as IRasterDataProvider)
            {
                if (dblv == null)
                    return null;
                env = new Envelope(dblv.CoordEnvelope.MinX, dblv.CoordEnvelope.MinY, dblv.CoordEnvelope.MaxX, dblv.CoordEnvelope.MaxY);
                size = new Size(dblv.Width, dblv.Height);
                data = ReadData(dblv);
            }
            Feature[] features = CreateFeature(env, xInterval, yInterval, data, env, size);
            return features;
        }

        private string GetIceDblv()
        { 
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            foreach (string item in files)
            {
                if (!File.Exists(item))
                    return null;
            }
            return files[0];
        }

        //生成覆盖度矢量
        private Feature[] CreateFeature(Envelope env, float xInterval, float yInterval, short[] data, Envelope dataEnv, Size dataSize)
        {
            int oid = 0;
            List<Feature> features = new List<Feature>();
            double leftTopX = env.MinX;
            while (leftTopX < env.MaxX)
            {
                double leftTopY = env.MaxY;
                while (leftTopY > env.MinY)
                {
                    ShapeRing ring = new ShapeRing(
                        new ShapePoint[]
                        {
                            new ShapePoint(leftTopX,leftTopY),
                            new ShapePoint(leftTopX+xInterval,leftTopY),
                            new ShapePoint(leftTopX+xInterval,leftTopY-yInterval),
                            new ShapePoint(leftTopX,leftTopY-yInterval)
                        }
                        );
                    ShapePolygon sp = new ShapePolygon(new ShapeRing[] { ring });
                    int degree = CalcIceDegree(sp, data, dataEnv, dataSize);
                    Feature fet = new Feature(oid++, sp, new string[] { "覆盖度" }, new string[] { degree == 0 ? "" : degree.ToString() }, null);
                    features.Add(fet);
                    leftTopY -= yInterval;
                }
                leftTopX += xInterval;
            }
            return features.ToArray();
        }

        private short[] ReadData(IRasterDataProvider dblv)
        {
            Envelope env = new Envelope(dblv.CoordEnvelope.MinX, dblv.CoordEnvelope.MinY, dblv.CoordEnvelope.MaxX, dblv.CoordEnvelope.MaxY);
            Size size = new Size(dblv.Width, dblv.Height);
            GCHandle buffer = new GCHandle();
            IntPtr bufferPtr;
            short[] vBufferData = new short[dblv.Width * dblv.Height];
            try
            {
                buffer = GetHandles(vBufferData);
                bufferPtr = buffer.AddrOfPinnedObject();
                IRasterBand band = dblv.GetRasterBand(1);
                band.Read(0, 0, dblv.Width, dblv.Height, bufferPtr, enumDataType.Int16, dblv.Width, dblv.Height);
                return vBufferData;
            }
            finally
            {
                if (buffer.IsAllocated)
                    buffer.Free();
            }
        }

        //计算覆盖度(百分比)
        private int CalcIceDegree(ShapePolygon sp, short[] data, Envelope env, Size size)
        {
            int[] aoi = GetAoi(sp, env, size);
            int degree = 0;
            int count = 0;
            foreach (int aoiindex in aoi)
            {
                if (data[aoiindex] == 1)
                    count++;
            }
            degree = (int)(count * 100f / aoi.Length);
            return degree;
        }

        internal static GCHandle GetHandles<T>(T[] virtureInData)
        {
            return GCHandle.Alloc(virtureInData, GCHandleType.Pinned);
        }

        private static int[] GetAoi(ShapePolygon shpPolygon, Envelope env, Size size)
        {
            try
            {
                int[] aoi = null;
                using (VectorAOIGenerator vectorGen = new VectorAOIGenerator())
                {
                    aoi = vectorGen.GetAOI(new ShapePolygon[] { shpPolygon }, env, size);
                }
                return aoi;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 保存矢量数据到文件
        /// </summary>
        /// <param name="shpFileName"></param>
        /// <param name="features"></param>
        private void SaveToShp(string shpFileName, Feature[] features)
        {
            IEsriShapeFilesWriter writer = null;
            try
            {
                writer = new EsriShapeFilesWriterII(shpFileName, enumShapeType.Polygon);
                writer.BeginWrite();
                writer.Write(features);
            }
            finally
            {
                if (writer != null)
                    writer.EndWriter();
            }
        }
        
        //通过mcd的方式设置显示方案。
        private void ApplyMcdToGxd(string shpFile)
        {
            try
            {
                //1.文件复制
                string sourceFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SystemData\ProductArgs\海冰覆盖度矢量模版.mcd");
                if (!File.Exists(sourceFileName))
                    return;
                string newFileName = Path.ChangeExtension(shpFile, ".mcd");
                File.Copy(sourceFileName, newFileName, true);
                //2.修改属性
                XDocument doc = XDocument.Load(newFileName);
                XElement layerElement = doc.Element("Map").Element("Layers").Elements("Layer").Single(
                    (ele) => 
                    { 
                        return ele.Attribute("name").Value == "DEGR"; 
                    }
                    );
                if (layerElement != null)
                {
                    ApplyLayer(shpFile, layerElement);
                    doc.Save(newFileName);
                }
            }
            catch
            {
                PrintInfo("创建mcd失败。");
            }
        }

        private void ApplyLayer(string shpFile, XElement layerElement)
        {
            string shpName = Path.GetFileNameWithoutExtension(shpFile);
            layerElement.Attribute("name").Value = shpName;//
            //XElement uniqueSymbolsEle = layerElement.Element("Renderer").Element("Symbol");
            //if (uniqueSymbolsEle != null)
            //{
            //    Color itemColor = Color.FromArgb(255, 166, 208, 255);
            //    uniqueSymbolsEle.Attribute("color").Value = string.Format("{0},{1},{2},{3}", itemColor.A, itemColor.R, itemColor.G, itemColor.B);
            //}
            XElement dataSourceEle = layerElement.Element("FeatureClass").Element("DataSource");
            if (dataSourceEle != null)
            {
                dataSourceEle.Attribute("name").Value = shpName;
                dataSourceEle.Attribute("fileurl").Value = shpFile;
            }
        }

        #endregion 
        
        private string GenOutFiename(string rasterFile, string outFileIdentify, string ext)
        {
            RasterIdentify rd = new RasterIdentify(rasterFile);
            rd.SubProductIdentify = outFileIdentify;
            rd.ProductIdentify = _subProductDef.ProductDef.Identify;
            rd.Format = ext;
            return rd.ToWksFullFileName(ext);
        }

        public void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }
}
