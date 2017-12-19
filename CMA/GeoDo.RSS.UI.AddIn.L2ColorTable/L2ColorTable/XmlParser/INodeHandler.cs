using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.L2ColorTable
{
    public interface INodeHandler
    {
        object XmlNodeToObject(XmlNode node);
        XmlNode ObjectToXmlNode(object obj);
    }

    public abstract class NodeHandler:INodeHandler
    {
        #region INodeHandler Members

        public abstract object XmlNodeToObject(XmlNode node);

        public abstract XmlNode ObjectToXmlNode(object obj);

        #endregion

        public static string NodeAtt2String(XmlNode xmlNode, string attName)
        {
            try
            {
                if (xmlNode.Attributes == null || xmlNode.Attributes[attName]==null)
                    return string.Empty;
                return xmlNode.Attributes[attName].Value;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static float NodeAtt2Float(XmlNode xmlNode, string attName)
        {
            float returnValue = float.NaN;
            return float.TryParse(NodeAtt2String(xmlNode, attName), out returnValue) ? returnValue : 0;
        }

        public static double NodeAtt2Double(XmlNode xmlNode, string attName)
        {
            double returnValue = double.NaN;
            return double.TryParse(NodeAtt2String(xmlNode, attName), out returnValue) ? returnValue : 0;
        }

        public static int NodeAtt2Int(XmlNode xmlNode, string attName)
        {
            int returnValue = 0;
            return int.TryParse(NodeAtt2String(xmlNode, attName), out returnValue) ? returnValue : 0;
        }

        public static Font NodeAtt2Font(XmlNode xmlNode, string attName)
        {
            try
            {
                return new Font(NodeAtt2String(xmlNode, attName), 9);
            }
            catch { }
            return new Font("宋体", 9);
        }

        public static bool NodeAtt2Bool(XmlNode xmlNode, string attName)
        {
            bool returnValue = false;
            return bool.TryParse(NodeAtt2String(xmlNode, attName), out returnValue) ? returnValue : false;
        }

        public static TimeSpan NodeAtt2TimpSpan(XmlNode xmlNode, string attName)
        {
            TimeSpan returnValue = TimeSpan.Zero;
            return TimeSpan.TryParse(NodeAtt2String(xmlNode, attName), out returnValue) ? returnValue : TimeSpan.Zero;
        }

        public static Color NodeAtt2Color(XmlNode xmlNode, string attName)
        {
            try
            {
                string cstr = NodeAtt2String(xmlNode, attName);
                if (string.IsNullOrEmpty(cstr))
                    return Color.Empty;
                string[] rgbs = cstr.Split(new char[] { ',' });
                if (rgbs.Length == 3)
                    return Color.FromArgb(int.Parse(rgbs[0]), int.Parse(rgbs[1]), int.Parse(rgbs[2]));
                if (rgbs.Length == 4)
                    return Color.FromArgb(int.Parse(rgbs[0]), int.Parse(rgbs[1]), int.Parse(rgbs[2]), int.Parse(rgbs[3]));
                return Color.Empty;
            }
            catch
            {
                return Color.Empty;
            }
        }

        //public static masSatelliteTypes NodeAtt2MasSatelliteTypes(XmlNode xmlNode, string attName)
        //{
        //    try
        //    {
        //        return (masSatelliteTypes)Enum.Parse(typeof(masSatelliteTypes), NodeAtt2String(xmlNode, attName), true);
        //    }
        //    catch
        //    {
        //        return masSatelliteTypes.Unknow;
        //    }
        //}

        //public static masSensorTypes NodeAtt2MasSensorTypes(XmlNode xmlNode, string attName)
        //{
        //    try
        //    {
        //        return (masSensorTypes)Enum.Parse(typeof(masSensorTypes), NodeAtt2String(xmlNode, attName), true);
        //    }
        //    catch 
        //    {
        //        return masSensorTypes.Unknow;
        //    }
        //}

        public static Type NodeAtt2Type(XmlNode xmlNode, string attName)
        {
            try
            {
                TypeCode typeCode = (TypeCode)Enum.Parse(typeof(TypeCode), NodeAtt2String(xmlNode, attName), true);
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        return typeof(Boolean);
                    case TypeCode.Byte:
                        return typeof(Byte);
                    case TypeCode.Char:
                        return typeof(Char);
                    case TypeCode.DBNull:
                        return typeof(DBNull);
                    case TypeCode.DateTime:
                        return typeof(DateTime);
                    case TypeCode.Decimal:
                        return typeof(Decimal);
                    case TypeCode.Double:
                        return typeof(Double);
                    case TypeCode.Empty:
                        return null;
                    case TypeCode.Int16:
                        return typeof(Int16);
                    case TypeCode.Int32:
                        return typeof(Int32);
                    case TypeCode.Int64:
                        return typeof(Int64);
                    case TypeCode.Object:
                        return typeof(Object);
                    case TypeCode.SByte:
                        return typeof(SByte);
                    case TypeCode.Single:
                        return typeof(Single);
                    case TypeCode.String:
                        return typeof(String);
                    case TypeCode.UInt16:
                        return typeof(UInt16);
                    case TypeCode.UInt32:
                        return typeof(UInt32);
                    case TypeCode.UInt64:
                        return typeof(UInt64);
                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
