using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.Core.CA;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.CA
{
    [Export(typeof(IRgbProcessorArgEditor))]
    public partial class frmExponentEnhanceII : frmRgbArgsEditor
    {
        private ExponentEnhanceArg _actualArg = null;
        private bool _isTry = true;

        public frmExponentEnhanceII()
        {
            InitializeComponent();
            Load += new EventHandler(frmLogEnhance_Load);
        }

        void frmLogEnhance_Load(object sender, EventArgs e)
        {
            InitMaxMinValue();
            _actualArg = _arg as ExponentEnhanceArg;
            _actualArg.Maxs[0] = (int)dblBarR.MaxValue;
            _actualArg.Mins[0] = (int)dblBarR.MinValue;
            SetToFullChannel(true);
        }

        private void InitMaxMinValue()
        {
            _isTry = false;
            dblBarR.MinValue = 0;
            dblBarG.MinValue = 0;
            dblBarB.MinValue = 0;
            dblBarR.MaxValue = 255;
            dblBarG.MaxValue = 255;
            dblBarB.MaxValue = 255;
            _isTry = true;
        }

        private void SetToFullChannel(bool needUpdateValue)
        {
            labelRed.Visible = false;
            labelGreen.Visible = false;
            labelBlue.Visible = false;
            dblBarG.Visible = dblBarB.Visible = false;
            if (needUpdateValue)
            {
                _actualArg.IsSameArg = true;
                _actualArg.Maxs[0] = (int)dblBarR.MaxValue;
                _actualArg.Mins[0] = (int)dblBarR.MinValue;
            }
            else
            {
                _actualArg.Maxs[0] = 255;
                _actualArg.Mins[0] = 0;
            }
            _actualArg.Maxs[1] = 255;
            _actualArg.Maxs[2] = 255;
            _actualArg.Mins[1] = 0;
            _actualArg.Mins[2] = 0;
        }

        private void SetToSingleChannel(bool needUpdateValue)
        {
            labelRed.Visible = true;
            labelGreen.Visible = true;
            labelBlue.Visible = true;
            dblBarG.Visible = dblBarB.Visible = true;
            if (needUpdateValue)
            {
                _actualArg.IsSameArg = false;
                _actualArg.Maxs[0] = (int)dblBarR.MaxValue;
                _actualArg.Maxs[1] = (int)dblBarG.MaxValue;
                _actualArg.Maxs[2] = (int)dblBarB.MaxValue;

                _actualArg.Mins[0] = (int)dblBarR.MinValue;
                _actualArg.Mins[1] = (int)dblBarG.MinValue;
                _actualArg.Mins[2] = (int)dblBarB.MinValue;
            }
        }

        private bool _raiseCheckedEvent = true;
        protected override void CollectArguments()
        {
            if (rdFullChannel.Checked)
                SetToFullChannel(true);
            else
                SetToSingleChannel(false);
            _raiseCheckedEvent = true;
        }

        private void rdSingleChannel_CheckedChanged(object sender, EventArgs e)
        {
            if (_raiseCheckedEvent)
            {
                if (rdSingleChannel.Checked)
                    SetToSingleChannel(true);
            }
        }

        private void rdFullChannel_CheckedChanged(object sender, EventArgs e)
        {
            if (_raiseCheckedEvent)
            {
                if (rdFullChannel.Checked)
                    SetToFullChannel(true);
            }
        }

        private void dblBarR_BarValueChangedFinished(object sender, double minValue, double maxValue)
        {
            if (!_isTry)
                return;
            if (rdFullChannel.Checked)
                _actualArg.IsSameArg = true;
            else
                _actualArg.IsSameArg = false;
            SetMaxMinValue(dblBarR, (int)maxValue, (int)minValue);
            _actualArg.Maxs[0] = (int)maxValue;
            _actualArg.Mins[0] = (int)minValue;
            TryApply();
        }

        private void dblBarG_BarValueChangedFinished(object sender, double minValue, double maxValue)
        {
            if (!_isTry)
                return;
            _actualArg.IsSameArg = false;
            SetMaxMinValue(dblBarG, (int)maxValue, (int)minValue);
            _actualArg.Maxs[1] = (int)maxValue;
            _actualArg.Mins[1] = (int)minValue;
            TryApply();
        }

        private void dblBarB_BarValueChangedFinished(object sender, double minValue, double maxValue)
        {
            if (!_isTry)
                return;
            _actualArg.IsSameArg = false;
            SetMaxMinValue(dblBarB, (int)maxValue, (int)minValue);
            _actualArg.Maxs[2] = (int)maxValue;
            _actualArg.Mins[2] = (int)minValue;
            TryApply();
        }

        private void SetMaxMinValue(DblBarTrackWithBoxs dblBar, int max, int min)
        {
            _isTry = false;
            dblBar.MaxValue = max;
            dblBar.MinValue = min;
            _isTry = true;
        }

        private void dblBarR_EndValueChanged(object sender, double minValue, double maxValue)
        {
            dblBarR_BarValueChangedFinished(sender, minValue, maxValue);
        }

        private void dblBarG_EndValueChanged(object sender, double minValue, double maxValue)
        {
            dblBarG_BarValueChangedFinished(sender, minValue, maxValue);
        }

        private void dblBarB_EndValueChanged(object sender, double minValue, double maxValue)
        {
            dblBarB_BarValueChangedFinished(sender, minValue, maxValue);
        }

        public override bool IsSupport(System.Type type)
        {
            return type.Equals(typeof(ExponentEnhanceProcessor));
        }
    }
}
