using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.BlockOper
{
    public class PathHelper
    {
        /// <summary>
        /// 自动生成非重复的文件名：
        /// 如果已经存在了，自动在其后添加(1)、(2)...等依次累加。
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetFilename(string filename)
        {
            if (File.Exists(filename))
            {
                string dir = Path.GetDirectoryName(filename);
                string filenameWithExt = Path.GetFileNameWithoutExtension(filename);
                string fileExt = Path.GetExtension(filename);
                int i = 1;
                string outFileNmae = Path.Combine(dir, filenameWithExt + "(" + i + ")" + fileExt);
                while (File.Exists(outFileNmae))
                    outFileNmae = Path.Combine(dir, filenameWithExt + "(" + i++ + ")" + fileExt);
                return outFileNmae;
            }
            else
                return filename;
        }
    }
}
