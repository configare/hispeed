using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;

namespace CodeCell.AgileMap.Core
{
    /// <summary>
    /// 内存缓存数量控制对象
    /// </summary>
    public class MemoryGridLimiter:IPersistable
    {
        protected enumGridLimitType _limitType = enumGridLimitType.None;
        protected int _count = 0;//0:不限制
        protected int _maxMemorySize = 0;//0:不限制 //字节数

        [DisplayName("限制方式")]
        public enumGridLimitType LimitType
        {
            get { return _limitType; }
            set { _limitType = value; }
        }

        [DisplayName("数量限制最大数")]
        public int Count
        {
            get { return _count; }
            set 
            {
                if (value < 0)
                    return;
                _count = value; 
            }
        }

        [DisplayName("内存限制最大数")]
        public int MaxMemorySize
        {
            get { return _maxMemorySize; }
            set
            {
                if (value < 0)
                    return;
                _maxMemorySize = value; 
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }

        #region IPersistable Members

        public PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("MemoryLimiter");
            obj.AddAttribute("type", _limitType.ToString());
            obj.AddAttribute("maxcount", _count.ToString());
            obj.AddAttribute("maxmemory", _maxMemorySize.ToString());
            return obj;
        }

        #endregion

        public static MemoryGridLimiter FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            string type = ele.Attribute("type").Value;
            enumGridLimitType t = enumGridLimitType.None;
            foreach (enumGridLimitType it in Enum.GetValues(typeof(enumGridLimitType)))
            { 
                if(it.ToString() == type)
                {
                    t = it;
                    break ;
                }
            }
            MemoryGridLimiter mgt =  new MemoryGridLimiter();
            mgt.Count = int.Parse(ele.Attribute("maxcount").Value);
            mgt.MaxMemorySize = int.Parse(ele.Attribute("maxmemory").Value);
            return mgt;
        }
    }
}
