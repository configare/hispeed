using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.UI;

namespace GeoDo.RSS.MIF.Prds.HAZ
{
    public partial class UCTimeZone : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handle = null;
        private string txtFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MonitoringProductArgs", "HAZ", "真彩图数据时区.txt");
        private bool _isInit = false;
        private List<string> _lst = new List<string>();
        private string _selectDir;
        private void InitListDir()
        {
            if (!File.Exists(txtFile))
                return;

            using (var sr = new StreamReader(txtFile, Encoding.GetEncoding("GB2312")))
            {
                string dirPath = sr.ReadLine();
                while (string.IsNullOrWhiteSpace(dirPath = sr.ReadLine()))
                {
                    if (!dirPath.Contains("选择："))
                        _lst.Add(dirPath);
                    else _selectDir = dirPath.Replace("选择：","");
                }
            }
        }

        private void Init()
        {
            //InitListDir();
            InitComboBox();
            SelectedValueChanged();
        }

        private void InitComboBox()
        {
            _isInit = true;
            comboBox1.Items.Clear();
            foreach (var lst in _lst)
            {
                comboBox1.Items.Add(lst);
            }
            comboBox1.Text = _selectDir;
            _isInit = false;
        }

        public UCTimeZone()
        {
            InitializeComponent();
        }

        #region IArgumentEditorUI2 成员

        public object GetArgumentValue()
        {
            string argument = comboBox1.Text;
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

        public object ParseArgumentValue(XElement ele)
        {
            if (ele == null)
                return null;
            foreach (XNode xNode in ele.Nodes())
            {
                var name = Convert.ToString((xNode as XElement).Name);
                var value = (xNode as XElement).Value;
                if (name == "Item")
                    _lst.Add(value);
                else if (name == "DefaultItem") _selectDir = value;
            }
            return null;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handle = handler;
        }

        #endregion

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            SelectedValueChanged();
        }

        private void SelectedValueChanged()
        {
            string value = comboBox1.Text;
            if (string.IsNullOrWhiteSpace(value))
                return;
            _selectDir = value;
            SaveList();

            if (_handle != null)
                _handle(GetArgumentValue());
        }

        private void SaveList()
        {
            //if (_isInit)
            //    return;
            
            //using (StreamWriter sw = new StreamWriter(txtFile, false, Encoding.GetEncoding("GB2312")))
            //{
            //    foreach (string s in _lst)
            //    {
            //        sw.WriteLine(s);
            //    }
            //    sw.WriteLine("选择：{0}", _selectDir);
            //}
        }
    }
}
