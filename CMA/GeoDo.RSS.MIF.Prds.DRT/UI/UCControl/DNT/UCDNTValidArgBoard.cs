using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public partial class UCDNTValidArgBoard : UserControl, IArgumentEditorUI
    {
        private Action<object> _handler;
        private Dictionary<string, Int16> _result = null;

        public UCDNTValidArgBoard()
        {
            InitializeComponent();
        }

        public object GetArgumentValue()
        {
            _result = new Dictionary<string, short>();
            string cloud = txtCloud.Text;
            if (string.IsNullOrEmpty(cloud))
                return null;
            Int16 cloudValue = Int16.Parse(cloud);
            string valid = txtValid.Text;
            if (string.IsNullOrEmpty(valid))
                return null;
            Int16 validValue = Int16.Parse(valid);
            _result.Add("CloudValue", cloudValue);
            _result.Add("ValidValue", validValue);
            return _result;
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return GetArgumentValue();
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgumentValue());
        }
    }
}
