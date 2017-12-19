using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Layout.GDIPlus;
using GeoDo.RSS.Layout;
using GeoDo.RSS.UI.AddIn.Layout;
using System.Drawing;
using System.Drawing.Imaging;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public class AutoGeneratorSettings
    {
        public static AutoGeneratorSettings CurrentSettings;
        public ISmartSession Session;
        public List<string> GeneratedFileNames;
        public List<string> GxdFileNames;
        public bool IsOutputGxd = true;
        public bool IsOutputPng = true;
        public string FolderOfCopyTo;
        public bool OpenFileAfterFinished = false;
        public bool NeedSaveSettings = false;
        public enumActionOfExisted ActionOfExisted = enumActionOfExisted.Overide;

        public enum enumActionOfExisted
        {
            Overide,
            ReName,
            Skip
        }

        public void ExportImageOfGxdFiles()
        {
            if (!AutoGeneratorSettings.CurrentSettings.IsOutputPng)
                return;
            if (AutoGeneratorSettings.CurrentSettings.GxdFileNames == null || AutoGeneratorSettings.CurrentSettings.GxdFileNames.Count == 0)
                return;
            bool isNeedCopy = !string.IsNullOrEmpty(AutoGeneratorSettings.CurrentSettings.FolderOfCopyTo);
            if (isNeedCopy)
                AutoGeneratorSettings.CurrentSettings.GeneratedFileNames.Clear();
            IContextMessage msg = Session.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(9006) as IContextMessage;
            if (msg != null)
                msg.PrintMessage("开始导出专题图图片......");
            foreach (string fname in GxdFileNames)
            {
                IGxdDocument doc = GxdDocument.LoadFrom(fname);
                if (doc == null)
                {
                    if (msg != null)
                        msg.PrintMessage("专题图\"" + fname + "\"打开失败。");
                    return;
                }
                using (LayoutViewer viewer = new LayoutViewer())
                {
                    viewer.LayoutHost.ApplyGxdDocument(doc);
                    using (Bitmap bm = viewer.LayoutHost.ExportToBitmap(System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                    {
                        if (bm == null)
                        {
                            if (msg != null)
                                msg.PrintMessage("专题图\"" + fname + "\"导出图片失败。");
                            return;
                        }
                        string fileName = Path.Combine(Path.GetDirectoryName(fname), Path.GetFileNameWithoutExtension(fname) + ".png");
                        bm.Save(fileName, ImageFormat.Png);
                        if (isNeedCopy)
                            AutoGeneratorSettings.CurrentSettings.GeneratedFileNames.Add(fileName);
                    }
                }
                if (msg != null)
                    msg.PrintMessage("已导出专题图\"" + fname + "\"。");
            }
            //
            if (msg != null)
                msg.PrintMessage("导出专题图图片结束。");
            //
            CopyFileToCopyFolder();
        }

        public void CopyFileToCopyFolder()
        {
            if (string.IsNullOrEmpty(FolderOfCopyTo) || GeneratedFileNames == null || GeneratedFileNames.Count == 0)
                return;
            IContextMessage msg = Session.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(9006) as IContextMessage;
            if (msg != null)
                msg.PrintMessage("开始拷贝文件......");
            string fileName;
            foreach (string fname in GeneratedFileNames)
            {
                try
                {
                    fileName = Path.GetFileName(fname);
                    File.Copy(fname, Path.Combine(FolderOfCopyTo, fileName));
                    msg.PrintMessage("拷贝文件\"" + fileName + "\" 到 " + FolderOfCopyTo);
                }
                catch (Exception ex)
                {
                    if (msg != null)
                        msg.PrintMessage(ex.Message);
                }
            }
            if (msg != null)
                msg.PrintMessage("开始拷贝文件结束。");
        }
    }
}
