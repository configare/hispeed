using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.UI;


namespace GeoDo.RSS.MIF.Prds.DST
{
    public partial class UCDstFY3Extract : UserControl,IArgumentEditorUI2
    {
        private int _extractWindowID = 9019;
        private int _extractWindowWidth = 480;
        private Action<object> _handler;
        private bool _isValueChangedEvent = false;
        private DstFY3ExtractArgSet _extractArg = null;

        public UCDstFY3Extract()
        {
            InitializeComponent();
        }

        public object GetArgumentValue()
        {
            float value;
            if(float.TryParse(txtTBB11BValue1.Text,out value))
               _extractArg.BareLandArg.TBB11[0]= value;
            if (float.TryParse(txtTBB11BValue2.Text, out value))
                _extractArg.BareLandArg.TBB11[1] = value;
            if (float.TryParse(txtTBB11BValue3.Text, out value))
                _extractArg.BareLandArg.TBB11[2] = value;
            if (float.TryParse(txtTBB11BValue4.Text, out value))
                _extractArg.BareLandArg.TBB11[3] = value;
            if (float.TryParse(txtTBB11VValue1.Text, out value))
                _extractArg.VegetationArg.TBB11[0] = value;
            if (float.TryParse(txtTBB11VValue2.Text, out value))
                _extractArg.VegetationArg.TBB11[1] = value;
            if (float.TryParse(txtTBB11VValue3.Text, out value))
                _extractArg.VegetationArg.TBB11[2] = value;
            if (float.TryParse(txtTBB11VValue4.Text, out value))
                _extractArg.VegetationArg.TBB11[3] = value;
            if (float.TryParse(txtTBB11WValue1.Text, out value))
                _extractArg.WaterArg.TBB11[0] = value;
            if (float.TryParse(txtTBB11WValue2.Text, out value))
                _extractArg.WaterArg.TBB11[1] = value;
            if (float.TryParse(txtTBB11WValue3.Text, out value))
                _extractArg.WaterArg.TBB11[2] = value;
            if (float.TryParse(txtTBB11WValue4.Text, out value))
                _extractArg.WaterArg.TBB11[3] = value;
            if (float.TryParse(txtRef650BValue1.Text, out value))
                _extractArg.BareLandArg.Ref650[0] = value;
            if (float.TryParse(txtRef650BValue2.Text, out value))
                _extractArg.BareLandArg.Ref650[1] = value;
            if (float.TryParse(txtRef650BValue3.Text, out value))
                _extractArg.BareLandArg.Ref650[2] = value;
            if (float.TryParse(txtRef650BValue4.Text, out value))
                _extractArg.BareLandArg.Ref650[3] = value;
            if (float.TryParse(txtRef650VValue1.Text, out value))
                _extractArg.VegetationArg.Ref650[0] = value;
            if (float.TryParse(txtRef650VValue2.Text, out value))
                _extractArg.VegetationArg.Ref650[1] = value;
            if (float.TryParse(txtRef650VValue3.Text, out value))
                _extractArg.VegetationArg.Ref650[2] = value;
            if (float.TryParse(txtRef650VValue4.Text, out value))
                _extractArg.VegetationArg.Ref650[3] = value;
            if (float.TryParse(txtRef650WValue1.Text, out value))
                _extractArg.WaterArg.Ref650[0] = value;
            if (float.TryParse(txtRef650WValue2.Text, out value))
                _extractArg.WaterArg.Ref650[1] = value;
            if (float.TryParse(txtRef650WValue3.Text, out value))
                _extractArg.WaterArg.Ref650[2] = value;
            if (float.TryParse(txtRef650WValue4.Text, out value))
                _extractArg.WaterArg.Ref650[3] = value;
            if (float.TryParse(txtTBB37BValue1.Text, out value))
                _extractArg.BareLandArg.TBB37[0] = value;
            if (float.TryParse(txtTBB37BValue2.Text, out value))
                _extractArg.BareLandArg.TBB37[1] = value;
            if (float.TryParse(txtTBB37BValue3.Text, out value))
                _extractArg.BareLandArg.TBB37[2] = value;
            if (float.TryParse(txtTBB37BValue4.Text, out value))
                _extractArg.BareLandArg.TBB37[3] = value;
            if (float.TryParse(txtTBB37VValue1.Text, out value))
                _extractArg.VegetationArg.TBB37[0] = value;
            if (float.TryParse(txtTBB37VValue2.Text, out value))
                _extractArg.VegetationArg.TBB37[1] = value;
            if (float.TryParse(txtTBB37VValue3.Text, out value))
                _extractArg.VegetationArg.TBB37[2] = value;
            if (float.TryParse(txtTBB37VValue4.Text, out value))
                _extractArg.VegetationArg.TBB37[3] = value;
            if (float.TryParse(txtTBB37WValue1.Text, out value))
                _extractArg.WaterArg.TBB37[0] = value;
            if (float.TryParse(txtTBB37WValue2.Text, out value))
                _extractArg.WaterArg.TBB37[1] = value;
            if (float.TryParse(txtTBB37WValue3.Text, out value))
                _extractArg.WaterArg.TBB37[2] = value;
            if (float.TryParse(txtTBB37WValue4.Text, out value))
                _extractArg.WaterArg.TBB37[3] = value;
            if (float.TryParse(txtBTD11_12BValue1.Text, out value))
                _extractArg.BareLandArg.BTD11_12[0] = value;
            if (float.TryParse(txtBTD11_12BValue2.Text, out value))
                _extractArg.BareLandArg.BTD11_12[1] = value;
            if (float.TryParse(txtBTD11_12BValue3.Text, out value))
                _extractArg.BareLandArg.BTD11_12[2] = value;
            if (float.TryParse(txtBTD11_12BValue4.Text, out value))
                _extractArg.BareLandArg.BTD11_12[3] = value;
            if (float.TryParse(txtBTD11_12VValue1.Text, out value))
                _extractArg.VegetationArg.BTD11_12[0] = value;
            if (float.TryParse(txtBTD11_12VValue2.Text, out value))
                _extractArg.VegetationArg.BTD11_12[1] = value;
            if (float.TryParse(txtBTD11_12VValue3.Text, out value))
                _extractArg.VegetationArg.BTD11_12[2] = value;
            if (float.TryParse(txtBTD11_12VValue4.Text, out value))
                _extractArg.VegetationArg.BTD11_12[3] = value;
            if (float.TryParse(txtBTD11_12WValue1.Text, out value))
                _extractArg.WaterArg.BTD11_37[0] = value;
            if (float.TryParse(txtBTD11_12WValue2.Text, out value))
                _extractArg.WaterArg.BTD11_37[1] = value;
            if (float.TryParse(txtBTD11_12WValue3.Text, out value))
                _extractArg.WaterArg.BTD11_37[2] = value;
            if (float.TryParse(txtBTD11_12WValue4.Text, out value))
                _extractArg.WaterArg.BTD11_37[3] = value;
            if (float.TryParse(txtBTD12_37BValue1.Text, out value))
                _extractArg.BareLandArg.BTD11_37[0] = value;
            if (float.TryParse(txtBTD12_37BValue2.Text, out value))
                _extractArg.BareLandArg.BTD11_37[1] = value;
            if (float.TryParse(txtBTD12_37BValue3.Text, out value))
                _extractArg.BareLandArg.BTD11_37[2] = value;
            if (float.TryParse(txtBTD12_37BValue4.Text, out value))
                _extractArg.BareLandArg.BTD11_37[3] = value;
            if (float.TryParse(txtBTD12_37VValue1.Text, out value))
                _extractArg.VegetationArg.BTD11_37[0] = value;
            if (float.TryParse(txtBTD12_37VValue2.Text, out value))
                _extractArg.VegetationArg.BTD11_37[1] = value;
            if (float.TryParse(txtBTD12_37VValue3.Text, out value))
                _extractArg.VegetationArg.BTD11_37[2] = value;
            if (float.TryParse(txtBTD12_37VValue4.Text, out value))
                _extractArg.VegetationArg.BTD11_37[3] = value;
            if (float.TryParse(txtBTD12_37WValue1.Text, out value))
                _extractArg.WaterArg.BTD11_37[0] = value;
            if (float.TryParse(txtBTD12_37WValue2.Text, out value))
                _extractArg.WaterArg.BTD11_37[1] = value;
            if (float.TryParse(txtBTD12_37WValue3.Text, out value))
                _extractArg.WaterArg.BTD11_37[2] = value;
            if (float.TryParse(txtBTD12_37WValue4.Text, out value))
                _extractArg.WaterArg.BTD11_37[3] = value;
            if (float.TryParse(txtR47_64BValue1.Text, out value))
                _extractArg.BareLandArg.R47_64[0] = value;
            if (float.TryParse(txtR47_64BValue2.Text, out value))
                _extractArg.BareLandArg.R47_64[1] = value;
            if (float.TryParse(txtR47_64BValue3.Text, out value))
                _extractArg.BareLandArg.R47_64[2] = value;
            if (float.TryParse(txtR47_64BValue4.Text, out value))
                _extractArg.BareLandArg.R47_64[3] = value;
            if (float.TryParse(txtR47_64VValue1.Text, out value))
                _extractArg.VegetationArg.R47_64[0] = value;
            if (float.TryParse(txtR47_64VValue2.Text, out value))
                _extractArg.VegetationArg.R47_64[1] = value;
            if (float.TryParse(txtR47_64VValue3.Text, out value))
                _extractArg.VegetationArg.R47_64[2] = value;
            if (float.TryParse(txtR47_64VValue4.Text, out value))
                _extractArg.VegetationArg.R47_64[3] = value;
            if (float.TryParse(txtR47_64WValue1.Text, out value))
                _extractArg.WaterArg.R47_64[0] = value;
            if (float.TryParse(txtR47_64WValue2.Text, out value))
                _extractArg.WaterArg.R47_64[1] = value;
            if (float.TryParse(txtR47_64WValue3.Text, out value))
                _extractArg.WaterArg.R47_64[2] = value;
            if (float.TryParse(txtR47_64WValue4.Text, out value))
                _extractArg.WaterArg.R47_64[3] = value;
            if (float.TryParse(txtNDVIBValue1.Text, out value))
                _extractArg.BareLandArg.NDVI[0] = value;
            if (float.TryParse(txtNDVIBValue2.Text, out value))
                _extractArg.BareLandArg.NDVI[1] = value;
            if (float.TryParse(txtNDVIBValue3.Text, out value))
                _extractArg.BareLandArg.NDVI[2] = value;
            if (float.TryParse(txtNDVIBValue4.Text, out value))
                _extractArg.BareLandArg.NDVI[3] = value;
            if (float.TryParse(txtNDVIVValue1.Text, out value))
                _extractArg.VegetationArg.NDVI[0] = value;
            if (float.TryParse(txtNDVIVValue2.Text, out value))
                _extractArg.VegetationArg.NDVI[1] = value;
            if (float.TryParse(txtNDVIVValue3.Text, out value))
                _extractArg.VegetationArg.NDVI[2] = value;
            if (float.TryParse(txtNDVIVValue4.Text, out value))
                _extractArg.VegetationArg.NDVI[3] = value;
            if (float.TryParse(txtNDVIWValue1.Text, out value))
                _extractArg.WaterArg.NDVI[0] = value;
            if (float.TryParse(txtNDVIWValue2.Text, out value))
                _extractArg.WaterArg.NDVI[1] = value;
            if (float.TryParse(txtNDVIWValue3.Text, out value))
                _extractArg.WaterArg.NDVI[2] = value;
            if (float.TryParse(txtNDVIWValue4.Text, out value))
                _extractArg.WaterArg.NDVI[3] = value;
            if (float.TryParse(txtNDSIBValue1.Text, out value))
                _extractArg.BareLandArg.NDSI[0] = value;
            if (float.TryParse(txtNDSIBValue2.Text, out value))
                _extractArg.BareLandArg.NDSI[1] = value;
            if (float.TryParse(txtNDSIBValue3.Text, out value))
                _extractArg.BareLandArg.NDSI[2] = value;
            if (float.TryParse(txtNDSIBValue4.Text, out value))
                _extractArg.BareLandArg.NDSI[3] = value;
            if (float.TryParse(txtNDSIVValue1.Text, out value))
                _extractArg.VegetationArg.NDSI[0] = value;
            if (float.TryParse(txtNDSIVValue2.Text, out value))
                _extractArg.VegetationArg.NDSI[1] = value;
            if (float.TryParse(txtNDSIVValue3.Text, out value))
                _extractArg.VegetationArg.NDSI[2] = value;
            if (float.TryParse(txtNDSIVValue4.Text, out value))
                _extractArg.VegetationArg.NDSI[3] = value;
            if (float.TryParse(txtNDSIWValue1.Text, out value))
                _extractArg.WaterArg.NDSI[0] = value;
            if (float.TryParse(txtNDSIWValue2.Text, out value))
                _extractArg.WaterArg.NDSI[1] = value;
            if (float.TryParse(txtNDSIWValue3.Text, out value))
                _extractArg.WaterArg.NDSI[2] = value;
            if (float.TryParse(txtNDSIWValue4.Text, out value))
                _extractArg.WaterArg.NDSI[3] = value;
            if (float.TryParse(txtNDDIBValue1.Text, out value))
                _extractArg.BareLandArg.NDDI[0] = value;
            if (float.TryParse(txtNDDIBValue2.Text, out value))
                _extractArg.BareLandArg.NDDI[1] = value;
            if (float.TryParse(txtNDDIBValue3.Text, out value))
                _extractArg.BareLandArg.NDDI[2] = value;
            if (float.TryParse(txtNDDIBValue4.Text, out value))
                _extractArg.BareLandArg.NDDI[3] = value;
            if (float.TryParse(txtNDDIVValue1.Text, out value))
                _extractArg.VegetationArg.NDDI[0] = value;
            if (float.TryParse(txtNDDIVValue2.Text, out value))
                _extractArg.VegetationArg.NDDI[1] = value;
            if (float.TryParse(txtNDDIVValue3.Text, out value))
                _extractArg.VegetationArg.NDDI[2] = value;
            if (float.TryParse(txtNDDIVValue4.Text, out value))
                _extractArg.VegetationArg.NDDI[3] = value;
            if (float.TryParse(txtNDDIWValue1.Text, out value))
                _extractArg.WaterArg.NDDI[0] = value;
            if (float.TryParse(txtNDDIWValue2.Text, out value))
                _extractArg.WaterArg.NDDI[1] = value;
            if (float.TryParse(txtNDDIWValue3.Text, out value))
                _extractArg.WaterArg.NDDI[2] = value;
            if (float.TryParse(txtNDDIWValue4.Text, out value))
                _extractArg.WaterArg.NDDI[3] = value;
            if (float.TryParse(txtRef470BValue1.Text, out value))
                _extractArg.BareLandArg.Ref470[0] = value;
            if (float.TryParse(txtRef470BValue2.Text, out value))
                _extractArg.BareLandArg.Ref470[1] = value;
            if (float.TryParse(txtRef470BValue3.Text, out value))
                _extractArg.BareLandArg.Ref470[2] = value;
            if (float.TryParse(txtRef470BValue4.Text, out value))
                _extractArg.BareLandArg.Ref470[3] = value;
            if (float.TryParse(txtRef470VValue1.Text, out value))
                _extractArg.VegetationArg.Ref470[0] = value;
            if (float.TryParse(txtRef470VValue2.Text, out value))
                _extractArg.VegetationArg.Ref470[1] = value;
            if (float.TryParse(txtRef470VValue3.Text, out value))
                _extractArg.VegetationArg.Ref470[2] = value;
            if (float.TryParse(txtRef470VValue4.Text, out value))
                _extractArg.VegetationArg.Ref470[3] = value;
            if (float.TryParse(txtRef470WValue1.Text, out value))
                _extractArg.WaterArg.Ref470[0] = value;
            if (float.TryParse(txtRef470WValue2.Text, out value))
                _extractArg.WaterArg.Ref470[1] = value;
            if (float.TryParse(txtRef470WValue3.Text, out value))
                _extractArg.WaterArg.Ref470[2] = value;
            if (float.TryParse(txtRef470WValue4.Text, out value))
                _extractArg.WaterArg.Ref470[3] = value;
            if (float.TryParse(txtIDSIBValue1.Text, out value))
                _extractArg.BareLandArg.IDSI[0] = value;
            if (float.TryParse(txtIDSIBValue2.Text, out value))
                _extractArg.BareLandArg.IDSI[1] = value;
            if (float.TryParse(txtIDSIBValue3.Text, out value))
                _extractArg.BareLandArg.IDSI[2] = value;
            if (float.TryParse(txtIDSIBValue4.Text, out value))
                _extractArg.BareLandArg.IDSI[3] = value;
            if (float.TryParse(txtIDSIVValue1.Text, out value))
                _extractArg.VegetationArg.IDSI[0] = value;
            if (float.TryParse(txtIDSIVValue2.Text, out value))
                _extractArg.VegetationArg.IDSI[1] = value;
            if (float.TryParse(txtIDSIVValue3.Text, out value))
                _extractArg.VegetationArg.IDSI[2] = value;
            if (float.TryParse(txtIDSIVValue4.Text, out value))
                _extractArg.VegetationArg.IDSI[3] = value;
            if (float.TryParse(txtIDSIWValue1.Text, out value))
                _extractArg.WaterArg.IDSI[0] = value;
            if (float.TryParse(txtIDSIWValue2.Text, out value))
                _extractArg.WaterArg.IDSI[1] = value;
            if (float.TryParse(txtIDSIWValue3.Text, out value))
                _extractArg.WaterArg.IDSI[2] = value;
            if (float.TryParse(txtIDSIWValue4.Text, out value))
                _extractArg.WaterArg.IDSI[3] = value;
            if (float.TryParse(txtRef1380BValue1.Text, out value))
                _extractArg.BareLandArg.Ref1380[0] = value;
            if (float.TryParse(txtRef1380BValue2.Text, out value))
                _extractArg.BareLandArg.Ref1380[1] = value;
            if (float.TryParse(txtRef1380BValue3.Text, out value))
                _extractArg.BareLandArg.Ref1380[2] = value;
            if (float.TryParse(txtRef1380BValue4.Text, out value))
                _extractArg.BareLandArg.Ref1380[3] = value;
            if (float.TryParse(txtRef1380VValue1.Text, out value))
                _extractArg.VegetationArg.Ref1380[0] = value;
            if (float.TryParse(txtRef1380VValue2.Text, out value))
                _extractArg.VegetationArg.Ref1380[1] = value;
            if (float.TryParse(txtRef1380VValue3.Text, out value))
                _extractArg.VegetationArg.Ref1380[2] = value;
            if (float.TryParse(txtRef1380VValue4.Text, out value))
                _extractArg.VegetationArg.Ref1380[3] = value;
            if (float.TryParse(txtRef1380WValue1.Text, out value))
                _extractArg.WaterArg.Ref1380[0] = value;
            if (float.TryParse(txtRef1380WValue2.Text, out value))
                _extractArg.WaterArg.Ref1380[1] = value;
            if (float.TryParse(txtRef1380WValue3.Text, out value))
                _extractArg.WaterArg.Ref1380[2] = value;
            if (float.TryParse(txtRef1380WValue4.Text, out value))
                _extractArg.WaterArg.Ref1380[3] = value;
            _extractArg.isSmooth = chbSmooth.Checked;
            return _extractArg;
        }

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            //调整监测分析面板窗体宽度，以显示完整整个控件
            if (panel == null)
                return;
            UCExtractPanel ucPanel = panel as UCExtractPanel;
            if (ucPanel == null)
                return;
            IMonitoringSubProduct subProduct = ucPanel.MonitoringSubProduct;
            if (subProduct == null)
                return;
            IArgumentProvider arp = subProduct.ArgumentProvider;
            if (arp != null)
            {
                object obj = arp.GetArg("SmartSession");
                ISmartSession session = obj as ISmartSession;
                if (session != null)
                {
                    ISmartToolWindow wnd = session.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(_extractWindowID);
                    (wnd as DockWindow).TabStrip.SizeInfo.AbsoluteSize = new System.Drawing.Size(_extractWindowWidth, 0);
                }
            }
            InitArgument();
            //if (_handler != null)
            //    _handler(GetArgumentValue());
        }

