using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.UI;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public partial class UCFirALevel : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler;
        private bool _isExcuteArgumentChanged = false;
        private IArgumentProvider _argProvider = null;

        public UCFirALevel()
        {
            InitializeComponent();
        }

        public object GetArgumentValue()
        {
            return GetParameterValues();
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        private string[] GetParameterValues()
        {
            List<string> rets = new List<string>();
            rets.Add(txtMaxNear.Text);
            rets.Add(txtMinNear.Text);
            return rets.ToArray();
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return null;
        }

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            if (panel == null)
                return;
            UCExtractPanel ucPanel = panel as UCExtractPanel;
            if (ucPanel == null)
                return;
            IMonitoringSubProduct subProduct = ucPanel.MonitoringSubProduct;
            if (subProduct == null)
                return;
            _argProvider = subProduct.ArgumentProvider;
        }

        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return _isExcuteArgumentChanged;
            }
            set
            {
                _isExcuteArgumentChanged = false;
            }
        }

        private void btnMinNear_Click(object sender, EventArgs e)
        {
            GetCurrentRasterInteractiver(txtMinNear, "percectMin", false);
        }

        private void btnMaxNear_Click(object sender, EventArgs e)
        {
            GetCurrentRasterInteractiver(txtMaxNear, "percectMax", true);
        }

        private void GetCurrentRasterInteractiver(TextBox txt, string percentArgName, bool maxValue)
        {
            if (_argProvider != null)
            {
                ICurrentRasterInteractiver crt = _argProvider.CurrentRasterInteractiver;
                if (crt != null)
                {
                    int bandNI = (int)_argProvider.GetArg("NearInfrared");
                    if (bandNI <= 0)
                        return;
                    float percent = (float)_argProvider.GetArg(percentArgName) / 100;
                    if (maxValue)
                    {
                        crt.StartAOIDrawing(() =>
                            {
                                txt.Text = Math.Round(crt.GetMaxAvgBandValueInAOI(bandNI, percent), 2).ToString();
                                if (_handler != null)
                                    _handler(GetParameterValues());
                                crt.TryFinishPencilTool();
                            });
                    }
                    else
                    {
                        crt.StartAOIDrawing(() =>
                        {
                            txt.Text = Math.Round(crt.GetMinAvgBandValueInAOI(bandNI, percent), 2).ToString();
                            if (_handler != null)
                                _handler(GetParameterValues());
                            crt.TryFinishPencilTool();
                        });
                    }
                }
            }
        }

    }
}
