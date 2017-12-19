using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.UI;
using GeoDo.Project;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    /// <summary>
    /// 当前视图分辨率输出图片
    /// </summary>
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandImageOutput : Command
    {
        public CommandImageOutput()
            : base()
        {
            _id = 2003;
            _text = _name = "导出影像为图片(当前视图分辨率)";
        }

        public override void Execute()
        {
            ICanvasViewer canViewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (canViewer == null)
                return;
            string filename = "";
            IRasterDrawing rd = canViewer.ActiveObject as IRasterDrawing;
            if (rd != null)
                filename = rd.FileName;
            try
            {
                using (SaveFileDialog diag = new SaveFileDialog())
                {
                    diag.Filter = "png(*.png)|*.png|Bitmap(*.bmp)|*.bmp|Jpeg(*.jpg)|*.jpg|Tif(*.tif)|*.tif";
                    diag.FilterIndex = 0;
                    if (string.IsNullOrWhiteSpace(diag.InitialDirectory))
                        diag.InitialDirectory = Path.GetDirectoryName(filename);
                    diag.FileName = Path.ChangeExtension(Path.GetFileName(filename), "png");
                    if (diag.ShowDialog() == DialogResult.OK)
                    {
                        string file = diag.FileName;
                        string ext = Path.GetExtension(diag.FileName);
                        ImageFormat imageFormat = GetImageFormat(ext);
                        Size imgSize = Size.Empty;
                        GeoDo.RSS.Core.DrawEngine.CoordEnvelope envelope;
                            using (Bitmap img = CreateRasterBitmap(rd))
                            {
                                img.Save(file, imageFormat);
                                imgSize = img.Size;
                                envelope = rd.Envelope;
                            }
                            double resolutionX = envelope.Width / imgSize.Width;
                            double resolutionY = envelope.Height / imgSize.Height;

                            WriteWorldFile(envelope, SpatialReference.GetDefault(), resolutionX, resolutionY, file);
                    }
                }
            }
            catch (Exception ex)
            {
                MsgBox.ShowInfo(ex.Message);
            }
        }

        private Bitmap CreateRasterBitmap(IRasterDrawing rd)
        {
            Bitmap img = rd.Bitmap.Clone() as Bitmap;
            {
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

        private void WriteWorldFile(GeoDo.RSS.Core.DrawEngine.CoordEnvelope envlope, ISpatialReference spatial, double resolutionX, double resolutionY, string bmpFilename)
        {
            try
            {
                double minx = envlope.MinX;
                double maxy = envlope.MaxY;
                //double resolutionX = prd.ResolutionX;
                //double resolutionY = prd.ResolutionY;
                //GeoDo.Project.ISpatialReference spatial = prd.SpatialRef;
                WorldFile worldFile = new WorldFile();
                worldFile.CreatWorldFile(resolutionX, -resolutionY, minx, maxy, bmpFilename);
                worldFile.CreatXmlFile(spatial == null ? GeoDo.Project.SpatialReference.GetDefault() : spatial, bmpFilename);
            }
            catch
            {
            }
        }
    }
}
