using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Layout
{
    public interface IGxdRasterItem:IGxdItem
    {
        string FileName { get; }
        object Arguments { get; }
        /// <summary>
        /// 栅格文件打开的参数，默认为null
        /// </summary>
        string[] FileOpenArgs{get;}
        /// <summary>
        /// 栅格绘制的色表，默认为null
        /// </summary>
        string ColorTableName { get; }
    }
}
