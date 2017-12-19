using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.UI;


namespace GeoDo.RSS.MIF.Prds.FOG
{
    public partial class UCHistroyCSRSetting : UserControl, IArgumentEditorUI2
    {
        private string _fileDir = null;
        private Action<object> _handler;
        private string _filetype;
        private string _searchStr = "*.*";
        private bool _isExcuteArgumentValueChangedEvent = false;

        public UCHistroyCSRSetting()
        {
            InitializeComponent();
        }

        private string GetStringArgument(IArgumentProvider arp, string argument)
        {
            object obj = arp.GetArg(argument);
            if (obj == null)
                return null;
            return obj.ToString();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = GetFormatString(_filetype);
                dlg.InitialDirectory = _fileDir;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtFileDir.Text = dlg.FileName;
                    if(_handler!=null)
                       _handler(GetArgumentValue());
                }
            }
        }

        private string GetFormatString(string filetype)
        {
            switch (filetype.ToUpper())
            {
                case "LDF":
                    return HistroySRCFileType.LDFType;
                case "DAT":
                    return HistroySRCFileType.DatType;
                case "DATLDF":
                    return HistroySRCFileType.DatAndLDFType;
                default:
                    return HistroySRCFileType.LDFType;
            }
        }

        public object GetArgumentValue()
        {
            if (string.IsNullOrEmpty(txtFileDir.Text))
                return null;
            string crsFile = txtFileDir.Text;
            Dictionary<string, string> fileMap = new Dictionary<string, string>();
            fileMap.Add("0CSR", crsFile);
            string maxNDVI = crsFile.Replace("_0CSR_", "_MAXN_");
            if (File.Exists(maxNDVI))
                fileMap.Add("MAXN", maxNDVI);
            return fileMap;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        public object ParseArgumentValue(XElement ele)
        {
            GetFiletype(ele);
            Dictionary<string, string> csrStr = new Dictionary<string, string>();
            csrStr.Add("0CSR", "");
            csrStr.Add("MAXN", "");
            return csrStr;
        }

        private void GetFiletype(XElement ele)
        {
            if (ele == null)
                _filetype = null;
            XElement[] eles = ele.Elements("Type").ToArray();
            _filetype = eles[0].FirstAttribute.Value.ToString();
            switch (_filetype.ToUpper())
            {
                case "LDF":
                    _searchStr = "*.ldf";
                    break;
                case "DAT":
                    _searchStr = "*.dat";
                    break;
                case "DATLDF":
                    _searchStr = "*.*";
                    break;
                default:
                    _searchStr = "*.ldf";
                    break;
            }
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
            if(arp==null)
                return;
            string filename = arp.DataProvider.fileName;
            if (string.IsNullOrEmpty(filename)||!File.Exists(filename))
                filename = GetStringArgument(arp, "mainfiles");
            txtFileDir.Text = GetFileByDirArg.GetFileBySattileSensor(arp, "HistroyCSRDir", "DefHistroyCSRDir", filename);
            if (_handler != null)
                _handler(GetArgumentValue());
        }


        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return _isExcuteArgumentValueChangedEvent;
            }
            set
            {
                _isExcuteArgumentValueChangedEvent=value;
            }
        }
    }

    public static class HistroySRCFileType
    {
        public static string DatType = "SMART File(*.dat)|*.dat";
        public static string LDFType = "Histroy SRC File(*.ldf)|*.ldf";
        public static string DatAndLDFType = "Histroy SRC File(*.ldf,*.dat)|*.ldf;*.dat";
    }
}
