using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.CMA
{
    public partial class TemplateTypeSelector : Form
    {
        private SubProductInstanceDef[] _instanceDefs = null;
        private string _outFileIdentify = string.Empty;
        private string _templateName = string.Empty;
        private List<RadioButton> _radioes = null;

        public TemplateTypeSelector()
        {
            InitializeComponent();
            LoadInstance();
        }

        public TemplateTypeSelector(SubProductInstanceDef[] instanceDefs)
        {
            InitializeComponent();
            _instanceDefs = instanceDefs;
            LoadInstance();
        }

        public string OutFileIdentify
        {
            get { return _outFileIdentify; }
        }

        public string TemplateName
        {
            get { return _templateName; }
        }

        private void LoadInstance()
        {
            if (_instanceDefs == null || _instanceDefs.Length == 0)
                return;
            _radioes = new List<RadioButton>();
            for (int i = 0; i < _instanceDefs.Length; i++)
            {
                if (_instanceDefs[i] == null || string.IsNullOrEmpty(_instanceDefs[i].Name))
                    continue;
                CreatTemplateTypeRadioButton(_instanceDefs[i], i);
            }
        }

        bool _isFirst = true;
        int _leftBlank = 0;
        private void CreatTemplateTypeRadioButton(SubProductInstanceDef instance, int i)
        {
            RadioButton radio = new RadioButton();
            radio.Tag = instance;
            radio.Text = instance.Name;
            radio.Font = new Font("微软雅黑", 11);
            this.Controls.Add(radio);
            radio.AutoSize = true;
            radio.Top = (int)(radio.Height * 1.5 * i) + 25;
            if (_isFirst)
            {
                _leftBlank = (this.Width - radio.Width) / 2 - 5;
                _isFirst = false;
            }
            radio.Left = _leftBlank;
            _radioes.Add(radio);
        }

        private void ok_Click(object sender, EventArgs e)
        {
            if (_radioes == null || _radioes.Count == 0)
                return;
            foreach (RadioButton radio in _radioes)
            {
                if (radio.Checked)
                {
                    SubProductInstanceDef instance = radio.Tag as SubProductInstanceDef;
                    if (instance == null)
                        break;
                    _outFileIdentify = instance.OutFileIdentify;
                    _templateName = instance.LayoutName;
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                    return;
                }
            }
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
