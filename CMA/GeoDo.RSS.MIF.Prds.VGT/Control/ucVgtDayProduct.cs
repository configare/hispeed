using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.UI;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public partial class ucVgtDayProduct : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler;
        private IExtractPanel _panel;
        private string[] _aoiTemplate;
        private ucAOITemplate aoiTemplate;

        public ucVgtDayProduct()
        {
            InitializeComponent();
            SetAOIControl();
        }

        void mfs_FileChanged(object obj)
        {
            
        }

        public object GetArgumentValue()
        {
            VgtDayProductArgs vgtarg = new VgtDayProductArgs();
            vgtarg.NDVI = checkBox1.Checked;
            vgtarg.RVI = checkBox2.Checked;
            vgtarg.DVI = checkBox3.Checked;
            vgtarg.EVI = checkBox4.Checked;
            vgtarg.Evi_G = 2.5d;
            vgtarg.Evi_C1 = 6d;
            vgtarg.Evi_C2 = 7.5d;
            vgtarg.Evi_L = 1d;
            vgtarg.AOITemplate = aoiTemplate.Aois;
            return vgtarg;
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return GetArgumentValue();
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            _panel = panel;

            SubProductDef subProductDef = _panel.MonitoringSubProduct.Definition;
            aoiTemplate.BuildControl(subProductDef.ProductDef.AOITemplates,subProductDef.AoiTemplates);
        }

        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return false;
            }
            set
            {
                ;
            }
        }

        private void SetAOIControl()
        {
            //Control ctrl = BuildAOITemplate();
            //ctrl.Dock = DockStyle.Fill;
            //this.groupBox1.Controls.Add(ctrl);
        }

        private Control BuildAOITemplate()
        {
            aoiTemplate = new ucAOITemplate();
            aoiTemplate.Height = 96;
            aoiTemplate.AOIChangedHandler += new OnAOITempleteChangedHandler(aoiTemplate_AOIChangedHandler);

            return aoiTemplate;
        }

        void aoiTemplate_AOIChangedHandler(object sender, string[] aois)
        {
            _aoiTemplate = aois;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgumentValue());
        }
    }

    public class VgtDayProductArgs
    {
        public string[] AOITemplate;

        public bool NDVI;
        public bool RVI;
        public bool DVI;
        public bool EVI;

        public double Evi_G;
        public double Evi_C1;
        public double Evi_C2;
        public double Evi_L;
    }
}
