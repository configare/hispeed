using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public interface ITemporalFileManager
    {
        string NextTemporalFilename(string extName, string[] secondaryExtnames);
        string NextTemporalFilename(string suggestFilename, string extName, string[] secondaryExtnames);
    }
}
