using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    /// <summary>
    /// 火区数据头
    /// </summary>
    public class FireSubareaColumnHeader
    {
        private string _name = null;
        private Type _type = null;
        private string _comment = null;
        private string _caption = null;

        public FireSubareaColumnHeader() { }

        public FireSubareaColumnHeader(string name, Type type)
        {
            _name = name;
            _type = type;
        }

        public FireSubareaColumnHeader(string name, Type type, string caption)
        {
            _name = name;
            _type = type;
            _caption = caption;
        }

        public FireSubareaColumnHeader(string name, Type type, string caption, string comment)
        {
            _name = name;
            _type = type;
            _caption = caption;
            _comment = comment;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public Type Type
        {
            get
            {
                return _type;
            }
        }

        public string Caption
        {
            get
            {
                return _caption;
            }
        }

        public string Comment
        {
            get
            {
                return _comment;
            }
        }
    }

    public class FireSubareaColumnHeaderDef
    {
        #region 火区数据列名
        public static FireSubareaColumnHeader[] FireSubarea = 
        {
            new FireSubareaColumnHeader("C0", typeof(int), "火区号", ""),
            new FireSubareaColumnHeader("C1", typeof(float), "经度    ", ""),
            new FireSubareaColumnHeader("C2", typeof(float), "纬度    ", ""),
            new FireSubareaColumnHeader("C3", typeof(int), "火点像元个数".PadRight(10), ""),
            new FireSubareaColumnHeader("C4", typeof(float), "像元覆盖面积（平方公里）".PadRight(10), ""),
            new FireSubareaColumnHeader("C5", typeof(float), "明火面积(公顷)".PadRight(10), ""),
            new FireSubareaColumnHeader("C6", typeof(string), "省地县".PadRight(30), ""),
            new FireSubareaColumnHeader("C7", typeof(string), "林地".PadRight(10), ""),
            new FireSubareaColumnHeader("C8", typeof(string), "草地".PadRight(10), ""),
            new FireSubareaColumnHeader("C9", typeof(string), "农田".PadRight(10), ""),
            new FireSubareaColumnHeader("C10", typeof(string), "其他".PadRight(10), "")            
        };
        #endregion
    }
}
