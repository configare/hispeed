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
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    public partial class UCIceCoverage : UserControl, IArgumentEditorUI2
    {
        private bool _isExcuteArgumentValueChanged = false;
        private Action<object> _handler;

        public UCIceCoverage()
        {
            InitializeComponent();
        }

        public object GetArgumentValue()
        {
            string bandName="";
            if (cmbRange.SelectedIndex == 0)
            {
                switch (cmbBand.SelectedIndex)
                {
                    case 0: bandName = "datasets=icecon_north_asc";
                        break;
                    case 1: bandName = "datasets=icecon_north_des";
                        break;
                    case 2: bandName = "datasets=icecon_north_avg";
                        break;
                }
            }
            else
            {
                switch (cmbBand.SelectedIndex)
                {
                    case 0: bandName = "datasets=icecon_south_asc";
                        break;
                    case 1: bandName = "datasets=icecon_south_des";
                        break;
                    case 2: bandName = "datasets=icecon_south_avg";
                        break;
                }
            }
            return new string[]{txtFileName.Text ,bandName};
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
                {
                    IRasterDataProvider dataPrd=arp.DataProvider;
                    txtFileName.Text = arp.DataProvider.fileName;
                    //set cmbBand and cmbRange
                    if (dataPrd.BandCount >= 1)
                    {
                        string datasetStr = dataPrd.GetRasterBand(1).Description;
                        if (!string.IsNullOrEmpty(datasetStr))
                        {
                            string[] stringArray = datasetStr.Split(new char[]{'_'},StringSplitOptions.RemoveEmptyEntries);
                            if (stringArray != null && stringArray.Length == 3)
                            {
                                if (stringArray[1] == "north")
                                    cmbRange.SelectedIndex = 0;
                                else
                                    cmbRange.SelectedIndex = 1;
                                switch (stringArray[2])
                                {
                                    case "asc":
                                        {
                                            cmbBand.SelectedIndex = 0;
                                            break;
                                        }
                                    case "des":
                                        {
                                            cmbBand.SelectedIndex = 1;
                                            break;
                                        }
                                    case "avg":
                                        {
                                            cmbBand.SelectedIndex = 2;
                                            break;
                                        }
                                }
                            }
                        }
                    }
                }
            }
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
            return null;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog frm = new OpenFileDialog())
            {
                frm.Filter = "极区海冰覆盖度日产品(*.hdf)|*.hdf";
                frm.Multiselect = false;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    txtFileName.Text = frm.FileName;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(_handler!=null)
                _handler(GetArgumentValue());
        }
    }
}
