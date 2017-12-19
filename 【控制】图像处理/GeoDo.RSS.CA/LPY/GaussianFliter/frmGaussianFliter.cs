using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.CA
{
    [Export(typeof(IRgbProcessorArgEditor))]
    public partial class frmGaussianFliter : frmRgbArgsEditor
    {
        private GaussianFliterARG _actualArg = null;
        private bool _isFromTxt = false;
        public frmGaussianFliter()
        {
            InitializeComponent();
            Load += new EventHandler(frmGaussianFliter_Load);
        }

        void frmGaussianFliter_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void Init()
        {
            _actualArg = _arg as GaussianFliterARG;
        }

        private void numericSigma_ValueChanged(object sender, EventArgs e)
        {
            _isFromTxt = true;
            trackSigma.Value = (int)(txtnSigma.Value*10);
            _actualArg.GaussianSigma = txtnSigma.Value;
            TryApply();
        }

        protected override void CollectArguments()
        {
            _actualArg.WndHeight = _actualArg.WndWidth = trackKernel.Value*2+1;
        }

        private void trackSigma_Scroll_1(object sender, EventArgs e)
        {
            if (!_isFromTxt)
                txtnSigma.Value =((decimal ) trackSigma.Value) / 10;
            _isFromTxt = false;
            _actualArg.GaussianSigma = txtnSigma.Value;
            TryApply();
        }

        private void trackKernel_Scroll(object sender, EventArgs e)
        {
            _actualArg.GaussianRadius = trackKernel.Value*2+1;
            txtKernelSize.Text = _actualArg.GaussianRadius.ToString() + "×" + _actualArg.GaussianRadius.ToString();
            TryApply();
        }

        public override bool IsSupport(Type type)
        {
            return type.Equals(typeof(GaussianFliter));
        }
    }
}