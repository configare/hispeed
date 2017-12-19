using System;
using System.Collections.Generic;
using System.Drawing;


namespace Telerik.WinControls.Layouts
{
    internal sealed class ContextLayoutManager : ILayoutManager, IDisposable
    {
        private const int s_LayoutRecursionLimit = 250;

        private ILayoutHandler layoutHandler;

        private InternalArrangeQueue _arrangeQueue;
        private int _arrangesOnStack;
        private bool _firePostLayoutEvents;
        private RadElement _forceLayoutElement;
        private bool _gotException;
        private bool _inFireLayoutUpdated;
        private bool _inFireSizeChanged;
        private bool _isDead;
        private bool _isInUpdateLayout;
        private bool _isUpdating;
        private List<EventHandler> _layoutEvents;
        private bool _layoutRequestPosted;
        private InternalMeasureQueue _measureQueue;
        private int _measuresOnStack;
        private SizeChangedInfo _sizeChangedChain;
        private static LayoutCallback _updateCallback = new LayoutCallback(UpdateLayoutCallback);
        private static LayoutCallback _updateLayoutBackground = new LayoutCallback(UpdateLayoutBackground);

        private int layoutCalls = 0;


        static ContextLayoutManager()
        {
        }

        internal ContextLayoutManager(ILayoutHandler layoutHandler)
        {
            this.layoutHandler = layoutHandler;
        }

        public void Dispose()
        {
            if (this._layoutEvents != null)
                this._layoutEvents.Clear();
            // This method is called when the layout handler is being disposed.
            // Don't dispose the handler here!
            this.layoutHandler = null;
            this._layoutEvents = null;

            this._measureQueue = null;
            this._arrangeQueue = null;
            this._sizeChangedChain = null;
            this._forceLayoutElement = null;
            this._isDead = true;
        }

        internal void ClearSizeChangedChain()
        {
            this._sizeChangedChain = null;
        }

        private void InvokeAsyncCallback(LayoutCallback callback)
        {
            if (this.layoutHandler != null)
                this.layoutHandler.InvokeLayoutCallback(callback);
        }

        public void InvokeUpdateLayoutAsync()
        {
            InvokeAsyncCallback(ContextLayoutManager._updateCallback);
        }

        private void setForceLayout(RadElement e)
        {
            this._forceLayoutElement = e;
        }

        public void RemoveElementFromQueues(RadElement e)
        {
            this.ArrangeQueueInternal.Remove(e);
            this.MeasureQueueInternal.Remove(e);
            if (object.ReferenceEquals(e, this._forceLayoutElement))
            {
                this._forceLayoutElement = null;
            }
        }

        private void NeedsRecalc()
        {
            if (this._isDead)
            {
                return;
            }

            if (!this._layoutRequestPosted && !this._isUpdating)
            {
                this.InvokeAsyncCallback(ContextLayoutManager._updateCallback);
                this._layoutRequestPosted = true;
            }
        }

        private static void UpdateLayoutBackground(ILayoutManager manager)
        {
            ContextLayoutManager layoutManager = manager as ContextLayoutManager;
            if (layoutManager != null)
            {
                layoutManager.NeedsRecalc();
            }
        }

        private static void UpdateLayoutCallback(ILayoutManager manager)
        {
            ContextLayoutManager layoutManager = manager as ContextLayoutManager;
            if (layoutManager != null && !layoutManager._isDead)
            {
                layoutManager.UpdateLayout();
            }
        }

        public void AddToSizeChangedChain(SizeChangedInfo info)
        {
            info.Next = this._sizeChangedChain;
            this._sizeChangedChain = info;
        }

        public void EnterArrange()
        {
            this._arrangesOnStack++;
            if (this._arrangesOnStack > ContextLayoutManager.s_LayoutRecursionLimit)
            {
                string str = string.Format("Exceeded arrange recursion limit ({0})", ContextLayoutManager.s_LayoutRecursionLimit);
                throw new InvalidOperationException(str);
            }
            this._firePostLayoutEvents = true;
        }

        public void EnterMeasure()
        {
            this._measuresOnStack++;
            if (this._measuresOnStack > ContextLayoutManager.s_LayoutRecursionLimit)
            {
                string str = string.Format("Exceeded measure recursion limit ({0})", ContextLayoutManager.s_LayoutRecursionLimit);
                throw new InvalidOperationException(str);
            }
            this._firePostLayoutEvents = true;
        }

        public void ExitArrange()
        {
            this._arrangesOnStack--;
        }

        public void ExitMeasure()
        {
            this._measuresOnStack--;
        }

