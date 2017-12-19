using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Drawing;

namespace GeoDo.RSS.DF.HJ
{
    public class HJXML
    {
        public static bool Read(string hjXMLFile, out string[] filenames)
        {
            filenames = null;
            if (!File.Exists(hjXMLFile))
                return false;
            int lines = 0, smaple = 0;
            Dictionary<int, int> existBand = null;
            ReadXMLFile(hjXMLFile, out filenames, out existBand, out lines, out smaple);
            if (filenames == null)
                return false;
            return true;
        }

        public static Dictionary<string, string> ReadXMLFile(string hjXMLFile, out string[] filenames,
             out Dictionary<int, int> existBand, out int lines, out int smaple)
        {
            filenames = null;
            existBand = new Dictionary<int, int>();
            List<string> fileTemp = new List<string>();
            lines = 0;
            smaple = 0;
            XElement root = XElement.Load(hjXMLFile);
            if (root == null)
                return null;
            string[] satelliteAtt = HJSatelliteDef.HJSatelliteAttList();
            XElement node = null;
            Dictionary<string, string> satelliteAttDic = new Dictionary<string, string>();
            foreach (string attName in satelliteAtt)
            {
                node = root.Element(attName);
                if (node != null)
                    satelliteAttDic.Add(node.Name.ToString(), node.Value);
                else if (node == null && root.HasElements)
                    foreach (XElement subnode in root.Elements())
                    {
                        node = subnode.Element(attName);
                        if (node == null)
                            continue;
                        else
                        {
                            satelliteAttDic.Add(node.Name.ToString(), node.Value);
                            break;
                        }
                    }
                if (!satelliteAttDic.ContainsKey(attName))
                    satelliteAttDic.Add(attName, "");

            }
            if (string.IsNullOrEmpty(satelliteAttDic["satelliteId"]) || !satelliteAttDic["satelliteId"].Contains("HJ"))
                return null;
            if (string.IsNullOrEmpty(satelliteAttDic["bands"]))
                return null;
            int[] bands = GetBands(satelliteAttDic["bands"]);
            if (bands == null || bands.Length == 0)
                return null;
            string baseFile = Path.Combine(Path.GetDirectoryName(hjXMLFile), Path.GetFileNameWithoutExtension(hjXMLFile));
            string tifFile;
            int bandIndex = 0;
            //int 数据提供者band索引，从0开始   int 对应的HJ星数据band索引，从1开始
            for (int i = 0; i < bands.Length; i++)
            {
                tifFile = baseFile + "-" + bands[i] + ".TIF";
                if (File.Exists(tifFile))
                {
                    fileTemp.Add(tifFile);
                    existBand.Add(bandIndex, bands[i]);
                    bandIndex++;
                }
            }
            filenames = fileTemp.Count == 0 ? null : fileTemp.ToArray();
            //if (filenames != null)
            //{
            //    Image imgPhoto = Image.FromFile(filenames[0]);
            //    lines = imgPhoto.Height;
            //    smaple = imgPhoto.Width;
               
            //}

            return satelliteAttDic;
        }

        private static int[] GetBands(string bandStr)
        {
            string[] splitTemp = bandStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitTemp == null || splitTemp.Length == 0)
                return null;
            List<int> bandIndex = new List<int>();
            int bandIndexTemp = 0;
            foreach (string item in splitTemp)
                if (int.TryParse(item, out bandIndexTemp))
                    bandIndex.Add(bandIndexTemp);
            return bandIndex.Count == 0 ? null : bandIndex.ToArray();
        }
    }
}
