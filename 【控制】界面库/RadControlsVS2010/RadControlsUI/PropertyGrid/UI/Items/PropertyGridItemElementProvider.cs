using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class PropertyGridItemElementProvider : VirtualizedPanelElementProvider<PropertyGridItemBase, PropertyGridItemElementBase>
    {
        #region Fields

        private PropertyGridTableElement propertyGridElement;

        #endregion

        #region Constructor

        public PropertyGridItemElementProvider(PropertyGridTableElement propertyGridElement)
        {
            this.propertyGridElement = propertyGridElement;
        }

        #endregion

        #region Properties

        protected PropertyGridTableElement PropertyGridElement
        {
            get
            {
                return this.propertyGridElement;
            }
        }

        #endregion

        #region Methods

        public override IVirtualizedElement<PropertyGridItemBase> CreateElement(PropertyGridItemBase data, object context)
        {
            if (propertyGridElement != null)
            {
                CreatePropertyGridItemElementEventArgs args = new CreatePropertyGridItemElementEventArgs(data);
                propertyGridElement.OnCreateItemElement(args);
                if (args.ItemElement != null)
                {
                    return args.ItemElement;
                }
            }

            if (data is PropertyGridItem)
            {
                return new PropertyGridItemElement();
            }

            if (data is PropertyGridGroupItem)
            {
                return new PropertyGridGroupElement();
            }

            return base.CreateElement(data, context);
        }

        public override SizeF GetElementSize(PropertyGridItemBase item)
        {
            return new SizeF(0, propertyGridElement.ItemHeight);
        }

        #endregion
    }
}
