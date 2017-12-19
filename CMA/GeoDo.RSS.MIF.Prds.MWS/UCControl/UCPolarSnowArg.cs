using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.UI;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    public partial class UCPolarSnowArg : UserControl,IArgumentEditorUI2
    {
        private bool _isExcuteArgumentValueChanged = false;
        private Action<object> _handler;

        public UCPolarSnowArg()
        {
            InitializeComponent();
        }

        public object GetArgumentValue()
        {
            string bandName="";
            if (cmbRange.SelectedIndex == 0)
            {
                switch (cmbBands.SelectedIndex)
                {
                    case 0: bandName = "datasets=SD_NorthernDaily_A";
                        break;
                    case 1: bandName = "datasets=SD_NorthernDaily_D";
                        break;
                    case 2: bandName = "datasets=SWE_NorthernDaily_A";
                        break;
                    case 3: bandName = "datasets=SWE_NorthernDaily_D";
                        break;
                }
            }
            else
            {
                switch (cmbBands.SelectedIndex)
                {
                    case 0: bandName = "datasets=SD_SouthernDaily_A";
                        break;
                    case 1: bandName = "datasets=SD_SouthernDaily_D";
                        break;
                    case 2: bandName = "datasets=SWE_SouthernDaily_A";
                        break;
                    case 3: bandName = "datasets=SWE_SouthernDaily_D";
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
                    IRasterDataProvider dataPrd = arp.DataProvider;
                    txtFileName.Text = arp.DataProvider.fileName;
                    //set cmbBand and cmbRange
                    if (dataPrd.BandCount >= 1)
                    {
                        for (int i = 1; i <= dataPrd.BandCount; i++)
                        {
                            string datasetStr = dataPrd.GetRasterBand(i).Description;
                            if (!string.IsNullOrEmpty(datasetStr))
                            {
                                string[] stringArray = datasetStr.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                                if (stringArray != null && stringArray.Length == 3)
                                {
                                    if (stringArray[1] == "NorthernDaily")
                                        cmbRange.SelectedIndex = 0;
                                    else
                                        cmbRange.SelectedIndex = 1;
                                    if (stringArray[0] == "SD")
                                    {
                                        if (stringArray[2] == "A")
                                            cmbBands.SelectedIndex = 0;
                                        else
                                            cmbBands.SelectedIndex = 1;
                                    }
                                    else
                                    {
                                        if (stringArray[2] == "A")
                                            cmbBands.SelectedIndex = 2;
                                        else
                                            cmbBands.SelectedIndex = 3;
                                    }
                                    break;
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
                frm.Filter = "雪深雪水当量日产品(*.hdf)|*.hdf";
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
