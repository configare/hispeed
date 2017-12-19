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
    public abstract class ComponentElementTree : RadElementTree
    {
        // Fields
        internal protected bool ChildItemsCreatedInControl = false;

        #region Constructors
        public ComponentElementTree(IComponentTreeHandler owner) 
            : base(owner)
        {
            
        }
        #endregion

        /// <summary>Returns the element at the given point.</summary>
        public RadElement GetElementAtPoint(Point point)
        {
            return GetElementAtPoint(this.RootElement, point, null);
        }

        internal RadElement GetElementAtPoint(RadElement parent, Point point, List<RadElement> foundElements)
        {
            if (parent.Visibility != ElementVisibility.Visible)
            {
                return null;
            }

            ChildrenListOptions childrenFlags = ChildrenListOptions.ZOrdered | 
                                                ChildrenListOptions.ReverseOrder | 
                                                ChildrenListOptions.IncludeOnlyVisible;

            Rectangle client = this.Control.ClientRectangle;

            foreach (RadElement element in parent.GetChildren(childrenFlags))
            {
                if (element.ElementState != ElementState.Loaded)
                {
                    continue;
                }

                if (element.HitTest(point))
                {
                    bool addElement = (foundElements != null) && !(element is BorderPrimitive);
                    if (addElement)
                        foundElements.Add(element);

                    RadElement res = GetElementAtPoint(element, point, foundElements);
                    
                    if (res != null)
                        return res;

                    if (element.ShouldHandleMouseInput)
                        return element;
                }
            }

            return null;
        }
    }
}
