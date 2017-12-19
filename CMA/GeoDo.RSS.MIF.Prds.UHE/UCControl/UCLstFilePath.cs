using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.UHE
{
    public partial class UCLstFilePath : UserControl, IArgumentEditorUI
    {
        private Action<object> _handler;
        private LSTArgs _curLstArgs = null;
        private IArgumentProvider _argProvider = null;
        private string _algorithmName = null;

        public UCLstFilePath()
        {
            InitializeComponent();
            InitControls();
            Load += new EventHandler(UCLstFilePath_Load);
        }

        private void InitControls()
        {
            ucObritDateDir.lbFile.Text = "局地文件路径";
            ucObritDateDir.SelectDir += new UCDirSelectBase.SelectDirHandler(SelectDir_Click);
        }

        void UCLstFilePath_Load(object sender, EventArgs e)
        {
            ReadLstArgs();
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private LSTArgs ReadLstArgs()
        {
            _curLstArgs = null;
            if (File.Exists(ArgsFileProvider.OrbitFilePath))
            {
                string[] contexts = File.ReadAllLines(ArgsFileProvider.OrbitFilePath, Encoding.Default);
                if (contexts == null || contexts.Length == 0)
                    return _curLstArgs;
                _curLstArgs = new LSTArgs();
                _curLstArgs.OrbitPath = new Dictionary<string, string>();
                string[] split = null;
                foreach (string item in contexts)
                {
                    split = item.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (split == null || split.Length < 2)
                        continue;
                    _curLstArgs.OrbitPath.Add(split[0], split[1]);
                }
                if (!string.IsNullOrEmpty(_algorithmName) && _curLstArgs != null && _curLstArgs.OrbitPath != null && _curLstArgs.OrbitPath.Count != 0 && _curLstArgs.OrbitPath.ContainsKey(_algorithmName))
                    ucObritDateDir.SetTextBoxInfo(_curLstArgs.OrbitPath[_algorithmName]);
                return _curLstArgs;
            }
            return null;
        }

        public void SelectDir_Click(TextBox txtDir)
        {
            SelectDir(txtDir);
        }

        private void SelectDir(TextBox txtDir)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtDir.Text = folderDialog.SelectedPath;
                    WriteLstArgs(txtDir.Text);
                }
            }
        }

        private void WriteLstArgs(string dir)
        {
            if (string.IsNullOrEmpty(dir))
                return;
            if (_curLstArgs == null)
                _curLstArgs = new LSTArgs();
            if (_curLstArgs.OrbitPath == null)
                _curLstArgs.OrbitPath = new Dictionary<string, string>();
            if (_curLstArgs.OrbitPath.ContainsKey(_algorithmName))
                _curLstArgs.OrbitPath[_algorithmName] = dir;
            else
                _curLstArgs.OrbitPath.Add(_algorithmName, dir);
            string argFileDir = Path.GetDirectoryName(ArgsFileProvider.OrbitFilePath);
            if (!Directory.Exists(argFileDir))
                Directory.CreateDirectory(argFileDir);
            File.WriteAllLines(ArgsFileProvider.OrbitFilePath, GetStringArrayByDic(_curLstArgs), Encoding.Default);
        }

        private string[] GetStringArrayByDic(LSTArgs _curLstArgs)
        {
            List<string> argsStr = new List<string>();
            foreach (string key in _curLstArgs.OrbitPath.Keys)
                argsStr.Add(key + "=" + _curLstArgs.OrbitPath[key]);
            return argsStr.Count == 0 ? null : argsStr.ToArray();
        }

        public object GetArgumentValue()
        {
            ReadLstArgs();
            return _curLstArgs;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            _algorithmName = ele.Value.ToString();
            if (_curLstArgs == null)
                ReadLstArgs();
            return _curLstArgs;
        }

        private void UCLstFilePath_VisibleChanged(object sender, EventArgs e)
        {
            if (_handler != null && this.Visible)
                _handler(GetArgumentValue());
        }
    }
}
