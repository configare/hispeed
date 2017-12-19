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
    public partial class frmBandVarMapper : Form
    {
        public frmBandVarMapper()
        {
            InitializeComponent();
        }

        public void SetExpression(string expression)
        {
            ucBandVarSetter1.SetExpression(expression);
        }

        public void SetFiles(UCBandVarSetter.FileBandNames[] files)
        {
            ucBandVarSetter1.SetFileBandNames(files);
        }

        public Dictionary<string, int> MappedBandNos
        {
            get { return ucBandVarSetter1.MappedBandNos; }
        }

        public string OutFileName
        {
            get { return ucBandVarSetter1.SaveAsFileName; }
            set { ucBandVarSetter1.SetSaveAsFileName(value); }
        }

        private void ucBandVarSetter1_ApplyClicked(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void ucBandVarSetter1_CancelClicked(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
