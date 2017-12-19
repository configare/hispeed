using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.DF.HDF4.Cloudsat;
using HDF5DotNet;

namespace GeoDo.Smart.Tools.MODLSTPro
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fname = "hdf4.hdf";

            var hdf = new H4File(null, null, null, new long[] {0, 0});
            hdf.Load(fname);
            H4SDS[] sds = hdf.Datasets;

            //测试读取的科学数据集及其属性
            for (int i = 0; i < hdf.Num_Datasets; i++)
            {
                H4SDS sd = hdf.Datasets[i];
                HDFAttribute[] attrs = sd.SDAttributes;
                if (sd.Rank == 2)
                {
                    int buffersize = (int) sd.Dimsizes[0]*sd.Dimsizes[1];
                    int typesize = HDFDataType.GetSize(sd.Datatype);
                    IntPtr ptr = Marshal.AllocHGlobal(buffersize*typesize);
                    sd.Read(new int[] {0, 0}, null, sd.Dimsizes, ptr);
                    short[] buffer = new short[buffersize];
                    Marshal.Copy(ptr, buffer, 0, buffersize);
                    Marshal.FreeHGlobal(ptr);
                }
            }

            //byte[] head1024 = GetHeader1024Bytes(fname);
            //object args = null;

            //var hdf4Driver = new HDF4Driver();
            //hdf4Driver.Open(fname, head1024, enumDataProviderAccess.ReadOnly, null);
            //var prd = new Hdf4RasterDataProvider(fname, head1024, hdf4Driver, args);

            //List<IRasterBand> _rasterBands = new List<IRasterBand>();
            //for (int i = 0; i < sds.Length; i++)
            //{
            //    CloudSatRasterBand rasterBand = new CloudSatRasterBand(prd, sds[i], i + 1);
            //    _rasterBands.Add(rasterBand);
            //    break;
            //}
        }

        private static byte[] GetHeader1024Bytes(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[1024];
                fs.Read(buffer, 0, 1024);
                fs.Close();
                return buffer;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var f4name = "hdf41.hdf";
            var f5name = "hdf5.hdf";

            ConvertHdf4To5(f4name, f5name, null);

            MessageBox.Show("转换完成！");
        }

        private void ConvertHdf4To5(string f4name, string f5name, Action<string> messageAction)
        {
            var hdf = new H4File(null, null, null, new long[] {0, 0});
            hdf.Load(f4name);
            H4SDS[] sds = hdf.Datasets;

            // Create a new file using H5F_ACC_TRUNC access,
            // default file creation properties, and default file
            // access properties.
            H5FileId fileId = H5F.create(f5name, H5F.CreateMode.ACC_TRUNC);

            //测试读取的科学数据集及其属性
            //for (int k = 0; k < hdf.Num_Datasets; k++)
            for (int k = 0; k < 1; k++)
            {
                H4SDS sd = hdf.Datasets[k];
                HDFAttribute[] attrs = sd.SDAttributes;
                string sdName = sd.Name;
                int rank = sd.Rank;

                if (messageAction != null)
                    messageAction(string.Format("正在转换数据集 {0}", sdName));

                if (rank == 2)
                {
                    int nx = sd.Dimsizes[0];
                    int ny = sd.Dimsizes[1];

                    int buffersize = nx*ny;
                    int typesize = HDFDataType.GetSize(sd.Datatype);
                    IntPtr ptr = Marshal.AllocHGlobal(buffersize*typesize);
                    sd.Read(new int[] {0, 0}, null, sd.Dimsizes, ptr);
                    short[] buffer = new short[buffersize];
                    Marshal.Copy(ptr, buffer, 0, buffersize);
                    Marshal.FreeHGlobal(ptr);

                    // Data and input buffer initialization.
                    int[,] data = new int[nx, ny];
                    for (int i = 0; i < nx; i++)
                        for (int j = 0; j < ny; j++)
                        {
                            int index = i*ny + j;
                            data[i, j] = buffer[index];
                        }

                    // Describe the size of the array and create the data space for fixed
                    // size dataset.
                    long[] dims = new long[rank];
                    dims[0] = Convert.ToInt64(nx);
                    dims[1] = Convert.ToInt64(ny);
                    H5DataSpaceId dspaceId = H5S.create_simple(rank, dims);

                    // Define datatype for the data in the file.
                    H5DataTypeId dtypeId = H5T.copy(H5T.H5Type.NATIVE_INT);

                    // Create the data set DATASETNAME.
                    H5DataSetId dsetId = H5D.create(fileId, sdName, dtypeId, dspaceId);

                    // Write the one-dimensional data set array
                    H5D.write(dsetId, new H5DataTypeId(H5T.H5Type.NATIVE_INT), new H5Array<int>(data));

                    // Close dataset and file.
                    H5D.close(dsetId);
                    H5F.close(fileId);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string dir = @"E:\工作资料\中科九度\业务学习\MODLST\500836287";
            var f4name1 = "hdf41.hdf";
            var f4name2 = "hdf42.hdf";

            var f5name = "hdf5_3.hdf";

            //string[] f4names = { f4name1, f4name2 };
            string[] f4names =
            {
                Path.Combine(dir, "MOD11A1.A2013225.h25v04.041.2013226172532.hdf"),
                Path.Combine(dir, "MOD11A1.A2013225.h25v05.041.2013226172515.hdf"),
                Path.Combine(dir, "MOD11A1.A2013225.h25v06.041.2013226172440.hdf"),
                Path.Combine(dir, "MOD11A1.A2013225.h26v04.041.2013226171945.hdf"),
                Path.Combine(dir, "MOD11A1.A2013225.h26v05.041.2013226172452.hdf"),
                Path.Combine(dir, "MOD11A1.A2013225.h26v06.041.2013226172433.hdf"),
                Path.Combine(dir, "MOD11A1.A2013225.h27v04.041.2013226171935.hdf"),
                Path.Combine(dir, "MOD11A1.A2013225.h27v05.041.2013226171828.hdf"),
                Path.Combine(dir, "MOD11A1.A2013225.h27v06.041.2013226172423.hdf")
            };

            //var hdf4FileAttrs = getHdf4FileAttrs(f4names);

            //if (!Validate(hdf4FileAttrs))
            //    return;           

            //ConvertHdf4To5(hdf4FileAttrs, f5name, (mess) =>
            //{
            //    label1.Text = mess;
            //    Application.DoEvents();
            //});

            SHdf4To5 hdf4To5 = new SHdf4To5(f4names, f5name) {MessageAction = MessageAction(), SdsCountAction = null};

            try
            {
                UtilHdf4To5.ConvertHdf4To5(hdf4To5);
            }
            catch (Exception ex)
            {
                MessageBox.Show("拼接失败！" + ex.Message);
            }
            MessageBox.Show("转换完成！");
        }

        private Action<string, int, int> MessageAction()
        {
            return (mess, i, j) =>
            {
                label1.Text = mess;
                Application.DoEvents();
            };
        }


        private static Hdf4FileAttr[] getHdf4FileAttrs(string[] f4names)
        {
            var hdf4FileAttrs = new Hdf4FileAttr[f4names.Length];
            for (int i = 0; i < f4names.Length; i++)
            {
                hdf4FileAttrs[i] = new Hdf4FileAttr(f4names[i]);
            }
            return hdf4FileAttrs;
        }

        private static bool Validate(Hdf4FileAttr[] hdf4FileAttrs)
        {
            if (hdf4FileAttrs.Length < 1)
                return false;
            if (hdf4FileAttrs.Length == 1)
                return true;

            Hdf4FileAttr hdf4FileAttr = hdf4FileAttrs[0];
            for (int i = 1; i < hdf4FileAttrs.Length; i++)
            {
                if (!hdf4FileAttr.IsSameClassHdf4(hdf4FileAttrs[i]))
                    return false;
            }

            return true;
        }

        //private static bool IsH4SDSSame(H4SDS[] sds0, H4SDS[] sdsi)
        //{
        //    for (int i = 0; i < sds0.Length; i++)
        //    {
        //        var s0 = sds0[i];
        //        var si = sdsi[i];
        //        if (s0.Name != si.Name)
        //            return false;
        //    }
        //    return true;
        //}
    }
}
