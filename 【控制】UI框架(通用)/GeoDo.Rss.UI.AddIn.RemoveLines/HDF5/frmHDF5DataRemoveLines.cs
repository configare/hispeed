using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Text.RegularExpressions;

namespace GeoDo.RSS.UI.AddIn.RemoveLines
{
    public partial class frmHDF5DataRemoveLines : Form
    {
        public frmHDF5DataRemoveLines()
        {
            InitializeComponent();
        }
        public ISmartSession _smartSession;

        private void btnOk_Click(object sender, EventArgs e)
        {
            string inputFileName = txtFileDir.Text;
            string outputFileName = txtOutDir.Text;
            if (string.IsNullOrWhiteSpace(inputFileName) || string.IsNullOrWhiteSpace(outputFileName))
            {
                MessageBox.Show("输入文件、输出文件不能为空", "提示");
                return;
            }

            int[] bandNos = GetInputBandNos();
            if (bandNos == null)
            {
                MessageBox.Show("波段信息输入有误,请输入待处理的波段,如 5或5,6", "提示");
                return;
            }
            string errBandNo = string.Empty;
            if (!CheckBand(inputFileName, bandNos, out errBandNo))
            {
                MessageBox.Show("文件中不包含第" + errBandNo + "波段");
                return;
            }

            IHDF5RemoveLines opt = new HDF5RemoveLines();
            IProgressMonitor progress = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
            try
            {
                progress.Reset("获取文件信息...", 100);
                progress.Start(false);
                opt.RemoveLines(bandNos, inputFileName, HDFWriteMode.SaveAS, outputFileName,
                    (idx, tip) =>
                    {
                        progress.Boost(idx, tip);
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show("处理失败！" + ex.Message);
            }
            finally
            {
                progress.Finish();
                Close();
            }
        }

        private bool CheckBand(string fileName, int[] bandNos, out string errBandNo)
        {
            errBandNo = string.Empty;
            bool result = false;
            try
            {
                IRasterDataProvider prd = GeoDataDriver.Open(fileName) as IRasterDataProvider;

                for (int i = 0; i < bandNos.Length; i++)
                {
                    errBandNo = bandNos[i].ToString();
                    IRasterBand band = prd.GetRasterBand(bandNos[i]);
                    errBandNo = string.Empty;
                }
                result = true;
            }
            catch (Exception)
            {
                return false;
            }
            
            return result;
        }

        private int[] GetInputBandNos()
        {
            int[] bandNos = null;
            try
            {
                string bandInfo = txtBandNos.Text.Trim();
                string[] b = null;

                if (bandInfo.Contains(','))
                {
                    b = bandInfo.Split(',');
                }
                else
                {
                    b = new string[] { bandInfo };
                }
                bandNos = new int[b.Length];
                for (int i = 0; i < b.Length; i++)
                {
                    bandNos[i] = Convert.ToInt32(b[i]);
                }
            }
            catch (Exception)
            {
                return null;
            }
            return bandNos;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOpenDir_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Multiselect = false;
                dialog.Filter = SupportedFileFilters.HdfFilter;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string fname = dialog.FileName;
                    if (!string.IsNullOrWhiteSpace(fname))
                    {
                        txtFileDir.Text = dialog.FileName;
                        txtOutDir.Text = txtFileDir.Text.Substring(0, dialog.FileName.LastIndexOf('.')) + "_RL." + dialog.FileName.Substring(dialog.FileName.LastIndexOf('.'));
                    }
                }
            }
        }

        private void btnOutDir_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = SupportedFileFilters.HdfFilter;
                if (dialog.ShowDialog() == DialogResult.OK)
                    txtOutDir.Text = dialog.FileName;
            }
        }

    }
}
