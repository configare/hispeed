using System;
using System.Collections.Generic;
using System.Text;

namespace GeoDo.Tools.Mosaic
{
    public class Program
    {
        static void Main(string[] args)
        {
            string xmlFile = args[0];
            MosaicInputArg arg = MosaicInputArg.FromXml(xmlFile);
            Console.WriteLine("启动镶嵌拼接");
            Console.WriteLine(arg.InputFilename);
            try
            {
                MosaicSplice mosaic = new MosaicSplice(arg, new Action<int, string>(OnProgress));
                mosaic.DoMosaicSplice();
            }
            catch(Exception ex)
            {
                LogFactory.WriteLine(ex);
                Console.WriteLine(ex.Message);
            }
        }

        static void OnProgress(int progress, string text)
        {
            Console.WriteLine(progress + "," + text);
        }
    }
}
