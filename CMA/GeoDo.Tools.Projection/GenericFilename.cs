using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace GeoDo.Tools.Projection
{
    public class GenericFilename
    {
        Regex _prjRegex;

        public GenericFilename()
        {
            string pattern = @"(?<level>L1)_(?<date>\d{8})_(?<time>\d{0,4})";
            _prjRegex = new Regex(pattern, RegexOptions.Compiled);
        }

        public string GenericPrjFilename(string orbitFilename, string projectionIdentify)
        {
            string retFilename;
            if (_prjRegex.IsMatch(orbitFilename))
            {
                Match match = _prjRegex.Match(orbitFilename);
                string str = match.Value;
                retFilename = orbitFilename.Insert(match.Index - 1, "_" + projectionIdentify);
                retFilename = _prjRegex.Replace(orbitFilename, projectionIdentify +"_"+ str);
            }
            else
                retFilename = Path.GetDirectoryName(orbitFilename) + Path.GetFileNameWithoutExtension(orbitFilename) + "_" + projectionIdentify + Path.GetExtension(orbitFilename);
            return retFilename;
        }

        public string GenericBlockFilename(string orbitFilename,string blockIdentify)
        {
            string outFile;
            string[] parts = Path.GetFileName(orbitFilename).Split('_');
            if (parts.Length < 8)//文件名不规范
            {
                outFile = Path.GetFileNameWithoutExtension(orbitFilename) + "_" + blockIdentify + ".LDF";
            }
            else
            {
                parts[2] = blockIdentify.ToUpper();//第3段代表区域，默认GBAL
                string str = null;
                foreach (string s in parts)
                    str += (s + "_");
                str = str.Substring(0, str.Length - 1);
                outFile = Path.GetFileNameWithoutExtension(str) + ".LDF";
            }
            return outFile;
        }

        /// <summary>
        /// 自动生成非重复的文件名：
        /// 如果已经存在了，自动在其后添加(1)、(2)...等依次累加。
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GenericOnlyFilename(string filename)
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
