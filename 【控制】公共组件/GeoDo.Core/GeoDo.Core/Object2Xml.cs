using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;
using System.IO;
using System.Runtime.Remoting;
using System.Collections;

namespace GeoDo.Core
{
    public class Object2Xml : IObject2Xml
    {
        public static IPersistContextEnvironment PersistContextEnv;

        static Object2Xml()
        {
            PersistContextEnv = new PersistContextEnvironment();
        }

        public IPersistContextEnvironment PersistContextEnvironment
        {
            get { return Object2Xml.PersistContextEnv; }
        }

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
            GeoDo.Core.XmlConstructorAttribute ctorAtt = GetCtorAttribute(assembly, type);
            object obj = null;
            if (ctorAtt == null)
                obj = Activator.CreateInstance(assembly, type);
            else
            {
                Assembly ass = Assembly.Load(assembly);
                Type t = ass.GetType(type);
                object[] ctorArgs = GetCotorArgs(ctorAtt);
                obj = Activator.CreateInstance(t, ctorArgs);
            }
            if (obj is ObjectHandle)
                obj = (obj as ObjectHandle).Unwrap();
            GeoDo.Core.XmlContextVarAttribute xmlContextVarAtt = ContextVarAttribute(obj);
            if (xmlContextVarAtt != null)
                Object2Xml.PersistContextEnv.Put(xmlContextVarAtt.VarName, obj);
            foreach (XElement pElement in proElements)
            {
                if (ReadValueTypeProperty(pElement, obj))
                    continue;
                ReadComplexProperty(pElement, obj);
            }
            TryDoLoadAction(obj);
            return obj;
        }

        private void TryDoLoadAction(object obj)
        {
            IActionAtContructAfter a = obj as IActionAtContructAfter;
            if (a != null)
                a.Load();
        }

        private object[] GetCotorArgs(XmlConstructorAttribute ctorAtt)
        {
            if (ctorAtt == null || ctorAtt.VarNameOfCtorArgs == null)
                return null;
            List<object> args = new List<object>();
            foreach (string varName in ctorAtt.VarNameOfCtorArgs)
            {
                object v = Object2Xml.PersistContextEnv.Get(varName);
                args.Add(v);
            }
            return args.Count > 0 ? args.ToArray() : null;
        }

        private XmlConstructorAttribute GetCtorAttribute(string assembly, string type)
        {
            Assembly ass = Assembly.Load(assembly);
            Type t = ass.GetType(type);
            object[] atts = t.GetCustomAttributes(typeof(GeoDo.Core.XmlConstructorAttribute), true);
            if (atts == null || atts.Length == 0)
                return null;
            return atts[0] as XmlConstructorAttribute;
        }

        private void ReadComplexProperty(XElement pElement, object obj)
        {
            string proName = pElement.Name.LocalName;
            PropertyInfo pInfo = obj.GetType().GetProperty(proName);
            object[] atts = pInfo.GetCustomAttributes(typeof(GeoDo.Core.XmlPersistAttribute), true);
            if (atts == null || atts.Length == 0)
                return;
            GeoDo.Core.XmlPersistAttribute xmlPersisitAtt = atts[0] as GeoDo.Core.XmlPersistAttribute;
            GeoDo.Core.XmlContextVarAttribute xmlContextVarAtt = ContextVarAttribute(pInfo);
            object value = null;
            if (xmlPersisitAtt.PropertyConverter != null)
            {
                IPropertyConverter convter = Activator.CreateInstance(xmlPersisitAtt.PropertyConverter) as IPropertyConverter;
                convter.HostObject = obj;
                value = convter.FromXml(pElement);
            }
            else
            {
                var eles = pElement.Elements();
                var elesArray = eles.ToArray();
                int count = eles.Count();
                if (count > 0)
                {

                    if (pInfo.PropertyType.IsArray)
                    {
                        Array arry = Array.CreateInstance(xmlPersisitAtt.CollectionItemType, count);
                        for (int i = 0; i < count; i++)
                            arry.SetValue(FromXml(elesArray[i]), i);
                        value = arry;
                    }
                    else if (PropertyIsList(pInfo))
                    {
                        Type type = typeof(List<>);
                        Type type1 = type.MakeGenericType(xmlPersisitAtt.CollectionItemType);
                        IList lst = Activator.CreateInstance(type1) as IList;
                        for (int i = 0; i < count; i++)
                            lst.Add(FromXml(elesArray[i]));
                        value = lst;
                    }
                    else
                    {
                        value = FromXml(elesArray[0]);
                    }
                }
            }
            //
            if (obj.GetType().GetMethod("set_" + proName) != null)
                obj.GetType().InvokeMember(proName, BindingFlags.SetProperty, null, obj, new object[] { value });
            //
            if (xmlContextVarAtt != null)
            {
                Object2Xml.PersistContextEnv.Put(xmlContextVarAtt.VarName, value);
            }
        }

