using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing;
using GeoDo.RSS.Layout;
using System.IO;
using System.Runtime.InteropServices;


namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    class CmdLayoutExportToBMP : Command
    {
        public CmdLayoutExportToBMP()
            : base()
        {
            _id = 6005;
            _name = "LayoutExport";
            _text = _toolTip = "导出为图片";
        }

        public override void Execute()
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            if (viewer.LayoutHost == null)
                return;
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter =
                    "BMP (*.bmp)|*.bmp|" +
                    "PNG (*.png)|*.png|" +
                    "JPEG (*.jpg,*.jpeg)|*.jpg;*.jpeg|" +
                    "TIF (*.tiff,*.tif)|*.tif;*.tiff";
                string defaultFname = "";
                if ((viewer as LayoutViewer).Tag != null)
                    defaultFname = (viewer as LayoutViewer).Tag as string;
                string defaultFilename = string.Empty;
                if (!string.IsNullOrWhiteSpace(defaultFname))
                {
                    dlg.InitialDirectory = Path.GetDirectoryName(defaultFname);
                    dlg.FileName = Path.GetFileNameWithoutExtension(defaultFname);
                    defaultFilename = dlg.InitialDirectory + "\\" + dlg.FileName;
                }
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ImageFormat imgFormat = ImageFormat.Png;
                    string extName = Path.GetExtension(dlg.FileName.ToUpper());
                    switch (extName)
                    {
                        case ".BMP":
                            imgFormat = ImageFormat.Bmp;
                            break;
                        case ".JPEG":
                        case ".JPG":
                            imgFormat = ImageFormat.Jpeg;
                            break;
                        case ".TIF":
                        case ".TIFF":
                            imgFormat = ImageFormat.Tiff;
                            break;
                        case ".PNG":
                            imgFormat = ImageFormat.Png;
                            break;
                    }
                    //临时修改 修改网络合成图 为 24位bmp；修改网络灰度图 为 8位灰度图 by chennan 20130930
                    if (defaultFname.IndexOf("NSNI") != -1)
                        using (Bitmap bmp = viewer.LayoutHost.ExportToBitmap(PixelFormat.Format24bppRgb))
                        {
                            int length = bmp.Width * bmp.Height;
                            byte[] values = new byte[length];
                            try
                            {
                                for (int i = 0; i < bmp.Height; i++)
                                {
                                    for (int j = 0; j < bmp.Width; j++)
                                    {
                                        //将境外区域设置为黑色
                                        if (bmp.GetPixel(j, i).ToArgb() == -1)
                                            values[i * bmp.Width + j] = 0;
                                        else
                                            values[i * bmp.Width + j] = bmp.GetPixel(j, i).R;
                                    }
                                }
                                using (Bitmap tempbmp = BuiltGrayBitmap(values, bmp.Width, bmp.Height))
                                {
                                    tempbmp.Save(dlg.FileName, imgFormat);
                                    SaveDefaultPic(tempbmp, defaultFilename + extName, imgFormat, Path.GetDirectoryName(dlg.FileName));
                                }
                            }
                            finally
                            {
                                values = null;
                            }
                        }
                    else if (defaultFname.IndexOf("NMCS") != -1)
                        using (Bitmap bmp = viewer.LayoutHost.ExportToBitmap(PixelFormat.Format24bppRgb))
                        {
                            bmp.Save(dlg.FileName, imgFormat);
                            SaveDefaultPic(bmp, defaultFilename + extName, imgFormat, Path.GetDirectoryName(dlg.FileName));
                        }
                    else
                        //
                        using (Bitmap bmp = viewer.LayoutHost.ExportToBitmap(PixelFormat.Format32bppArgb))
                        {
                            bmp.Save(dlg.FileName, imgFormat);
                            SaveDefaultPic(bmp, defaultFilename + extName, imgFormat, Path.GetDirectoryName(dlg.FileName));
                        }
                }
            }
        }

        private void SaveDefaultPic(Bitmap bmp, string defaultFilename, ImageFormat imgFormat, string currDir)
        {
            if (!string.IsNullOrEmpty(defaultFilename)
              && Path.GetDirectoryName(defaultFilename).ToUpper() != currDir.ToUpper())
                bmp.Save(defaultFilename, imgFormat);
        }

        /// <summary>   
        /// 用灰度数组新建一个8位灰度图像。   
        /// </summary>   
        /// <param name="rawValues"> 灰度数组(length = width * height)。 </param>   
        /// <param name="width"> 图像宽度。 </param>   
        /// <param name="height"> 图像高度。 </param>   
        /// <returns> 新建的8位灰度位图。 </returns>   
        private static Bitmap BuiltGrayBitmap(byte[] rawValues, int width, int height)
        {
            // 新建一个8位灰度位图，并锁定内存区域操作   
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height),
                 ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            // 计算图像参数   
            int offset = bmpData.Stride - bmpData.Width;        // 计算每行未用空间字节数   
            IntPtr ptr = bmpData.Scan0;                         // 获取首地址   
            int scanBytes = bmpData.Stride * bmpData.Height;    // 图像字节数 = 扫描字节数 * 高度   
            byte[] grayValues = new byte[scanBytes];            // 为图像数据分配内存   

            // 为图像数据赋值   
            int posSrc = 0, posScan = 0;                        // rawValues和grayValues的索引   
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    grayValues[posScan++] = rawValues[posSrc++];
                }
                // 跳过图像数据每行未用空间的字节，length = stride - width * bytePerPixel   
                posScan += offset;
            }

            // 内存解锁   
            Marshal.Copy(grayValues, 0, ptr, scanBytes);
            bitmap.UnlockBits(bmpData);  // 解锁内存区域   

            // 修改生成位图的索引表，从伪彩修改为灰度   
            ColorPalette palette;
            // 获取一个Format8bppIndexed格式图像的Palette对象   
            using (Bitmap bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
            {
                palette = bmp.Palette;
            }
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            // 修改生成位图的索引表   
            bitmap.Palette = palette;

            return bitmap;
        }

    }
}
