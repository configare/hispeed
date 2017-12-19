using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface IFileExtractResult : IExtractResult, IExtractResultBase
    {
        string FileName { get; }
        bool Add2Workspace { get; set; }
        void SetDispaly(bool display);
        void SetOutIdentify(string outIdentify);
    }
}
