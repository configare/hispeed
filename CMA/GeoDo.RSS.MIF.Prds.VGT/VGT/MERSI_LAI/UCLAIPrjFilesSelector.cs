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
    public partial class UCLAIPrjFilesSelector : UserControl, IArgumentEditorUI
    {
        private Action<object> _handle = null;
        private Dictionary<string, string[]> _result = null;
        private string _saafile, _szafile, _vaafile, _vzafile;

        public UCLAIPrjFilesSelector()
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
            List<string> infiles = new List<string>();
            infiles.Add(txtL2LSR.Text);
            infiles.Add(_szafile);
            infiles.Add(_saafile);
            infiles.Add(_vzafile);
            infiles.Add(_vaafile);
            infiles.Add(txtCloudMask.Text);
            infiles.Add(txtLandCover.Text);
            _result.Add("INPUTFILES", infiles.ToArray());
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
            if (string.IsNullOrWhiteSpace(txtL2LSR.Text) || !File.Exists(txtL2LSR.Text))
                throw new ArgumentException("请输入正确的LSR地表反射率投影文件！");
            if (!File.Exists(_szafile))
                throw new ArgumentException("太阳天顶角数据投影文件:" + _szafile+"不存在！");
            if (!File.Exists(_saafile))
                throw new ArgumentException("太阳方位角数据投影文件:" + _saafile + "不存在！");
            if (!File.Exists(_vzafile))
                throw new ArgumentException("卫星天顶角数据投影文件:" + _vzafile + "不存在！");
            if (!File.Exists(_vaafile))
                throw new ArgumentException("卫星方位角数据投影文件:" + _vaafile + "不存在！");
            if (string.IsNullOrWhiteSpace(txtCloudMask.Text) ||!File.Exists(txtCloudMask.Text)||!Path.GetFileNameWithoutExtension(txtCloudMask.Text).Contains("L2_CLM"))
                throw new ArgumentException("请输入正确的云检测数据投影文件！");
            if (!IsSameTypeFile(new string[] { txtL2LSR.Text,txtCloudMask.Text }))
                throw new ArgumentException("输入的LSR地表反射率、云检测投影文件的属性信息(卫星、传感器、数据时间)不匹配，请重新选择！");
            if (string.IsNullOrWhiteSpace(txtLandCover.Text) || !File.Exists(txtLandCover.Text))
                throw new ArgumentException("请输入正确的地表覆盖类型文件！");
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
            dlg.Filter = "地表反射率投影文件(*.ldf,*.img,*.dat,*.raw)|*.ldf;*.img;*.dat;*.raw|所有文件(*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtL2LSR.Text = (dlg.FileName);
                GetAnglesFilesName(dlg.FileName);
            }
        }

        private void GetAnglesFilesName(string l2lsrFname)
        {
            txtAngles.Text = Path.GetDirectoryName(l2lsrFname);
            //FY3A_MERSI_GBAL_GLL_L1_20140611_0220_1000M.ldf
            //FY3A_MERSI_GBAL_GLL_L1_20140611_0220_1000M.SolarZenith/SolarAzimuth/SensorZenith/SensorAzimuth.ldf
            _szafile = Path.Combine(txtAngles.Text, Path.GetFileNameWithoutExtension(l2lsrFname) + ".SolarZenith" + Path.GetExtension(l2lsrFname));
            _saafile = Path.Combine(txtAngles.Text, Path.GetFileNameWithoutExtension(l2lsrFname) + ".SolarAzimuth" + Path.GetExtension(l2lsrFname));
            _vzafile = Path.Combine(txtAngles.Text, Path.GetFileNameWithoutExtension(l2lsrFname) + ".SensorZenith" + Path.GetExtension(l2lsrFname));
            _vaafile = Path.Combine(txtAngles.Text, Path.GetFileNameWithoutExtension(l2lsrFname) + ".SensorAzimuth" + Path.GetExtension(l2lsrFname));
        }

        private void btnOpenL1hdf_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "L1角度数据投影文件(*.ldf,*.img,*.dat,*.raw)|*.ldf;*.img;*.dat;*.raw|所有文件(*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtAngles.Text = (dlg.FileName);
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

        private void btnOpenCldMaskPrj_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "云检测数据投影文件(*.ldf,*.img,*.dat,*.raw)|*.ldf;*.img;*.dat;*.raw|所有文件(*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtCloudMask.Text = (dlg.FileName);
            }
        }
    }

}
