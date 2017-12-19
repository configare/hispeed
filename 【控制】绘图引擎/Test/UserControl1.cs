using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Test
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
            Load += new EventHandler(UserControl1_Load);
            DoubleBuffered = true;
        }

        void UserControl1_Load(object sender, EventArgs e)
        {
            this.Select();
        }
    }
}