        private XmlContextVarAttribute ContextVarAttribute(object obj)
        {
            object[] atts = obj.GetType().GetCustomAttributes(typeof(GeoDo.Core.XmlContextVarAttribute), true);
            if (atts == null || atts.Length == 0)
                return null;
            return atts[0] as XmlContextVarAttribute;
        }

        private XmlContextVarAttribute ContextVarAttribute(PropertyInfo pInfo)
        {
            object[] atts = pInfo.GetCustomAttributes(typeof(GeoDo.Core.XmlContextVarAttribute), true);
            if (atts == null || atts.Length == 0)
                return null;
            return atts[0] as XmlContextVarAttribute;
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
            if (obj.GetType().GetMethod("set_" + proName) != null)
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
            object[] atts = pInfo.GetCustomAttributes(typeof(GeoDo.Core.XmlPersistAttribute), true);
            if (atts == null || atts.Length == 0)
                return ValueTypePropertyToXml(obj, pInfo);
            GeoDo.Core.XmlPersistAttribute xmlPersistAtt = atts[0] as GeoDo.Core.XmlPersistAttribute;
            if (!xmlPersistAtt.IsNeedPersisted)
                return null;
            IPropertyConverter convter = null;
            try
            {
                object propertyValue = obj.GetType().InvokeMember(pInfo.Name, BindingFlags.GetProperty, null, obj, null);
                if (propertyValue != null)
                {
                    if (xmlPersistAtt.PropertyConverter != null)
                        convter = Activator.CreateInstance(xmlPersistAtt.PropertyConverter) as IPropertyConverter;
                    if (pInfo.PropertyType.IsArray)
                    {
                        XElement containerXml = new XElement(pInfo.Name);
                        Array array = propertyValue as Array;
                        GenerateCollectionItemXElement(pInfo, array as IList, convter, xmlPersistAtt, containerXml);
                        return containerXml;
                    }
                    else if (PropertyIsList(pInfo))
                    {
                        XElement containerXml = new XElement(pInfo.Name);
                        IList lst = propertyValue as IList;
                        GenerateCollectionItemXElement(pInfo, lst, convter, xmlPersistAtt, containerXml);
                        return containerXml;
                    }
                    else
                    {
                        return convter.ToXml(pInfo.Name, propertyValue);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private void GenerateCollectionItemXElement(PropertyInfo pInfo, IList items, IPropertyConverter convter, XmlPersistAttribute persistAtt, XElement containerXml)
        {
            foreach (object it in items)
            {
                XElement ele = null;
                if (convter != null)
                    ele = convter.ToXml(persistAtt.CollectionItemName, it);
                else
                    ele = ToXml(it);
                if (ele != null)
                    containerXml.Add(ele);
            }
        }

        private bool PropertyIsList(PropertyInfo pInfo)
        {
            return pInfo.PropertyType.GetInterface("IList") != null;
        }

        private XElement ValueTypePropertyToXml(object obj, PropertyInfo pInfo)
        {
            if (!pInfo.PropertyType.IsValueType && !pInfo.PropertyType.Equals(typeof(string)))
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
            else if (pInfo.PropertyType.Equals(typeof(double)))
                ele.SetAttributeValue("type", typeof(double).ToString());
            else if (pInfo.PropertyType.Equals(typeof(byte)))
                ele.SetAttributeValue("type", typeof(byte).ToString());
            return ele;
        }
    }
}
