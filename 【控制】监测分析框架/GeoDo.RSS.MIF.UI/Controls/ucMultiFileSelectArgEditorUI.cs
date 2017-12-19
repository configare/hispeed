using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.UI
{
    public partial class ucMultiFileSelectArgEditorUI : UserControl,IArgumentEditorUI2
    {
        private bool _isExcuteArgumentValueChangedEvent = false;
        private MultiFileSelect _ucFiles = null;
        private Action<object> _handler;

        public ucMultiFileSelectArgEditorUI()
        {
            InitializeComponent();
            _ucFiles = new MultiFileSelect();
            _ucFiles.FileFilter = "";
            _ucFiles.FileChanged += new Action<object>(_ucFiles_FileChanged);
            _ucFiles.Dock = DockStyle.Fill;
            this.Controls.Add(_ucFiles);
        }

        void _ucFiles_FileChanged(object obj)
        {
            if (_handler != null)
                _handler(_ucFiles.Files);
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

        IArgumentProvider _argumentProvider;
        IWorkspace _workspace;

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            _ucFiles.Title = arg.Description;
            List<string> files = new List<string>();
            _argumentProvider = panel.MonitoringSubProduct.ArgumentProvider;
            _workspace = panel.Workspace;
            IRasterDataProvider rad = _argumentProvider.DataProvider;
            if (panel.MonitoringSubProduct.Definition.IsNeedCurrentRaster && rad != null)
            {
                files.Add(rad.fileName);
            }
            _argName = (arg as ArgumentDef).Name;
            _refIdentify = (arg as ArgumentDef).RefIdentify;
            _fileFilter = (arg as ArgumentDef).RefFilter;
            _ucFiles.FileFilter = _fileFilter;
            string[] sfiles = GetSelectedFiles();
            if (sfiles != null)
                files.AddRange(sfiles);
            _ucFiles.Files = files.ToArray();
        }

        private string[] GetSelectedFiles()
        {
            IEnvironmentVarProvider varPrd = _argumentProvider.EnvironmentVarProvider;
            if (varPrd != null)
            {
                object varValue = varPrd.GetVar(_refIdentify);
                if (varValue != null)
                {
                    if (varValue is string && !string.IsNullOrWhiteSpace((string)varValue))
                        return new string[] { varValue.ToString() };
                    if (varValue is string[] && (varValue as string[]).Length != 0)
                        return varValue as string[];
                }
                if (_workspace != null)
                {
                    string[] selectedFile = null;
                    if (_workspace.ActiveCatalog != null)
                        selectedFile = _workspace.ActiveCatalog.GetSelectedFiles(_refIdentify);
                    return selectedFile;
                }
            }
            return null;
        }

        public object GetArgumentValue()
        {
            return _ucFiles.Files;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        string _argName;
        string _refIdentify;
        string _fileFilter;
        /// <summary>
        /// <arg argName="" refIdentify="" reffilter="">
        /// </summary>
        /// <param name="ele"></param>
        /// <returns></returns>
        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return _ucFiles.Files;
        }
    }
}
