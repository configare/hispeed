using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Core
{
    internal class MapToXML:IDisposable
    {
        public MapToXML()
        { 
        }

        public void SaveTo(IMap map, string filename)
        {
            if (map == null || string.IsNullOrEmpty(filename))
                return;
            try
            {
                XmlDocument doc = new XmlDocument();
                IPersistable persist = map as IPersistable;
                PersistObject mapObj = persist.ToPersistObject();
                XmlNode mapNode = GetXmlNode(doc, mapObj);
                doc.AppendChild(mapNode);
                if (mapObj.SubNodes != null)
                {
                    foreach (PersistObject sub in mapObj.SubNodes)
                    {
                        try
                        {
                            PersistObjectToXmlNode(doc, mapNode, sub);
                        }
                        catch (Exception ex)
                        {
                            Log.WriterException("MapToXML", "SaveTo", ex);
                        }
                    }
                }
                //
                XmlDeclaration xmldecl = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmldecl, root);
                //
                doc.Save(filename);
            }
            catch (Exception ex)
            {
                Log.WriterException("MapToXML", "SaveTo", ex);
                throw;
            }
        }

        private void PersistObjectToXmlNode(XmlDocument doc,XmlNode parentNode, PersistObject obj)
        {
            if(obj == null)
                return ;
            XmlNode node = GetXmlNode(doc, obj);
            parentNode.AppendChild(node);
            //
            if (obj.SubNodes != null)
                foreach (PersistObject subobj in obj.SubNodes)
                    PersistObjectToXmlNode(doc, node, subobj);
        }

        private XmlNode GetXmlNode(XmlDocument doc, PersistObject obj)
        {
            if (obj == null)
                return null;
            XmlNode node = doc.CreateNode(XmlNodeType.Element, obj.Name,null);
            if (obj.Attributes != null)
            {
                foreach (KeyValuePair<string, string> att in obj.Attributes)
                {
                    if (string.IsNullOrEmpty(att.Key))
                        continue;
                    XmlAttribute attNode = doc.CreateAttribute(att.Key);
                    if (att.Value == null)
                        attNode.Value = string.Empty;
                    else
                        attNode.Value = att.Value;
                    node.Attributes.Append(attNode);
                }
            }
            return node;
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
