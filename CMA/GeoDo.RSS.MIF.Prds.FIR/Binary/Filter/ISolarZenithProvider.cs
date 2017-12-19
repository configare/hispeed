using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public interface ISolarZenithProvider
    {
        void ReadSolarZenith(int pixelIndex,ref float solarZenith);
        float GetSolarZenith(int pixelIndex);
    }
}
