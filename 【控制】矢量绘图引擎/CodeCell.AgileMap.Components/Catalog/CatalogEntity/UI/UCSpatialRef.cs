using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.UIs;

namespace CodeCell.AgileMap.Components
{
    public partial class UCSpatialRef : UserControl
    {
        public UCSpatialRef()
        {
            InitializeComponent();
        }

        public ISpatialReference SpatialReference
        {
            get { return textBox1.Tag as ISpatialReference; }
            set
            {
                if (value != null)
                {
                    richTextBox1.Text = value.Name;
                    textBox1.Text = value.Name;
                    richTextBox1.Text = (value as ISpatialRefFormat).FormatToString();
                }
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Filter = "ESRI Projection Files(*.prj)|*.prj";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        ISpatialReference sref = SpatialReferenceFactory.GetSpatialReferenceByPrjFile(dlg.FileName);
                        richTextBox1.Text = (sref as ISpatialRefFormat).FormatToString();
                        textBox1.Text = dlg.FileName;
                        textBox1.Tag = sref;
                    }
                }
            }
            catch (Exception ex)
            {
                textBox1.Text = string.Empty;
                textBox1.Tag = null;
                richTextBox1.Text = string.Empty;
                MsgBox.ShowError(ex.Message);
            }
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            btnOpen_Click(null, null);
        }
    }
}
