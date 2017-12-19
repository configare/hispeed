using System;
using System.Collections.Generic;

namespace Telerik.WinControls.Layouts
{
    public delegate void LayoutCallback(ILayoutManager manager);

    public interface ILayoutManager
    {
        ILayoutQueue MeasureQueue { get; }
        ILayoutQueue ArrangeQueue { get; }
        List<EventHandler> LayoutEvents { get; }
        void RemoveElementFromQueues(RadElement e);

        void IncrementLayoutCalls();
        void DecrementLayoutCalls();
        void AddToSizeChangedChain(SizeChangedInfo info);
        void EnterArrange();
        void EnterMeasure();
        void ExitArrange();
        void ExitMeasure();

        bool IsUpdating { get; }
        void UpdateLayout();
        void InvokeUpdateLayoutAsync();
    }
}
