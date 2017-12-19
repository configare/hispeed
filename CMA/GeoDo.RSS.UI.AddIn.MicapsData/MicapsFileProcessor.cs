#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/5 13:45:20
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
using System.Windows.Forms;
using GeoDo.RSS.UI.AddIn.CanvasViewer;
using System.Xml.Linq;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.UI.AddIn.MicapsData
{
    /// <summary>
    /// 类名：MicapsFileProcessor
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/5 13:45:20
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    [Export(typeof(IOpenFileProcessor)), ExportMetadata("VERSION", "1")]
    internal class MicapsFileProcessor : OpenFileProcessor
    {
        public static string DATATYPE_CONFIG_DIR = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\MicapsDataConfig\MicapsDataDefine.xml";

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
            memoryIsNotEnough = false;
            return CreateVectorViewer(fname);
        }

        private bool CreateVectorViewer(string fname)
        {
            using (frmMicapsDataTypeSelect frm = new frmMicapsDataTypeSelect())
            {
                frm.StartPosition = FormStartPosition.CenterScreen;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    bool isSpatial = IsCanAddToView(fname);
                    string dataTypeId = GetDataTypeIdByName(frm.SelectDataType);
                    CanvasViewer.CanvasViewer cv = null;
                    CodeCell.AgileMap.Core.IFeatureLayer layer = null;
                    if (_session.SmartWindowManager.ActiveViewer is ICanvasViewer && isSpatial)
                        cv = GetCanvasViewer(fname);
                    if (cv == null && (_session.SmartWindowManager.ActiveViewer is ILayoutViewer))
                    {
                        ICanvas canvas = GetCanvas(fname);
                        layer = MicapsVectorLayerBuilder.CreateAndLoadVectorLayerForMicaps(_session, canvas, fname, dataTypeId);
                    }
                    else if (layer == null && cv == null)
                        cv = new CanvasViewer.CanvasViewer(OpenFileFactory.GetTextByFileName(fname), _session);
                    if (layer == null)
                    {
                        _session.SmartWindowManager.DisplayWindow(cv);
                        layer = MicapsVectorLayerBuilder.CreateAndLoadVectorLayerForMicaps(_session, cv.Canvas, fname, dataTypeId);
                    }
                    if (layer != null)
                        return true;
                    else
                        return false;
                }
                return true;
            }
        }

        private string GetDataTypeIdByName(string dataTypeName)
        {
            if (!File.Exists(DATATYPE_CONFIG_DIR))
                return null;
            XElement root = XElement.Load(DATATYPE_CONFIG_DIR);
            IEnumerable<XElement> items = root.Elements("DataDefine");
            if (items == null || items.Count() == 0)
                return null;
            string typeName;
            foreach (XElement item in items)
            {
                typeName = item.Attribute("name").Value;
                if (typeName == dataTypeName)
                    return item.Attribute("identify").Value;
            }
            return null;
        }

        /// <summary>
        /// 防止在轨道文件上添加mcd
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private bool IsCanAddToView(string filename)
        {
            string extName = Path.GetExtension(filename).ToUpper();
            if (extName == ".000")
                return true;
            ICanvasViewer v = _session.SmartWindowManager.ActiveCanvasViewer;
            if (v == null)
                return false;
            ICanvas c = v.Canvas;
            if (c == null)
                return false;
            IRasterDrawing drawing = c.PrimaryDrawObject as IRasterDrawing;
            if (drawing == null)
                return false;
            if (string.IsNullOrWhiteSpace(drawing.SpatialRef))
                return false;
            else
                return true;
        }

        public CanvasViewer.CanvasViewer GetCanvasViewer(string fname, params object[] options)
        {
            ICanvasViewer v = _session.SmartWindowManager.ActiveCanvasViewer;
            if (v == null)
                return null;
            ICanvas c = v.Canvas;
            if (c == null)
                return null;
            return v as CanvasViewer.CanvasViewer;
        }

        public ICanvas GetCanvas(string fname, params object[] options)
        {
            ILayoutViewer v = _session.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (v == null)
                return null;
            IDataFrame df = v.LayoutHost.ActiveDataFrame;
            if (df == null)
                return null;
            IDataFrameDataProvider provider = df.Provider as IDataFrameDataProvider;
            if (provider == null)
                return null;
            ICanvas c = provider.Canvas;
            return c;
        }

        private void TryCreateOrbitProjection(ICanvas c)
        {
            try
            {
                IRasterDrawing drawing = c.PrimaryDrawObject as IRasterDrawing;
                if (drawing == null)
                    return;
                drawing.TryCreateOrbitPrjection();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
