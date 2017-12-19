using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.Core
{
    public abstract class PropertyConverter : IPropertyConverter
    {
        protected object _hostObject = null;

        public object HostObject
        {
            get { return _hostObject; }
            set { _hostObject = value; }
        }

        public XElement ToXml(string name, object propertyValue)
        {
            if (string.IsNullOrEmpty(name) || propertyValue == null)
                return null;
            XElement ele = new XElement(name);
            SetAttributes(ele,propertyValue);
            return ele;
        }

        protected abstract void SetAttributes(XElement ele, object propertyValue);

        public object FromXml(XElement propertyXml)
        {
            if (propertyXml == null)
                return null;
            return CreateAndFillObject(propertyXml);
        }

        protected abstract object CreateAndFillObject(XElement propertyXml);

        public static byte AttToByte(XElement ele, string attName)
        {
            if (ele.Attribute(attName) == null)
                return 0;
            byte v = 0;
            if (byte.TryParse(ele.Attribute(attName).Value, out v))
                return v;
            return 0;
        }

        public static int AttToInt(XElement ele, string attName)
        {
            if (ele.Attribute(attName) == null)
                return 0;
            int v = 0;
            if (int.TryParse(ele.Attribute(attName).Value, out v))
                return v;
            return 0;
        }

        public static float AttToFloat(XElement ele, string attName)
        {
            if (ele.Attribute(attName) == null)
                return 0;
            float v = 0;
            if (float.TryParse(ele.Attribute(attName).Value, out v))
                return v;
            return 0;
        }

        public static double AttToDouble(XElement ele, string attName)
        {
            if (ele.Attribute(attName) == null)
                return 0;
            double v = 0;
            if (double.TryParse(ele.Attribute(attName).Value, out v))
                return v;
            return 0;
        }

        public static string AttToString(XElement ele, string attName)
        {
            if (ele.Attribute(attName) == null)
                return null;
            return ele.Attribute(attName).Value.ToString();
        }

        public static bool AttToBool(XElement ele, string attName)
        {
            if (ele.Attribute(attName) == null)
                return false;
            bool v = false;
            if (bool.TryParse(ele.Attribute(attName).Value, out v))
                return v;
            return false;
        }
    }
}
