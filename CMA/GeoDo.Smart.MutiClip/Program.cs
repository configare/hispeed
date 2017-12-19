using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.MutiClip
{
    class Program
    {
        static void Main(string[] args)
        {
            string xmlfile = args[0];
            ClipData clip = new ClipData();
            Console.WriteLine("开始裁切。。。");
            clip.ClipFile(xmlfile);
            Console.WriteLine("裁切结束。。。");
            Console.ReadLine();
        }
    }
}
