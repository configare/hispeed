using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.FileProject;
using System.IO;
using System.Xml.Linq;
using GeoDo.RasterProject;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class DataBaseArg
    {
        private string _serverName;
        private string _databaseName;
        private string _uid;
        private string _passwords;
        private string _OutputDir;

        #region 属性

        public string OutputDir
        {
            get { return _OutputDir; }
            set { _OutputDir = value; }
        }

        public string ServerName
        {
            get { return _serverName;}
            set { _serverName = value; }
        }

        public string DatabaseName
        {
            get { return _databaseName; }
            set { _databaseName = value; }
        }

        public string Passwords
        {
            get { return _passwords; }
            set { _passwords = value; }
        }

        public string UID
        {
            get { return _uid; }
            set { _uid = value; }
        }

        #endregion

        public static DataBaseArg ParseXml(string argXml)
        {
            if (string.IsNullOrWhiteSpace(argXml))
                throw new ArgumentNullException("argXml", "参数文件为空");
            if (!File.Exists(argXml))
                throw new FileNotFoundException("参数文件不存在" + argXml, argXml);
            try
            {
                string server = "", database = "", uid = "", passwords = "";//,outputdir="";
                string modisdir = "", airsdir = "", isccpdir = "", cloudsatdir = "";
                XElement xml = XElement.Load(argXml);
                XElement svr = xml.Element("Server");
                if (svr != null)
                {
                    server = svr.Value;
                }
                XElement dbase = xml.Element("Database");
                if (dbase != null)
                {
                    database = dbase.Value;
                }
                XElement id = xml.Element("Uid");
                if (id != null)
                {
                    uid = id.Value;
                }
                XElement passwds = xml.Element("Passwords");
                if (passwds != null)
                {
                    passwords = passwds.Value;
                }
                //XElement xoutputdir = xml.Element("OutputDir");
                //if (xoutputdir != null)
                //{
                //    outputdir = xoutputdir.Value;
                //}
                XElement modisXdir = xml.Element("MODIS");
                if (modisXdir != null)
                {
                    modisdir = modisXdir.Attribute("RootPath").Value;
                }
                XElement airsxdir = xml.Element("AIRS");
                if (airsxdir != null)
                {
                    airsdir = airsxdir.Attribute("RootPath").Value;
                }
                XElement isccpxdir = xml.Element("ISCCP");
                if (isccpxdir != null)
                {
                    isccpdir = isccpxdir.Attribute("RootPath").Value;
                }
                XElement cloudxdir = xml.Element("CloudSAT");
                if (cloudxdir != null)
                {
                    cloudsatdir = cloudxdir.Attribute("RootPath").Value;
                }
                DataBaseArg arg = new DataBaseArg();
                arg._serverName = server;
                arg._databaseName = database;
                arg._uid = uid;
                arg._passwords = passwords;
                arg._OutputDir = modisdir;
                arg.CloudSATRootPath = cloudsatdir;
                arg.AIRSRootPath = airsdir;
                arg.ISCCPRootPath = isccpdir;
                return arg;
            }
            catch (Exception ex)
            {
                throw new Exception("解析投影输入参数文件失败" + ex.Message, ex);
            }
        }

        public void ToXml(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException("xmlFilename");
            XElement xml = new XElement("xml",
                new XElement("Server", _serverName),
                new XElement("Database", _databaseName),
                new XElement("Uid", _uid),
                new XElement("Passwords", _passwords),
               new XElement("MODIS",new XAttribute("RootPath",_OutputDir)),
               new XElement("AIRS",new XAttribute("RootPath",_airsrootpath)),
               new XElement("ISCCP",new XAttribute("RootPath",_isccprootpath)),
               new XElement("CloudSAT",new XAttribute("RootPath",_cloudsatrootpath))
               );
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            xml.Save(fileName);
        }

        #region 预处理和归档路径的设置与读取
        private  string _isccprootpath = null, _airsrootpath = null, _cloudsatrootpath = null;
        private  Dictionary<int, string> _modisrootpath = null;

        public  bool ParseRootPathXml(string argXml,string sensor=null,int  modisyear=0)
        {
            if (string.IsNullOrWhiteSpace(argXml))
                throw new ArgumentNullException("argXml", "参数文件为空");
            if (!File.Exists(argXml))
                throw new FileNotFoundException("参数文件不存在!", argXml);
            try
            {
                //XElement xml = XElement.Load(argXml);
                XDocument _doc = XDocument.Load(argXml);
                if (_doc == null)
                    return false;
                XElement root = _doc.Root;
                IEnumerable<XElement> elements = root.Elements();
                string rootpath;
                foreach (XElement ele in elements)
                {
                    if (ele == null)
                        continue;
                    if (ele.Name == "MODIS")
                    {
                        IEnumerable<XElement> subeles = ele.Elements();
                        _modisrootpath = new Dictionary<int, string>();
                        int year;
                        foreach (XElement subele in subeles)
                        {
                            if (subele != null && subele.Name == "Types" && int.TryParse(subele.Attribute("Year").Value, out year))
                            {
                                rootpath = subele.Attribute("RootPath").Value;
                                if (string.IsNullOrWhiteSpace(rootpath))
                                    continue;
                                if (!_modisrootpath.ContainsKey(year))
                                    _modisrootpath.Add(year, rootpath);
                            }
                        }
                    }
                    else if (ele.Name == "AIRS")
                    {
                        _airsrootpath = ele.Attribute("RootPath").Value;
                    }
                    else if (ele.Name == "ISCCP")
                    {
                        _isccprootpath = ele.Attribute("RootPath").Value;
                    }
                    else if (ele.Name == "CloudSAT")
                    {
                        _cloudsatrootpath = ele.Attribute("RootPath").Value;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("解析投影输入参数文件失败" + ex.Message, ex);
            }
        }

        public void RootPathToXml(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException("xmlFilename");
            XElement modis = null, airs = null, isccp = null, cloudsat = null;
            XElement xml = new XElement("xml");
            if (_modisrootpath != null && _modisrootpath.Count>0)
            {
                List<XElement> mtl = new List<XElement>();
                foreach (int key in _modisrootpath.Keys)
                {
                    mtl.Add(new XElement("Types", new XAttribute("Year", key),new XAttribute("RootPath",_modisrootpath[key])));
                }
                modis = new XElement("MODIS",mtl.ToArray());
                xml.Add(modis);
            }
            if (_airsrootpath != null)
            {
                //airs = new XElement("AIRS", new XElement("Types", new XAttribute("RootPath", _airsrootpath)));
                airs = new XElement("AIRS", new XAttribute("RootPath", _airsrootpath));
                xml.Add(airs);
            }
            if (_isccprootpath != null)
            {
                isccp = new XElement("ISCCP",new XAttribute("RootPath", _isccprootpath));
                xml.Add(isccp);
            }
            if (_cloudsatrootpath != null)
            {
                cloudsat = new XElement("CloudSAT",new XAttribute("RootPath", _cloudsatrootpath));
                xml.Add(cloudsat);
            }
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            xml.Save(fileName);
        }

        public  string ISCCPRootPath
        {
            get { return _isccprootpath; }
            set { _isccprootpath = value; }
        }

        public  string AIRSRootPath
        {
            get { return _airsrootpath; }
            set { _airsrootpath = value; }
        }

        public  string CloudSATRootPath
        {
            get { return _cloudsatrootpath; }
            set { _cloudsatrootpath = value; }
        }

        public  Dictionary<int,string> MODISRootPath
        {
            get { return _modisrootpath; }
            set { _modisrootpath = value; }
        }

        public  string GetMODISRootPath(int year)
        {
            return _modisrootpath[year];
        }

        public  bool SetMODISRootPath(int year,string dir)
        {
            if(_modisrootpath!=null)
            {
                if (_modisrootpath.ContainsKey(year))
                {
                    _modisrootpath[year]=dir;
                } 
                else
                {
                    _modisrootpath.Add(year,dir);
                }
                return true;
            }
            return false;
        }


        #endregion

    }
}
