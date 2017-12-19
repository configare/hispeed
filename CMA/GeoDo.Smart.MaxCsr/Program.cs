using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace GeoDo.Smart.MaxCsr
{
    //static class Program
    //{
    //    /// <summary>
    //    /// 应用程序的主入口点。
    //    /// </summary>
    //    [STAThread]
    //    static void Main()
    //    {
    //        Application.EnableVisualStyles();
    //        Application.SetCompatibleTextRenderingDefault(false);
    //        Application.Run(new Form1());
    //    }
    //}
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("请输入参数：文件目录及计算波段号！");
                return;
            }
            string[] argAry = args[0].Trim().Split(';');
            if (argAry == null || argAry.Length < 2)
            {
                Console.WriteLine("请输入有效的文件目录及计算波段号！");
                return;
            }
            string path = argAry[0];    //@"E:\NDVI"
            string[] fs = Directory.GetFiles(path, "*.ldf");
            string[] bandNos = argAry[1].Trim().Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
            if (bandNos == null || bandNos.Length != 2)
                return;
            Executer s = new Executer();
            s.StatFiles(fs, new int[] { int.Parse(bandNos[0]), int.Parse(bandNos[1]) }, (p, pstring) => { Console.WriteLine(pstring); });
            Console.ReadLine();
        }
    }
}
