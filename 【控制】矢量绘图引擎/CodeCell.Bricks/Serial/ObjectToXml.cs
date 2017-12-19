using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CodeCell.Bricks.Serial
{
    public static class ObjectToXml
    {
        public static void SaveObjectToXmlFile(string filename,object obj)
        {
            Type t = obj.GetType();
            XmlSerializer ser = new XmlSerializer(t);
            TextWriter writer = new StreamWriter(filename);
            ser.Serialize(writer, obj);
            writer.Close();
        }

        public static object GetObjectFromXmlFile(string filename, Type type)
        {
            XmlSerializer ser = new XmlSerializer(type);
            FileStream fs = new FileStream(filename, FileMode.Open);
            object obj = ser.Deserialize(fs);
            fs.Close();
            return obj;
        }
    }
}
