using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;

namespace GeoDo.RSS.Core.UI
{
    /// <summary>
    /// 视窗管理器,从RadDock中的DockableDocumentContainer中获取视窗
    /// </summary>
    public interface ISmartWindowManager
    {
        OnActiveWindowChangedHandler OnActiveWindowChanged { get; set; }
        ISmartWindow ActiveWindow { get; }
        ISmartViewer ActiveViewer { get; }
        ICanvasViewer ActiveCanvasViewer { get; }
        void DisplayWindow(ISmartWindow window);
        void DisplayWindow(ISmartWindow window, WindowPosition position,params object[] options);
        bool IsExist(ISmartWindow window);
        ISmartWindow GetSmartWindow(Func<ISmartWindow, bool> where);
        ISmartWindow[] GetSmartWindows(Func<ISmartWindow, bool> where);
        void ToHorizontalLayout(ISmartWindow[] windows);
        void ToVerticalLayout(ISmartWindow[] windows);
        void Hide(ISmartWindow window);
        void Show(ISmartWindow window);
        ILinkableViewerManager LinkableViewerManager { get; }
        void UpdatePrimaryLinkWindow();
        void NewHorizontalGroup(ISmartViewer viewer);
        void NewVerticalGroup(ISmartViewer viewer);
        ISmartToolWindowFactory SmartToolWindowFactory { get; }
        object MainForm { get; }
        Point ViewLeftUpperCorner { get; }
        ICursorInfoDisplayer CursorInfoDisplayer { get; }

        void AddDocument(ISmartWindow cv);

        void ActivateWindow(ISmartWindow cv);
    }
}
