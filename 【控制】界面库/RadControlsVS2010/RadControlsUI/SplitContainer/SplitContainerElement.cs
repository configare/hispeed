using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Design;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class SplitContainerElement : RadItem
    {
        #region Initialization

        static SplitContainerElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new SplitContainerElementStateManager(), typeof(SplitContainerElement));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.ShouldHandleMouseInput = false;
        }

        #endregion

        #region Rad Properties

        public static RadProperty IsVerticalProperty = RadProperty.Register(
            "IsVertical", typeof(bool),
            typeof(SplitContainerElement),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.None));

        public static RadProperty SplitterWidthProperty = RadProperty.Register(
            "SplitterWidth", 
            typeof(int), 
            typeof(SplitContainerElement), 
            new RadElementPropertyMetadata(3, ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region CLR Properties

        internal SplitterElement this[int index]
        {
            get
            {
                return (SplitterElement)this.Children[index];
            }
        }

        internal int Count
        {
            get
            {
                return this.Children.Count;
            }
        }

        /// <summary>
        /// Gets or sets the width of each splitter within the container.
        /// </summary>
        [Description("Gets or sets the width of each splitter within the container.")]
        [RadPropertyDefaultValue("SplitterWidth", typeof(SplitContainerElement))]
        public int SplitterWidth
        {
            get
            {
                return (int)this.GetValue(SplitterWidthProperty);
            }
            set
            {
                value = Math.Max(0, value);
                this.SetValue(SplitterWidthProperty, value);
            }
        }

        #endregion

        #region Overrides

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == SplitterWidthProperty && this.ElementState == ElementState.Loaded)
            {
                RadSplitContainer container = this.ElementTree.Control as RadSplitContainer;
                if (container != null)
                {
                    container.ApplySplitterWidth((int)e.NewValue);
                }
            }
        }

        #endregion
    }
}
