using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public interface IExtractPanelWindow
    {
        /// <summary>
        /// 禁用参数改变时启动判识动作
        /// </summary>
        bool DisableIntimeExtracting { get; set; }
        void Apply(IWorkspace wks, IMonitoringSubProduct subProduct);
        void DoOk();
        bool IsShowSaveButton { get; set; }
        bool IsShowIntimeCheckBox { get; set; }
        void CanResetUserControl();
    }
}
