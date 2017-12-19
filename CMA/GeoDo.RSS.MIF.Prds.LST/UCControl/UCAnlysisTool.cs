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
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.UI;
using GeoDo.RSS.UI.AddIn.Theme;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.MIF.Prds.LST
{
    public partial class UCAnlysisTool : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler;
        private IArgumentProvider _arp = null;

        public UCAnlysisTool()
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
            _arp = subProduct.ArgumentProvider;
            if (_arp == null)
                return;
            _arp.SetArg("ucAnlysisTool", this);
        }

        public void btnGetInfos_Click(object sender, EventArgs e)
        {
            if (_arp != null)
            {
                double lstBandRoom = (double)_arp.GetArg("lstband_Zoom");
                IRasterDataProvider prd = _arp.DataProvider;
                if (prd == null)
                    return;
                IBandNameRaster bandNameRaster = prd as IBandNameRaster;
                int lstbandNo = TryGetBandNo(bandNameRaster, "lstband");
                if (lstbandNo == -1 || lstBandRoom == -1)
                    return;
                ArgumentProvider ap = new ArgumentProvider(prd, null);
                Size size = new Size(prd.Width, prd.Height);
                if (_arp.AOIs != null && _arp.AOIs.Length != 0)
                {
                    txtIdenfiy.Text = GetInfoExt(_arp.AOIs);
                }
                Rectangle rect = AOIHelper.ComputeAOIRect(_arp.AOI, size);
                Dictionary<string, Dictionary<string, float>> anlysis = new Dictionary<string, Dictionary<string, float>>();
                InitAnlysisInfo(anlysis);
                int count = 0;
                Int16[] cloudValues = GetNanValues("CloudyValue");
                Int16[] waterValues = GetNanValues("WaterValue");
                Int16[] invaildValues = GetNanValues("InvailValue");
                using (RasterPixelsVisitor<Int16> visitor = new RasterPixelsVisitor<Int16>(ap))
                {
                    visitor.VisitPixel(rect, _arp.AOI, new int[] { lstbandNo },
                        (index, values) =>
                        {
                            if (IsNanValue(values[0], cloudValues) || IsNanValue(values[0], waterValues) || IsNanValue(values[0], invaildValues))
                                return;
                            AnlysisValues(anlysis, "地表温度", values[0]);
                            count++;
                        });
                }
                anlysis["地表温度"]["avg"] = (float)Math.Round(anlysis["地表温度"]["avg"] / count, 4);
                using (RasterPixelsVisitor<Int16> visitor = new RasterPixelsVisitor<Int16>(ap))
                {
                    visitor.VisitPixel(rect, _arp.AOI, new int[] { lstbandNo },
                        (index, values) =>
                        {
                            anlysis["地表温度"]["pc"] += (float)Math.Pow(values[0] - anlysis["地表温度"]["avg"], 2);
                        });
                }
                anlysis["地表温度"]["pc"] = (float)Math.Round(Math.Sqrt(anlysis["地表温度"]["pc"]) / count, 4);

                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("地表温度:\n  最小值：{0}\n  最大值：{1}\n  平均值：{2}\n  偏差值：{3}\n", anlysis["地表温度"]["min"], anlysis["地表温度"]["max"], anlysis["地表温度"]["avg"], anlysis["地表温度"]["pc"]));
                txtInfos.Text = sb.ToString();
            }
        }

        private string GetInfoExt(CodeCell.AgileMap.Core.Feature[] feature)
        {
            string extInfo = string.Empty;
            foreach (Feature item in feature)
            {
                for (int i = 0; i < item.FieldNames.Length; i++)
                {
                    if (item.FieldNames[i].ToUpper().Contains("NAME") || item.FieldNames[i].Contains("名称"))
                    {
                        extInfo += "_" + item.FieldValues[i];
                        break; ;
                    }
                }
            }
            return string.IsNullOrEmpty(extInfo) || extInfo == "_AOI" ? txtIdenfiy.Text : extInfo;
        }

        private void AnlysisValues(Dictionary<string, Dictionary<string, float>> anlysis, string key, float value)
        {
            anlysis[key]["min"] = anlysis[key]["min"] > value ? value : anlysis[key]["min"];
            anlysis[key]["max"] = anlysis[key]["max"] < value ? value : anlysis[key]["max"];
            anlysis[key]["avg"] += value;
        }

        private void InitAnlysisInfo(Dictionary<string, Dictionary<string, float>> anlysis)
        {
            anlysis.Add("地表温度", new Dictionary<string, float>());
            anlysis["地表温度"].Add("min", float.MaxValue);
            anlysis["地表温度"].Add("max", float.MinValue);
            anlysis["地表温度"].Add("avg", 0f);
            anlysis["地表温度"].Add("pc", 0f);
        }

        private int TryGetBandNo(IBandNameRaster bandNameRaster, string argName)
        {
            int bandNo = (int)_arp.GetArg(argName);
            if (bandNameRaster != null)
            {
                int newbandNo = -1;
                if (bandNameRaster.TryGetBandNoFromBandName(bandNo, out newbandNo))
                    bandNo = newbandNo;
            }
            return bandNo;
        }

        private string[] GetParameterValues()
        {
            List<string> rets = new List<string>();
            rets.Add(txtIdenfiy.Text);
            rets.Add(txtInfos.Text);
            return rets.ToArray();
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return null;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            btnGetInfos_Click(null, null);
            if (_handler != null)
                _handler(GetParameterValues());
        }


        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        private Int16[] GetNanValues(string argumentName)
        {
            string nanValuestring = _arp.GetArg(argumentName) as string;
            if (!string.IsNullOrEmpty(nanValuestring))
            {
                string[] valueStrings = nanValuestring.Split(new char[] { ',', '，' });
                if (valueStrings != null && valueStrings.Length > 0)
                {
                    List<short> values = new List<short>();
                    short value;
                    for (int i = 0; i < valueStrings.Length; i++)
                    {
                        if (Int16.TryParse(valueStrings[i], out value))
                            values.Add(value);
                    }
                    if (values.Count > 0)
                    {
                        return values.ToArray();
                    }
                }
            }
            return null;
        }

        private bool IsNanValue(short pixelValue, short[] nanValues)
        {
            if (nanValues == null || nanValues.Length < 1)
            {
                return false;
            }
            else
            {
                foreach (short value in nanValues)
                {
                    if (pixelValue == value)
                        return true;
                }
            }
            return false;
        }
    }
}
