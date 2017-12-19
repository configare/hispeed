using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RasterProject;
using GeoDo.RSS.RasterTools;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.UI;
using System.IO;
using System.ComponentModel.Composition;
using GeoDo.MathAlg;
using GeoDo.RSS.MIF.UI;

namespace GeoDo.RSS.MIF.Prds.PrdVal
{
     [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public partial class UCLSTSatelliteArgs : UserControl,IArgumentEditorUI2
    {
        private Action<object> _handler;
        private bool _IsExcuteArgumentValueChangedEvent = false;
        private ISmartSession _session = null;
        public UCLSTSatelliteArgs()
        {
            InitializeComponent();
        }
        # region IArgumentEditorUI 成员
        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
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
                object obj = arp.GetArg("SmartSession");
                if (obj != null)
                    _session = obj as ISmartSession;
            }
        }
        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return _IsExcuteArgumentValueChangedEvent;
            }
            set
            {

            }
        }
        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return null;
        }
        public object GetArgumentValue()
        {
            ValArguments args = new ValArguments();
            args.FileNamesForVal = new string[] { txtForVal1.Text };
            args.FileNamesToVal = new string[] { txtToVal.Text };
            args.Invalid = txtinvalid.Text.Trim();
            args.ForInvalid = txtforinvalid.Text.Trim();
            args.MinX = txtMinX.Text;
            args.MaxX = txtMaxX.Text;
            args.MinY = txtMinY.Text;
            args.MaxY = txtMaxY.Text;
            args.CreatScatter = chScatter.Checked;
            args.CreatHistogram = chHistogram.Checked;
            args.MaxColumns = txtColumn.Text;
            args.CreatRMSE = chRmse.Checked;
            return args;
        }
        #endregion
        
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnOpen1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog frm = new OpenFileDialog())
            {
                frm.Filter = "待验证LST数据文件(*.dat,*.ldf)|*.dat;*.ldf";
                frm.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    txtToVal.Text = frm.FileName;
                }
            }
        }

        private void btnOpen2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog frm = new OpenFileDialog())
            {
                frm.Filter = "MODIS LST数据文件(*.dat,*.ldf)|*.dat;*.ldf";
                frm.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    txtForVal1.Text = frm.FileName;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void txtToVal_TextChanged(object sender, EventArgs e)
        {
            IRasterDataProvider toValRaster = GeoDataDriver.Open(txtToVal.Text) as IRasterDataProvider;
            txtMinX.Text = Convert.ToString(toValRaster.CoordEnvelope.MinX);
            txtMaxX.Text = Convert.ToString(toValRaster.CoordEnvelope.MaxX);
            txtMinY.Text = Convert.ToString(toValRaster.CoordEnvelope.MinY);
            txtMaxY.Text = Convert.ToString(toValRaster.CoordEnvelope.MaxY);
        }

        private void chHistogram_CheckedChanged(object sender, EventArgs e)
        {
            showcolumn.Enabled = true;
            txtColumn.Enabled = true;
        }

        private void chScatter_CheckedChanged(object sender, EventArgs e)
        {
            if (chScatter.Checked)
            {
                lblmsg.Enabled = true;
                ExcuteScatter(txtToVal.Text, txtForVal1.Text);
            }
        }
        private void ExcuteScatter(string xfile, string yfile)
        {
            IRasterDataProvider XdataProvider = GeoDataDriver.Open(xfile) as IRasterDataProvider;
            IRasterDataProvider YdataProvider = GeoDataDriver.Open(yfile) as IRasterDataProvider;
            int[] aoi = null;
            int[] bandNos = new int[] { 1, 1 };
            if (bandNos == null || XdataProvider == null || YdataProvider == null)
            {
                throw new ArgumentException("文件打开为空或波段选择为空！");
            }
            if (XdataProvider.Width != YdataProvider.Width || XdataProvider.Height != YdataProvider.Height)
                throw new ArgumentException("两个文件大小不一致！");
            if (XdataProvider.DataType != YdataProvider.DataType)
                throw new ArgumentException("两个文件数据类型不一致！");
            //构建虚拟的dataProvider
            IRasterBand xband = XdataProvider.GetRasterBand(1);
            IRasterBand yband = YdataProvider.GetRasterBand(1);
            IRasterDataProvider localprd = new LogicalRasterDataProvider(Path.GetFileName(XdataProvider.fileName), new IRasterBand[2] { xband, yband }, null);
            if (localprd.BandCount != 2)
            {
                throw new ArgumentException("两个波段信息不一致，无法进行散点图运算！");
            }
            IProgressMonitor progress = _session.ProgressMonitorManager.DefaultProgressMonitor;
            try
            {
                progress.Reset("正在准备生成散点图...", 100);
                progress.Start(false);
                frmScatterGraph frm1 = new frmScatterGraph();
                frm1.Owner = _session.SmartWindowManager.MainForm as Form;
                frm1.StartPosition = FormStartPosition.CenterScreen;
                LinearFitObject fitObj = new LinearFitObject();
                double[] xin = null; double[] yin = null;
                if (!String.IsNullOrWhiteSpace(txtinvalid.Text))
                {
                    string[] toinvalids = new string[] { };
                    if (!String.IsNullOrWhiteSpace(txtinvalid.Text) && txtinvalid.Text.Contains(","))
                        toinvalids = txtinvalid.Text.Split(new char[] { ',' });
                    else
                        toinvalids = new string[] { txtinvalid.Text };
                    List<double> xinval = new List<double>();
                    if (toinvalids.Length >= 1)
                    {
                        foreach (string x in toinvalids)
                        {
                            xinval.Add(Convert.ToDouble(x));
                        }
                        xin = xinval.ToArray();
                    }
                }
                if(!String.IsNullOrWhiteSpace(txtforinvalid.Text))
                {
                    string[] forinvalids = new string[] { };
                    if (!String.IsNullOrWhiteSpace(txtforinvalid.Text) && txtforinvalid.Text.Contains(","))
                        forinvalids = txtforinvalid.Text.Split(new char[] { ',' });
                    else
                        forinvalids = new string[] { txtforinvalid.Text };
                    List<double> yinval = new List<double>();
                    if (forinvalids.Length >= 1)
                    {
                        foreach (string y in forinvalids)
                        {
                            yinval.Add(Convert.ToDouble(y));
                        }
                        yin = yinval.ToArray();
                    }
                }
                MessageBox.Show("生成散点图");
                if (xin == null && yin == null)
                {
                    frm1.Reset(localprd, 1, 2, aoi,
                        fitObj, (idx, tip) => { progress.Boost(idx, "正在准备生成散点图..."); }
                               );
                }
                else
                {
                    frm1.Reset(localprd, 1, 2, xin, yin, aoi,
                               fitObj,
                                (idx, tip) => { progress.Boost(idx, "正在准备生成散点图..."); }
                               );
                }
                progress.Finish();
                frm1.Show();
                frm1.Rerender();
                frm1.FormClosed += new FormClosedEventHandler((obj, e) =>
                {
                    XdataProvider.Dispose();
                    YdataProvider.Dispose();
                });
            }
            finally
            {
                progress.Finish();
            }

        }
    }
}
