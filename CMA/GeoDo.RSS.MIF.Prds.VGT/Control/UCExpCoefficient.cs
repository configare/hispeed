using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public partial class UCExpCoefficient : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler = null;
        private string expFilename;

        public UCExpCoefficient()
        {
            InitializeComponent();
        }

        public object GetArgumentValue()
        {
            return ReadExpCofficientFile.LoadExpCoefficientFile(expFilename);
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return GetArgumentValue();
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }


        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return ReadExpCofficientFile.LoadExpCoefficientFile(expFilename);
        }


        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\SystemData\\ProductArgs\\VGT\\";
            expFilename = Path.Combine(path, panel.MonitoringSubProduct.Definition.Name + "Exp.txt");
        }

        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return false;
            }
        }

        private void btEditArgs_Click(object sender, EventArgs e)
        {
            VGTExpCoefficientCollection expParas = File.Exists(expFilename) ? ReadExpCofficientFile.LoadExpCoefficientFile(expFilename) : new VGTExpCoefficientCollection();
            using (frmExpCoefficientSetting frm = new frmExpCoefficientSetting(expParas, expFilename))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (_handler != null)
                        _handler(ReadExpCofficientFile.LoadExpCoefficientFile(expFilename));
                }
            }
        }
    }
}
