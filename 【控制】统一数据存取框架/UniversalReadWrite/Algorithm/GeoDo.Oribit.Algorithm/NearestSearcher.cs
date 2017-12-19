using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GeoDo.Oribit.Algorithm
{
    public class NearestSearcher : IDisposable
    {
        private int width;
        private int height;
        private IntPtr ptr;
        //[DllImport("annSearch.dll")]
        [DllImport("annSearch.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void annSearchInit(ref float lon, ref float lat, int len, ref IntPtr ptr);
        [DllImport("annSearch.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void annSearchQuery(IntPtr ptr, float lon, float lat, ref int index);
        [DllImport("annSearch.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void annSearchClose(IntPtr ptr);

        //[DllImport("fannSearch.dll", CallingConvention = CallingConvention.Cdecl)]
        //private static extern void annSearchInit(ref float lon, ref float lat, int len, ref IntPtr ptr);
        //[DllImport("fannSearch.dll", CallingConvention = CallingConvention.Cdecl)]
        //private static extern void annSearchQuery(IntPtr ptr, float lon, float lat, ref int index);
        //[DllImport("fannSearch.dll", CallingConvention = CallingConvention.Cdecl)]
        //private static extern void annSearchClose(IntPtr ptr);

        public NearestSearcher(float[] lons, float[] lats, int w, int h)
        {
            width = w;
            height = h;
            annSearchInit(ref lons[0], ref lats[0], w * h, ref ptr);
        }

        public void Cal(float lon, float lat, ref int x, ref int y)
        {
            try
            {
                int index = 0;
                annSearchQuery(ptr, lon, lat, ref  index);
                y = index / width;
                x = index - y * width;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Dispose()
        {
            annSearchClose(ptr);
        }
    }
}
