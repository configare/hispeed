using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OSGeo.GDAL;
using System.IO;
using GeoDo.RSS.DF.LDF;
using System.Diagnostics;
using System.Runtime.InteropServices;
using GeoDo.HDF;
using GeoDo.HDF5;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.GDAL;
using GeoDo.RSS.DF.NOAA;
using GeoDo.RSS.DF.NOAA.BandPrd;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using GeoDo.Oribit.Algorithm;
using GeoDo.RSS.DF.MVG;
using GeoDo.Project;
using GeoDo.RSS.DF.GDAL.HDF5Universal;
using GeoDo.RSS.DF.SeaSurfaceWind;
using GeoDo.RSS.DF.FY1D;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private UInt16[] _buffer = null;
        public Form1()
        {
            InitializeComponent();
        }

        private unsafe void button1_Click(object sender, EventArgs e)
        {
            string dirName = textBox2.Text;
            if (string.IsNullOrEmpty(dirName) || !Directory.Exists(dirName))
            {
                MessageBox.Show("输入目录为空或不存在！");
                return;
            }    
            string[] fileNames = Directory.GetFiles(dirName, "*.1A5");
            //string fileName = @"G:\临时文件夹\FY1D\FY1D_AVHRR_GDPT_L1_ORB_MLT_NUL_20070101_1058_4000M_PJ.1A5";
            if (fileNames.Length > 0)
            {
                IRasterDataProvider dataPrd = null;
                string outDirName = textBox1.Text;
                if(string.IsNullOrEmpty(outDirName))
                   outDirName = Path.Combine(dirName, "辅助信息");
                if (!Directory.Exists(outDirName))
                    Directory.CreateDirectory(outDirName);
                foreach (string file in fileNames)
                {
                    try
                    {
                        dataPrd = GeoDataDriver.Open(file) as IRasterDataProvider;
                        IRasterBand band = dataPrd.GetRasterBand(4);
                        UInt16[] buffer = new UInt16[band.Width * band.Height];
                        fixed (UInt16* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(0, 0, dataPrd.Width, dataPrd.Height, bufferPtr, enumDataType.UInt16, dataPrd.Width, dataPrd.Height);
                        }
                        //输出单波段
                        string outFileName = Path.Combine(outDirName,Path.GetFileNameWithoutExtension(file) + "_ch5.raw");
                        using (FileStream fs = new FileStream(outFileName, FileMode.Create, FileAccess.Write))
                        {
                            using (BinaryWriter br = new BinaryWriter(fs))
                            {
                                for (int i = 0; i < buffer.Length; i++)
                                {
                                    br.Write(buffer[i]);
                                }
                            }
                        }
                        string hdrFile = Path.ChangeExtension(outFileName, ".hdr");
                        SaveHdrFile(hdrFile, band.Width, band.Height, 12);
                        //轨道根数
                        string headerFile = Path.Combine(outDirName,Path.GetFileNameWithoutExtension(file)+".HeaderInfo.txt");
                        string headerstring = (dataPrd as GeoDo.RSS.DF.FY1D.FY1_1A5DataProvider).GetHeaderInfo();
                        File.WriteAllText(headerFile, headerstring);
                        //经纬度
                        string lonlatFile = Path.Combine(outDirName,Path.GetFileNameWithoutExtension(file)+".LonLat.txt");
                        double[] firstLon; double[] firstLat; double[] lastLon; double[] lastLat;
                        (dataPrd as GeoDo.RSS.DF.FY1D.FY1_1A5DataProvider).GetPositionInfo(out firstLon, out firstLat, out lastLon, out lastLat);
                        StringBuilder lonstr1 = new StringBuilder();
                        lonstr1.AppendLine("首行纬度、首行经度、末行纬度、末行经度如下：");
                        lonstr1.AppendLine(string.Join(",", firstLon));
                        lonstr1.AppendLine(string.Join(",", firstLat));
                        lonstr1.AppendLine(string.Join(",", lastLon));
                        lonstr1.AppendLine(string.Join(",", lastLat));
                        lonstr1.AppendLine("采样点数：1018 ，定位点51个，从第8个点开始，每个20个点定位一个点。");
                        File.WriteAllText(lonlatFile, lonstr1.ToString());
                    }
                    finally
                    {
                        if (dataPrd != null)
                            dataPrd.Dispose();
                    }
                }
                MessageBox.Show("FY1D数据处理完成！");
            }
        }

        private unsafe void button2_Click(object sender, EventArgs e)
        {
            //string filename = @"F:\TERRA_2010_03_25_03_09_GZ.MOD021KM.hdf";
            //Dataset ds = Gdal.Open(filename, Access.GA_ReadOnly);
            string dirName = textBox2.Text;
            if (string.IsNullOrEmpty(dirName) || !Directory.Exists(dirName))
            {
                MessageBox.Show("输入目录为空或不存在！");
                return;
            }   
            string[] fileNames = Directory.GetFiles(dirName, "*.hdf");
            if (fileNames.Length > 0)
            {
                IRasterDataProvider dataPrd = null;
                string outDirName = textBox1.Text;
                if (string.IsNullOrEmpty(outDirName))
                    outDirName = Path.Combine(dirName, "辅助信息");
                if (!Directory.Exists(outDirName))
                    Directory.CreateDirectory(outDirName);
                foreach (string file in fileNames)
                {
                    try
                    {
                        dataPrd = GeoDataDriver.Open(file) as IRasterDataProvider;
                        //纬度
                        ReadAndWriteOneBandRaw(dataPrd, "Latitude", outDirName);
                        //经度
                        ReadAndWriteOneBandRaw(dataPrd, "Longitude", outDirName);
                        //通道
                        IRasterBand band = dataPrd.GetRasterBand(5);
                        UInt16[] buffer = new UInt16[band.Width * band.Height];
                        fixed (UInt16* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(0, 0, dataPrd.Width, dataPrd.Height, bufferPtr, enumDataType.UInt16, dataPrd.Width, dataPrd.Height);
                        }
                        //输出单波段
                        string outFileName = Path.Combine(outDirName, Path.GetFileNameWithoutExtension(file) + "_ch5.raw");
                        using (FileStream fs = new FileStream(outFileName, FileMode.Create, FileAccess.Write))
                        {
                            using (BinaryWriter br = new BinaryWriter(fs))
                            {
                                for (int i = 0; i < buffer.Length; i++)
                                {
                                    br.Write(buffer[i]);
                                }
                            }
                        }
                        string hdrFile = Path.ChangeExtension(outFileName, ".hdr");
                        SaveHdrFile(hdrFile, dataPrd.Width, dataPrd.Height, 12);
                    }
                    finally
                    {
                        if (dataPrd != null)
                            dataPrd.Dispose();
                    }
                }
                MessageBox.Show("FY3数据处理完成！");
            }
        }

        private void SaveHdrFile(string hdrFile, int width, int height, int dataType)
        {
            using (StreamWriter sw = new StreamWriter(hdrFile, false, Encoding.Default))
            {
                sw.WriteLine("ENVI");
                sw.WriteLine("description = {File Imported into ENVI}");
                sw.WriteLine(string.Format("samples = {0}", width));
                sw.WriteLine(string.Format("lines = {0}", height));
                sw.WriteLine("bands = 1");
                sw.WriteLine("file type = ENVI Standard");
                sw.WriteLine(string.Format("data type = {0}", dataType));
                sw.WriteLine("interleave = bip");
                sw.WriteLine("byte order = 0");
            }
        }

        private unsafe void ReadAndWriteOneBandRaw(IRasterDataProvider dataPrd,string fieldName,string outDirName)
        {
            IRasterBand[] bands = dataPrd.BandProvider.GetBands(fieldName);
            if (bands != null)
            {
                float[] buffer = new float[dataPrd.Width * dataPrd.Height];
                fixed (float* ptr = buffer)
                {
                    IntPtr bufferPtr = new IntPtr(ptr);
                    bands[0].Read(0, 0, dataPrd.Width, dataPrd.Height, bufferPtr, enumDataType.Float, dataPrd.Width, dataPrd.Height);
                }
                //输出单波段
                string outFileName = Path.Combine(outDirName, Path.GetFileNameWithoutExtension(dataPrd.fileName) + "_" + fieldName + ".raw");
                using (FileStream fs = new FileStream(outFileName, FileMode.Create, FileAccess.Write))
                {
                    using (BinaryWriter br = new BinaryWriter(fs))
                    {
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            br.Write(buffer[i]);
                        }
                    }
                }
                string hdrFile = Path.ChangeExtension(outFileName, ".hdr");
                SaveHdrFile(hdrFile, dataPrd.Width, dataPrd.Height, 4);
            }
            
        }

        private unsafe void btnOpenLdf_Click(object sender, EventArgs e)
        {
            string filename = @"E:\测试\FY3A_VIRRX_GBAL_L1_HAM_20110914_0500_1000M_MS_LW_阿勒泰地区.LDF";
            filename = @"E:\HDFDATA\ldf问题数据\EI2040714.ldf";
            filename = @"E:\HDFDATA\ldf问题数据\EI2040814.ldf";
            filename = @"E:\气象局项目\FY3B_VIRRX_GBAL_L1_20101128_0500_1000M_MS_PRJ_Whole.LDF";
            IRasterDataDriver driver = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            IRasterDataProvider prd = GeoDataDriver.Open(filename) as IRasterDataProvider;
            ILdfDataProvider ldfPrd = prd as ILdfDataProvider;
            LdfHeaderBase ldfHeader = new Ldf1Header(filename);
            HdrFile hdr = ldfHeader.ToHdrFile(); //new Hdr(ldfHeader);
            byte[] buffer = new byte[prd.Width * prd.Height * 2];
            IRasterBand band = prd.GetRasterBand(1);
            fixed (byte* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                band.Read(0, 0, prd.Width, prd.Height, bufferPtr, enumDataType.UInt16, prd.Width, prd.Height);
            }
        }

        private unsafe void ReadBSQByBand_Click_1(object sender, EventArgs e)
        {
            string fname = @"F:\中文目录\FY3B_VIRRX_GBAL_L1_20101128_0500_1000M_MS_PRJ_Whole.LDF";
            fname = @"E:\测试\save.LDF";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            IRasterBand band = prd.GetRasterBand(1);
            _buffer = new UInt16[band.Width * band.Height];

            // UInt16[] buffer = new UInt16[(band.Width / 2) * (band.Height / 2)];
            Stopwatch _watch = new Stopwatch();
            fixed (UInt16* ptr = _buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                _watch.Start();
                // band.Read(0, 0, band.Width, band.Height, bufferPtr, enumDataType.UInt16, band.Width/2, band.Height/2);
                band.Read(0, 0, band.Width, band.Height, bufferPtr, enumDataType.UInt16, band.Width, band.Height);
                long k = _watch.ElapsedMilliseconds;
                _watch.Stop();
            }
            using (FileStream fileStream = new FileStream(@"e:\测试\BSQbiger_LDF3.bin", FileMode.Create))
            // using (FileStream fileStream = new FileStream(@"e:\测试\BSQbiger_gdal3.bin", FileMode.Create))
            {
                BinaryWriter bw = new BinaryWriter(fileStream);
                for (int i = 0; i < _buffer.Length; i++)
                    bw.Write(_buffer[i]);
            }
        }

        private unsafe void ReadBIPByBand_Click(object sender, EventArgs e)
        {
            string fname = @"F:\中文目录\FY3B_VIRRX_GBAL_L1_20101128_0500_1000M_MS_PRJ_Whole.LDF";
            //fname = @"E:\测试\BIP.LDF";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            IRasterBand band = prd.GetRasterBand(5);
            UInt16[] buffer = new UInt16[band.Width * band.Height];
            int[] bandMap = { 5 };

            // UInt16[] buffer = new UInt16[(band.Width / 2) * (band.Height / 2)];
            Stopwatch _watch = new Stopwatch();
            fixed (UInt16* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                _watch.Start();
                band.Read(0, 0, band.Width, band.Height, bufferPtr, enumDataType.UInt16, band.Width, band.Height);
                //band.Read(83, 121, band.Width / 2, band.Height / 2, bufferPtr, enumDataType.UInt16, band.Width, band.Height);
                long k = _watch.ElapsedMilliseconds;
                _watch.Stop();
            }
            using (FileStream fileStream = new FileStream(@"e:\测试\BIPadd0_gdal3.bin", FileMode.Create))
            // using (FileStream fileStream = new FileStream(@"e:\测试\BIPadd0_LDF3.bin", FileMode.Create))
            // using (FileStream fileStream = new FileStream(@"e:\测试\BIPless_gdal3.bin", FileMode.Create))
            //using (FileStream fileStream = new FileStream(@"e:\测试\BIPless_LDF3.bin", FileMode.Create))
            // using (FileStream fileStream = new FileStream(@"e:\测试\BIP_LDF3.bin", FileMode.Create))
            //using (FileStream fileStream = new FileStream(@"e:\测试\BIP_gdal3.bin", FileMode.Create))
            {
                BinaryWriter bw = new BinaryWriter(fileStream);
                for (int i = 0; i < buffer.Length; i++)
                    bw.Write(buffer[i]);
            }

        }

        private unsafe void ReadBILByBand_Click(object sender, EventArgs e)
        {
            string fname = @"F:\中文目录\FY3B_VIRRX_GBAL_L1_20101128_0500_1000M_MS_PRJ_Whole.LDF";
            fname = @"E:\测试\BIL.LDF";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            IRasterBand band = prd.GetRasterBand(5);
            UInt16[] buffer = new UInt16[band.Width * band.Height];
            // UInt16[] buffer = new UInt16[(band.Width / 2) * (band.Height / 2)];
            Stopwatch _watch = new Stopwatch();
            fixed (UInt16* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                _watch.Start();
                band.Read(0, 0, band.Width, band.Height, bufferPtr, enumDataType.UInt16, band.Width, band.Height);
                //band.Read(83, 121, band.Width / 2, band.Height / 2, bufferPtr, enumDataType.UInt16, band.Width, band.Height);
                long k = _watch.ElapsedMilliseconds;
                _watch.Stop();
            }
            using (FileStream fileStream = new FileStream(@"e:\测试\BIL_LDF3.bin", FileMode.Create))
            //  using (FileStream fileStream = new FileStream(@"e:\测试\BIL_gdal3.bin", FileMode.Create))
            //using (FileStream fileStream = new FileStream(@"e:\测试\BILbiger_gdal3.bin", FileMode.Create))
            //using (FileStream fileStream = new FileStream(@"e:\测试\BILbiger_LDF3.bin", FileMode.Create))
            {
                BinaryWriter bw = new BinaryWriter(fileStream);
                for (int i = 0; i < buffer.Length; i++)
                    bw.Write(buffer[i]);
            }

        }

        private unsafe void ReadToBSQ_Click(object sender, EventArgs e)
        {
            string fname = @"E:\测试\save.LDF";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            IRasterBand band = prd.GetRasterBand(5);
            //int[] bandMap = { 1};
            int[] bandMap = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            _buffer = new UInt16[band.Width * band.Height * 10];

            // UInt16[] buffer = new UInt16[(band.Width / 2) * (band.Height / 2)];
            Stopwatch _watch = new Stopwatch();
            fixed (UInt16* ptr = _buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                _watch.Start();
                //  band.Read(0, 0, band.Width, band.Height, bufferPtr, enumDataType.UInt16, band.Width, band.Height);
                prd.Read(0, 0, band.Width, band.Height, bufferPtr, enumDataType.Int16, band.Width, band.Height, 10, bandMap, enumInterleave.BSQ);
            }
            long k = _watch.ElapsedMilliseconds;
            _watch.Stop();

            using (FileStream fileStream = new FileStream(@"e:\测试\ReadBSQ_gdal.bin", FileMode.Create))
            // using (FileStream fileStream = new FileStream(@"e:\测试\ReadBSQ_provider.bin", FileMode.Create))
            // using (FileStream fileStream = new FileStream(@"e:\测试\ReadBSQprovider_LDF.bin", FileMode.Create))
            {
                BinaryWriter bw = new BinaryWriter(fileStream);
                for (int i = 0; i < _buffer.Length; i++)
                    bw.Write(_buffer[i]);
            }
        }

        private unsafe void ReadToBIL_Click(object sender, EventArgs e)
        {
            string fname = @"E:\测试\save.LDF";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            IRasterBand band = prd.GetRasterBand(1);
            // int[] bandMap = { 1 };
            int[] bandMap = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            _buffer = new UInt16[band.Width * band.Height * bandMap.Length];

            Stopwatch _watch = new Stopwatch();
            fixed (UInt16* ptr = _buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                _watch.Start();
                prd.Read(0, 0, band.Width, band.Height, bufferPtr, enumDataType.Int16, band.Width, band.Height, 1, bandMap, enumInterleave.BIL);
            }
            long k = _watch.ElapsedMilliseconds;
            _watch.Stop();
            using (FileStream fileStream = new FileStream(@"e:\测试\ReadToBIL_LDF.bin", FileMode.Create))
            {
                BinaryWriter bw = new BinaryWriter(fileStream);
                for (int i = 0; i < _buffer.Length; i++)
                    bw.Write(_buffer[i]);
            }
        }

        private unsafe void ReadToBIP_Click(object sender, EventArgs e)
        {
            string fname = @"E:\测试\save.LDF";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            IRasterBand band = prd.GetRasterBand(1);
            int[] bandMap = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            _buffer = new UInt16[band.Width * band.Height * bandMap.Length];

            Stopwatch _watch = new Stopwatch();
            fixed (UInt16* ptr = _buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                _watch.Start();
                prd.Read(0, 0, band.Width, band.Height, bufferPtr, enumDataType.Int16, band.Width, band.Height, 10, bandMap, enumInterleave.BIP);
            }
            long k = _watch.ElapsedMilliseconds;
            _watch.Stop();
            using (FileStream fileStream = new FileStream(@"e:\测试\ReadToBIP_LDF.bin", FileMode.Create))
            {
                BinaryWriter bw = new BinaryWriter(fileStream);
                for (int i = 0; i < _buffer.Length; i++)
                    bw.Write(_buffer[i]);
            }
        }

        /// <summary>
        ///  将单波段的数据以BSQ写入创建的LDF文件测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private unsafe void WriteBSQ_Click(object sender, EventArgs e)
        {
            /*
             * "INTERLEAVE=[BSQ|BIP|BIL]
             * VERSION=[LDF|LD2|LD3]"
             */
            string fname = @"E:\Smart\FY3A_MERSI_DXX_GLL_L1_20120919_D_0250M_MS.LDF";
            IRasterDataProvider prdRead = GeoDataDriver.Open(fname) as IRasterDataProvider;
            IRasterBand bandRead = prdRead.GetRasterBand(1);
            //  int[] bandMap = { 1, 2, 3, 4, 5 };
            UInt16[] buffer = new UInt16[(bandRead.Width) * (bandRead.Height)];
            Stopwatch sw = new Stopwatch();
            long k;
            fixed (UInt16* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                bandRead.Read(0, 0, bandRead.Width, bandRead.Height, bufferPtr, enumDataType.UInt16, bandRead.Width, bandRead.Height);
                //prdRead.Read(0, 0, bandRead.Width, bandRead.Height, bufferPtr, enumDataType.UInt16, bandRead.Width, bandRead.Height, 5, bandMap, enumInterleave.BSQ);
            }
            using (FileStream fileStream = new FileStream(@"D:\ReadToBSQ.tiff", FileMode.Create))
            {
                BinaryWriter bw = new BinaryWriter(fileStream);
                for (int i = 0; i < buffer.Length; i++)
                    bw.Write(buffer[i]);
            }

            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            prdRead = drv.Create(@"e:\测试\WriteToBSQ.ldf", bandRead.Width, bandRead.Height, 1, enumDataType.UInt16, "INTERLEAVE=BSQ", "VERSION=LDF") as IRasterDataProvider;
            IRasterBand bandWriter = prdRead.GetRasterBand(1);
            fixed (UInt16* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                sw.Start();
                bandWriter.Write(0, 0, bandWriter.Width * 2, bandWriter.Height * 2, bufferPtr, enumDataType.UInt16, bandRead.Width, bandRead.Height);
                sw.Stop();
            }
            k = sw.ElapsedMilliseconds; // k = 21ms;
        }

        /// <summary>
        /// 测试—将单波段的数据以BIL排布方式写入LDF文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private unsafe void WriteBIL_Click(object sender, EventArgs e)
        {
            string fname = @"E:\测试\save.LDF";
            IRasterDataProvider prdRead = GeoDataDriver.Open(fname) as IRasterDataProvider;
            IRasterBand bandRead = prdRead.GetRasterBand(2);
            int[] bandMap = { 1, 2 };
            UInt16[] buffer = new UInt16[(bandRead.Width) * (bandRead.Height)];
            Stopwatch sw = new Stopwatch();
            long k;
            fixed (UInt16* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                bandRead.Read(0, 0, bandRead.Width, bandRead.Height, bufferPtr, enumDataType.UInt16, bandRead.Width, bandRead.Height);
                // prdRead.Read(0, 0, bandRead.Width, bandRead.Height, bufferPtr, enumDataType.UInt16, bandRead.Width, bandRead.Height, 5, bandMap, enumInterleave.BIL);
            }
            using (FileStream fileStream = new FileStream(@"e:\测试\ReadToBIL.bin", FileMode.Create))
            {
                BinaryWriter bw = new BinaryWriter(fileStream);
                for (int i = 0; i < buffer.Length; i++)
                    bw.Write(buffer[i]);
            }

            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            prdRead = drv.Create(@"e:\测试\WriteToBIL.ldf", bandRead.Width, bandRead.Height, 2, enumDataType.UInt16, "INTERLEAVE = BIL ", "VERSION=LDF") as IRasterDataProvider;
            IRasterBand bandWriter = prdRead.GetRasterBand(2);
            fixed (UInt16* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                sw.Start();
                //应多偏移1644个字节
                bandWriter.Write(10, 1, bandWriter.Width / 2, bandWriter.Height / 2, bufferPtr, enumDataType.UInt16, bandRead.Width, bandRead.Height);
                sw.Stop();
            }
            k = sw.ElapsedMilliseconds; // kmax = 79ms; 
        }

        /// <summary>
        /// 测试—将单波段的数据以BIP排布方式写入LDF文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private unsafe void WriteBIP_Click(object sender, EventArgs e)
        {
            //1.ldf
            //用GDAL方式读取一块数据
            //用LDFDriver创建一个和1.ldf同样大小和波段数的文件，new.ldf
            //用GDAL分快读1.ldf,同时写入新文件new.ldf
            //比较1.ldf和new.ldf

            string fname = @"E:\测试\save.LDF";
            IRasterDataProvider prdRead = GeoDataDriver.Open(fname) as IRasterDataProvider;
            IRasterBand bandRead = prdRead.GetRasterBand(1);
            UInt32[] buffer = new UInt32[(bandRead.Width) * (bandRead.Height)];
            Stopwatch sw = new Stopwatch();
            long k;
            fixed (UInt32* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                sw.Start();
                bandRead.Read(0, 0, bandRead.Width, bandRead.Height, bufferPtr, enumDataType.UInt32, bandRead.Width, bandRead.Height);
                sw.Stop();
            }
            long t = sw.ElapsedMilliseconds;
            sw.Reset();
            using (FileStream fileStream = new FileStream(@"e:\测试\ReadToBIP.bin", FileMode.Create))
            {
                BinaryWriter bw = new BinaryWriter(fileStream);
                for (int i = 0; i < buffer.Length; i++)
                    bw.Write(buffer[i]);
            }

            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            prdRead = drv.Create(@"e:\测试\WriteToBIP.ldf", bandRead.Width, bandRead.Height, 1, enumDataType.UInt32, "INTERLEAVE=BIP", "VERSION=LDF") as IRasterDataProvider;
            IRasterBand bandWriter = prdRead.GetRasterBand(1);
            fixed (UInt32* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                sw.Start();
                bandWriter.Write(10, 1, bandWriter.Width, bandWriter.Height, bufferPtr, enumDataType.UInt32, bandRead.Width, bandRead.Height);
                sw.Stop();
            }
            k = sw.ElapsedMilliseconds; // k = 
        }

        private unsafe void WriteBandsBSQ_Click(object sender, EventArgs e)
        {
            string fname = @"E:\测试\save.LDF";
            IRasterDataProvider prdRead = GeoDataDriver.Open(fname) as IRasterDataProvider;
            IRasterBand bandRead = prdRead.GetRasterBand(1);
            int[] bandMap = { 1, 2, 3 };
            byte[] buffer = new byte[(bandRead.Width) * (bandRead.Height) * 3 * 2];
            Stopwatch sw = new Stopwatch();
            long k;
            fixed (byte* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                sw.Start();
                //bandRead.Read(0, 0, bandRead.Width, bandRead.Height, bufferPtr, enumDataType.UInt32, bandRead.Width, bandRead.Height);
                prdRead.Read(0, 0, bandRead.Width, bandRead.Height, bufferPtr, enumDataType.Byte, bandRead.Width, bandRead.Height, 3, bandMap, enumInterleave.BSQ);
                sw.Stop();
            }
            long t = sw.ElapsedMilliseconds;
            sw.Reset();
            using (FileStream fileStream = new FileStream(@"e:\测试\ReadBandsToBSQ.bin", FileMode.Create))
            {
                BinaryWriter bw = new BinaryWriter(fileStream);
                bw.Write(new byte[128]);
                for (int i = 0; i < buffer.Length; i++)
                    bw.Write(buffer[i]);
            }

            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            prdRead = drv.Create(@"e:\测试\WriteBandsToBSQ.ldf", bandRead.Width, bandRead.Height, 3, enumDataType.UInt16, "INTERLEAVE=BSQ", "VERSION=LDF") as IRasterDataProvider;
            IRasterBand bandWriter = prdRead.GetRasterBand(1);
            fixed (byte* ptr = buffer)
            {

                IntPtr bufferPtr = new IntPtr(ptr);
                sw.Start();
                //bandWriter.Write(10, 1, bandWriter.Width, bandWriter.Height, bufferPtr, enumDataType.UInt32, bandRead.Width, bandRead.Height);
                prdRead.Write(0, 0, bandWriter.Width, bandWriter.Height, bufferPtr, enumDataType.Byte, bandRead.Width, bandRead.Height, 3, bandMap, enumInterleave.BSQ);
                sw.Stop();
            }
            k = sw.ElapsedMilliseconds; // k = 
            prdRead.Dispose();
            using (FileStream fileStream = new FileStream(@"e:\测试\WriteBandsToBSQ.ldf", FileMode.Open))
            {
                BinaryWriter bw = new BinaryWriter(fileStream);
                bw.Write(new byte[128]);
            }
        }

        private unsafe void WriteBandsBIL_Click(object sender, EventArgs e)
        {
            string fname = @"E:\测试\save.LDF";
            IRasterDataProvider prdRead = GeoDataDriver.Open(fname) as IRasterDataProvider;
            IRasterBand bandRead = prdRead.GetRasterBand(1);
            int[] bandMap = { 1, 2, 3 };
            UInt32[] buffer = new UInt32[(bandRead.Width) * (bandRead.Height) * 3];
            Stopwatch sw = new Stopwatch();
            long k;
            fixed (UInt32* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                sw.Start();
                //bandRead.Read(0, 0, bandRead.Width, bandRead.Height, bufferPtr, enumDataType.UInt32, bandRead.Width, bandRead.Height);
                prdRead.Read(0, 0, bandRead.Width, bandRead.Height, bufferPtr, enumDataType.UInt32, bandRead.Width, bandRead.Height, 3, bandMap, enumInterleave.BSQ);
                sw.Stop();
            }
            long t = sw.ElapsedMilliseconds;
            sw.Reset();
            using (FileStream fileStream = new FileStream(@"e:\测试\ReadBandsToBIL.bin", FileMode.Create))
            {
                BinaryWriter bw = new BinaryWriter(fileStream);
                for (int i = 0; i < buffer.Length; i++)
                    bw.Write(buffer[i]);
            }

            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            prdRead = drv.Create(@"e:\测试\WriteBandsToBIL.ldf", bandRead.Width, bandRead.Height, 1, enumDataType.UInt32, "INTERLEAVE=BIL", "VERSION=LDF") as IRasterDataProvider;
            IRasterBand bandWriter = prdRead.GetRasterBand(1);
            fixed (UInt32* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                sw.Start();
                //bandWriter.Write(10, 1, bandWriter.Width, bandWriter.Height, bufferPtr, enumDataType.UInt32, bandRead.Width, bandRead.Height);
                prdRead.Write(0, 0, bandWriter.Width, bandWriter.Height, bufferPtr, enumDataType.UInt32, bandRead.Width, bandRead.Height, 3, bandMap, enumInterleave.BIL);
                sw.Stop();
            }
            k = sw.ElapsedMilliseconds; // k = 
        }

        private unsafe void WriteBandsBIP_Click(object sender, EventArgs e)
        {
            string fname = @"E:\测试\save.LDF";
            IRasterDataProvider prdRead = GeoDataDriver.Open(fname) as IRasterDataProvider;
            IRasterBand bandRead = prdRead.GetRasterBand(1);
            int[] bandMap = { 1, 2, 3 };
            UInt32[] buffer = new UInt32[(bandRead.Width) * (bandRead.Height) * 3];
            Stopwatch sw = new Stopwatch();
            long k;
            fixed (UInt32* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                sw.Start();
                //bandRead.Read(0, 0, bandRead.Width, bandRead.Height, bufferPtr, enumDataType.UInt32, bandRead.Width, bandRead.Height);
                prdRead.Read(0, 0, bandRead.Width, bandRead.Height, bufferPtr, enumDataType.UInt32, bandRead.Width, bandRead.Height, 3, bandMap, enumInterleave.BSQ);
                sw.Stop();
            }
            long t = sw.ElapsedMilliseconds;
            sw.Reset();
            using (FileStream fileStream = new FileStream(@"e:\测试\ReadBandsToBIP.bin", FileMode.Create))
            {
                BinaryWriter bw = new BinaryWriter(fileStream);
                for (int i = 0; i < buffer.Length; i++)
                    bw.Write(buffer[i]);
            }

            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            prdRead = drv.Create(@"e:\测试\WriteBandsToBIP.ldf", bandRead.Width, bandRead.Height, 1, enumDataType.UInt32, "INTERLEAVE=BIP", "VERSION=LDF") as IRasterDataProvider;
            IRasterBand bandWriter = prdRead.GetRasterBand(1);
            fixed (UInt32* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                sw.Start();
                //bandWriter.Write(10, 1, bandWriter.Width, bandWriter.Height, bufferPtr, enumDataType.UInt32, bandRead.Width, bandRead.Height);
                prdRead.Write(0, 0, bandWriter.Width, bandWriter.Height, bufferPtr, enumDataType.UInt32, bandRead.Width, bandRead.Height, 3, bandMap, enumInterleave.BIP);
                sw.Stop();
            }
            k = sw.ElapsedMilliseconds; // k = 
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        int X = 0;
        private unsafe void btnMERSI250_Click(object sender, EventArgs e)
        {
            string fnam = @"D:\masData中文\Mersi\FY3A_MERSI_GBAL_L1_20100429_0300_0250M_MS.HDF";
            //fnam = "f:\\FY3A_VIRR_2010_06_24_11_39_1000M_L1B.HDF";
            IRasterDataProvider prd = GeoDataDriver.Open(fnam) as IRasterDataProvider;
            UInt16[] buffer = new UInt16[prd.Width * prd.Height];
            fixed (UInt16* pointer = buffer)
            {
                IntPtr ptr = new IntPtr(pointer);
                for (int i = 0; i < 10; i++)
                {
                    prd.GetRasterBand(1).Read(0, 0, prd.Width, prd.Height, ptr, prd.DataType, prd.Width, prd.Height);
                }
            }

            prd.Dispose();
            Text = (X++).ToString();
            GC.Collect();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string fnam = @"D:\masData\MODIS\TERRA_2010_03_25_03_09_GZ.MOD021KM.hdf";
            using (IRasterDataProvider prd = GeoDataDriver.Open(fnam) as IRasterDataProvider)
            {
                MessageBox.Show(prd.ToString() + prd.BandCount);
                UInt16[] buffer = new UInt16[prd.Width * prd.Height];
                unsafe
                {
                    int bandCount = prd.BandCount;
                    fixed (UInt16* pointer = buffer)
                    {
                        IntPtr ptr = new IntPtr(pointer);
                        for (int i = 0; i < bandCount; i++)
                        {
                            IRasterBand band = prd.GetRasterBand(i + 1);
                            MessageBox.Show(band.ToString() + i.ToString());
                            band.Read(0, 0, prd.Width, prd.Height, ptr, prd.DataType, prd.Width, prd.Height);
                            MessageBox.Show(buffer[0].ToString());
                        }
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Read1BDBand();
        }

        private void REad1BDLon()
        {
            string fname = @"D:\masData\noaa_1bd\NOAA18_AVHRR_CHINA_L1_20090806_N3_1000M.1bd";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            IBandProvider bp = prd.BandProvider;
            string[] dsnames = bp.GetDatasetNames();
            IRasterBand[] rb = bp.GetBands("Latitude");//SolarZenith,"Latitude"//"SatelliteZenith""RelativeAzimuth"
            Console.WriteLine(rb);
            Double[] buffer = new Double[prd.Width * prd.Height];
            unsafe
            {
                fixed (Double* ptr = buffer)
                {
                    IntPtr bufferPtr = new IntPtr(ptr);
                    rb[0].Read(0, 0, prd.Width, prd.Height, bufferPtr, enumDataType.Double, prd.Width, prd.Height);
                }
            }
            double min = buffer[0];
            foreach (double d in buffer)
            {
                if (min > d)
                    min = d;
                if (min == 0.0)
                    break;
            }
        }

        private void Read1BDBand()
        {
            string fname = @"D:\masData\noaa_1bd\NOAA18_AVHRR_CHINA_L1_20090806_N3_1000M.1bd";
            //fname = @"D:\masData\NOAA18_1BD\NA18_AVHRR_HRPT_L1_ORB_MLT_NUL_20120319_2026_1100M_PJ.L1B";

            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            UInt16[] buffer = new UInt16[prd.Width * prd.Height];
            unsafe
            {
                fixed (UInt16* ptr = buffer)
                {
                    IntPtr bufferPtr = new IntPtr(ptr);
                    prd.Read(0, 0, prd.Width, prd.Height, bufferPtr, enumDataType.UInt16, prd.Width, prd.Height, 1, new int[] { 1 }, enumInterleave.BSQ);
                    using (IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver)
                    {
                        using (IRasterDataProvider prdWriter = drv.Create(@"D:\masData\noaa_1bd\t.ldf", prd.Width, prd.Height, 1,
                                enumDataType.UInt16, "INTERLEAVE=BSQ", "VERSION=LDF", "WITHHDR=TRUE") as IRasterDataProvider)
                        {
                            IRasterBand band = prdWriter.GetRasterBand(1);
                            band.Write(0, 0, band.Width, band.Height, bufferPtr, enumDataType.UInt16, band.Width, band.Height);
                        }
                    }
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            TestHdfRead t = new TestHdfRead();
            t.Test();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string fnam = @"D:\masData中文\Mersi\FY3A_MERSI_GBAL_L1_20110501_0245_0250M_MS.HDF";
            int n = 1000;
            for (int i = 0; i < n; i++)
            {
                using (IHdfOperator hdf = new Hdf5Operator(fnam))
                {
                }
                GC.Collect();
            }
            Text = "OK.";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string fname = "d:\\NOAA18_AVHRR_CHINA_L1_20090806_N3_1000M.1bd";
            fname = "f:\\FY3A_MERSI_GBAL_L1_20110501_0250_0250M_MS_PRJ_Whole.LDF";
            fname = @"G:\项目数据库\测试数据\00_通用功能\多源数据读取\NOAA18_AVHRR_2013_05_10_14_59.1BD";
            IRasterDataProvider srcRaster = RasterDataDriver.Open(fname) as IRasterDataProvider;
            //D1BDDataProvider dp = srcRaster as D1BDDataProvider;
            IBandProvider bandPrd = srcRaster.BandProvider;
            IRasterBand[] bands = bandPrd.GetBands("Longitude");
            if (bands == null || bands.Length == 0 || bands[0] == null)
                throw new Exception("读取波段" + "Longitude" + "失败:无法获取该通道信息");
            try
            {
                using (IRasterBand band = bands[0])
                {
                    double[] data = new double[band.Width * band.Height];
                    unsafe
                    {
                        fixed (Double* ptr = data)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(0, 0, band.Width, band.Height, bufferPtr, enumDataType.Double, band.Width, band.Height);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("读取波段" + "Longitude" + "失败:" + ex.Message, ex);
            }
            sw.Stop();
            MessageBox.Show("读取NOAA经度数据集花费时间" + sw.ElapsedMilliseconds + "ms");
        }

        object st;
        private unsafe void button9_Click(object sender, EventArgs e)
        {
            string fname = "f:\\FY3A_MERSI_GBAL_L1_20110501_0250_0250M_MS_PRJ_Whole.LDF";
            //fname = @"E:\气象局项目\FY3B_VIRRX_GBAL_L1_20101128_0500_1000M_MS_PRJ_Whole.LDF";
            //fname = @"H:\测试\测试数据\ld3\eb_2011_07_27_03_02_GZ局部有云.ld3";
            //fname = @"f:\\FIR_NDVI_FY3A_MERSI_250M_20120610164136_20120611164136.dat";
            //fname = @"F:\技术研究\MAS_II\源代码\【控制】监测分析框架\Output\SystemData\RasterTemplate\China_XjRaster.dat";
            //fname = @"F:\产品与项目\MAS-II\源代码0618-night\【控制】UI框架\SMART\bin\Release\TEMP\DST_VISY_NUL_NUL_NUL_20120620112207_20120621112207.dat";
            //fname = @"f:\FOG_CCYI_FY3A_MERSI_1000M_20120328014000_20120731140816.dat";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;

            st = prd.GetRasterBand(1).Stretcher;

            IRasterDataProvider prd2 = GeoDataDriver.Open(fname) as IRasterDataProvider;
            prd.Dispose();
            //prd2.Read(0,0,

            // DataIdentify it = DataIdentifyMatcher.Match("f:\\FY3A_MERSI_GBAL_L1_20110501_0250_0250M_MS_PRJ_Whole.LDF");

            //object obj = RgbStretcherFactory.GetStretcher("FY3A", "VIRR", false, 1);
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string fname = dlg.FileName;
                    OpenFile(fname);
                }
            }
        }

        ICoordTransform cd;
        ISpatialReference spRef;
        private void OpenFile(string fname)
        {
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            cd = prd.CoordTransform;
            spRef = prd.SpatialRef;
            IOrbitProjectionTransformControl c = prd.OrbitProjectionTransformControl;
            c.Build();
            IOrbitProjectionTransform tran = c.OrbitProjectionTransform;
            float x = 0, y = 0;
            int row = 0, col = 0;
            tran.InvertTransform(100, 200, ref x, ref y);
            tran.Transform(x, y, ref row, ref col);
            c.Free();
            MessageBox.Show(prd.DataIdentify.IsOrbit.ToString());
        }

        private void btnGetGDAL_Click(object sender, EventArgs e)
        {
            //int maxCacheSize = Gdal.GetCacheMax();
            //int usedCacheSize = Gdal.GetCacheUsed();
            //MessageBox.Show(maxCacheSize.ToString() + "," + usedCacheSize.ToString());
            //Gdal.AllRegister();
            //Driver drv = Gdal.GetDriverByName("MEM");
            //Dataset ds = drv.Create("M1", 10000, 100000, 1, DataType.GDT_UInt16, new string[] { "DATAPOINTER=0"});
            //Console.WriteLine(ds.RasterXSize.ToString());
            //Console.WriteLine(ds.RasterYSize.ToString());
            //Console.WriteLine(ds.RasterCount.ToString());
        }

        private unsafe void button11_Click(object sender, EventArgs e)
        {
            string fname = null;
            using (OpenFileDialog dlg = new OpenFileDialog())
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    fname = dlg.FileName;
            if (fname == null)
                return;
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            IRasterBand lonBand = null, latBand = null;
            IRasterBand[] bands = prd.BandProvider.GetBands("Longitude");
            lonBand = bands[0];
            bands = prd.BandProvider.GetBands("Latitude");
            latBand = bands[0];
            int width = latBand.Width;
            int height = latBand.Height;
            float[] lonValues = new float[width * height];
            float[] latValues = new float[width * height];
            fixed (float* lonPtr = lonValues, latPtr = latValues)
            {
                IntPtr lonBuffer = new IntPtr(lonPtr);
                IntPtr latBuffer = new IntPtr(latPtr);
                lonBand.Read(0, 0, width, height, lonBuffer, enumDataType.Float, width, height);
                latBand.Read(0, 0, width, height, latBuffer, enumDataType.Float, width, height);
                //
                MessageBox.Show("OK.");
                int[] rowIdxes = new int[10];
                int[] colIdxs = new int[10];
                float[] lineLon = new float[10] { 115, 115, 115, 115, 115, 115, 115, 115, 115, 115 };
                float[] lineLat = new float[10] { 33, 36f, 39, 42, 45, 48, 49, 52, 54, 56 };

                NearestSearcher ns = new NearestSearcher(lonValues, latValues, width, height);
                for (int i = 0; i < 10; i++)
                {
                    ns.Cal(lineLon[i], lineLat[i], ref colIdxs[i], ref rowIdxes[i]);
                }

                using (Bitmap bm = new Bitmap(width, height, PixelFormat.Format24bppRgb))
                {
                    using (Graphics g = Graphics.FromImage(bm))
                    {
                        Point[] pts = new Point[rowIdxes.Length];
                        for (int i = 0; i < rowIdxes.Length; i++)
                        {
                            int x = colIdxs[i];
                            int y = rowIdxes[i];
                            if (x == 0 || y == 0)
                                continue;
                            pts[i] = new Point(x, y);
                        }
                        g.DrawCurve(Pens.Red, pts);
                    }
                    bm.Save("f:\\orbitGrid.bmp", ImageFormat.Bmp);
                }
            }
        }

        private unsafe void ReadLd2_Click(object sender, EventArgs e)
        {
            string filename = @"H:\测试\问题数据\DC-TERRA_2008_07_30_03_28_GZ.MOD02HKM_PRJ.ld2";
            filename = @"H:\测试\测试数据\ld3\eb_2011_07_27_03_02_GZ局部有云.ld3";
            filename = @"E:\王羽\蓝藻-LD3数据\th_2012_05_05_02_44_GZ.ld3";
            //  filename = @"E:\王羽\蓝藻-LD3数据\th_2012_05_16_02_26_GZ.ld3";
            //HdrFile hdrtest = TestHdrParser();
            //return;
            IRasterDataDriver driver = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            IRasterDataProvider prd = GeoDataDriver.Open(filename) as IRasterDataProvider;
            ILdfDataProvider ldfPrd = prd as ILdfDataProvider;
            LdfHeaderBase ldfHeader = new Ld3Header(filename);
            HdrFile hdr = ldfHeader.ToHdrFile(); //new Hdr(ldfHeader);
            byte[] buffer = new byte[prd.Width * prd.Height * 2];
            IRasterBand band = prd.GetRasterBand(1);
            fixed (byte* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                band.Read(0, 0, prd.Width, prd.Height, bufferPtr, enumDataType.UInt16, prd.Width, prd.Height);
            }
        }

        private HdrFile TestHdrParser()
        {
            string filename = @"E:\第一张盘\通用功能测试数据\01_投影\FY3A_VIRRX_GBAL_L1_20110322_0525_1000M_MS_PRJ_DXX.HDR";
            filename = @"E:\王羽\蓝藻-LD3数据\th_2012_05_05_02_44_GZ.hdr";
            HdrFile hdr = HdrFileParser.ParseFromHdrfile(filename);
            return hdr;
        }

        private void Read1a5_Click(object sender, EventArgs e)
        {
            string fname = @"H:\测试\测试数据\1a5\1110908.1a5";
            IRasterDataDriver driver = GeoDataDriver.GetDriverByName("NOAA_1A5") as IRasterDataDriver;
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            GeoDo.RSS.DF.NOAA.ID1A5DataProvider noaaPrd = prd as GeoDo.RSS.DF.NOAA.ID1A5DataProvider;
            GeoDo.RSS.DF.NOAA.D1A5Header header = noaaPrd.Header as GeoDo.RSS.DF.NOAA.D1A5Header;
        }

        private unsafe void ReadMVG_Click(object sender, EventArgs e)
        {
            string fname = @"H:\测试\测试数据\mvg\FireSoilVectorToReaster.mvg";
            //fname = @"H:\测试\测试数据\mvg\FIR_DBLV_FY3A_VIRR_1000M_FED_P001_20090429021500.mvg";
            IRasterDataDriver dri = GeoDataDriver.GetDriverByName("MVG") as IRasterDataDriver;
            IMvgDataProvider prd = dri.Open(fname, enumDataProviderAccess.ReadOnly) as IMvgDataProvider;
            //IMvgDataProvider prd = GeoDataDriver.Open(fname) as IMvgDataProvider;

            MvgHeader header = prd.Header;
            HdrFile hdr = header.ToHdrFile(); //new Hdr(ldfHeader);
            byte[] buffer = new byte[prd.Width * prd.Height * 2];
            IRasterBand band = prd.GetRasterBand(1);
            fixed (byte* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                band.Read(0, 0, prd.Width, prd.Height, bufferPtr, enumDataType.UInt16, prd.Width, prd.Height);
            }

            using (FileStream fileStream = new FileStream(@"h:\WriteMvg.mvg", FileMode.Create))
            {
                BinaryWriter bw = new BinaryWriter(fileStream);
                byte[] xxx = new byte[header.HeaderSize];
                for (int i = 0; i < header.HeaderSize; i++)
                    xxx[i] = Byte.MaxValue;
                bw.Write(xxx);
                bw.Write(buffer);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            IRasterDataProvider prd = drv.Create("f:\\mmf1.dat", 30000, 30000, 1, enumDataType.Byte, new object[] { "EXTHEADERSIZE=8" });
        }

        private void button13_Click(object sender, EventArgs e)
        {
            DataIdentify di = DataIdentifyMatcher.Match(@"f:\\FY3A_MERSI_GBAL_L1_20120328_0140_1000M_MS_PRJ_DXX.LDF");

            string fname = "f:\\FIR_NDVI_FY3A_MERSI_250M_20120620003711_20120621003715.dat";
            IRasterDataProviderConverter c = new RasterDataProviderConverter();
            IRasterDataProvider dataProvider = c.ConvertDataType<float, Int16>(fname, enumDataType.Int16, "f:\\converInt16.ldf", (v) => { return (Int16)(v * 1000); });
        }

        private void WorldFile_Click(object sender, EventArgs e)
        {
            string rasterFileName = @"E:\VGT_NDVI_FY3A_MERSI_1000M_DXX_P001_20090130033500.LDF";
            WorldFile wf = new WorldFile();
            IRasterDataProvider raster = GeoDataDriver.Open(rasterFileName) as IRasterDataProvider;
            string dir = Path.GetDirectoryName(rasterFileName);
            string fileName = Path.GetFileNameWithoutExtension(rasterFileName) + ".bmp";
            fileName = Path.Combine(dir, fileName);
            wf.CreatWorldFile(raster.ResolutionX, raster.ResolutionY, raster.CoordEnvelope.MinX, raster.CoordEnvelope.MinY, fileName);
            SpatialReference spa = new SpatialReference(new GeographicCoordSystem());
            wf.CreatXmlFile(spa, fileName);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string fname = dlg.FileName;
                    GenerateOverview(fname);
                }
            }
        }

        private void GenerateOverview(string fname)
        {
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            IOverviewGenerator v = prd as IOverviewGenerator;
            Size size = v.ComputeSize(1000);
            Bitmap bm = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            v.Generate(new int[] { 3, 4, 2 }, ref bm);
            sw.Stop();
            Text = sw.ElapsedMilliseconds.ToString();
            bm.Save("f:\\1.bmp", ImageFormat.Bmp);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            Rectangle srcRect = new Rectangle();
            Rectangle dstRect = new Rectangle();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 10000000; i++)
            {
                int x = srcRect.Width;
                int y = dstRect.Height;
            }
            Text = "struct:" + sw.ElapsedMilliseconds.ToString();
        }

        private unsafe void button16_Click(object sender, EventArgs e)
        {
            Rectangle srcRect = new Rectangle();
            Rectangle dstRect = new Rectangle();
            Rectangle[] rects = new Rectangle[] { srcRect, dstRect };

            Stopwatch sw = new Stopwatch();
            sw.Start();
            fixed (Rectangle* srcRectPtr = &rects[0], dstRectPtr = &rects[1])
            {
                int w = srcRect.Width;
                int h = dstRect.Height;
                for (int i = 0; i < 10000000; i++)
                {
                    int x = w;// srcRectPtr->Width;
                    int y = h;// dstRectPtr->Height;
                }
            }
            Text = "pointer:" + sw.ElapsedMilliseconds.ToString();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    GetFileInfo(dlg.FileName);
                }
            }
        }

        private void GetFileInfo(string fileName)
        {
            IRasterDataProvider prd = GeoDataDriver.Open(fileName) as IRasterDataProvider;
            if (prd != null)
            {
                int w = prd.Width;
                RasterToRaw(prd);
            }
        }

        IRasterDataProvider dstProvider;
        private void RasterToRaw(IRasterDataProvider srcProvider)
        {
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("GDAL") as IRasterDataDriver;
            dstProvider = drv.Create("f:\\wbq.raw", srcProvider.Width, srcProvider.Height, srcProvider.BandCount, srcProvider.DataType, "DriverName=ENVI");

            dstProvider.Dispose();
        }

        private void btnArrayDataProvider_Click(object sender, EventArgs e)
        {
            byte[][] bandValues = new byte[3][];
            int width = 0, height = 0;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string fname = dlg.FileName;
                    IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
                    bandValues[0] = GetBandValues(prd, 1);
                    bandValues[1] = GetBandValues(prd, 2);
                    bandValues[2] = GetBandValues(prd, 3);
                    width = prd.Width;
                    height = prd.Height;
                    prd.Dispose();
                }
            }

            IRasterDataProvider arrayPrd = new ArrayRasterDataProvider<byte>("Array", bandValues, width, height);
            Size size = arrayPrd.ComputeSize(100);
            Bitmap bm = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
            arrayPrd.Generate(new int[] { 1, 2, 3 }, ref bm);
            bm.Save("f:\\1.bmp", ImageFormat.Bmp);
        }

        private byte[] GetBandValues(IRasterDataProvider prd, int bandNo)
        {
            byte[] buffer = new byte[prd.Width * prd.Height];
            GCHandle h = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            prd.GetRasterBand(bandNo).Read(0, 0, prd.Width, prd.Height, h.AddrOfPinnedObject(), prd.DataType, prd.Width, prd.Height);
            h.Free();
            return buffer;
        }

        private void button18_Click(object sender, EventArgs e)
        {
            IGeoDataProvider prd = GeoDataDriver.Open(@"E:\DATA\GeoEye\po_621270_metadata.txt", "ComponentID=0000000") as IRasterDataProvider;
        }

        private void button19_Click(object sender, EventArgs e)
        {
            DataIdentify id = new DataIdentify();
            if (id.IsOrbit)
                Text = id.ToString();

            OpenFileDialog dlg = new OpenFileDialog();
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    IRasterDataProvider prd = GeoDataDriver.Open(dlg.FileName) as IRasterDataProvider;
                    if (prd.IsSupprtOverviews)
                    {
                        prd.BuildOverviews((pro, tip) => { Text = pro.ToString() + "%"; });
                    }
                }
            }

            IRasterDataProvider prd1 = GeoDataDriver.Open(dlg.FileName) as IRasterDataProvider;
        }

        private void button20_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Hdf5DatasetSelection frm = new Hdf5DatasetSelection();
                    frm.LoadFile(dlg.FileName);
                    frm.ShowDialog();
                }
            }
        }

        private void button24_Click(object sender, EventArgs e)
        { }

        private unsafe void button21_Click(object sender, EventArgs e)
        {
            /*
             * "INTERLEAVE=[BSQ|BIP|BIL]
             * VERSION=[LDF|LD2|LD3]"
             */
            string fname = @"E:\Smart\2013-05-28 二期用户测试\测试数据\00_通用功能\多源数据读取\FY3A_MERSI_L2_PAD_MLT_GLL_20130527-150023_POAD_0250M_MS.tif";
            fname = @"D:\Smart\培训数据（通用功能）\减灾星数据\20130330-L20000972984-xz.tif";
            using (IRasterDataProvider prdRead = GeoDataDriver.Open(fname) as IRasterDataProvider)
            {
                int width = prdRead.Width;
                int height = prdRead.Height;
                ISpatialReference _dstSpatialRef = prdRead.SpatialRef;
                CoordEnvelope env = prdRead.CoordEnvelope;
                string[] options = new string[]{
                    "DRIVERNAME=GTiff",
                    "TFW=YES",
                    "TILED=YES",
                    //"GDAL_TIFF_INTERNAL_MASK_TO_8BIT =NO",
                    //"INTERLEAVE=PIXEL", 
                    //"TFW=YES",
                    //"SPATIALREF=" + _dstSpatialRef.ToProj4String(),
                    //"MAPINFO={" + 1 + "," + 1 + "}:{" + env.MinX + "," + env.MaxY + "}:{" + prdRead.ResolutionX + "," + prdRead.ResolutionY + "}",
                    "WKT="+_dstSpatialRef.ToWKTString(),
                    "GEOTRANSFORM="+string.Format("{0},{1},{2},{3},{4},{5}",env.MinX, prdRead.ResolutionX,0, env.MaxY,0, -prdRead.ResolutionY)
                    };                

                string[] gdalOptions = new string[]
                {
                    "TFW=YES",
                    "TILED=YES",
                    "WKT="+_dstSpatialRef.ToWKTString(),
                    "GEOTRANSFORM="+string.Format("{0},{1},{2},{3},{4},{5}",env.MinX, prdRead.ResolutionX,0, env.MaxY,0, -prdRead.ResolutionY)
                };

                //Gdal.AllRegister();
                //Driver div = OSGeo.GDAL.Gdal.GetDriverByName("GTiff");
                //using (Dataset ds = div.Create(@"D:\test.tif", width, height, prdRead.BandCount, DataType.GDT_Byte, gdalOptions))
                //{
                //    double[] geo = new double[] { env.MinX, prdRead.ResolutionX, 0, env.MaxY, 0, -prdRead.ResolutionY };
                //    ds.SetGeoTransform(geo);
                //    ds.SetProjection(_dstSpatialRef.ToWKTString());
                //    Byte[] buffer = new Byte[width * height];

                //    for (int i = 1; i <= prdRead.BandCount; i++)
                //    {
                //        fixed (Byte* ptr = buffer)
                //        {
                //            IntPtr bufferPtr = new IntPtr(ptr);
                //            prdRead.GetRasterBand(i).Read(0, 0, width, height, bufferPtr, enumDataType.Byte, width, height);
                //        }
                //        fixed (Byte* ptr = buffer)
                //        {
                //            IntPtr bufferPtr = new IntPtr(ptr);
                //            ds.GetRasterBand(i).WriteRaster(0, 0, width, height, bufferPtr, width, height, DataType.GDT_Byte, 0, 0);
                //        }
                //    }
                //}

                IRasterDataDriver drv = GeoDataDriver.GetDriverByName("GDAL") as IRasterDataDriver;
                using (IRasterDataProvider prdWrite = drv.Create(@"D:\test.tif", prdRead.Width, prdRead.Height, prdRead.BandCount, enumDataType.Byte, options) as IRasterDataProvider)
                {
                    Byte[] buffer = new Byte[width * height];
                    for (int i = 1; i <= prdRead.BandCount; i++)
                    {
                        //Stopwatch sw = new Stopwatch();
                        fixed (Byte* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            prdRead.GetRasterBand(i).Read(0, 0, width, height, bufferPtr, enumDataType.Byte, width, height);
                        }
                        fixed (Byte* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            prdWrite.GetRasterBand(i).Write(0, 0, width, height, bufferPtr, enumDataType.Byte, width, height);
                        }
                    }
                }
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fis = new FolderBrowserDialog())
            {
                if (fis.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = fis.SelectedPath;
                }
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fis = new FolderBrowserDialog())
            {
                if (fis.ShowDialog() == DialogResult.OK)
                {
                    textBox2.Text = fis.SelectedPath;
                }
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            string fname = @"G:\临时文件夹\FY1D\FY1D_AVHRR_GDPT_L1_ORB_MLT_NUL_20070101_0041_4000M_PJ.1A5";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            FY1_1A5DataProvider fy1a5Prd = prd as FY1_1A5DataProvider;
            IRasterBand[] bands = fy1a5Prd.BandProvider.GetBands("Latitude");
            try
            {
                using (IRasterBand band = bands[0])
                {
                    double[] data = new double[band.Width * band.Height];
                    unsafe
                    {
                        fixed (Double* ptr = data)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(0, 0, band.Width, band.Height, bufferPtr, enumDataType.Double, band.Width, band.Height);
                            MessageBox.Show("读取波段成功");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("读取波段" + "Longitude" + "失败:" + ex.Message, ex);
            }
        }

        private unsafe void button25_Click(object sender, EventArgs e)
        {


            string file = @"G:\工程项目\Smart二期\气象MAS二期\SMART\bin\Release\SystemData\FY2NOM\FY2E_latlon22882288.raw";
            IRasterDataProvider dataPrd=null;
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
                            br.Write(buffer[i]+7.5f);
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

        private unsafe void button26_Click(object sender, EventArgs e)
        {
           
            string fname = @"C:\Users\DongW\Desktop\data\SNW_MWSD_FY3B_MWRIA_010KM_20130101043400.dat";
            IRasterDataProvider dataPrd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            double[] value = new double[dataPrd.Width * dataPrd.Height];
            double[] x = new double[value.Length];
            double[] y = new double[value.Length];
            fixed(double* ptr=value)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                dataPrd.GetRasterBand(1).Read(0, 0, dataPrd.Width, dataPrd.Height, bufferPtr, enumDataType.Double, dataPrd.Width, dataPrd.Height);
            }
            double minX=dataPrd.CoordEnvelope.MinX;
            double maxY=dataPrd.CoordEnvelope.MaxY;
            double reX=dataPrd.ResolutionX;
            double reY=dataPrd.ResolutionY;
            for (int i = 0; i < dataPrd.Height; i++)
            {
                for (int j = 0; j < dataPrd.Width; j++)
                {
                    x[i * dataPrd.Width + j] = minX + j * reX;
                    y[i * dataPrd.Width + j] = maxY - i * reY;
                }
            }
            List<double> valueList = new List<double>();
            List<double> xList = new List<double>();
            List<double> yList = new List<double>();
            for (int i = 0; i < value.Length; i += 10)
            {
                valueList.Add(value[i]);
                xList.Add(x[i]);
                yList.Add(y[i]);
            }
            //IDW_Interpolation interpolation = new IDW_Interpolation();
            //interpolation.CoordPointXArr = xList.ToArray();
            //interpolation.CoordPointYArr = yList.ToArray();
            //interpolation.PointValueArr = valueList.ToArray();
            //interpolation.DoIDWinterpolation(0.05, 0.05, @"D:\1.ldf", "LDF", enumDataType.Double, new Action<int, string>((int progerss, string text) =>
            //{
            //    Console.WriteLine(progerss+"  "+text);
            //}),dataPrd.SpatialRef.ToProj4String());
            
        }
    }
}
