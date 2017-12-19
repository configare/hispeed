using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Telerik.WinControls
{
    public class MeasurementGraphics : IDisposable
    {
        #region Static fields & properties

        [ThreadStatic]
        private static int controlCount;

        [ThreadStatic]
        private static IntPtr memoryDC;

        private static object instance = new object();

        public static object SyncObject
        {
            get { return instance; }
        }

        #endregion

        Graphics graphics;

        private MeasurementGraphics()
        {
            if (memoryDC == IntPtr.Zero)
            {
                memoryDC = NativeMethods.CreateCompatibleDC(new HandleRef(null, IntPtr.Zero));
            }

            graphics = Graphics.FromHdcInternal(memoryDC);
        }

        public Graphics Graphics
        {
            get
            {
                return graphics;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        public static MeasurementGraphics CreateMeasurementGraphics()
        {
            return new MeasurementGraphics();
        }

        internal static void IncreaseControlCount()
        {
            controlCount++;
        }

        internal static void DecreaseControlCount()
        {
            if (controlCount > 0)
            {
                controlCount--;
                if (controlCount == 0 && memoryDC != IntPtr.Zero)
                {
                    NativeMethods.DeleteDC(new HandleRef(null, memoryDC));
                    memoryDC = IntPtr.Zero;
                }
            }
        }
    }

}
