using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.CMA;


namespace GeoDo.RSS.MIF.Prds.CLD
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new GPCPreProcess());
            Application.Run(new frmMod06DataPro());//"AIRS"
            //Application.Run(new frmMod06DataPro("AIRS"));//
            //GPCPreProcess frm = new GPCPreProcess();
            //frm.Show();
            //Application.Run(new FileToDatabase());
        }
    }

}
