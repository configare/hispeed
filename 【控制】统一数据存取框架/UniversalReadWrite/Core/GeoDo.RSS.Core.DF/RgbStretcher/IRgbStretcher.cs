using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public unsafe interface IRgbStretcher<T> : IDisposable
    {
        Func<T, byte> Stretcher { get; }
        void Stretch(T data, byte* rgb);
        bool IsUseMap { get; set; }
        int[] DefaultBands { get; set; }
        void ResetMap();
        byte[] Map { get; }
    }
}
