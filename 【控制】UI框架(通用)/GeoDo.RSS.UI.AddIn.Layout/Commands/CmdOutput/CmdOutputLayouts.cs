#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-3-27 17:21:43
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System.Drawing;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using System.Drawing.Imaging;
using GeoDo.RSS.UI.AddIn.Layout;
using System.IO;
using System.Windows.Forms;
using GeoDo.RSS.Layout;
using System.Collections.Generic;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 类名：cmdOutputLayouts
    /// 属性描述： 多专题图输出
    /// 创建者：Li Xj 修改日期：
    /// 修改描述：
    /// 修改时间 2014年11月6日11:44:34
    /// 修改人 ：CA
    /// 修改内容 扩展实现类方法，增加参数使导出方法为沙尘和大雾为png
    /// 备注：
    /// </summary>
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdOutputLayouts : Command
    {
        public CmdOutputLayouts()
            : base()
        {
            _id = 36604;
            _name = "CmdOutputLayouts";
            _text = _toolTip = "多专题图输出";


        }
        public override void Execute()
        {
            Execute("btnHisLayoutput");
        }
        public override void Execute(string argument)
        {
            ISmartWindow[] wnds = _smartSession.SmartWindowManager.GetSmartWindows((wnd) => { return wnd is ILayoutViewer; });

            if (wnds != null)
            {
                foreach (ISmartWindow wnd in wnds)
                {

                    ILayoutViewer viewer = wnd as ILayoutViewer;
                    if (viewer == null)
                        continue;
                    if (viewer.LayoutHost == null)
                        continue;
                    //刷新命令
                    IDataFrame frm = viewer.LayoutHost.ActiveDataFrame;
                    if (frm != null)
                        viewer.LayoutHost.Render(true);

                    string defaultFname = string.Empty;
                    if ((viewer as LayoutViewer).Tag != null)
                        defaultFname = (viewer as LayoutViewer).Tag as string;
                    defaultFname = Path.Combine(Path.GetDirectoryName(defaultFname), Path.GetFileNameWithoutExtension(defaultFname) + ".bmp");
                    ImageFormat imgFormat = ImageFormat.Bmp;// Jpeg;

                    using (Bitmap bmp = viewer.LayoutHost.ExportToBitmap(PixelFormat.Format32bppArgb))
                    {
                        bmp.Save(defaultFname, imgFormat);
                    }
                }
            }
            _smartSession.UIFrameworkHelper.SetVisible(argument, false);
            _smartSession.UIFrameworkHelper.SetLockBesideX(argument, false);
            MessageBox.Show("专题图导出完毕！");
        }
        /// <summary>
        /// 导出图片重载方法-支持是否PNG-BMP 以后如需要根据传递文件类型进行导出图片，修改此方法
        /// </summary>
        /// <param name="IsPng">是否是png文件</param>
        public override void Execute(string argument, params string[] args)
        {
            argument = "btnHisLayoutput";
            ISmartWindow[] wnds = _smartSession.SmartWindowManager.GetSmartWindows((wnd) => { return wnd is ILayoutViewer; });

            if (wnds != null)
            {
                foreach (ISmartWindow wnd in wnds)
                {

                    ILayoutViewer viewer = wnd as ILayoutViewer;
                    if (viewer == null)
                        continue;
                    if (viewer.LayoutHost == null)
                        continue;
                    //刷新命令
                    IDataFrame frm = viewer.LayoutHost.ActiveDataFrame;
                    if (frm != null)
                        viewer.LayoutHost.Render(true);

                    string defaultFname = string.Empty;
                    if ((viewer as LayoutViewer).Tag != null)
                        defaultFname = (viewer as LayoutViewer).Tag as string;

                    ImageFormat imgFormat = GetImageFormatByArgs(ref defaultFname, args);
                    using (Bitmap bmp = viewer.LayoutHost.ExportToBitmap(PixelFormat.Format32bppArgb))
                    {
                        bmp.Save(defaultFname, imgFormat);
                    }
                }
            }
            _smartSession.UIFrameworkHelper.SetVisible(argument, false);
            _smartSession.UIFrameworkHelper.SetLockBesideX(argument, false);
            bool isShowMessage = true;
            if (args.Length > 1)
                bool.TryParse(args[1], out isShowMessage);
            if (isShowMessage)
                MessageBox.Show("专题图导出完毕！");
        }

        private static ImageFormat GetImageFormatByArgs(ref string defaultFname, string[] args)
        {
            defaultFname = Path.Combine(Path.GetDirectoryName(defaultFname), Path.GetFileNameWithoutExtension(defaultFname) + ".PNG");
            if (args == null || args.Length == 0)
                return ImageFormat.Png;
            switch (args[0].ToUpper())
            {
                case "JPG":
                case "JPEG":
                    defaultFname = Path.Combine(Path.GetDirectoryName(defaultFname), Path.GetFileNameWithoutExtension(defaultFname) + ".JPG");
                    return ImageFormat.Jpeg;
                case "BMP":
                    defaultFname = Path.Combine(Path.GetDirectoryName(defaultFname), Path.GetFileNameWithoutExtension(defaultFname) + ".BMP");
                    return ImageFormat.Jpeg;
                case "TIF":
                case "TIFF":
                    defaultFname = Path.Combine(Path.GetDirectoryName(defaultFname), Path.GetFileNameWithoutExtension(defaultFname) + ".TIF");
                    return ImageFormat.Tiff;
                case "GIF":
                    defaultFname = Path.Combine(Path.GetDirectoryName(defaultFname), Path.GetFileNameWithoutExtension(defaultFname) + ".TIF");
                    return ImageFormat.Gif;
                default:
                    return ImageFormat.Png;
            }
        }

    }
}
