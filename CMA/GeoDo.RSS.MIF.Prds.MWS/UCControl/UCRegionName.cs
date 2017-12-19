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
using GeoDo.RSS.Core.DF;
namespace GeoDo.RSS.MIF.Prds.MWS
{
    public partial class UCRegionName : UserControl, IArgumentEditorUI
    {
        private Action<object> _handler;
        private string info = null;
        public UCRegionName()
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            info = textBox1.Text;
            if (_handler != null)
                _handler(GetArgumentValue());
        }
        //private void SetOutDir(string dir)
        //{
        //    textBox1.Text = dir;
        //    info = textBox1.Text;
        //}
    }
}
