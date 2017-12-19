using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
    public abstract class DetailListViewCellElement : LightVisualElement,
    IVirtualizedElement<ListViewDetailColumn>
    {
        #region RadProperties

        public static RadProperty CurrentProperty = RadProperty.Register("Current",
               typeof(bool), typeof(DetailListViewCellElement), new RadElementPropertyMetadata(false));

        public static RadProperty IsSortedProperty = RadProperty.Register("IsSorted",
              typeof(bool), typeof(DetailListViewCellElement), new RadElementPropertyMetadata(false));

        #endregion

        #region Fields

        protected ListViewDetailColumn column;
       
        #endregion

        #region Constructors

        static DetailListViewCellElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ListViewCellStateManagerFactory(), typeof(DetailListViewCellElement));
        }

        public DetailListViewCellElement(ListViewDetailColumn column)
        {
            this.column = column;
        }

        #endregion

        #region IVirtualizedElement members

        public virtual ListViewDetailColumn Data
        {
            get
            {
                return this.column;
            }
        }

        public virtual void Attach(ListViewDetailColumn data, object context)
        {
            if (data == null)
            {
                return;
            }

            this.column = data;
            this.Synchronize();
            this.BindProperty(DetailListViewCellElement.CurrentProperty, column, ListViewDetailColumn.CurrentProperty, PropertyBindingOptions.OneWay);
            this.column.RadPropertyChanged += new RadPropertyChangedEventHandler(OnColumnRadPropertyChanged);
        }

        public virtual void Detach()
        {
            if (this.column != null)
            {
                this.column.RadPropertyChanged -= new RadPropertyChangedEventHandler(OnColumnRadPropertyChanged);
            }

            this.column = null;
            this.UnbindProperty(DetailListViewCellElement.CurrentProperty);
        }

        public virtual void Synchronize()
        {
            this.Text = column.HeaderText;
            this.SetValue(CurrentProperty, column.Current);
            
            int descriptorIndex = column.Owner.SortDescriptors.IndexOf(column.Name);
            if (descriptorIndex >= 0)
            {
                this.SetValue(IsSortedProperty, true);
            }
            else
            {
                this.SetValue(IsSortedProperty, false);
            }

            this.column.Owner.OnCellFormatting(new ListViewCellFormattingEventArgs(this));
        }

        public virtual bool IsCompatible(ListViewDetailColumn data, object context)
        {
            return true;
        }
 
        #endregion

        #region Overrides
         
        protected override SizeF MeasureOverride(System.Drawing.SizeF availableSize)
        {
            SizeF desiredSize = base.MeasureOverride(availableSize);
            return new SizeF(this.Data.Width, desiredSize.Height);
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.AllowDrag = true;
            this.AllowDrop = true;
            this.NotifyParentOnMouseInput = true;
            this.ImageLayout = System.Windows.Forms.ImageLayout.None;
        }

        #endregion
         
        protected virtual void OnColumnRadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
        {

        }
        
    }
}