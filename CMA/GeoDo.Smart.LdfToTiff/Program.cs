using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace GeoDo.Smart.LdfToTiff
{
    class Program
    {
        /// <summary>
        /// "输入文件夹或输入文件名" "输出文件夹或输出文件夹" "*_ch5.raw" "1,2,3" "图像增强方案文件名"
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args == null || args.Length < 4)
            {
                Console.WriteLine("请输入有效的待转换文件（文件夹）名及目标文件（文件夹）名、待转换波段号！");
                return;
            }
            string inputFName = args[0];
            string outputFName = args[1];
            string searchPattern = args[2];
            string bandNos = args[3];
            string argFile=null;
            if (args.Length == 5)
                argFile = args[4];
            LdfToTiffWriter writer = new LdfToTiffWriter();
            writer.Write(inputFName, outputFName, searchPattern,bandNos,argFile,(p, pstring) => { Console.WriteLine(pstring); });
            return;
        }
    }
}
