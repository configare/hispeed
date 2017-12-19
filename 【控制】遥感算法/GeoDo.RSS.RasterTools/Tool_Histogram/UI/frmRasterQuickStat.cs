using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.RasterTools
{
    public partial class frmRasterQuickStat : Form
    {
        public frmRasterQuickStat()
        {
            InitializeComponent();
            SizeChanged += new EventHandler(frmRasterQuickStat_SizeChanged);
        }

        public void Apply(string fileName, Dictionary<int, RasterQuickStatResult> results)
        {
            ucHistogramGraph1.Apply(fileName,results);
            ucHistogramValues1.Apply(fileName,results);
        }

        void frmRasterQuickStat_SizeChanged(object sender, EventArgs e)
        {
            this.splitContainer1.SplitterDistance = Height / 2;
        }
    }
}
