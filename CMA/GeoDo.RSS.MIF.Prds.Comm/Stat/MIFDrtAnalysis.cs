using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    public class MIFDrtAnalysis
    {
        public static IExtractResult DrtAreaStat(ISmartSession session, string outFileIdentify, string productTitle, bool isCustom)
        {
            return StatAnalysisDrt(session, outFileIdentify, productTitle, isCustom);
        }

        private static IExtractResult StatAnalysisDrt(ISmartSession session, string outFileIdentify, string productTitle, bool isCustom)
        {
            (session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("STAT");
            IMonitoringSubProduct msp = (session.MonitoringSession as IMonitoringSession).ActiveMonitoringSubProduct;
            if (msp == null)
                return null;
            object obj = msp.ArgumentProvider.GetArg("SelectedPrimaryFiles");
            if (obj == null || !File.Exists(obj.ToString()))
            {
                if (!SetSelectedPrimaryFiles(session, ref obj, msp.ArgumentProvider))
                    return null;
            }
            string fname = (obj as string[])[0];
            Dictionary<string, int[]> aoi = null;
            if (isCustom)
            {
                using (IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider)
                {
                    using (frmStatSubRegionTemplates frm = new frmStatSubRegionTemplates(new Size(prd.Width, prd.Height), prd.CoordEnvelope, prd.ResolutionX))
                    {
                        frm.listView1.MultiSelect = true;
                        if (frm.ShowDialog() == DialogResult.OK)
                            aoi=frm.GetFeatureAOIIndex();
                    }
                    if (aoi != null && aoi.Count > 0)
                        msp.ArgumentProvider.SetArg("AOI", aoi);
                }
            }
            if (!string.IsNullOrEmpty(outFileIdentify))
                msp.ArgumentProvider.SetArg("OutFileIdentify", outFileIdentify);
            if (!string.IsNullOrEmpty(productTitle))
                msp.ArgumentProvider.SetArg("ProductTitle", productTitle);
            GetCommandAndExecute(session, 6602);
            return null;
        }

        /// <summary>
        /// 临时测试用获取文件填充 SelectedPrimaryFiles 属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="iArgumentProvider"></param>
        /// <returns></returns>
        public static bool SetSelectedPrimaryFiles(ISmartSession session, ref object obj, IArgumentProvider iArgumentProvider)
        {
            IWorkspace wks = (session.MonitoringSession as IMonitoringSession).Workspace;
            if (wks == null)
                return false;
            ICatalog catalog = wks.ActiveCatalog;
            if (catalog == null)
                return false;
            string[] fnames = catalog.GetSelectedFiles();
            if (fnames == null || fnames.Length == 0)
                return false;
            obj = fnames;
            iArgumentProvider.SetArg("SelectedPrimaryFiles", fnames);
            return true;
        }

        private static void GetCommandAndExecute(ISmartSession session, int cmdID)
        {
            ICommand cmd = session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute("DRT");
        }
    }
}

