using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    public static class StreamReadHelper
    {
        /// <summary>
        /// 从源目标流中读取字符并往目标序列中写入数据
        /// </summary>
        /// <param name="sourceStream">目标源流</param>
        /// <param name="target">包含从目标源流读取的字符序列</param>
        /// <param name="start">目标序列开始的索引号</param>
        /// <param name="count">从目标源流读取的最大字符数</param>
        /// <returns>读取的字符数，到达末尾返回-1</returns>
        public static Int32 ReadInput(Stream sourceStream, sbyte[] target, int start, int count)
        {
            // Returns 0 bytes if not enough space in target
            if (target.Length == 0)
                return 0;

            byte[] receiver = new byte[target.Length];
            int bytesRead = sourceStream.Read(receiver, start, count);

            // Returns -1 if EOF
            if (bytesRead == 0)
                return -1;

            for (int i = start; i < start + bytesRead; i++)
                target[i] = (sbyte)receiver[i];

            return bytesRead;
        }
    }
}
