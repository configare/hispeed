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

namespace GeoDo.RSS.MIF.Prds.HAZ
{
    public class SubProductNCIMHAZE : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProductNCIMHAZE(SubProductDef subProductDef)
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
            string[] files = natrueColorFile.Split(new char[] { ',' });
            if (files.Length != 2)
                return null;
            string filename = files[0];

            string newfilename = files[1];
            string newfile = Path.Combine(outdir, newfilename);
            if (filename.ToUpper() != newfile.ToUpper())
            {
                if (File.Exists(newfile))
                {
                    try
                    {
                        File.Delete(newfile);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("新文件被占用不能删除，请手动删除后尝试运行！");
                        return null;
                    }
                }
                File.Copy(filename, newfile);
            }

            OpenFileFactory.Open(newfile);
            bool onlyOpen = bool.Parse(_argumentProvider.GetArg("OnlyOpen").ToString());
            if (onlyOpen)
                return null;

            float resolution = 0f;
            int width = 0, height = 0;
            string rasterfilename = GetRasterFilenameFromDrawing(out resolution, out width, out height);
            IMonitoringSession ms = _argumentProvider.EnvironmentVarProvider as IMonitoringSession;

            IMonitoringSubProduct subDef = ms.ActiveMonitoringProduct.GetSubProductByIdentify("0IMG");
            if (subDef != null)
                try
                {
                    subDef.Definition.IsKeepUserControl = true;

                    ms.ChangeActiveSubProduct("0IMG");
                    ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "NCIM");   // 低分  "霾监测示意图模板");
                    string tempname1 = GetThemeGraphTemplateName(resolution, width, height, TemplateType.低分);
                    ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("HEAThemeGraphTemplateName", tempname1);
                    ms.DoAutoExtract(false);

                    ms.ChangeActiveSubProduct("0IMG");
                    ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "TNCI");   // 原始分辨率
                    //string tempname2 = GetThemeGraphTemplateName(resolution, width, height, TemplateType.低分);
                    ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("HEAThemeGraphTemplateName", "");
                    ms.DoAutoExtract(false);

                    ms.ChangeActiveSubProduct("0IMG");
                    ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "ONCI");   // 原始
                    string tempname3 = GetThemeGraphTemplateName(resolution, width, height, TemplateType.原始);
                    ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("HEAThemeGraphTemplateName", tempname3);
                    ms.DoAutoExtract(false);

                    ms.ChangeActiveSubProduct("NCIM");
                }
                finally
                {
                    if (subDef != null)
                        subDef.Definition.IsKeepUserControl = false;
                }
            return null;
        }

        private long _themeGraphResolution = 17950640L; // 17950640 = 5360 * 3349  1000
        private long _themeGraphResolution250 = 287210240L; // 71802560 = 5360 * 3349 * 16
        private string GetThemeGraphTemplateName(float resolution, int width, int height, TemplateType templateType)
        {
            //double rsd = (width*height)/Convert.ToDouble(resolution*100);
            //double rs = rsd/_themeGraphResolution;
            string rss = "霾监测示意图模板_全国1000 ";
            //if (rs <= 1 / 3)        //1000 全国1/4 
            //    rss = "霾监测示意图模板_4全国1000";
            //else if (rs <= 2 / 3)   //1000 全国1/2
            //    rss = "霾监测示意图模板_2全国1000";
            //else if (rs <= 1.0)     //1000 全国标准
            //    rss = "霾监测示意图模板_全国1000";
            //else if (rs <= 4 / 3)   // 250 全国1/4
            //    rss = "霾监测示意图模板_4全国250";
            //else if (rs <= 8 / 3)   // 250 全国1/2
            //    rss = "霾监测示意图模板_2全国250";
            //else                    // 250 全国
            //    rss = "霾监测示意图模板_全国250";
            long rsl = width * height;
            if (Math.Abs(resolution - 0.0025f) < float.Epsilon)
            {
                if (rsl <= _themeGraphResolution250 * 2 / 5)
                    rss = "霾监测示意图模板_4全国250";
                else if (rsl <= _themeGraphResolution250 * 4 / 5)
                    rss = "霾监测示意图模板_2全国250";
                else
                    rss = "霾监测示意图模板_全国250";
            }
            else  // (Math.Abs(resolution - 0.01) < float.Epsilon)
            {
                if (rsl <= _themeGraphResolution * 2 / 5)
                    rss = "霾监测示意图模板_4全国1000";
                else if (rsl <= _themeGraphResolution * 4 / 5)
                    rss = "霾监测示意图模板_2全国1000";
                else
                    rss = "霾监测示意图模板_全国1000";
            }


            string outs = string.Format("{0}_{1}", rss, templateType);

            return outs;
        }

        enum TemplateType
        {
            原始,
            低分
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
