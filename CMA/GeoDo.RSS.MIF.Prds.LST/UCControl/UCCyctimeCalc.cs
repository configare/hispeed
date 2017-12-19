using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.LST.UCControl
{
    public partial class UCCyctimeCalc : UserControl, IArgumentEditorUI
    {
        public UCCyctimeCalc()
        {
            InitializeComponent();
        }

        public object GetArgumentValue()
        {
            throw new NotImplementedException();
        }

        public void SetChangeHandler(Action<object> handler)
        {
            throw new NotImplementedException();
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            throw new NotImplementedException();
        }
    }
}
