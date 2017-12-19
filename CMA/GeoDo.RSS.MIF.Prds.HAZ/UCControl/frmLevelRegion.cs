using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.HAZ
{
    public partial class frmLevelRegion : Form
    {
        string _argFileName;


        public frmLevelRegion(string argFileName)
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(argFileName))
                _argFileName = AppDomain.CurrentDomain.BaseDirectory + "/SystemData/ProductArgs/Segments/AODLevelDef.txt";
            else
                _argFileName = argFileName;
            InitControls();
        }

        private void InitControls()
        {
            if (string.IsNullOrEmpty(_argFileName))
                return;
            if (!File.Exists(_argFileName))
                return;
            string[] lines = File.ReadAllLines(_argFileName);
            string subItemStr;
            foreach (string item in lines)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    if (CheckIsArg(item, out subItemStr))
                        lbxValues.Items.Add(subItemStr);
                }
            }
        }

        private bool CheckIsArg(string argLine, out string subItemStr)
        {
            subItemStr = argLine;
            if (string.IsNullOrEmpty(argLine))
                return false;
            if (!argLine.Contains("~"))
                return false;
            string[] parts = argLine.Split(new char[] { '~', ' ' });
            if (parts == null || parts.Length != 3)
                return false;
            float min, max, level;
            if (float.TryParse(parts[0], out min) && float.TryParse(parts[1], out max) && float.TryParse(parts[2], out level))
            {
                if (MathCompare.FloatCompare(min, float.MinValue))
                    subItemStr = "最小" + '~' + parts[1] + " " + level;
                else if (MathCompare.FloatCompare(max, float.MaxValue))
                    subItemStr = parts[0] + '~' + "最大" + " " + level;
                return true;
            }
            return false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SortedDictionary<float, float[]> regionList = GetRegionSetting();
            if (regionList != null && regionList.Count > 0)
                SaveArgToTxt(regionList);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void SaveArgToTxt(SortedDictionary<float, float[]> regionList)
        {
            if (string.IsNullOrEmpty(_argFileName))
                return;
            string path = Path.GetDirectoryName(_argFileName);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            using (StreamWriter sw = new StreamWriter(_argFileName, false, Encoding.Default))
            {
                foreach (float item in regionList.Keys)
                {
                    sw.WriteLine(item.ToString() + "~" + regionList[item][0].ToString() + " " + regionList[item][1].ToString());
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if ((txtMinValue.Text == string.Empty && txtMaxValue.Text == string.Empty) || string.IsNullOrEmpty(txtLevel.Text))
                    return;
                float min = float.MinValue;
                float max = float.MaxValue;
                float level = 1;
                if (!string.IsNullOrEmpty(txtMinValue.Text))
                    min = float.Parse(txtMinValue.Text.Trim());
                if (!string.IsNullOrEmpty(txtMaxValue.Text))
                    max = float.Parse(txtMaxValue.Text.Trim());
                level = float.Parse(txtLevel.Text.Trim());
                if (min < max)
                {
                    string sLine = (min == float.MinValue ? "最小" : min.ToString()) + "~" + (max == float.MaxValue ? "最大" : max.ToString()) + " " + level.ToString();
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
                txtLevel.Text = string.Empty;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lbxValues.SelectedIndices.Count < 1)
                return;
            while (lbxValues.SelectedIndices.Count > 0)
            {
                lbxValues.Items.RemoveAt(lbxValues.SelectedIndices[0]);
            }
        }

        private SortedDictionary<float, float[]> GetRegionSetting()
        {
            List<string> regions = new List<string>();
            foreach (object item in lbxValues.Items)
            {
                if (item.ToString() != null)
                {
                    regions.Add(item.ToString());
                }
            }
            SortedDictionary<float, float[]> values = GetValues(regions);
            return values;
        }

        private void txt_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtLevel_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8/*.*/ || keyValue == 46 /*退格键*/|| keyValue == 45/*-*/)
            {
                if (keyValue == 46)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private SortedDictionary<float, float[]> GetValues(List<string> regions)
        {
            if (regions.Count() == 0)
                return null;
            SortedDictionary<float, float[]> values = new SortedDictionary<float, float[]>();
            foreach (string re in regions)
            {
                float[] minmax = ParseRegionToFloat(re);
                if (minmax == null || minmax.Length == 0)
                    continue;
                values.Add(minmax[0], new float[] { minmax[1], minmax[2] });
            }
            return values;
        }

        private float[] ParseRegionToFloat(string re)
        {
            if (string.IsNullOrEmpty(re))
                return null;
            string[] parts = re.Split(new char[] { '~', ' ' });
            if (parts == null || parts.Length == 0)
                return null;
            float min = parts[0] == "最小" ? float.MinValue : float.Parse(parts[0]);
            float max = parts[1] == "最大" ? float.MaxValue : float.Parse(parts[1]);
            float level = float.Parse(parts[2]);
            return new float[] { min, max, level };
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
