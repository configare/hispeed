using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.UI;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.UI;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.DST
{
    public partial class UCDstFY2Extract : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler;
        private string _backFileName = null;
        private IRasterDataProvider _dataProvider;
        private bool _isValueChangedEvent = true;
        private IProgressMonitorManager _progress = null;
        private ContextMenuStrip cms = new ContextMenuStrip();

        public UCDstFY2Extract()
        {
            InitializeComponent();
        }

        public object GetArgumentValue()
        {
            DstFY2ExtractArgSet set = new DstFY2ExtractArgSet();
            set.IsSmooth = chbSmooth.Checked;
            set.IsNightExtract = rdoNightExtract.Checked;
            set.BackFileName = _backFileName;
            List<double> argValues = new List<double>();
            double value;
            if (double.TryParse(txtTBD12Min.Text, out value))
                argValues.Add(value);
            if (double.TryParse(txtTBD12Max.Text, out value))
                argValues.Add(value);
            if (double.TryParse(txtIDDIMin.Text, out value))
                argValues.Add(value);
            if (double.TryParse(txtIDDIMax.Text, out value))
                argValues.Add(value);
            if (double.TryParse(txtTBD13Min.Text, out value))
                argValues.Add(value);
            if (double.TryParse(txtTBD13Max.Text, out value))
                argValues.Add(value);
            if (double.TryParse(txtTBD14Min.Text, out value))
                argValues.Add(value);
            if (double.TryParse(txtTBD14Max.Text, out value))
                argValues.Add(value);
            if (double.TryParse(txtTBD2Min.Text, out value))
                argValues.Add(value);
            if (double.TryParse(txtTBD2Max.Text, out value))
                argValues.Add(value);
            if (double.TryParse(txtTBD3Min.Text, out value))
                argValues.Add(value);
            if (double.TryParse(txtTBD3Max.Text, out value))
                argValues.Add(value);
            if (double.TryParse(txtTBD4Min.Text, out value))
                argValues.Add(value);
            if (double.TryParse(txtTBD4Max.Text, out value))
                argValues.Add(value);
            if (double.TryParse(txtRefMidIRMax.Text, out value))
                argValues.Add(value);
            if (double.TryParse(txtVisrefMin.Text, out value))
                argValues.Add(value);
            if (double.TryParse(txtVisrefMax.Text, out value))
                argValues.Add(value);
            set.ArgValueArray = argValues.ToArray();
            return set;
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
            IArgumentProvider arp = subProduct.ArgumentProvider;
            if (arp != null)
            {
                if (arp.DataProvider != null)
                    _dataProvider = arp.DataProvider;
                object obj = arp.GetArg("SmartSession");
                if (obj != null)
                {
                    ISmartSession session = obj as ISmartSession;
                    _progress = session.ProgressMonitorManager;
                }
            }
            InitArgument();
        }

        private void InitArgument()
        {
            RasterIdentify rid = new RasterIdentify(_dataProvider);
            string satellite = rid.Satellite;
            DateTime dataTime = rid.OrbitDateTime;
            bool isDay = CheckIsDayTime(satellite,dataTime);
            if (isDay)
            {
                rdoDayExtract.Checked = true;
            }
            else
                rdoNightExtract.Checked = false;
            //根据时间设置默认阈值
            SetDefaultThresholdValue(dataTime);
        }

        private void SetDefaultThresholdValue(DateTime dataTime)
        {
            if (dataTime == DateTime.MaxValue || dataTime == DateTime.MinValue)
                return;
            else
            {
                //读取默认参数
                string thresholdFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FY2ExtractThreshold.txt");
                if (!File.Exists(thresholdFileName))
                    return;
                string[] valueLines = File.ReadAllLines(thresholdFileName);
                if (valueLines != null && valueLines.Length != 25)
                    return;
                string[] values = valueLines[dataTime.Hour + 1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (values != null && values.Length != 18)
                    return;
                //根据读取的默认参数进行界面设置
                txtTBD12Max.Text = values[1];
                txtTBD12Min.Text = values[2];
                txtIDDIMax.Text = values[3];
                txtIDDIMin.Text = values[4];
                txtTBD14Max.Text = values[5];
                txtTBD14Min.Text = values[6];
                txtTBD13Max.Text = values[7];
                txtTBD13Min.Text = values[8];
                txtTBD2Max.Text = values[9];
                txtTBD2Min.Text = values[10];
                txtVisrefMax.Text = values[11];
                txtVisrefMin.Text = values[12];
                txtTBD3Max.Text = values[13];
                txtTBD3Min.Text = values[14];
                txtTBD4Max.Text = values[15];
                txtTBD4Min.Text = values[16];
                txtRefMidIRMax.Text = values[17];
                double[] argPair = new double[] { double.Parse(values[2]), double.Parse(values[1]) };
                mtbTBD12.SetValues(argPair);
                mtbTBD12.Tag = new double[]{mtbTBD12.MinEndPointValue,mtbTBD12.MaxEndPointValue};
                argPair = new double[] { double.Parse(values[4]), double.Parse(values[3]) };
                mtbIDDI.SetValues(argPair);
                mtbIDDI.Tag = new double[] { mtbIDDI.MinEndPointValue, mtbIDDI.MaxEndPointValue };
                argPair = new double[] { double.Parse(values[6]), double.Parse(values[5]) };
                mtbTBD14.SetValues(argPair);
                mtbTBD14.Tag = new double[] { mtbTBD14.MinEndPointValue, mtbTBD14.MaxEndPointValue };
                argPair = new double[] { double.Parse(values[8]), double.Parse(values[7]) };
                mtbTBD13.SetValues(argPair);
                mtbTBD13.Tag = new double[] { mtbTBD13.MinEndPointValue, mtbTBD13.MaxEndPointValue };
                argPair = new double[] { double.Parse(values[10]), double.Parse(values[9]) };
                mtbTBD2.SetValues(argPair);
                mtbTBD2.Tag = new double[] { mtbTBD2.MinEndPointValue, mtbTBD2.MaxEndPointValue }; ;
                argPair = new double[] { double.Parse(values[12]), double.Parse(values[11]) };
                mtbVisref.SetValues(argPair);
                mtbVisref.Tag = new double[] { mtbVisref.MinEndPointValue, mtbVisref.MaxEndPointValue };
                argPair = new double[] { double.Parse(values[14]), double.Parse(values[13]) };
                mtbTBD3.SetValues(argPair);
                mtbTBD3.Tag = new double[] { mtbTBD3.MinEndPointValue, mtbTBD3.MaxEndPointValue };
                argPair = new double[] { double.Parse(values[16]), double.Parse(values[15]) };
                mtbTBD4.SetValues(argPair);
                mtbTBD4.Tag = new double[] { mtbTBD4.MinEndPointValue, mtbTBD4.MaxEndPointValue };
                argPair = new double[] { double.Parse(values[17]) };
                mtbRefMidIR.SetValues(argPair);
                mtbRefMidIR.Tag = new double[] { mtbRefMidIR.MinEndPointValue, mtbRefMidIR.MaxEndPointValue };
            }
        }

        private bool CheckIsDayTime(string satellite, DateTime dataTime)
        {
            int hour = dataTime.Hour+8;
            switch (satellite)
            {
                case "FY2D":
                case "FY2E":
                    {
                        if (hour <= 17 && hour >= 10)
                            return true;
                        else
                            return false;
                    }
                case "FY2F":
                    {
                        if (hour <= 17 && hour >= 8)
                            return true;
                        else
                            return false;
                    }
                default: return true;
            }
        }

        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return _isValueChangedEvent;
            }
            set
            {
                _isValueChangedEvent = value;
            }
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return null;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        private void rdoDayExtract_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoDayExtract.Checked)
            {
                rdoNightExtract.Checked = false;
                lblRefMidIR.Visible = lblRefMidIROPR.Visible = true;
                lblVisref.Visible = lblVisrefOPR1.Visible = lblVisrefOPR2.Visible = true;
                txtRefMidIRMax.Visible = txtVisrefMax.Visible = txtVisrefMin.Visible = true;
                mtbRefMidIR.Visible = mtbVisref.Visible = true;
            }
            else
            {
                rdoNightExtract.Checked = true;
                lblRefMidIR.Visible = lblRefMidIROPR.Visible = false;
                lblVisref.Visible = lblVisrefOPR1.Visible = lblVisrefOPR2.Visible = false;
                txtRefMidIRMax.Visible = txtVisrefMax.Visible = txtVisrefMin.Visible = false;
                mtbRefMidIR.Visible = mtbVisref.Visible = false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.Multiselect = true;
            f.Filter = "局地文件(*.ldf,*.ldff,*.ld2,*.ld3)|*.ldf;*.ld2;*.ld3";
            if (f.ShowDialog() == DialogResult.OK)
            {
                AddFiles(f.FileNames);
            }
        }

        private void AddFiles(string[] value)
        {
            if (value != null && value.Length != 0)
            {
                foreach (string f in value)
                {
                    if (!Exist(f))
                        listBox1.Items.Add(new FileItem(f));
                }
            }
            if (FileChanged != null)
                FileChanged(this);
        }

        private bool Exist(string filename)
        {
            if (listBox1.Items == null || listBox1.Items.Count == 0)
                return false;
            foreach (FileItem item in listBox1.Items)
            {
                if (item.FileName == filename)
                    return true;
            }
            return false;
        }

        public event Action<object> FileChanged;

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                List<FileItem> items = new List<FileItem>();
                foreach (FileItem item in listBox1.SelectedItems)
                    items.Add(item);
                foreach (FileItem item in items)
                    listBox1.Items.Remove(item);
            }
            if (FileChanged != null)
                FileChanged(this);
        }

        private void btnComputeBK_Click(object sender, EventArgs e)
        {
            //计算背景数据
            if(_dataProvider==null)
                return;
            //检查卫星传感器以及轨道时间（时、分）是否一致
            if (!CheckFileList())
                return;
            try
            {
                _progress.DefaultProgressMonitor.Reset(string.Empty, 100);
                _progress.DefaultProgressMonitor.Start(false);
                _backFileName = ComputeBackgroudImage(
                    (p, txt) =>
                        {
                            _progress.DefaultProgressMonitor.Boost(p, txt);
                        });
            }
            finally
            {
                _progress.DefaultProgressMonitor.Finish();
            }
            SetArgValue();
        }

        private string ComputeBackgroudImage(Action<int,string> processTracker)
        {
            List<string> computeFiles = new List<string>();
            foreach (FileItem item in listBox1.Items)
                computeFiles.Add(item.FileName);
            if (computeFiles == null || computeFiles.Count < 1)
            {
                return null;
            }
            foreach(string item in computeFiles)
            {
                if(!File.Exists(item))
                {
                    MessageBox.Show("所选择的数据:\"" + item + "\"不存在。");
                    return null;
                }
            }
            IArgumentProvider argPrd=Tag as IArgumentProvider;
            if(argPrd==null)
                return null;
            int visiNo = (int)argPrd.GetArg("Visible");
            int midNo2 = (int)argPrd.GetArg("MiddleInfrared2");
            int farNo1 = (int)argPrd.GetArg("FarInfrared11");
            int farNo2 = (int)argPrd.GetArg("FarInfrared12");
            int[] bandNos = new int[] { farNo1, farNo2, midNo2, visiNo };
            List<RasterMaper> rms = new List<RasterMaper>();
            try
            {
                for (int i = 0; i < computeFiles.Count; i++)
                {
                    IRasterDataProvider inRaster = RasterDataDriver.Open(computeFiles[i]) as IRasterDataProvider;
                    if (inRaster.BandCount < bandNos.Length)
                    {
                        MessageBox.Show("请选择正确的数据进行背景亮温计算。");
                        return null;
                    }
                    RasterMaper rm = new RasterMaper(inRaster, bandNos);
                    rms.Add(rm);
                }
                //输出文件准备（作为输入栅格并集处理）
                RasterIdentify ri = new RasterIdentify(_dataProvider.fileName);
                string outFileName = MifEnvironment.GetFullFileName(Path.GetFileName(ri.ToWksFullFileName(".ldf")));
                using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, bandNos.Length))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    List<int> bandNoList=new List<int>();
                    for (int i = 1; i <= bandNos.Length; i++)
                        bandNoList.Add(i);
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, bandNoList.ToArray()) };
                    //创建处理模型
                    RasterProcessModel<short, short> rfr = null;
                    rfr = new RasterProcessModel<short, short>(null);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<short, short>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        for (int j = 0; j < rvInVistor.Length;j++ )
                        {
                            for (int i = 0; i < bandNos.Length; i++)
                            {
                                short[] dt = rvInVistor[j].RasterBandsData[i];
                                if (dt != null)
                                {
                                    for (int index = 0; index < dt.Length; index++)
                                    {
                                        if (dt[index] > rvOutVistor[0].RasterBandsData[i][index])
                                        {
                                            rvOutVistor[0].RasterBandsData[i][index] = dt[index];
                                        }
                                    }
                                }
                            }
                        }
                    }));
                    //执行
                    rfr.Excute();
                    return outFileName;
                }
            }
            finally
            {
                foreach (RasterMaper rm in rms)
                {
                    rm.Raster.Dispose();
                }
            }
        }

        private bool CheckFileList()
        {
            RasterIdentify currentId = new RasterIdentify(_dataProvider);
            foreach (FileItem file in listBox1.Items)
            {
                RasterIdentify id = new RasterIdentify(file.FileName);
                if (id.Satellite != currentId.Satellite || id.Sensor != currentId.Sensor)
                    return false;
                DateTime time = id.OrbitDateTime;
                if (time.Hour != currentId.OrbitDateTime.Hour||time.Minute!=currentId.OrbitDateTime.Minute)
                    return false;
            }
            return true;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            //清空背景数据
            if (!string.IsNullOrEmpty(_backFileName) && File.Exists(_backFileName))
            {
                File.Delete(_backFileName);
                _backFileName = null;
            }
        }

        private IRasterDataProvider CreateOutRaster(string outFileName, int bandCount)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            CoordEnvelope outEnv = _dataProvider.CoordEnvelope;
            float resX = _dataProvider.ResolutionX;
            float resY = _dataProvider.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, bandCount, _dataProvider.DataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        private void txtTBD12Min_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double minValue = CheckValue( mtbTBD12, txtTBD12Min);
                KeyDownEvent(mtbTBD12, minValue, double.Parse(txtTBD12Max.Text));
                SetArgValue();
            }
        }

        private double CheckValue(UI.MultiBarTrack multiBar, DoubleTextBox textBox)
        {
            double value;
            if (double.TryParse(textBox.Text, out value))
            {
                if (value > multiBar.MaxEndPointValue)
                {
                    value = multiBar.MaxEndPointValue;
                    textBox.Text = value.ToString();
                }
                else if (value < multiBar.MinEndPointValue)
                {
                    value = multiBar.MinEndPointValue;
                    textBox.Text = value.ToString();
                }
                return value;
            }
            return double.NaN;
        }

        private void KeyDownEvent(MultiBarTrack multiBar, double minValue, double maxValue)
        {
            if (minValue == double.MinValue)
            {
                if (maxValue == double.MaxValue)
                    return;
                else
                    multiBar.SetValues(new double[] { maxValue });
            }
            else
            {
                if (maxValue == double.MaxValue)
                    multiBar.SetValues(new double[] { minValue });
                else
                    multiBar.SetValues(new double[] { minValue, maxValue });
            }
        }

        private void mtbTBD12_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            if (barIndex == 0)
                txtTBD12Min.Text = value.ToString("0.##");
            else
                txtTBD12Max.Text = value.ToString("0.##");
            SetArgValue();
        }

        private void mtbIDDI_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            if (barIndex == 0)
                txtIDDIMin.Text = value.ToString("0.##");
            else
                txtIDDIMax.Text = value.ToString("0.##");
            SetArgValue();
        }

        private void mtbTBD13_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            if (barIndex == 0)
                txtTBD13Min.Text = value.ToString("0.##");
            else
                txtTBD13Max.Text = value.ToString("0.##");
            SetArgValue();
        }

        private void mtbTBD14_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            if (barIndex == 0)
                txtTBD14Min.Text = value.ToString("0.##");
            else
                txtTBD14Max.Text = value.ToString("0.##");
            SetArgValue();
        }

        private void mtbTBD2_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            if (barIndex == 0)
                txtTBD2Min.Text = value.ToString("0.##");
            else
                txtTBD2Max.Text = value.ToString("0.##");
            SetArgValue();
        }

        private void mtbTBD3_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            if (barIndex == 0)
                txtTBD3Min.Text = value.ToString("0.##");
            else
                txtTBD3Max.Text = value.ToString("0.##");
            SetArgValue();
        }

        private void mtbTBD4_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            if (barIndex == 0)
                txtTBD4Min.Text = value.ToString("0.##");
            else
                txtTBD4Max.Text = value.ToString("0.##");
            SetArgValue();
        }

        private void mtbRefMidIR_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            txtRefMidIRMax.Text = value.ToString("0.##");
            SetArgValue();
        }

        private void mtbVisref_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            if (barIndex == 0)
                txtVisrefMin.Text = value.ToString("0.##");
            else
                txtVisrefMax.Text = value.ToString("0.##");
            SetArgValue();
        }

        private void txtTBD12Max_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double maxValue = CheckValue(mtbTBD12, txtTBD12Max);
                KeyDownEvent(mtbTBD12,double.Parse(txtTBD12Min.Text),maxValue);
                SetArgValue();
            }
        }

        private void txtIDDIMin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double minValue = CheckValue(mtbIDDI, txtIDDIMin);
                KeyDownEvent(mtbIDDI, minValue, double.Parse(txtIDDIMax.Text));
                SetArgValue();
            }
        }

        private void txtIDDIMax_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double maxValue = CheckValue(mtbIDDI, txtIDDIMax);
                KeyDownEvent(mtbIDDI, double.Parse(txtIDDIMin.Text), maxValue);
                SetArgValue();
            }
        }

        private void txtTBD13Min_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double minValue = CheckValue(mtbTBD13, txtTBD13Min);
                KeyDownEvent(mtbTBD13, minValue, double.Parse(txtTBD13Max.Text));
                SetArgValue();
            }
        }

        private void txtTBD13Max_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double maxValue = CheckValue(mtbTBD13, txtTBD13Max);
                KeyDownEvent(mtbTBD13, double.Parse(txtTBD13Min.Text), maxValue);
                SetArgValue();
            }
        }

        private void txtTBD14Min_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double minValue = CheckValue(mtbTBD14, txtTBD14Min);
                KeyDownEvent(mtbTBD14, minValue, double.Parse(txtTBD14Max.Text));
                SetArgValue();
            }
        }

        private void txtTBD14Max_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double maxValue = CheckValue(mtbTBD14, txtTBD12Max);
                KeyDownEvent(mtbTBD14, double.Parse(txtTBD14Min.Text), maxValue);
                SetArgValue();
            }
        }

        private void txtTBD2Min_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double minValue = CheckValue(mtbTBD2, txtTBD2Min);
                KeyDownEvent(mtbTBD2, minValue, double.Parse(txtTBD2Max.Text));
                SetArgValue();
            }
        }

        private void txtTBD2Max_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double maxValue = CheckValue(mtbTBD2, txtTBD2Max);
                KeyDownEvent(mtbTBD2, double.Parse(txtTBD2Min.Text), maxValue);
                SetArgValue();
            }
        }

        private void txtTBD3Min_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double minValue = CheckValue(mtbTBD3, txtTBD3Min);
                KeyDownEvent(mtbTBD3, minValue, double.Parse(txtTBD3Max.Text));
                SetArgValue();
            }
        }

        private void txtTBD3Max_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double maxValue = CheckValue(mtbTBD3, txtTBD3Max);
                KeyDownEvent(mtbTBD3, double.Parse(txtTBD3Min.Text), maxValue);
                SetArgValue();
            }
        }

        private void txtTBD4Min_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double minValue = CheckValue(mtbTBD4, txtTBD4Min);
                KeyDownEvent(mtbTBD4, minValue, double.Parse(txtTBD4Max.Text));
                SetArgValue();
            }
        }

        private void txtTBD4Max_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double maxValue = CheckValue(mtbTBD4, txtTBD4Max);
                KeyDownEvent(mtbTBD4, double.Parse(txtTBD4Min.Text), maxValue);
                SetArgValue();
            }
        }

        private void txtRefMidIRMax_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double maxValue = CheckValue(mtbRefMidIR, txtRefMidIRMax);
                KeyDownEvent(mtbRefMidIR, double.MinValue, maxValue);
                SetArgValue();
            }
        }

        private void txtVisrefMin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double minValue = CheckValue(mtbVisref, txtVisrefMin);
                KeyDownEvent(mtbVisref, minValue, double.Parse(txtVisrefMax.Text));
                SetArgValue();
            }
        }

        private void txtVisrefMax_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double maxValue = CheckValue(mtbVisref, txtVisrefMax);
                KeyDownEvent(mtbVisref, double.Parse(txtVisrefMin.Text),maxValue);
                SetArgValue();
            }
        }

        private void SetArgValue()
        {
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void chbSmooth_CheckedChanged(object sender, EventArgs e)
        {
            SetArgValue();
        }

        void pairTrack_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                cms.Items.Clear();
                //
                ToolStripMenuItem mnuSetEndpointValue = new ToolStripMenuItem("设置端值");
                mnuSetEndpointValue.Tag = sender;
                mnuSetEndpointValue.Click += new EventHandler(mnuSetEndpointValue_Click);
                cms.Items.Add(mnuSetEndpointValue);
                //
                double[] valuePair = (sender as MultiBarTrack).Tag as double[];
                if (valuePair == null || valuePair.Length != 2)
                    return;
                ToolStripMenuItem quickValue = new ToolStripMenuItem("默认端值：" + valuePair[0] + " " + valuePair[1]);
                quickValue.Tag = new object[] { sender, new double[] { valuePair[0], valuePair[1] } };
                quickValue.Click += new EventHandler(quickValue_Click);
                cms.Items.Add(quickValue);
                cms.Show(sender as MultiBarTrack, e.Location);
            }
        }

        void mnuSetEndpointValue_Click(object sender, EventArgs e)
        {
            MultiBarTrack bar = (sender as ToolStripMenuItem).Tag as MultiBarTrack;
            SetEndPoint(bar);
        }

        void quickValue_Click(object sender, EventArgs e)
        {
            object[] obj = (sender as ToolStripMenuItem).Tag as object[];
            MultiBarTrack bar = obj[0] as MultiBarTrack;
            Double[] values = obj[1] as Double[];
            bar.MinEndPointValue = values[0];
            bar.MaxEndPointValue = values[1];
        }

        private void SetEndPoint(MultiBarTrack bar)
        {
            using (frmSetEndpointValue frm = new frmSetEndpointValue(bar.MinEndPointValue, bar.MaxEndPointValue))
            {
                frm.Location = Control.MousePosition;
                if (frm.Location.X + frm.Width > Screen.GetBounds(this).Width)
                    frm.Location = new Point(Screen.GetBounds(this).Width - frm.Width, Control.MousePosition.Y);
                if (frm.ShowDialog(bar) == DialogResult.OK)
                {
                    bar.MinEndPointValue = frm.MinValue;
                    bar.MaxEndPointValue = frm.MaxValue;
                }
            }
            this.Refresh();
        }
    }
}
