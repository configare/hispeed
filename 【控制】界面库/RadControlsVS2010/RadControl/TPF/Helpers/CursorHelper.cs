using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls
{
    public static class CursorHelper
    {
        #region Public Methods

        public static Cursor CursorFromBitmap(Bitmap bitmap, Point hotSpot)
        {
            IntPtr hBitmap = bitmap.GetHicon();
            NativeMethods.IconInfo iconInfo = new NativeMethods.IconInfo();

            //fill the icon info
            NativeMethods.GetIconInfo(hBitmap, ref iconInfo);
            iconInfo.xHotspot = hotSpot.X;
            iconInfo.yHotspot = hotSpot.Y;
            iconInfo.fIcon = false;

            return new Cursor(NativeMethods.CreateIconIndirect(ref iconInfo));
        }

        #endregion
    }
}
