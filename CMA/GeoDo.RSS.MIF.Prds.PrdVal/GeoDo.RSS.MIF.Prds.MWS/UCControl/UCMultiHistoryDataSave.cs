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
    public partial class UCMultiHistoryDataSave : UserControl, IArgumentEditorUI
    {
        private Action<object> _handler;
        private string info = null;
        public UCMultiHistoryDataSave()
        {
            InitializeComponent();
            info = txtFileDir.Text;
            
        }

        private void labBackWater_Click(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgumentValue());
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
        

        private void txtFileDir_TextChanged(object sender, EventArgs e)
        {

            if (_handler != null)
                  _handler(GetArgumentValue());
          
        }

        private void btnsavefile_Click_1(object sender, EventArgs e)
        {
            txtFileDir.Text = "";
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    SetOutDir(dlg.SelectedPath);
                }
            }
            
            if (_handler != null)
                _handler(GetArgumentValue());
        }
        private void SetOutDir(string dir)
        {
            txtFileDir.Text = dir;
            info = txtFileDir.Text;
        }
    }
}
