using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.AWX;
using System.IO;

namespace AWXtest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string awxfile = @"D:\Geodo\陈超\TEST\FOG_Sea_FY3B_VIRR_1000M_20140713100000_N.AWX";
            using (IRasterDataProvider awxdprd = GeoDataDriver.Open(awxfile) as IRasterDataProvider)
            {
                if (awxdprd != null)
                {
                    int bandx = awxdprd.BandCount;
                }
            }
            awxfile = @"D:\Geodo\陈超\TEST\MST2_FDI_ALL_NOM_20140714_0732.hdf.awx";
            using (IRasterDataProvider awxdprd = GeoDataDriver.Open(awxfile) as IRasterDataProvider)
            {
                if (awxdprd != null)
                {
                    int bandx = awxdprd.BandCount;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            string datf = @"D:\Geodo\陈超\TEST\FOG_Sea_FY3B_VIRR_1000M_20140713100000.dat";
            string hdrf = @"D:\Geodo\陈超\TEST\FOG_Sea_FY3B_VIRR_1000M_20140713100000.hdr";
            try
            {
                AWXFile awx = new AWXFile();
                awx.Write(hdrf, datf,false);
            }
            catch(SystemException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            int a = 3;
            Byte[] b=BitConverter.GetBytes(a);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //string dataFile1 = "K:\\待更新\\异常文件\\MOD06_L2.A2001051.1515.hdf";
            //string dataFile2 = "K:\\待更新\\异常文件\\MOD06_L2.A2001051.1515.hdf";
            //string dataFile3 = "K:\\待更新\\异常文件\\MOD06_L2.A2001051.1515.hdf";
            //string dataFile4 = "K:\\待更新\\异常文件\\MOD06_L2.A2001051.1515.hdf";

            string dataSet = "Cloud_Optical_Thickness";
            string[] openArgs = new string[] { "datasets=" + dataSet };
            string dir = "K:\\待更新\\异常文件";
            string[] dataFiles = Directory.GetFiles(dir, "MOD06_L2*.HDF", SearchOption.AllDirectories);
            foreach (string dataFile in dataFiles)
            {
                try
                {
                    using (IRasterDataProvider mainRaster = RasterDataDriver.Open(dataFile, openArgs) as IRasterDataProvider)
                    {
                        if (mainRaster != null)
                            continue;
                    }
                }
                catch (System.Exception ex)
                {
                    continue;
                }
            }

        }
    }
}
