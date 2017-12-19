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
using GeoDo.RSS.MIF.UI;

namespace GeoDo.RSS.MIF.Prds.LST
{
    public partial class UCDataCorrection : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handle = null;
        private string txtFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MonitoringProductArgs", "LST", "数据修正文件夹.txt");
        string _compareDateStr = "yyMMddHH";
        private bool _isInit = false;

        public UCDataCorrection()
        {
            InitializeComponent();
            Init();
            //GetDefaultPathFromTxt(); 
            Load += new EventHandler(UCDataCorrection_Load);
        }

        void UCDataCorrection_Load(object sender, EventArgs e)
        {
            GetDefaultPathFromTxt();
        }

        private void GetDefaultPathFromTxt()
        {
            if (!File.Exists(txtFile))
                return;
            _isInit = true;
            using (StreamReader sr = new StreamReader(txtFile, Encoding.GetEncoding("GB2312")))
            {
                string dirPath = sr.ReadLine();
                if (Directory.Exists(dirPath))
                {
                    txtDir.Text = dirPath;
                    SearchOvserveDataFile();
                    //txtObservationData.Text = dirPath;
                }
                dirPath = sr.ReadLine();
                if (File.Exists(dirPath))
                {
                    txtNDVIFile.Text = dirPath;
                    txtNDVIFile.Tag = dirPath;
                }
                dirPath = sr.ReadLine();
                if (File.Exists(dirPath))
                    txtParaFile.Text = dirPath;
                dirPath = sr.ReadLine();
                txtOutputDir.Text = dirPath;
            }
            _isInit = false;
        }

        private void Init()
        {
            txtCorrectValue.Text = "2730";
            txtOutputDir.Text = @"D:\1.csv";
        }

        public object GetArgumentValue()
        {
            string argument = (txtObservationData.Tag == null ? string.Empty : txtObservationData.Tag.ToString()) + ";" +
                txtNDVIFile.Text + ";" +
                txtParaFile.Text + ";" +
                txtOutputDir.Text + ";" +
                txtCorrectValue.Text;
            return argument;
        }

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            _isInit = true;
            txtDir.Tag = null;
            if (panel == null)
                return;
            UCExtractPanel ucPanel = panel as UCExtractPanel;
            if (ucPanel == null)
                return;
            IMonitoringSubProduct subProduct = ucPanel.MonitoringSubProduct;
            if (subProduct == null)
                return;
            IArgumentProvider arp = subProduct.ArgumentProvider;
            if (arp == null)
                return;
            string filename = arp.DataProvider.fileName;
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                return;
            txtDir.Tag = filename;
            txtDir_TextChanged(null, null);
            _isInit = false;
        }

        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return false;
            }
            set
            {
                ;
            }
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return null;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handle = handler;
        }

        private void btnOpenObsvt_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "常规观测数据(*.000)|*.000|所有文件(*.*)|*.*";
                dlg.Multiselect = false;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtObservationData.Tag = dlg.FileName;
                    txtObservationData.Text = Path.GetFileName(dlg.FileName);
                }
            }
        }

        private void btnOpenNDVI_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "旬NDVI文件(*.dat,*.raw)|*.dat;*.raw|所有文件(*.*)|*.*";
                dlg.Multiselect = false;
                dlg.RestoreDirectory = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtNDVIFile.Text = dlg.FileName;
                }
            }
        }

        private void btnOpentxt_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "NDVI分段系数文件(*.txt)|*.txt|所有文件(*.*)|*.*";
                dlg.Multiselect = false;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtParaFile.Text = dlg.FileName;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_handle != null)
                _handle(GetArgumentValue());
            if (!_isInit)
            {
                //保存文件路径
                SaveDefaultFile();
            }
        }

        private void SaveDefaultFile()
        {
            if (string.IsNullOrEmpty(txtCorrectValue.Text))
                return;
            //if (string.IsNullOrEmpty(txtNDVIFile.Text))
            //    return;
            if (string.IsNullOrEmpty(txtParaFile.Text))
                return;
            if (!File.Exists(txtFile))
            {
                string dir = Path.GetDirectoryName(txtFile);
                Directory.CreateDirectory(dir);
            }
            using (StreamWriter sw = new StreamWriter(txtFile, false, Encoding.GetEncoding("GB2312")))
            {
                //sw.WriteLine(txtObservationData.Text);
                sw.WriteLine(txtDir.Text);
                sw.WriteLine(txtNDVIFile.Text);
                sw.WriteLine(txtParaFile.Text);
                sw.WriteLine(txtOutputDir.Text);
            }
        }

        private void btnDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                txtDir.Text = folderDialog.SelectedPath;
                SearchOvserveDataFile();
            }
        }

        private void SearchOvserveDataFile()
        {
            if (txtDir.Tag == null || string.IsNullOrEmpty(txtDir.Text) || !Directory.Exists(txtDir.Text))
                return;
            RasterIdentify rid = new RasterIdentify(Path.GetFileName(txtDir.Tag.ToString()));
            if (rid == null)
                return;
            string[] files = Directory.GetFiles(txtDir.Text, "*" + rid.OrbitDateTime.AddHours(8).AddMinutes(30).ToString(_compareDateStr) + "*.000", SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                return;
            txtObservationData.Text = Path.GetFileName(files[0]);
            txtObservationData.Tag = files[0];
        }

        private void txtDir_TextChanged(object sender, EventArgs e)
        {
            SearchOvserveDataFile();
            btnOK_Click(null, null);
        }

        private void txtObservationData_TextChanged(object sender, EventArgs e)
        {
            btnOK_Click(null, null);
        }

        private void txtNDVIFile_TextChanged(object sender, EventArgs e)
        {
            btnOK_Click(null, null);
        }

        private void txtParaFile_TextChanged(object sender, EventArgs e)
        {
            btnOK_Click(null, null);
        }

        private void txtCorrectValue_TextChanged(object sender, EventArgs e)
        {
            btnOK_Click(null, null);
        }
    }
}
