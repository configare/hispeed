using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.Smart.ValueComposite
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length != 3)
            {
                Console.WriteLine("请输入参数：文件目录 目标文件名 最值标示（MIN|MAX）");
                return;
            }
            string path = args[0];    
            ValueCompositer exe = new ValueCompositer();
            exe.Composite(args[0], args[1], args[2], (p, pstring) => { Console.WriteLine(pstring); });
            Console.ReadLine();
        }
    }
}
