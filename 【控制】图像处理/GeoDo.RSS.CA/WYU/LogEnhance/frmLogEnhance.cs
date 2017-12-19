using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.CA
{
    [Export(typeof(IRgbProcessorArgEditor))]
    public partial class frmLogEnhance : frmRgbArgsEditor
    {
        private LogEnhanceArg _actualArg = null;

        public frmLogEnhance()
        {
            InitializeComponent();
            Load += new EventHandler(frmLogEnhance_Load);
        }

        void frmLogEnhance_Load(object sender, EventArgs e)
        {
            _actualArg = _arg as LogEnhanceArg;
            txtBaseRed.SelectedIndex = txtBaseGreen.SelectedIndex = txtBaseBlue.SelectedIndex = 2;
            txtScaleRed.Value = txtScaleGreen.Value = txtScaleBlue.Value = _actualArg.Scales[0];
            SetToFullChannel();
        }

        private double GetLogBase(ComboBox box)
        {
            string v = box.Text;
            if (!string.IsNullOrEmpty(v))
            {
                if (v.ToUpper() == "E")
                    v = Math.E.ToString();
                return double.Parse(v);
            }
            return 1;
        }

        private void SetToFullChannel()
        {
            labelRed.Visible = false;
            labelGreen.Visible = false;
            labelBlue.Visible = false;
            txtBaseGreen.Visible = txtBaseBlue.Visible = false;
            txtScaleGreen.Visible = txtScaleBlue.Visible = false;
            labelBaseBlue.Visible = labelBaseRed.Visible = false;
            labelScaleGreen.Visible = labelScaleBlue.Visible = false;
            _actualArg.Scales[0] = (int)txtScaleRed.Value;
            _actualArg.Scales[1] = 0;
            _actualArg.Scales[2] = 0;
            _actualArg.Scales[3] = 0;
            _actualArg.LogBases[0] = GetLogBase(txtBaseRed);
            _actualArg.LogBases[1] = 0;
            _actualArg.LogBases[2] = 0;
            _actualArg.LogBases[3] = 0;
        }

        private void SetToSingleChannel()
        {
            labelRed.Visible = true;
            labelGreen.Visible = true;
            labelBlue.Visible = true;
            txtBaseGreen.Visible = txtBaseBlue.Visible = true;
            txtScaleGreen.Visible = txtScaleBlue.Visible = true;
            labelBaseBlue.Visible = labelBaseRed.Visible = true;
            labelScaleGreen.Visible = labelScaleBlue.Visible = true;
            _actualArg.Scales[0] = 0;
            _actualArg.Scales[1] = (int)txtScaleRed.Value;
            _actualArg.Scales[2] = (int)txtScaleGreen.Value;
            _actualArg.Scales[3] = (int)txtScaleBlue.Value;
            _actualArg.LogBases[0] = 0;
            _actualArg.LogBases[1] = GetLogBase(txtBaseRed);
            _actualArg.LogBases[2] = GetLogBase(txtBaseGreen);
            _actualArg.LogBases[3] = GetLogBase(txtBaseBlue);
        }

        private bool _raiseCheckedEvent = true;
        protected override void CollectArguments()
        {
            _actualArg.IsSameArg = rdFullChannel.Checked;
            if (rdFullChannel.Checked)
                SetToFullChannel();
            else
            {
                rdSingleChannel.Checked = true;
                SetToSingleChannel();
            }
        }

        public override bool IsSupport(System.Type type)
        {
            return type.Equals(typeof(RgbProcessorLogEnhance));
        }

        private void rdSingleChannel_CheckedChanged(object sender, EventArgs e)
        {
            if (_raiseCheckedEvent)
            {
                if (rdSingleChannel.Checked)
                    SetToSingleChannel();
            }
        }

        private void rdFullChannel_CheckedChanged(object sender, EventArgs e)
        {
            if (_raiseCheckedEvent)
            {
                if (rdFullChannel.Checked)
                    SetToFullChannel();
            }
        }

        private void txtBaseRed_TextChanged(object sender, EventArgs e)
        {
            string v = (sender as ComboBox).Text;
            if (string.IsNullOrEmpty(v))
                return;
            int idx = rdFullChannel.Checked ? 0 : 1;
            if (v.ToUpper() == "E")
                _actualArg.LogBases[idx] = Math.E;
            else
                _actualArg.LogBases[idx] = double.Parse(v);
            TryApply();
        }

        private void txtBaseGreen_TextUpdate(object sender, EventArgs e)
        {
            string v = (sender as ComboBox).Text;
            if (string.IsNullOrEmpty(v))
                return;
            _actualArg.LogBases[2] = double.Parse(v);
            TryApply();
        }

        private void txtBaseBlue_TextUpdate(object sender, EventArgs e)
        {
            string v = (sender as ComboBox).Text;
            if (string.IsNullOrEmpty(v))
                return;
            _actualArg.LogBases[3] = double.Parse(v);
            TryApply();
        }

        private void txtScaleRed_ValueChanged(object sender, EventArgs e)
        {
            if (rdFullChannel.Checked)
                _actualArg.Scales[0] = (int)txtScaleRed.Value;
            else
                _actualArg.Scales[1] = (int)txtScaleRed.Value;
            TryApply();
        }

        private void txtScaleGreen_ValueChanged(object sender, EventArgs e)
        {
            _actualArg.Scales[2] = (int)txtScaleGreen.Value;
            TryApply();
        }

        private void txtScaleBlue_ValueChanged(object sender, EventArgs e)
        {
            _actualArg.Scales[3] = (int)txtScaleBlue.Value;
            TryApply();
        }
    }
}
