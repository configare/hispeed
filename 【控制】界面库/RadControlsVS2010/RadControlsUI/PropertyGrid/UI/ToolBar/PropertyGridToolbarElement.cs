using System;
using System.Windows.Forms;
using Telerik.WinControls.Data;
using System.ComponentModel;
using System.Drawing;
using Telerik.WinControls.Enumerations;

namespace Telerik.WinControls.UI
{
    public class PropertyGridToolbarElement : StackLayoutElement
    {
        #region Fields

        private ToolbarTextBoxElement searchTextBoxElement;
        private FilterDescriptor searchCriteria;
        private AlphabeticalToggleButton alphabeticalPropertySort;
        private CategorizedAlphabeticalToggleButton categorizedAlphabeticalPropertySort;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the parent <see cref="PropertyGridElement"/>.
        /// </summary>
        public PropertyGridElement PropertyGridElement
        {
            get
            {
                return this.FindAncestor<PropertyGridElement>();
            }
        }

        /// <summary>
        /// Gets the <see cref="RadToggleButtonElement"/> that enables CategorizedAlphabetical view in the <see cref="RadPropertyGrid"/>
        /// </summary>
        public CategorizedAlphabeticalToggleButton CategorizedAlphabeticalToggleButton 
        {
            get
            {
                return this.categorizedAlphabeticalPropertySort;
            }
        }

        /// <summary>
        /// Gets the <see cref="RadToggleButtonElement"/> that enables Alphabetical view in the <see cref="RadPropertyGrid"/>
        /// </summary>
        public AlphabeticalToggleButton AlphabeticalToggleButton
        {
            get
            {
                return this.alphabeticalPropertySort;
            }
        }

        /// <summary>
        /// Gets the <see cref="RadTextBoxElement"/>.
        /// </summary>
        public ToolbarTextBoxElement SearchTextBoxElement
        {
            get
            {
                return this.searchTextBoxElement;
            }
        }

        /// <summary>
        /// Gets or sets the property name by which the search will be performed.
        /// </summary>
        [Description("Gets or sets the property name by which the search will be performed."),
        Category(RadDesignCategory.DataCategory),
        Browsable(true), DefaultValue("Name")]
        public string FilterPropertyName
        {
            get
            {
                return this.searchCriteria.PropertyName;
            }
            set
            {
                this.searchCriteria.PropertyName = value;
                this.ExecuteSearch();
            }
        }

        /// <summary>
        /// Gets or sets the filter operator which will be used for the search.
        /// </summary>
        [Description("Gets or sets the filter operator which will be used for the search."),
        Category(RadDesignCategory.DataCategory),
        Browsable(true), DefaultValue(FilterOperator.Contains)]
        public FilterOperator FilterOperator
        {
            get
            {
                return this.searchCriteria.Operator;
            }
            set
            {
                this.searchCriteria.Operator = value;
                this.ExecuteSearch();
            }
        }

        /// <summary>
        /// Gets or sets the value by which the search will be performed.
        /// </summary>
        [Description("Gets or sets the value by which the search will be performed."),
        Category(RadDesignCategory.DataCategory),
        Browsable(true), DefaultValue("")]
        public object FilterValue
        {
            get
            {
                return this.searchCriteria.Value;
            }
            set
            {
                this.searchCriteria.Value = value;
                this.ExecuteSearch();
            }
        }
        
        #endregion

        #region Initialization

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.StretchHorizontally = true;
            this.Orientation = Orientation.Horizontal;
            this.FitInAvailableSize = true;
            this.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.MaxSize = new Size(0, 25);
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.searchTextBoxElement = new ToolbarTextBoxElement();
            this.alphabeticalPropertySort = new AlphabeticalToggleButton();
            this.categorizedAlphabeticalPropertySort = new CategorizedAlphabeticalToggleButton();

            this.alphabeticalPropertySort.MaxSize = new Size(25, 0);
            this.categorizedAlphabeticalPropertySort.MaxSize = new Size(25, 0);

            this.alphabeticalPropertySort.MinSize = new Size(25, 0);
            this.categorizedAlphabeticalPropertySort.MinSize = new Size(25, 0);

            this.searchTextBoxElement.StretchHorizontally = true;
            this.alphabeticalPropertySort.StretchHorizontally = false;
            this.categorizedAlphabeticalPropertySort.StretchHorizontally = false;

            this.alphabeticalPropertySort.Text = String.Empty;
            this.categorizedAlphabeticalPropertySort.Text = String.Empty;
            
            this.Children.Add(this.alphabeticalPropertySort);
            this.Children.Add(this.categorizedAlphabeticalPropertySort);
            this.Children.Add(this.searchTextBoxElement);
            this.searchCriteria = new FilterDescriptor("Name", FilterOperator.Contains, "");

