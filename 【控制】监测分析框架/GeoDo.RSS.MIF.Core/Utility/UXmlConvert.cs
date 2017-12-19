using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 目的：完成 XML 对象和 XML 字符串的互转换
    /// 创建人：王立峰
    /// 创建日期：2011-05-26
    /// </summary>
    public class UXmlConvert
    {
        public static Encoding DefaultEncoding = Encoding.UTF8;

        /// <summary>
        /// 将 XML 对象转换为XML字符串
        /// </summary>
        /// <param name="obj">XML对象</param>
        /// <returns>XML字符串</returns>
        public static string GetString(object obj)
        {
            return GetString(obj, DefaultEncoding);
        }

        /// <summary>
        /// 将 XML 对象转换为XML字符串
        /// </summary>
        /// <param name="obj">XML对象</param>
        /// <param name="encoding">字符编码</param>
        /// <returns>XML字符串</returns>
        public static string GetString(object obj, Encoding encoding)
        {
            if (obj == null)
                throw new Exception("Geoway.MIS.ProxyCommon.GetString(object) 异常，输入参数为 NULL ");

            try
            {
                var ms = new MemoryStream();

                // Serializer the User object to the stream.
                var ser = new XmlSerializer(obj.GetType());
                ser.Serialize(ms, obj);
                var json = ms.ToArray();
                ms.Close();
                return encoding.GetString(json, 0, json.Length);
            }
            catch (Exception ex)
            {
                throw new Exception("Geoway.MIS.ProxyCommon.GetString(object) 执行过程异常，具体信息：" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 将 XML 字符串转化为 XML 对象
        /// </summary>
        /// <param name="json">XML 字符</param>
        /// <param name="type">XML 对象的类型</param>
        /// <returns>XML 对象</returns>
        public static object GetObject(string xml, Type type)
        {
            return GetObject(xml, type, DefaultEncoding);
        }


        public static T GetObject<T>(string xml)
        {
            return (T) GetObject(xml, typeof (T));
        }

        /// <summary>
        /// 将 XML 字符串转化为 XML 对象
        /// </summary>
        /// <param name="xml">XML 字符</param>
        /// <param name="type">XML 对象的类型</param>
        /// <param name="encoding">字符编码</param>
        /// <returns>XML 对象</returns>
        public static object GetObject(string xml, Type type, Encoding encoding)
        {
            try
            {
                var ms = new MemoryStream(encoding.GetBytes(xml));
                var ser = new XmlSerializer(type);
                var obj = ser.Deserialize(ms);
                ms.Close();

                return obj;
            }
            catch (Exception ex)
            {
                throw new Exception("Geoway.MIS.ProxyCommon.GetObject(string, Type) 执行过程异常，具体信息：" + ex.Message, ex);
            }
        }
    }
}