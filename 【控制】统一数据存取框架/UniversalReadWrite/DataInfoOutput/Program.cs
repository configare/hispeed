using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataInfoOutput
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length != 3)
                return;
            //输入，输出，类型（FY1/FY3）
            string inputDir = args[0];
            string outputDir = args[1];
            string type = args[2].ToUpper();
            OutputProcessor processor = new OutputProcessor();
            processor.Process(inputDir, outputDir, type, (p, pstring) => { Console.WriteLine(pstring); });

        }


    }
}
