using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a ribbon bar button group. You can group buttons that are
    /// logically related, for example, bold, italic, and underline buttons in 
    /// a text editor application.
    /// </summary>
    [ToolboxItem(false), ComVisible(false)]
    [Designer(DesignerConsts.RadRibbonBarButtonGroupDesignerString)]
    public class RadRibbonBarButtonGroup : CollapsibleElement, IItemsElement
    {
        private BorderPrimitive borderPrimitive;
        private RadItemOwnerCollection items;
        private StackLayoutPanel layoutPanel;
        private FillPrimitive fillPrimitive;

        public static RoutedEvent OnRoutedButtonSelected = RadTabStripElement.RegisterRoutedEvent("OnRoutedButtonSelected", typeof(RadRibbonBarButtonGroup));
        public static RoutedEvent OnRoutedButtonDeselected = RadTabStripElement.RegisterRoutedEvent("OnRoutedButtonDeselected", typeof(RadRibbonBarButtonGroup));


        public static RadProperty IsSelectedProperty = RadProperty.Register(
     "IsSelected", typeof(bool), typeof(RadRibbonBarButtonGroup), new RadElementPropertyMetadata(
         false, ElementPropertyOptions.None));

        public static RadProperty OrientationProperty = RadProperty.Register(
            "Orientation", typeof(Orientation), typeof(RadRibbonBarButtonGroup),
            new RadElementPropertyMetadata(
                Orientation.Horizontal,
                ElementPropertyOptions.AffectsArrange
                | ElementPropertyOptions.AffectsDisplay
                | ElementPropertyOptions.AffectsLayout
                | ElementPropertyOptions.AffectsMeasure
                | ElementPropertyOptions.AffectsParentArrange
                | ElementPropertyOptions.AffectsParentMeasure
                | ElementPropertyOptions.AffectsTheme));

        internal static RadProperty InternalOrientationProperty = RadProperty.Register(
            "InternalOrientation", typeof(Orientation), typeof(RadRibbonBarButtonGroup),
            new RadElementPropertyMetadata(
                Orientation.Horizontal,
                ElementPropertyOptions.CanInheritValue));

        //This property is attached to the items put in this button group.
        internal static RadProperty IsItemAtEndIndexProperty = RadProperty.Register(
            "IsItemAtEndIndex", typeof(bool), typeof(RadRibbonBarButtonGroup),
            new RadElementPropertyMetadata(
                true,
                ElementPropertyOptions.CanInheritValue |
                ElementPropertyOptions.AffectsDisplay |
                ElementPropertyOptions.AffectsLayout));


        /// <summary>Gets the collection of items in the button group.</summary>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [Description("Gets the collection of items in the button group.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor))]
        //This is a dummy attribute. It serves to enable the Add an item..
        //option in the context menu of the group when in design mode
        [RadNewItem("", false, false, false)]
        public RadItemOwnerCollection Items
        {
            get
            {
                return this.items;
            }
        }

        /// <summary>Gets or sets the orientation of the elements inside the button group: Horizontal or Vertical.</summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets the orientation of the elements inside the button group: Horizontal or Vertical.")]
        [DefaultValue(Orientation.Horizontal)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(RadRibbonBarButtonGroup.OrientationProperty);
            }
            set
            {
                this.SetValue(RadRibbonBarButtonGroup.OrientationProperty, value);
            }
        }

        /// <summary>Gets or sets a value indicating whether the border is shown.</summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a value indicating whether the border is shown.")]
        [DefaultValue(true)]
        public bool ShowBorder
        {
            get
            {
                return (this.borderPrimitive.Visibility == ElementVisibility.Visible);
            }
            set
            {
                this.borderPrimitive.Visibility = value ? ElementVisibility.Visible : ElementVisibility.Collapsed;

                foreach (RadItem item in this.Items)
                {
                    if( item.GetType() == typeof(RadButtonElement))
//                    if (item is RadButtonElement)
                    {
                        SynchShowBorder((RadButtonElement)item);
                    }
                }
            }
        }

        /// <summary>Gets or sets a value indicating whether the back color is shown.</summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a value indicating whether the back color is shown.")]
        [DefaultValue(true)]
        public bool ShowBackColor
        {
            get
            {
                return (this.fillPrimitive.Visibility == ElementVisibility.Visible);
            }
            set
            {
                this.fillPrimitive.Visibility = value ? ElementVisibility.Visible : ElementVisibility.Collapsed;
            }
        }

        [DefaultValue(typeof(Size), "22, 22")]
        public override Size MinSize
        {
            get
            {
                return base.MinSize;
            }
            set
            {
                base.MinSize = value;
            }
        }

        /// <summary>
        /// Gets the stack layout panel
        /// that holds all elements.
        /// </summary>
        internal StackLayoutPanel LayoutPanel
        {
            get
            {
                return this.layoutPanel;
            }
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
            this.Shape = new RoundRectShape(3);
            this.StretchVertically = false;
        }

        protected override void CreateChildElements()
        {
            this.borderPrimitive = new BorderPrimitive();
            this.borderPrimitive.Class = "ButtonGroupBorder";

            this.fillPrimitive = new FillPrimitive();
            this.fillPrimitive.Class = "ButtonGroupFill";

            this.layoutPanel = new StackLayoutPanel();
            this.items = new RadItemOwnerCollection(this.layoutPanel);
            this.items.ItemsChanged += new ItemChangedDelegate(ItemChanged);
            this.items.ItemTypes = new Type[] {                
                typeof(RadButtonElement),
                typeof(RadCheckBoxElement),
                typeof(RadComboBoxElement),
                typeof(RadDropDownButtonElement),
                typeof(RadDropDownListElement),
                typeof(RadImageButtonElement),
                typeof(RadLabelElement),
                typeof(RadMaskedEditBoxElement),
                typeof(RadRadioButtonElement),
                typeof(RadRepeatButtonElement),
                typeof(RadRibbonBarButtonGroup),
                typeof(RadSplitButtonElement),
                typeof(RadTextBoxElement),
                typeof(RadToggleButtonElement),
                typeof(RibbonBarGroupSeparator)
            };

            this.layoutPanel.BindProperty(
                StackLayoutPanel.OrientationProperty,
                this,
                RadRibbonBarButtonGroup.OrientationProperty,
                PropertyBindingOptions.OneWay);

            this.Children.Add(this.fillPrimitive);
            this.Children.Add(this.layoutPanel);
            this.Children.Add(this.borderPrimitive);
        }
        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == RadRibbonBarButtonGroup.OrientationProperty)
            {
                this.SetDefaultValueOverride(RadRibbonBarButtonGroup.InternalOrientationProperty, e.NewValue);

                foreach (RadItem item in this.items)
                {
                    if (item is RibbonBarGroupSeparator)
                    {
                        item.SetDefaultValueOverride(RibbonBarGroupSeparator.OrientationProperty, e.NewValue);
                    }
                }
            }
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            if (this.items.Count > 0)
            {
                if (this.items[0] is RadButtonElement && this.Orientation == Orientation.Horizontal)
                {
                    (this.items[0] as RadButtonElement).BorderElement.SetDefaultValueOverride( BorderPrimitive.LeftWidthProperty,0f);
                }

                for (int i = 0; i < this.items.Count; i++)
                {
                    if (this.items[i] is RadButtonElement)
                    {
                        this.items[i].Shape = null;
                    }
                }
            }
        }

        protected virtual void ResetItemsIsAtUnevenIndexProperty()
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                if (items[i].GetType().Equals(typeof(RadButtonElement)))
                {
                    RadButtonElement buttonElement = this.items[i] as RadButtonElement;

                    if (buttonElement.StateManager.GetType() != typeof(ItemInButtonGroupStateManager))
                    {
                        buttonElement.StateManager = new ItemInButtonGroupStateManager().StateManagerInstance;
                    }

                    buttonElement.SetDefaultValueOverride(RadRibbonBarButtonGroup.IsItemAtEndIndexProperty, i + 1 == this.items.Count);
                }
            }
        }

        /// <summary>
        /// Fires ItemChanged event.
        ///</summary>
        public void ItemChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            if (operation == ItemsChangeOperation.Inserted || operation == ItemsChangeOperation.Removed)
            {
                this.ResetItemsIsAtUnevenIndexProperty();
            }

            this.invalidateCollapsableChildrenCollection = true;
            if (operation == ItemsChangeOperation.Inserted ||
                operation == ItemsChangeOperation.Set)
            {
                target.NotifyParentOnMouseInput = true;
                if (target.GetType() == typeof(RadButtonElement))
                {
                    RadButtonElement buttonElement = target as RadButtonElement;
                    buttonElement.ButtonFillElement.Class = "ButtonInRibbonFill";
                    buttonElement.ThemeRole = "ButtonInRibbonButtonGroup";
                    SynchShowBorder(buttonElement);
                }

                if (target.GetType() == typeof(RibbonBarGroupSeparator))
                {
                    target.SetDefaultValueOverride(RibbonBarGroupSeparator.OrientationProperty, this.Orientation);
                }

                if (target is RadRibbonBarButtonGroup)
                {
                    RadRibbonBarButtonGroup buttonGroup = (RadRibbonBarButtonGroup)target;
                    buttonGroup.SetDefaultValueOverride(RadElement.MinSizeProperty, new Size(22, 22));
                }
                if (target is RadComboBoxElement || target is RadDropDownListElement)
                {
                    target.SetDefaultValueOverride(RadElement.MinSizeProperty, new Size(140, 0));
                    target.SetDefaultValueOverride(RadElement.AutoSizeModeProperty, RadAutoSizeMode.WrapAroundChildren);
                }
                if (target is RadTextBoxElement)
                {
                    RadTextBoxElement textBoxElement = (RadTextBoxElement)target;
                    target.SetDefaultValueOverride(RadElement.MinSizeProperty, new Size(140, 0));
                    target.SetDefaultValueOverride(RadElement.AutoSizeModeProperty, RadAutoSizeMode.WrapAroundChildren);
                    //// The border size is more correct in without horizontal padding
                    Padding temp = textBoxElement.Padding;
                    temp.Left = 0;
                    temp.Right = 0;
                    textBoxElement.Padding = temp;
                    textBoxElement.SetDefaultValueOverride(RadElement.PaddingProperty, temp);
                }
                if (target is RadGalleryElement)
                {
                    target.SetDefaultValueOverride(RadElement.PaddingProperty, new Padding(2, 2, 0, 0));
                }
                if (target is RadSplitButtonElement)
                {
                    target.SetDefaultValueOverride(RadElement.StretchHorizontallyProperty, false);
                    target.Children[0].SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Hidden);
                }
                if (target is RadCheckBoxElement)
                {
                    target.SetDefaultValueOverride(RadElement.StretchVerticallyProperty, false);
                }
                if (target is RadRadioButtonElement)
                {
                    target.SetDefaultValueOverride(RadElement.StretchVerticallyProperty, false);
                    target.SetDefaultValueOverride(RadElement.AutoSizeModeProperty, RadAutoSizeMode.WrapAroundChildren);
                }
            }
        }

        private void SynchShowBorder(RadButtonElement target)
        {
            RadElement borderElement = target.BorderElement;
            if (this.ShowBorder)
            {
                target.Class = "ButtonElement";
                if (borderElement != null)
                    borderElement.Class = "ButtonInGroupBorder";
            }
            else
            {
                target.Class = "RibbonBarButtonElement";
                if (borderElement != null)
                    borderElement.Class = "ButtonInRibbonBorder";
            }
            target.ForceReApplyStyle();
        }

        /// <summary>
        /// Fires ItemClicked event.
        /// </summary>
        /// <param name="item"></param>
        public void ItemClicked(RadItem item)
        {
            if (this.GetIsSelected(item))
            {
                return;
            }

            if (!this.CanSelectButton(item))
            {
                return;
            }

            this.SetSelectedItem(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public void SetSelectedItem(RadElement element)
        {
            this.SetIsSelected(element, true);
            this.RefreshItems(element);
        }

        protected virtual bool CanSelectButton(RadItem item)
        {
            return true;
        }

        /// <summary>Refreshes the items nested in the argument.</summary>
        public virtual void RefreshItems(RadElement element)
        {
            foreach (RadElement child in this.Items)
            {
                if (child != element)
                    SetIsSelected(child, false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="childElement"></param>
        /// <param name="value"></param>
        public void SetIsSelected(RadElement childElement, bool value)
        {
            if (GetIsSelected(childElement) != value)
            {
                childElement.SetValue(RadTabStripElement.IsSelectedProperty, value);
                this.RaiseButtonSelected(childElement);
            }
        }

        /// <summary></summary>
        public bool GetIsSelected(RadElement childElement)
        {
            return (bool)childElement.GetValue(RadRibbonBarButtonGroup.IsSelectedProperty);
        }

        private void RaiseButtonSelected(RadElement button)
        {
            bool value = this.GetIsSelected(button);
            if (value)
            {
                RaiseRoutedEvent(new RoutedEventArgs(EventArgs.Empty, OnRoutedButtonSelected), button);
            }
            else
            {
                RaiseRoutedEvent(new RoutedEventArgs(EventArgs.Empty, OnRoutedButtonDeselected), button);
            }
        }

        private void RaiseRoutedEvent(RoutedEventArgs args, RadElement button)
        {
            button.RaiseTunnelEvent(button, args);
            if (!args.Canceled)
            {
                this.RaiseBubbleEvent(this, args);
            }
        }

        #region ICollapsibleElement Members

        override public bool ExpandElementToStep(int nextStep)
        {
            this.InvalidateIfNeeded();
            return this.ExpandCollection(nextStep);
        }

        override public bool CollapseElementToStep(int nextStep)
        {
            this.InvalidateIfNeeded();
            return this.CollapseCollection(nextStep);
        }

        public override bool CanExpandToStep(int step)
        {
            if (step > this.CollapseStep)
            {
                return false;
            }
            this.InvalidateIfNeeded();
            if (this.Orientation == Orientation.Vertical)
            {
                if (this.collapsableChildren.Count == 0)
                {
                    return false;
                }

                bool canExpandToNextStep = step > 0;

                foreach (CollapsibleElement element in this.collapsableChildren)
                {
                    canExpandToNextStep |= element.CanExpandToStep(step);
                }

                return canExpandToNextStep;
            }
            else
            {
                if (this.collapsableChildren.Count > 0)
                {
                    //CollapsibleElement lastCollapsibleItem = this.collapsableChildren[this.collapsableChildren.Count - 1];
                    //return lastCollapsibleItem.CanExpandToStep(step);
                    bool canCollapseToNextStep = step <= this.CollapseMaxSteps;

                    foreach (CollapsibleElement element in this.collapsableChildren)
                    {
                        canCollapseToNextStep |= element.CanCollapseToStep(step);
                    }
                    return canCollapseToNextStep;
                }
                return false;
            }
        }

        public override bool CanCollapseToStep(int step)
        {
            if (step < this.CollapseStep)
            {
                return false;
            }

            this.InvalidateIfNeeded();

            if (this.Orientation == Orientation.Vertical)
            {
                if (this.collapsableChildren.Count == 0)
                {
                    return false;
                }
                //int nextStep = this.collapsableChildren[0].CollapseStep + 1;
                bool canCollapseToNextStep = false;// step <= this.CollapseMaxSteps;

                foreach (CollapsibleElement element in this.collapsableChildren)
                {
                    canCollapseToNextStep |= element.CanCollapseToStep(step);
                }

                return canCollapseToNextStep;
            }
            else
            {
                if (this.collapsableChildren.Count > 0)
                {
                    //CollapsibleElement lastCollapsibleItem = this.collapsableChildren[this.collapsableChildren.Count - 1];
                    //return lastCollapsibleItem.CanCollapseToStep(step);
                    bool canCollapseToNextStep = false;// step <= this.CollapseMaxSteps;

                    foreach (CollapsibleElement element in this.collapsableChildren)
                    {
                        canCollapseToNextStep |= element.CanCollapseToStep(step);
                    }
                    return canCollapseToNextStep;
                }
                return false;
            }
        }

        public override int CollapseMaxSteps
        {
            get
            {
                return 3;
            }
        }
        #endregion
    }
}
