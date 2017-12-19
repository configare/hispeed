using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public partial class frmDroughtSetting : Form
    {
        private object _droughtParas = null;
        private string _filename = null;

        public frmDroughtSetting()
        {
            InitializeComponent();
        }

        public frmDroughtSetting(object droughtParas,string filename)
        {
            InitializeComponent();
            propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid1_PropertyValueChanged);
            propertyGrid1.SelectedObject = droughtParas;
            _droughtParas = droughtParas;
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
            WriteExpCoefficient.SaveToFile(_droughtParas,_filename);
            DialogResult = DialogResult.OK;
        }

        private void btApply_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
