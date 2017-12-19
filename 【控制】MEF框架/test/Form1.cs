using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeoDo.MEF;
using GeoDo.RSS.Core.DF;

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] dlls = MefConfigParser.GetAssemblysByCatalog("数据驱动");

            string fname = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "LoadOrder.xml");
            string[] fnames = (new LoadOrderParser()).Parse(fname);

            using (IComponentLoader<IGeoDataDriver> loader = new ComponentLoader<IGeoDataDriver>())
            {
                object[] obj = loader.LoadComponents(dlls);
            }
        }
    }
}