            this.WireEvents();
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Executes a search with the current state of the filter.
        /// </summary>
        protected virtual void ExecuteSearch()
        {
            this.PropertyGridElement.PropertyTableElement.FilterDescriptors.Clear();
            this.PropertyGridElement.PropertyTableElement.FilterDescriptors.Add(this.searchCriteria);
        }

        protected virtual void WireEvents()
        {
            this.searchTextBoxElement.TextChanged += new EventHandler(searchTextBoxElement_TextChanged);
            this.alphabeticalPropertySort.ToggleStateChanging += new StateChangingEventHandler(alphabeticalPropertySort_ToggleStateChanging);
            this.categorizedAlphabeticalPropertySort.ToggleStateChanging += new StateChangingEventHandler(categorizedAlphabeticalPropertySort_ToggleStateChanging);
        }
        
        protected virtual void UnwireEvents()
        {
            this.searchTextBoxElement.TextChanged -= searchTextBoxElement_TextChanged;
            this.alphabeticalPropertySort.ToggleStateChanging -= alphabeticalPropertySort_ToggleStateChanging;
            this.categorizedAlphabeticalPropertySort.ToggleStateChanging -= categorizedAlphabeticalPropertySort_ToggleStateChanging;
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            this.UnwireEvents();
        }

        /// <summary>
        /// Syncronizes the default toggle buttons in the <see cref="PropertyGridToolbarElement"/> 
        /// with the PropertySort property of the <see cref="PropertyGridElement"/>.
        /// </summary>
        public virtual void SyncronizeToggleButtons()
        {
            PropertyGridElement propertyGridElement = this.PropertyGridElement;
            if (propertyGridElement != null)
            {
                PropertySort initialPropertySort = propertyGridElement.PropertyTableElement.PropertySort;

                this.UnwireEvents();

                switch (initialPropertySort)
                {
                    case PropertySort.Alphabetical:
                        this.alphabeticalPropertySort.ToggleState = ToggleState.On;
                        this.categorizedAlphabeticalPropertySort.ToggleState = ToggleState.Off;
                        break;
                    case PropertySort.Categorized:
                        this.alphabeticalPropertySort.ToggleState = ToggleState.Off;
                        this.categorizedAlphabeticalPropertySort.ToggleState = ToggleState.Off;
                        break;
                    case PropertySort.CategorizedAlphabetical:
                        this.alphabeticalPropertySort.ToggleState = ToggleState.Off;
                        this.categorizedAlphabeticalPropertySort.ToggleState = ToggleState.On;
                        break;
                    case PropertySort.NoSort:
                        this.alphabeticalPropertySort.ToggleState = ToggleState.Off;
                        this.categorizedAlphabeticalPropertySort.ToggleState = ToggleState.Off;
                        break;
                }

                this.WireEvents();
            }
        }

        #endregion

        #region Event handlers

        private void searchTextBoxElement_TextChanged(object sender, EventArgs e)
        {
            this.searchCriteria.Value = this.searchTextBoxElement.Text;
            this.ExecuteSearch();
            this.PropertyGridElement.PropertyTableElement.Update(PropertyGridTableElement.UpdateActions.ExpandedChanged);
        }

        void categorizedAlphabeticalPropertySort_ToggleStateChanging(object sender, StateChangingEventArgs args)
        {
            this.PropertyGridElement.PropertyTableElement.EndEdit();
            if (args.NewValue == ToggleState.On)
            {
                this.alphabeticalPropertySort.ToggleStateChanging -= alphabeticalPropertySort_ToggleStateChanging;
                this.alphabeticalPropertySort.ToggleState = ToggleState.Off;
                this.alphabeticalPropertySort.ToggleStateChanging += alphabeticalPropertySort_ToggleStateChanging;

                this.PropertyGridElement.PropertyTableElement.PropertySort = PropertySort.CategorizedAlphabetical;
            }
            else
            {
                args.Cancel = true;
            }
        }

        void alphabeticalPropertySort_ToggleStateChanging(object sender, StateChangingEventArgs args)
        {
            this.PropertyGridElement.PropertyTableElement.EndEdit();
            if (args.NewValue == ToggleState.On)
            {
                this.categorizedAlphabeticalPropertySort.ToggleStateChanging -= categorizedAlphabeticalPropertySort_ToggleStateChanging;
                this.categorizedAlphabeticalPropertySort.ToggleState = ToggleState.Off;
                this.categorizedAlphabeticalPropertySort.ToggleStateChanging += categorizedAlphabeticalPropertySort_ToggleStateChanging;

                this.PropertyGridElement.PropertyTableElement.PropertySort = PropertySort.Alphabetical;
            }
            else
            {
                args.Cancel = true;
            }
        }

        #endregion
    }
}
