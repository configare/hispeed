using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    public partial class frmColorRampEditor : Form
    {
        public event EventHandler ApplyHandler = null;
        public delegate void StatMinValueAndMaxValueHandler(out int minValue, out int maxValue);
        public event StatMinValueAndMaxValueHandler OnStatMinValueAndMaxValue;
        private int _maxLimitValue = 0;
        private int _minLimitValue = 0;

        public frmColorRampEditor()
        {
            InitializeComponent();
            Load += new EventHandler(frmColorRampEditor_Load);
        }

        public int MaxLimitValue
        {
            set { _maxLimitValue = value; }
        }

        public int MinLimitValue
        {
            set { _minLimitValue = value; }
        }

        void frmColorRampEditor_Load(object sender, EventArgs e)
        {
            ColorItemPersist[] ps = LinearGradientTableFactory.GetAll();
            if (ps == null || ps.Length == 0)
                return;
            foreach (ColorItemPersist p in ps)
            {
                listBox1.Items.Add(p.Name);
            }
            ucLinearColorRampEditor1.IsDrawScales = false;
            btnApplyMinValueMaxValue_Click(null, null);
            ucLinearColorRampEditor1.ReApply();
        }
       
        public int MinValue
        {
            set
            {
                txtMinValue.Value = value;
                ucLinearColorRampEditor1.MinValue = value;
            }
            get
            {
                return (int)ucLinearColorRampEditor1.MinValue;
            }
        }

        public int MaxValue
        {
            set
            {
                txtMaxValue.Value = value;
                ucLinearColorRampEditor1.MaxValue = value;
            }
            get
            {
                return (int)ucLinearColorRampEditor1.MaxValue;
            }
        }

        public ColorItem[] ColorItems
        {
            get
            {
                return ucLinearColorRampEditor1.ColorItems;
            }
        }

        public Color InvalidColor
        {
            get { return txtInvalidColor.BackColor; }
        }

        public void AddColorItems(ColorItem[] items)
        {
            ucLinearColorRampEditor1.AddColorItems(items);
        }

        public void AddColorItem(ColorItem item)
        {
            ucLinearColorRampEditor1.AddColorItem(item);
        }

        public Color[] ComputeColors(int count)
        {
            return ucLinearColorRampEditor1.ComputeColors(count);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtMinValue.Value < _minLimitValue)
            {
                MsgBox.ShowInfo("输入的最小值超过当前影像的数据类型的最小值,请重新输入。");
                return;
            }
            if (txtMaxValue.Value > _maxLimitValue)
            {
                MsgBox.ShowInfo("输入的最大值超过当前影像的数据类型的最大值,请重新输入。");
                return;
            }
            DialogResult = DialogResult.OK;
            if (ApplyHandler != null)
                ApplyHandler(this, e);
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (txtMinValue.Value < _minLimitValue)
            {
                MsgBox.ShowInfo("输入的最小值超过当前影像的数据类型的最小值,请重新输入。");
                return;
            }
            if (txtMaxValue.Value > _maxLimitValue)
            {
                MsgBox.ShowInfo("输入的最大值超过当前影像的数据类型的最大值,请重新输入。");
                return;
            }
            btnApply.Enabled = false;
            try
            {
                if (ApplyHandler != null)
                    ApplyHandler(this, e);
            }
            finally
            {
                btnApply.Enabled = true;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == string.Empty)
            {
                MsgBox.ShowInfo("方案名称不能为空,请重新输入!");
                return;
            }
            string nme = textBox1.Text.Trim();
            foreach (object obj in listBox1.Items)
            {
                if (obj.ToString() == nme)
                {
                    MsgBox.ShowInfo("已存在名称为\"" + nme + "\"的方案,请重新输入或者先删除已经存在的同名方案!");
                    textBox1.SelectAll();
                    return;
                }
            }
            LinearGradientTableFactory.SaveToFile(ucLinearColorRampEditor1.ColorItems, nme);
            listBox1.Items.Add(nme);
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                LinearGradientTableFactory.Delete(listBox1.SelectedItem.ToString());
                listBox1.Items.Remove(listBox1.SelectedItem);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
                return;
            string name = listBox1.SelectedItem.ToString();
            ColorItemPersist p = LinearGradientTableFactory.GetByName(name);
            if (p == null)
                return;
            textBox1.Text = p.Name;
            ucLinearColorRampEditor1.Apply(p.ColorItems);
        }

        private void frmColorRampEditor_Load_1(object sender, EventArgs e)
        {

        }

        private void txtInvalidColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog dlg = new ColorDialog())
            {
                dlg.Color = txtInvalidColor.BackColor;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtInvalidColor.BackColor = dlg.Color;
                }
            }
        }

        private void btnStatMinValueMaxValue_Click(object sender, EventArgs e)
        {
            btnStatMinValueMaxValue.Enabled = false;
            try
            {
                if (OnStatMinValueAndMaxValue != null)
                {
                    int minValue = 0;
                    int maxValue = 0;
                    OnStatMinValueAndMaxValue(out minValue, out maxValue);
                    MinValue = minValue;
                    MaxValue = maxValue;
                    ucLinearColorRampEditor1.Invalidate();
                }
            }
            finally
            {
                btnStatMinValueMaxValue.Enabled = true;
            }
        }

        private void btnApplyMinValueMaxValue_Click(object sender, EventArgs e)
        {
            btnApplyMinValueMaxValue.Enabled = false;
            try
            {
                ucLinearColorRampEditor1.MinValue = (int)txtMinValue.Value;
                ucLinearColorRampEditor1.MaxValue = (int)txtMaxValue.Value;
                ucLinearColorRampEditor1.ReApply();
            }
            finally
            {
                btnApplyMinValueMaxValue.Enabled = true;
            }
        }
    }
}
