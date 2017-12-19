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
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.MIF.Prds.UHI
{
    public partial class UCShpFileSetting : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler;
        private string _currentRasterFile;
        private ISmartSession _session = null;
        private string _shpFname = null;
        private bool _isExcuteArgumentValueChangedEvent = false;

        public UCShpFileSetting()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog dialog=new OpenFileDialog())
            {
                dialog.Multiselect = false;
                dialog.Filter = "矢量点文件(*.txt)|*.txt|所有文件(*.*)|*.*";
                if(dialog.ShowDialog()==DialogResult.OK)
                {
                    _shpFname = dialog.FileName;
                    txtShpFname.Text = dialog.FileName;
                    if(_handler!=null)
                        _handler(GetArgumentValue());
                }
            }
        }

        public object GetArgumentValue()
        {
            return _shpFname;
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
            if (arp != null)
            {
                if (arp.DataProvider != null)
                    _currentRasterFile = arp.DataProvider.fileName;
                object obj = arp.GetArg("SmartSession");
                if (obj != null)
                    _session = obj as ISmartSession;
            }
        }

        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return _isExcuteArgumentValueChangedEvent;
            }
            set
            {
                _isExcuteArgumentValueChangedEvent = value;
            }
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return null;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }
    }
}
