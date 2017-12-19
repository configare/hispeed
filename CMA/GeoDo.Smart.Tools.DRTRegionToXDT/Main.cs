using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using GeoDo.RSS.UI.AddIn.DataPro;
using GeoDo.FileProject;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.LDF;

namespace GeoDo.Smart.Tools.DRTRegionToXDT
{
    public partial class Main : Form
    {
        private Regex _regex;
        private BlockItemGroup _group;
        private FileStream _logFileStream = null;
        private StreamWriter _logFileWriter = null;

        public Main()
        {
            InitializeComponent();
            _regex = new Regex(@"\S+_\S+_(?<region>(0D|0S)\d{2})", RegexOptions.Compiled);
            DefinedRegionParse parse = new DefinedRegionParse();
            BlockDefined block = parse.BlockDefined;
            _group = block.FindGroup("干旱");
        }

        private void btnInputPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog diag = new FolderBrowserDialog())
            {
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtInputPath.Text = diag.SelectedPath;
                }
            }
        }

        private void btnOutputPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog diag = new FolderBrowserDialog())
            {
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtOutputPath.Text = diag.SelectedPath;
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string inputPath = txtInputPath.Text;
            string outputPath = txtOutputPath.Text;
            if (string.IsNullOrWhiteSpace(inputPath) || string.IsNullOrWhiteSpace(outputPath))
            {
                ShowMsg("输入输出路径不能为空");
                return;
            }
            string[] files = Directory.GetFiles(inputPath, "*.ldf");
            if (files == null || files.Length == 0)
            {
                ShowMsg("指定路径下没有ldf文件");
                return;
            }
            if (_group == null || _group.BlockItems == null || _group.BlockItems.Length == 0)
            {
                ShowMsg("未取得分块定义，无法执行修改");
                return;
            }
            try
            {
                string logfile = Path.Combine(outputPath, DateTime.Now.ToString("yyyyMMddHHmmss") + "log.txt");
                BeginLog(logfile);
                EditRegion(files, outputPath);
                ShowMsg("执行结束，详细情况请查看日志文件" + logfile);
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message);
            }
            finally
            {
                EndLog();
            }
        }

        #region log
        private void BeginLog(string logFilename)
        {
            if (!Directory.Exists(Path.GetDirectoryName(logFilename)))
                Directory.CreateDirectory(Path.GetDirectoryName(logFilename));
            _logFileStream = new FileStream(logFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            _logFileWriter = new StreamWriter(_logFileStream);
            _logFileWriter.AutoFlush = true;
        }

        private void EndLog()
        {
            if (_logFileWriter != null)
                _logFileWriter.Dispose();
            if (_logFileStream != null)
                _logFileStream.Dispose();
        }

        private void WriteLog(string msg)
        {
            _logFileWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:ss:mm"));
            _logFileWriter.WriteLine(msg);
        }
        #endregion

        /// <summary>
        /// NOAA18_AVHRR_D01_
        /// </summary>
        /// <param name="files"></param>
        private void EditRegion(string[] files, string outputPath)
        {
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                if (File.Exists(file))
                {
                    try
                    {
                        WriteLog("文件：" + file);
                        Editfile(file, outputPath);
                    }
                    finally
                    {
                    }
                }
            }
        }

        private void Editfile(string file, string outputPath)
        {
            Match match = _regex.Match(file);
            string fileRegion = match.Groups["region"].Value;
            string region = fileRegion.Replace("0S", "0D");
            if (string.IsNullOrWhiteSpace(region))
            {
                WriteLog("[失败]无法识别文件区域");
                return;
            }
            PrjEnvelopeItem item = _group.GetPrjEnvelopeItem(region);
            if (item == null || item.PrjEnvelope == null)
            {
                WriteLog("[失败]区域" + region + "无法匹配");
                return;
            }
            using (IRasterDataProvider raster = GeoDataDriver.Open(file) as IRasterDataProvider)
            {
                if (!(raster is ILdfDataProvider))
                {
                    WriteLog("[失败]源文件无法读取");
                    return;
                }
            }
            string destFile = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(file) + "_XDT.LDF").Replace(fileRegion, region);
            if (File.Exists(destFile))
            {
                WriteLog("[失败]目标文件已经存在" + destFile);
                return;
            }
            File.Copy(file, destFile, true);
            try
            {
                CoordEnvelope newEnv = new CoordEnvelope(item.PrjEnvelope.MinX + 0.005d, item.PrjEnvelope.MaxX - 0.005d, item.PrjEnvelope.MinY + 0.005d, item.PrjEnvelope.MaxY - 0.005d);
                using (IRasterDataProvider raster = GeoDataDriver.Open(destFile, RSS.Core.DF.enumDataProviderAccess.Update, null) as IRasterDataProvider)
                {
                    (raster as ILdfDataProvider).Update(newEnv);
                    (raster as ILdfDataProvider).IsStoreHeaderChanged = true;
                    (raster as ILdfDataProvider).UpdateHeader();
                }
                WriteLog("[成功]，目标文件" + destFile);
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
                File.Delete(destFile);
            }
        }

        private void ShowMsg(string msg)
        {
            MessageBox.Show(msg, "系统提示", MessageBoxButtons.OK);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
