using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Tools.FreqStat
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args == null || args.Length == 0)
                    throw new ArgumentNullException("args", "参数不能为空");
                //string argFile = @"G:\工程项目\Smart二期\气象MAS二期\SMART\bin\Release\FreqStat.xml";
                string argFile = args[0];
                FreqExecutor executor = new FreqExecutor(argFile, new Action<int, string>(OnProgress));
                executor.Execute();
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine(ex);
                Console.WriteLine(ex.Message);
            }
        }

        static void OnProgress(int progress, string text)
        {
            Console.WriteLine(progress + "%," + text);
        }
    }
}
