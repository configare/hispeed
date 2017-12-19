using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.UHE
{
    public partial class frmTemperatureRegion : Form
    {
        string _argFileName;


        public frmTemperatureRegion(string argFileName)
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(argFileName))
                _argFileName = AppDomain.CurrentDomain.BaseDirectory + "/SystemData/ProductArgs/Segments/ANMIStat.txt";
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
            string[] lines=File.ReadAllLines(_argFileName);
            foreach(string item in lines)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    if (CheckIsArg(item))
                        lbxValues.Items.Add(item);
                }
            }
        }

        private bool CheckIsArg(string argLine)
        {
            if (string.IsNullOrEmpty(argLine))
                return false;
            if (!argLine.Contains("~"))
                return false;
            string[] parts = argLine.Split('~');
            if (parts == null || parts.Length != 2)
                return false;
            float min, max;
            if (float.TryParse(parts[0], out min) && float.TryParse(parts[1], out max))
                return true;
            return false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SortedDictionary<float, float> regionList = GetRegionSetting();
            if (regionList != null && regionList.Count > 0)
                SaveArgToTxt(regionList);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void SaveArgToTxt(SortedDictionary<float, float> regionList)
        {
            if (string.IsNullOrEmpty(_argFileName))
                return;
            if (!Directory.Exists(Path.GetDirectoryName(_argFileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_argFileName));
            }
            using (StreamWriter sw = new StreamWriter(_argFileName,false, Encoding.Default))
            {
                foreach (float item in regionList.Keys)
                {
                    sw.WriteLine(item + "~" + regionList[item]);
                }
            }
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
            while (lbxValues.SelectedIndices.Count > 0)
            {
                lbxValues.Items.RemoveAt(lbxValues.SelectedIndices[0]);
            }
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

        private void txtMaxValue_KeyPress(object sender, KeyPressEventArgs e)
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
