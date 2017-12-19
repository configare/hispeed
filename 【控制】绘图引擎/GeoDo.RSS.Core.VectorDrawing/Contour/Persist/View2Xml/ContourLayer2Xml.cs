using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;
using System.IO;

namespace GeoDo.RSS.Core.VectorDrawing
{
    internal class ContourLayer2Xml
    {
        public IContourLayer FromXml(string fname)
        {
            if (string.IsNullOrEmpty(fname) || !File.Exists(fname))
                return null;
            XDocument doc = XDocument.Load(fname);
            return FromXml(doc.Root);
        }

        public IContourLayer FromXml(XElement element)
        {
            var proElements = element.Elements();
            if (proElements == null)
                return null;
            IContourLayer lyr = new ContourLayer(PropertyConverter.AttToString(element, "layername"));
            foreach (XElement pElement in proElements)
            {
                if (ReadValueTypeProperty(pElement, lyr))
                    continue;
                ReadComplexProperty(pElement, lyr);
            }
            return lyr;
        }

        private void ReadComplexProperty(XElement pElement, IContourLayer lyr)
        {
            string proName = pElement.Name.LocalName;
            PropertyInfo pInfo = lyr.GetType().GetProperty(proName);
            object[] atts = pInfo.GetCustomAttributes(typeof(XmlPersistAttribute), false);
            XmlPersistAttribute xmlPersisitAtt = atts[0] as XmlPersistAttribute;
            IPropertyConverter convter = Activator.CreateInstance(xmlPersisitAtt.PropertyConverter) as IPropertyConverter;
            object value = convter.FromXml(pElement);
            lyr.GetType().InvokeMember(proName, BindingFlags.SetProperty, null, lyr, new object[] { value });
        }

        private bool ReadValueTypeProperty(XElement pElement, IContourLayer lyr)
        {
            string proName = pElement.Name.LocalName;
            string type = PropertyConverter.AttToString(pElement, "type");
            object value = null;
            if (type == typeof(bool).ToString())
                value = PropertyConverter.AttToBool(pElement, "value");
            else if (type == typeof(int).ToString())
                value = PropertyConverter.AttToInt(pElement, "value");
            else if (type == typeof(string).ToString())
                value = PropertyConverter.AttToString(pElement, "value");
            else if (type == typeof(float).ToString())
                value = PropertyConverter.AttToFloat(pElement, "value");
            else if (type == typeof(byte).ToString())
                value = PropertyConverter.AttToByte(pElement, "value");
            else if (type == typeof(double).ToString())
                value = PropertyConverter.AttToDouble(pElement, "value");
            else
                return false;
            lyr.GetType().InvokeMember(proName, BindingFlags.SetProperty, null, lyr, new object[] { value });
            return true;
        }

        public void ToXmlFile(IContourLayer contourLayer, string fname)
        {
            XElement rootElement = ToXml(contourLayer);
            if (rootElement == null)
                return;
            XDocument doc = new XDocument(rootElement);
            doc.Save(fname);
        }

        public XElement ToXml(IContourLayer contourLayer)
        {
            if (contourLayer == null)
                return null;
            PropertyInfo[] proInfos = contourLayer.GetType().GetProperties();
            if (proInfos == null || proInfos.Length == 0)
                return null;
            XElement rootElement = new XElement("ContourLayer");
            rootElement.SetAttributeValue("layername", contourLayer.Name ?? string.Empty);
            foreach (PropertyInfo pInfo in proInfos)
            {
                XElement pEle = ToXml(contourLayer, pInfo);
                if (pEle != null)
                    rootElement.Add(pEle);
            }
            return rootElement;
        }

        private XElement ToXml(object obj, PropertyInfo pInfo)
        {
            if (pInfo == null)
                return null;
            object[] atts = pInfo.GetCustomAttributes(typeof(XmlPersistAttribute), false);
            if (atts == null || atts.Length == 0)
                return ValueTypePropertyToXml(obj, pInfo);
            XmlPersistAttribute xlmPersistAtt = atts[0] as XmlPersistAttribute;
            IPropertyConverter convter = null;
            try
            {
                convter = Activator.CreateInstance(xlmPersistAtt.PropertyConverter) as IPropertyConverter;
                object propertyValue = obj.GetType().InvokeMember(pInfo.Name, BindingFlags.GetProperty, null, obj, null);
                if (propertyValue != null)
                    return convter.ToXml(pInfo.Name, propertyValue);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private XElement ValueTypePropertyToXml(object obj, PropertyInfo pInfo)
        {
            if (!pInfo.PropertyType.IsValueType)
                return null;
            XElement ele = new XElement(pInfo.Name);
            object propertyValue = obj.GetType().InvokeMember(pInfo.Name, BindingFlags.GetProperty, null, obj, null);
            ele.SetAttributeValue("value", propertyValue);
            if (pInfo.PropertyType.Equals(typeof(bool)))
                ele.SetAttributeValue("type", typeof(bool).ToString());
            else if (pInfo.PropertyType.Equals(typeof(float)))
                ele.SetAttributeValue("type", typeof(float).ToString());
            else if (pInfo.PropertyType.Equals(typeof(int)))
                ele.SetAttributeValue("type", typeof(int).ToString());
            else if (pInfo.PropertyType.Equals(typeof(string)))
                ele.SetAttributeValue("type", typeof(string).ToString());
            else if (pInfo.PropertyType.Equals(typeof(byte)))
                ele.SetAttributeValue("type", typeof(byte).ToString());
            else if (pInfo.PropertyType.Equals(typeof(double)))
                ele.SetAttributeValue("type", typeof(double).ToString());
            return ele;
        }
    }
}
