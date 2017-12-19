using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandViewerOutput : Command
    {
        public CommandViewerOutput()
            : base()
        {
            _id = 2001;
            _text = _name = "导出为图片(使用原始分辨率)";
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
                        //update by ca 2014-09-24
                        // 修改原有导出tif+tfw方式为geotif
                        if (ext == ".tif")
                        {
                            IRasterDataProvider prd = rd.DataProvider;
                            Bitmap img = rd.GetBitmapUseOriginResolution();
                            BitmapData dain = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
                            List<byte[]> listdata = new List<byte[]>();
                            unsafe
                            {
                                byte* pointin = (byte*)(dain.Scan0.ToPointer());
                                byte[] datar = new byte[img.Height * img.Width];
                                byte[] datag = new byte[img.Height * img.Width];
                                byte[] datab = new byte[img.Height * img.Width];
                                int width = img.Width;
                                int height = img.Height;
                                if (img.PixelFormat == PixelFormat.Format24bppRgb)//彩色图像
                                {
                                    for (int j = 0; j < height; j++)
                                    {
                                        for (int i = 0; i < width; i++)
                                        {
                                            //顺序为bgr
                                            datab[j * width + i] = pointin[0];
                                            datag[j * width + i] = pointin[1];
                                            datar[j * width + i] = pointin[2];
                                            pointin += 3;
                                        }
                                        pointin += dain.Stride - dain.Width * 3;
                                    }
                                    listdata.Add(datar);
                                    listdata.Add(datag);
                                    listdata.Add(datab);
                                }
                                else if (img.PixelFormat == PixelFormat.Format8bppIndexed)//黑白图像
                                {
                                    for (int j = 0; j < height; j++)
                                    {
                                        for (int i = 0; i < width; i++)
                                        {
                                            datab[j * width + i] = pointin[0];
                                            pointin += 1;
                                        }
                                        pointin += dain.Stride - dain.Width * 1;
                                    }
                                    listdata.Add(datab);
                                }
                            }
                            using (IRasterDataProvider prdWriter = GetOutFileProvider(prd, img.Size, file))
                            {
                                for (int i = 0; i < listdata.Count; i++)
                                {
                                    IRasterBand band = null;
                                    try
                                    {
                                        band = prdWriter.GetRasterBand(i + 1);
                                        GCHandle h = GCHandle.Alloc(listdata[i], GCHandleType.Pinned);
                                        try
                                        {
                                            IntPtr bufferPtr = h.AddrOfPinnedObject();
                                            band.Write(0, 0, img.Width, img.Height, bufferPtr, enumDataType.Byte, img.Width, img.Height);
                                        }
                                        finally
                                        {
                                            h.Free();
                                        }
                                    }
                                    finally
                                    {
                                        //这里不能释放，由于大部分

                                    }
                                }
                            }
                        }
                        else// 常规图片处理
                        {
                            using (Bitmap img = CreateRasterBitmap(rd))
                            {
                                img.Save(file, imageFormat);

                            }
                            WriteWorldFile(rd.DataProvider, file);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MsgBox.ShowInfo(ex.Message);
            }
        }
        /// <summary>
        /// 创建目标tif文件读取驱动
        /// </summary>
        /// <returns></returns>
        private IRasterDataProvider GetOutFileProvider(IRasterDataProvider srcRaster, Size outimg, string outfilename)
        {
            string[] options = null;
            string driver = string.Empty;
            string dstwktstr = string.Empty;
            dstwktstr = "GEOGCS['GCS_WGS_1984',DATUM['D_WGS_1984',SPHEROID['WGS_1984',6378137,298.257223563]],PRIMEM['Greenwich',0],UNIT['Degree',0.0174532925199433]]";
            dstwktstr = dstwktstr.Replace('\'', '"');
            double minx = srcRaster.CoordEnvelope.MinX;
            double maxy = srcRaster.CoordEnvelope.MaxY;
            double resolutionX = srcRaster.ResolutionX;
            double resolutionY = srcRaster.ResolutionY;
            driver = "GDAL";
            options = new string[]{
                    "DRIVERNAME=GTiff",
                    "TFW=NO",
                    "WKT=" + dstwktstr,
                    "GEOTRANSFORM=" + string.Format("{0},{1},{2},{3},{4},{5}",minx, resolutionX,0,maxy,0, -resolutionY)
                    };
            int bandCount = srcRaster.BandCount == 1 ? 1 : 3;
            string outdir = Path.GetDirectoryName(outfilename);
            if (!Directory.Exists(outdir))
                Directory.CreateDirectory(outdir);
            IRasterDataDriver outdrv = GeoDataDriver.GetDriverByName(driver) as IRasterDataDriver;
            return outdrv.Create(outfilename, outimg.Width, outimg.Height, bandCount, enumDataType.Byte, options) as IRasterDataProvider;
        }

        private Bitmap CreateRasterBitmap(IRasterDrawing rd)
        {
            Bitmap img = rd.GetBitmapUseOriginResolution();
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

        private void WriteWorldFile(IRasterDataProvider prd, string bmpFilename)
        {
            try
            {
                double minx = prd.CoordEnvelope.MinX;
                double maxy = prd.CoordEnvelope.MaxY;
                double resolutionX = prd.ResolutionX;
                double resolutionY = prd.ResolutionY;
                GeoDo.Project.ISpatialReference spatial = prd.SpatialRef;
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
