using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class BaseListViewVisualItem : LightVisualElement, IVirtualizedElement<ListViewDataItem>
    {
        #region RadProperties

        public static RadProperty HotTrackingProperty = RadProperty.Register(
            "HotTracking", typeof(bool), typeof(BaseListViewVisualItem), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty SelectedProperty = RadProperty.Register(
            "Selected", typeof(bool), typeof(BaseListViewVisualItem),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty CurrentProperty = RadProperty.Register(
            "Current", typeof(bool), typeof(BaseListViewVisualItem),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty IsControlInactiveProperty = RadProperty.Register(
            "IsControlInactive", typeof(bool), typeof(BaseListViewVisualItem),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        #endregion

        static BaseListViewVisualItem()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ListViewVisualItemStateManagerFactory(), typeof(BaseListViewVisualItem));
        }

        #region Fields

        protected ListViewDataItem dataItem;
        protected RadToggleButtonElement toggleElement;

        #endregion

        #region Overrides

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.toggleElement = new ListViewItemCheckbox();
            this.toggleElement.ToggleState = Enumerations.ToggleState.Off;
            this.toggleElement.NotifyParentOnMouseInput = false;
            this.toggleElement.ShouldHandleMouseInput = true;
            this.toggleElement.MinSize = new Size(16, 16);
            this.toggleElement.ToggleStateChanging += toggleButton_ToggleStateChanging;
            this.toggleElement.Class = "ListViewItemToggleButton";
            this.toggleElement.ThemeRole = "ListViewItemToggleButton";
            this.Children.Add(this.toggleElement);

            this.StretchHorizontally = true;
            this.StretchVertically = true;
            this.ImageLayout = System.Windows.Forms.ImageLayout.None;

            this.ImageAlignment = ListViewDataItemStyle.DefaultImageAlignment;
            this.TextAlignment = ListViewDataItemStyle.DefaultImageAlignment;
            this.TextImageRelation = ListViewDataItemStyle.DefaultTextImageRelation;

            this.DrawFill = true;
            this.NumberOfColors = 1;
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == RadElement.IsMouseOverElementProperty && 
                this.IsInValidState(true) && this.Data != null)
            {
                if (this.Data.Owner.HotTracking)
                {
                    this.SetValue(HotTrackingProperty, e.NewValue);
                }
                else
                {
                    this.SetValue(HotTrackingProperty, false);
                }
            }
        }

        #endregion

        #region EventHandlers

        void toggleButton_ToggleStateChanging(object sender, StateChangingEventArgs args)
        {
            args.Cancel = this.OnToggleButtonStateChanging(args);
        }

        #endregion

        #region IVirtualizedElement members

        public ListViewDataItem Data
        {
            get
            {
                return dataItem;
            }
        }

        public virtual void Attach(ListViewDataItem data, object context)
        {
            this.dataItem = data;

            Debug.Assert(this.dataItem != null, "Data item must never be null at this point.");

            this.WireItemEvents();
            this.Synchronize();
        }

        public virtual void Detach()
        {
            Debug.Assert(this.dataItem != null, "Data item must never be null at this point.");

            if (this.IsInEditMode)
            {
                this.Data.Owner.CancelEdit();
                this.RemoveEditor(this.editor);
            }

            this.UnwireItemEvents();

            this.dataItem = null;
        }

        public void Synchronize()
        {
            if (this.dataItem == null || this.dataItem.Owner == null)
            {
                return;
            }

            this.SynchronizeProperties();

            this.dataItem.Owner.OnVisualItemFormatting(this);
        }

        public virtual bool IsCompatible(ListViewDataItem data, object context)
        {
            if (!(data is ListViewDataItemGroup) && data.Owner.ViewType == ListViewType.ListView)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Helpers

        protected virtual void UnwireItemEvents()
        {
            this.dataItem.PropertyChanged -= this.DataPropertyChanged;
        }

        protected virtual void WireItemEvents()
        {
            this.dataItem.PropertyChanged += this.DataPropertyChanged;
        }

        protected virtual void SynchronizeProperties()
        {
            this.toggleElement.ToggleStateChanging -= toggleButton_ToggleStateChanging;

            this.Text = this.Data.Text;
            this.Image = this.Data.Image;
            this.toggleElement.ToggleState = this.Data.CheckState;
            this.Selected = this.Data.Selected;
            this.Current = this.Data.Current;
            this.Enabled = this.Data.Enabled; 

            this.SetValue(IsControlInactiveProperty, !this.Data.Owner.ElementTree.Control.Focused);

            if (this.Data.HasStyle)
            {
                if (this.Data.Font != ListViewDataItemStyle.DefaultFont && this.Data.Font != this.Font)
                {
                    this.Font = this.Data.Font;
                }

                if (this.Data.ForeColor != ListViewDataItemStyle.DefaultForeColor && this.Data.ForeColor != this.ForeColor)
                {
                    this.ForeColor = this.Data.ForeColor;
                }

                if (this.Data.BackColor != ListViewDataItemStyle.DefaultBackColor && this.Data.BackColor != this.BackColor)
                {
                    this.BackColor = this.Data.BackColor;
                }

                if (this.Data.BackColor2 != ListViewDataItemStyle.DefaultBackColor2 && this.Data.BackColor2 != this.BackColor2)
                {
                    this.BackColor2 = this.Data.BackColor2;
                }

                if (this.Data.BackColor3 != ListViewDataItemStyle.DefaultBackColor3 && this.Data.BackColor3 != this.BackColor3)
                {
                    this.BackColor3 = this.Data.BackColor3;
                }

                if (this.Data.BackColor4 != ListViewDataItemStyle.DefaultBackColor4 && this.Data.BackColor4 != this.BackColor4)
                {
                    this.BackColor4 = this.Data.BackColor4;
                }

                if (this.Data.BorderColor != ListViewDataItemStyle.DefaultBorderColor && this.Data.BorderColor != this.BorderColor)
                {
                    this.BorderColor = this.Data.BorderColor;
                }

                if (this.Data.NumberOfColors != ListViewDataItemStyle.DefaultNumberOfColors && this.Data.NumberOfColors != this.NumberOfColors)
                {
                    this.NumberOfColors = this.Data.NumberOfColors;
                }

                if (this.Data.GradientPercentage != ListViewDataItemStyle.DefaultGradientPercentage && this.Data.GradientPercentage != this.GradientPercentage)
                {
                    this.GradientPercentage = this.Data.GradientPercentage;
                }

                if (this.Data.GradientPercentage2 != ListViewDataItemStyle.DefaultGradientPercentage2 && this.Data.GradientPercentage2 != this.GradientPercentage2)
                {
                    this.GradientPercentage2 = this.Data.GradientPercentage2;
                }

                if (this.Data.GradientAngle != ListViewDataItemStyle.DefaultGradientAngle && this.Data.GradientAngle != this.GradientAngle)
                {
                    this.GradientAngle = this.Data.GradientAngle;
                }

                if (this.Data.GradientStyle != ListViewDataItemStyle.DefaultGradientStyle && this.Data.GradientStyle != this.GradientStyle)
                {
                    this.GradientStyle = this.Data.GradientStyle;
                }

                if (this.Data.TextAlignment != this.TextAlignment)
                {
                    this.TextAlignment = this.Data.TextAlignment;
                }

                if (this.Data.TextImageRelation != ListViewDataItemStyle.DefaultTextImageRelation && this.Data.TextImageRelation != this.TextImageRelation)
                {
                    this.TextImageRelation = this.Data.TextImageRelation;
                }

                if (this.Data.ImageAlignment != ListViewDataItemStyle.DefaultImageAlignment && this.Data.ImageAlignment != this.ImageAlignment)
                {
                    this.ImageAlignment = this.Data.ImageAlignment;
                }
            }
            else
            {
                if (this.GetValueSource(FontProperty) != ValueSource.Style)
                {
                    this.ResetValue(FontProperty);
                }

                if (this.GetValueSource(ForeColorProperty) != ValueSource.Style)
                {
                    this.ResetValue(ForeColorProperty);
                }

                if (this.GetValueSource(BackColorProperty) != ValueSource.Style)
                {
                    this.ResetValue(BackColorProperty);
                }

                if (this.GetValueSource(BackColor2Property) != ValueSource.Style)
                {
                    this.ResetValue(BackColor2Property);
                }

                if (this.GetValueSource(BackColor3Property) != ValueSource.Style)
                {
                    this.ResetValue(BackColor3Property);
                }

                if (this.GetValueSource(BackColor4Property) != ValueSource.Style)
                {
                    this.ResetValue(BackColor4Property);
                }

                if (this.GetValueSource(BorderColorProperty) != ValueSource.Style)
                {
                    this.ResetValue(BorderColorProperty);
                }

                if (this.GetValueSource(NumberOfColorsProperty) != ValueSource.Style)
                {
                    this.ResetValue(NumberOfColorsProperty);
                }

                if (this.GetValueSource(GradientPercentageProperty) != ValueSource.Style)
                {
                    this.ResetValue(GradientPercentageProperty);
                }

                if (this.GetValueSource(GradientPercentage2Property) != ValueSource.Style)
                {
                    this.ResetValue(GradientPercentage2Property);
                }

                if (this.GetValueSource(GradientAngleProperty) != ValueSource.Style)
                {
                    this.ResetValue(GradientAngleProperty);
                }

                if (this.GetValueSource(GradientStyleProperty) != ValueSource.Style)
                {
                    this.ResetValue(GradientStyleProperty);
                }

                if (this.GetValueSource(TextAlignmentProperty) != ValueSource.Style)
                {
                    this.ResetValue(TextAlignmentProperty);
                    this.SetDefaultValueOverride(TextAlignmentProperty, ListViewDataItemStyle.DefaultTextAlignment);
                }

                if (this.GetValueSource(TextImageRelationProperty) != ValueSource.Style)
                {
                    this.ResetValue(TextImageRelationProperty);
                    this.SetDefaultValueOverride(TextImageRelationProperty, ListViewDataItemStyle.DefaultTextImageRelation);
                }

                if (this.GetValueSource(ImageAlignmentProperty) != ValueSource.Style)
                {
                    this.ResetValue(ImageAlignmentProperty);
                    this.SetDefaultValueOverride(ImageAlignmentProperty, ListViewDataItemStyle.DefaultImageAlignment);
                }
            }

            this.toggleElement.ToggleStateChanging += toggleButton_ToggleStateChanging;
        }

        #endregion

        #region Virtual methods

        protected virtual bool OnToggleButtonStateChanging(StateChangingEventArgs args)
        {
            if (this.IsInValidState(true))
            {
                this.dataItem.CheckState = args.NewValue;
                args.Cancel = this.dataItem.CheckState != args.NewValue;
            }

            return args.Cancel;
        }

        protected virtual void DataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsDesignMode || e.PropertyName == "ActualSize")
            {
                return;
            }

            if (this.dataItem == null || this.dataItem.Owner == null)
            {
                return;
            }

            this.SuspendLayout();

            this.SynchronizeProperties();

            this.dataItem.Owner.OnVisualItemFormatting(this);

            this.ResumeLayout(true);
        }

        #endregion

        #region Properties

        public bool IsControlActive
        {
            get
            {
                return !(bool)this.GetValue(IsControlInactiveProperty);
            }
        }

        public RadToggleButtonElement ToggleElement
        {
            get
            {
                return toggleElement;
            }
        }

        [Browsable(false)]
        public bool Selected
        {
            get
            {
                return (bool)this.GetValue(SelectedProperty);
            }
            internal set
            {
                this.SetValue(SelectedProperty, value);
            }
        }

        [Browsable(false)]
        public bool Current
        {
            get
            {
                return (bool)this.GetValue(CurrentProperty);
            }
            internal set
            {
                this.SetValue(CurrentProperty, value);
            }
        }

        #endregion

        #region Layout

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            base.ArrangeOverride(finalSize);
            RectangleF clientRect = GetClientRectangle(finalSize); 

            ArrangeContentCore(clientRect);
            
            return finalSize;
        }
  
        protected virtual void ArrangeContentCore(RectangleF clientRect)
        {
            if (this.toggleElement.Visibility != ElementVisibility.Collapsed)
            {
                this.toggleElement.Arrange(new RectangleF(new PointF(clientRect.X, clientRect.Y + (clientRect.Height - this.toggleElement.DesiredSize.Height) / 2), this.toggleElement.DesiredSize));
            }
               
            this.layoutManagerPart.Arrange(new RectangleF(
                clientRect.X + this.toggleElement.DesiredSize.Width, clientRect.Y,
                clientRect.Width - this.toggleElement.DesiredSize.Width, clientRect.Height));

            RadItem editorElement = this.GetEditorElement(editor);

            if (IsInEditMode && editorElement != null)
            {
                editorElement.Arrange(this.GetEditorArrangeRectangle(clientRect));
            }
        }

        protected virtual RectangleF GetEditorArrangeRectangle(RectangleF clientRect)
        {
            RectangleF rect = new RectangleF(clientRect.X + this.toggleElement.DesiredSize.Width, clientRect.Y, 
                clientRect.Width - this.toggleElement.DesiredSize.Width, clientRect.Height);

            if (rect.Width > this.Data.Owner.ViewElement.ViewElement.DesiredSize.Width)
            {
                rect.Width = this.Data.Owner.ViewElement.ViewElement.DesiredSize.Width - clientRect.X;
            }
            return rect;
        }

        protected override SizeF MeasureElements(SizeF availableSize, SizeF clientSize, Padding borderThickness)
        {
            SizeF desiredSize = SizeF.Empty;

            if (this.AutoSize)
            {
                foreach (RadElement child in this.Children)
                {
                    if (child == this.GetEditorElement(this.editor))
                    {
                        continue;
                    }

                    SizeF childDesiredSize = SizeF.Empty;

                    if (child.FitToSizeMode == RadFitToSizeMode.FitToParentBounds || BypassLayoutPolicies)
                    {
                        child.Measure(availableSize);
                        childDesiredSize = child.DesiredSize;
                    }
                    else if (child.FitToSizeMode == RadFitToSizeMode.FitToParentPadding)
                    {
                        child.Measure(new SizeF(clientSize.Width - borderThickness.Horizontal, clientSize.Height - borderThickness.Vertical));
                        childDesiredSize.Width = child.DesiredSize.Width + borderThickness.Horizontal;
                        childDesiredSize.Height = childDesiredSize.Height + borderThickness.Vertical;
                    }
                    else
                    {
                        child.Measure(clientSize);
                        childDesiredSize.Width += child.DesiredSize.Width + Padding.Horizontal + borderThickness.Horizontal;
                        childDesiredSize.Height += child.DesiredSize.Height + Padding.Vertical + borderThickness.Vertical;
                    }

                    desiredSize.Width = Math.Max(desiredSize.Width, childDesiredSize.Width);
                    desiredSize.Height = Math.Max(desiredSize.Height, childDesiredSize.Height);
                }
            }
            else
            {
                foreach (RadElement child in this.Children)
                {
                    child.Measure(availableSize);
                }
                desiredSize = this.Size;
            }

            return desiredSize;
        }

        #endregion

        #region Editing

        protected IInputEditor editor;

        public bool IsInEditMode
        {
            get
            {
                return this.editor != null;
            }
        }

        public IInputEditor Editor
        {
            get
            {
                return this.editor;
            }
        }

        public virtual void AddEditor(IInputEditor editor)
        {
            if (editor != null && this.editor != editor)
            {
                this.editor = editor;
                RadItem element = GetEditorElement(editor);

                if (element != null && !this.Children.Contains(element))
                {
                    this.Children.Add(element);
                    this.Data.Owner.ViewElement.ViewElement.UpdateItems();
                    this.UpdateLayout();
                }
            }
        }
        
        public virtual void RemoveEditor(IInputEditor editor)
        {
            if (editor != null && this.editor == editor)
            {
                RadItem element = GetEditorElement(editor);

                if (element != null && this.Children.Contains(element))
                {
                    this.Children.Remove(element);
                }

                this.editor = null;
                this.Synchronize();
            }
        }

        protected RadItem GetEditorElement(IValueEditor editor)
        {
            BaseInputEditor baseInputEditor = this.editor as BaseInputEditor;
            if (baseInputEditor != null)
            {
                return baseInputEditor.EditorElement as RadItem;
            }
            return editor as RadItem;
        }

        #endregion

        #region Events Management

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (this.Data != null && this.Data.Owner != null)
            {
                this.Data.Owner.OnItemMouseDown(new ListViewItemMouseEventArgs(this.Data, e));
            }
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (this.Data != null && this.Data.Owner != null)
            {
                this.Data.Owner.OnItemMouseUp(new ListViewItemMouseEventArgs(this.Data, e));
            }
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.Data != null && this.Data.Owner != null)
            {
                this.Data.Owner.OnItemMouseMove(new ListViewItemMouseEventArgs(this.Data, e));
            }
        }

        protected override void OnMouseHover(System.EventArgs e)
        {
            base.OnMouseHover(e);
            if (this.Data != null && this.Data.Owner != null)
            {
                this.Data.Owner.OnItemMouseHover(new ListViewItemEventArgs(this.Data));
            }
        }

        protected override void OnMouseEnter(System.EventArgs e)
        {
            base.OnMouseEnter(e);
            if (this.Data != null && this.Data.Owner != null)
            {
                this.Data.Owner.OnItemMouseEnter(new ListViewItemEventArgs(this.Data));
            }
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            base.OnMouseLeave(e);
            if (this.Data != null && this.Data.Owner != null)
            {
                this.Data.Owner.OnItemMouseLeave(new ListViewItemEventArgs(this.Data));
            }
        }

        protected override void OnClick(System.EventArgs e)
        {
            base.OnClick(e);
            if (this.Data != null && this.Data.Owner != null)
            {
                this.Data.Owner.OnItemMouseClick(new ListViewItemEventArgs(this.Data));
            }
        }

        protected override void OnDoubleClick(System.EventArgs e)
        {
            base.OnDoubleClick(e);
            if (this.Data != null && this.Data.Owner != null)
            {
                this.Data.Owner.OnItemMouseDoubleClick(new ListViewItemEventArgs(this.Data));
            }
        }
       
        #endregion
    }
}