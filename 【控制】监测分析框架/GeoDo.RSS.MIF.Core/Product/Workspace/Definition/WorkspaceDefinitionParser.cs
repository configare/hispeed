using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public class WorkspaceDefinitionParser
    {
        public WorkspaceDefinitionParser()
        {
        }

        public WorkspaceDef[] Parse()
        {
            string dir = System.AppDomain.CurrentDomain.BaseDirectory + @"SystemData\Workspace\";
            WorkspaceDef[] defs = Load(dir);
            return defs;
        }

        private WorkspaceDef[] Load(string defDir)
        {
            if (!Directory.Exists(defDir))
                return null;
            string[] files = Directory.GetFiles(defDir, "*.xml", SearchOption.AllDirectories);
            List<WorkspaceDef> defs = new List<WorkspaceDef>();
            foreach (string file in files)
            {
                WorkspaceDef def = LoadConfig(file);
                if (def != null)
                    defs.Add(def);
            }
            if (defs.Count == 0)
                return null;
            else
                return defs.ToArray();
        }

        private WorkspaceDef LoadConfig(string workspaceDef)
        {
            XDocument doc = XDocument.Load(workspaceDef);
            if (doc == null)
                return null;
            XElement ele = doc.Element("Workspace");
            if(ele==null)
                return null;
            WorkspaceDef wksDef = GetWorkspaceDef(ele);
            return wksDef;
        }
        
        private WorkspaceDef GetWorkspaceDef(XElement ele)
        {
            string text = GetStringAttValue(ele, "text");
            string identify = GetStringAttValue(ele, "identify");
            bool enabled = GetBoolAttValue(ele, "isenabled");
            WorkspaceDef wks = new WorkspaceDef(text, identify, enabled);
            var eles = ele.Elements();
            if (eles != null)
            {
                foreach (XElement e in eles)
                {
                    CatalogDef catalog = GetCatalogDef(e);
                    if (catalog != null)
                        wks.CatalogDefs.Add(catalog);
                }
            }
            return wks;
        }

        private CatalogDef GetCatalogDef(XElement ele)
        {
            if (ele == null)
                return null;
            string name = ele.Name.LocalName.ToUpper();
            switch (name)
            {
                case "EXTRACTCATALOG"://ExtractCatalog
                    return GetExtractCatalog(ele);
                case "CATALOG":
                    return GetCatalog(ele);
            }
            return null;
        }

        private CatalogDef GetCatalog(XElement ele)
        {
            string text = GetStringAttValue(ele, "text");
            string classString = GetStringAttValue(ele, "class");
            string identify = GetStringAttValue(ele, "identify");
            string filter = GetStringAttValue(ele, "filter");
            string pattern = GetStringAttValue(ele, "pattern");
            string folder = GetStringAttValue(ele, "folder");
            SubProductCatalogDef catalog = new SubProductCatalogDef(classString, text, identify,filter,pattern,folder);
            if (ele.Element("Attributes") != null)
            {
                var xeles = ele.Element("Attributes").Elements("Attribute");
                if (xeles != null)
                {
                    foreach (XElement e in xeles)
                    {
                        CatalogAttriteDef attDef = GetCatalogAttributeDdef(e);
                        if (attDef != null)
                            catalog.AttributeDefs.Add(attDef);
                    }
                }
            }
            return catalog;
        }

        private CatalogAttriteDef GetCatalogAttributeDdef(XElement e)
        {
            string text = GetStringAttValue(e, "text");
            string identify = GetStringAttValue(e, "identify");
            string format = GetStringAttValue(e, "format");
            bool visible = GetBoolAttValue(e, "visible");
            return new CatalogAttriteDef(text, identify, format, visible);
        }

        private CatalogDef GetExtractCatalog(XElement ele)
        {
            var eles = ele.Elements();
            if (eles == null)
                return null;
            string text = GetStringAttValue(ele, "text");
            string classString = GetStringAttValue(ele, "class");
            string identify = GetStringAttValue(ele, "identify");
            List<CatalogNodeItemDef> nodes = new List<CatalogNodeItemDef>();
            foreach (XElement e in eles)
            {
                string name = e.Name.LocalName.ToUpper();
                switch (name)
                {
                    case "CATALOGNODEGROUP":
                        CatalogNodeGroupDef group = GetGroupNode(e);
                        if (group != null)
                        {
                            nodes.Add(group);
                            GetSubNode(group, e);
                        }
                        break;
                    case "CATALOGNODE":
                        CatalogNodeDef node = GetNode(e);
                        if (node != null)
                            nodes.Add(node);
                        break;
                }
            }
            ExtractingCatalogDef catalog = new ExtractingCatalogDef(classString, text, identify);
            catalog.NodeDefs = nodes;
            return catalog;
        }

        private void GetSubNode(CatalogNodeGroupDef groupNode, XElement groupEle)
        {
            var eles = groupEle.Elements();
            if (eles == null)
                return;
            foreach (XElement e in eles)
            {
                string name = e.Name.LocalName.ToUpper();
                switch (name)
                {
                    case "CATALOGNODEGROUP":
                        CatalogNodeGroupDef group = GetGroupNode(e);
                        if (group != null)
                        {
                            groupNode.NodeDefs.Add(group);
                            GetSubNode(group, e);
                        }
                        break;
                    case "CATALOGNODE":
                        CatalogNodeDef node = GetNode(e);
                        if (node != null)
                            groupNode.NodeDefs.Add(node);
                        break;
                }
            }
        }

        private CatalogNodeDef GetNode(XElement e)
        {
            string text = GetStringAttValue(e, "text");
            string identify = GetStringAttValue(e, "identify");
            string format = GetStringAttValue(e, "format");
            return new CatalogNodeDef(text, format, identify);
        }

        private CatalogNodeGroupDef GetGroupNode(XElement e)
        {
            string text = GetStringAttValue(e, "text");
            string identify = GetStringAttValue(e, "identify");
            string format = GetStringAttValue(e, "format");
            string pattern = GetStringAttValue(e, "pattern");
            CatalogNodeGroupDef group = new CatalogNodeGroupDef(text, format, identify,pattern);
            return group;
        }

        public string GetStringAttValue(XElement ele, string attName)
        {
            if (ele == null)
                return string.Empty;
            var v = ele.Attribute(attName);
            if (v == null)
                return string.Empty;
            return v.Value ?? string.Empty;
        }

        public bool GetBoolAttValue(XElement ele, string attName)
        {
            if (ele == null)
                return false;
            var v = ele.Attribute(attName);
            if (v == null)
                return false;
            bool b;
            if (bool.TryParse(v.Value, out b))
                return b;
            return false;
        }
    }
}
