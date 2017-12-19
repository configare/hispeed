using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public partial class UCClousatSingleProfile : UserControl
    {
        public UCClousatSingleProfile()
        {
            InitializeComponent();
        }

        public UCClousatSingleProfile(ISmartSession session)
        {
            InitializeComponent();
            this.Text = "Cloudsat单点剖面显示";
            //_smartSession = session;
            //AddCanvasHost();
            //Load += new EventHandler(UCGbalFirRevise_Load);
            //SizeChanged += new EventHandler(UCGbalFirRevise_SizeChanged);
            //Disposed += new EventHandler(UCGbalFirRevise_Disposed);
            //this.lstbFilelist.MouseDoubleClick += new MouseEventHandler(lstbFilelist_MouseDoubleClick);
            //panel1.Visible = false;
            //InitializeInterface();
        }

        public void Free()
        {
        }

    }
}
