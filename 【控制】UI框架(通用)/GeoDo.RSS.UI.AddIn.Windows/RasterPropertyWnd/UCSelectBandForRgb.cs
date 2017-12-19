using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    public partial class UCSelectBandForRgb : UserControl
    {
        private int _bandCount = 0;

        public UCSelectBandForRgb()
        {
            InitializeComponent();
            txtRed.KeyPress += new KeyPressEventHandler(txtBandNo_KeyPress);
            txtGreen.KeyPress += new KeyPressEventHandler(txtBandNo_KeyPress);
            txtBlue.KeyPress += new KeyPressEventHandler(txtBandNo_KeyPress);
            rdGray.CheckedChanged += new EventHandler(rdGray_CheckedChanged);
            rdRGB.CheckedChanged += new EventHandler(rdRGB_CheckedChanged);
            txtRed.Enter += new EventHandler(txtRed_Enter);
            txtGreen.Enter += new EventHandler(txtGreen_Enter);
            txtBlue.Enter += new EventHandler(txtBlue_Enter);
        }

        void txtBlue_Enter(object sender, EventArgs e)
        {
            rdBlue.Checked = true;
        }

        void txtGreen_Enter(object sender, EventArgs e)
        {
            rdGreen.Checked = true;
        }

        void txtRed_Enter(object sender, EventArgs e)
        {
            rdRed.Checked = true;
        }

        void rdRGB_CheckedChanged(object sender, EventArgs e)
        {
            rdRed.Text = "红(Red)";
            rdRed.ForeColor = Color.Red;
            rdGreen.Visible = rdBlue.Visible = true;
            txtGreen.Visible = txtBlue.Visible = true;
            rdRed.Checked = true;
        }

        void rdGray_CheckedChanged(object sender, EventArgs e)
        {
            rdRed.Text = "灰(Gray)";
            rdRed.ForeColor = Color.Black;
            rdGreen.Visible = rdBlue.Visible = false;
            txtGreen.Visible = txtBlue.Visible = false;
            rdRed.Checked = true;
        }

        void txtBandNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) || e.KeyChar == (char)0;
        }

        public int[] SelectedBandNos
        {
            get
            {
                int band1, band2, band3;
                if (rdGray.Checked)
                {
                    int.TryParse(txtRed.Text.ToString(), out band1);
                    CorrectBandNo(ref band1);
                    return new int[] { band1 };
                }
                else
                {
                    int.TryParse(txtRed.Text.ToString(), out band1);
                    int.TryParse(txtGreen.Text.ToString(), out band2);
                    int.TryParse(txtBlue.Text.ToString(), out band3);
                    CorrectBandNo(ref band1);
                    CorrectBandNo(ref band2);
                    CorrectBandNo(ref band3);
                    return new int[] { band1, band2, band3 };
                }
            }
        }

        private void CorrectBandNo(ref int band)
        {
            if (band < 1 || band > _bandCount)
                band = 1;
        }

        public void SetBandNo(int b)
        {
            if (rdGray.Checked)
            {
                txtRed.SelectedIndex = b - 1;
                rdRed.Checked = true;
                return;
            }
            if (rdRed.Checked)
            {
                txtRed.SelectedIndex = b - 1;
                rdGreen.Checked = true;
            }
            else if (rdGreen.Checked)
            {
                txtGreen.SelectedIndex = b - 1;
                rdBlue.Checked = true;
            }
            else
            {
                txtBlue.SelectedIndex = b - 1;
                rdRed.Checked = true;
            }
        }

        public void Apply(IRasterDrawing drawing)
        {
            txtRed.Items.Clear();
            txtGreen.Items.Clear();
            txtBlue.Items.Clear();
            txtRed.Text = txtGreen.Text = txtBlue.Text = string.Empty;
            if (drawing == null)
                return;
            //
            _bandCount = drawing.DataProvider.BandCount;
            rdRGB.Enabled = _bandCount != 1;
            rdRGB.Checked = drawing.SelectedBandNos.Length == 3;
            for (int i = 1; i <= _bandCount; i++)
            {
                txtRed.Items.Add(i.ToString());
                txtGreen.Items.Add(i.ToString());
                txtBlue.Items.Add(i.ToString());
            }
            //
            if (drawing.SelectedBandNos.Length == 1)
            {
                txtRed.SelectedIndex = drawing.SelectedBandNos[0] - 1;
                rdGray.Checked = true;
            }
            else
            {
                txtRed.SelectedIndex = drawing.SelectedBandNos[0] - 1;
                txtGreen.SelectedIndex = drawing.SelectedBandNos[1] - 1;
                txtBlue.SelectedIndex = drawing.SelectedBandNos[2] - 1;
                rdRGB.Checked = true;
            }
        }
    }
}
