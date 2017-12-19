using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.Smart.MaxValueComposites
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("请输入参数：文件目录");
                return;
            }
            string path = args[0];    //@"E:\NDVI"
            string[] fs = Directory.GetFiles(path, "*.ldf");
            Class1 s = new Class1();
            s.StatFiles(fs, 1, (p, pstring) => { Console.WriteLine(pstring); });
            Console.ReadLine();
        }
    }
}
