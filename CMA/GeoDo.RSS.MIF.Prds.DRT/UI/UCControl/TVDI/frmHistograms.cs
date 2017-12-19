using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public partial class frmHistograms : Form
    {
        private TVDIUCArgs _ucArgs = null;
        private string _error = string.Empty;
        private DryWetEdgeArgs _args = null;

        public DryWetEdgeArgs DryWetEdgeArgs
        {
            get
            {
                return _args;
            }
        }

        public float AParaMax
        {
            get { return string.IsNullOrEmpty(txtAMax.Text) ? 0 : float.Parse(txtAMax.Text); ;}
        }

        public float BParaMax
        {
            get { return string.IsNullOrEmpty(txtBMax.Text) ? 0 : float.Parse(txtBMax.Text); ;}
        }

        public float AParaMin
        {
            get { return string.IsNullOrEmpty(txtAMin.Text) ? 0 : float.Parse(txtAMin.Text); ;}
        }

        public float BParaMin
        {
            get { return string.IsNullOrEmpty(txtBMin.Text) ? 0 : float.Parse(txtBMin.Text); ;}
        }

        public float NdviMax
        {
            get { return string.IsNullOrEmpty(txtNdviMax.Text) ? 0 : int.Parse(txtNdviMax.Text); ;}
        }

        public float NdviMin
        {
            get { return string.IsNullOrEmpty(txtNdviMin.Text) ? 0 : int.Parse(txtNdviMin.Text); ;}
        }

        public float LstMax
        {
            get { return string.IsNullOrEmpty(txtLstMax.Text) ? 0 : int.Parse(txtLstMax.Text); ;}
        }

        public float LstMin
        {
            get { return string.IsNullOrEmpty(txtLstMin.Text) ? 0 : int.Parse(txtLstMin.Text); ;}
        }

        public frmHistograms()
        {
            InitializeComponent();
            Load += new EventHandler(frmHistograms_Load);
        }

        public frmHistograms(ref TVDIUCArgs ucArgs, ref string error)
        {
            InitializeComponent();
            _ucArgs = ucArgs;
            _error = error;
            Load += new EventHandler(frmHistograms_Load);
        }

        void frmHistograms_Load(object sender, EventArgs e)
        {
            InitfrmHistograms();
        }

        private void InitfrmHistograms()
        {
            if (_ucArgs == null)
                return;
            nudLength.Value = _ucArgs.TVDIParas.Length;
            txtNdviMax.Text = _ucArgs.TVDIParas.NdviFile.Max.ToString();
            txtLstMax.Text = _ucArgs.TVDIParas.LstFile.Max.ToString();
            txtNdviMin.Text = _ucArgs.TVDIParas.NdviFile.Min.ToString();
            txtLstMin.Text = _ucArgs.TVDIParas.LstFile.Min.ToString();
            nudLstPC.Value = (decimal)_ucArgs.TVDIParas.LstFreq;
            nudLimit.Value = (decimal)_ucArgs.TVDIParas.FLimit;
            nudHGYZ.Value = (decimal)_ucArgs.TVDIParas.HGYZ;
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if (_ucArgs == null)
                return;
            _ucArgs.TVDIParas.Length = (int)nudLength.Value;
            int temp = 0;
            int.TryParse(txtNdviMax.Text, out temp);
            _ucArgs.TVDIParas.NdviFile.Max = temp;
            int.TryParse(txtLstMax.Text, out temp);
            _ucArgs.TVDIParas.LstFile.Max = temp;
            int.TryParse(txtNdviMin.Text, out temp);
            _ucArgs.TVDIParas.NdviFile.Min = temp;
            int.TryParse(txtLstMin.Text, out temp);
            _ucArgs.TVDIParas.LstFile.Min = temp;
            DialogResult = DialogResult.OK;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btGetAB_Click(object sender, EventArgs e)
        {
            btOK_Click(sender, e);
            if (!CheckENV())
                return;
            //if (!ElevationCorrections.DoElevationCorrections(_ucArgs, ref _error))
            if (!ElevationCorrectionsII.DoElevationCorrections(_ucArgs, ref _error))
                return;
          // if (!DryWetEdgeFitting.DoDryWetEdgeFitting(out _args, _ucArgs, ref _error))
             if (!DryWetEdgeFittingII.DoDryWetEdgeFitting(out _args, _ucArgs, ref _error))
            {
                txtAMax.Text = "0";
                txtAMin.Text = "0";
                txtBMax.Text = "0";
                txtBMin.Text = "0";
                //MsgBox.ShowInfo("干湿边系数计算失败!");
                return;
            }
            txtAMax.Text = _args.MaxA.ToString();
            txtAMin.Text = _args.MinA.ToString();
            txtBMax.Text = _args.MaxB.ToString();
            txtBMin.Text = _args.MinB.ToString();
        }

        private Int16[] GetInt16(int[] value)
        {
            if (value == null || value.Length == 0)
                return null;
            Int16[] result = new Int16[value.Length];
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int16)value[i];
            return result;
        }

        private bool CheckENV()
        {
            if (nudLength.Value == 0)
            {
                //MsgBox.ShowInfo("请确认步长已经填写正确!");
                return false;
            }

            if (string.IsNullOrEmpty(txtNdviMax.Text) || string.IsNullOrEmpty(txtNdviMin.Text) ||
                string.IsNullOrEmpty(txtLstMax.Text) || string.IsNullOrEmpty(txtLstMin.Text))
            {
                //MsgBox.ShowInfo("请确认NDVI和LST的最大最小值以填写!");
                return false;
            }
            return true;
        }
    }
}
