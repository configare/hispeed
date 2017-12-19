using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.RasterTools;
using GeoDo.RSS.Core.RasterDrawing;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.GeoCorrection
{
    public partial class frmGeoCorrection : UserControl
    {
        private UCImageControl _imageControl;
        private UCImageControl _baseDataControl;
        private ISmartSession _smartSession;
        private CoordPoint _imageCrtPoint;
        private CoordPoint _baseDataCrtPoint;
        private CoordPoint _imageCrtRasPoint = new CoordPoint();
        private int _GCPCount = 0;
        private List<GeoCorrectionGCP> _listGCP = new List<GeoCorrectionGCP>();

        public frmGeoCorrection(ISmartSession session)
        {
            InitializeComponent();
            this.Text = "影像几何精校正";

            _smartSession = session;

            LoadWarpImage();
            LoadBaseData();
        }

        private string GetWarpImgName()
        {
            ICanvasViewer canViewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (canViewer == null)
                return "";
            IRasterDrawing drawing = _smartSession.SmartWindowManager.ActiveCanvasViewer.Canvas.PrimaryDrawObject as IRasterDrawing;
            GeoDo.RSS.Core.DF.IRasterDataProvider rdp = drawing.DataProvider;
            if (rdp == null)
                return "";
            else
                return rdp.fileName;
        }

        private void LoadWarpImage()
        {
            try
            {
                UCImageControl imgView = new UCImageControl();
                imgView.AOIIsChanged += new EventHandler(imgView_AOIIsChanged);
                imgView.ImgIsChanged += new EventHandler(imgView_ImgIsChanged);
                imgView.SetLebelName("待校正影像");
                imgView.SetSession(_smartSession);
                string fileName = GetWarpImgName();
                if (File.Exists(fileName))
                {
                    imgView.RasterName = fileName;
                    string fileTitle = Path.GetFileNameWithoutExtension(fileName);
                    string extName = ".ldf";
                    fileTitle = fileTitle + "_GeoRectify" + extName;
                    OutPath.Text = Path.Combine(Path.GetDirectoryName(fileName), fileTitle);
                }
                imgView.IsOnlyOneImg = true;
                _imageControl = imgView;
                imgView.Dock = DockStyle.Fill;
                WarpPanel.Visible = true;
                WarpPanel.Controls.Add(imgView);
            }
            catch
            {
                MsgBox.ShowInfo("影像数据加载失败，暂时不能使用影像功能");
            }
        }

        private void LoadBaseData()
        {
            try
            {
                UCImageControl imgView = new UCImageControl();
                imgView.AOIIsChanged += new EventHandler(imgView_AOIIsChanged);
                imgView.SetLebelName("基准数据");
                imgView.SetSession(_smartSession);
                imgView.IsOnlyOneImg = false;
                _baseDataControl = imgView;
                imgView.Dock = DockStyle.Fill;
                BasePanel.Visible = true;
                BasePanel.Controls.Add(imgView);
            }
            catch
            {
                MsgBox.ShowInfo("影像数据加载失败，暂时不能使用影像功能");
            }
        }

        public void CalError()
        {
            if (_GCPCount <= 3)
            {
                for (int i = 0; i < _GCPCount; i++)
                {
                    GCPdataView[5, i].Value = 0;
                    GCPdataView[6, i].Value = 0;
                    GCPdataView[7, i].Value = 0;
                }
                TotalError.Text = "0";
                return;
            }
            CalGeoCoef calCoef = new CalGeoCoef();
            double[] imgX = new double[_listGCP.Count];
            double[] imgY = new double[_listGCP.Count];
            double[] baseX = new double[_listGCP.Count];
            double[] baseY = new double[_listGCP.Count];
            double[] rmsX = new double[_listGCP.Count];
            double[] rmsY = new double[_listGCP.Count];

            for (int i = 0; i < _listGCP.Count; i++)
            {
                imgX[i] = _listGCP[i].ImagePoint.X;
                imgY[i] = _listGCP[i].ImagePoint.Y;

                baseX[i] = _listGCP[i].BasePoint.X;
                baseY[i] = _listGCP[i].BasePoint.Y;
            }
            if (_GCPCount >= 6)
            {
                double[] coefX = new double[6];
                double[] coefY = new double[6];
                calCoef.PolyCoef2(imgX, imgY, baseX, baseY, _listGCP.Count, out coefX, out coefY, out rmsX, out rmsY);
            }
            else
            {
                double[] coefX = new double[3];
                double[] coefY = new double[3];
                calCoef.PolyCoef1(imgX, imgY, baseX, baseY, _listGCP.Count, out coefX, out coefY, out rmsX, out rmsY);
            }
            double rms = 0.0;
            for (int i = 0; i < _GCPCount; i++ )
            {
                GCPdataView[5, i].Value = rmsX[i];
                GCPdataView[6, i].Value = rmsY[i];
                double rmsXY = Math.Sqrt(rmsX[i]*rmsX[i]+rmsY[i]*rmsY[i]);
                GCPdataView[7, i].Value = rmsXY;
                rms = rms + rmsXY;
            }
            rms = rms / (_GCPCount - 1);
            TotalError.Text = rms.ToString();
        }

        void imgView_AOIIsChanged(object sender, EventArgs e)
        {
            UCImageControl imgView = sender as UCImageControl;
            if (imgView == _imageControl)
            {
                _imageCrtPoint = imgView.DrawedAOI;
            }
            else if (imgView == _baseDataControl)
            {
                _baseDataCrtPoint = imgView.DrawedAOI;
            }
        }

        void imgView_ImgIsChanged(object sender, EventArgs e)
        {
            UCImageControl imgView = sender as UCImageControl;
            string fileName = Path.GetFileNameWithoutExtension(imgView.RasterName);
            //string extName = Path.GetExtension(imgView.RasterName);
            string extName = ".ldf";
        //    string pathName = Path.GetDirectoryName(imgView.RasterName);

            fileName = fileName + "_GeoRectify" + extName;
            fileName = Path.Combine(Path.GetDirectoryName(imgView.RasterName), fileName);
          //  fileName = pathName + "\\" + fileName;
            OutPath.Text = fileName;
        }

        private void AddPoint_Click(object sender, EventArgs e)
        {
            if (_imageCrtPoint == null)
            {
                MessageBox.Show("请从待校正影像中选择控制点！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (_baseDataCrtPoint == null)
            {
                MessageBox.Show("请从基准数据中选择控制点！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _GCPCount++;
            GCPdataView.Rows.Insert(_GCPCount - 1, 1);
            GCPdataView[0, _GCPCount - 1].Value = _GCPCount;
            GCPdataView[1, _GCPCount - 1].Value = _imageCrtPoint.X;
            GCPdataView[2, _GCPCount - 1].Value = _imageCrtPoint.Y;
            GCPdataView[3, _GCPCount - 1].Value = _baseDataCrtPoint.X;
            GCPdataView[4, _GCPCount - 1].Value = _baseDataCrtPoint.Y;
     
            _imageControl.AddGCP_Draw();
            _baseDataControl.AddGCP_Draw();

            double rasX = 0;
            double rasY = 0;
            _imageControl.GetRasCoord(ref rasX, ref rasY);
            _imageCrtRasPoint = new CoordPoint(rasX, rasY);

            GeoCorrectionGCP geoGCP = new GeoCorrectionGCP(_imageCrtRasPoint, _baseDataCrtPoint);
            _listGCP.Add(geoGCP);

            CalError();
        }

        private void DelPoint_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow GCProw in GCPdataView.SelectedRows)
            {
                int nNo = (int)(GCProw.Cells[0].Value);
                _listGCP.RemoveAt(nNo - 1);
                _imageControl.DelGCP_Draw(nNo - 1);
                _baseDataControl.DelGCP_Draw(nNo - 1);
                GCPdataView.Rows.Remove(GCProw);

                _GCPCount--;
            }

            for (int i = 0; i < _GCPCount; i++)
            {
                GCPdataView[0, i].Value = i + 1;
            }
            CalError();
        }

        private void Export_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "控制点文件(*.txt)|*.txt";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                StreamWriter txtWriter = new StreamWriter(dlg.FileName, true, Encoding.Default);
                txtWriter.WriteLine("序号  待校正影像X坐标  待校正影像Y坐标  基准数据X坐标  基准数据Y坐标");
                for (int i = 0; i < _GCPCount; i++)
                {
                    object oLine = GCPdataView[0, i].Value + "  " + GCPdataView[1, i].Value + "  " + GCPdataView[2, i].Value + "  " + GCPdataView[3, i].Value + "  " + GCPdataView[4, i].Value;
                    txtWriter.WriteLine(oLine);
                }
                txtWriter.Flush();
                txtWriter.Close();
            }
        }

        private void GeoCorrect_Click(object sender, EventArgs e)
        {
            if (_listGCP.Count < 3)
            {
                MessageBox.Show("请至少选择3个控制点！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_baseDataControl.GetSpatialRef() == null)
            {
                MessageBox.Show("基准数据没有地理坐标！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (File.Exists(OutPath.Text))
            {
                DialogResult dlgResult = MessageBox.Show("输出文件已存在，是否替换？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlgResult == DialogResult.No)
                {
                    return;
                }
            }

            CalGeoCoef calCoef = new CalGeoCoef();
            double[] imgX = new double[_listGCP.Count];
            double[] imgY = new double[_listGCP.Count];
            double[] baseX = new double[_listGCP.Count];
            double[] baseY = new double[_listGCP.Count];

            for (int i = 0; i < _listGCP.Count; i++)
            {
                imgX[i] = _listGCP[i].ImagePoint.X;
                imgY[i] = _listGCP[i].ImagePoint.Y;

                baseX[i] = _listGCP[i].BasePoint.X;
                baseY[i] = _listGCP[i].BasePoint.Y;
            }

            IProgressMonitor progress = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
            try
            {
                if (progress != null)
                {
                    progress.Reset("", 100);
                    progress.Start(false);
                }

                int coordType = 0;
                int imgType = _imageControl.GetCoordType();
                int baseType = _baseDataControl.GetCoordType();
                if ((imgType == 0) && (baseType == 0))
                    coordType = 0;
                else if ((imgType == 0) && (baseType == 1))
                    coordType = 1;
                else if ((imgType == 0) && (baseType == 2))
                    coordType = 2;
                else if ((imgType == 1) && (baseType == 0))
                    coordType = 3;
                else if ((imgType == 1) && (baseType == 1))
                    coordType = 4;
                else if ((imgType == 1) && (baseType == 2))
                    coordType = 5;
                else if ((imgType == 2) && (baseType == 0))
                    coordType = 6;
                else if ((imgType == 2) && (baseType == 1))
                    coordType = 7;
                else if ((imgType == 2) && (baseType == 2))
                    coordType = 8;

                string spatialRef = _baseDataControl.GetSpatialRef().ToProj4String();
                if (_listGCP.Count >= 6)
                {
                    double[] coefX = new double[6];
                    double[] coefY = new double[6];
                    double[] RcoefX = new double[6];
                    double[] RcoefY = new double[6];

                    calCoef.PolyCoef2(baseX, baseY, imgX, imgY, _listGCP.Count, out coefX, out coefY);
                    calCoef.PolyCoef2(imgX, imgY, baseX, baseY, _listGCP.Count, out RcoefX, out RcoefY);
                    PolyCorrection polyCorrect = new PolyCorrection();
                    polyCorrect.PolyOrder = 2;
                    polyCorrect.DoPolyCorrection(_imageControl.RasterName, OutPath.Text, coordType, coefX, coefY, RcoefX, RcoefY, spatialRef, "LDF", new Action<int, string>((int progerss, string text) =>
                    {
                        if (progress != null)
                            progress.Boost(progerss, text);
                    }));
                }
                else
                {
                    double[] coefX = new double[3];
                    double[] coefY = new double[3];
                    double[] RcoefX = new double[3];
                    double[] RcoefY = new double[3];
                    //imgX[0] = 13.0;
                    //imgX[1] = 116.0;
                    //imgX[2] = 228.0;
                    //imgY[0] = 13.0;
                    //imgY[1] = 132.0;
                    //imgY[2] = 241.0;
                    //baseX[0] = 119.515789;
                    //baseX[1] = 120.036845;
                    //baseX[2] = 120.612091;
                    //baseY[0] = 39.099891;
                    //baseY[1] = 38.556071;
                    //baseY[2] = 38.085306;

                    calCoef.PolyCoef1(baseX, baseY, imgX, imgY, _listGCP.Count, out coefX, out coefY);
                    calCoef.PolyCoef1(imgX, imgY, baseX, baseY, _listGCP.Count, out RcoefX, out RcoefY);
                    PolyCorrection polyCorrect = new PolyCorrection();
                    polyCorrect.PolyOrder = 1;
                    polyCorrect.DoPolyCorrection(_imageControl.RasterName, OutPath.Text, coordType, coefX, coefY, RcoefX, RcoefY, spatialRef, "LDF", new Action<int, string>((int progerss, string text) =>
                    {
                        if (progress != null)
                            progress.Boost(progerss, text);
                    }));
                }
            }
            finally
            {
                if (progress != null)
                    progress.Finish();
            }


            string strText = "几何精校正处理完成，是否打开输出影像" + Path.GetFileName(OutPath.Text) + "?";
            DialogResult result = MessageBox.Show(strText, "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                OpenFileFactory.Open(OutPath.Text);
            }
        }

        private void GCPdataView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            _imageControl.SetCurrentGCPPos(e.RowIndex);
            _baseDataControl.SetCurrentGCPPos(e.RowIndex);
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "LDF文件(*.ldf)|*.ldf|普通影像文件(*.tif)|*.tif";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                OutPath.Text = dlg.FileName;
            }
        }

        public void Free()
        {
            if (_imageControl != null)
            {
                _imageControl.Dispose();
                _imageControl = null;
            }

            if (_baseDataControl != null)
            {
                _baseDataControl.Dispose();
                _baseDataControl = null;
            }

            _listGCP.Clear();
        }
    }
}