        private void fireLayoutUpdateEvent()
        {
            if (!this._inFireLayoutUpdated)
            {
                try
                {
                    this._inFireLayoutUpdated = true;
                    this._firePostLayoutEvents = false;
                    EventHandler[] handlersArray = new EventHandler[this.LayoutEvents.Count];
                    this.LayoutEvents.CopyTo(handlersArray);
                    for (int i = 0; i < handlersArray.Length; i++)
                    {
                        EventHandler handler1 = handlersArray[i];
                        if (handler1 != null)
                        {
                            handler1(null, EventArgs.Empty);
                            if (this.hasDirtiness)
                            {
                                return;
                            }
                        }
                    }
                }
                finally
                {
                    this._inFireLayoutUpdated = false;
                }
            }
        }

        private void fireSizeChangedEvents()
        {
            if (!this._inFireSizeChanged)
            {
                try
                {
                    this._inFireSizeChanged = true;
                    while (this._sizeChangedChain != null)
                    {
                        SizeChangedInfo info1 = this._sizeChangedChain;
                        this._sizeChangedChain = info1.Next;
                        info1.Element.sizeChangedInfo = null;
                        info1.Element.OnRenderSizeChanged(info1);
                        if (this.hasDirtiness)
                        {
                            return;
                        }
                    }
                }
                finally
                {
                    this._inFireSizeChanged = false;
                }
            }
        }

        private void invalidateTreeIfRecovering()
        {
            if ((this._forceLayoutElement != null) || this._gotException)
            {
                if (this._forceLayoutElement != null)
                {
                    this.markTreeDirty(this._forceLayoutElement);
                }
                this._forceLayoutElement = null;
                this._gotException = false;
            }
        }

        private void markTreeDirty(RadElement e)
        {
            while (true)
            {
                RadElement element1 = e.Parent;
                if (element1 == null)
                {
                    break;
                }
                e = element1;
            }
            this.markTreeDirtyHelper(e);
            this.MeasureQueueInternal.Add(e);
            this.ArrangeQueueInternal.Add(e);
        }

        private void markTreeDirtyHelper(RadElement v)
        {
            if (v != null)
            {
                v.InvalidateMeasureInternal();
                v.InvalidateArrangeInternal();
                int num2 = v.Children.Count;
                for (int i = 0; i < num2; i++)
                {
                    RadElement visual1 = v.Children[i];
                    if (visual1 != null)
                    {
                        this.markTreeDirtyHelper(visual1);
                    }
                }
            }
        }

        public void UpdateLayout()
        {
            if ((!this._isInUpdateLayout && (this._measuresOnStack <= 0)) && ((this._arrangesOnStack <= 0) && !this._isDead))
            {
                int num2 = 0;
                bool flag1 = true;
                RadElement element1 = null;
                try
                {
                    this.invalidateTreeIfRecovering();
                    while (this.hasDirtiness || this._firePostLayoutEvents)
                    {
                        if (++num2 > LayoutQueue.PocketCapacity)
                        {
                            this.InvokeAsyncCallback(ContextLayoutManager._updateLayoutBackground);
                            element1 = null;
                            flag1 = false;
                            return;
                        }
                        this._isUpdating = true;
                        this._isInUpdateLayout = true;

                        int num1 = 0;
                        DateTime time1 = new DateTime(0L);
                        do
                        {
                            if (++num1 > LayoutQueue.PocketCapacity)
                            {
                                num1 = 0;
                                if (time1.Ticks == 0)
                                {
                                    time1 = DateTime.Now;
                                }
                                else
                                {
                                    TimeSpan span2 = (TimeSpan)(DateTime.Now - time1);
                                    if (span2.Milliseconds > 0x132)
                                    {
                                        this.InvokeAsyncCallback(ContextLayoutManager._updateLayoutBackground);
                                        element1 = null;
                                        flag1 = false;
                                        return;
                                    }
                                }
                            }
                            element1 = this.MeasureQueueInternal.GetTopMost();
                            if (element1 != null)
                            {
                                element1.Measure(element1.PreviousConstraint);
                            }
                        }
                        while (element1 != null);

                        num1 = 0;
                        time1 = new DateTime((long)0);
                        while (this.MeasureQueueInternal.IsEmpty)
                        {
                            if (++num1 > LayoutQueue.PocketCapacity)
                            {
                                num1 = 0;
                                if (time1.Ticks == 0)
                                {
                                    time1 = DateTime.Now;
                                }
                                else
                                {
                                    TimeSpan span1 = (TimeSpan)(DateTime.Now - time1);
                                    if (span1.Milliseconds > 0x132)
                                    {
                                        this.InvokeAsyncCallback(ContextLayoutManager._updateLayoutBackground);
                                        element1 = null;
                                        flag1 = false;
                                        return;
                                    }
                                }
                            }
                            element1 = this.ArrangeQueueInternal.GetTopMost();
                            if (element1 == null)
                            {
                                break;
                            }
                            RectangleF rect1 = element1.GetArrangeRect(element1.PreviousArrangeRect);
                            element1.Arrange(rect1);
                        }
                        if (!this.MeasureQueueInternal.IsEmpty)
                        {
                            continue;
                        }
                        this._isInUpdateLayout = false;

                        this.fireSizeChangedEvents();
                        if (!this.hasDirtiness)
                        {
                            this.fireLayoutUpdateEvent();
                        }
                    }
                    element1 = null;
                    flag1 = false;
                }
                finally
                {
                    this._isUpdating = false;
                    this._layoutRequestPosted = false;
                    this._isInUpdateLayout = false;
                    if (flag1)
                    {
                        this._gotException = true;
                        this._forceLayoutElement = element1;
                        this.InvokeAsyncCallback(ContextLayoutManager._updateLayoutBackground);
                    }
                }
            }
        }


