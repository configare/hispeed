using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace GeoDo.Tools.ReNameFY
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("批量重命名工具启动");
            Console.WriteLine("******************");
            Console.WriteLine("支持将类似如下文件");
            Console.WriteLine("Z_SATE_C_BAWX_20130321034403_P_FY3B_MERSI_GBAL_L1_20110220_0510_0250M_MS.HDF");
            Console.WriteLine("Z_SATE_C_BAWX_20130321034729_P_FY3B_MERSI_GBAL_L1_20110220_0510_1000M_MS.HDF");
            Console.WriteLine("重命名为");
            Console.WriteLine("FY3B_MERSI_GBAL_L1_20110220_0510_0250M_MS.HDF");
            Console.WriteLine("FY3B_MERSI_GBAL_L1_20110220_0510_1000M_MS.HDF");
            Console.WriteLine("******************");
            Console.WriteLine();
            string arg = null;
            if (args != null && args.Length != 0)
            {
                arg = args[0];
                if (!Directory.Exists(arg))
                    return;
            }
            else
            {
                while (string.IsNullOrWhiteSpace(arg))
                {
                    Console.WriteLine("请输入待重命名的文件目录");
                    arg = Console.ReadLine();
                    if (!Directory.Exists(arg))
                    {
                        Console.WriteLine("输入的文件目录不存在");
                        arg = null;
                    }
                }
            }
            Regex regex = new Regex(@"Z_SATE_C_BAWX_\d{14}_P_(?<filiename>FY3B_MERSI_GBAL_L1_\d{8}_\d{4}_\d{4}M_MS.HDF)", RegexOptions.Compiled);

            string[] files = Directory.GetFiles(arg, "*.HDF");
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                string filenameonly = Path.GetFileName(file);
                if (regex.IsMatch(filenameonly))
                {
                    string rename = "";
                    Match match = regex.Match(filenameonly);
                    if (!match.Success)
                        continue;
                    Group gp = match.Groups["filiename"];
                    if (!gp.Success)
                        continue;
                    rename = gp.Value;
                    Console.WriteLine(filenameonly);
                    Console.WriteLine("重命名为");
                    Console.WriteLine(rename);
                    rename = Path.Combine(Path.GetDirectoryName(file), rename);
                    TryRename(file, rename);
                }
            }
            Console.WriteLine("重命名完毕");
        }

        private static void TryRename(string file, string rename)
        {
            try
            {
                File.Move(file, rename);
            }
            catch
            { 
            }
        }
    }
}
