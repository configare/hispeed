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
    public partial class HistGetGrades : frmRgbArgsEditor
    {
        private HistGetGradesArg _actualArg = null;

        public HistGetGrades()
        {
            InitializeComponent();
            Load += new EventHandler(HistGetGrades_Load);
        }

        private void numericUpDownPixelRL_ValueChanged(object sender, EventArgs e)
        {
            _actualArg.LhRL =(float) numericUpDownPixelRL.Value/100;
            TryApply();
        }
  
        private void numericUpDownRH_ValueChanged(object sender, EventArgs e)
        {
            _actualArg.LhRH = (float)numericUpDownRH.Value/100;
            TryApply();
        }

        private void numericUpDownPixelGL_ValueChanged(object sender, EventArgs e)
        {
            _actualArg.LhGL = (float)numericUpDownPixelGL.Value/100;
            TryApply();
        }

        private void numericUpDownGH_ValueChanged(object sender, EventArgs e)
        {
            _actualArg.LhGH = (float)numericUpDownGH.Value/100;
            TryApply();
        }

        private void numericUpDownPixelBL_ValueChanged(object sender, EventArgs e)
        {
            _actualArg.LhBL = (float)numericUpDownPixelBL.Value/100;
            TryApply();
        }

        private void numericUpDownBH_ValueChanged(object sender, EventArgs e)
        {
            _actualArg.LhBH = (float)numericUpDownBH.Value/100;
            TryApply();
        }

        protected override void CollectArguments()
        {            
        }

        void HistGetGrades_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void Init()
        {
            _actualArg = _arg as HistGetGradesArg;
        }

        public override bool IsSupport(Type type)
        {
            return type.Equals(typeof(HistGetGradesProcessor));
        }
    }
}
