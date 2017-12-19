using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    public partial class UCLinearColorRampEditor : UserControl
    {
        public UCLinearColorRampEditor()
        {
            InitializeComponent();
            toolTip1.SetToolTip(txtColor, "单击编辑待插入颜色");
            ucColorPanel1.OnMoveColorBarHandler += new OnMoveColorBarHandler(ucColorPanel1_OnMoveColorBarHandler);
        }

        void ucColorPanel1_OnMoveColorBarHandler(object sender, string value, Color color)
        {
            if (!string.IsNullOrEmpty(value))
                label1.Text = "波段值:" + value;
            else
                label1.Text = string.Empty;
            txtColor.BackColor = color;
        }

        public int MinValue
        {
            set
            {
                ucColorPanel1.MinValue = value; 
            }
            get 
            {
                return ucColorPanel1.MinValue;
            }
        }

        public int MaxValue
        {
            set 
            {
                ucColorPanel1.MaxValue = value;
            }
            get 
            {
                return ucColorPanel1.MaxValue;
            }
        }

        public Color[] ComputeColors(int count)
        {
            return ucColorPanel1.GetColors(count);
        }

        public bool IsDrawScales
        {
            get { return ucColorPanel1.IsDrawScales; }
            set { ucColorPanel1.IsDrawScales = value; }
        }

        public void ClearItems()
        {
            ucColorPanel1.ClearItems();
        }

        public ColorItem[] ColorItems
        {
            get
            {
                return ucColorPanel1.ColorItems;
            }
        }

        public void AddColorItems(ColorItem[] items)
        {
            ucColorPanel1.AddColorItems(items);
        }

        public void AddColorItem(ColorItem item)
        {
            ucColorPanel1.AddColorItem(item,true);
        }

        private void txtColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog dlg = new ColorDialog())
            {
                dlg.Color = txtColor.BackColor;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtColor.BackColor = dlg.Color;
                }
            }
        }

        private void btnInsertColor_Click(object sender, EventArgs e)
        {
            ColorItem item = new ColorItem(0f, txtColor.BackColor);
            ucColorPanel1.AddColorItem(item,true);
        }

        internal void Apply(ColorItem[] colorItems)
        {
            ucColorPanel1.ClearItems();
            ucColorPanel1.AddColorItems(colorItems);
        }

        internal void ReApply()
        {
            ucColorPanel1.Invalidate();
        }
    }
}
