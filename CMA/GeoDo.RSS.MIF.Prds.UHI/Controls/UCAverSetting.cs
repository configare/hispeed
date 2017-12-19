using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.MIF.UI;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Prds.UHI
{
    public partial class UCAverSetting : UserControl, IArgumentEditorUI2
    {
        private string _fileDir = null;
        private string _observationFile;
        private Action<object> _handler;
        private bool _isExcuteArgumentValueChangedEvent = false;
        private IArgumentProvider _arp = null;
        private int _outBoundCount = 0;
        private int _cloudyCount = 0;
        private int _vaildCount = 0;
        private Int16[] _cloudy = null;

        public UCAverSetting()
        {
            InitializeComponent();
            InitFileDir();
        }

        private void InitFileDir()
        {
            _fileDir = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\ProductArgs\UHI\";
            if (!Directory.Exists(_fileDir))
            {
                Directory.CreateDirectory(_fileDir);
                return;
            }
            txtFileDir.Text = Path.GetFileName(GetMatchFile());
            if (!string.IsNullOrEmpty(txtFileDir.Text))
                _observationFile = Path.Combine(_fileDir, txtFileDir.Text);
            if (_handler != null)
            {
                _handler(GetArgumentValue());
            }
        }

        private string GetMatchFile()
        {
            string[] files = Directory.GetFiles(_fileDir, "*.txt", SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                return string.Empty;
            if (Tag == null)
                return files[0];
            _arp = Tag as IArgumentProvider;
            RasterIdentify rs = new RasterIdentify(_arp.DataProvider.fileName);
            if (rs == null || (string.IsNullOrEmpty(rs.Satellite) && string.IsNullOrEmpty(rs.Sensor)))
                return files[0];
            string search = "*" + rs.Satellite + "*" + rs.Sensor + "*.txt";
            files = Directory.GetFiles(_fileDir, search, SearchOption.AllDirectories);
            return files == null || files.Length == 0 ? string.Empty : files[0];
        }

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            if (panel == null)
                return;
            UCExtractPanel ucPanel = panel as UCExtractPanel;
            if (ucPanel == null)
                return;
            IMonitoringSubProduct subProduct = ucPanel.MonitoringSubProduct;
            if (subProduct == null)
                return;
            _arp = subProduct.ArgumentProvider;
            if (_arp == null)
                return;
            Tag = _arp;
            InitFileDir();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "观测文件(*.txt)|*.txt";
                dlg.FileName = "*基准站*";
                dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\SystemData\ProductArgs\UHI\";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtFileDir.Text = Path.GetFileName(dlg.FileName);
                    _observationFile = dlg.FileName;
                }
            }
        }

        public object GetArgumentValue()
        {
            return string.IsNullOrEmpty(txtAverValue.Text) ? (UInt16)0 : UInt16.Parse(txtAverValue.Text);
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        public object ParseArgumentValue(XElement ele)
        {
            return (UInt16)0;
        }

        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return _isExcuteArgumentValueChangedEvent;
            }
            set
            {
                _isExcuteArgumentValueChangedEvent = value;
            }
        }

        private void btnAverValue_Click(object sender, EventArgs e)
        {
            ObservationInfo[] infos = null;
            if (!string.IsNullOrEmpty(_observationFile) && File.Exists(_observationFile))
                infos = ObservationInfo.ReadFile(_observationFile);
            if (infos == null)
            {
                txtAverValue.Text = "0";
                return;
            }
            txtAverValue.Text = CalcAverValue(infos).ToString();
            if (_handler != null)
                _handler(GetArgumentValue());
            MessageBox.Show(ObservationPintInfo, "均值计算", MessageBoxButtons.OK);
        }

        public string ObservationPintInfo
        {
            get { return "观测点处于区域外:" + _outBoundCount + "个,处于云下:" + _cloudyCount + "个, 统计:" + _vaildCount + "个"; }
        }

        public unsafe UInt16 CalcAverValue(ObservationInfo[] infos)
        {
            _outBoundCount = _cloudyCount = _vaildCount = 0;
            int lwIndex = (int)_arp.GetArg("FarInfrared11");
            if (lwIndex == -1)
                return 0;
            double FarInfrared11Zoom = (double)_arp.GetArg("FarInfrared11_Zoom");
            int width = _arp.DataProvider.Width;
            int Height = _arp.DataProvider.Height;
            int row = -1;
            int col = -1;
            UInt16[] temp = new UInt16[1];
            UInt16 Sum = 0;
            for (int i = 0; i < infos.Length; i++)
            {
                CalcRowCol(infos[i], out row, out col);
                if (Vaild(row, col, width, Height))
                {
                    fixed (UInt16* buffer = temp)
                    {
                        IntPtr ptr = new IntPtr(buffer);
                        _arp.DataProvider.GetRasterBand(lwIndex).Read(col, row, 1, 1, ptr, enumDataType.UInt16, 1, 1);
                    }
                    if (temp != null && temp.Length != 0 && temp[0] != 0)
                    {
                        Sum += (UInt16)(temp[0] - (infos[i].chazhi * FarInfrared11Zoom));
                        _vaildCount++;
                        temp = new UInt16[1];
                    }
                    else
                    {
                        _outBoundCount++;
                    }
                }
            }
            if (_vaildCount != 0)
                return (UInt16)(Sum / _vaildCount);
            return 0;
        }

        private bool Vaild(int row, int col, int width, int height)
        {
            if (row < 0 || col < 0 || row > height || col > width)
            {
                _outBoundCount++;
                return false;
            }
            if (_cloudy != null && _cloudy[row * width + col] == 1)
            {
                _cloudyCount++;
                return false;
            }
            return true;
        }

        private void CalcRowCol(ObservationInfo watchInfo, out int row, out int col)
        {
            row = col = -1;
            col = (int)Math.Floor((watchInfo.Lon - _arp.DataProvider.CoordEnvelope.MinX) / _arp.DataProvider.ResolutionX);
            if (col < 0)
                return;
            row = (int)Math.Floor((_arp.DataProvider.CoordEnvelope.MaxY - watchInfo.Lat) / _arp.DataProvider.ResolutionY);
        }

        private void txtAverValue_TextChanged(object sender, EventArgs e)
        {
            if (_handler != null)
            {
                _handler(GetArgumentValue());
            }
        }
    }
}
