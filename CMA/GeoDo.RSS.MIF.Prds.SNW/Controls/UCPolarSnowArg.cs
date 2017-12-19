using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.SNW
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
                    case 0: bandName = "SD_NorthernDaily_A";
                        break;
                    case 1: bandName = "SD_NorthernDaily_D";
                        break;
                    case 2: bandName = "SWE_NorthernDaily_A";
                        break;
                    case 3: bandName = "SWE_NorthernDaily_D";
                        break;
                }
            }
            else
            {
                switch (cmbBands.SelectedIndex)
                {
                    case 0: bandName = "SD_SouthernDaily_A";
                        break;
                    case 1: bandName = "SD_SouthernDaily_D";
                        break;
                    case 2: bandName = "SWE_SouthernDaily_A";
                        break;
                    case 3: bandName = "SWE_SouthernDaily_D";
                        break;
                }
            }
            return new string[]{txtFileName.Text ,bandName};
        }

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            cmbBands.SelectedIndex = 0;
            cmbRange.SelectedIndex = 0;
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
