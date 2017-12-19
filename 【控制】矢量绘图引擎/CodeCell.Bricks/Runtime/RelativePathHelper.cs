using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeCell.Bricks.Runtime
{
    public static class RelativePathHelper
    {
        //fname:  "F:\\产品与项目\\MAS-II\\SMART0718\\SMART\\【控制】UI框架(新整理)\\SMART\\bin\\Release\\数据引用\\基础矢量\\行政区划\\面\\中国行政区.shp";
        //ref  :  "F:\\产品与项目\\MAS-II\\SMART0718\\SMART\\【控制】UI框架(新整理)\\SMART\\bin\\Release\\LayoutTemplate\\自定义\\TempMcd.xml";
        public static string GetRelativePath(string refPath, string fullFilePath)
        {
            if (string.IsNullOrEmpty(fullFilePath))
                return null;
            if (refPath == null)
                refPath = Directory.GetCurrentDirectory();
            else
            {
                if (!refPath.EndsWith(":\\"))
                    refPath = Path.GetDirectoryName(refPath);
            }
            refPath = refPath.ToUpper();
            fullFilePath = fullFilePath.ToUpper();
            string fdir = Path.GetDirectoryName(fullFilePath);
            fullFilePath = Path.GetFileName(fullFilePath);
            //计算参考路径和文件路径的重叠部分
            string overlapPart = null;
            string[] parts_fdir = fdir.Split('\\');
            string[] parts_refdir = refPath.Split('\\');
            int count = Math.Min(parts_fdir.Length, parts_refdir.Length);
            int overlapPartCount = 0;
            for (int i = 0; i < count; i++)
            {
                if (parts_fdir[i] != parts_refdir[i])
                    break;
                overlapPart += (parts_refdir[i] + "\\");
                overlapPartCount++;
            }
            if (overlapPartCount == 0)
                return Path.Combine(fdir, fullFilePath);
            //计算参考路径最末文件夹到重叠部分的相对路径
            string relativeDir = null;
            if (fdir.Contains(refPath))
                relativeDir += ".\\";
            else
                for (int i = overlapPartCount; i < parts_refdir.Length; i++)
                    relativeDir += "..\\";
            for (int i = overlapPartCount; i < parts_fdir.Length; i++)
            {
                relativeDir += (parts_fdir[i] + "\\");
            }
            return relativeDir + fullFilePath;
        }

        public static string GetFullPath(string refPath, string relativePath)
        {
            if (!refPath.EndsWith(":\\"))
                refPath = Path.GetDirectoryName(refPath);
            string p = Path.Combine(refPath, relativePath);
            return Path.GetFullPath(p);
        }
    }
}
