#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-1-16 9:54:38
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using System.IO;
using System.Text.RegularExpressions;
using GeoDo.HDF5;
using GeoDo.RSS.UI.AddIn.CanvasViewer;
using CanvasView = GeoDo.RSS.UI.AddIn.CanvasViewer.CanvasViewer;
using GeoDo.RSS.DF.GDAL.HDF5GEO;

namespace GeoDo.RSS.DF.AddIn.HDF5GEO
{
    /// <summary>
    /// 类名：FYSnowPrdFileProcessor
    /// 属性描述：
    /// 创建者：LiXJ   创建日期：2014-1-16 9:54:38
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    [Export(typeof(IOpenFileProcessor)), ExportMetadata("VERSION", "1")]
   public class FYSnowPrdFileProcessor: OpenFileProcessor,IRasterFileProcessor
    {
        string[] _alldatasets = new string[16]{ "SD_Flags_NorthernDaily_A","SD_Flags_NorthernDaily_D","SD_Flags_SouthernDaily_A","SD_Flags_SouthernDaily_D", 
                          "SD_NorthernDaily_A","SD_NorthernDaily_D","SD_SouthernDaily_A","SD_SouthernDaily_D",
                           "SWE_Flags_NorthernDaily_A", "SWE_Flags_NorthernDaily_D", "SWE_Flags_SouthernDaily_A","SD_Flags_SouthernDaily_D", 
                          "SWE_NorthernDaily_A","SWE_NorthernDaily_D","SWE_SouthernDaily_A","SWE_SouthernDaily_D"};
        public FYSnowPrdFileProcessor()
            : base()
        {
            _extNames.Add(".HDF");
        }

        public override bool Open(string fname, out bool memoryIsNotEnough)
        {
            memoryIsNotEnough = true;
            CreateCanvasViewer(fname);
            return true;
        }
        private void CreateCanvasViewer(string fname)
        {
            using (frmFYSnowPrdDataSelecte frm = new frmFYSnowPrdDataSelecte())
            {
                string[] slec = new string[] { "EASE-Grid North", "EASE-Grid South" };
                frm.Apply(slec);
                frm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string[] cid = new string[] { frm.ComponentID };
                    if (cid == null || cid[0] == null)
                        return;
                    CanvasView cv = new CanvasView(OpenFileFactory.GetTextByFileName(fname), _session);
                    _session.SmartWindowManager.DisplayWindow(cv);
                    object[] args = new object[] { cid };
                    RasterLayerBuilder.CreateAndLoadRasterLayer(_session, cv.Canvas, fname, args);
                }
            }
        }

        public override bool IsSupport(string fname, string extName)
        {
            return FileHeaderIsOK(fname, extName);
        }

        protected override bool FileHeaderIsOK(string fname, string extName)
        {
            return FYSnowPrdDriver.IsCompatible(fname,null);
        }
    }
}
