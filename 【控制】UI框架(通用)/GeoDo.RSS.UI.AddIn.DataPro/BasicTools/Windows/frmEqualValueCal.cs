using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public partial class frmEqualValueCal : Form
    {
        public static EventHandler FormClosedHandler = null;

        public frmEqualValueCal()
        {
            InitializeComponent();
            FormClosed += new FormClosedEventHandler(frmEqualValueCal_FormClosed);
        }

        public frmEqualValueCal(ISmartSession session)
        {
            InitializeComponent();
            ucEqualValueCal evc = new ucEqualValueCal(session);
            this.Controls.Add(evc);
            FormClosed += new FormClosedEventHandler(frmEqualValueCal_FormClosed);
        }

        void frmEqualValueCal_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (FormClosedHandler != null)
            //    FormClosedHandler(txtOutputtShapeFile.Text, e);
            //OutFileDir = null;
            //_openFilename = string.Empty;
        }

    }
}
