using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public class ObritFileFinder : FileFinder
    {
        private Dictionary<string, string> argDic = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentRasterFile"></param>
        /// <param name="extinfo"></param>
        /// <param name="argument">eg. Dir=d:\test,Flag=*_ndvi_*.ldf,Sort=des[asc],ProFlag=true,FindRegion= all[top]
        ///                        Dir:轨道数据路径；Flag：文件查找标识；Sort：文件排序方式；ProFlag：是否有已生产标记
        ///                        FindRegion:文件查找范围，all 包含子文件夹  top 当前文件夹
        ///                        </param>
        /// <returns></returns>
        public override string[] Find(string currentRasterFile, ref string extinfo, string argument)
        {
            if (!ParseArugment(argument, out argDic))
                return null;
            string[] resultfiles = null;
            if (!FindAllFiles(out resultfiles))
                return null;
            return resultfiles;
        }

        private bool FindAllFiles(out string[] srcfiles)
        {
            srcfiles = null;
            string dir;
            if (!GetArg(argDic, "DIR", out dir))
                return false;
            if (!Directory.Exists(dir))
                return false;
            string fileFlag;
            if (!GetArg(argDic, "FLAG", out fileFlag))
                fileFlag = "*.*";
            string findregion;
            if (!GetArg(argDic, "FINDREGION", out findregion))
                findregion = "all";
            srcfiles = Directory.GetFiles(dir, fileFlag, findregion.ToUpper() == "ALL" ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            if (srcfiles == null || srcfiles.Length == 0)
                return false;
            return true;
        }

    }
}
