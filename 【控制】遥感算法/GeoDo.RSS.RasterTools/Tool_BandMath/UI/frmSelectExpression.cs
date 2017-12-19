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
    public partial class frmSelectExpression : Form
    {
        private string _expression = null;

        public frmSelectExpression()
        {
            InitializeComponent();
        }

        public string Expression
        {
            get { return _expression; }
            set 
            {
                _expression = value;
                ucBandMath1.SetExpression(_expression);
            }
        }

        private void ucBandMath1_ApplyClicked(object sender, EventArgs e)
        {
            if (sender == null)
                return;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            _expression = sender.ToString();
            this.Close();
        }

        private void ucBandMath1_CancelClicked(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
