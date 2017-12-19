using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.DF.MVG
{
    public interface IMvgRasterBand : IRasterBand
    {
        MvgHeader Header { get; }
    }
}
