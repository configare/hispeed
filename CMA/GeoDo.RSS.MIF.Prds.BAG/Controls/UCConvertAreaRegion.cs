using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Linq;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public partial class UCConvertAreaRegion : UserControl, IArgumentEditorUI
    {
        private Action<object> _handler;

        public UCConvertAreaRegion()
        {
            InitializeComponent();
        }

        public string[] GetConvertArea()
        {
            if (lsbAreaRegion.Items.Count < 1)
                return null;
            List<string> rets = new List<string>();
            foreach (string sLine in lsbAreaRegion.Items)
                rets.Add(sLine);
            return rets.ToArray();
        }

        public object GetArgumentValue()
        {
            return lsbAreaRegion.Items.ToString();
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetConvertArea());
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string exp = @"^\d+(\.\d+)?$";
            if (!Regex.IsMatch(txtMaxConvertArea.Text, exp))
                txtMaxConvertArea.Text = string.Empty;
            if (!Regex.IsMatch(txtMinConvertArea.Text, exp))
                txtMinConvertArea.Text = string.Empty;
            if (txtMinConvertArea.Text == string.Empty || txtMaxConvertArea.Text == string.Empty)
                return;
            try
            {
                float min = float.Parse(txtMinConvertArea.Text);
                float max = float.Parse(txtMaxConvertArea.Text);
                string sLine = min.ToString() + "~" + max.ToString();
                if (!lsbAreaRegion.Items.Contains(sLine))
                    lsbAreaRegion.Items.Add(sLine);
            }
            catch
            {
            }
            txtMinConvertArea.Text = string.Empty;
            txtMaxConvertArea.Text = string.Empty;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lsbAreaRegion.SelectedIndices.Count < 1)
                return;
            lsbAreaRegion.Items.RemoveAt(lsbAreaRegion.SelectedIndex);
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            IEnumerable<XElement> node = ele.Elements("ValueItem");
            List<string> areaRegion = new List<string>();
            if (node != null && node.Count()!=0)
            {
                foreach (XElement item in node)
                {
                    string value = item.Attribute("value").Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        areaRegion.Add(value);
                        lsbAreaRegion.Items.Add(value);
                    }
                }
                return areaRegion.ToArray();
            }
            return null;
        }
    }
}
