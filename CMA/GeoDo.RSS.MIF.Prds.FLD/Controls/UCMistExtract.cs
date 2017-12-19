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

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public partial class UCMistExtract : UserControl,IArgumentEditorUI
    {
        private Action<object> _handler;

        public UCMistExtract()
        {
            InitializeComponent();
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            if(this.Tag!=null)
            {
                IArgumentProvider arg = this.Tag as IArgumentProvider;
                ICurrentRasterInteractiver crt = arg.CurrentRasterInteractiver;
                if (crt != null)
                {
                    int bandMI = (int)arg.GetArg("MiddleInfrared");
                    int bandFI = (int)arg.GetArg("FarInfrared");
                    if (bandFI <= 0 || bandMI <= 0)
                        return;
                    crt.StartAOIDrawing(() =>
                        {
                            try
                            {
                                double[] mIBand = crt.GetBandValuesInAOI(bandMI);
                                double[] fIBand = crt.GetBandValuesInAOI(bandFI);
                                if (mIBand == null || fIBand == null || mIBand.Length != fIBand.Length)
                                    return;
                                float avgValue = 0;
                                double totalValue = 0;
                                for (int i = 0; i < mIBand.Length; i++)
                                {
                                    totalValue += mIBand[i] - fIBand[i];
                                }
                                avgValue = (float)Math.Round(totalValue / mIBand.Length, 2);
                                txtValue.Text = avgValue.ToString();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("未能获取薄雾判识所需通道值！", "提示", MessageBoxButtons.OK);
                            }
                        });
                }
            }
            return;
        }

        public object GetArgumentValue()
        {
            return txtValue.Text;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        private void txtValue_TextChanged(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(txtValue.Text);
        }

        public object ParseArgumentValue(XElement ele)
        {
            return null;
        }
    }
}
