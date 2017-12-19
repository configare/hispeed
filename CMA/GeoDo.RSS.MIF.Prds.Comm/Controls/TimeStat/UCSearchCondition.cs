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
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    public partial class UCSearchCondition : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler;
        private bool _isExcuteArgumentValueChangedEvent = false;
        private SearchConditionClass _searchCondition = null;
        private Dictionary<string, string> _lakeDic = new Dictionary<string, string>();
        private string _argsFilename;

        public UCSearchCondition()
        {
            InitializeComponent();
            _searchCondition = new SearchConditionClass();
            ucStatTimeRegion1.ArgChanged = new UCStatTimeRegion.ArgChangedDelegate(ArgChanged);
            ucStatTimeFrame1.ArgChanged = new UCStatTimeFrame.ArgChangedDelegate(ArgChanged);
            dbBegin.Value = DateTime.Now.AddYears(-5);
        }

        public object GetArgumentValue()
        {
            if (_searchCondition == null)
                _searchCondition = new SearchConditionClass();
            _searchCondition.BeginDate = dbBegin.Value;
            _searchCondition.EndDate = dbEnd.Value;
            _searchCondition.RegionIdentify = GetIdentifyByName();
            _searchCondition.RegionName = cbLake.Text;
            _searchCondition.Resolution = GetResultByCb();
            _searchCondition.TimeQuantum = rbTimeFrame.Checked ? ucStatTimeFrame1.TimeFrame : null;
            _searchCondition.TimeRegion = rbTimeRegion.Checked ? ucStatTimeRegion1.TimeRegion : null;
            return _searchCondition;
        }

        private int GetResultByCb()
        {
            if (string.IsNullOrEmpty(cbResolution.Text))
                return -1;
            string temp;
            int result = -1;
            if (cbResolution.Text.ToUpper().IndexOf("KM") != -1)
            {
                temp = cbResolution.Text.ToUpper().Replace("KM", "");
                if (int.TryParse(temp, out result))
                    return result > 1 ? result : result * 1000;
                else
                    return -1;
            }
            if (cbResolution.Text.ToUpper().IndexOf("M") != -1)
            {
                temp = cbResolution.Text.ToUpper().Replace("M", "");
                if (int.TryParse(temp, out result))
                    return result;
                else
                    return -1;
            }
            if (cbResolution.Text.ToUpper().IndexOf("米") != -1)
            {
                temp = cbResolution.Text.ToUpper().Replace("米", "");
                if (int.TryParse(temp, out result))
                    return result;
                else
                    return -1;
            }
            if (cbResolution.Text.ToUpper().IndexOf("公里") != -1)
            {
                temp = cbResolution.Text.ToUpper().Replace("公里", "");
                if (int.TryParse(temp, out result))
                    return result > 1 ? result : result * 1000;
                else
                    return -1;
            }
            if (cbResolution.Text.ToUpper().IndexOf("千米") != -1)
            {
                temp = cbResolution.Text.ToUpper().Replace("千米", "");
                if (int.TryParse(temp, out result))
                    return result > 1 ? result : result * 1000;
                else
                    return -1;
            }
            return result;
        }

        private string GetIdentifyByName()
        {
            if (_lakeDic.Count == 0)
                return cbLake.Text;
            if (_lakeDic.ContainsKey(cbLake.Text))
                return _lakeDic[cbLake.Text];
            return cbLake.Text;
        }

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            _lakeDic.Clear();
            cbLake.Items.Clear();
            if (!File.Exists(_argsFilename))
            {
                InitLakeResulotion();
                return;
            }
            string[] argContent = File.ReadAllLines(_argsFilename, Encoding.Default);
            string[] stringSplit = null;
            for (int i = 0; i < argContent.Length; i++)
            {
                if (string.IsNullOrEmpty(argContent[i]))
                    continue;
                stringSplit = argContent[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (stringSplit == null || stringSplit.Length < 2)
                    continue;
                _lakeDic.Add(stringSplit[0], stringSplit[1]);
                cbLake.Items.Add(stringSplit[0]);
            }
            InitLakeResulotion();
        }

        private void InitLakeResulotion()
        {
            if (cbLake.Items.Count == 0)
                cbLake.Items.Add("未定义");
            if (cbResolution.Items.Count == 0)
                cbResolution.Items.Add("未定义");
            cbLake.SelectedIndex = 0;
            cbResolution.SelectedIndex = 0;
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

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            var argEle = ele.Elements("ArgFile");
            string argFile = "";
            if (argEle != null && argEle.Count() != 0)
                argFile = argEle.ToArray()[0].Value;
            if (!string.IsNullOrEmpty(argFile))
                _argsFilename = argFile.StartsWith("\\") ? AppDomain.CurrentDomain.BaseDirectory + argFile : argFile;
            return GetArgumentValue();
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        private void rbTimeFrame_CheckedChanged(object sender, EventArgs e)
        {
            ucStatTimeFrame1.Visible = rbTimeFrame.Checked;
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void rbTimeRegion_CheckedChanged(object sender, EventArgs e)
        {
            ucStatTimeRegion1.Visible = rbTimeRegion.Checked;
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void ArgChanged()
        {
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void dbBegin_ValueChanged(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void dbEnd_ValueChanged(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void cbLake_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void cbResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void btEditArgs_Click(object sender, EventArgs e)
        {
            using (frmEditTimeStatArg frm = new frmEditTimeStatArg(_argsFilename))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    _lakeDic = frm.ResultDic;
                    InitControl(null, null);
                }
            }
        }
    }
}