        //读取参数值并初始化控件
        #region 

        private void InitArgument()
        {
            _extractArg = new DstFY3ExtractArgSet();
            txtTBB11BValue1.Text = _extractArg.BareLandArg.TBB11[0].ToString();
            txtTBB11BValue2.Text = _extractArg.BareLandArg.TBB11[1].ToString();
            txtTBB11BValue3.Text = _extractArg.BareLandArg.TBB11[2].ToString();
            txtTBB11BValue4.Text = _extractArg.BareLandArg.TBB11[3].ToString();
            txtTBB11VValue1.Text = _extractArg.VegetationArg.TBB11[0].ToString();
            txtTBB11VValue2.Text = _extractArg.VegetationArg.TBB11[1].ToString();
            txtTBB11VValue3.Text = _extractArg.VegetationArg.TBB11[2].ToString();
            txtTBB11VValue4.Text = _extractArg.VegetationArg.TBB11[3].ToString();
            txtTBB11WValue1.Text = _extractArg.WaterArg.TBB11[0].ToString();
            txtTBB11WValue2.Text = _extractArg.WaterArg.TBB11[1].ToString();
            txtTBB11WValue3.Text = _extractArg.WaterArg.TBB11[2].ToString();
            txtTBB11WValue4.Text = _extractArg.WaterArg.TBB11[3].ToString();
            txtRef650BValue1.Text = _extractArg.BareLandArg.Ref650[0].ToString();
            txtRef650BValue2.Text = _extractArg.BareLandArg.Ref650[1].ToString();
            txtRef650BValue3.Text = _extractArg.BareLandArg.Ref650[2].ToString();
            txtRef650BValue4.Text = _extractArg.BareLandArg.Ref650[3].ToString();
            txtRef650VValue1.Text = _extractArg.VegetationArg.Ref650[0].ToString();
            txtRef650VValue2.Text = _extractArg.VegetationArg.Ref650[1].ToString();
            txtRef650VValue3.Text = _extractArg.VegetationArg.Ref650[2].ToString();
            txtRef650VValue4.Text = _extractArg.VegetationArg.Ref650[3].ToString();
            txtRef650WValue1.Text = _extractArg.WaterArg.Ref650[0].ToString();
            txtRef650WValue2.Text = _extractArg.WaterArg.Ref650[1].ToString();
            txtRef650WValue3.Text = _extractArg.WaterArg.Ref650[2].ToString();
            txtRef650WValue4.Text = _extractArg.WaterArg.Ref650[3].ToString();
            txtTBB37BValue1.Text = _extractArg.BareLandArg.TBB37[0].ToString();
            txtTBB37BValue2.Text = _extractArg.BareLandArg.TBB37[1].ToString();
            txtTBB37BValue3.Text = _extractArg.BareLandArg.TBB37[2].ToString();
            txtTBB37BValue4.Text = _extractArg.BareLandArg.TBB37[3].ToString();
            txtTBB37VValue1.Text = _extractArg.VegetationArg.TBB37[0].ToString();
            txtTBB37VValue2.Text = _extractArg.VegetationArg.TBB37[1].ToString();
            txtTBB37VValue3.Text = _extractArg.VegetationArg.TBB37[2].ToString();
            txtTBB37VValue4.Text = _extractArg.VegetationArg.TBB37[3].ToString();
            txtTBB37WValue1.Text = _extractArg.WaterArg.TBB37[0].ToString();
            txtTBB37WValue2.Text = _extractArg.WaterArg.TBB37[1].ToString();
            txtTBB37WValue3.Text = _extractArg.WaterArg.TBB37[2].ToString();
            txtTBB37WValue4.Text = _extractArg.WaterArg.TBB37[3].ToString();
            txtBTD11_12BValue1.Text = _extractArg.BareLandArg.BTD11_12[0].ToString();
            txtBTD11_12BValue2.Text = _extractArg.BareLandArg.BTD11_12[1].ToString();
            txtBTD11_12BValue3.Text = _extractArg.BareLandArg.BTD11_12[2].ToString();
            txtBTD11_12BValue4.Text = _extractArg.BareLandArg.BTD11_12[3].ToString();
            txtBTD11_12VValue1.Text = _extractArg.VegetationArg.BTD11_12[0].ToString();
            txtBTD11_12VValue2.Text = _extractArg.VegetationArg.BTD11_12[1].ToString();
            txtBTD11_12VValue3.Text = _extractArg.VegetationArg.BTD11_12[2].ToString();
            txtBTD11_12VValue4.Text = _extractArg.VegetationArg.BTD11_12[3].ToString();
            txtBTD11_12WValue1.Text = _extractArg.WaterArg.BTD11_37[0].ToString();
            txtBTD11_12WValue2.Text = _extractArg.WaterArg.BTD11_37[1].ToString();
            txtBTD11_12WValue3.Text = _extractArg.WaterArg.BTD11_37[2].ToString();
            txtBTD11_12WValue4.Text = _extractArg.WaterArg.BTD11_37[3].ToString();
            txtBTD12_37BValue1.Text = _extractArg.BareLandArg.BTD11_37[0].ToString();
            txtBTD12_37BValue2.Text = _extractArg.BareLandArg.BTD11_37[1].ToString();
            txtBTD12_37BValue3.Text = _extractArg.BareLandArg.BTD11_37[2].ToString();
            txtBTD12_37BValue4.Text = _extractArg.BareLandArg.BTD11_37[3].ToString();
            txtBTD12_37VValue1.Text = _extractArg.VegetationArg.BTD11_37[0].ToString();
            txtBTD12_37VValue2.Text = _extractArg.VegetationArg.BTD11_37[1].ToString();
            txtBTD12_37VValue3.Text = _extractArg.VegetationArg.BTD11_37[2].ToString();
            txtBTD12_37VValue4.Text = _extractArg.VegetationArg.BTD11_37[3].ToString();
            txtBTD12_37WValue1.Text = _extractArg.WaterArg.BTD11_37[0].ToString();
            txtBTD12_37WValue2.Text = _extractArg.WaterArg.BTD11_37[1].ToString();
            txtBTD12_37WValue3.Text = _extractArg.WaterArg.BTD11_37[2].ToString();
            txtBTD12_37WValue4.Text = _extractArg.WaterArg.BTD11_37[3].ToString();
            txtR47_64BValue1.Text = _extractArg.BareLandArg.R47_64[0].ToString();
            txtR47_64BValue2.Text = _extractArg.BareLandArg.R47_64[1].ToString();
            txtR47_64BValue3.Text = _extractArg.BareLandArg.R47_64[2].ToString();
            txtR47_64BValue4.Text = _extractArg.BareLandArg.R47_64[3].ToString();
            txtR47_64VValue1.Text = _extractArg.VegetationArg.R47_64[0].ToString();
            txtR47_64VValue2.Text = _extractArg.VegetationArg.R47_64[1].ToString();
            txtR47_64VValue3.Text = _extractArg.VegetationArg.R47_64[2].ToString();
            txtR47_64VValue4.Text = _extractArg.VegetationArg.R47_64[3].ToString();
            txtR47_64WValue1.Text = _extractArg.WaterArg.R47_64[0].ToString();
            txtR47_64WValue2.Text = _extractArg.WaterArg.R47_64[1].ToString();
            txtR47_64WValue3.Text = _extractArg.WaterArg.R47_64[2].ToString();
            txtR47_64WValue4.Text = _extractArg.WaterArg.R47_64[3].ToString();
            txtNDVIBValue1.Text = _extractArg.BareLandArg.NDVI[0].ToString();
            txtNDVIBValue2.Text = _extractArg.BareLandArg.NDVI[1].ToString();
            txtNDVIBValue3.Text = _extractArg.BareLandArg.NDVI[2].ToString();
            txtNDVIBValue4.Text = _extractArg.BareLandArg.NDVI[3].ToString();
            txtNDVIVValue1.Text = _extractArg.VegetationArg.NDVI[0].ToString();
            txtNDVIVValue2.Text = _extractArg.VegetationArg.NDVI[1].ToString();
            txtNDVIVValue3.Text = _extractArg.VegetationArg.NDVI[2].ToString();
            txtNDVIVValue4.Text = _extractArg.VegetationArg.NDVI[3].ToString();
            txtNDVIWValue1.Text = _extractArg.WaterArg.NDVI[0].ToString();
            txtNDVIWValue2.Text = _extractArg.WaterArg.NDVI[1].ToString();
            txtNDVIWValue3.Text = _extractArg.WaterArg.NDVI[2].ToString();
            txtNDVIWValue4.Text = _extractArg.WaterArg.NDVI[3].ToString();
            txtNDSIBValue1.Text = _extractArg.BareLandArg.NDSI[0].ToString();
            txtNDSIBValue2.Text = _extractArg.BareLandArg.NDSI[1].ToString();
            txtNDSIBValue3.Text = _extractArg.BareLandArg.NDSI[2].ToString();
            txtNDSIBValue4.Text = _extractArg.BareLandArg.NDSI[3].ToString();
            txtNDSIVValue1.Text = _extractArg.VegetationArg.NDSI[0].ToString();
            txtNDSIVValue2.Text = _extractArg.VegetationArg.NDSI[1].ToString();
            txtNDSIVValue3.Text = _extractArg.VegetationArg.NDSI[2].ToString();
            txtNDSIVValue4.Text = _extractArg.VegetationArg.NDSI[3].ToString();
            txtNDSIWValue1.Text = _extractArg.WaterArg.NDSI[0].ToString();
            txtNDSIWValue2.Text = _extractArg.WaterArg.NDSI[1].ToString();
            txtNDSIWValue3.Text = _extractArg.WaterArg.NDSI[2].ToString();
            txtNDSIWValue4.Text = _extractArg.WaterArg.NDSI[3].ToString();
            txtNDDIBValue1.Text = _extractArg.BareLandArg.NDDI[0].ToString();
            txtNDDIBValue2.Text = _extractArg.BareLandArg.NDDI[1].ToString();
            txtNDDIBValue3.Text = _extractArg.BareLandArg.NDDI[2].ToString();
            txtNDDIBValue4.Text = _extractArg.BareLandArg.NDDI[3].ToString();
            txtNDDIVValue1.Text = _extractArg.VegetationArg.NDDI[0].ToString();
            txtNDDIVValue2.Text = _extractArg.VegetationArg.NDDI[1].ToString();
            txtNDDIVValue3.Text = _extractArg.VegetationArg.NDDI[2].ToString();
            txtNDDIVValue4.Text = _extractArg.VegetationArg.NDDI[3].ToString();
            txtNDDIWValue1.Text = _extractArg.WaterArg.NDDI[0].ToString();
            txtNDDIWValue2.Text = _extractArg.WaterArg.NDDI[1].ToString();
            txtNDDIWValue3.Text = _extractArg.WaterArg.NDDI[2].ToString();
            txtNDDIWValue4.Text = _extractArg.WaterArg.NDDI[3].ToString();
            txtRef470BValue1.Text = _extractArg.BareLandArg.Ref470[0].ToString();
            txtRef470BValue2.Text = _extractArg.BareLandArg.Ref470[1].ToString();
            txtRef470BValue3.Text = _extractArg.BareLandArg.Ref470[2].ToString();
            txtRef470BValue4.Text = _extractArg.BareLandArg.Ref470[3].ToString();
            txtRef470VValue1.Text = _extractArg.VegetationArg.Ref470[0].ToString();
            txtRef470VValue2.Text = _extractArg.VegetationArg.Ref470[1].ToString();
            txtRef470VValue3.Text = _extractArg.VegetationArg.Ref470[2].ToString();
            txtRef470VValue4.Text = _extractArg.VegetationArg.Ref470[3].ToString();
            txtRef470WValue1.Text = _extractArg.WaterArg.Ref470[0].ToString();
            txtRef470WValue2.Text = _extractArg.WaterArg.Ref470[1].ToString();
            txtRef470WValue3.Text = _extractArg.WaterArg.Ref470[2].ToString();
            txtRef470WValue4.Text = _extractArg.WaterArg.Ref470[3].ToString();
            txtIDSIBValue1.Text = _extractArg.BareLandArg.IDSI[0].ToString();
            txtIDSIBValue2.Text = _extractArg.BareLandArg.IDSI[1].ToString();
            txtIDSIBValue3.Text = _extractArg.BareLandArg.IDSI[2].ToString();
            txtIDSIBValue4.Text = _extractArg.BareLandArg.IDSI[3].ToString();
            txtIDSIVValue1.Text = _extractArg.VegetationArg.IDSI[0].ToString();
            txtIDSIVValue2.Text = _extractArg.VegetationArg.IDSI[1].ToString();
            txtIDSIVValue3.Text = _extractArg.VegetationArg.IDSI[2].ToString();
            txtIDSIVValue4.Text = _extractArg.VegetationArg.IDSI[3].ToString();
            txtIDSIWValue1.Text = _extractArg.WaterArg.IDSI[0].ToString();
            txtIDSIWValue2.Text = _extractArg.WaterArg.IDSI[1].ToString();
            txtIDSIWValue3.Text = _extractArg.WaterArg.IDSI[2].ToString();
            txtIDSIWValue4.Text = _extractArg.WaterArg.IDSI[3].ToString();
            txtRef1380BValue1.Text = _extractArg.BareLandArg.Ref1380[0].ToString();
            txtRef1380BValue2.Text = _extractArg.BareLandArg.Ref1380[1].ToString();
            txtRef1380BValue3.Text = _extractArg.BareLandArg.Ref1380[2].ToString();
            txtRef1380BValue4.Text = _extractArg.BareLandArg.Ref1380[3].ToString();
            txtRef1380VValue1.Text = _extractArg.VegetationArg.Ref1380[0].ToString();
            txtRef1380VValue2.Text = _extractArg.VegetationArg.Ref1380[1].ToString();
            txtRef1380VValue3.Text = _extractArg.VegetationArg.Ref1380[2].ToString();
            txtRef1380VValue4.Text = _extractArg.VegetationArg.Ref1380[3].ToString();
            txtRef1380WValue1.Text = _extractArg.WaterArg.Ref1380[0].ToString();
            txtRef1380WValue2.Text = _extractArg.WaterArg.Ref1380[1].ToString();
            txtRef1380WValue3.Text = _extractArg.WaterArg.Ref1380[2].ToString();
            txtRef1380WValue4.Text = _extractArg.WaterArg.Ref1380[3].ToString();
        }

        #endregion

        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return _isValueChangedEvent; 
            }
            set
            {
                _isValueChangedEvent = value;
            }
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return null;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        private void SetArgValue()
        {
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            InitArgument();
        }
    }
}
