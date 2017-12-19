using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public partial class UCTVDIPanel : UserControl, IArgumentEditorUI
    {
        private string _ndviFile = null;
        private string _lstFile = null;
        private string _demFile = null;
        private string _fliter = "(*.dat,*.ldf)|*.dat;*.ldf|所有文件(*.*)|*.*";
        private Action<object> _handler = null;
        private TVDIUCArgs _ucArgs = null;
        private TVDIParaClass _cur = null;
        private DryWetEdgeArgs _dryWetEdgesArgs = null;
        private string _error = string.Empty;

        public UCTVDIPanel()
        {
            InitializeComponent();
            InitControls();
            Load += new EventHandler(UCTVDIPanel_Load);
        }

        private void InitControls()
        {
            ndviFileSelect.lbFile.Text = "植被指数";
            ndviFileSelect.OpenFile = new UCFileSelectBase.OpenFileHandler(NdviOpenFile_Click);
            LstFileSelect.lbFile.Text = "陆表高温";
            LstFileSelect.OpenFile = new UCFileSelectBase.OpenFileHandler(LstOpenFile_Click);
            demFileSelect.lbFile.Text = "高程数据";
            demFileSelect.OpenFile = new UCFileSelectBase.OpenFileHandler(DemOpenFile_Click);
            ucExpCoefficientBase1.SetExpType("TVDI");
            ucExpCoefficientBase1.BtnEdit_Click = new UCExpCoefficientBase.BtnEditClickHandle(BtnEdit_Click);
        }

        void UCTVDIPanel_Load(object sender, EventArgs e)
        {
            _cur = ReadExpCofficientFile.LoadTVDIParasFile(ArgsFileProvider.TVDIArgsFile);
            if (_cur == null)
                _cur = new TVDIParaClass();
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        public void NdviOpenFile_Click(ComboBox cmbFile)
        {
            OpenFile("植被指数" + _fliter, cmbFile, ref _ndviFile);
            ndviFileSelect.Tag = _ndviFile;
        }

        public void LstOpenFile_Click(ComboBox cmbFile)
        {
            OpenFile("陆表高温" + _fliter, cmbFile, ref  _lstFile);
            ndviFileSelect.Tag = _lstFile;
        }

        public void DemOpenFile_Click(ComboBox cmbFile)
        {
            OpenFile("高程数据" + _fliter, cmbFile, ref _demFile);
            ndviFileSelect.Tag = _demFile;
        }

        private void OpenFile(string filter, ComboBox cmbFile, ref string filename)
        {
            using (OpenFileDialog open = new OpenFileDialog())
            {
                open.Filter = filter;
                open.Multiselect = false;
                if (open.ShowDialog() == DialogResult.OK)
                {
                    cmbFile.Text = Path.GetFileName(open.FileName);
                    filename = open.FileName;
                }
            }
        }

        private void btNH_Click(object sender, EventArgs e)
        {
            GetUCArgs();
            using (frmHistograms frm = new frmHistograms(ref _ucArgs, ref _error))
            {
                DialogResult dr = frm.ShowDialog();
                _dryWetEdgesArgs = frm.DryWetEdgeArgs;
                SetDryWetEdgesArgs();
            }
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void SetDryWetEdgesArgs()
        {
            if (_dryWetEdgesArgs == null)
                return;
            txtMaxA.Text = _dryWetEdgesArgs.MaxA.ToString();
            txtMinA.Text = _dryWetEdgesArgs.MinA.ToString();
            txtMaxB.Text = _dryWetEdgesArgs.MaxB.ToString();
            txtMinB.Text = _dryWetEdgesArgs.MinB.ToString();
        }

        public void BtnEdit_Click(object curArg)
        {
            _cur = curArg as TVDIParaClass;
        }

        public object GetArgumentValue()
        {
            GetUCArgs();
            return _ucArgs;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        private TVDIUCArgs GetUCArgs()
        {
            if (_ucArgs == null)
                _ucArgs = new TVDIUCArgs();
            _ucArgs.TVDIParas = _cur;
            _ucArgs.NDVIFile = _ndviFile;
            _ucArgs.LSTFile = _lstFile;
            _ucArgs.DEMFile = _demFile;
            _ucArgs.DryWetEdgesFitting = _dryWetEdgesArgs;
            return _ucArgs;
        }



        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return null;
        }
    }
}
