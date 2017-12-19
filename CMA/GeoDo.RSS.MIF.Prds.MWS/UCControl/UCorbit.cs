using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.Theme;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    public partial class UCorbit :  UserControl, IArgumentEditorUI
    {
        private Action<object> _handler;
        private string info = null;
        public UCorbit()
        {
            InitializeComponent();
        }
        # region IArgumentEditorUI 成员
        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }
        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return null;
        }
        public object GetArgumentValue()
        {
            return info;
        }
        #endregion

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void radioD_CheckedChanged(object sender, System.EventArgs e)
        {
            if(radioD.Checked)
                  info = "Descend";
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void radioA_CheckedChanged(object sender, System.EventArgs e)
        {
            if (radioA.Checked)
                info = "Ascend";
            if (_handler != null)
                _handler(GetArgumentValue());
        }
    }
}
