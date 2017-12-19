
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;


namespace GeoDo.RSS.UI.AddIn.Tools
{
    public class ClipData
    {
       
        public  InputArgs GetConfigByXml(string xmlfile)
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
                input.ListInputFiles =listfiles==null?new List<string>(): listfiles.ToList();
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
        public void SetXmlConfig(string xmlfile,InputArgs input)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlfile);
                XmlElement inputfileele = doc.DocumentElement.SelectSingleNode("InputDir") as XmlElement;
                inputfileele.SetAttribute("dir",input.InputDir);
                XmlElement outfileele = doc.DocumentElement.SelectSingleNode("OutputDir") as XmlElement;
                outfileele.SetAttribute("dir", input.OutDir);
                //删除全部coordinfo节点
                XmlNodeList oldcoordlist = doc.DocumentElement.SelectNodes("CoordEnv");
                for (int i = 0; i < oldcoordlist.Count;i++)
                {
                    doc.DocumentElement.RemoveChild(oldcoordlist[i]);
                }
                //添加新的coordinfo 节点
                for (int i = 0; i < input.ListCoord.Count; i++)
                    {
                        XmlElement coordelement = doc.CreateElement("CoordEnv");

                        XmlElement itemcorrdname = doc.CreateElement("CoordName");
                        XmlAttribute itemcorrdnameattr = doc.CreateAttribute("value");
                        itemcorrdnameattr.Value = input.ListCoord[i].CoordName;
                        itemcorrdname.Attributes.Append(itemcorrdnameattr);
                        coordelement.AppendChild(itemcorrdname);

                        XmlElement itemminx = doc.CreateElement("MinX");
                        XmlAttribute itemcorrdminxattr = doc.CreateAttribute("value");
                        itemcorrdminxattr.Value = input.ListCoord[i].MinX.ToString();
                        itemminx.Attributes.Append(itemcorrdminxattr);
                        coordelement.AppendChild(itemminx);

                        XmlElement itemmaxx = doc.CreateElement("MaxX");
                        XmlAttribute itemcorrdmaxxattr = doc.CreateAttribute("value");
                        itemcorrdmaxxattr.Value = input.ListCoord[i].MaxX.ToString();
                        itemmaxx.Attributes.Append(itemcorrdmaxxattr);
                        coordelement.AppendChild(itemmaxx);

                        XmlElement itemminy = doc.CreateElement("MinY");
                        XmlAttribute itemcorrdminyattr = doc.CreateAttribute("value");
                        itemcorrdminyattr.Value = input.ListCoord[i].MinY.ToString();
                        itemminy.Attributes.Append(itemcorrdminyattr);
                        coordelement.AppendChild(itemminy);

                        XmlElement itemmaxy = doc.CreateElement("MaxY");
                        XmlAttribute itemcorrdmaxyattr = doc.CreateAttribute("value");
                        itemcorrdmaxyattr.Value = input.ListCoord[i].MaxY.ToString();
                        itemmaxy.Attributes.Append(itemcorrdmaxyattr);
                        coordelement.AppendChild(itemmaxy);
                        doc.DocumentElement.AppendChild(coordelement);
                    }
                    
                
                doc.Save(xmlfile);
            }
            catch (Exception ex)
            {
                return ;
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
