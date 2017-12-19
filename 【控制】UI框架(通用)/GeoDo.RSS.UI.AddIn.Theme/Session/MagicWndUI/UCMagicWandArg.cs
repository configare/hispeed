using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.UI.AddIn.CanvasViewer;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public partial class UCMagicWandArg : UserControl, IContextMenuArgProvider
    {
        private const string ARG_NAME_TOLERANCE = "tolerance";
        private const string ARG_NAME_ISCONTINUED = "iscontinued";

        public UCMagicWandArg()
        {
            InitializeComponent();
        }

        public void SetArg(string name, object value)
        {
            if (string.IsNullOrEmpty(name) || value == null)
                return;
            if (ARG_NAME_TOLERANCE == name.ToLower())
                txtTolerance.Value = int.Parse(value.ToString());
            else if (ARG_NAME_ISCONTINUED == name.ToLower())
                ckIsContinued.Checked = bool.Parse(value.ToString());
        }

        public object GetArg(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            if (ARG_NAME_TOLERANCE == name.ToLower())
                return (int)txtTolerance.Value;
            else if (ARG_NAME_ISCONTINUED == name.ToLower())
                return ckIsContinued.Checked;
            return null;
        }
    }
}
