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
    public partial class TestElementToolbox : Form
    {
        public TestElementToolbox()
        {
            InitializeComponent();
            UCElementsListView eleList = new UCElementsListView();
            eleList.Dock = DockStyle.Fill;
            this.Controls.Add(eleList);
        }
    }
}
