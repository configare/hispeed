using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.RasterTools
{
    public partial class UCHistogramGraph : UserControl
    {
        public UCHistogramGraph()
        {
            InitializeComponent();
        }

        public void Apply(string fileName, Dictionary<int, RasterQuickStatResult> results)
        {
            btnStatItems.DropDownItems.Add(GetItem("所有波段",-1));
            btnStatItems.DropDownItems.Add(new ToolStripSeparator());
            foreach (int bandNo in results.Keys)
            {
                btnStatItems.DropDownItems.Add(GetItem("波段 " + bandNo.ToString(), bandNo));
            }
            //
            ucHistogramGrapCanvas1.Apply(fileName,results);
            //
            (btnStatItems.DropDownItems[0] as ToolStripMenuItem).Checked = true;
        }

        private ToolStripItem GetItem(string text, int bandNo)
        {
            ToolStripMenuItem it = new ToolStripMenuItem(text);
            it.Tag = bandNo;
            it.Click += new EventHandler(it_Click);
            return it;
        }

        void it_Click(object sender, EventArgs e)
        {
            foreach (object item in btnStatItems.DropDownItems)
            {
                if (item is ToolStripMenuItem)
                    (item as ToolStripMenuItem).Checked = false;
            }
            ToolStripMenuItem it = sender as ToolStripMenuItem;
            it.Checked = true;
            int bandNo = int.Parse(it.Tag.ToString());
            if (bandNo == -1)
                ucHistogramGrapCanvas1.ChangeBand(-1);
            else
                ucHistogramGrapCanvas1.ChangeBand(bandNo);
        }

        private void btnSaveToFile_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Bitmap Files(*.bmp)|*.bmp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    ucHistogramGrapCanvas1.SaveAsFile(dlg.FileName);
                }
            }
        }
    }
}
