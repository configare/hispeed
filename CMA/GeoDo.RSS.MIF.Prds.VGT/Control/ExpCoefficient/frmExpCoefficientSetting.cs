using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public partial class frmExpCoefficientSetting : Form
    {
        private object _expParas = null;
        private string _filename = null;

        public frmExpCoefficientSetting()
        {
            InitializeComponent();
        }

        public frmExpCoefficientSetting(object expParas,string filename)
        {
            InitializeComponent();
            propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid1_PropertyValueChanged);
            propertyGrid1.SelectedObject = expParas;
            _expParas = expParas;
            _filename = filename;
        }

        void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyGrid1.Refresh();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            WriteExpCoefficient.SaveToFile(_expParas, _filename);
            DialogResult = DialogResult.OK;
        }

        private void btApply_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
