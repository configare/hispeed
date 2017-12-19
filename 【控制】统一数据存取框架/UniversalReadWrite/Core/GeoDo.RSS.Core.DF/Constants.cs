using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public class Constants
    {
        public const int IS_TRUE = 1;
        public const int IS_FALSE = 0;
        /// <summary>
        /// 在做逐像素的统计操作时,如果设置isCanApproy则需要抽样,该参数是缺省的抽样值
        /// </summary>
        public const int DEFAULT_PIXELES_INTERLEAVE = 100;
        /// <summary>
        /// 在读大文件时,需要分块读,该参数表示最大处理的像素数
        /// 函数内部将其转换为行数,每次处理指定的行数
        /// </summary>
        public const int MAX_PIXELS_BLOCK = 1000000; 
    }
}