using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Tools.Projection
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //ImportBlock s = new ImportBlock();
                //s.GetFeatures();

                string xmlFile = args[0];
                InputArg arg = InputArg.ParseXml(xmlFile);
                Console.WriteLine(arg.InputFilename);
                new Execute().Do(arg);

                //GenericFilename g = new GenericFilename();
                //string prjFilename = g.GenericPrjFilename("D:\\FY3A_MERSI_GBAL_L1_20120808_0000_1000M_MS.HDF", "GLL");
                //Console.WriteLine(prjFilename);
                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
