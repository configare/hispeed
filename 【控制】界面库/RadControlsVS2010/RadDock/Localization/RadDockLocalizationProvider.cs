using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Localization;

namespace Telerik.WinControls.UI.Localization
{
    /// <summary>
    /// Provides localization services for RadDock
    /// </summary>
    public class RadDockLocalizationProvider : LocalizationProvider<RadDockLocalizationProvider>
    {
        /// <summary>
        /// Gets the string corresponding to the given ID.
        /// </summary>
        /// <param name="id">String ID</param>
        /// <returns>The string corresponding to the given ID.</returns>
        public override string GetLocalizedString(string id) 
        {
            switch (id)
            {
                case RadDockStringId.ContextMenuFloating:
                    return "浮动";
                case RadDockStringId.ContextMenuDockable:
                    return "停靠";
                case RadDockStringId.ContextMenuTabbedDocument:
                    return "标签页";
                case RadDockStringId.ContextMenuAutoHide:
                    return "自动隐藏";
                case RadDockStringId.ContextMenuHide:
                    return "隐藏";
                case RadDockStringId.ContextMenuClose:
                    return "关闭";
                case RadDockStringId.ContextMenuCloseAll:
                    return "关闭所有";
                case RadDockStringId.ContextMenuCloseAllButThis:
                    return "关闭除除此之外的视窗";
                case RadDockStringId.ContextMenuMoveToPreviousTabGroup:
                    return "移动到前一个标签组";
                case RadDockStringId.ContextMenuMoveToNextTabGroup:
                    return "移动到下一个标签组";
                case RadDockStringId.ContextMenuNewHorizontalTabGroup:
                    return "纵向排列";
                case RadDockStringId.ContextMenuNewVerticalTabGroup:
                    return "横向排列";
                case RadDockStringId.ToolTabStripCloseButton:
                    return "关闭窗口";
                case RadDockStringId.ToolTabStripDockStateButton:
                    return "窗口状态";
                case RadDockStringId.ToolTabStripUnpinButton:
                    return "自动隐藏";
                case RadDockStringId.ToolTabStripPinButton:
                    return "停靠";
                case RadDockStringId.DocumentTabStripCloseButton:
                    return "关闭文档";
                case RadDockStringId.DocumentTabStripListButton:
                    return "活动文档";
            }

            return string.Empty;
        }
    }
}
