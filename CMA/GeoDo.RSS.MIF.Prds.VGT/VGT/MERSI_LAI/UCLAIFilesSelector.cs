using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.HDF5;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    /// <summary>
    /// 待计算文件选择方式
    /// 1、输入影像，Dictionary<"INPUTFILES",string[]>;
    /// 2、输出路径及文件，返回Dictionary<"OUTPARAS",string[]>;
    /// </summary>
    public partial class UCLAIFilesSelector : UserControl, IArgumentEditorUI
    {
        private Action<object> _handle = null;
        private Dictionary<string, string[]> _result = null;

        public UCLAIFilesSelector()
        {
            InitializeComponent();
        }

        #region IArgumentEditorUI 成员
        public object GetArgumentValue()
        {
            if (_result != null)
                _result.Clear();
            else
                _result = new Dictionary<string, string[]>();
            _result.Add("INPUTFILES", new string[] { txtL2LSR.Text, txtL1hdf.Text, txtLandCover.Text });
            //List<string> outfiles = new List<string>();
            //if (cbxSR_LAI.Checked)
            //    outfiles.Add("SR_LAI");
            //if (cbxESR_LAIfile.Checked)
            //    outfiles.Add("ESR_LAI");
            //if (cbxESR_eff_LAI.Checked)
            //    outfiles.Add("ESR_eff_LAI");
            //_result.Add("OUTPARAS", outfiles.ToArray());
            return _result;
        }
        
        public void SetChangeHandler(Action<object> handler)
        {
            _handle = handler;
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement xml)
        {
            return null;
        }
        #endregion

        private bool IsSameTypeFile(string []fileNames)
        {
            bool _isFirst = true;
            string _satellite = null;
            string _sensor = null;
            DateTime _datatime = DateTime.MinValue;
            foreach (string fileName in fileNames)
            {
                RasterIdentify l2datid = new RasterIdentify(Path.GetFileName(fileName));
                if (_isFirst)
                {
                    _satellite = l2datid.Satellite;
                    _sensor = l2datid.Sensor;
                    _datatime = l2datid.OrbitDateTime;
                    _isFirst = false;
                }
                else
                {
                    if (l2datid.Satellite != _satellite || l2datid.Sensor != _sensor
                        || l2datid.OrbitDateTime != _datatime)
                        return false;
                }
            }
            return true;
        }

        private bool CheckArgsIsOK()
        {
            if (string.IsNullOrWhiteSpace(txtL2LSR.Text) || /*Path.GetExtension(txtL2LSR.Text).ToUpper() != ".HDF" ||*/
                !Path.GetFileNameWithoutExtension(txtL2LSR.Text).Contains("L2_LSR") || !File.Exists(txtL2LSR.Text))
                throw new ArgumentException("请输入正确的L2 LSR地表反射率文件，仅支持HDF格式！");
            if (!IsHDF5(txtL2LSR.Text))
                throw new ArgumentException("输入的L2 LSR地表反射率文件不是正确的HDF5文件！");
            if (string.IsNullOrWhiteSpace(txtL1hdf.Text) || /*Path.GetExtension(txtL1hdf.Text).ToUpper() != ".HDF" ||*/
                !Path.GetFileNameWithoutExtension(txtL1hdf.Text).Contains("L1") || !File.Exists(txtL1hdf.Text))
                throw new ArgumentException("请输入正确的L1轨道数据文件，仅支持HDF格式！");
            if (!IsHDF5(txtL1hdf.Text))
                throw new ArgumentException("输入的L1轨道数据文件不是正确的HDF5文件！");
            if (!IsSameTypeFile(new string[] { txtL2LSR.Text, txtL1hdf.Text }))
                throw new ArgumentException("L2 LSR地表反射率文件与L1轨道数据文件的属性信息(卫星、传感器、数据时间)不匹配，请重新选择！");
            if (string.IsNullOrWhiteSpace(txtLandCover.Text) || !File.Exists(txtLandCover.Text))
                throw new ArgumentException("请输入正确的地表覆盖类型文件！");
            return true;
        }

        private bool IsHDF5(string fileName)
        {
            byte[] header1024 = new byte[1024];
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                fs.Read(header1024, 0, 1024);
                fs.Close();
            }
            if (!HDF5Helper.IsHdf5(header1024))
                return false;
            return true;
        }

        #region 按钮事件
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                CheckArgsIsOK();
                if (_handle != null)
                    _handle(GetArgumentValue());
            }
            catch(System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void btnOpenL2LSR_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "FY3 MERSI_LSR地表反射率文件(*.Hdf)|*.hdf|所有文件(*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtL2LSR.Text = (dlg.FileName);
            }
        }

        private void btnOpenL1hdf_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "FY3 MERSI_L1轨道数据文件(*.Hdf)|*.hdf|所有文件(*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtL1hdf.Text = (dlg.FileName);
            }
        }

        private void btnOpenLandCover_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "地表覆盖类型文件(*.ldf,*.img,*.dat,*.raw)|*.ldf;*.img;*.dat;*.raw|所有文件(*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtLandCover.Text = (dlg.FileName);
            }
        }

        #endregion

        private void cbxESR_eff_LAI_CheckedChanged(object sender, EventArgs e)
        {

        }
    }

}
