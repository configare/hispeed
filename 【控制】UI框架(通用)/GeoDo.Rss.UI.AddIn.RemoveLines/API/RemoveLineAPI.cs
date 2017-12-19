using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.UI.AddIn.RemoveLines
{
    public class RemoveLineAPI
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ProgressCallback(int value);

        [DllImport("RemoveLines", EntryPoint = "remove_lines")]
        public unsafe static extern bool remove_lines(float* data, int dataHeight, int dataWidth, ProgressCallback getProgess);
    }
}
