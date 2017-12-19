using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.UI.WinForm;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.Core.DF;

namespace GeoDo.Smart.AutoNDVI
{
    public class Producter
    {
        ISmartSession _session;
        IContextMessage _context;

        public Producter()
        {
            frmMainForm mainForm = new frmMainForm(new StartProgress());
            _session = mainForm.Session;
            _context = new ContentText();
        }

        /// <summary>
        /// NDVI\0RVI\0DVI\0EVI
        /// 0VCI
        /// </summary>
        /// <param name="args"></param>
        public void Make(string subproduct, string ldfFile, string outfile)
        {
            PrintInfo("启动计算");
            try
            {
                //LstArgs arg = new LstArgs(args);
                //if (arg == null)
                //    PrintInfo("输入参数为空，计算终止");
                //string ldf = arg.LdfFile;
                //string ndvi = arg.NDVIFile;
                //string outfile = arg.OutFile;
                if (string.IsNullOrWhiteSpace(subproduct))
                {
                    PrintInfo("输出文件设置为空，计算终止");
                    return;
                }
                if (string.IsNullOrWhiteSpace(ldfFile))
                {
                    PrintInfo("输入ldf局地文件为空，计算终止");
                    return;
                }
                if (string.IsNullOrWhiteSpace(outfile))
                {
                    PrintInfo("输出文件设置为空，计算终止");
                    return;
                }
                string subProductId = subproduct;
                string rstFileName = ldfFile;
                //int zoomValue = 1;
                //int.TryParse(zoom, out zoomValue);
                IMonitoringSubProduct subprd = CreateMonitoringSubProduct(rstFileName, "VGT", subProductId);
                if (subprd == null)
                {
                    PrintInfo(string.Format("创建子产品[{0}]失败,计算终止",subProductId));
                    return;
                }
                string resultFile = null;
                //下面需要设置文件来源参数
                using (IRasterDataProvider raster = GeoDataDriver.Open(rstFileName) as IRasterDataProvider)
                {
                    subprd.ArgumentProvider.DataProvider = raster;
                    subprd.ArgumentProvider.SetArg("RasterFile", rstFileName);
                    subprd.ArgumentProvider.SetArg("OutFile", outfile);
                    IExtractResult result = subprd.Make(null, _context);
                    if (result is ExtractResultArray)
                        resultFile = ((result as ExtractResultArray).PixelMappers[0] as IFileExtractResult).FileName;
                }
                //搬运文件，或者裁切文件。
                TryCutFile(resultFile, outfile);
            }
            catch (Exception ex)
            {
                PrintInfo(ex.Message);
            }
            finally
            {
                PrintInfo("结束计算");
            }
        }

        /// <summary>
        /// 转移文件及其附属文件，包括hdr等文件
        /// </summary>
        /// <param name="resultFile"></param>
        /// <param name="outfile"></param>
        private void TryCutFile(string resultFile, string outfile)
        {
            if (resultFile != outfile)
            {
                if (File.Exists(resultFile))
                {
                    File.Copy(resultFile, outfile);
                    TryDeleteFile(resultFile);
                    string hdr = Path.ChangeExtension(resultFile, ".hdr");
                    string dsthdr = Path.ChangeExtension(outfile, ".hdr");
                    if (File.Exists(hdr))
                    {
                        File.Copy(hdr, dsthdr);
                        TryDeleteFile(hdr);
                    }
                }
            }
        }

        private void TryDeleteFile(string file)
        {
            try
            {
                File.Delete(file);
            }
            catch(Exception ex)
            {
                PrintInfo(ex.Message);
            }
        }

        private IMonitoringSubProduct CreateMonitoringSubProduct(string rstFileName, string productIdentify, string subProductIdentify)
        {
            MonitoringSession session = new MonitoringSession(_session);
            IMonitoringProduct monitorProduct = session.ChangeActiveProduct(productIdentify);
            IMonitoringSubProduct subprd = session.ChangeActiveSubProduct(subProductIdentify);
            if (monitorProduct == null || subprd == null)
                return null;
            RasterIdentify rstIdentify = new RasterIdentify(rstFileName);
            ExtractAlgorithmIdentify id = new ExtractAlgorithmIdentify();
            id.Satellite = rstIdentify.Satellite;
            id.Sensor = rstIdentify.Sensor;
            AlgorithmDef alg = subprd.Definition.GetAlgorithmDefByAlgorithmIdentify(id);
            if (alg == null)
            {
                PrintInfo("没有匹配的算法：" + "卫星" + rstIdentify.Satellite + "，传感器" + rstIdentify.Sensor);
                return null;
            }
            subprd.ResetArgumentProvider(alg.Identify);
            subprd.ArgumentProvider.SetArg("AlgorithmName", alg.Identify);
            subprd.ArgumentProvider.SetArg("FileNameGenerator", FileNameGeneratorDefault.GetFileNameGenerator());
            if (alg.Bands != null && alg.Bands.Length > 0)
            {
                MonitoringThemeFactory.SetBandArgs(subprd, rstIdentify.Satellite, rstIdentify.Sensor);
                foreach (BandDef band in alg.Bands)
                {
                    if (subprd.ArgumentProvider.GetArg(band.Identify).ToString() == "-1")
                    {
                        PrintInfo("从波段映射表获取\"" + band.Identify + "\"的波段序号失败,生成过程终止！");
                        return null;
                    }
                }
            }
            return subprd;
        }

        private void PrintInfo(string message)
        {
            if (_context != null)
                _context.PrintMessage(message);
        }

    }

    public class ContentText : IContextMessage
    {
        public ContentText()
        { }

        public void PrintMessage(string message)
        {
            //...记录处理过程.
            Console.WriteLine(message);
        }
    }

    //系统启动进度
    public class StartProgress : IStartProgress
    {
        public void PrintStartInfo(string sInfo)
        {
            Console.WriteLine(sInfo);
        }

        public void Stop()
        {
        }
    }

}
