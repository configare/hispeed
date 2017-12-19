using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using System.IO;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    public partial class UCCmpSD : UserControl,IArgumentEditorUI
    {
        /// 复制参数 需要整理
        private Dictionary<string, object> _result = null;
        private GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer aoiContainer = new GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer();
        List<string> fieldValues = new List<string>();
        private string IsSeniorQuery = "NO";
        private string IsAllContry = "NO";
       

        private Action<object> _handler;
        private string sdParas;                    //新输入的参数
        private string ExtractParameters = "";
        private string sdParameters = "";         // 配置文件中获取的参数
        public UCCmpSD()
        {
            InitializeComponent();
            //读Xml
            ParseXml(txtsdPara.Text);
            //显示参数
            GetSDPara(sdParameters);
        }
        #region IArgumentEditorUI 成员
        public object GetArgumentValue()
        {
            //将算法参数全部传入数组，然后在细节代码处按序号取出需求参数，这种方式维护阅读性不是很好-如果需要修改算法 此处将更改
            //暂时无修改-暂时搁置
            List<float> argValues = new List<float>();
            float value;
            if (float.TryParse(txta1.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txta2.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txta3.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtb1.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtb2.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtb3.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtb4.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtb5.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtc1.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtc2.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtc3.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtd1.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtd2.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtd3.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtd4.Text, out value))
                argValues.Add(value);
            string[] infos = ExtractParameters.Split(new char[] { ',' });
            if (float.TryParse(infos[0].Split('=')[1], out value))
                argValues.Add(value);
            if (float.TryParse(infos[1].Split('=')[1], out value))
                argValues.Add(value);
            if (float.TryParse(infos[2].Split('=')[1], out value))
                argValues.Add(value);
            if (float.TryParse(infos[3].Split('=')[1], out value))
                argValues.Add(value);
            if (float.TryParse(infos[4].Split('=')[1], out value))
                argValues.Add(value); 
            if (float.TryParse(infos[5].Split('=')[1], out value))
                argValues.Add(value);
            return argValues.ToArray();
        }
        /// <summary>
        /// 重构该方法上面方法
        /// 方便传递参数的可扩展
        /// </summary>
        /// <returns>参数对象</returns>
        public object GetArgumentValues()
        {
            SNWSDSettingPar inputpars = new SNWSDSettingPar();

            List<float> argValues = new List<float>();
            #region  算法参数
            float value;
            if (float.TryParse(txta1.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txta2.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txta3.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtb1.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtb2.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtb3.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtb4.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtb5.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtc1.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtc2.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtc3.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtd1.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtd2.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtd3.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txtd4.Text, out value))
                argValues.Add(value);
            string[] infos = ExtractParameters.Split(new char[] { ',' });
            if (float.TryParse(infos[0].Split('=')[1], out value))
                argValues.Add(value);
            if (float.TryParse(infos[1].Split('=')[1], out value))
                argValues.Add(value);
            if (float.TryParse(infos[2].Split('=')[1], out value))
                argValues.Add(value);
            if (float.TryParse(infos[3].Split('=')[1], out value))
                argValues.Add(value);
            if (float.TryParse(infos[4].Split('=')[1], out value))
                argValues.Add(value);
            if (float.TryParse(infos[5].Split('=')[1], out value))
                argValues.Add(value);
            #endregion
            inputpars.AlgorithmPars = argValues.ToArray();
            inputpars.AoiContainer = string.IsNullOrEmpty(this.txtRegionName.Text.Trim())?null:aoiContainer;
            inputpars.SelectRegionName = this.txtRegionName.Text.Trim();
           
            return inputpars;
        }
        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }
        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return null;
        }
        #endregion

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgumentValues());
        }

        private void ToXml(string xmlFileName)
        {
            if(string.IsNullOrEmpty(ExtractParameters))
                ExtractParameters = "23V89Vmin=5" + ","
                                    + "18V36Vmin=5" +","
                                    + "23Vmax=260"+ ","
                                    + "18Vsec36Vmin=20" + ","
                                    + "si1si2thickmin=-35"+ ","
                                    + "si1thinsi2min=8";
            XElement xml = new XElement("xml",
                new XElement("ExtractParameters", ExtractParameters),
                new XElement("sdParameters", sdParas));
            xml.Save(xmlFileName);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog frm = new SaveFileDialog())
            {
                frm.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS");
                frm.Filter = "雪深计算参数文件（sdParas_*.xml）|sdParas_*.xml";
                string xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS");
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    string xmlfile = frm.FileName;
                    sdParas = "a1=" + txta1.Text.Trim() + ","
                            + "a2=" + txta2.Text.Trim() + ","
                            + "a3=" + txta3.Text.Trim() + ","
                            + "b1=" + txtb1.Text.Trim() + ","
                            + "b2=" + txtb2.Text.Trim() + ","
                            + "b3=" + txtb3.Text.Trim() + ","
                            + "b4=" + txtb4.Text.Trim() + ","
                            + "b5=" + txtb5.Text.Trim() + ","
                            + "c1=" + txtc1.Text.Trim() + ","
                            + "c2=" + txtc2.Text.Trim() + ","
                            + "c3=" + txtc3.Text.Trim() + ","
                            + "d1=" + txtd1.Text.Trim() + ","
                            + "d2=" + txtd2.Text.Trim() + ","
                            + "d3=" + txtd3.Text.Trim() + ","
                            + "d4=" + txtd4.Text.Trim();
                    ToXml(xmlfile);
                }
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog frm = new OpenFileDialog())
            {
                frm.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS");
                frm.Filter = "雪深计算参数文件（sdParas_*.xml）|sdParas_*.xml";
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    txtsdPara.Text = Path.GetFileName(frm.FileName);
                    //读Xml
                    ParseXml(txtsdPara.Text);
                    //显示参数
                    GetSDPara(sdParameters);
                }
            }
        }
        private void ParseXml(string xmlfile)
        {
            string xmlfilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS");
            xmlfile = Path.Combine(xmlfilePath, xmlfile);
            if (!File.Exists(xmlfile))
                throw new FileNotFoundException("参数文件不存在" + xmlfile, xmlfile);
            if (string.IsNullOrWhiteSpace(xmlfile))
                throw new ArgumentNullException("argXml", "参数文件为空");
            XElement xml = XElement.Load(xmlfile);
            XElement extractPara = xml.Element("ExtractParameters");
            if (extractPara != null)
                ExtractParameters = extractPara.Value;
            XElement sdPara = xml.Element("sdParameters");
            if (sdPara != null)
                sdParameters = sdPara.Value;
        }
        private void GetSDPara(string sdPara)
        {
            string[] infos = sdPara.Split(new char[] { ',' });
            txta1.Text = infos[0].Split('=')[1];
            txta2.Text = infos[1].Split('=')[1];
            txta3.Text = infos[2].Split('=')[1];
            txtb1.Text = infos[3].Split('=')[1];
            txtb2.Text = infos[4].Split('=')[1];
            txtb3.Text = infos[5].Split('=')[1];
            txtb4.Text = infos[6].Split('=')[1];
            txtb5.Text = infos[7].Split('=')[1];
            txtc1.Text = infos[8].Split('=')[1];
            txtc2.Text = infos[9].Split('=')[1];
            txtc3.Text = infos[10].Split('=')[1];
            txtd1.Text = infos[11].Split('=')[1];
            txtd2.Text = infos[12].Split('=')[1];
            txtd3.Text = infos[13].Split('=')[1];
            txtd4.Text = infos[14].Split('=')[1];
        }
        /// <summary>
        /// 选择区域按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChooseAOI_Click(object sender, EventArgs e)
        {
            int fieldIndex = -1; string fieldName; string shapeFilename; string regionName = "";
            if (aoiContainer != null)
                aoiContainer.Dispose();
            if (fieldValues != null)
                fieldValues.Clear();
            using (frmStatSubRegionTemplatesMWS frm = new frmStatSubRegionTemplatesMWS())
            {
                frm.listView1.MultiSelect = true;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    Feature[] fets = frm.GetSelectedFeatures();
                    fets = frm.GetStatFeatures(out fieldName, out shapeFilename, out fieldIndex);
                    if (fets == null)
                    {
                        MessageBox.Show("未选定目标区域，请选择区域");
                    }
                    else
                    {
                        string chinafieldValue = fets[0].GetFieldValue(fieldIndex);
                        if (chinafieldValue == "中国")
                        {
                            aoiContainer.AddAOI(fets[0]);
                            regionName = "全国";
                            fieldValues.Add("全国");
                            IsAllContry = "YES";
                        }
                        else
                        {
                            foreach (Feature fet in fets)
                            {
                            
                                fieldValues.Add(fet.GetFieldValue(fieldIndex)); //获得选择区域名称
                                aoiContainer.AddAOI(fet);
                            }
                            regionName = "";
                            foreach (string region in fieldValues)
                            {
                                regionName += region;
                            }
                        }
                    }
                    txtRegionName.Text = regionName;
                    
                }
                else
                {
                    MessageBox.Show("请选择区域");
                }
            }
        }
    }
}