        private LayoutQueue ArrangeQueueInternal
        {
            get
            {
                if (this._arrangeQueue == null)
                {
                    this._arrangeQueue = new InternalArrangeQueue();
                }
                return this._arrangeQueue;
            }
        }

        public ILayoutQueue ArrangeQueue
        {
            get { return this.ArrangeQueueInternal; }
        }

        private bool hasDirtiness
        {
            get
            {
                if (this.MeasureQueueInternal.IsEmpty)
                {
                    return !this.ArrangeQueueInternal.IsEmpty;
                }
                return true;
            }
        }

        // TODO: Use weak references to the event handlers...
        public List<EventHandler> LayoutEvents
        {
            get
            {
                if (this._layoutEvents == null)
                {
                    this._layoutEvents = new List<EventHandler>();
                }
                return this._layoutEvents;
            }
        }

        private LayoutQueue MeasureQueueInternal
        {
            get
            {
                if (this._measureQueue == null)
                {
                    this._measureQueue = new InternalMeasureQueue();
                }
                return this._measureQueue;
            }
        }

        public ILayoutQueue MeasureQueue
        {
            get { return this.MeasureQueueInternal; }
        }

        public bool IsUpdating
        {
            get { return this._isUpdating || layoutCalls > 0; }
        }

        public void IncrementLayoutCalls()
        {
            this.layoutCalls++;
        }

        public void DecrementLayoutCalls()
        {
            if (this.layoutCalls > 0)
                this.layoutCalls--;
        }

        internal class InternalArrangeQueue : LayoutQueue
        {
            internal override bool canRelyOnParentRecalc(RadElement parent)
            {
                if (!parent.IsArrangeValid)
                {
                    return !parent.GetBitState(RadElement.ArrangeInProgressStateKey);
                }
                return false;
            }

            internal override ContextLayoutManager.LayoutQueue.Request getRequest(RadElement e)
            {
                return e.ArrangeRequest;
            }

            internal override void invalidate(RadElement e)
            {
                e.InvalidateArrangeInternal();
            }

            internal override void setRequest(RadElement e, ContextLayoutManager.LayoutQueue.Request r)
            {
                e.ArrangeRequest = r;
            }

        }

        internal class InternalMeasureQueue : LayoutQueue
        {
            internal override bool canRelyOnParentRecalc(RadElement parent)
            {
                if (!parent.IsMeasureValid)
                {
                    return !parent.GetBitState(RadElement.MeasureInProgressStateKey);
                }
                return false;
            }

            internal override ContextLayoutManager.LayoutQueue.Request getRequest(RadElement e)
            {
                return e.MeasureRequest;
            }

            internal override void invalidate(RadElement e)
            {
                e.InvalidateMeasureInternal();
            }

            internal override void setRequest(RadElement e, ContextLayoutManager.LayoutQueue.Request r)
            {
                e.MeasureRequest = r;
            }

        }

        internal abstract class LayoutQueue : ILayoutQueue
        {
            internal LayoutQueue()
            {
                for (int num1 = 0; num1 < PocketCapacity; num1++)
                {
                    Request request1 = new Request();
                    request1.Next = this._pocket;
                    this._pocket = request1;
                }
                this._pocketSize = PocketCapacity;
            }

            private void _addRequest(RadElement e)
            {
                Request request1 = this._getNewRequest(e);
                if (request1 != null)
                {
                    request1.Next = this._head;
                    if (this._head != null)
                    {
                        this._head.Prev = request1;
                    }
                    this._head = request1;
                    this.setRequest(e, request1);
                }
            }

