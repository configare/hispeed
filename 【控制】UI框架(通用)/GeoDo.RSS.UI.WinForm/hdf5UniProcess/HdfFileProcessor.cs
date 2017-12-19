#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：admin     时间：2013-09-09 10:29:10
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
using System.IO;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.DF.GDAL.HDF5Universal;
using GeoDo.RSS.UI.AddIn.CanvasViewer;

namespace GeoDo.RSS.UI.WinForm
{
    /// <summary>
    /// 类名：HdfFileProcessor
    /// 属性描述：
    /// 创建者：admin   创建日期：2013-09-09 10:29:10
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>

    public class HdfFileProcessor : OpenFileProcessor, IRasterFileProcessor
    {
        public HdfFileProcessor()
            : base()
        {
            _extNames.Add(".HDF");
        }

        protected override bool FileHeaderIsOK(string fname, string extName)
        {
            string f = Path.GetFileName(fname).ToLower();
            if (Path.GetExtension(f) == ".hdf")
                return true;
            return false;
        }

        public override bool Open(string fname, out bool memoryIsNotEnough)
        {
            memoryIsNotEnough = true;
            CreateCanvasViewer(fname);
            return true;
        }

        private void CreateCanvasViewer(string filename)
        {
            using (Hdf5DatasetSelection frm = new Hdf5DatasetSelection())
            {
                frm.LoadFile(filename);
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    filename = frm.FileName;
                    string[] hdfOptions = frm.GetOpenOptions();
                    CanvasViewer cv = new CanvasViewer(OpenFileFactory.GetTextByFileName(filename), _session);
                    _session.SmartWindowManager.DisplayWindow(cv);
                    RasterLayerBuilder.CreateAndLoadRasterLayer(_session, cv.Canvas, filename, hdfOptions, new RgbStretcherProvider());
                    _session.SmartWindowManager.DisplayWindow(cv);//
                }
            }
        }
    }
}
