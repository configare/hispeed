using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.UI
{
    public partial class ucAOITemplateArgEditorUI : UserControl,IArgumentEditorUI2
    {
        private bool _isExcuteArgumentValueChangedEvent = false;
        private ucAOITemplate _ucAOITemplate = null;
        private Action<object> _handler;

        public ucAOITemplateArgEditorUI()
        {
            InitializeComponent();
            _ucAOITemplate = new ucAOITemplate();
            _ucAOITemplate.Dock = DockStyle.Fill;
            _ucAOITemplate.AOIChangedHandler += new OnAOITempleteChangedHandler(_ucAOITemplate_AOIChangedHandler);
            this.Controls.Add(_ucAOITemplate);
        }

        void _ucAOITemplate_AOIChangedHandler(object sender, string[] aois)
        {
            if (_handler != null)
                _handler(_ucAOITemplate.Aois);
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

         public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            _ucAOITemplate.BuildControl(panel.MonitoringSubProduct.Definition.ProductDef.AOITemplates, panel.MonitoringSubProduct.Definition.AoiTemplates);
        }

        public object GetArgumentValue()
        {
            return _ucAOITemplate.Aois;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            List<string> templates = new List<string>();
            IEnumerable<XElement> node = ele.Elements("Value");
            if (node != null && node.Count() != 0)
            {
                foreach (XElement item in node)
                {
                    string value = item.Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        templates.Add(value);
                    }
                }
                return templates.ToArray();
            }
            return null;
        }
    }
}