            private Request _getNewRequest(RadElement e)
            {
                Request request1;
                if (this._pocket != null)
                {
                    Request request2;
                    request1 = this._pocket;
                    this._pocket = request1.Next;
                    this._pocketSize--;
                    request1.Prev = (Request)(request2 = null);
                    request1.Next = request2;
                }
                else
                {
                    ContextLayoutManager manager1 = e.LayoutManager as ContextLayoutManager;
                    try
                    {
                        request1 = new Request();
                    }
                    catch (OutOfMemoryException exception1)
                    {
                        if (manager1 != null)
                        {
                            manager1.setForceLayout(e);
                        }
                        throw exception1;
                    }
                }
                request1.Target = e;
                return request1;
            }

            private void _removeRequest(Request entry)
            {
                if (entry.Prev == null)
                {
                    this._head = entry.Next;
                }
                else
                {
                    entry.Prev.Next = entry.Next;
                }
                if (entry.Next != null)
                {
                    entry.Next.Prev = entry.Prev;
                }
                this.ReuseRequest(entry);
            }

            //public bool Contains(RadElement e)
            //{
            //    Request request = this._head;
            //    while (request != null)
            //    {
            //        if (object.ReferenceEquals(request.Target, e))
            //            return true;
            //        request = request.Next;
            //    }
            //    return false;
            //}

            public void Add(RadElement e)
            {
                if ((this.getRequest(e) == null) && !e.IsLayoutSuspended)
                {
                    this.RemoveOrphans(e);
                    RadElement element2 = e.Parent;
                    if ((element2 == null) || !this.canRelyOnParentRecalc(element2))
                    {
                        ContextLayoutManager manager1 = e.LayoutManager as ContextLayoutManager;
                        if (!manager1._isDead)
                        {
                            if (this._pocketSize <= PocketReserve)
                            {
                                while (e != null)
                                {
                                    RadElement element1 = e.Parent;
                                    this.invalidate(e);
                                    if (element1 != null)
                                    {
                                        this.Remove(e);
                                    }
                                    else if (this.getRequest(e) == null)
                                    {
                                        this.RemoveOrphans(e);
                                        this._addRequest(e);
                                    }
                                    e = element1;
                                }
                            }
                            else
                            {
                                this._addRequest(e);
                            }
                            manager1.NeedsRecalc();
                        }
                    }
                }
            }

            internal abstract bool canRelyOnParentRecalc(RadElement parent);
            internal abstract Request getRequest(RadElement e);
            internal RadElement GetTopMost()
            {
                RadElement element1 = null;
                ulong num2 = ulong.MaxValue;
                for (Request request1 = this._head; request1 != null; request1 = request1.Next)
                {
                    ulong num1 = request1.Target.TreeLevel;
                    if (num1 < num2)
                    {
                        num2 = num1;
                        element1 = request1.Target;
                    }
                }
                return element1;
            }

            internal abstract void invalidate(RadElement e);
            public void Remove(RadElement e)
            {
                Request request1 = this.getRequest(e);
                if (request1 != null)
                {
                    this._removeRequest(request1);
                    this.setRequest(e, null);
                }
            }

            internal void RemoveOrphans(RadElement parent)
            {
                Request request2;
                for (Request request1 = this._head; request1 != null; request1 = request2)
                {
                    RadElement element1 = request1.Target;
                    request2 = request1.Next;
                    ulong num1 = parent.TreeLevel;
                    if ((element1.TreeLevel == (num1 + 1)) && (element1.Parent == parent))
                    {
                        this._removeRequest(this.getRequest(element1));
                        this.setRequest(element1, null);
                    }
                }
            }

            private void ReuseRequest(Request r)
            {
                r.Target = null;
                if (this._pocketSize < PocketCapacity)
                {
                    r.Next = this._pocket;
                    this._pocket = r;
                    this._pocketSize++;
                }
            }

            internal abstract void setRequest(RadElement e, Request r);

            internal bool IsEmpty
            {
                get
                {
                    return (this._head == null);
                }
            }

            public int Count
            {
                get
                {
                    int res = 0;
                    Request currentRequest = this._head;
                    while (currentRequest != null)
                    {
                        res++;
                        currentRequest = currentRequest.Next;
                    }
                    return res;
                }
            }


            private Request _head;
            private Request _pocket;
            private int _pocketSize;

            internal const int PocketCapacity = 0x99;
            private const int PocketReserve = 8;


            internal class Request
            {
                internal ContextLayoutManager.LayoutQueue.Request Next;
                internal ContextLayoutManager.LayoutQueue.Request Prev;
                internal RadElement Target;
            }
        }
    }
}

