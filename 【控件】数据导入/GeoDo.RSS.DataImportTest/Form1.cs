using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.DI.MVG;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.DI;

namespace GeoDo.RSS.DITest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnMVGXMLParser_Click(object sender, EventArgs e)
        {
            string error = string.Empty;
            IDataImportDriver driver = DataImport.GetDriver("FIR", "PLST", @"D:\MAS_Workspace\OutputItem\Fire\MultiValueGraph\20130311\FIR_DBLV_NOAA18_AVHRR_1000M_NUL_P001_20120407061200.mvg", null);

        }

        private void btPGS_Click(object sender, EventArgs e)
        {
            string fileanme;
            OpenFileDialog dilaog = new OpenFileDialog();
            if (dilaog.ShowDialog() == DialogResult.OK)
                fileanme = dilaog.FileName;
            else
                return;
            IGeoDataDriver geoDriver = GeoDataDriver.GetDriverByName("GDAL");
            if (geoDriver == null)
                return;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fileanme;
            OpenFileDialog dilaog = new OpenFileDialog();
            if (dilaog.ShowDialog() == DialogResult.OK)
                fileanme = dilaog.FileName;
            else
                return;
            IDataImportDriver driver = DataImport.GetDriver("FIR", "DBLV", fileanme, null);
            string error = string.Empty;
            RasterDataProvider pro=GeoDataDriver.Open(@"G:\培训光盘内容\培训数据（监测产品）\03_火情\山西火\FY3A_VIRR_0FHA_GLL_L1_20130426_0245_1000M_Day_MOSA.LDF") as RasterDataProvider;
            driver.Do("FIR", "DBLV", pro, fileanme, out error);
        }
    }
}
