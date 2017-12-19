using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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

namespace GeoDo.RSS.MIF.Prds.PrdVal
{
    public partial class UCCLDdataset : UserControl,IArgumentEditorUI
    {
        private Action<object> _handler;
        private string info = null;
        public UCCLDdataset()
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

        private void cmbSets_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            info = cmbSets.SelectedItem.ToString();
            if (_handler != null)
                _handler(GetArgumentValue());
        }
    }
}
