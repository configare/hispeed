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
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.Prds.SNW
{
    public partial class UCSnowDepthArg : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler;
        string _currentRasterDir = null;
        bool _isExcuteArgumentValueChanged = false;
        List<string> defaultArgs ;

        public UCSnowDepthArg()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "角度文件(*.ldf)|*.ldf|所有文件(*.*)|*.*";
                if (string.IsNullOrEmpty(_currentRasterDir))
                    dialog.InitialDirectory = _currentRasterDir;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtFileName.Text = dialog.FileName;
                    //if (_handler != null)
                    //    _handler(GetArgumentValue());
                }

            }
        }

        public object GetArgumentValue()
        {
            string[] argStrs = new string[9];
            argStrs[0] = txtFileName.Text;
            argStrs[1] = txtA0.Text;
            argStrs[2] = txtA1.Text;
            argStrs[3] = txtA2.Text;
            argStrs[4] = txtA3.Text;
            argStrs[5] = txtB0.Text;
            argStrs[6] = txtB1.Text;
            argStrs[7] = txtB2.Text;
            argStrs[8] = txtB3.Text;
            return argStrs;
        }

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            if (panel == null)
                return;
            UCExtractPanel ucPanel = panel as UCExtractPanel;
            if (ucPanel == null)
                return;
            IMonitoringSubProduct subProduct = ucPanel.MonitoringSubProduct;
            if (subProduct == null)
                return;
            IArgumentProvider arp = subProduct.ArgumentProvider;
            if (arp == null)
                return;
            string filename = arp.DataProvider.fileName;
            _currentRasterDir = Path.GetDirectoryName(filename);
        }

        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return _isExcuteArgumentValueChanged;
            }
            set
            {
                _isExcuteArgumentValueChanged = value;
            }
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            if (ele == null)
                return null;
            IEnumerable<XElement> elements = ele.Elements("ValueItem");
            if (elements != null && elements.Count() > 0)
            {
                string name;
                defaultArgs = new List<string>();
                foreach (XElement item in elements)
                {
                    name = item.Attribute("name").Value.ToUpper();
                    foreach (Control tb in this.Controls)
                        if (tb is TextBox)
                        {
                            if (tb.Name.Contains(name))
                            {
                                tb.Text = item.Attribute("value").Value;
                                defaultArgs.Add(tb.Text);
                            }
                        }  
                }
            }
            return null;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (defaultArgs != null && defaultArgs.Count == 8)
            {
                txtA0.Text = defaultArgs[0];
                txtA1.Text = defaultArgs[1];
                txtA2.Text = defaultArgs[2];
                txtA3.Text = defaultArgs[3];
                txtB0.Text = defaultArgs[4];
                txtB1.Text = defaultArgs[5];
                txtB2.Text = defaultArgs[6];
                txtB3.Text = defaultArgs[7];
            }
        }
    }
}
