using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.Tools.Mosaic;

namespace GeoDo.Smart.BatchMosaic
{
    public class Program
    {
        //BatchMosaicArgs.xml
        static void Main(string[] args)
        {
            Execute.SimplePrj(args);
        }

        static void OnProgress(int progress, string text)
        {
            Console.WriteLine(progress + "," + text);
        }
    }
}
