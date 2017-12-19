using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    public partial class frmColorItem : Form
    {
        ProductColor _productColor;

        public frmColorItem()
        {
            InitializeComponent();
        }

        public void SetProductColor(ProductColor productColor)
        {
            if (productColor == null)
            {
                productColor = new ProductColor();
                productColor.Color = Color.Black;
            }
            txtName.Text = productColor.LableText;
            txtMin.Text = productColor.MinValue.ToString();
            txtMax.Text = productColor.MaxValue.ToString();
            checkBox1.Checked = productColor.DisplayLengend;
            pnlColor.BackColor = productColor.Color;
            _productColor = productColor;
        }

        public ProductColor ProductColor
        {
            get { return _productColor; }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            string lable = txtName.Text;
            //if (string.IsNullOrWhiteSpace(lable))
            //{
            //    errorProvider1.SetError(txtName, "不能为空");
            //    return;
            //}
            float minValue;
            if (!float.TryParse(txtMin.Text, out minValue))
            {
                errorProvider1.SetError(txtMin, "不合法");
                return;
            }
            float maxValue;
            if (!float.TryParse(txtMax.Text, out maxValue))
            {
                errorProvider1.SetError(txtMax, "不合法");
                return;
            }
            Color color = pnlColor.BackColor;
            _productColor.LableText = lable;
            _productColor.MinValue = minValue;
            _productColor.MaxValue = maxValue;
            _productColor.DisplayLengend = checkBox1.Checked;
            _productColor.Color = color;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void pnlColor_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            using (ColorDialog color = new ColorDialog())
            {
                color.AllowFullOpen = true;
                color.AnyColor = true;
                if (color.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    pnlColor.BackColor = color.Color;
                }
            }
        }

        private void btnCalcel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
