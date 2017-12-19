using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Reflection;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    public partial class DensitySliceForm : Form
    {
        public delegate void UpdateDensSlice(int band, ColorMapTable<double> colorMap);
        public event UpdateDensSlice UpdateDensSliceEvent;
        private int _devide_num = 8;
        private int _selectBand = 0;
        private float _maxValue = 0;
        private float _minValue = 0;
        private float _interval = 0;
        bool _isInit = true;
        List<DensityRange> _rangeList;
        private float _min_value = 0;
        private float _max_value = 0;
        private IRasterDrawing _drawing = null;
        private IProgressMonitor _progress = null;
        private ColorMapTable<double> _mapColor = null;
        private IntervalType _intervalType = IntervalType.IntType;
        private bool _isOpenXML = false;
        private List<IRgbProcessor> _oRgbProcessors = null;
        private int[] _oSelectBands = null;

        public DensitySliceForm(IRasterDrawing rd)
        {
            InitializeComponent();
            _drawing = rd;
            if (_drawing.SelectedBandNos.Length == 1)
                _selectBand = _drawing.SelectedBandNos[0];
            _oSelectBands = _drawing.SelectedBandNos;
            if (rd.RgbProcessorStack != null && rd.RgbProcessorStack.Processors != null)
            {
                _oRgbProcessors = new List<IRgbProcessor>();
                foreach (IRgbProcessor processor in rd.RgbProcessorStack.Processors)
                    _oRgbProcessors.Add(processor);
                _oRgbProcessors.Reverse();
            }
            InitForm();
        }

        public IProgressMonitor Progress
        {
            set { _progress = value; }
        }

        public ColorMapTable<double> ColorTable
        {
            get { return _mapColor; }
        }

        public int SelectBand
        {
            get { return _selectBand; }
            set { _selectBand = value; }
        }


        public IntervalType IntervalType
        {
            set { _intervalType = value; }
        }

        private void InitForm()
        {
            //InitAttribute
            //InitTreeView
            tvFileInfo.ImageList = imglist;
            TreeNode root = new TreeNode(_drawing.FileName);
            root.ImageIndex = 0;
            TreeNode selectBandNode = null;
            for (int k = 0; k < _drawing.BandCount; k++)
            {
                TreeNode tr = new TreeNode(string.Format("波段　{0}", k + 1));
                tr.ImageIndex = 1;
                //tr.SelectedImageIndex = 1;
                if (_selectBand == k + 1)
                    selectBandNode = tr;
                root.Nodes.Add(tr);
            }
            this.tvFileInfo.Nodes.Add(root);
            this.tvFileInfo.ExpandAll();
            this.tvFileInfo.SelectedNode = selectBandNode;
        }

        private Color[] colorList;

        private void ConstructColorList()
        {
            if (colorList != null)
                return;
            string[] cList = new string[]{
            "#973302","#343200","#013300","#003466","#000083","#313398","#333333","#810004",
            "#FD6802","#858200","#008002","#008081","#0201FF","#69669D","#80807E","#FE0002","#FE9B00",
            "#9ACB00","#339A65","#33CBCC","#3C62FF","#780179","#99999B","#FF00FE","#FFCB03","#FFFE01",
            "#00FF01","#01FFFF","#00CCFF","#993365","#C0C0C0","#FF99CB","#FFCA9B","#FFFE99","#CDFFCC",
            "#CDFFFF","#99CDFD","#C89CFB","#FFFFFF", "#000000"
                };
            colorList = new Color[cList.Length];
            for (int i = 0; i < colorList.Length; i++)
            {
                colorList[i] = ColorTranslator.FromHtml(cList[i].ToString());
            }
        }

        private List<DensityRange> InitRange(int range)
        {
            ConstructColorList();
            if (ckInterval.Checked)
            {
                if (StringToNumberHelper.isFloatPointNumber(txtInterval.Text))
                    _intervalType = IntervalType.FloatType;
                else
                    _intervalType = IntervalType.IntType;
            }
            else
            {
                _interval = (float)Math.Round((_maxValue - _minValue) / (float)numSliceRange.Value, 2);
            }
            if (_intervalType == IntervalType.IntType)
                _interval = (int)Math.Ceiling(_interval);
            List<DensityRange> m_RangeList = new List<DensityRange>();
            for (int i = 0; i < range; i++)
            {
                int color_idx = i > colorList.Length - 1 ? i % (colorList.Length - 1) : i;
                Color rangeColor = colorList[color_idx];
                DensityRange r = (i == 0) ?
                    new DensityRange(_minValue + (i * _interval), _minValue + ((i + 1) * _interval), rangeColor.R, rangeColor.G, rangeColor.B) :
                    new DensityRange(_minValue + (i * _interval), _minValue + ((i + 1) * _interval), rangeColor.R, rangeColor.G, rangeColor.B);
                m_RangeList.Add(r);
            }
            return m_RangeList;
        }

        private List<DensityRange> InitRange()
        {
            List<DensityRange> m_RangeList = new List<DensityRange>();

            float length = _max_value - _min_value;
            float step = length / _devide_num;
            if (_intervalType == IntervalType.IntType)
                step = (int)Math.Ceiling(step);

            for (int i = 0; i < _devide_num; i++)
            {
                switch (i)
                {
                    case 0:
                        DensityRange range0 = new DensityRange(_min_value + (i * step), _min_value + ((i + 1) * step) - 1, 255, 0, 0);//red
                        m_RangeList.Add(range0);
                        break;
                    case 1:
                        DensityRange range1 = new DensityRange(_min_value + (i * step), _min_value + ((i + 1) * step) - 1, 0, 255, 0);//green
                        m_RangeList.Add(range1);
                        break;
                    case 2:
                        DensityRange range2 = new DensityRange(_min_value + (i * step), _min_value + ((i + 1) * step) - 1, 0, 0, 255);//blue
                        m_RangeList.Add(range2);
                        break;
                    case 3:
                        DensityRange range3 = new DensityRange(_min_value + (i * step), _min_value + ((i + 1) * step) - 1, 255, 255, 0);//yellow
                        m_RangeList.Add(range3);
                        break;
                    case 4:
                        DensityRange range4 = new DensityRange(_min_value + (i * step), _min_value + ((i + 1) * step) - 1, 0, 255, 255);//cyan
                        m_RangeList.Add(range4);
                        break;
                    case 5:
                        DensityRange range5 = new DensityRange(_min_value + (i * step), _min_value + ((i + 1) * step) - 1, 255, 0, 255);//magenta
                        m_RangeList.Add(range5);
                        break;
                    case 6:
                        DensityRange range6 = new DensityRange(_min_value + (i * step), _min_value + ((i + 1) * step) - 1, 176, 48, 96);//maroon
                        m_RangeList.Add(range6);
                        break;
                    case 7:
                        DensityRange range7 = new DensityRange(_min_value + (i * step), _min_value + ((i + 1) * step), 46, 139, 87);//sea green
                        m_RangeList.Add(range7);
                        break;
                    default:
                        break;

                }

            }

            return m_RangeList;
        }

        private void tvFileInfo_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Nodes.Count == 0)
            {
                _isInit = true;
                _selectBand = e.Node.Index + 1;
                if (_drawing != null)
                {
                    CalcMinMax();
                    if (_intervalType == IntervalType.IntType)
                        txtInterval.Text = Convert.ToString(Math.Ceiling((_max_value - _min_value) / (float)numSliceRange.Value));
                    else
                        txtInterval.Text = Convert.ToString(Math.Round((_maxValue - _minValue) / (float)numSliceRange.Value, 2));
                    numSliceRange.Value = numSliceRange.Value;
                }
                this.groupBox1.Enabled = true;
                this.panel1.Enabled = true;
                this.btnApply.Enabled = true;
                this.btnOk.Enabled = true;
                _isInit = false;
            }
        }

        private void CalcMinMax()
        {
            double pdfMin = 0;
            double pdfMax = 0;
            try
            {
                _progress.Reset("正在统计端值...", 100);
                _progress.Start(false);
                IRasterBand band = _drawing.DataProviderCopy.GetRasterBand(_selectBand);
                band.ComputeMinMax(out pdfMin, out pdfMax, false,
                    (idx, tip) => { _progress.Boost(idx); });
            }
            finally
            {
                _progress.Finish();
            }
            this.txtMin.Text = Math.Round(pdfMin,2).ToString();
            this.txtMax.Text = Math.Round(pdfMax,2).ToString();
            if (StringToNumberHelper.isFloatPointNumber(txtMin.Text) || StringToNumberHelper.isFloatPointNumber(txtMax.Text))
                _intervalType = IntervalType.FloatType;
            else
                _intervalType = IntervalType.IntType;
            _min_value = (int)pdfMin;
            _max_value = (int)pdfMax;
            _minValue = (float)Math.Round(pdfMin, 2);
            _maxValue = (float)Math.Round(pdfMax, 2);
        }

        private string _densityXML;
        private float _lwMinValue = 0;
        private float _lwInterval = 0;

        public void InitByArugment(string densityXML, int selectBand, float minValue, float intreval)
        {
            _lwMinValue = minValue;
            _lwInterval = intreval;
            GetTreeNodeAndSelect(selectBand);
            if (_drawing != null)
                CalcMinMax();
            _densityXML = densityXML;
            tsmiOpenFile_Click(tsmiOpenFile, null);
        }

        private void GetTreeNodeAndSelect(int lwBand)
        {
            if (tvFileInfo.Nodes[0].Nodes == null || tvFileInfo.Nodes[0].Nodes.Count == 0)
                return;
            _selectBand = lwBand;
            foreach (TreeNode item in tvFileInfo.Nodes[0].Nodes)
            {
                if (item.Text == "波段　" + lwBand)
                    tvFileInfo.SelectedNode = item;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lstDensityRange.SelectedIndex >= 0)
            {
                DensityRange dr = _rangeList[lstDensityRange.SelectedIndex];
                GetMinMaxValue(ref dr);

                EditRange temp_editrange = new EditRange(dr);

                string list_text;
                temp_editrange.StartPosition = FormStartPosition.Manual;
                temp_editrange.Location = new Point(this.Location.X + this.Width, this.Location.Y);
                if (temp_editrange.ShowDialog(this) == DialogResult.OK)
                {
                    DensityRange temp_range = new DensityRange(temp_editrange.Edit_min, temp_editrange.Edit_max, temp_editrange.Red, temp_editrange.Green,
                    temp_editrange.Blue);

                    if (lstDensityRange.Items.Count > _rangeList.Count)
                    {
                        _rangeList.Add(temp_range);
                    }
                    else
                        _rangeList[lstDensityRange.SelectedIndex] = temp_range;

                    list_text = temp_range.ToString();
                    lstDensityRange.Items[lstDensityRange.SelectedIndex] = list_text;
                }
            }
        }

        private void GetMinMaxValue(ref DensityRange dr)
        {
            if (lstDensityRange.SelectedIndex - 1 < 0)
                dr.minValue = _rangeList[lstDensityRange.SelectedIndex].minValue;
            else
                dr.minValue = _rangeList[lstDensityRange.SelectedIndex - 1].maxValue;
            if (lstDensityRange.SelectedIndex + 1 >= lstDensityRange.Items.Count)
                dr.maxValue = _rangeList[lstDensityRange.SelectedIndex].maxValue;
            else
                dr.maxValue = _rangeList[lstDensityRange.SelectedIndex + 1].minValue;

        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstDensityRange.SelectedIndex > -1)
            {
                int temp_index = lstDensityRange.SelectedIndex;
                lstDensityRange.Items.Remove(lstDensityRange.SelectedItem);
                _rangeList.RemoveAt(temp_index);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (this._rangeList != null && this._rangeList.Count != 0)
            {
                _rangeList.Clear();
                this.lstDensityRange.Items.Clear();
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (_rangeList == null || _rangeList.Count == 0)
            {
                MessageBox.Show("请设置密度分割范围！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (_rangeList != null)
            {
                if (UpdateDensSliceEvent != null)
                {
                    _mapColor = DensitySolution.GetColorMapTable(_rangeList.ToArray());
                    UpdateDensSliceEvent(_selectBand, _mapColor);
                }
            }
        }

        private void AddDefaultRange_Click(object sender, EventArgs e)
        {
            if (_selectBand == 0)
                return;
            List<DensityRange> rangelist = InitRange();
            _rangeList = rangelist;
            this.lstDensityRange.Items.Clear();
            foreach (DensityRange r in rangelist)
            {
                lstDensityRange.Items.Add(r.ToString());
            }
        }

        private void AddDensityRange_Click(object sender, EventArgs e)
        {
            EditRange temp_editrange = new EditRange();
            if (temp_editrange.ShowDialog(this) == DialogResult.OK)
            {
                DensityRange temp_range = new DensityRange(temp_editrange.Edit_min, temp_editrange.Edit_max, temp_editrange.Red, temp_editrange.Green,
                temp_editrange.Blue);
                int index = 0;
                if (_rangeList == null)
                    return;
                foreach (DensityRange r in _rangeList)
                {
                    if (temp_range.minValue < r.minValue)
                    {
                        break;
                    }
                    index++;
                }

                lstDensityRange.Items.Insert(index, temp_range.ToString());


                if (lstDensityRange.Items.Count > _rangeList.Count)
                {
                    _rangeList.Insert(index, temp_range);
                }
                else
                    _rangeList[index] = temp_range;
            }

        }

        private void btnComputeRange_Click(object sender, EventArgs e)
        {
            if (_selectBand == 0 || (_minValue ==_maxValue))
                return;
            float min, max = 0;
            if (!float.TryParse(txtMin.Text, out min))
            {
                txtMin.Focus();
                txtMin.SelectAll();
                MessageBox.Show("密度分割最小值输入错误，请重新输入！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (!float.TryParse(txtMax.Text, out max))
            {
                txtMax.Focus();
                txtMax.SelectAll();
                MessageBox.Show("密度分割最大值输入错误，请重新输入！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (min>_minValue)
            {
                txtMin.Focus();
                txtMin.SelectAll();
                MessageBox.Show("密度分割最小值输入错误，请重新输入！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (max<_maxValue)
            {
                txtMax.Focus();
                txtMax.SelectAll();
                MessageBox.Show("密度分割最大值输入错误，请重新输入！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            List<DensityRange> rangelist = InitRange((int)numSliceRange.Value);//InitRange();
            _rangeList = rangelist;
            this.lstDensityRange.Items.Clear();
            foreach (DensityRange r in rangelist)
            {
                this.lstDensityRange.Items.Add(r.ToString());
            }
        }

        private void tsmiOpenFile_Click(object sender, EventArgs e)
        {
            lstDensityRange.Items.Clear();
            if (_rangeList != null)
            {
                _rangeList.Clear();
            }
            else
                _rangeList = new List<DensityRange>();
            string filename = null;
            if (e != null)
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "密度分割方案(*.xml)|*.xml";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    filename = dlg.FileName;
                }
                else
                    return;
            }
            else
                filename = _densityXML;
            DensityDef densityDef = DensitySolution.Read(filename);
            if (densityDef == null)
                return;
            txtMax.Text = densityDef.MaxValue.ToString();
            txtMin.Text = densityDef.MinValue.ToString();
            _isOpenXML = true;
            txtInterval.Text = densityDef.Interval.ToString();
            _isOpenXML = false;
            ckInterval.Checked = Convert.ToBoolean(densityDef.ApplayInterval.ToString());
            int length = densityDef.Ranges == null || densityDef.Ranges.Length == 0 ? 0 : densityDef.Ranges.Length;
            DensityRange temprange;
            for (int i = 0; i < length; i++)
            {
                if (e != null)
                {
                    temprange.minValue = densityDef.Ranges[i].minValue;
                    temprange.maxValue = densityDef.Ranges[i].maxValue;
                }
                else
                {
                    temprange.minValue = i == 0 ? _min_value : (_lwMinValue == 0 ? _min_value : _lwMinValue) + _lwInterval * (i - 1);
                    temprange.maxValue = i + 1 == length ? _max_value : (_lwMinValue == 0 ? _min_value : _lwMinValue) + _lwInterval * i;
                }
                temprange.RGB_r = densityDef.Ranges[i].RGB_r;
                temprange.RGB_g = densityDef.Ranges[i].RGB_g;
                temprange.RGB_b = densityDef.Ranges[i].RGB_b;
                _rangeList.Add(temprange);
            }

            for (int i = 0; i < _rangeList.Count; i++)
            {
                lstDensityRange.Items.Add(_rangeList[i].ToString());
            }
        }

        private void tsmiSaveFile_Click(object sender, EventArgs e)
        {
            if (_rangeList == null || _rangeList.Count == 0)
            {
                MessageBox.Show("请先设置密度分割范围,然后保存方案！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string filename = null;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML文件|*.xml|所有文件|*.*";
            saveFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\\SesitySliceForm\";
            if (!Directory.Exists(saveFileDialog.InitialDirectory))
                Directory.CreateDirectory(saveFileDialog.InitialDirectory);
            saveFileDialog.FileName = CreateFileName();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                filename = saveFileDialog.FileName;
            }
            else
                return;
            DensityDef densityDef = new DensityDef();
            densityDef.MaxValue = _maxValue;
            densityDef.MinValue = _minValue;
            densityDef.Interval = _interval;
            densityDef.RangeCount = _rangeList.Count;
            densityDef.ApplayInterval = ckInterval.Checked;
            List<DensityRange> ranges = new List<DensityRange>();
            for (int i = 0; i < _rangeList.Count; i++)
            {
                DensityRange tempRange = new DensityRange();
                tempRange.minValue = _rangeList[i].minValue;
                tempRange.maxValue = _rangeList[i].maxValue;
                tempRange.RGB_r = _rangeList[i].RGB_r;
                tempRange.RGB_g = _rangeList[i].RGB_g;
                tempRange.RGB_b = _rangeList[i].RGB_b;
                ranges.Add(tempRange);
            }
            densityDef.Ranges = ranges.ToArray();
            DensitySolution.Save(filename, densityDef);
        }

        private string CreateFileName()
        {
            string filename;
            RasterIdentify rs = new RasterIdentify(_drawing.FileName);
            filename = rs.Satellite + "_" + rs.Sensor + "_Min" + _minValue + "_Max" + _maxValue + "_" + tvFileInfo.SelectedNode.Text.Replace("　", "");
            return filename;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //恢复原始影像显示
            _drawing.SelectedBandNos = _oSelectBands;
            if (_oRgbProcessors != null && _oRgbProcessors.Count > 0)
            {
                _drawing.RgbProcessorStack.Clear();
                foreach (IRgbProcessor processor in _oRgbProcessors)
                    _drawing.RgbProcessorStack.Process(processor);
            }
            Close();
        }

        private void lstDensityRange_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            btnEdit_Click(sender, e);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            btnApply_Click(sender, e);
            this.Close();
        }

        //private void txtInterval_TextChanged(object sender, EventArgs e)
        //{
        //    //if (!_isOpenXML)
        //    //    CalcSliceRange();
        //}

        private void CalcSliceRange()
        {
            if (_isInit)
                return;
            if (!CheckMaxMinValue())
                return;
            if (!CheckInterval())
                return;
            if (StringToNumberHelper.isFloatPointNumber(txtInterval.Text))
                IntervalType = IntervalType.FloatType;
            else 
                IntervalType = IntervalType.IntType;
            float numValue = (float)(_maxValue - _minValue) / _interval;
            numSliceRange.Value = (decimal)Math.Ceiling(numValue <= 0 ? 1 :
                numValue > (float)numSliceRange.Maximum ? numSliceRange.Maximum : (decimal)numValue);
        }

        private bool CheckInterval()
        {
            string errorInfo = "请检查间隔值设置：";
            if (string.IsNullOrEmpty(txtInterval.Text))
            {
                MsgBox.ShowInfo(errorInfo + "是否为空!");
                return false;
            }
            if (!float.TryParse(txtInterval.Text, out _interval))
            {
                MsgBox.ShowInfo(errorInfo + "填写信息是否正确!");
                return false;
            }
            if (_interval <= 0)
            {
                MsgBox.ShowInfo(errorInfo + "间隔值不可以为0或负数!");
                return false;
            }
            return true;
        }

        private bool CheckMaxMinValue()
        {
            string errorInfo = "请检查最大、最小值设置：";
            if (string.IsNullOrEmpty(txtMax.Text) || string.IsNullOrEmpty(txtMin.Text))
            {
                MsgBox.ShowInfo(errorInfo + "是否为空!");
                return false;
            }
            if (!float.TryParse(txtMax.Text, out _maxValue) || !float.TryParse(txtMin.Text, out _minValue))
            {
                MsgBox.ShowInfo(errorInfo + "填写信息是否正确!");
                return false;
            }
            return true;
        }

        private void textBoxMin_TextChanged(object sender, EventArgs e)
        {
            CalcSliceRange();
        }

        private void textBoxMax_TextChanged(object sender, EventArgs e)
        {
            CalcSliceRange();
        }

        private void numSliceRange_ValueChanged(object sender, EventArgs e)
        {
            CalcJianGe();
        }

        private void CalcJianGe()
        {
            if (ckInterval.Checked)
                return;
            if (_isInit)
                return;
            if (!CheckMaxMinValue())
                return;
            if (_intervalType == IntervalType.IntType)
                txtInterval.Text = Convert.ToString(Math.Ceiling((_maxValue - _minValue) / (float)numSliceRange.Value));
            else
                txtInterval.Text = Convert.ToString((_maxValue - _minValue) / (float)numSliceRange.Value);
        }

        private void txtInterval_Leave(object sender, EventArgs e)
        {
            if (!_isOpenXML)
                CalcSliceRange();
        }

        private void txtMax_Leave(object sender, EventArgs e)
        {
            if (StringToNumberHelper.isFloatPointNumber(txtMax.Text))
                _intervalType = IntervalType.FloatType;
        }
   
    }


}
