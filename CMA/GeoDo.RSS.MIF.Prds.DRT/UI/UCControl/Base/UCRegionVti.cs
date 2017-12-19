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

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public partial class UCRegionVti : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler;

        public UCRegionVti()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtMinValue.Text == string.Empty || txtMaxValue.Text == string.Empty)
                    return;
                float min = float.Parse(txtMinValue.Text.Trim());
                float max = float.Parse(txtMaxValue.Text.Trim());
                if (min < max)
                {
                    string sLine = min.ToString() + "~" + max.ToString();
                    if (!lbxValues.Items.Contains(sLine))
                        lbxValues.Items.Add(sLine);
                }
            }
            catch
            {
            }
            finally
            {
                txtMinValue.Text = string.Empty;
                txtMaxValue.Text = string.Empty;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lbxValues.SelectedIndices.Count < 1)
                return;
            lbxValues.Items.RemoveAt(lbxValues.SelectedIndex);
        }

        public object GetArgumentValue()
        {
            return lbxValues.Items.ToString();
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

        private SortedDictionary<float, float> GetRegionSetting()
        {
            List<string> regions = new List<string>();
            foreach (object item in lbxValues.Items)
            {
                if (item.ToString() != null)
                {
                    regions.Add(item.ToString());
                }
            }
            SortedDictionary<float, float> values = GetValues(regions);
            return values;
        }

        private SortedDictionary<float, float> GetValues(List<string> regions)
        {
            if (regions.Count() == 0)
                return null;
            SortedDictionary<float, float> values = new SortedDictionary<float, float>();
            foreach (string re in regions)
            {
                float[] minmax = ParseRegionToFloat(re);
                if (minmax == null || minmax.Length == 0)
                    continue;
                values.Add(minmax[0], minmax[1]);
            }
            return values;
        }

        private float[] ParseRegionToFloat(string re)
        {
            if (string.IsNullOrEmpty(re))
                return null;
            string[] parts = re.Split('~');
            if (parts == null || parts.Length == 0)
                return null;
            float min = float.Parse(parts[0]);
            float max = float.Parse(parts[1]);
            return new float[] { min, max };
        }

        //限定文本输入框只能输入数字、小数点和控制键
        private void txtMinValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8/*.*/ || keyValue == 46 /*退格键*/|| keyValue == 45/*-*/)
            {
                if (sender != null && sender is TextBox && keyValue == 46)
                {
                    if (((TextBox)sender).Text.IndexOf(".") >= 0)
                        e.Handled = true;
                    else
                        e.Handled = false;
                }
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            if (ele == null)
                return null;
            XElement segEle = ele.Element(XName.Get("Segements"));
            if (segEle == null)
                return null;
            IEnumerable<XElement> segs = segEle.Elements(XName.Get("Segement"));
            if (segs == null || segs.Count() == 0)
                return null;
            SortedDictionary<float, float> values = new SortedDictionary<float, float>();
            foreach (XElement seg in segs)
            {
                ParseMinMaxValue(seg, ref values);
            }
            return values.Count != 0 ? values : null;
        }

        private void ParseMinMaxValue(XElement seg, ref SortedDictionary<float, float> values)
        {
            if (seg == null)
                return;
            string value = null;
            float max = float.MinValue;
            float min = float.MaxValue;
            if (seg.Attribute(XName.Get("min")) != null)
            {
                value = seg.Attribute(XName.Get("min")).Value;
                min = float.Parse(value);
            }
            if (seg.Attribute(XName.Get("max")) != null)
            {
                value = seg.Attribute(XName.Get("max")).Value;
                max = float.Parse(value);
            }
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
    }
}
