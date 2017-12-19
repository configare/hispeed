using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.UI;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public partial class UCCommExpCoefficient : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler;
        private string expFilename;

        public UCCommExpCoefficient()
        {
            InitializeComponent();
        }

        public object GetArgumentValue()
        {
            if (!File.Exists(expFilename))
                return new DRTExpCoefficientCollection();
            return ReadExpCofficientFile.LoadExpCoefficientFile(expFilename); ;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return GetArgumentValue();
        }

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            expFilename = string.Empty;
            if (panel == null)
                return;
            UCExtractPanel ucPanel = panel as UCExtractPanel;
            if (ucPanel == null)
                return;
            IMonitoringSubProduct subProduct = ucPanel.MonitoringSubProduct;
            if (subProduct == null)
                return;
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\SystemData\\ProductArgs\\" + subProduct.Definition.ProductDef.Identify;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            expFilename = Path.Combine(path, subProduct.Definition.Identify + "ExpInfo.xml");
            txtFilename.Text = Path.GetFileName(expFilename);
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            DRTExpCoefficientCollection exp = null;
            if (File.Exists(expFilename))
                exp = ReadExpCofficientFile.LoadExpCoefficientFile(expFilename);
            else
                exp = new DRTExpCoefficientCollection();
            using (frmDroughtSetting frm = new frmDroughtSetting(exp, expFilename))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (_handler != null)
                        _handler(GetArgumentValue());
                }
            }
        }

        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return false;
            }
            set
            {

            }
        }
    }
}
