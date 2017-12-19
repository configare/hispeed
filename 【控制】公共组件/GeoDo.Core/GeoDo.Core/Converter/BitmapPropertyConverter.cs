using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GeoDo.Core
{
    public class BitmapPropertyConverter:PropertyConverter
    {
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void MemoryCopy(IntPtr pdest, IntPtr psrc, int length);

        protected override void SetAttributes(System.Xml.Linq.XElement ele, object propertyValue)
        {
            if (propertyValue == null)
                return;
            Bitmap bitmap = propertyValue as Bitmap;
            if (bitmap == null)
                return;
            BitmapData pdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            try
            {
                byte[] buffer = new byte[bitmap.Height * pdata.Stride];
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                try
                {
                    MemoryCopy(handle.AddrOfPinnedObject(), pdata.Scan0, buffer.Length);
                    ele.SetAttributeValue("pixelformat", bitmap.PixelFormat);
                    ele.SetAttributeValue("width", bitmap.Width);
                    ele.SetAttributeValue("height", bitmap.Height);
                    ele.SetAttributeValue("stride", pdata.Stride);
                    ele.SetValue(Convert.ToBase64String(buffer));
                }
                finally 
                {
                    handle.Free();
                }
            }
            finally 
            {
                bitmap.UnlockBits(pdata);
            }
        }

        protected override object CreateAndFillObject(System.Xml.Linq.XElement propertyXml)
        {
            if (propertyXml == null)
                return null;
            string data = propertyXml.Value;
            if (string.IsNullOrEmpty(data))
                return null;
            byte[] buffer = Convert.FromBase64String(data);
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try 
            {
                PixelFormat pf = GetPixelFormat(propertyXml);
                int width = AttToInt(propertyXml, "width");
                int height = AttToInt(propertyXml, "height");
                int stride = AttToInt(propertyXml,"stride");
                Bitmap bitmap = new Bitmap(width, height, pf);
                BitmapData pdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, pf);
                try
                {
                    MemoryCopy(pdata.Scan0, handle.AddrOfPinnedObject(), height * stride);
                    return bitmap;
                }
                finally 
                {
                    bitmap.UnlockBits(pdata);
                }
            }
            finally 
            {
                handle.Free();
            }
        }

        private PixelFormat GetPixelFormat(System.Xml.Linq.XElement propertyXml)
        {
            string pixelformat = AttToString(propertyXml, "pixelformat");
            foreach (PixelFormat pf in Enum.GetValues(typeof(PixelFormat)))
            {
                if (pf.ToString() == pixelformat)
                    return pf;
            }
            return PixelFormat.Format24bppRgb;
        }
    }
}
