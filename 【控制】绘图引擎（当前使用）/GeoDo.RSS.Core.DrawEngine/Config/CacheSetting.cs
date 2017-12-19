using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class CacheSetting
    {
        private string _dir = AppDomain.CurrentDomain.BaseDirectory + "cache";
        private int _maxSizeMB = 0;//无限大
        private int _minSizeMB = 100;//最小磁盘空闲空间,若小于该空间则不实用缓存，缺省100MB

        public CacheSetting()
        { 
        }

        public CacheSetting(string dir, int maxSizeMB,int minSizeMB)
        {
            _dir = dir;
            _maxSizeMB = maxSizeMB;
            _minSizeMB = minSizeMB;
        }

        public string Dir
        {
            get { return _dir; }
            set { _dir = value; }
        }

        public int MaxSizeMB
        {
            get { return _maxSizeMB; }
            set { _maxSizeMB = value; }
        }

        public int MinSizeMB
        {
            get { return _minSizeMB; }
            set { _minSizeMB = value; }
        }
    }
}
