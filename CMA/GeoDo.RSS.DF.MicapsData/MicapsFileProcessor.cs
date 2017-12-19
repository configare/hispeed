#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/4 9:09:20
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
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GeoDo.RSS.UI.AddIn.CanvasViewer;

namespace GeoDo.RSS.DF.MicapsData
{
    /// <summary>
    /// 类名：MicapsFileProcessor
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/4 9:09:20
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    [Export(typeof(IOpenFileProcessor)), ExportMetadata("VERSION", "1")]
    internal class MicapsFileProcessor : OpenFileProcessor
    {
        public MicapsFileProcessor()
            : base()
        {
            _extNames.Add(".000");
        }

        protected override bool FileHeaderIsOK(string fname, string extName)
        {
            string f = Path.GetFileName(fname).ToLower();
            if (!Regex.Match(f, @"\d*\.000").Success)
                return false;
            return true;
        }

        public override bool Open(string fname, out bool memoryIsNotEnough)
        {
            memoryIsNotEnough = true;
            CreateVectorViewer(fname);
            return true;
        }

        private void CreateVectorViewer(string fname)
        {
            using(frmMicapsDataTypeSelect frm=new frmMicapsDataTypeSelect())
            {
                if (frm.DialogResult == DialogResult.OK)
                {
                    string dataTypeId="GroundObserveData";
                    CanvasViewer cv = new CanvasViewer(OpenFileFactory.GetTextByFileName(fname), _session);
                    _session.SmartWindowManager.DisplayWindow(cv);
                    MicapsVectorLayerBuilder.CreateAndLoadVectorLayerForMicaps(_session, cv.Canvas, fname, dataTypeId);
                }
            }
        }
    }
}
