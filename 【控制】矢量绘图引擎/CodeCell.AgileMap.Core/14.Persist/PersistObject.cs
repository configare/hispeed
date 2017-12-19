using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

namespace CodeCell.AgileMap.Core
{
    public class PersistObject
    {
        private string _name = string.Empty;
        private Dictionary<string, string> _attributes = null;
        private List<PersistObject> _subNodes = null;

        public PersistObject()
        { 
        }

        public PersistObject(string name)
            : this()
        {
            _name = name;
        }

        public PersistObject(string name, Dictionary<string, string> attributes)
        {
            _name = name;
            _attributes = attributes;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public IEnumerable<KeyValuePair<string, string>> Attributes
        {
            get { return _attributes; }
        }

        public IEnumerable<PersistObject> SubNodes
        {
            get { return _subNodes; }
        }

        public void AddSubNode(PersistObject child)
        {
            if (child == null)
                return;
            if (_subNodes == null)
                _subNodes = new List<PersistObject>();
            if (!_subNodes.Contains(child))
                _subNodes.Add(child);
        }

        public void AddAttribute(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("属性名称为空。");
            if (value == null)
                throw new ArgumentNullException("属性值为空。");
            if (_attributes == null)
                _attributes = new Dictionary<string, string>();
            if (_attributes.ContainsKey(name))
                throw new ArgumentException("名称为\""+name+"\"的属性已经存在。");  
            _attributes.Add(name, value);
        }

        private static Dictionary<string, Assembly> _loadedAssemblies = new Dictionary<string, Assembly>();
        internal static void BeginParse()
        {
            _loadedAssemblies = new Dictionary<string, Assembly>();
        }

        internal static void EndParse()
        {
            _loadedAssemblies.Clear();
        }

        internal static object ReflectObjFromXElement(XElement ele)
        {
            string typeString = ele.Attribute("type").Value;
            string[] parts = typeString.Split(',');
            if (parts.Length == 1)
                parts = new string[] { @"CodeCell.AgileMap.Core.dll", parts[0] };
            //string assfilename = AppDomain.CurrentDomain.BaseDirectory + parts[0];
            string assfilename = typeof(FeatureClass).Assembly.Location;
            //if (!File.Exists(assfilename))//web
            //    assfilename = AppDomain.CurrentDomain.BaseDirectory + "bin\\" + parts[0];
            Assembly ass = null ;
            if (_loadedAssemblies.ContainsKey(assfilename))
                ass = _loadedAssemblies[assfilename];
            else
            {
               
                ass = Assembly.LoadFile(assfilename);
                _loadedAssemblies.Add(assfilename, ass);
            }
            Type type = ass.GetType(parts[1]);
            return type.InvokeMember("FromXElement", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new object[] { ele });
        }

        /// <summary>
        /// 对象到二进制流
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Stream ObjectToStream(object obj)
        {
            Stream st = new MemoryStream();
            IFormatter formatter = (IFormatter)new BinaryFormatter();
            formatter.Serialize(st, obj);
            st.Position = 0;
            return st;
        }

        public static object StreamToObject(Stream st)
        {
            IFormatter formatter = (IFormatter)new BinaryFormatter();
            return formatter.Deserialize(st);
        }

        public static Stream ObjectToStreamWithZIP(object obj)
        {
            using (Stream st = new MemoryStream())
            {
                IFormatter formatter = (IFormatter)new BinaryFormatter();
                formatter.Serialize(st, obj);
                st.Position = 0;
                return ZipStream(st);
            }
        }

        public static object StreamToObjectWithZIP(Stream st)
        {
            using (Stream uzip = UZipStream(st))
            {
                IFormatter formatter = (IFormatter)new BinaryFormatter();
                return formatter.Deserialize(uzip);
            }
        }

        public static Stream ZipStream(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                zip.Write(buffer, 0, buffer.Length);
            }
            ms.Position = 0;
            return ms;
        }

        public static Stream UZipStream(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            {
                stream.Position = 0;
                using (GZipStream zip = new GZipStream(stream, CompressionMode.Decompress))
                {
                    int b = zip.ReadByte();
                    while (b != -1)
                    {
                        ms.WriteByte((byte)b);
                        b = zip.ReadByte();
                    }
                }
            }
            ms.Position = 0;
            return ms;
        }
    }
}
