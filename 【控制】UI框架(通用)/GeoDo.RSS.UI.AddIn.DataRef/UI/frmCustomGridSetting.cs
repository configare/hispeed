using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.DataRef
{
    public partial class frmCustomGridSetting : Form
    {
        private ISmartSession _session;
        private double _stepX;

        public frmCustomGridSetting()
        {
            InitializeComponent();
        }

        public frmCustomGridSetting(ISmartSession session)
        {
            InitializeComponent();
            _session = session;
        }

        private void btnSure_Click(object sender, EventArgs e)
        {
            _stepX = 1;
            if (_session == null)
                return;
            try
            {
                _stepX = Convert.ToDouble(txtJD.Text.Trim());
            }
            catch (Exception)
            {
                MessageBox.Show("经纬度间隔不正确，请输入大于0的数字！");
            }
            if (_stepX <= 0 )
                return;
            ICommand cmd = _session.CommandEnvironment.Get(4041);
            if (cmd != null)
                cmd.Execute(_stepX.ToString());
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
