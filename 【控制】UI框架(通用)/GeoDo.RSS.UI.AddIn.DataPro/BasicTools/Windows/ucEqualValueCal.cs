using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using CodeCell.AgileMap.Core;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public partial class ucEqualValueCal : UserControl
    {
        private string srcfilepath, outPutShapeFilesPath;
        private UInt16[] _values = null;
        private int _boundCount = 0;
        private Size _size;
        private ShapePoint _leftUpCoord = null;
        private double _dx;
        private double _dy;
        private int _bits = 0;
        private string filter = null;
        public event EventHandler OpenFileHandler = null;
        private bool _isFromMemory = true;
        public event GetValuesFromCurrentAIOViewImageHandler GetValuesFromCurrentAIOViewImageHandler = null;
        public static string OutFileDir = null;
        public static EventHandler FormClosedHandler = null;
        public static EventHandler CkChanged = null;
        public static EventHandler OpenNewFileHandler = null;
        public static bool Sucess = true;
        private int _selectedBound = 1;
        private string _openFilename = string.Empty;
        public static bool _openSucess = true;
        public static bool Cancel = true;
        //private EventHandler _onWindowClosed;
        ISmartSession _session = null;

        public ucEqualValueCal()
        {
            InitializeComponent();
            InitControls();
           
            //FormClosed += new FormClosedEventHandler(IsoLine_FormClosed);
        }

        private void InitControls()
        {
            filter += "TIFF (*.TIF;*.TIFF)|*.TIF;*.TIFF";
            filter += "|LDF (*.LDF;*.LD2)|*.LDF;*.LD2";
            filter += "|HDF RAW (*.HDFRAW)|*.HDFRAW";
            filter += "|JPEG(*.JPG;*.JPEG;*.JPE)|*.JPEG;*.JPG";
            filter += "|BMP(*.BMP)|*.BMP;*.DIB";
            filter += "|PNG(*.PNG)|*.PNG";
            filter += "|IMG(*.IMG)|*.img;*.IMG";
        }

        public ucEqualValueCal(ISmartSession session)
        {
            InitializeComponent();
            _session = session;
            InitControls();

            //_onWindowClosed += new EventHandler(IsoLine_FormClosed);
            //FormClosed += new FormClosedEventHandler(IsoLine_FormClosed);
        }

        void IsoLine_FormClosed(object sender, EventArgs e)
        {
            if (FormClosedHandler != null)
                FormClosedHandler(txtOutputtShapeFile.Text, e);
            OutFileDir = null;
            _openFilename = string.Empty;
        }

        private void IsoLine_Load(object sender, EventArgs e)
        {
            if (_session.SmartWindowManager.ActiveCanvasViewer == null || _session.SmartWindowManager.ActiveCanvasViewer.Canvas == null)
            {
                brnFromMemory.Enabled = false;
            }
            InitCmbBoundCount();
            txtOutputtShapeFile.Text = (OutFileDir != null ? OutFileDir : string.Empty);
        }

        private void IntValues()
        {
            cmbBoundCount.SelectedValueChanged -= new System.EventHandler(this.cmbBoundCount_SelectedValueChanged);
            if (_isFromMemory)
            {
                ComputeArgsForMemoryImage();
            }
            else
            {
                ComputeArgsForFileResource();
            }
            if (_isFromMemory && (_values == null || _bits != 16))//8位数据读取不正确
            {
                ComputeArgsForFileResource();
            }
            cmbBoundCount.SelectedValueChanged += new System.EventHandler(this.cmbBoundCount_SelectedValueChanged);
            if (_values == null)
            {
                return;
            }
            UInt16 minV = 0, maxV = 0;
            ComputeMixMax(_values, out minV, out maxV);
            //txtPixMin.Text = minV.ToString();
            //txtPixMax.Text = maxV.ToString();
            numericUpDown1.Text = GetMinValue(minV);
            numericUpDown2.Text = GetMaxValue(maxV);
            if (!_isFromMemory && _values != null)
                _values = null;
        }

        private string GetMaxValue(UInt16 maxV)
        {
            int result = (int)(Math.Floor(maxV / 10f));
            if (result % 2 == 0)
                result = (result - 1) * 10;
            else
                result *= 10;
            return result.ToString();
        }

        private string GetMinValue(UInt16 minV)
        {
            int result = (int)(Math.Ceiling(minV / 10f));
            if (result % 2 == 0)
                result = (result + 1) * 10;
            else
                result *= 10;
            return result.ToString();
        }

        private void InitCmbBoundCount()
        {
            cmbBoundCount.Items.Clear();
            if (_boundCount == 0)
            {
                cmbBoundCount.Text = "1";
                cmbBoundCount.Enabled = false;
            }
            else
            {
                for (int i = 1; i <= _boundCount; i++)
                {
                    cmbBoundCount.Items.Add(i);
                }
                cmbBoundCount.Text = "1";
                cmbBoundCount.Enabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="size"></param>
        /// <param name="minx">左上角X</param>
        /// <param name="maxy">左上角Y</param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public void SetDataMatrix(object data, Size size, double minx, double maxy, double dx, double dy)
        {
            txtResFile.Enabled = false;
            btnResFile.Enabled = false;
            //btnComputePixMinMax.Enabled = false;
            UInt16 minValue;
            UInt16 maxValue;
            try
            {
                _values = (UInt16[])data;
                _size = size;
                _dx = dx;
                _dy = dy;
                _leftUpCoord = new ShapePoint(minx, maxy);
                ComputeMixMax(_values, out minValue, out maxValue);
                //txtPixMin.Text = minValue.ToString();
                //txtPixMax.Text = maxValue.ToString();
                InitCmbBoundCount();
            }
            catch
            {
                _values = null;
            }
        }

        public string OutShapeFilesFile
        {
            set
            {
                if (Path.GetFullPath(value) != string.Empty)
                {
                    outPutShapeFilesPath = Path.GetFullPath(value);
                    txtOutputtShapeFile.Text = outPutShapeFilesPath;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = filter;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (OpenFileHandler != null && !string.IsNullOrEmpty(dlg.FileName))//&& (string.IsNullOrEmpty(_openFilename) || _openFilename != txtResFile.Text.ToUpper().Trim()))
                    {
                        OpenFileHandler(dlg.FileName, null);
                        _isFromMemory = true;
                        _openFilename = dlg.FileName.ToUpper().Trim();
                        if (OpenNewFileHandler != null)
                        {
                            OpenNewFileHandler(dlg.FileName, e);
                            txtOutputtShapeFile.Text = (OutFileDir != null ? OutFileDir : string.Empty);
                        }
                        txtResFile.Text = dlg.FileName;
                    }
                    //cmbBoundCount.Enabled = false;
                    //if (CkChanged != null)
                    //{
                    //    CkChanged(sender, e);
                    //    txtOutputtShapeFile.Text = (OutFileDir != null ? OutFileDir : string.Empty);
                    //}
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            srcfilepath = "";
            SaveFileDialog savefileDlg = new SaveFileDialog();
            savefileDlg.Filter = filter;
            savefileDlg.AddExtension = true;
            if (savefileDlg.ShowDialog() == DialogResult.OK)
                srcfilepath = savefileDlg.FileName;
            txtOutputtShapeFile.Text = srcfilepath;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            btnGetShapeFile.Enabled = false;
            button5.Enabled = false;
            try
            {
                if (!cmbBoundCount.Enabled)
                {
                    MessageBox.Show("还未获取信息或选择通道,请先获取信息!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                int pixMin = Convert.ToInt32(numericUpDown1.Text);
                int pixMax = Convert.ToInt32(numericUpDown2.Text);
                int steps = Convert.ToInt32(numericUpDown3.Text);
                if (pixMax - pixMin < steps)
                {
                    MessageBox.Show("输入的等值线间隔值过大,请检查!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                //DataFileUtility.maxIgnorePoints = (int)txtIgnore.Value;
                srcfilepath = Convert.ToString(txtResFile.Text);
                outPutShapeFilesPath = Convert.ToString(txtOutputtShapeFile.Text);
                try
                {
                    string shpfile = Path.GetDirectoryName(outPutShapeFilesPath);
                    shpfile = Path.GetFileName(outPutShapeFilesPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("输出文件输入错误(" + ex.Message + "),请重新输入!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (outPutShapeFilesPath.Trim() == "")
                {
                    MessageBox.Show("必须输入输出文件,请重新输入!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    // 等值线设置
                    int nMin = Convert.ToInt32(numericUpDown1.Text); ;        // 等值线最小值
                    int nMax = Convert.ToInt32(numericUpDown2.Text); ;        // 等值线最大值
                    int nSteps = Convert.ToInt32(numericUpDown3.Text); ;      // 等值线间距
                    Application.DoEvents();
                    //if (_values == null)
                    //    WeatherFY.AlgProcessing.WAPIsoLine(outPutShapeFilesPath, nMin, nMax, nSteps);
                    //else
                    //    WeatherFY.AlgProcessing.WAPIsoLine(outPutShapeFilesPath, _values, _size.Width, _size.Height, _leftUpCoord
                    //                                , _dx, _dy, nMin, nMax, nSteps);
                }
            }
            finally
            {
                Cancel = false;
                btnGetShapeFile.Enabled = true;
                button5.Enabled = true;
            }
            //this.Close();
        }

        //private unsafe void btnComputePixMinMax_Click(object sender, EventArgs e)
        //{
        //    btnGetShapeFile.Enabled = true;
        //    if (btnFromFile.Checked && (string.IsNullOrEmpty(txtResFile.Text) || !File.Exists(txtResFile.Text)))
        //        return;
        //    IntValues();
        //}

        private void ComputeMixMax(UInt16[] values, out UInt16 min, out UInt16 max)
        {
            min = UInt16.MaxValue;// values[0];
            max = 0;// values[0];
            if (values != null && values.Length != 0)
                foreach (UInt16 value in values)
                {
                    if (value == UInt16.MaxValue)
                        continue;
                    if (min > value)
                        min = value;
                    if (max < value)
                        max = value;
                }
            if (min == UInt16.MaxValue)
            {
                MessageBox.Show("当前图像整体位于陆地之上,无法进行海冰等值线提取!");
                btnGetShapeFile.Enabled = false;
            }
            else if (min == max)
            {
                MessageBox.Show("当前图像的当前通道数据只有一个值,无法进行海冰等值线提取!");
                btnGetShapeFile.Enabled = false;
            }
        }

        private void ComputeArgsForMemoryImage()
        {
            if (GetValuesFromCurrentAIOViewImageHandler != null)
            {
                CurrentMemoryImage mem = GetValuesFromCurrentAIOViewImageHandler(this, _selectedBound - 1) as CurrentMemoryImage;
                if (mem == null)
                    return;
                _dx = mem.Dx;
                _dy = mem.Dy;
                _leftUpCoord = mem.LeftUpCoord;
                _size = mem.Size;
                _bits = mem.bits;
                _values = mem.Values as UInt16[];
                cmbBoundCount.Enabled = true;
                if (_boundCount != mem.BoundCound)
                {
                    _boundCount = mem.BoundCound;
                    InitCmbBoundCount();
                }
                if (_values == null)
                    return;
            }
        }

        private unsafe void ComputeArgsForFileResource()
        {
            if (_session.SmartWindowManager.ActiveCanvasViewer == null)
                return;
            ICanvas canvas = _session.SmartWindowManager.ActiveCanvasViewer.Canvas;
            if (canvas == null)
                return;
            IRasterDrawing drawing = _session.SmartWindowManager.ActiveCanvasViewer.ActiveObject as IRasterDrawing;
            if (drawing == null)
                return;
            int width = drawing.DataProvider.Width;
            int height = drawing.DataProvider.Height;
            if (_boundCount != drawing.BandCount)
            {
                _boundCount = drawing.BandCount;
                InitCmbBoundCount();
            }
            _leftUpCoord = new ShapePoint(canvas.CurrentEnvelope.MinX, canvas.CurrentEnvelope.MaxY);
            _size = new Size(width, height);
            _dx = Math.Abs(canvas.CurrentEnvelope.MaxX - canvas.CurrentEnvelope.MinX) / width;
            _dy = Math.Abs(canvas.CurrentEnvelope.MaxY - canvas.CurrentEnvelope.MinY) / height;
            if (_dx == 0)
                _dx = 1;
            else if (_dy == 0)
                _dy = 1;
            int selectedBound = 1;
            int.TryParse(cmbBoundCount.Text, out selectedBound);
            if (drawing.DataProvider.DataType != enumDataType.UInt16)
            {
                MessageBox.Show("统计的数据类型不是UInt16，请选择其它数据！");
                return;
            }
            byte[] retValues = new byte[width * height];
            fixed (byte* ptr = retValues)
            {
                int[] retValuesTest = new int[width * height];
                byte* pt = ptr;
                drawing.DataProvider.Read(0, 0, width, height, new IntPtr(pt), enumDataType.UInt16, width, height, 1, new int[] { selectedBound }, enumInterleave.BSQ);
                //view.ImageLayer.EdittedImg.ReadBandData(selectedBound, 0, 0, width, height, new IntPtr(pt).ToInt32());
                int intPtr = new IntPtr(pt).ToInt32();
                // 图像像素的范围
                //UInt16 PixelMin = 10;
                //UInt16 PixelMax = 10;
                ////WeatherFY.AlgProcessing.WAPGetPixelRange(pt, width, height, ref PixelMin, ref PixelMax);
                //ComputeMixMax(retValues, out PixelMin, out PixelMax);
                //txtPixMin.Text = PixelMin.ToString();
                //txtPixMax.Text = PixelMax.ToString();
                //numericUpDown1.Text = PixelMin.ToString();
                //numericUpDown2.Text = PixelMax.ToString();
            }
            try
            {
                int length = retValues.Length;
                _values = new UInt16[length];
                for (int i = 0; i < length; i++)
                    _values[i] = Convert.ToUInt16(retValues[i]);
            }
            finally
            {
                retValues = null;
                GC.Collect();
            }
        }

        private void btnOutputShapeFile_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "ShapeFiles 矢量文件 (*.shp;)|*.shp;";
                saveFileDialog.AddExtension = true;
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(OutFileDir);
                saveFileDialog.FileName = Path.GetFileName(OutFileDir);
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    outPutShapeFilesPath = saveFileDialog.FileName;
                txtOutputtShapeFile.Text = outPutShapeFilesPath;
            }
        }

         private void txtOutputtShapeFile_DoubleClick(object sender, EventArgs e)
        {
            btnOutputShapeFile_Click(null, null);
        }

        private void brnFromMemory_CheckedChanged(object sender, EventArgs e)
        {
            _isFromMemory = brnFromMemory.Checked;
            cmbBoundCount.Enabled = false;
            btnResFile.Enabled = !_isFromMemory;
            txtResFile.Enabled = btnResFile.Enabled;
            //if (CkChanged != null)
            //{
            //    CkChanged(sender, e);
            //    txtOutputtShapeFile.Text = (OutFileDir != null ? OutFileDir : string.Empty);
            //}
        }

        private void btnFromFile_CheckedChanged(object sender, EventArgs e)
        {
            _isFromMemory = !btnFromFile.Checked;
            btnResFile.Enabled = !_isFromMemory;
            txtResFile.Enabled = btnResFile.Enabled;
            cmbBoundCount.Enabled = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Cancel = true;
            //this.Close();
        }

        private void cmbBoundCount_SelectedValueChanged(object sender, EventArgs e)
        {
            int.TryParse(cmbBoundCount.Text, out _selectedBound);
            if (cmbBoundCount.Enabled)
                IntValues();
        }

        private void IsoLine_FormClosed_1(object sender, FormClosedEventArgs e)
        {
            _values = null;
        }

        private void txtResFile_TextChanged(object sender, EventArgs e)
        {
            if (!File.Exists(txtResFile.Text))
                return;
            if (OpenFileHandler != null)
            {
                OpenFileHandler(txtResFile.Text, null);
                _isFromMemory = true;
                _openFilename = txtResFile.Text.ToUpper().Trim();
                if (OpenNewFileHandler != null)
                {
                    OpenNewFileHandler(txtResFile.Text, e);
                    txtOutputtShapeFile.Text = (OutFileDir != null ? OutFileDir : string.Empty);
                }
                txtResFile.Text = txtResFile.Text;
                cmbBoundCount.Enabled = false;
                //if (CkChanged != null)
                //{
                //    CkChanged(sender, e);
                //    txtOutputtShapeFile.Text = (OutFileDir != null ? OutFileDir : string.Empty);
                //}
            }
        }


        //public EventHandler OnWindowClosed
        //{
        //    get { return _onWindowClosed; }
        //    set { _onWindowClosed = value; }
        //}
    }

    public delegate object GetValuesFromCurrentAIOViewImageHandler(object sender, int boundIndex);

    public class CurrentMemoryImage
    {
        public object Values = null;
        public ShapePoint LeftUpCoord = null;
        public Size Size = Size.Empty;
        public double Dx = 0;
        public double Dy = 0;
        public int BoundCound = 0;
        public int bits = 0;
    }
}