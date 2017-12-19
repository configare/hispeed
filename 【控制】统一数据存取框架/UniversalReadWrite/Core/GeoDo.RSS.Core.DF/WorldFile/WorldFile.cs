using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.Project;
using System.Xml.Linq;
using System.Xml;

namespace GeoDo.RSS.Core.DF
{
    /// <summary>
    /// byte[0]:x 比例因子；像素的 x 方向尺寸，采用地图单位；
    /// byte[1]:旋转项；
    /// byte[2]:旋转项；
    /// byte[3]: y 比例因子的负值；像素的 y 方向尺寸，采用地图单位;
    /// byte[4]、byte[5]:平移项；左上角像素的中心点的 x,y 地图坐标;
    /// 注：y 比例因子 (E) 为负值，因为图像的原点与地理坐标系的原点不同。
    ///       图像的原点位于左上角，而地图坐标系的原点位于左下角。
    ///       图像中的行值从原点开始向下逐渐增大，而地图中的 y 坐标值从原点开始向上逐渐增大。
    /// </summary>
    public class WorldFile
    {
        //左上角像素经度
        private double _minX = 0;
        //左上角像素纬度
        private double _maxY = 0;
        private double _xResolution = 0;
        private double _yResolution = 0; //negative 

        public WorldFile()
        {
        }

        public WorldFile(string worldfilename)
        {
            try
            {
                if (!File.Exists(worldfilename))
                    throw new FileNotFoundException(worldfilename);
                string[] lines = File.ReadAllLines(worldfilename);
                if (lines == null || lines.Length == 0)
                    throw new Exception(worldfilename + "为空。");
                if (lines.Length < 6)
                    throw new Exception(worldfilename + "不是标准的ESRI World Files。");
                _xResolution = double.Parse(lines[0]);
                //1 ,2 is rotation terms
                _yResolution = double.Parse(lines[3]);
                _minX = double.Parse(lines[4]);
                _maxY = double.Parse(lines[5]);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public double MinX
        {
            get { return _minX; }
            set { _minX = value; }
        }

        public double MaxY
        {
            get { return _maxY; }
            set { _maxY = value; }
        }

        public double XResolution
        {
            get { return _xResolution; }
            set { _xResolution = value; }
        }

        public double YResolution
        {
            get { return _yResolution; }
            set { _yResolution = value; }
        }

        public static string GetWorldFilenameByRasterFilename(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return null;
            try
            {
                string extName = Path.GetExtension(filename).ToUpper();
                string dir = Path.GetDirectoryName(filename);
                string mainfilename = Path.GetFileNameWithoutExtension(filename);
                string retExtname = null;
                switch (extName)
                {
                    case ".BMP":
                        retExtname = ".bpw";
                        break;
                    case ".JPG":
                        retExtname = ".jpw";
                        break;
                    case ".GIF":
                        retExtname = ".gfw";
                        break;
                    case ".TIF":
                        retExtname = ".tfw";
                        break;
                    case ".PNG":
                        retExtname = ".pgw";
                        break;
                    default:
                        break;
                }
                if (retExtname == null)
                    return null;
                return Path.Combine(dir, mainfilename + retExtname);
            }
            catch
            {
                return null;
            }
        }

        public void CreatWorldFile(double xResolution, double yResolution, double minX, double maxY, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;
            string worldFileName = GetWorldFilenameByRasterFilename(fileName);
            _xResolution = xResolution;
            _yResolution = yResolution;
            _minX = minX;
            _maxY = maxY;
            double[] bytes = new double[6];
            bytes[0] = _xResolution;
            bytes[3] = _yResolution;
            bytes[4] = _minX;
            bytes[5] = _maxY;
            string[] infos = new string[6];
            for (int i = 0; i < 6; i++)
                infos[i] = bytes[i].ToString();
            File.WriteAllLines(worldFileName, infos);
        }

        public void CreatXmlFile(ISpatialReference spatialRef, string fname)
        {
            if (string.IsNullOrEmpty(fname))
                return;
            string xmlName = fname + ".aux.xml";
            XmlDocument doc = new XmlDocument();
            XmlNode root = doc.CreateElement("PAMDataset");
            CreatPrjElement(doc, root,spatialRef);
            CreatMetaDataNode(doc, root);
            CreateBandsNode(doc, root);
            doc.AppendChild(root);
            doc.Save(xmlName);
        }

        private void CreatPrjElement(XmlDocument doc, XmlNode root,ISpatialReference spatialRef)
        {
            XmlNode prjNd = doc.CreateElement("SRS");
            prjNd.InnerText = spatialRef.ToWKTString();
            root.AppendChild(prjNd);
        }

        private void CreatMetaDataNode(XmlDocument doc, XmlNode root)
        {
            XmlNode dataNd = doc.CreateElement("Metadata");
            CreatMdiNode(doc, dataNd);           
            root.AppendChild(dataNd);
        }

        private void CreatMdiNode(XmlDocument doc, XmlNode dataNd)
        {
            XmlNode mdi = doc.CreateElement("MDI");
            XmlAttribute att = doc.CreateAttribute("key");
            att.Value = "PyramidResamplingType";
            mdi.Attributes.Append(att);
            mdi.InnerText = "NEAREST";
            dataNd.AppendChild(mdi);
        }

        private void CreateBandsNode(XmlDocument doc, XmlNode root)
        {
            for (int i = 1; i < 4; i++)
            {
                CreateSingleNode(doc, root,i);
            }
        }

        private void CreateSingleNode(XmlDocument doc, XmlNode root,int i)
        {
            XmlNode band = doc.CreateElement("PAMRasterBand");
            XmlAttribute att = doc.CreateAttribute("band");
            att.Value = i.ToString();
            band.Attributes.Append(att);
            XmlNode child = doc.CreateElement("Metadata");
            band.AppendChild(child);
            root.AppendChild(band);
        }
    }
}
