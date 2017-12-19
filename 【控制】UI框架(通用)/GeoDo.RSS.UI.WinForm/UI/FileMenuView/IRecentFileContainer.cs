using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.WinControls.UI;

namespace GeoDo.RSS.UI.WinForm
{
    internal interface IRecentFileContainer
    {
        void LoadItemsByRecentUsedFiles();
        void SetFileMenView(RadRibbonBarBackstageView view);
    }
}
