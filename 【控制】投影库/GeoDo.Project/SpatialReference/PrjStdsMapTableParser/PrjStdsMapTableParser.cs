using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace GeoDo.Project
{
    public class PrjStdsMapTableParser : IPrjStdsMapTableParser, IDisposable
    {
        public static string CnfgFile = null;
        protected string _cnfgFile = null;
        private XDocument _doc = null;
        private XElement _root = null;
        private List<NameMapItem> _prjParameterItems = null;
        private List<NameMapItem> _prjNameMapItems = null;
        private Dictionary<string, string> _datumItems = null;
        private EnviPrjInfoArgDef[] _enviPrjInfoArgDefs = null;

        public PrjStdsMapTableParser()
        {
            string configfile = null;
            configfile = CnfgFile ?? Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "GeoDo.Project.Cnfg.xml");
            _cnfgFile = configfile;
            InitCnfgfileParser();
        }

        private void InitCnfgfileParser()
        {
            if (_cnfgFile == null)
                return;
            _doc = XDocument.Load(_cnfgFile);
            _root = _doc.Root;
            var eleProjection = _root.Element(XName.Get("ProjectionMap"));
        }

        public EnviPrjInfoArgDef GetEnviPrjInfoArgDef(int prjId)
        {
            EnviPrjInfoArgDef[] enviPrjInfoArgDefs = EnviPrjInfoArgDefs;
            if (enviPrjInfoArgDefs == null)
                return null;
            foreach (EnviPrjInfoArgDef def in enviPrjInfoArgDefs)
                if (def.PrjId == prjId)
                    return def;
            return null;
        }

        public EnviPrjInfoArgDef[] EnviPrjInfoArgDefs
        {
            get
            {
                if (_enviPrjInfoArgDefs == null)
                {
                    XElement defEles = _root.Element("EnviPrjInfoArgDefs");
                    List<EnviPrjInfoArgDef> defs = new List<EnviPrjInfoArgDef>();
                    foreach (XElement ele in defEles.Elements())
                    {
                        EnviPrjInfoArgDef def = GetEnviPrjInfoArgDef(ele);
                        if (def != null)
                            defs.Add(def);
                    }
                    _enviPrjInfoArgDefs = defs.ToArray();
                }
                return _enviPrjInfoArgDefs;
            }
        }

        private EnviPrjInfoArgDef GetEnviPrjInfoArgDef(XElement ele)
        {
            return new EnviPrjInfoArgDef(int.Parse(ele.Attribute(XName.Get("id")).Value),
                ele.Attribute(XName.Get("name")).Value,
                ele.Attribute(XName.Get("args")).Value);
        }

        public Dictionary<string, string> GetDatumsItems()
        {
            _datumItems = new Dictionary<string, string>();
            XElement datum = _root.Element(XName.Get("Datums")).Element(XName.Get("Datum"));
            _datumItems.Add(datum.Attribute(XName.Get("name")).Value, datum.Attribute(XName.Get("proj4")).Value);
            return _datumItems;
        }

        public List<NameMapItem> GetPrjParameterItems()
        {
            if (_prjParameterItems != null)
                return _prjParameterItems;
            _prjParameterItems = new List<NameMapItem>();
            IEnumerable<XElement> prjParamters = _root.Element(XName.Get("PrjParamters")).Elements(XName.Get("PrjParameter"));
            NameMapItem nameMapItem = null;
            foreach (XElement prjParamter in prjParamters)
            {
                nameMapItem = CreatNameMapItem(prjParamter);
                _prjParameterItems.Add(nameMapItem);
            }
            return _prjParameterItems;
        }

        public List<NameMapItem> GetPrjNameMapItems()
        {
            if (_prjNameMapItems != null)
                return _prjNameMapItems;
            _prjNameMapItems = new List<NameMapItem>();
            IEnumerable<XElement> projects = GetProjectNodes();
            NameMapItem nameMapItem = null;
            foreach (XElement project in projects)
            {
                nameMapItem = CreatNameMapItem(project);
                _prjNameMapItems.Add(nameMapItem);
            }
            return _prjNameMapItems;
        }

        private NameMapItem CreatNameMapItem(XElement prjParamter)
        {
            NameMapItem nameMapItem = new NameMapItem(prjParamter.Attribute(XName.Get("name")).Value,
                                                      prjParamter.Attribute(XName.Get("esri")).Value,
                                                      prjParamter.Attribute(XName.Get("wkt")).Value,
                                                      prjParamter.Attribute(XName.Get("proj4")).Value,
                                                      prjParamter.Attribute(XName.Get("epsg")).Value,
                                                      prjParamter.Attribute(XName.Get("envi")).Value,
                                                      prjParamter.Attribute(XName.Get("geotiff")).Value);
            return nameMapItem;
        }

        public List<CoordinateDomain> GetCoordinateDomainItems()
        {
            List<CoordinateDomain> coordinateDomainItems = new List<CoordinateDomain>();
            IEnumerable<XElement> projects = GetProjectNodes();
            foreach (XElement project in projects)
            {
                XElement coordDomain = project.Element(XName.Get("CoordDomain"));
                if (!String.IsNullOrEmpty(coordDomain.Attribute(XName.Get("minx")).Value) &&
                    !String.IsNullOrEmpty(coordDomain.Attribute(XName.Get("maxx")).Value) &&
                    !String.IsNullOrEmpty(coordDomain.Attribute(XName.Get("miny")).Value) &&
                    !String.IsNullOrEmpty(coordDomain.Attribute(XName.Get("maxy")).Value))
                {
                    CoordinateDomain coordinateDomain = null;
                    coordinateDomain = CreatCoordinateDomain(coordDomain);
                    coordinateDomainItems.Add(coordinateDomain);
                }
            }
            return coordinateDomainItems;
        }

        public string GetDatumNameByPrj4DatumName(string prj4Name)
        {
            if (_datumItems == null)
                GetDatumsItems();
            foreach (string datumName in _datumItems.Keys)
                if (_datumItems[datumName] == prj4Name)
                    return datumName;
            return null;
        }

        #region 通过投影名获取投影坐标系统的NameMapItem对象
        public NameMapItem GetNameItem(Func<NameMapItem, bool> finder)
        {
            if (_prjNameMapItems == null)
                GetPrjNameMapItems();
            foreach (NameMapItem item in _prjNameMapItems)
            {
                if (finder(item))
                    return item;
            }
            return null;
        }

        public NameMapItem GetPrjNameItemByEnviName(string name)
        {
            return GetNameItem(it => it.ENVIName == name);
        }

        public NameMapItem GetPrjNameItemByPrj4(string name)
        {
            return GetNameItem(it => it.Proj4Name == name);
        }

        public NameMapItem GetPrjNameItemByName(string name)
        {
            return GetNameItem(it => it.Name == name);
        }

        public NameMapItem GetPrjNameItemByWktName(string name)
        {
            return GetNameItem(it => it.WktName == name);
        }

        public NameMapItem GetPrjNameItemByEsriName(string name)
        {
            return GetNameItem(it => it.EsriName == name);
        }

        public NameMapItem GetPrjNameItemByEPSGName(string name)
        {
            return GetNameItem(it => it.EPSGName == name);
        }

        public NameMapItem GetPrjNameItemByGeoTiffName(string name)
        {
            return GetNameItem(it => it.GeoTiffName == name);
        }
        #endregion

        #region 通过参数名字获取参数NameMapItem对象
        public NameMapItem GetPrjParamterItem(Func<NameMapItem, bool> finder)
        {
            if (_prjParameterItems == null)
                GetPrjParameterItems();
            foreach (NameMapItem item in _prjParameterItems)
            {
                if (finder(item))
                    return item;
            }
            return null;
        }

        public NameMapItem GetPrjParamterItemByPrj4(string name)
        {
            return GetPrjParamterItem(it => it.Proj4Name == name);
        }

        public NameMapItem GetPrjParamterItemByName(string name)
        {
            return GetPrjParamterItem(it => it.Name == name);
        }

        public NameMapItem GetPrjParamterItemByWktName(string name)
        {
            return GetPrjParamterItem(it => it.WktName == name);
        }

        public NameMapItem GetPrjParamterItemByEsriName(string name)
        {
            return GetPrjParamterItem(it => it.EsriName == name);
        }

        public NameMapItem GetPrjParamterItemByEnviName(string name)
        {
            return GetPrjParamterItem(it => it.ENVIName == name);
        }

        public NameMapItem GetPrjParamterItemByEPSGName(string name)
        {
            return GetPrjParamterItem(it => it.EPSGName == name);
        }

        public NameMapItem GetPrjParamterItemByGeoTiffName(string name)
        {
            return GetPrjParamterItem(it => it.GeoTiffName == name);
        }
        #endregion

        #region 通过投影名获取参数名数组
        public string[] GetEnviPrjInfoArgDefByPrjName(string name)
        {             
            foreach (EnviPrjInfoArgDef prjInfoArgsDef in EnviPrjInfoArgDefs)
            {
                if (prjInfoArgsDef.PrjName == name)
                    return prjInfoArgsDef.Args;
            }
            return null;
        }
        #endregion

        private CoordinateDomain CreatCoordinateDomain(XElement coordDomain)
        {
            CoordinateDomain coordinateDomain = new CoordinateDomain(Convert.ToDouble(coordDomain.Attribute(XName.Get("minx")).Value),
                                                                     Convert.ToDouble(coordDomain.Attribute(XName.Get("maxx")).Value),
                                                                     Convert.ToDouble(coordDomain.Attribute(XName.Get("miny")).Value),
                                                                     Convert.ToDouble(coordDomain.Attribute(XName.Get("maxy")).Value));
            return coordinateDomain;
        }

        private IEnumerable<XElement> GetProjectNodes()
        {
            IEnumerable<XElement> projects = _root.Element(XName.Get("Projects")).Elements(XName.Get("Project"));
            return projects;
        }

        public void Dispose()
        {
        }
    }
}
