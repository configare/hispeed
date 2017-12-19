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
    public partial class UCComputeMixPixel : UserControl,IArgumentEditorUI
    {
        private Action<object> _handler;

        public UCComputeMixPixel()
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

        private void btnFld_Click(object sender, EventArgs e)
        {
            if (this.Tag != null)
            {
                IArgumentProvider arg = this.Tag as IArgumentProvider;
                ICurrentRasterInteractiver crt = arg.CurrentRasterInteractiver;
                if (crt != null)
                {
                    int bandNI = (int)arg.GetArg("NearInfrared");
                    if (bandNI <= 0)
                        return;
                    crt.StartAOIDrawing(() => 
                    {
                        float fldAvgValue = (float)Math.Round(crt.GetAvgBandValueInAOI(bandNI), 2);
                        txtFld.Text = fldAvgValue.ToString();
                    });
                    //float fldAvgValue = (float)Math.Round(crt.GetAvgBandValueInAOI(bandNI),2);
                    //txtFld.Text = fldAvgValue.ToString();
                }
            }
        }

        private void btnLand_Click(object sender, EventArgs e)
        {
            if (this.Tag != null)
            {
                IArgumentProvider arg = this.Tag as IArgumentProvider;
                ICurrentRasterInteractiver crt = arg.CurrentRasterInteractiver;
                if (crt != null)
                {
                    int bandNI = (int)arg.GetArg("NearInfrared");
                    if (bandNI <= 0)
                        return;
                    crt.StartAOIDrawing(() => 
                    {
                        float landAvgValue = (float)Math.Round(crt.GetAvgBandValueInAOI(bandNI),2);
                        txtLand.Text = landAvgValue.ToString();
                    });
                }
            }
        }

        private string[] GetParameterValues()
        {
            List<string> rets = new List<string>();
            rets.Add(txtFld.Text);
            rets.Add(txtLand.Text);
            return rets.ToArray();
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            //XElement node = ele.Element("ValueItem");
            //string[] values = new string[2];
            //if (node != null)
            //{
            //    values[0] = node.Attribute("fldvalue").Value;
            //    values[1] = node.Attribute("landvalue").Value;
            //    return values;
            //}
            return null;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetParameterValues());
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtFld.Text = null;
            txtLand.Text = null;
        }

    }
}
