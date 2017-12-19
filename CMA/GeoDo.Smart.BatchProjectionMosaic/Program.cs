using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.BatchProjectionMosaic
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Execute.SimplePrj(args);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Console.Read();
            }
        }
    }
}
