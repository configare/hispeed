using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RasterProject;

namespace GeoDo.RSS.MIF.Prds.PrdVal
{
    public partial class UCCLDSatelliteArgs : UserControl,IArgumentEditorUI2
    {
        private Action<object> _handler;
        private bool _IsExcuteArgumentValueChangedEvent = false;
        public UCCLDSatelliteArgs()
        {
            InitializeComponent();
        }
        # region IArgumentEditorUI 成员
        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }
        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            //if (panel == null)
            //    return;
            //UCExtractPanel ucPanel = panel as UCExtractPanel;
            //if (ucPanel == null)
            //    return;
            //IMonitoringSubProduct subProduct = ucPanel.MonitoringSubProduct;
            //if (subProduct == null)
            //    return;
            //IArgumentProvider arp = subProduct.ArgumentProvider;
            //if (arp != null)
            //{
            //    object obj = arp.GetArg("SmartSession");
            //    if (obj != null)
            //        _session = obj as ISmartSession;
            //}
        }
        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return _IsExcuteArgumentValueChangedEvent;
            }
            set
            {

            }
        }
        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return null;
        }
        public object GetArgumentValue()
        {
            ValArguments args = new ValArguments();
            args.FileNamesForVal = new string[] { txtForVal1.Text };
            args.FileNamesToVal = new string[] { txtToVal.Text };
            args.Invalid = txtinvalid.Text.Trim();
            args.MinX = txtMinX.Text;
            args.MaxX = txtMaxX.Text;
            args.MinY = txtMinY.Text;
            args.MaxY = txtMaxY.Text;
            args.CreatScatter = chScatter.Checked;
            args.CreatHistogram = chHistogram.Checked;
            args.MaxColumns = txtColumn.Text;
            args.CreatRMSE = chRmse.Checked;
            return args;
        }
        #endregion

        private void btnOpen1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog frm = new OpenFileDialog())
            {
                if(radfrac.Checked)
                   frm.Filter = "待验证云量数据(*.dat,*.ldf)|*.dat;*.ldf";
                if(radtemp.Checked)
                   frm.Filter = "待验证云顶温度数据(*.dat,*.ldf)|*.dat;*.ldf";
                frm.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    txtToVal.Text = frm.FileName;
                }
            }
        }
        private void btnOpen2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog frm = new OpenFileDialog())
            {
                if (radfrac.Checked)
                   frm.Filter = "MODIS06云量数据(*.dat,*.ldf)|*.dat;*.ldf";
                if(radtemp.Checked)
                    frm.Filter = "MODIS06云顶温度数据(*.dat,*.ldf)|*.dat;*.ldf";
                frm.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    txtForVal1.Text = frm.FileName;
                }
            }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void txtToVal_TextChanged(object sender, EventArgs e)
        {
            IRasterDataProvider toValRaster = GeoDataDriver.Open(txtToVal.Text) as IRasterDataProvider;
            txtMinX.Text = Convert.ToString(toValRaster.CoordEnvelope.MinX);
            txtMaxX.Text = Convert.ToString(toValRaster.CoordEnvelope.MaxX);
            txtMinY.Text = Convert.ToString(toValRaster.CoordEnvelope.MinY);
            txtMaxY.Text = Convert.ToString(toValRaster.CoordEnvelope.MaxY);
        }
        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
