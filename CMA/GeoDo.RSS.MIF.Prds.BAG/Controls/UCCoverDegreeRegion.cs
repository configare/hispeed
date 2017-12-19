using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public partial class UCCoverDegreeRegion : UserControl,IArgumentEditorUI
    {
        private Action<object> _handler;
        public UCCoverDegreeRegion()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string exp = @"^\d+\.\d+$";
            if (txtMaxConvertDegree.Text != "0" && txtMaxConvertDegree.Text != "1")
                if (!Regex.IsMatch(txtMaxConvertDegree.Text, exp))
                    txtMaxConvertDegree.Text = string.Empty;
            if (txtMinConvertDegree.Text != "0" && txtMinConvertDegree.Text != "1")
                if (!Regex.IsMatch(txtMinConvertDegree.Text, exp))
                    txtMinConvertDegree.Text = string.Empty;
            if (txtMinConvertDegree.Text == string.Empty || txtMaxConvertDegree.Text == string.Empty)
                return;
            try
            {
                float min = float.Parse(txtMinConvertDegree.Text) * 100;
                float max = float.Parse(txtMaxConvertDegree.Text) * 100;
                if (min >= 0 && min <= 100)
                    if (max >= 0 && max <= 100)
                    {
                        string sLine = min.ToString() + "%~" + max.ToString() + "%";
                        if (!lsbDegreeRegion.Items.Contains(sLine))
                            lsbDegreeRegion.Items.Add(sLine);
                    }
            }
            catch
            {
            }
            txtMinConvertDegree.Text = string.Empty;
            txtMaxConvertDegree.Text = string.Empty;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lsbDegreeRegion.SelectedIndices.Count < 1)
                return;
            lsbDegreeRegion.Items.RemoveAt(lsbDegreeRegion.SelectedIndex);
        }

        public object GetArgumentValue()
        {
            return lsbDegreeRegion.Items.ToString();
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

        private List<string> GetRegionSetting()
        {
            List<string> regions=new List<string>();
            foreach (object item in lsbDegreeRegion.Items)
            {
                if (item.ToString() != null)
                {
                    regions.Add(item.ToString());
                }
            }
            return regions;
        }

        public object ParseArgumentValue(XElement ele)
        {
            //<DefaultValue>
            //<ValueItem value="0%~30%"/>
            //</DefaultValue>
            IEnumerable<XElement> node = ele.Elements("ValueItem");
            List<string> degreeRegion = new List<string>();
            if (node != null && node.Count() != 0)
            {
                foreach (XElement item in node)
                {
                    string value=item.Attribute("value").Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        degreeRegion.Add(value);
                        lsbDegreeRegion.Items.Add(value);
                    }
                }
                if (_handler != null)
                    _handler(degreeRegion);
                return degreeRegion;
            }
            return null;
        }
    }
}
