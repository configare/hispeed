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

namespace GeoDo.RSS.MIF.Prds.MWS
{
    public partial class UCIdentifySnow : UserControl,IArgumentEditorUI
    {
        private Action<object> _handler;
        private string extractPara;
        private string ExtractParameters;
        private string xmlfile;
        private string sdParameters = "";

        public UCIdentifySnow()
        {
            InitializeComponent();
            //读Xml
            ParseXml(txtsdPara.Text);
            //显示参数
            GetExtractPara(ExtractParameters);
        }
        //private void GetXmlFile()
        //{
        //    string xmlfilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS");
        //    xmlfile = Path.Combine(xmlfilePath, "sdParas_Default.xml");
        //    if (!File.Exists(xmlfile))
        //        throw new FileNotFoundException("参数文件不存在" + xmlfile, xmlfile);
        //    if (string.IsNullOrWhiteSpace(xmlfile))
        //        throw new ArgumentNullException("argXml", "参数文件为空");
        //    XElement xml = XElement.Load(xmlfile);
        //    XElement extractPara = xml.Element("ExtractParameters");
        //    if (extractPara != null)
        //        ExtractParameters = extractPara.Value;
        //    GetExtractPara(ExtractParameters);
        //}
        
        #region IArgumentEditorUI 成员
        public object GetArgumentValue()
        {
            List<float> argValues = new List<float>();
            float value;
            if(float.TryParse(txt23V89Vmin.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txt18V36Vmin.Text, out value))
                argValues.Add(value);
            if (float.TryParse(txt23Vmax.Text, out value))
                argValues.Add(value);
            if(float.TryParse(txt18Vsec36Vmin.Text,out value))
                argValues.Add(value);
            if(float.TryParse(txtsi1si2thickmin.Text, out value))
                argValues.Add(value);
            if(float.TryParse(txtsi1thinsi2min.Text,out value))
                argValues.Add(value);
            return argValues.ToArray();
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
        private void ToXml(string xmlFileName)
        {
            if(string.IsNullOrEmpty(sdParameters))
                sdParameters = "a1=-5.690"+","
                            + "a2=0.345"+ ","
                            + "a3=0.817"+ ","
                            + "b1=4.320" + ","
                            + "b2=0.506" + ","
                            + "b3=0.131" + ","
                            + "b4=0.183" + ","
                            + "b5=0.123" +","
                            + "c1=3.418" +  ","
                            + "c2=0.411" + ","
                            + "c3=0.212"+ ","
                            + "d1=10.766"+ ","
                            + "d2=0.421"+","
                            + "d3=1.121"+ ","
                            + "d4=0.673";
            XElement xml = new XElement("xml",
                new XElement("ExtractParameters", extractPara),
                new XElement("sdParameters", sdParameters));
            xml.Save(xmlFileName);
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
                    GetExtractPara(ExtractParameters);
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
        private void GetExtractPara(string extractPara)
        {
            string[] infos = extractPara.Split(new char[] { ',' });
            txt23V89Vmin.Text = infos[0].Split('=')[1];
            txt18V36Vmin.Text = infos[1].Split('=')[1];
            txt23Vmax.Text = infos[2].Split('=')[1];
            txt18Vsec36Vmin.Text = infos[3].Split('=')[1];
            txtsi1si2thickmin.Text = infos[4].Split('=')[1];
            txtsi1thinsi2min.Text = infos[5].Split('=')[1];
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            using (SaveFileDialog frm = new SaveFileDialog())
            {
                frm.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS");
                frm.Filter = "雪深计算参数文件（sdParas_*.xml）|sdParas_*.xml";
                string xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS");
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    string xmlfile = frm.FileName;
                    extractPara = "23V89Vmin=" + txt23V89Vmin.Text.Trim() + ","
                                + "18V36Vmin=" + txt18V36Vmin.Text.Trim() + ","
                                + "23Vmax=" + txt23Vmax.Text.Trim() + ","
                                + "18Vsec36Vmin=" + txt18Vsec36Vmin.Text.Trim() + ","
                                + "si1si2thickmin=" + txtsi1si2thickmin.Text.Trim() + ","
                                + "si1thinsi2min=" + txtsi1thinsi2min.Text.Trim();
                    ToXml(xmlfile);
                }
            }
        }

        private void btnOK_Click_1(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void txt18Vsec36Vmin_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = txt18Vsec36Vmin.Text;
        }

        private void txtsdPara_TextChanged(object sender, EventArgs e)
        {

        } 
    }
}
