using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.CMA
{
    public partial class frmMonitorShowSetting : Form
    {
        public frmMonitorShowSetting()
        {
            InitializeComponent();
        }

        public MonitorShowSettings Settings
        {
            get
            {
                return CollectSettings();
            }
            set
            {
                FillSettings(value);
            }
        }

        private void FillSettings(MonitorShowSettings value)
        {
            if (value == null)
                return;
            switch (value.OutputRegion)
            {
                case MonitorShowSettings.enumOutputRegion.AllView: rdAllView.Checked = true;
                    break;
                case MonitorShowSettings.enumOutputRegion.CurrentView: rdCurrentRegion.Checked = true;
                    break;
                case MonitorShowSettings.enumOutputRegion.SomeRegion:
                    {
                        rdSetRegion.Checked = true;
                        comboBox1.SelectedIndex = GetRegionIndex(value.regionName);
                    }
                    break;
            }
        }

        private int GetRegionIndex(string regionName)
        {
            switch (regionName)
            {
                case "": return 0;
                case "  ": return 1;
                case "   ": return 2;
                default: return -1;
            }
        }

        private MonitorShowSettings CollectSettings()
        {
            MonitorShowSettings settings = new MonitorShowSettings();
            settings.IsOrigResolution = rdOrigResolution.Checked;
            settings.IsOutputBinImage = ckIsOutputBin.Checked;
            settings.IsOutputVector = ckIsOutputVector.Checked;
            settings.IsOutputGrid = ckIsOutputGrid.Checked;
            if (rdAllView.Checked == true)
                settings.OutputRegion = MonitorShowSettings.enumOutputRegion.AllView;
            else if (rdCurrentRegion.Checked == true)
                settings.OutputRegion = MonitorShowSettings.enumOutputRegion.CurrentView;
            else if(rdSetRegion.Checked==true)
            {
                settings.OutputRegion = MonitorShowSettings.enumOutputRegion.SomeRegion;
                settings.regionName = GetRegionName(comboBox1.SelectedIndex);
            }
            return settings;
        }

        private string GetRegionName(int index)
        {
            switch (index)
            {
                case 0: return null;
                case 1: return null;
                default:
                    return string.Empty;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!ArgIsOK())
                return;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private bool ArgIsOK()
        {
            if (rdSetRegion.Checked)
            {
                int selectIndex = comboBox1.SelectedIndex;
                if (selectIndex==-1)
                {
                    MsgBox.ShowInfo("输出区域没有设置！");
                    comboBox1.Focus();
                    return false;
                }
            }
            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void rdSetRegion_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = rdSetRegion.Checked;
        }
    }
}
