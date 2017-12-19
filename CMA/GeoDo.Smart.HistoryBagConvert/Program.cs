using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace GeoDo.Smart.HistoryBagConvert
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args==null||args.Length < 1)
            {
                Console.WriteLine("请输入有效的待转换文件（文件夹）名！");
                return;
            }
            string inputFile = args[0];
            string outputFile = null;
            if (args.Length == 2)
                outputFile = args[1];
            BAGFormatConverter converter = new BAGFormatConverter();
            converter.Convert(inputFile, outputFile, (p, pstring) => { Console.WriteLine(pstring); });
        }

        private static unsafe void ConvertFile()
        {
            string file = @"G:\工程项目\气象局现场\SmartRelease\trunk\SystemData\FY2NOM\FY2E_latlon22882288.raw";
            IRasterDataProvider dataPrd = null;
            try
            {

                dataPrd = GeoDataDriver.Open(file) as IRasterDataProvider;
                string outfile = @"D:\1.raw";
                IRasterBand band = dataPrd.GetRasterBand(1);
                IRasterBand band2 = dataPrd.GetRasterBand(2);
                float[] buffer = new float[band.Width * band.Height];
                fixed (float* ptr = buffer)
                {
                    IntPtr bufferPtr = new IntPtr(ptr);
                    band.Read(0, 0, dataPrd.Width, dataPrd.Height, bufferPtr, enumDataType.Float, dataPrd.Width, dataPrd.Height);
                }
                float[] buffer2 = new float[band.Width * band.Height];
                fixed (float* ptr = buffer2)
                {
                    IntPtr bufferPtr = new IntPtr(ptr);
                    band2.Read(0, 0, dataPrd.Width, dataPrd.Height, bufferPtr, enumDataType.Float, dataPrd.Width, dataPrd.Height);
                }
                using (FileStream fs = new FileStream(outfile, FileMode.Create, FileAccess.Write))
                {
                    using (BinaryWriter br = new BinaryWriter(fs))
                    {
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            if(buffer[i]==300f)
                                br.Write(buffer[i]);
                            else
                                br.Write(buffer[i] + 7.5f);
                        }
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            br.Write(buffer2[i]);
                        }
                    }
                }
            }
            finally
            {
                if (dataPrd != null)
                    dataPrd.Dispose();
            }
        }
    }
}
