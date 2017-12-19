using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface IExtractResultArray : IExtractResult
    {
        IExtractResultBase[] PixelMappers { get; }
        bool Add(IExtractResultBase pixelMapper);
    }
}
