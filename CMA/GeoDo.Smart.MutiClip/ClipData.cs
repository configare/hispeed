using CodeCell.AgileMap.Core;
using GeoDo.RSS.BlockOper;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.MEM;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.UI.AddIn.DataPro;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;


namespace GeoDo.Smart.MutiClip
{
    public class ClipData
    {

        public void ClipFile(string xmlfile)
        {

            RasterClip clip = new RasterClip();
            InputArgs input = GetConfigByXml(xmlfile);
            List<BlockDefWithAOI> blocklist = new List<BlockDefWithAOI>();
            for (int i = 0; i < input.ListCoord.Count; i++)
            {
                BlockDefWithAOI blo = new BlockDefWithAOI(input.ListCoord[i].CoordName, input.ListCoord[i].MinX, input.ListCoord[i].MinY, input.ListCoord[i].MaxX, input.ListCoord[i].MaxY);
                blocklist.Add(blo);
            }

            foreach (string itemfile in input.ListInputFiles)
            {
                Console.WriteLine(string.Format("正在裁切:{0}",itemfile));
                clip.RasterClipT(itemfile, blocklist.ToArray(), input.OutDir, null, "Cut");
            }
        }
        public InputArgs GetConfigByXml(string xmlfile)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlfile);
                InputArgs input = new InputArgs();
                XmlElement inputfileele = doc.DocumentElement.SelectSingleNode("InputDir") as XmlElement;
                string localfile = inputfileele.GetAttribute("dir");
                input.InputDir = localfile;
                string[] listfiles = GetDirFiles(localfile);
                input.ListInputFiles = listfiles == null ? new List<string>() : listfiles.ToList();
                XmlElement outfileele = doc.DocumentElement.SelectSingleNode("OutputDir") as XmlElement;
                input.OutDir = outfileele.GetAttribute("dir");
               
                XmlNodeList listnodecoord = doc.DocumentElement.SelectNodes("CoordEnv");
                for (int i = 0; i < listnodecoord.Count; i++)
                {
                    CoordInfo corrd = new CoordInfo();
                    XmlElement coordnameele = listnodecoord[i].SelectSingleNode("CoordName") as XmlElement;
                    string coorname = coordnameele.GetAttribute("value");
                    corrd.CoordName = coorname;
                    XmlElement minxele = listnodecoord[i].SelectSingleNode("MinX") as XmlElement;
                    double MinX = double.Parse(minxele.GetAttribute("value"));
                    corrd.MinX = MinX;
                    XmlElement maxele = listnodecoord[i].SelectSingleNode("MaxX") as XmlElement;
                    double MaxX = double.Parse(maxele.GetAttribute("value"));
                    corrd.MaxX = MaxX;
                    XmlElement minyele = listnodecoord[i].SelectSingleNode("MinY") as XmlElement;
                    double MinY = double.Parse(minyele.GetAttribute("value"));
                    corrd.MinY = MinY;
                    XmlElement maxyele = listnodecoord[i].SelectSingleNode("MaxY") as XmlElement;
                    double MaxY = double.Parse(maxyele.GetAttribute("value"));
                    corrd.MaxY = MaxY;
                    input.ListCoord.Add(corrd);
                }
                return input;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private string[] GetDirFiles(string fileOrDir)
        {
            if (fileOrDir.Contains("|*"))
            {
                string[] fileInfo = fileOrDir.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (fileInfo == null || fileInfo.Length != 2)
                    return null;
                if (Directory.Exists(fileInfo[0]))
                {
                    string[] files = Directory.GetFiles(fileInfo[0], fileInfo[1], SearchOption.TopDirectoryOnly);
                    return files;

                }
                return null;

            }
            else
                return null;
        }

    }
}
