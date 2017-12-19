using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.MIF.UI;

namespace GeoDo.RSS.MIF.Prds.FOG
{
    public partial class UCCoverageFile : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handle = null;
        private string txtFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MonitoringProductArgs", "FOG", "真彩图数据输出目录.txt");
        private bool _isInit = false;
        private List<string> _lst = new List<string>();
        private string _selectDir;
        private void InitListDir()
        {
            if (!File.Exists(txtFile))
                return;

            _isInit = true;
            using (var sr = new StreamReader(txtFile, Encoding.GetEncoding("GB2312")))
            {
                string dirPath = sr.ReadLine();
                if (Directory.Exists(dirPath))
                {
                    txtOutDir.Text = dirPath;
                }
            }
            _isInit = false;
        }

        private void Init()
        {
            InitListDir();
        }

        public UCCoverageFile()
        {
            InitializeComponent();
        }

        #region IArgumentEditorUI2 成员

        public object GetArgumentValue()
        {
            string argument = txtOutDir.Text;
            return argument;
        }

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            UCExtractPanel ucPanel = panel as UCExtractPanel;
            if (ucPanel == null)
                return;
            IMonitoringSubProduct subProduct = ucPanel.MonitoringSubProduct;
            if (subProduct == null)
                return;
            IArgumentProvider arp = subProduct.ArgumentProvider;
            if (arp == null)
                return;
            Init();
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
        
        private void SaveDefaultFile()
        {
            try
            {
                string dir = Path.GetDirectoryName(txtFile);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                using (StreamWriter sw = new StreamWriter(txtFile, false, Encoding.GetEncoding("GB2312")))
                {
                    sw.WriteLine(txtOutDir.Text);
                }
            }
            catch
            {
            }
        }

        #endregion

        private void btnNatrueColorDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                txtOutDir.Text = folderDialog.SelectedPath;
            }
        }

        private void txtOutDir_TextChanged(object sender, EventArgs e)
        {
            SaveArgsFile();
        }

        private void SaveArgsFile()
        {
            if (_handle != null)
                _handle(GetArgumentValue());
            if (!_isInit)
            {
                //保存文件路径
                SaveDefaultFile();
            }
        }
    }
}
