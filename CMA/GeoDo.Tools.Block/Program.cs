using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Tools.Block
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args == null || args.Length == 0)
                    throw new ArgumentNullException("args", "参数不能为空");
                Execute e = new Execute();
                e.Do(args[0]);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
