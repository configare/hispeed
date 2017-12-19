using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using Telerik.WinControls.UI.Properties;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a base class for most of the <see cref="RadCommandBar"/> elements.
    /// </summary>
    public class RadCommandBarVisualElement : LightVisualElement
    {
        #region Fields

        protected Orientation orientation;        
        private CollectionBase owner; 
        private Orientation cachedOrientation;

        #endregion

        #region Cstros
        public RadCommandBarVisualElement()
        {
            this.cachedOrientation = (Orientation)this.GetValue(RadCommandBarVisualElement.OrientationProperty);
            this.orientation = this.cachedOrientation;
        }

        #endregion

        #region Overrides

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.DrawText = false;           
        }

        protected internal override Padding GetBorderThickness(bool checkDrawBorder)
        {
            return base.GetBorderThickness(true);
        }

        #endregion

        #region Properties

        public static RadProperty OrientationProperty = RadProperty.Register(
          "Orientation", typeof(Orientation), typeof(RadCommandBarVisualElement), new RadElementPropertyMetadata(
                Orientation.Horizontal, ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.CanInheritValue));

        /// <summary>
        /// Gets or sets the orientation of the element - colud be horizontal or vertical.
        /// </summary>
        public virtual Orientation Orientation
        {
            get
            {
                return cachedOrientation;
            }
            set
            {
                if (value == this.cachedOrientation)
                {
                    return;
                }

                this.FillPrimitiveImpl.InvalidateFillCache(RadCommandBarVisualElement.OrientationProperty);
                this.InvalidateMeasure();             
                this.cachedOrientation = value;
                this.orientation = value;
                this.SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name that is displayed in command bar dialogs and context menus.
        /// </summary>
        public string DisplayName
        {
            get;
            set;
        }

        #endregion

        #region Collection

        protected internal virtual void SetOwnerCommandBarCollection(CollectionBase ownerCollection)
        {
            this.owner = ownerCollection;
        }

        protected override void DisposeManagedResources()
        {
            //logic to remove the item from its owner collection upon disposing
            if (this.owner != null)
            {
                int index = ((IList)this.owner).IndexOf(this);
                if (index >= 0)
                {
                    this.owner.RemoveAt(index);
                }
                
                this.owner = null;
            }

            base.DisposeManagedResources();
        }

        #endregion

        #region Layouts

        //protected override System.Drawing.SizeF MeasureOverride(System.Drawing.SizeF availableSize)
        //{
        //     SizeF measuredSize = base.MeasureOverride(availableSize);                      
        //     //using a switch in case that should add left orientation and right orientation
        //     switch (this.Orientation)
        //     {
        //         case Orientation.Vertical:
        //             measuredSize = new SizeF(measuredSize.Height, measuredSize.Width);
        //             break;
        //     }
            
        //     return measuredSize;
        //}

        #endregion
    }
}

