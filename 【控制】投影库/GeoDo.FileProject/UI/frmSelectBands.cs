using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using GeoDo.FileProject.UI;

namespace GeoDo.FileProject
{
    public partial class frmSelectBands : Form
    {
        private string _satelite;
        private string _sensor;

        private frmSelectBands()
        {
            InitializeComponent();
        }

        public frmSelectBands(string satelite,string sensor)
            :this()
        {
            _satelite = satelite;
            _sensor = sensor;
            
        }


    }
}
