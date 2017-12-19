using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.MIF.Prds.DST
{
    public class SubProductNCIMHDST : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductNCIMHDST(SubProductDef subProductDef)
            : base(subProductDef)
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;
            if (_argumentProvider == null)// || _argumentProvider.DataProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "NCIMAlgorithm")
            {
                return NCIMAlgorithm();
            }
            else
            {
                PrintInfo("指定的算法没有实现。");
                return null;
            }
        }

        private IExtractResult NCIMAlgorithm()
        {
            //工作路径
            string outImageDir = Convert.ToString(_argumentProvider.GetArg("OutImageDir"));

            if (string.IsNullOrWhiteSpace(outImageDir))
            {
                MessageBox.Show("没有设置工作路径，不能进行数据处理！");
                return null;
            }
            string day = DateTime.Now.ToString("yyyyMMdd");
            string outdir = Path.Combine(outImageDir, day);
            if (!Directory.Exists(outdir))
                Directory.CreateDirectory(outdir);

            //真彩图处理逻辑
            string natrueColorFile = Convert.ToString(_argumentProvider.GetArg("NatrueColorFile"));
            if (string.IsNullOrWhiteSpace(natrueColorFile))
                return null;
            string[] files = natrueColorFile.Split(new char[] {','});
            if (files.Length != 2)
                return null;
            string filename = files[0];

            string newfilename = files[1];
            string newfile = Path.Combine(outdir, newfilename);
            if (File.Exists(newfile))
            {
                try
                {
                    File.Delete(newfile);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("新文件被占用不能删除，请手动删除后尝试运行！");
                    return null;
                }
            }
            File.Copy(filename, newfile);

            OpenFileFactory.Open(newfile);



            return null;
        }

      

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

    }
}
