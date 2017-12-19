using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GeoDo.FileProject;

namespace test
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
            //Test_Project.Hammer();
            //Application.Run(new frmProjectionALL());
            //Application.Run(new frmTestProjection());
            //Application.Run(new frmFY3OrbitProductProjection());
            Application.Run(new Form1());

        }
    }
}
