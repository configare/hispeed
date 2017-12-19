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

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public partial class UCAdjustConfigEdit : UserControl, IConfigEdit
    {
        private AdjustConfig _adjustCfg;

        public UCAdjustConfigEdit()
        {
            InitializeComponent();
            this.Load += new EventHandler(UCAdjustConfigEdit_Load);
        }

        private bool IsChanged()
        {
            if (_adjustCfg == null)
                return false;
            string IsOpenResult = _adjustCfg.GetConfigValue("IsOpenResult");
            return IsOpenResult != ckbIsOpenResult.Checked.ToString();
        }

        private void UCAdjustConfigEdit_Load(object sender, EventArgs e)
        {
            _adjustCfg = new AdjustConfig();
            bool IsOpenResult;
            if (bool.TryParse(_adjustCfg.GetConfigValue("IsOpenResult"), out IsOpenResult))
            {
                ckbIsOpenResult.Checked = IsOpenResult;
            }
        }

        bool IConfigEdit.TrySaveConfig(out string message)
        {
            message = "";
            if (!IsChanged())
                return true;
            //保存至文件
            _adjustCfg.SetConfig("IsOpenResult", ckbIsOpenResult.Checked.ToString());
            return _adjustCfg.Save(out message);
        }
    }
}
