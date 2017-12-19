using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;
using System.IO;
using System.Runtime.Remoting;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class Object2Xml
    {
        public object FromXml(string fname)
        {
            if (string.IsNullOrEmpty(fname) || !File.Exists(fname))
                return null;
            XDocument doc = XDocument.Load(fname);
            return FromXml(doc.Root);
        }

        public object FromXml(XElement element)
        {
            var proElements = element.Elements();
            if (proElements == null)
                return null;
            string assembly = PropertyConverter.AttToString(element, "assembly");
            string type = PropertyConverter.AttToString(element, "type");
            object obj = Activator.CreateInstance(assembly,type);
            if (obj is ObjectHandle)
                obj = (obj as ObjectHandle).Unwrap();
            foreach (XElement pElement in proElements)
            {
                if (ReadValueTypeProperty(pElement, obj))
                    continue;
                ReadComplexProperty(pElement, obj);
            }
            return obj;
        }

        private void ReadComplexProperty(XElement pElement, object obj)
        {
            string proName = pElement.Name.LocalName;
            PropertyInfo pInfo = obj.GetType().GetProperty(proName);
            object[] atts = pInfo.GetCustomAttributes(typeof(XmlPersistAttribute), false);
            XmlPersistAttribute xmlPersisitAtt = atts[0] as XmlPersistAttribute;
            IPropertyConverter convter = Activator.CreateInstance(xmlPersisitAtt.PropertyConverter) as IPropertyConverter;
            object value = convter.FromXml(pElement);
            obj.GetType().InvokeMember(proName, BindingFlags.SetProperty, null, obj, new object[] { value });
        }

        private bool ReadValueTypeProperty(XElement pElement, object obj)
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
            obj.GetType().InvokeMember(proName, BindingFlags.SetProperty, null, obj, new object[] { value });
            return true;
        }

        public void ToXmlFile(object obj, string fname)
        {
            XElement rootElement = ToXml(obj);
            if (rootElement == null)
                return;
            XDocument doc = new XDocument(rootElement);
            doc.Save(fname);
        }

        public XElement ToXml(object obj)
        {
            if (obj == null)
                return null;
            PropertyInfo[] proInfos = obj.GetType().GetProperties();
            if (proInfos == null || proInfos.Length == 0)
                return null;
            XElement rootElement = new XElement(obj.GetType().Name);
            rootElement.SetAttributeValue("assembly", obj.GetType().Assembly.FullName);
            rootElement.SetAttributeValue("type", obj.GetType().FullName);
            foreach (PropertyInfo pInfo in proInfos)
            {
                XElement pEle = ToXml(obj, pInfo);
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
            return ele;
        }
    }
}
