using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.BlockOper;

namespace GeoDo.RSS.UI.AddIn.DataPro.BlockOper.Commands
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandMoasic : Command
    {
        public CommandMoasic()
        {
            _id = 4201;
            _name = "Moasic";
            _text = "拼接/镶嵌";
            _toolTip = "拼接/镶嵌";
        }

        public override void Execute()
        {
            try
            {
                base.Execute();
                ISmartWindow smartWindow = _smartSession.SmartWindowManager.GetSmartWindow((w) => { return w.GetType() == typeof(frmDataMoasic); });
                if (smartWindow == null)
                {
                    smartWindow = new frmDataMoasic(_smartSession);
                    _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
                }
                else
                    _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OpenFileToWindows(string file)
        {
            OpenFileFactory.Open(file);
        }

        private string MoasicFiles(List<string> srcFiles, bool processInvaild, string[] invaildValues, string outDir, IProgressMonitor progress)
        {
            try
            {
                int count = srcFiles.Count;
                IRasterDataProvider[] srcRaster = new IRasterDataProvider[count];
                for (int i = 0; i < count; i++)
                {
                    IRasterDataProvider src = GeoDataDriver.Open(srcFiles[i]) as IRasterDataProvider;
                    srcRaster[i] = src;
                }
                if (progress != null)
                {
                    progress.Reset("", 100);
                    progress.Start(false);
                }
                RasterMoasicProcesser processer = new RasterMoasicProcesser();
                IRasterDataProvider dstRaster = processer.Moasic(srcRaster, "LDF", outDir, processInvaild, invaildValues,
                    new Action<int, string>((int progerss, string text) =>
                        {
                            if (progress != null)
                                progress.Boost(progerss, text);
                        }));
                string dstFileName = dstRaster.fileName;
                for (int i = 0; i < count; i++)
                {
                    srcRaster[i].Dispose();
                }
                dstRaster.Dispose();
                return dstFileName;
            }
            finally
            {
                if (progress != null)
                    progress.Finish();
            }
        }
    }
}
