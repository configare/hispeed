using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace GeoDo.RSS.CA
{
    public partial class ControlPoint : UserControl
    {
        private SortedList<int, int> _controlPoints = null;
        private Regex expPoint1 = new Regex(@"\((?<rgb0>\d*)\s*[,，](?<rgb1>\d*)\s*\)", RegexOptions.Compiled);
        private Regex expSeg1 = new Regex(@"\((?<oSegRgb0>\d*)\s*[,，](?<oSegRgb1>\d*)\s*\)\s*\-\>\s*\((?<tSegRgb0>\d*)\s*[,，](?<tSegRgb1>\d*)\s*\)", RegexOptions.Compiled);

        public event EventHandler ControlPointChanged;

        protected void OnControlPointChanged()
        {
            if (ControlPointChanged != null)
                ControlPointChanged(this, null);
        }

        public ControlPoint()
        {
            InitializeComponent();
        }

        public SortedList<int, int> ControlPoints
        {
            get { return _controlPoints; }
            set
            {
                _controlPoints = value;
                if (_controlPoints == null)
                    _controlPoints = new SortedList<int, int>();
                UpdateControlPointList();
            }
        }

        private void UpdateControlPointList()
        {
            lstControlPoints.Items.Clear();
            string controlPointItem;
            if (rdPoint.Checked)
            {
                for (int i = 0; i < _controlPoints.Count; i++)
                {
                    controlPointItem = string.Format("({0},{1})", _controlPoints.Keys[i], _controlPoints.Values[i]);
                    lstControlPoints.Items.Add(controlPointItem);
                }
            }
            else
            {
                for (int i = 0; i < _controlPoints.Count - 1; i++)
                {
                    controlPointItem = string.Format("({0},{1})->({2},{3})", _controlPoints.Keys[i], _controlPoints.Keys[i + 1], _controlPoints.Values[i], _controlPoints.Values[i + 1]);
                    lstControlPoints.Items.Add(controlPointItem);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (rdPoint.Checked)
            {
                Match m = expPoint1.Match(txtInputPoint.Text);
                if (m.Success)
                {
                    string v1 = m.Groups["rgb0"].Value;
                    string v2 = m.Groups["rgb1"].Value;
                    int rgb0, rgb1;
                    if (!int.TryParse(v1, out rgb0) || !int.TryParse(v2, out rgb1))
                        return;
                    if (rgb0 > 255 || rgb1 > 255 || rgb0 < 0 || rgb1 < 0)
                        return;
                    if (_controlPoints.ContainsKey(rgb0))
                        _controlPoints[rgb0] = rgb1;
                    else
                        _controlPoints.Add(rgb0, rgb1);
                    OnControlPointChanged();
                    return;
                }
            }
            else
            {
                Match m = expPoint1.Match(txtInputPoint.Text);
                if (m.Success)
                {
                    string o1 = m.Groups["oSegRgb0"].Value;
                    string o2 = m.Groups["oSegRgb1"].Value;
                    string t1 = m.Groups["tSegRgb0"].Value;
                    string t2 = m.Groups["tSegRgb1"].Value;
                    int orgb0, orgb1, trgb0, trgb1;
                    if (!int.TryParse(o1, out orgb0) || !int.TryParse(o2, out orgb1) ||
                        !int.TryParse(t1, out trgb0) || !int.TryParse(t2, out trgb1))
                        return;
                    if (orgb0 > 255 || orgb1 > 255 || orgb0 < 0 || orgb1 < 0 ||
                        trgb0 > 255 || trgb1 > 255 || trgb0 < 0 || trgb1 < 0)
                        return;
                    if (_controlPoints.ContainsKey(orgb0))
                        _controlPoints[orgb0] = trgb0;
                    else
                        _controlPoints.Add(orgb0, trgb0);
                    if (_controlPoints.ContainsKey(orgb1))
                        _controlPoints[orgb1] = trgb1;
                    else
                        _controlPoints.Add(orgb1, trgb1);
                    OnControlPointChanged();
                    return;
                }
            }
        }

        private void btnSub_Click(object sender, EventArgs e)
        {
            if (_controlPoints.Count <= 2)
                return;
            int index = lstControlPoints.SelectedIndex;
            if (index < 0)
                return;
            if (rdPoint.Checked)
                _controlPoints.RemoveAt(index);
            else
            {
                _controlPoints.RemoveAt(index);
                _controlPoints.RemoveAt(index + 1);
            }
            OnControlPointChanged();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (_controlPoints.Count <= 2)
                return;
            else
            {
                _controlPoints.Clear();
                _controlPoints.Add(0, 0);
                _controlPoints.Add(255, 255);
            }
            OnControlPointChanged();
        }

        private void rdPoint_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlPointList();
            lblTip.Text = "(oRgb,tRgb)";
        }

        private void rdSegment_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlPointList();
            lblTip.Text = "(oBegin,oEnd)->(tBegin,tEnd)";
        }
    }
}
