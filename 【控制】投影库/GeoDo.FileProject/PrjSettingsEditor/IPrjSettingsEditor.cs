using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.FileProject
{
    public interface IPrjSettingsEditor
    {
        void Apply(FilePrjSettings prjSettings);
        bool IsSupport(Type type);
    }
}
