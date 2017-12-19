using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Drawing;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Themes.Design;
using System.Collections;
using System.Reflection;
using Telerik.WinControls.Keyboard;
using Telerik.WinControls.Assistance;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using Telerik.WinControls.Layouts;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Telerik.WinControls
{
    public class ComponentInputBehavior : ComponentBehavior
    {
        // Fields
        private bool disableMouseEvents;
        private MouseHoverTimer mouseHoverTimer;
        private Timer elementUnderMouseMonitorTimer = null;
        private List<RadElement> hoveredElements = new List<RadElement>();
        private ArrayList elementsUnderMouseToMonitor = new ArrayList();

        #region Constructors
        public ComponentInputBehavior(IComponentTreeHandler owner)
            : base(owner)
        {
            //DK_2006_09_18
            this.ShowItemToolTips = this.DefaultShowItemToolTips;
        }
        #endregion

        #region Mouse events

        internal protected virtual void OnMouseCaptureChanged(EventArgs e)
        {
            if (this.ItemCapture != null)
            {
                RadItem capturedItem = this.ItemCapture as RadItem;
                if (capturedItem != null)
                {
                    Point mouseLocation = Cursor.Position;
                    capturedItem.CallOnLostMouseCapture(new MouseEventArgs(
                        MouseButtons.None, 1, mouseLocation.X, mouseLocation.Y, 0));
                }
                this.ItemCapture = null;
            }
            this.Owner.CallOnMouseCaptureChanged(e);
        }

        internal protected virtual bool OnMouseUp(MouseEventArgs e)
        {
            //base.OnMouseUp(e);
            if (!DisableMouseEvents)
            {
                if (this.OwnerControl.CausesValidation && this.ValidationCancelled)
                {
                    return true;
                }
                if (this.OwnerControl.Capture && this.ItemCapture != null)
                {
                    this.ItemCapture.CallDoMouseUp(e);
                }
                else if (this.selectedElement != null)
                {
                    this.selectedElement.CallDoMouseUp(e);
                }
                else //test if mouse is over any elemenet, select it and call mouse up
                {
                    //note we do not call the following method, because on after a modal dialog closed, windows sends MouseUp
                    //event without subsequent MouseLeave !
                    //this.SelectElementOnMouseOver(e);

                    RadElement elementUnderMouse = this.Owner.ElementTree.GetElementAtPoint(
                        this.Owner.ElementTree.RootElement, e.Location, null);
                    if (elementUnderMouse != null)
                    {
                        elementUnderMouse.CallDoMouseUp(e);
                    }
                }
            }
            return false;
        }

        internal protected virtual bool OnMouseDown(MouseEventArgs e)
        {
            //base.OnMouseDown(e);
            if (!DisableMouseEvents)
            {
                if (this.OwnerControl.CausesValidation && this.ValidationCancelled)
                {
                    return true;
                }
                if (this.OwnerControl.Capture && this.ItemCapture != null)
                {
                    this.ItemCapture.CallDoMouseDown(e);
                }
                else
                {
                    this.SelectElementOnMouseOver(e);
                    if (this.selectedElement != null)
                    {
                        this.selectedElement.CallDoMouseDown(e);
                    }
                }
            }

            // Key Tips Logic
            if (this.IsKeyMapActive)
            {
                this.ResetKeyMap();
            }

            return false;
        }

        internal protected virtual bool OnDoubleClick(EventArgs e)
        {
            return false;
        }

        internal protected virtual bool OnClick(EventArgs e)
        {
            return false;
        }

        internal protected virtual bool OnMouseEnter(EventArgs e)
        {
            if (!DisableMouseEvents)
            {
                this.MouseOver = true;
                this.Owner.ElementTree.RootElement.IsMouseOver = true;
                this.Owner.ElementTree.RootElement.IsMouseOverElement = true;
            }

            return false;
        }

        internal protected virtual bool OnMouseLeave(EventArgs e)
        {
            this.MouseOver = false;

            if (!DisableMouseEvents)
            {
                if (this.selectedElement != null)
                {
                    this.HandleMouseLeave(e);
                }

                this.Owner.ElementTree.RootElement.IsMouseOver = false;
                this.Owner.ElementTree.RootElement.IsMouseOverElement = false;
            }

            return false;
        }

        internal protected virtual bool OnMouseMove(MouseEventArgs e)
        {
            if (!DisableMouseEvents)
            {
                this.MouseOver = true;

                if (this.OwnerControl.Capture && this.ItemCapture != null)
                {
                    this.ItemCapture.CallDoMouseMove(e);
                    if (this.ItemCapture != null)
                    {
                        if (this.ItemCaptureState == false && this.ItemCapture.HitTest(e.Location))
                        {
                            this.ItemCapture.CallDoMouseEnter(e);
                            this.ItemCaptureState = true;
                        }
                        else if (this.ItemCaptureState == true && !this.ItemCapture.HitTest(e.Location))
                        {
                            this.ItemCapture.CallDoMouseLeave(e);
                            this.ItemCaptureState = false;
                        }
                    }
                }
                else
                {
                    SelectElementOnMouseOver(e);
                }
            }
            return false;
        }

        internal protected virtual bool OnMouseHover(EventArgs e)
        {
            if (!DisableMouseEvents)
            {
                if (this.OwnerControl.Capture && this.ItemCapture != null)
                {
                    this.ItemCapture.CallDoMouseHover(e);
                }
            }

            return false;
        }

        internal protected virtual bool OnMouseWheel(MouseEventArgs e)
        {
            //base.OnMouseWheel(e);
            if (this.currentFocusedElement != null &&
                this.currentFocusedElement is RadItem)
            {
                ((RadItem)this.currentFocusedElement).CallRaiseMouseWheel(e);
            }
            return false;
        }

        protected void HandleMouseLeave(EventArgs e)
        {
            this.HandleMouseLeave(e, null);
        }

        protected void HandleMouseLeave(EventArgs e, RadElement elementUnderMouse)
        {
            if (!this.Owner.IsDesignMode)
            {
                this.MouseHoverTimer.Cancel(this.selectedElement);
            }
            try
            {
                if (this.hoveredElements != null)
                {
                    foreach (RadElement hoveredElement in this.hoveredElements)
                    {
                        hoveredElement.IsMouseOverElement = false;
                        if (!this.MouseOver)
                        {
                            bool change = hoveredElement.IsMouseOver;
                            hoveredElement.IsMouseOver = false;
                            if (!change)
                            {
                                hoveredElement.UpdateContainsMouse();
                            }
                        }
                    }

                    this.hoveredElements.Clear();
                    this.hoveredElements = null;
                }
                CheckAddParentElementsUnderMouseToMonitor(this.selectedElement);
                if (elementUnderMouse != null && elementUnderMouse.NotifyParentOnMouseInput)
                {
                    bool found = false;
                    for (RadElement parent = elementUnderMouse.Parent; parent != null; parent = parent.Parent)
                    {
                        if (parent == this.selectedElement)
                        {
                            found = true;
                            AddElementsUnderMouseToMonitor(this.selectedElement);
                            break;
                        }
                    }

                    if (!found)
                        this.selectedElement.CallDoMouseLeave(e);
                }
                else
                    this.selectedElement.CallDoMouseLeave(e);
            }
            finally
            {
                this.selectedElement = null;
            }
        }

        #endregion

        #region Keyboard events
        internal protected virtual bool OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            return false;
        }

        internal protected virtual bool OnKeyDown(KeyEventArgs e)
        {
            //base.OnKeyDown(e);
            if (this.currentFocusedElement != null &&
                this.currentFocusedElement is RadItem)
            {
                ((RadItem)this.currentFocusedElement).CallRaiseKeyDown(e);
            }
            return false;
        }

        internal protected virtual void OnKeyDown(RadElement routedSender, KeyEventArgs e)
        {
            //base.OnKeyDown(e);
            if (this.currentFocusedElement != null && (routedSender != this.currentFocusedElement) &&
                this.currentFocusedElement is RadItem)
            {
                ((RadItem)this.currentFocusedElement).CallRaiseKeyDown(e);
            }
        }

        internal protected virtual bool OnKeyPress(KeyPressEventArgs e)
        {

            //base.OnKeyPress(e);
            if (this.currentFocusedElement != null &&
                this.currentFocusedElement is RadItem)
            {
                ((RadItem)this.currentFocusedElement).CallRaiseKeyPress(e);
            }
            return false;
        }

        internal protected virtual void OnKeyPress(RadElement routedSender, KeyPressEventArgs e)
        {

            //base.OnKeyPress(e);
            if (this.currentFocusedElement != null && (routedSender != this.currentFocusedElement) &&
                this.currentFocusedElement is RadItem)
            {
                ((RadItem)this.currentFocusedElement).CallRaiseKeyPress(e);
            }
        }

        internal protected virtual bool OnKeyUp(KeyEventArgs e)
        {
            //base.OnKeyUp(e);
            if (this.currentFocusedElement != null &&
                this.currentFocusedElement is RadItem)
            {
                ((RadItem)this.currentFocusedElement).CallRaiseKeyUp(e);
            }
            return false;
        }

        internal protected virtual void OnKeyUp(RadElement routedSender, KeyEventArgs e)
        {
            //base.OnKeyUp(e);
            if (this.currentFocusedElement != null && (routedSender != this.currentFocusedElement) &&
                this.currentFocusedElement is RadItem)
            {
                ((RadItem)this.currentFocusedElement).CallRaiseKeyUp(e);
            }
        }

        #endregion

        #region Properties

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)]
        public bool DisableMouseEvents
        {
            get
            {
                return disableMouseEvents;
            }
            set
            {
                disableMouseEvents = value;
            }
        }

        protected MouseHoverTimer MouseHoverTimer
        {
            get
            {
                if (this.mouseHoverTimer == null)
                {
                    this.mouseHoverTimer = new MouseHoverTimer();
                }
                return this.mouseHoverTimer;
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.mouseHoverTimer != null)
                {
                    this.mouseHoverTimer.Cancel();
                    this.mouseHoverTimer.Dispose();
                }
                if (this.elementUnderMouseMonitorTimer != null)
                {
                    this.elementUnderMouseMonitorTimer.Stop();
                    this.elementUnderMouseMonitorTimer.Dispose();
                }

                base.Dispose(disposing);
            }
        }

        private void AddElementsUnderMouseToMonitor(RadElement element)
        {
            this.elementsUnderMouseToMonitor.Add(element);
        }

        private void RemoveElementsUnderMouseToMonitor(RadElement element)
        {
            this.elementsUnderMouseToMonitor.Remove(element);
        }

        private void EnsureElementUnderMouseMonitorTimer()
        {
            if (this.elementUnderMouseMonitorTimer == null)
            {
                elementUnderMouseMonitorTimer = new Timer();
                elementUnderMouseMonitorTimer.Interval = 50;
                elementUnderMouseMonitorTimer.Tick += new EventHandler(ElementUnderMouseMonitorTimer_Tick);
                elementUnderMouseMonitorTimer.Start();
            }
        }

        private void ElementUnderMouseMonitorTimer_Tick(object sender, EventArgs e)
        {
            foreach (RadElement element in this.elementsUnderMouseToMonitor)
            {
                if (!element.IsInValidState(true))
                {
                    continue;
                }

                if (element.ElementTree.Control != this.Owner)
                {
                    continue;
                }

                if (element.IsMouseOver)
                {
                    element.CallDoMouseLeave(System.EventArgs.Empty);
                    element.IsMouseOverElement = false;
                }
            }

            this.elementsUnderMouseToMonitor.Clear();
        }

        private void CheckRemoveParentElementsUnderMouseToMonitor(RadElement elementUnderMouse)
        {
            if (!elementUnderMouse.NotifyParentOnMouseInput)
            {
                return;
            }
            for (RadElement element = elementUnderMouse.Parent; element != null; element = element.Parent)
            {
                if (element.ShouldHandleMouseInput)
                {
                    if (!element.IsMouseOver)
                    {
                        element.CallDoMouseEnter(System.EventArgs.Empty);
                    }

                    this.RemoveElementsUnderMouseToMonitor(element);

                    if (!element.NotifyParentOnMouseInput)
                    {
                        break;
                    }
                }
            }
        }

        private void CheckAddParentElementsUnderMouseToMonitor(RadElement elementUnderMouse)
        {
            if (!elementUnderMouse.NotifyParentOnMouseInput)
            {
                return;
            }

            for (RadElement element = elementUnderMouse.Parent; element != null; element = element.Parent)
            {
                if (element.ShouldHandleMouseInput)
                {
                    this.AddElementsUnderMouseToMonitor(element);

                    if (!element.NotifyParentOnMouseInput)
                    {
                        break;
                    }
                }
            }
        }

        private RadElement GetTopmostHoveredElementNoBoxElements(List<RadElement> hoveredElementList)
        {
            if (hoveredElementList == null)
                return null;

            RadElement res = null;

            for (int i = hoveredElementList.Count - 1; i >= 0; i--)
            {
                res = hoveredElementList[i];
                if (!(res is IBoxElement))
                {
                    break;
                }
            }

            return res;
        }

        public RadElement GetHoveredRadElement()
        {
            return this.GetTopmostHoveredElementNoBoxElements(this.hoveredElements);
        }

        private void HandleHoveredElementsChanged(List<RadElement> newHoveredElements)
        {
            //Hashtable intersection = new Hashtable();

            if (this.hoveredElements == null)
                this.hoveredElements = new List<RadElement>();

            if (newHoveredElements == null)
                newHoveredElements = new List<RadElement>();

            foreach (RadElement element in newHoveredElements)
            {
                bool found = false;
                foreach (RadElement oldHovered in hoveredElements)
                {
                    if (element == oldHovered)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) //Element hovered
                {
                    element.IsMouseOverElement = true;
                    //Debug.WriteLine("new IsMouseOverElement-" + element.ToString());
                }
            }
            foreach (RadElement element in hoveredElements)
            {
                bool found = false;
                foreach (RadElement newHovered in newHoveredElements)
                {
                    if (element == newHovered)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) //Element not hovered
                {
                    element.IsMouseOver = false;
                    element.IsMouseOverElement = false;
                }
            }

            RadElement oldTopHovered;
            oldTopHovered = this.GetTopmostHoveredElementNoBoxElements(this.hoveredElements);

            RadElement newTopHovered;
            newTopHovered = this.GetTopmostHoveredElementNoBoxElements(newHoveredElements);

            if (oldTopHovered != newTopHovered)
            {
                this.OnHoveredElementChanged(new HoveredElementChangedEventArgs(newTopHovered));
            }

            hoveredElements = newHoveredElements;
        }

        /// <summary>Fires when hovered element is changed.</summary>
        public event HoveredElementChangedEventHandler HoveredElementChanged;
        protected virtual void OnHoveredElementChanged(HoveredElementChangedEventArgs e)
        {
            if (HoveredElementChanged != null)
                HoveredElementChanged(this, e);
        }

        private void SelectElementOnMouseOver(MouseEventArgs e)
        {
            List<RadElement> newHoveredElements = new List<RadElement>();
            RadElement elementUnderMouse = this.Owner.ElementTree.GetElementAtPoint(
                this.Owner.ElementTree.RootElement, e.Location, newHoveredElements);

            //Handle the actually hovered element
            this.HandleHoveredElementsChanged(newHoveredElements);

            if (elementUnderMouse != selectedElement)
            {
                EnsureElementUnderMouseMonitorTimer();

                //Debug.WriteLine("----------------------------------------------------");
                if ((selectedElement != null) && (elementUnderMouse != null))
                {
                    //Debug.WriteLine("selectedElement: " + selectedElement.ToString());
                    //Debug.WriteLine("elementUnderMouse: " + elementUnderMouse.ToString());
                    //if (elementUnderMouse is FillPrimitive)
                    //	Debug.WriteLine("elementUnderMouse.Parent: " + elementUnderMouse.Parent.ToString());

                    if (!
                        (elementUnderMouse.ShouldHandleMouseInput &&
                         selectedElement.ShouldHandleMouseInput &&
                         elementUnderMouse.NotifyParentOnMouseInput &&
                         !selectedElement.NotifyParentOnMouseInput
                        ))
                    {
                        //Debug.WriteLine("HandleMouseLeave1 selectedElement");
                        HandleMouseLeave(e, elementUnderMouse);
                    }
                    else
                    {
                        this.AddElementsUnderMouseToMonitor(selectedElement);
                    }
                }
                else
                    if (elementUnderMouse == null)
                    {
                        //Debug.WriteLine("HandleMouseLeave2 selectedElement");
                        HandleMouseLeave(e);
                    }

                if (elementUnderMouse != null)
                {
                    //Debug.WriteLine("DoMouseEnter elementUnderMouse");
                    this.RemoveElementsUnderMouseToMonitor(elementUnderMouse);
                    this.CheckRemoveParentElementsUnderMouseToMonitor(elementUnderMouse);

                    elementUnderMouse.CallDoMouseEnter(e);

                    if (!this.Owner.IsDesignMode)
                    {
                        this.MouseHoverTimer.Start(elementUnderMouse);
                    }

                    //elementUnderMouse.StopMouseLeaveNotifyTimer();
                }
                //Debug.WriteLine("----------------------------------------------------");
            }

            this.selectedElement = elementUnderMouse;

            if ((this.selectedElement != null) && (this.selectedElement.ShouldHandleMouseInput))
            {
                this.selectedElement.CallDoMouseMove(e);
            }
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show("Internal error finding item under mouse " + ex.ToString());
            //}
        }
    }
}
