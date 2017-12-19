using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents data item for the list of strips in the customize dialog of the <c ref="RadCommandBar"/>.
    /// </summary>
    public class CommandBarStripsListDataItem : RadListDataItem
    {
        #region RadProperties

        public static readonly RadProperty VisibleProperty = RadProperty.Register("Visible", typeof(bool), typeof(CommandBarStripsListDataItem), new RadElementPropertyMetadata(true));
        public static readonly RadProperty NameProperty = RadProperty.Register("Name", typeof(string), typeof(CommandBarStripsListDataItem), new RadElementPropertyMetadata(""));

        #endregion

        #region Properties

        public bool Visible
        {
            get
            {
                return (bool)this.GetValue(CommandBarStripsListDataItem.VisibleProperty);
            }
            set
            {
                this.SetValue(CommandBarStripsListDataItem.VisibleProperty, value);
            }
        }

        public string Name
        {
            get
            {
                return (string)this.GetValue(CommandBarStripsListDataItem.NameProperty);
            }
            set
            {
                this.SetValue(CommandBarStripsListDataItem.NameProperty, value);
            }
        }

        #endregion

        #region Overrides

        protected internal override void SetDataBoundItem(bool dataBinding, object value)
        {
            base.SetDataBoundItem(dataBinding, value);
            CommandBarStripElement stripElement = value as CommandBarStripElement;
            if (value == null)
            {
                return;
            }

            this.Name = stripElement.DisplayName;
            this.Visible = stripElement.VisibleInCommandBar;

            stripElement.RadPropertyChanged += new RadPropertyChangedEventHandler(stripElement_RadPropertyChanged);
        }


        #endregion

        #region Private Methods

        void stripElement_RadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
        {
            if (e.Property == CommandBarStripElement.VisibleInCommandBarProperty)
            {
                CommandBarStripElement strip = (this.DataBoundItem as CommandBarStripElement);
                if (strip != null)
                {
                    this.Visible = strip.VisibleInCommandBar;
                }
            }

            if (e.Property == RadElement.NameProperty)
            {
                CommandBarStripElement strip = (this.DataBoundItem as CommandBarStripElement);
                if (strip != null)
                {
                    this.Name = strip.DisplayName;
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents visual item for the list of strips in the customize dialog of the <c ref="RadCommandBar"/>.
    /// </summary>
    public class CommandBarStripsListVisualItem : RadListVisualItem
    {
        #region   Fields
        private RadLabelElement label = new RadLabelElement();
        private RadCheckBoxElement checkBox = new RadCheckBoxElement();
        #endregion

        #region Initialization

        static CommandBarStripsListVisualItem()
        {
            RadListVisualItem.SynchronizationProperties.Add(CommandBarStripsListDataItem.VisibleProperty);
            RadListVisualItem.SynchronizationProperties.Add(CommandBarStripsListDataItem.NameProperty);
        }

        #endregion

        #region Overrides

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RadListVisualItem);
            }
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.checkBox.ToggleStateChanged += this.ToggleStateChanged;
            this.label.StretchHorizontally = true;
            StackLayoutPanel stack = new StackLayoutPanel();
            stack.Orientation = Orientation.Horizontal;
            stack.Children.Add(checkBox);
            stack.Children.Add(label);
            this.Children.Add(stack);
        }

        protected override void PropertySynchronized(RadProperty property)
        {
            CommandBarStripsListDataItem dataItem = (CommandBarStripsListDataItem)this.Data;
            if (property == CommandBarStripsListDataItem.VisibleProperty || property == CommandBarStripsListDataItem.NameProperty)
            {
                this.checkBox.Checked = dataItem.Visible;
                this.label.Text = dataItem.Name;
            }

            this.Text = "";
        }

        #endregion

        #region Private Methods

        private void ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            CommandBarStripElement boundItem = (this.Data.DataBoundItem as CommandBarStripElement);
            if (boundItem == null)
            {
                return;
            }

            boundItem.VisibleInCommandBar = this.checkBox.Checked;
        }

        #endregion
    }

    /// <summary>
    /// Represents data item for the list of strip items in the customize dialog of the <c ref="RadCommandBar"/>.
    /// </summary>
    public class CommandBarItemsListDataItem : RadListDataItem
    {
        #region RadProperties

        public static readonly RadProperty VisibleProperty = RadProperty.Register("Visible", typeof(bool), typeof(CommandBarItemsListDataItem), new RadElementPropertyMetadata(true));
        public static readonly RadProperty NameProperty = RadProperty.Register("Name", typeof(string), typeof(CommandBarItemsListDataItem), new RadElementPropertyMetadata(""));

        #endregion

        #region Properties

        public bool Visible
        {
            get
            {
                return (bool)this.GetValue(CommandBarItemsListDataItem.VisibleProperty);
            }
            set
            {
                this.SetValue(CommandBarItemsListDataItem.VisibleProperty, value);
            }
        }

        public string Name
        {
            get
            {
                return (string)this.GetValue(CommandBarItemsListDataItem.NameProperty);
            }
            set
            {
                this.SetValue(CommandBarItemsListDataItem.NameProperty, value);
            }
        }

        #endregion

        #region Overrides

        protected internal override void SetDataBoundItem(bool dataBinding, object value)
        {
            base.SetDataBoundItem(dataBinding, value);
            this.Name = (value as RadCommandBarBaseItem).DisplayName;
            this.Visible = (value as RadCommandBarBaseItem).VisibleInStrip;

            if (value is RadElement)
            {
                (value as RadElement).RadPropertyChanged += new RadPropertyChangedEventHandler(ItemsListDataItem_RadPropertyChanged);
            }
        }


        #endregion

        #region Private Methods

        void ItemsListDataItem_RadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
        {
            if (e.Property == RadCommandBarBaseItem.VisibleInStripProperty)
            {
                this.Visible = (this.DataBoundItem as RadCommandBarBaseItem).VisibleInStrip;
            }

            if (e.Property == RadElement.NameProperty)
            {
                this.Name = (this.DataBoundItem as RadCommandBarBaseItem).DisplayName;
            }

        }

        #endregion
    }

    /// <summary>
    /// Represents visual item for the list of strip items in the customize dialog of the <c ref="RadCommandBar"/>.
    /// </summary>
    public class CommandBarItemsListVisualItem : RadListVisualItem
    {
        #region Fields
        private RadLabelElement label = new RadLabelElement();
        private RadCheckBoxElement checkBox = new RadCheckBoxElement();
        #endregion

        #region Initialization

        static CommandBarItemsListVisualItem()
        {
            RadListVisualItem.SynchronizationProperties.Add(CommandBarItemsListDataItem.VisibleProperty);
            RadListVisualItem.SynchronizationProperties.Add(CommandBarItemsListDataItem.NameProperty);
        }

        #endregion

        #region Overrides

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RadListVisualItem);
            }
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.checkBox.ToggleStateChanged += this.ToggleStateChanged;
            this.label.StretchHorizontally = true;
            StackLayoutPanel stack = new StackLayoutPanel();
            stack.Orientation = Orientation.Horizontal;
            stack.Children.Add(checkBox);
            stack.Children.Add(label);
            this.Children.Add(stack);
        }

        protected override void PropertySynchronized(RadProperty property)
        {
            RadCommandBarBaseItem boundItem = this.Data.DataBoundItem as RadCommandBarBaseItem;
            if (boundItem == null)
            {
                return;
            }

            if (property == CommandBarItemsListDataItem.VisibleProperty || property == CommandBarItemsListDataItem.NameProperty)
            {
                this.checkBox.Checked = boundItem.VisibleInStrip;
                this.label.Text = boundItem.DisplayName;
            }

            this.Text = "";
        }

        #endregion

        #region Private Methods

        private void ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            RadCommandBarBaseItem boundItem = (this.Data.DataBoundItem as RadCommandBarBaseItem);

            if (boundItem != null)
            {
                boundItem.VisibleInStrip = this.checkBox.Checked;
            }
        }

        #endregion
    }

}
