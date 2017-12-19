using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GeoDo.Smart.Tools.NSMCDataCoordCorrect
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
            Application.Run(new Main());

        }

        static void GetCode()
        {
            LSTBlock10Modify lb = new LSTBlock10Modify();

            BlockItem[] items = lb.GetAllBlockItems();
            foreach (BlockItem item in items)
            {
                Console.WriteLine(string.Format("{0},{1},{2},{3},{4}", item.Name, item.MinX, item.MaxX, item.MinY, item.MaxY));
            }
        }
    }
}
