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

namespace GeoDo.RSS.MIF.Prds.PrdVal
{
    public partial class UCModLSTProcess : UserControl,IArgumentEditorUI2
    {
        private Action<object> _handler;
        private bool _IsExcuteArgumentValueChangedEvent = false;

        public UCModLSTProcess()
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
            if (!string.IsNullOrEmpty(txtinput.Text))
                args.InputDir = txtinput.Text; //= Directory.GetFiles(txtinput.Text, "MOD11A1*.hdf", SearchOption.AllDirectories);
            args.Outdir = txtOutdir.Text;
            args.MinX = txtMinX.Text;
            args.MaxX = txtMaxX.Text;
            args.MinY = txtMinY.Text;
            args.MaxY = txtMaxY.Text;
            args.OutRes = txtRes.Text;
            args.RegionNam = txtRegion.Text;
            return args;
        }
        #endregion
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void grbLatLon_Enter(object sender, EventArgs e)
        {

        }

        private void btnOutDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtOutdir.Text = dlg.SelectedPath;
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            IRasterDataProvider toValRaster = GeoDataDriver.Open(txtToVal.Text) as IRasterDataProvider;
            txtMinX.Text = Convert.ToString(toValRaster.CoordEnvelope.MinX);
            txtMaxX.Text = Convert.ToString(toValRaster.CoordEnvelope.MaxX);
            txtMinY.Text = Convert.ToString(toValRaster.CoordEnvelope.MinY);
            txtMaxY.Text = Convert.ToString(toValRaster.CoordEnvelope.MaxY);
            txtRes.Text = Convert.ToString(toValRaster.ResolutionX);
            toValRaster.Dispose();
        }

        private void rdioData_CheckedChanged(object sender, EventArgs e)
        {
            if (rdioData.Checked)
            {
                txtToVal.Enabled = true;
                btnOpenData.Enabled = true;
            }
            else
            {
                txtToVal.Enabled = false;
                btnOpenData.Enabled = false;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using(FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if(dlg.ShowDialog()== DialogResult.OK)
                {
                    txtinput.Text = dlg.SelectedPath;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void btnOpenData_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog frm = new OpenFileDialog())
            {
                frm.Filter = "待验证LST数据文件(*.dat,*.ldf)|*.dat;*.ldf";
                frm.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    txtToVal.Text = frm.FileName;
                }
            }
        }

        private void txtOutdir_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
