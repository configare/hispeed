using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Layout.Elements;

namespace test
{
    public partial class TestTemplateWindow : Form
    {
        public TestTemplateWindow()
        {
            InitializeComponent();
            UCTemplateSelectWindow tempSel = new UCTemplateSelectWindow();
            tempSel.Dock = DockStyle.Fill;
            this.Controls.Add(tempSel);
        }
    }
}
