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

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public partial class UCAnlysisTool : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler;
        private IArgumentProvider _arp = null;
        private string[] _writeTxt = null;

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
                double visiBandRoom = (double)_arp.GetArg("Visible_Zoom");
                double niBandRoom = (double)_arp.GetArg("NearInfrared_Zoom");
                double shortInfraredRoom = (double)_arp.GetArg("ShortInfrared_Zoom");
                IRasterDataProvider prd = _arp.DataProvider;
                if (prd == null)
                    return;
                IBandNameRaster bandNameRaster = prd as IBandNameRaster;
                int visiBandNo = TryGetBandNo(bandNameRaster, "Visible");
                int niBandNo = TryGetBandNo(bandNameRaster, "NearInfrared");
                int shortInfraredBandNo = TryGetBandNo(bandNameRaster, "ShortInfrared");
                if (visiBandNo == -1 || niBandNo == -1 || shortInfraredBandNo == -1 || shortInfraredRoom == -1 || visiBandRoom == -1 || niBandRoom == -1)
                    return;
                ArgumentProvider ap = new ArgumentProvider(prd, null);
                Size size = new Size(prd.Width, prd.Height);
                if (_arp.AOIs != null && _arp.AOIs.Length != 0)
                {
                    cmbType.Text = GetInfoExt(_arp.AOIs);
                }
                Rectangle rect = AOIHelper.ComputeAOIRect(_arp.AOI, size);
                Dictionary<string, Dictionary<string, float>> anlysis = new Dictionary<string, Dictionary<string, float>>();
                InitAnlysisInfo(anlysis);
                int count = 0;
                int ndviCount = 0;
                using (RasterPixelsVisitor<UInt16> visitor = new RasterPixelsVisitor<UInt16>(ap))
                {
                    visitor.VisitPixel(rect, _arp.AOI, new int[] { visiBandNo, niBandNo, shortInfraredBandNo },
                        (index, values) =>
                        {
                            AnlysisValues(anlysis, "可见光", values[0]);
                            AnlysisValues(anlysis, "近红外", values[1]);
                            AnlysisValues(anlysis, "短波红外", values[2]);
                            if (values[1] + values[0] != 0)
                            {
                                AnlysisValues(anlysis, "NDVI", (float)Math.Round((float)(values[1] - values[0]) / (values[1] + values[0]), 4));
                                ndviCount++;
                            }
                            count++;
                        });
                }
                anlysis["可见光"]["avg"] = (float)Math.Round(anlysis["可见光"]["avg"] / count, 4);
                anlysis["近红外"]["avg"] = (float)Math.Round(anlysis["近红外"]["avg"] / count, 4);
                anlysis["短波红外"]["avg"] = (float)Math.Round(anlysis["短波红外"]["avg"] / ndviCount, 4);
                anlysis["NDVI"]["avg"] = (float)Math.Round(anlysis["NDVI"]["avg"] / ndviCount, 4);
                using (RasterPixelsVisitor<UInt16> visitor = new RasterPixelsVisitor<UInt16>(ap))
                {
                    visitor.VisitPixel(rect, _arp.AOI, new int[] { visiBandNo, niBandNo, shortInfraredBandNo },
                        (index, values) =>
                        {
                            anlysis["可见光"]["pc"] += (float)Math.Pow(values[0] - anlysis["可见光"]["avg"], 2);
                            anlysis["近红外"]["pc"] += (float)Math.Pow(values[1] - anlysis["近红外"]["avg"], 2);
                            anlysis["短波红外"]["pc"] += (float)Math.Pow(values[2] - anlysis["短波红外"]["avg"], 2);
                            if (values[1] + values[0] != 0)
                                anlysis["NDVI"]["pc"] += (float)Math.Pow((float)(values[1] - values[0]) / (values[1] + values[0]) - anlysis["NDVI"]["avg"], 2);
                        });
                }
                anlysis["可见光"]["pc"] = (float)Math.Round(Math.Sqrt(anlysis["可见光"]["pc"]) / count, 4);
                anlysis["近红外"]["pc"] = (float)Math.Round(Math.Sqrt(anlysis["近红外"]["pc"]) / count, 4);
                anlysis["短波红外"]["pc"] = (float)Math.Round(Math.Sqrt(anlysis["短波红外"]["pc"]) / count, 4);
                anlysis["NDVI"]["pc"] = (float)Math.Round(Math.Sqrt(anlysis["NDVI"]["pc"]) / ndviCount, 4);

                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("可见光:\n  最小值：{0}\n  最大值：{1}\n  平均值：{2}\n  偏差值：{3}\n", anlysis["可见光"]["min"], anlysis["可见光"]["max"], anlysis["可见光"]["avg"], anlysis["可见光"]["pc"]));
                sb.Append(string.Format("近红外:\n  最小值：{0}\n  最大值：{1}\n  平均值：{2}\n  偏差值：{3}\n", anlysis["近红外"]["min"], anlysis["近红外"]["max"], anlysis["近红外"]["avg"], anlysis["近红外"]["pc"]));
                sb.Append(string.Format("NDVI:\n  最小值：{0}\n  最大值：{1}\n  平均值：{2}\n  偏差值：{3}\n", anlysis["NDVI"]["min"], anlysis["NDVI"]["max"], anlysis["NDVI"]["avg"], anlysis["NDVI"]["pc"]));
                sb.Append(string.Format("短波红外:\n  最小值：{0}\n  最大值：{1}\n  平均值：{2}\n  偏差值：{3}\n", anlysis["短波红外"]["min"], anlysis["短波红外"]["max"], anlysis["短波红外"]["avg"], anlysis["短波红外"]["pc"]));
                txtInfos.Text = sb.ToString();

                InitWriteTxt(anlysis);
            }
        }

        private void InitWriteTxt(Dictionary<string, Dictionary<string, float>> anlysis)
        {
            int pcValue = 2;
            string title1 = string.Format("{0}\t{1}\t{2}\t{3}", "可见光", "近红外".PadLeft(13 * pcValue), "NDVI".PadLeft(13 * pcValue), "短波红外".PadLeft(13 * pcValue));

            StringBuilder sb1 = new StringBuilder();
            for (int i = 0; i < 4; i++)
                sb1.Append(string.Format("{0}\t{1}\t{2}\t{3}\t", "最小值".PadLeft(pcValue), "最大值".PadLeft(pcValue), "平均值".PadLeft(pcValue), "偏差值".PadLeft(pcValue)));
            StringBuilder sb2 = new StringBuilder();
            sb2.Append(string.Format("{0}\t{1}\t{2}\t{3}\t", SetStrStyle(anlysis["可见光"]["min"]), SetStrStyle(anlysis["可见光"]["max"]), SetStrStyle(anlysis["可见光"]["avg"]), SetStrStyle(anlysis["可见光"]["pc"])));
            sb2.Append(string.Format("{0}\t{1}\t{2}\t{3}\t", SetStrStyle(anlysis["近红外"]["min"]), SetStrStyle(anlysis["近红外"]["max"]), SetStrStyle(anlysis["近红外"]["avg"]), SetStrStyle(anlysis["近红外"]["pc"])));
            sb2.Append(string.Format("{0}\t{1}\t{2}\t{3}\t", SetStrStyle(anlysis["NDVI"]["min"]), SetStrStyle(anlysis["NDVI"]["max"]), SetStrStyle(anlysis["NDVI"]["avg"]), SetStrStyle(anlysis["NDVI"]["pc"])));
            sb2.Append(string.Format("{0}\t{1}\t{2}\t{3}\t", SetStrStyle(anlysis["短波红外"]["min"]), SetStrStyle(anlysis["短波红外"]["max"]), SetStrStyle(anlysis["短波红外"]["avg"]), SetStrStyle(anlysis["短波红外"]["pc"])));
            _writeTxt = new string[] { title1, sb1.ToString(), sb2.ToString() };
        }

        private string SetStrStyle(float value)
        {
            int pcValue = 6;
            return value.ToString().PadLeft(pcValue);
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
            return string.IsNullOrEmpty(extInfo) || extInfo == "_AOI" ? cmbType.Text : extInfo;
        }

        private void AnlysisValues(Dictionary<string, Dictionary<string, float>> anlysis, string key, float value)
        {
            anlysis[key]["min"] = anlysis[key]["min"] > value ? value : anlysis[key]["min"];
            anlysis[key]["max"] = anlysis[key]["max"] < value ? value : anlysis[key]["max"];
            anlysis[key]["avg"] += value;
        }

        private void InitAnlysisInfo(Dictionary<string, Dictionary<string, float>> anlysis)
        {
            anlysis.Add("可见光", new Dictionary<string, float>());
            anlysis["可见光"].Add("min", float.MaxValue);
            anlysis["可见光"].Add("max", float.MinValue);
            anlysis["可见光"].Add("avg", 0f);
            anlysis["可见光"].Add("pc", 0f);
            anlysis.Add("近红外", new Dictionary<string, float>());
            anlysis["近红外"].Add("min", float.MaxValue);
            anlysis["近红外"].Add("max", float.MinValue);
            anlysis["近红外"].Add("avg", 0f);
            anlysis["近红外"].Add("pc", 0f);
            anlysis.Add("NDVI", new Dictionary<string, float>());
            anlysis["NDVI"].Add("min", float.MaxValue);
            anlysis["NDVI"].Add("max", float.MinValue);
            anlysis["NDVI"].Add("avg", 0f);
            anlysis["NDVI"].Add("pc", 0f);
            anlysis.Add("短波红外", new Dictionary<string, float>());
            anlysis["短波红外"].Add("min", float.MaxValue);
            anlysis["短波红外"].Add("max", float.MinValue);
            anlysis["短波红外"].Add("avg", 0f);
            anlysis["短波红外"].Add("pc", 0f);
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
            rets.Add(cmbType.Text);
            rets.Add(txtInfos.Text);
            return rets.ToArray();
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            IEnumerable<XElement> node = ele.Elements("ValueItem");
            List<string> infoType = new List<string>();
            if (node != null && node.Count() != 0)
            {
                foreach (XElement item in node)
                {
                    string value = item.Attribute("value").Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        infoType.Add(value);
                        cmbType.Items.Add(value);
                    }
                }
                cmbType.SelectedIndex = 0;
                return infoType.ToArray();
            }
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

        public string[] WriteText
        {
            get { return _writeTxt; }
        }
    }
}
