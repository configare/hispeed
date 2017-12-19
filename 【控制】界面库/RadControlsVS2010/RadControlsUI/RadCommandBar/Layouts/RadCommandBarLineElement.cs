using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using Telerik.WinControls.Elements;
using System.ComponentModel;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Design;
using System.Drawing.Design;


namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a row of the <see cref="RadCommandBarElement"/>.
    /// Contains a collection of <see cref="CommandBarStripElement"/> elements.
    /// </summary>
    [Designer(DesignerConsts.RadCommandBarToolstripsHolderPanelDesignerString)]
    public class CommandBarRowElement : RadCommandBarVisualElement
    {
        #region Consts
        const int NOT_SET = -1;
        #endregion

        #region Fields

        protected CommandBarStripElementCollection strips;
        protected RadCommandBarElement owner;
        private float cachedSumOfDesiredSpaces = -1;

        #endregion

        #region Events

        /// <summary>
        /// Occurs before dragging is started.
        /// </summary>
        public event CancelEventHandler BeginDragging;

        /// <summary>
        /// Occurs when item is being dragged.
        /// </summary>
        public event MouseEventHandler Dragging;

        /// <summary>
        /// Occurs when item is released and dragging is stopped.
        /// </summary>
        public event EventHandler EndDragging;

        /// <summary>
        /// Occurs when Orientation property is changed.
        /// </summary>
        public event EventHandler OrientationChanged;

        /// <summary>
        /// Occurs before Orientation property is changed.
        /// </summary>
        public event CancelEventHandler OrientationChanging;

        #endregion

        #region Cstros

        public CommandBarRowElement()
        {
            this.MinSize = new Size(25, 25);
            if (this.Site != null)
            {
                this.MinSize = new Size(20, 20);
            }
        }

        #endregion

        #region Overrides

        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
        {
            base.OnBubbleEvent(sender, args);

            if (args.RoutedEvent == RadCommandBarGrip.BeginDraggingEvent)
            {
                CancelEventArgs dragArgs = (CancelEventArgs)args.OriginalEventArgs;
                OnBeginDragging(sender, dragArgs);
                args.Canceled = dragArgs.Cancel;
            }

            if (args.RoutedEvent == RadCommandBarGrip.EndDraggingEvent)
            {
                EventArgs dragArgs = args.OriginalEventArgs;
                OnEndDragging(sender, dragArgs);
            }

            if (args.RoutedEvent == RadCommandBarGrip.DraggingEvent)
            {
                MouseEventArgs dragArgs = (MouseEventArgs)args.OriginalEventArgs;
                OnDragging(sender, dragArgs);
            }
        }

        public override string Text
        {
            get
            {
                return "";
            }
            set
            {
                base.Text = value;
            }
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.strips = new CommandBarStripElementCollection(this);
            this.strips.ItemTypes = new Type[] { typeof(CommandBarStripElement) };
            this.strips.ItemsChanged += new CommandBarStripElementCollectionItemChangedDelegate(strips_ItemsChanged);
            this.SetOrientationCore(this.Orientation);
        }

        private void strips_ItemsChanged(CommandBarStripElementCollection changed, CommandBarStripElement target, ItemsChangeOperation operation)
        {
            if (operation == ItemsChangeOperation.Inserted || operation == ItemsChangeOperation.Set)
            {
                target.Orientation = this.orientation;
                if (this.owner != null)
                {
                    this.owner.StripInfoHolder.AddStripInfo(target);
                }
            }

            if (operation == ItemsChangeOperation.Removed || operation == ItemsChangeOperation.Setting)
            {
                if (this.owner != null)
                {
                    this.owner.StripInfoHolder.RemoveStripInfo(target);
                }
            }

            if (operation == ItemsChangeOperation.Clearing && this.owner!=null)
            {
                foreach (CommandBarStripElement strip in this.strips)
                {
                    this.owner.StripInfoHolder.RemoveStripInfo(strip);
                }
            }
              
        }

        #endregion

        #region Event management

        /// <summary>
        /// Raises the <see cref="E:CommandBarRowElement.BeginDragging"/> event
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event.</param>
        /// <param name="args">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnBeginDragging(object sender, CancelEventArgs args)
        {
            if (BeginDragging != null)
            {
                BeginDragging(sender, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CommandBarRowElement.EndDragging"/> event
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event.</param>
        /// <param name="args">A <see cref="T:System.EventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnEndDragging(object sender, EventArgs args)
        {
            if (EndDragging != null)
            {
                EndDragging(sender, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CommandBarRowElement.Dragging"/> event
        /// </summary>
        /// <param name="sender">The element that is responsible for firing the event.</param>
        /// <param name="args">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnDragging(object sender, MouseEventArgs args)
        {
            if (Dragging != null)
            {
                Dragging(sender, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CommandBarRowElement.OrientationChanged"/> event
        /// </summary> 
        /// <param name="e">A <see cref="T:System.EventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnOrientationChanged(EventArgs e)
        {
            if (this.OrientationChanged != null)
            {
                this.OrientationChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CommandBarRowElement.OrientationChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the
        /// event data.</param>
        /// <returns>True if the change of orientation should be canceled, false otherwise.</returns>
        protected virtual bool OnOrientationChanging(CancelEventArgs e)
        {
            if (this.OrientationChanging != null)
            {
                this.OrientationChanging(this, e);
                return e.Cancel;
            }
            return false;
        }

        #endregion

        #region Layouts

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            List<RadCommandBarStripPanelLayoutInfo> elementsOrdered = (this.orientation == Orientation.Horizontal) ?
                this.GetChildrenOrdered(CompareCommandBarStripElementByX, availableSize, true) :
                this.GetChildrenOrdered(CompareCommandBarStripElementByY, availableSize, true);

            float totalAvailableSpace = (this.orientation == Orientation.Horizontal) ? availableSize.Width : availableSize.Height;
            this.cachedSumOfDesiredSpaces = this.GetSumOfDesiredSpace(elementsOrdered, availableSize);

            SizeF measuredSize = SizeF.Empty;
            if (this.cachedSumOfDesiredSpaces > totalAvailableSpace && this.Site == null)
            {
                measuredSize = MeasureElementsWithoutFreeSpace(elementsOrdered, availableSize);
            }
            else
            {
                measuredSize = MeasureElementWithFreeSpace(elementsOrdered, availableSize);
            }

            if (float.IsInfinity(availableSize.Height))
            {
                availableSize.Height = measuredSize.Height;
            }
            if (float.IsInfinity(availableSize.Width))
            {
                availableSize.Width = measuredSize.Width;
            }

            return availableSize;
        }

        private SizeF MeasureElementWithFreeSpace(List<RadCommandBarStripPanelLayoutInfo> elementsOrdered, SizeF availableSize)
        {
            int barsCount = this.strips.Count;
            SizeF measuredSize = SizeF.Empty;

            for (int i = 0; i < barsCount; i++)
            {
                RadCommandBarStripPanelLayoutInfo currentElement = elementsOrdered[i];
                if (!currentElement.commandBarStripElement.VisibleInCommandBar && this.Site == null)
                {
                    currentElement.commandBarStripElement.Measure(SizeF.Empty);
                }
                else
                {
                    currentElement.commandBarStripElement.Measure(availableSize);
                }

                measuredSize.Width = Math.Max(measuredSize.Width, currentElement.commandBarStripElement.DesiredSize.Width);
                measuredSize.Height = Math.Max(measuredSize.Height, currentElement.commandBarStripElement.DesiredSize.Height);
            }

            return measuredSize;
        }

        private SizeF MeasureElementsWithoutFreeSpace(List<RadCommandBarStripPanelLayoutInfo> elementsOrdered, SizeF availableSize)
        {
            int barsCount = this.strips.Count;
            float totalAvailableSpace = (this.orientation == Orientation.Horizontal) ? availableSize.Width : availableSize.Height;
            SizeF measuredSize = SizeF.Empty;
            float currentLocation = 0;

            for (int i = 0; i < barsCount; i++)
            {
                RadCommandBarStripPanelLayoutInfo currentElement = elementsOrdered[i];
                RadCommandBarStripPanelLayoutInfo nextElement = (i + 1 < barsCount) ? elementsOrdered[i + 1] : null;

                if (!currentElement.commandBarStripElement.VisibleInCommandBar && this.Site == null)
                {
                    currentElement.commandBarStripElement.Measure(SizeF.Empty);
                    continue;
                }

                float nextLocation = totalAvailableSpace;

                if (nextElement != null)
                {
                    nextLocation = (this.orientation == System.Windows.Forms.Orientation.Horizontal) ?
                                   nextElement.DesiredLocation.X :
                                   nextElement.DesiredLocation.Y;
                }

                float availableSpaceToEnd = (totalAvailableSpace - currentLocation);
                float desiredSpace = (this.orientation == System.Windows.Forms.Orientation.Horizontal) ?
                                     currentElement.ExpectedDesiredSize.Width :
                                     currentElement.ExpectedDesiredSize.Height;
                float desiredSpaceToEnd = currentElement.DesiredSpaceToEnd - desiredSpace;
                float minSpace = (this.orientation == System.Windows.Forms.Orientation.Horizontal) ?
                                 currentElement.commandBarStripElement.MinSize.Width :
                                 currentElement.commandBarStripElement.MinSize.Height;
                float arrangeSpace = Math.Min(desiredSpace, nextLocation - currentLocation);

                if (desiredSpaceToEnd < availableSpaceToEnd - arrangeSpace)
                {
                    float shouldGrow = Math.Min(desiredSpace, arrangeSpace + availableSpaceToEnd - arrangeSpace - desiredSpaceToEnd);
                    arrangeSpace = Math.Max(arrangeSpace, shouldGrow);
                }

                if (nextElement != null && nextElement.MinSpaceToEnd > availableSpaceToEnd - arrangeSpace)
                {
                    arrangeSpace = Math.Min(arrangeSpace, arrangeSpace - (nextElement.MinSpaceToEnd - availableSpaceToEnd + arrangeSpace));
                }

                arrangeSpace = Math.Max(arrangeSpace, minSpace);
                SizeF measureSize = (this.orientation == System.Windows.Forms.Orientation.Horizontal) ?
                                    new SizeF(arrangeSpace, availableSize.Height) :
                                    new SizeF(availableSize.Width, arrangeSpace);

                currentElement.commandBarStripElement.Measure(measureSize);

                float actualDesiredSpace = (this.orientation == System.Windows.Forms.Orientation.Horizontal) ?
                                           currentElement.commandBarStripElement.DesiredSize.Width :
                                           currentElement.commandBarStripElement.DesiredSize.Height;
                currentLocation += actualDesiredSpace;

                if (this.orientation == System.Windows.Forms.Orientation.Horizontal)
                {
                    measuredSize.Width += actualDesiredSpace;
                    measuredSize.Height = Math.Max(measuredSize.Height, currentElement.commandBarStripElement.DesiredSize.Height);
                }
                else
                {
                    measuredSize.Height += actualDesiredSpace;
                    measuredSize.Width = Math.Max(measuredSize.Width, currentElement.commandBarStripElement.DesiredSize.Width);
                }
            }

            return measuredSize;
        }

        private float GetSumOfDesiredSpace(List<RadCommandBarStripPanelLayoutInfo> elementsOrdered, SizeF availableSize)
        {
            float sum = 0;
            foreach (RadCommandBarStripPanelLayoutInfo element in elementsOrdered)
            {
                if (this.orientation == Orientation.Horizontal)
                {
                    sum += element.ExpectedDesiredSize.Width;
                }
                else
                {
                    sum += element.ExpectedDesiredSize.Height;
                }

            }
            return sum;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            int barsCount = this.Children.Count;
            List<RadCommandBarStripPanelLayoutInfo> elementsOrdered = null;
             
            if (this.orientation == Orientation.Horizontal)
            {
                elementsOrdered = this.GetChildrenOrdered(CompareCommandBarStripElementByX, SizeF.Empty, false);
            }
            else
            {
                elementsOrdered = this.GetChildrenOrdered(CompareCommandBarStripElementByY, SizeF.Empty, false);
            }

            this.ArrangeElements(elementsOrdered, finalSize);

            for (int i = 0; i < barsCount; ++i)
            {
                RadCommandBarStripPanelLayoutInfo layoutInfo = elementsOrdered[i];
                this.AdjustDesiredLocationIfEmpty(layoutInfo);
                if (this.RightToLeft && this.Orientation == System.Windows.Forms.Orientation.Horizontal)
                {
                    layoutInfo.ArrangeRectangle.X = finalSize.Width - layoutInfo.ArrangeRectangle.X - layoutInfo.ArrangeRectangle.Width;
                }
                layoutInfo.commandBarStripElement.Arrange(layoutInfo.ArrangeRectangle);
            }

            return finalSize;
        }
           
        private void AdjustDesiredLocationIfEmpty(RadCommandBarStripPanelLayoutInfo layoutInfo)
        {
            if (this.Site != null)
            {
                return;
            }

            if (layoutInfo.commandBarStripElement.DesiredLocation.X < 0 || layoutInfo.commandBarStripElement.DesiredLocation.Y < 0
                || float.IsInfinity(layoutInfo.commandBarStripElement.DesiredLocation.X) || float.IsInfinity(layoutInfo.commandBarStripElement.DesiredLocation.Y))
            {
                layoutInfo.commandBarStripElement.cachedDesiredLocation = new PointF(NOT_SET, NOT_SET);
            }

            if (layoutInfo.commandBarStripElement.DesiredLocation == new PointF(NOT_SET, NOT_SET) ||
                (layoutInfo.commandBarStripElement.DesiredLocation.X == 0 && this.orientation == Orientation.Horizontal) ||
                (layoutInfo.commandBarStripElement.DesiredLocation.Y == 0 && this.orientation == Orientation.Vertical))
            {
                layoutInfo.commandBarStripElement.cachedDesiredLocation = layoutInfo.ArrangeRectangle.Location;
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Moves specified <see cref="CommandBarStripElement"/> in coresponding row 
        /// if its <see cref="P:CommandBarStripElement.DesiredLocation"/> property points to a location in other row.
        /// </summary>
        /// <param name="currentElement">The <see cref="CommandBarStripElement"/> to move.</param>
        protected internal void MoveCommandStripInOtherLine(CommandBarStripElement currentElement)
        {
            if (this.owner == null)
            {
                return;
            }

            if (this.Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                if (currentElement.DesiredLocation.Y < currentElement.ControlBoundingRectangle.Top - SystemInformation.DragSize.Height)
                {
                    this.ResetDesiredLocation(currentElement);
                    this.owner.MoveToUpperLine(currentElement, this);
                    return;
                }
                else if (currentElement.DesiredLocation.Y > currentElement.ControlBoundingRectangle.Bottom + SystemInformation.DragSize.Height)
                {
                    this.ResetDesiredLocation(currentElement);
                    owner.MoveToDownerLine(currentElement, this);
                    return;
                }
            }
            if (this.Orientation == System.Windows.Forms.Orientation.Vertical)
            {
                if (currentElement.DesiredLocation.X < currentElement.ControlBoundingRectangle.Left - SystemInformation.DragSize.Height)
                {
                    this.ResetDesiredLocation(currentElement);
                    this.owner.MoveToUpperLine(currentElement, this);
                    return;
                }
                else if (currentElement.DesiredLocation.X > currentElement.ControlBoundingRectangle.Right + SystemInformation.DragSize.Height)
                {
                    this.ResetDesiredLocation(currentElement);
                    owner.MoveToDownerLine(currentElement, this);
                    return;
                }
            }
        }


        private void ResetDesiredLocation(CommandBarStripElement currentElement)
        {
            //currentElement.cachedDesiredLocation = new PointF(currentElement.DesiredLocation.X, 0);
        }

        private void ArrangeElements(List<RadCommandBarStripPanelLayoutInfo> elementsOrdered, SizeF finalSize)
        {
            float availableSpace = (this.Orientation == Orientation.Horizontal) ? finalSize.Width : finalSize.Height;

            if (this.cachedSumOfDesiredSpaces <= availableSpace && this.Site == null)
            {
                ArrangeElementsWithFreeSpace(elementsOrdered, finalSize);
                ArrangeStretchedElements(elementsOrdered, finalSize);
            }
            else
            {
                ArrangeElementsWithoutFreeSpace(elementsOrdered, finalSize);
            }
        }

        private void ArrangeElementsWithoutFreeSpace(List<RadCommandBarStripPanelLayoutInfo> elementsOrdered, SizeF finalSize)
        {
            int barsCount = elementsOrdered.Count;
            float sumOfSpace = 0;
            float availableSpace = (this.Orientation == Orientation.Horizontal) ? finalSize.Width : finalSize.Height;
            for (int i = 0; i < barsCount; ++i)
            {
                RadCommandBarStripPanelLayoutInfo currentElement = elementsOrdered[i];

                float desiredSpace = (this.orientation == Orientation.Horizontal) ?
                                     currentElement.commandBarStripElement.DesiredSize.Width :
                                     currentElement.commandBarStripElement.DesiredSize.Height;

                PointF arrangePoint = (this.orientation == System.Windows.Forms.Orientation.Horizontal) ?
                                      new PointF(sumOfSpace, 0) : new PointF(0, sumOfSpace);
                SizeF arrangeSize = (this.orientation == System.Windows.Forms.Orientation.Horizontal) ?
                                    new SizeF(desiredSpace, finalSize.Height) :
                                    new SizeF(finalSize.Width, desiredSpace);

                if (i + 1 == barsCount)
                {
                    if (this.orientation == System.Windows.Forms.Orientation.Horizontal)
                    {
                        arrangeSize.Width = availableSpace - sumOfSpace;
                    }
                    else
                    {
                        arrangeSize.Height = availableSpace - sumOfSpace;
                    }
                }

                currentElement.ArrangeRectangle = new RectangleF(arrangePoint, arrangeSize);

                sumOfSpace += desiredSpace;
            }
        }

        private void ArrangeStretchedElements(List<RadCommandBarStripPanelLayoutInfo> elementsOrdered, SizeF finalSize)
        {
            float availableSpace = (this.Orientation == Orientation.Horizontal) ? finalSize.Width : finalSize.Height;
            int barsCount = elementsOrdered.Count;
            for (int i = 0; i < barsCount; ++i)
            {
                RadCommandBarStripPanelLayoutInfo previousElement = (i - 1 >= 0) ? elementsOrdered[i - 1] : null;
                RadCommandBarStripPanelLayoutInfo currentElement = elementsOrdered[i];
                RadCommandBarStripPanelLayoutInfo nextElement = (i + 1 < barsCount) ? elementsOrdered[i + 1] : null;

                if (!currentElement.commandBarStripElement.VisibleInCommandBar
                    || currentElement.commandBarStripElement.Visibility == ElementVisibility.Collapsed)
                {
                    continue;
                }

                if ((this.orientation == System.Windows.Forms.Orientation.Horizontal && currentElement.commandBarStripElement.StretchHorizontally) ||
                    (this.orientation == System.Windows.Forms.Orientation.Vertical && currentElement.commandBarStripElement.StretchVertically))
                {
                    float nextElementPlace = availableSpace;
                    if (nextElement != null)
                    {
                        nextElementPlace = (this.orientation == Orientation.Horizontal) ?
                                           nextElement.ArrangeRectangle.X :
                                           nextElement.ArrangeRectangle.Y;
                    }

                    float previousElementPlace = 0;
                    if (previousElement != null)
                    {
                        previousElementPlace = (this.orientation == Orientation.Horizontal) ?
                                               previousElement.ArrangeRectangle.X + previousElement.ArrangeRectangle.Width :
                                               previousElement.ArrangeRectangle.Y + previousElement.ArrangeRectangle.Height;
                    }

                    float desiredSpace = nextElementPlace - previousElementPlace;
                    SizeF arrangeSize = (this.orientation == System.Windows.Forms.Orientation.Horizontal) ?
                                        new SizeF(desiredSpace, finalSize.Height) :
                                        new SizeF(finalSize.Width, desiredSpace);
                    PointF arrangePoint = (this.orientation == Orientation.Horizontal) ?
                                          new PointF(previousElementPlace, currentElement.ArrangeRectangle.Y) :
                                          new PointF(currentElement.ArrangeRectangle.X, previousElementPlace);

                    currentElement.ArrangeRectangle.Location = arrangePoint;
                    currentElement.ArrangeRectangle.Size = arrangeSize;
                }
            }
        }

        private void ArrangeElementsWithFreeSpace(List<RadCommandBarStripPanelLayoutInfo> elementsOrdered, SizeF finalSize)
        {
            float availableSpace = (this.Orientation == Orientation.Horizontal) ? finalSize.Width : finalSize.Height;
            int barsCount = elementsOrdered.Count;
            float sumOfSpace = 0;
            for (int i = 0; i < barsCount; ++i)
            {
                RadCommandBarStripPanelLayoutInfo currentElement = elementsOrdered[i];

                PointF desiredLocation = currentElement.DesiredLocation;
                float desiredSpace = (this.orientation == Orientation.Horizontal) ?
                                     currentElement.commandBarStripElement.DesiredSize.Width :
                                     currentElement.commandBarStripElement.DesiredSize.Height;

                float arrangedPlace = (this.orientation == Orientation.Horizontal) ?
                                      desiredLocation.X :
                                      desiredLocation.Y;

                arrangedPlace = Math.Max(arrangedPlace, sumOfSpace);

                if (currentElement.IntersectionSpaceToEnd > 0)
                {
                    float freeSpace = arrangedPlace - sumOfSpace;
                    float offset = Math.Min(freeSpace, currentElement.IntersectionSpaceToEnd);
                    arrangedPlace = Math.Max(sumOfSpace, arrangedPlace - offset);
                }

                float spaceToEnd = availableSpace - arrangedPlace;
                float spaceLeft = spaceToEnd - currentElement.DesiredSpaceToEnd;
                if (spaceLeft < 0)
                {
                    arrangedPlace += spaceLeft;
                }

                sumOfSpace = arrangedPlace + desiredSpace;

                PointF arrangePoint = (this.orientation == System.Windows.Forms.Orientation.Horizontal) ?
                                      new PointF(arrangedPlace, 0) :
                                      new PointF(0, arrangedPlace);

                SizeF arrangeSize = (this.orientation == System.Windows.Forms.Orientation.Horizontal) ?
                                    new SizeF(desiredSpace, finalSize.Height) :
                                    new SizeF(finalSize.Width, desiredSpace);

                currentElement.ArrangeRectangle = new RectangleF(arrangePoint, arrangeSize);
            }
        }

        private List<RadCommandBarStripPanelLayoutInfo> GetChildrenOrdered(System.Comparison<RadCommandBarStripPanelLayoutInfo> comparison, SizeF availableSize, bool calculateExpectedSize)
        {
            int barsCount = this.Strips.Count;

            List<RadCommandBarStripPanelLayoutInfo> elements = new List<RadCommandBarStripPanelLayoutInfo>(barsCount);
            for (int i = 0; i < barsCount; ++i)
            {
                elements.Add(new RadCommandBarStripPanelLayoutInfo(this.Strips[i]));
            }

            if (this.Site == null)
            {
                this.StableSort(elements, comparison);
            }

            float desiredSpaceToEnd = 0;
            float minSpaceToEnd = 0;
            int dragedIndex = -1;
            for (int i = barsCount - 1; i >= 0; --i)
            {

                elements[i].ExpectedDesiredSize = (calculateExpectedSize) ?
                    elements[i].commandBarStripElement.GetExpectedSize(availableSize) :
                    elements[i].commandBarStripElement.DesiredSize;

                float currentSpace = (this.orientation == System.Windows.Forms.Orientation.Horizontal) ?
                    elements[i].ExpectedDesiredSize.Width :
                    elements[i].ExpectedDesiredSize.Height;
                float currentMinSpace = 0;
                if (elements[i].commandBarStripElement.VisibleInCommandBar && elements[i].commandBarStripElement.Visibility != ElementVisibility.Collapsed)
                {
                    currentMinSpace = (this.orientation == System.Windows.Forms.Orientation.Horizontal) ?
                       elements[i].commandBarStripElement.MinSize.Width :
                       elements[i].commandBarStripElement.MinSize.Height;
                }

                desiredSpaceToEnd += currentSpace;
                minSpaceToEnd += currentMinSpace;
                elements[i].DesiredSpaceToEnd = desiredSpaceToEnd;
                elements[i].MinSpaceToEnd = minSpaceToEnd;

                if (elements[i].commandBarStripElement.IsDrag)
                    dragedIndex = i;
            }

            if (dragedIndex == -1) return elements;

            for (int i = dragedIndex - 1; i >= 0; --i)
            {
                float currentPosition = (this.orientation == Orientation.Horizontal) ?
                    elements[i].DesiredLocation.X :
                    elements[i].DesiredLocation.Y;

                float nextPosition = (this.orientation == Orientation.Horizontal) ?
                    elements[i + 1].DesiredLocation.X :
                    elements[i + 1].DesiredLocation.Y;

                float currentSpace = (this.orientation == System.Windows.Forms.Orientation.Horizontal) ?
                    elements[i].commandBarStripElement.DesiredSize.Width :
                    elements[i].commandBarStripElement.DesiredSize.Height;

                elements[i].IntersectionSpaceToEnd = elements[i + 1].IntersectionSpaceToEnd + currentPosition + currentSpace - nextPosition;
                if (elements[i].IntersectionSpaceToEnd <= 0) break;
            }

            return elements;
        }

        private void StableSort(List<RadCommandBarStripPanelLayoutInfo> elements, Comparison<RadCommandBarStripPanelLayoutInfo> comparison)
        {
            int elementsCount = elements.Count - 1;
            bool madeSwaps = false;

            do
            {
                madeSwaps = false;

                for (int i = 0; i < elementsCount; i++)
                {
                    if (comparison.Invoke(elements[i], elements[i + 1]) > 0)
                    {
                        madeSwaps = true;
                        RadCommandBarStripPanelLayoutInfo temp = elements[i];
                        elements[i] = elements[i + 1];
                        elements[i + 1] = temp;
                    }
                }
            }
            while (madeSwaps);
        }

        private static int CompareCommandBarStripElementByX(RadCommandBarStripPanelLayoutInfo a, RadCommandBarStripPanelLayoutInfo b)
        {
            if (a.DesiredLocation.X > b.DesiredLocation.X)
            {
                return 1;
            }

            if (a.DesiredLocation.X < b.DesiredLocation.X)
            {
                return -1;
            }

            return 0;
        }

        private static int CompareCommandBarStripElementByY(RadCommandBarStripPanelLayoutInfo a, RadCommandBarStripPanelLayoutInfo b)
        {
            if (a.DesiredLocation.Y > b.DesiredLocation.Y)
            {
                return 1;
            }

            if (a.DesiredLocation.Y < b.DesiredLocation.Y)
            {
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Applies the new orientation to the element and its children.
        /// </summary>
        /// <param name="newOrientation">The orientation to apply.</param>
        protected internal void SetOrientationCore(Orientation newOrientation)
        {
            if (newOrientation == Orientation.Vertical)
            {
                this.StretchHorizontally = false;
                this.StretchVertically = true;
            }
            else
            {
                this.StretchHorizontally = true;
                this.StretchVertically = false;
            }

            foreach (CommandBarStripElement item in strips)
            {
                item.SetOrientationCore(newOrientation);
            }

            this.orientation = newOrientation;
        }

        #endregion

        #region IbarsOwner Members

        /// <summary>
        /// Gets the <see cref="CommandBarStripElement"/> elements contained in this row.
        /// </summary>
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(DesignerConsts.CommandBarStripElementCollectionEditorString, typeof(UITypeEditor))]
        public CommandBarStripElementCollection Strips
        {
            get
            {
                return this.strips;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="RadCommandBarElement"/> that owns this row.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual RadCommandBarElement Owner
        {
            get
            {
                return this.owner;
            }

            set
            {
                if (this.owner == value)
                {
                    return;
                }

                this.owner = value;

                if (this.owner != null)
                {
                    this.Orientation = owner.Orientation;

                    foreach (CommandBarStripElement strip in this.Strips)
                    {
                        this.owner.StripInfoHolder.AddStripInfo(strip);
                    }
                }
            }
        }

        [RadPropertyDefaultValue("Orientation", typeof(CommandBarRowElement))]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Browsable(false)]
        public override Orientation Orientation
        {
            get
            {
                return base.orientation;
            }
            set
            {
                if (this.orientation != value && !this.OnOrientationChanging(new CancelEventArgs()))
                {
                    this.orientation = value;
                    this.SetOrientationCore(value);
                    this.InvalidateMeasure(true);
                    this.OnOrientationChanged(new EventArgs());
                }
            }
        }

        #endregion

        protected internal override bool? ShouldSerializeProperty(PropertyDescriptor property)
        {
            if (property.Name == "StretchHorizontally" ||
                property.Name == "StretchVertically")
                return false;
            return base.ShouldSerializeProperty(property);
        }
    }
}
