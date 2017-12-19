using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.Core;
using CodeCell.AgileMap.Core;
using System.Xml.Linq;
using System.IO;

namespace GeoDo.RSS.Core.VectorDrawing
{
    public class MapPropertyConverter : PropertyConverter
    {
        protected override object CreateAndFillObject(System.Xml.Linq.XElement propertyXml)
        {
            XElement mcdElement = propertyXml.Element("McdConfig").Element("Map");
            if (mcdElement == null)
                return null;
            IMap map = null;
            string tempFile = AppDomain.CurrentDomain.BaseDirectory + "temp.mcd";
            try
            {
                XDocument doc = new XDocument();
                doc.Add(mcdElement);
                doc.Save(tempFile);
                map = MapFactory.LoadMapFrom(tempFile);
            }
            finally
            {
                File.Delete(tempFile);
            }
            return map;
        }

        protected override void SetAttributes(System.Xml.Linq.XElement ele, object propertyValue)
        {
            IMap map = propertyValue as IMap;
            if (map == null || map.LayerContainer== null || map.LayerContainer.Layers == null || map.LayerContainer.Layers.Length == 0)
                return;
            XElement mcdElement = new XElement("McdConfig");
            string tempFile = AppDomain.CurrentDomain.BaseDirectory+"temp.mcd";
            try
            {
                map.SaveTo(tempFile, false);
                XDocument doc = XDocument.Load(tempFile);
                mcdElement.Add(doc.Root);
                ele.Add(mcdElement);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}
