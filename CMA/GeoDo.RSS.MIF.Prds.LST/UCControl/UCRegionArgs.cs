using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.Prds.LST
{
    public partial class UCRegionArgs : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler;
        private string _timeFromat = "HHdd";

        public UCRegionArgs()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string min = DTBegin.Value.ToString(_timeFromat);
                string max = DTEnd.Value.ToString(_timeFromat);
                if (min.CompareTo(max) < 0)
                {
                    string sLine = min + "~" + max;
                    if (!lbxValues.Items.Contains(sLine))
                        lbxValues.Items.Add(sLine);
                }
            }
            catch
            {
            }
            btnOK_Click(null, null);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lbxValues.SelectedIndices.Count < 1)
                return;
            while (lbxValues.SelectedIndices.Count > 0)
            {
                lbxValues.Items.RemoveAt(lbxValues.SelectedIndices[0]);
            }
            btnOK_Click(null, null);
        }

        public object GetArgumentValue()
        {
            return GetRegionSetting();
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetRegionSetting());
        }

        private RegionArg GetRegionSetting()
        {
            RegionArg args = new RegionArg();
            args.CK001Days = ckDay.Checked;
            args.CK007Days = ckWeek.Checked;
            args.CK010Days = ck10Days.Checked;
            args.CK030Days = ckMonth.Checked;
            args.CK090Days = ck90Days.Checked;
            args.CK365Days = ckYear.Checked;
            args.CKSatellite = ckSatellite.Checked;
            args.CKTime = ckTime.Checked;
            args.CycType = (enumProcessType)(Enum.Parse(typeof(enumProcessType), cbCycType.SelectedIndex.ToString()));
            if (lbxValues.Items.Count != 0)
            {
                List<string> regions = new List<string>();
                foreach (object item in lbxValues.Items)
                {
                    if (item.ToString() != null)
                    {
                        regions.Add(item.ToString());
                    }
                }
                args.TimeRegion = GetValues(regions);
            }
            return args;
        }

        private SortedDictionary<string, string> GetValues(List<string> regions)
        {
            if (regions.Count() == 0)
                return null;
            SortedDictionary<string, string> values = new SortedDictionary<string, string>();
            foreach (string re in regions)
            {
                string[] minmax = ParseRegionToString(re);
                if (minmax == null || minmax.Length == 0)
                    continue;
                values.Add(minmax[0], minmax[1]);
            }
            return values;
        }

        private string[] ParseRegionToString(string re)
        {
            if (string.IsNullOrEmpty(re))
                return null;
            string[] parts = re.Split('~');
            if (parts == null || parts.Length == 0)
                return null;
            return new string[] { parts[0], parts[1] };
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            if (ele == null)
                return null;
            RegionArg args = new RegionArg();
            XElement segEle = ele.Element(XName.Get("CycType"));
            if (segEle != null)
            {
                if (segEle.Attribute(XName.Get("type")) != null)
                    args.CycType = (enumProcessType)Enum.Parse(typeof(enumProcessType), segEle.Attribute(XName.Get("type")).Value);
            }
            segEle = ele.Element(XName.Get("CycFlag"));
            if (segEle != null)
            {
                if (segEle.Attribute(XName.Get("days001")) != null)
                    args.CK001Days = bool.Parse(segEle.Attribute(XName.Get("days001")).Value);
                if (segEle.Attribute(XName.Get("days007")) != null)
                    args.CK007Days = bool.Parse(segEle.Attribute(XName.Get("days007")).Value);
                if (segEle.Attribute(XName.Get("days010")) != null)
                    args.CK010Days = bool.Parse(segEle.Attribute(XName.Get("days010")).Value);
                if (segEle.Attribute(XName.Get("days030")) != null)
                    args.CK030Days = bool.Parse(segEle.Attribute(XName.Get("days030")).Value);
                if (segEle.Attribute(XName.Get("days090")) != null)
                    args.CK090Days = bool.Parse(segEle.Attribute(XName.Get("days090")).Value);
                if (segEle.Attribute(XName.Get("days365")) != null)
                    args.CK365Days = bool.Parse(segEle.Attribute(XName.Get("days365")).Value);
            }
            segEle = ele.Element(XName.Get("TypeFlag"));
            if (segEle != null)
            {
                if (segEle.Attribute(XName.Get("satellite")) != null)
                    args.CKSatellite = bool.Parse(segEle.Attribute(XName.Get("satellite")).Value);
                if (segEle.Attribute(XName.Get("time")) != null)
                    args.CKTime = bool.Parse(segEle.Attribute(XName.Get("time")).Value);

            }
            segEle = ele.Element(XName.Get("Segements"));
            InitRegionArgs(args);
            if (segEle == null)
                return args;
            IEnumerable<XElement> segs = segEle.Elements(XName.Get("Segement"));
            if (segs == null || segs.Count() == 0)
                return null;
            args.TimeRegion = new SortedDictionary<string, string>();
            foreach (XElement seg in segs)
            {
                ParseMinMaxValue(seg, ref args.TimeRegion);
            }
            return args;
        }

        private void InitRegionArgs(RegionArg args)
        {
            ckDay.Checked = args.CK001Days;
            ckWeek.Checked = args.CK007Days;
            ck10Days.Checked = args.CK010Days;
            ckMonth.Checked = args.CK030Days;
            ck90Days.Checked = args.CK090Days;
            ckYear.Checked = args.CK365Days;
            ckSatellite.Checked = args.CKSatellite;
            ckTime.Checked = args.CKTime;
            foreach (object e in Enum.GetValues(typeof(enumProcessType)))
                cbCycType.Items.Add(RegionArg.GetEnumDescription(e));
            cbCycType.Text = RegionArg.GetEnumDescription(args.CycType);
        }

        private void ParseMinMaxValue(XElement seg, ref SortedDictionary<string, string> values)
        {
            if (seg == null)
                return;
            string max = string.Empty;
            string min = string.Empty;
            if (seg.Attribute(XName.Get("min")) != null)
                min = seg.Attribute(XName.Get("min")).Value;
            if (seg.Attribute(XName.Get("max")) != null)
                max = seg.Attribute(XName.Get("max")).Value;
            values.Add(min, max);
            string sLine = min.ToString() + "~" + max.ToString();
            if (!lbxValues.Items.Contains(sLine))
                lbxValues.Items.Add(sLine);
        }

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            if (_handler != null)
                _handler(GetRegionSetting());
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

        private void ckTime_CheckedChanged(object sender, EventArgs e)
        {
            panelTime.Enabled = ckTime.Checked;
            btnOK_Click(null, null);
        }

        private void CheckedChanged(object sender, EventArgs e)
        {
            btnOK_Click(null, null);
        }

        private void cbCycType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOK_Click(null, null);
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            btnOK_Click(null, null);
        }
    }

    public class RegionArg
    {
        public bool CK001Days = false;
        public bool CK007Days = false;
        public bool CK010Days = false;
        public bool CK030Days = false;
        public bool CK090Days = false;
        public bool CK365Days = false;
        public bool CKSatellite = false;
        public bool CKTime = false;
        public SortedDictionary<string, string> TimeRegion = null;
        public enumProcessType CycType = enumProcessType.AVG;

        public RegionArg()
        {

        }

        /// <summary>
        /// 获取枚举值的详细文本
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetEnumDescription(object e)
        {
            //获取字段信息
            System.Reflection.FieldInfo[] ms = e.GetType().GetFields();

            Type t = e.GetType();
            foreach (System.Reflection.FieldInfo f in ms)
            {
                //判断名称是否相等
                if (f.Name != e.ToString())
                    continue;

                //反射出自定义属性
                foreach (Attribute attr in f.GetCustomAttributes(true))
                {
                    //类型转换找到一个Description，用Description作为成员名称
                    System.ComponentModel.DescriptionAttribute dscript = attr as System.ComponentModel.DescriptionAttribute;
                    if (dscript != null)
                        return dscript.Description;
                }

            }
            //如果没有检测到合适的注释，则用默认名称
            return e.ToString();
        }
    }
}
