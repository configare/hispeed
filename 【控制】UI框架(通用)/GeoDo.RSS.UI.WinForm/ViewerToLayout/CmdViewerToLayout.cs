#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：admin     时间：2013-09-09 14:09:22
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
using GeoDo.RSS.Core.RasterDrawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using GeoDo.RSS.UI.AddIn.Layout;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using GeoDo.RasterProject;

namespace GeoDo.RSS.UI.WinForm
{
    /// <summary>
    /// 类名：CmdViewerToLayout
    /// 属性描述：当前视窗图像输出为专题图
    /// 创建者：罗战克   创建日期：2013-09-09 14:09:22
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdViewerToLayout : Command
    {
        public CmdViewerToLayout()
            : base()
        {
            _id = 2005;
            _name = "ViewerToLayout";
            _text = _toolTip = "通用专题图";
        }

        public override void Execute()
        {
            Execute("");
        }

        public override void Execute(string argument)
        {
            ICanvasViewer canViewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (canViewer == null)
                return;
            IRasterDrawing rd = canViewer.ActiveObject as IRasterDrawing;
            if (rd == null)
                return;
            string filename = rd.FileName;
            try
            {
                string theme = SelectLayoutTheme();
                if (string.IsNullOrWhiteSpace(theme))
                    return;
                string bmpFilename = CreateViewBitmap(rd.DataProvider.CoordEnvelope, rd.DataProvider.SpatialRef, filename);
                string gxdFilename = SaveGxdFilename(rd);
                if (string.IsNullOrWhiteSpace(gxdFilename))
                    return;
                ThematicGraphHelper thematic = new ThematicGraphHelper(_smartSession, Path.GetFileNameWithoutExtension(theme));
                //thematic.AOI = MasicAOI(aoi, ref extInfos);
                //thematic.SetOrbitTimes(obtTimes);
                //thematic.DateString = dateString;
                string resultFilename = thematic.CreateThematicGraphic(bmpFilename, theme, gxdFilename, null, null);
                //return new FileExtractResult(outFileIdentify, resultFilename);
                if (!string.IsNullOrWhiteSpace(resultFilename))
                    OpenFileFactory.Open(resultFilename);
            }
            catch (Exception ex)
            {
                MsgBox.ShowInfo(ex.Message);
            }
        }

        private string SaveGxdFilename(IRasterDrawing rd)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Smart Theme Document(*.gxd)|*.gxd";
                dlg.RestoreDirectory = true;
                dlg.InitialDirectory = Path.GetDirectoryName(rd.FileName);
                dlg.FileName = Path.GetFileNameWithoutExtension(rd.FileName);
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return dlg.FileName;
                }
            }
            return null;
        }

        /// <summary>
        /// 选择专题图模版
        /// </summary>
        /// <returns></returns>
        private string SelectLayoutTheme()
        {
            ISmartToolWindow wnd = _smartSession.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(5002);
            if (wnd == null || string.IsNullOrWhiteSpace((wnd as ThemeTemplateSelectWindow).SelectedTheme))
            {
                _smartSession.CommandEnvironment.Get(5002).Execute();//激活专题图模版选择窗口
                return null;
            }
            return (wnd as ThemeTemplateSelectWindow).SelectedTheme;
        }

        /// <summary>
        /// 生成视窗图片
        /// </summary>
        /// <param name="rd"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string CreateViewBitmap(CoordEnvelope env, ISpatialReference spatial, string filename)
        {
            string bmpFilename = GetTempBmpFilename(filename);
            string ext = Path.GetExtension(bmpFilename);
            ImageFormat imageFormat = GetImageFormat(ext);
            Size imgSize;
            using (Bitmap img = CreateRasterBitmap())
            {
                img.Save(bmpFilename, imageFormat);
                imgSize = img.Size;
            }
            WriteWorldFile(env, imgSize, spatial, bmpFilename);
            return bmpFilename;
        }

        private string GetTempBmpFilename(string filename)
        {
            string sugFilename = Path.ChangeExtension(Path.GetFileName(filename), ".png");
            return _smartSession.TemporalFileManager.NextTemporalFilename(sugFilename, ".png", null);
        }

        #region 创建当前视窗图像

        private Bitmap CreateRasterBitmap()
        {
            ICanvasViewer canViewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (canViewer == null)
                return null;
            Bitmap img = canViewer.Canvas.FullRasterRangeToBitmap();
            if (IsPixelFormatIndexed(img.PixelFormat))
            {
                Bitmap newimg = img.Clone(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format24bppRgb);
                img.Dispose();
                return newimg;
            }
            else
            {
                return img;
            }
        }

        private static PixelFormat[] indexedPixelFormats = { PixelFormat.Undefined, PixelFormat.DontCare, PixelFormat.Format16bppArgb1555, PixelFormat.Format1bppIndexed, PixelFormat.Format4bppIndexed, PixelFormat.Format8bppIndexed };

        /// <summary>
        ///  判断图片的PixelFormat 是否在 引发异常的 PixelFormat 之中
        ///  无法从带有索引像素格式的图像创建graphics对象
        /// </summary>
        /// <param name="imgPixelFormat"></param>
        /// <returns></returns>
        private static bool IsPixelFormatIndexed(PixelFormat imgPixelFormat)
        {
            foreach (PixelFormat pf in indexedPixelFormats)
            {
                if (pf.Equals(imgPixelFormat)) return true;
            }
            return false;
        }

        private ImageFormat GetImageFormat(string ext)
        {
            switch (ext.ToLower())
            {
                case ".png":
                    return ImageFormat.Png;
                case ".bmp":
                    return ImageFormat.Bmp;
                case ".jpeg":
                case ".jpg":
                    return ImageFormat.Jpeg;
                case ".tif":
                case ".tiff":
                    return ImageFormat.Tiff;
                default:
                    return ImageFormat.Png;
                    break;
            }
        }

        private void WriteWorldFile(CoordEnvelope env, Size imgSize, ISpatialReference spatial, string bmpFilename)
        {
            try
            {
                double minx = env.MinX;
                double maxy = env.MaxY;
                double resolutionX = env.Width / imgSize.Width;
                double resolutionY = env.Height / imgSize.Height;
                WorldFile worldFile = new WorldFile();
                worldFile.CreatWorldFile(resolutionX, -resolutionY, minx, maxy, bmpFilename);
                worldFile.CreatXmlFile(spatial == null ? GeoDo.Project.SpatialReference.GetDefault() : spatial, bmpFilename);
            }
            catch
            {
            }
        }
        #endregion
    }
}
