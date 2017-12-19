using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using System.Drawing.Design;
using Telerik.WinControls.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Drawing.Drawing2D;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// ContextualTabGroups are used to organize RibbonBar Tabs in
	/// groups which are visible depending on certain context.
	/// </summary>
    [ToolboxItem(false)]
    [RadToolboxItem(false)]
    [Designer(DesignerConsts.ContextualTabGroupDesignerString)]
    public class ContextualTabGroup : RadItem
    {
        private RadItemCollection tabItems;

        /// <summary>
        /// Collection containing references to the TabItems in the group.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        [Editor(DesignerConsts.ContextualTabGroupTabsEditorsString, typeof(UITypeEditor))]
        public RadItemCollection TabItems
        {
            get
            {
                return this.tabItems;
            }
            set
            {
                this.tabItems = value;
            }
        }




        public static RadProperty BaseColorProperty = RadProperty.Register(
           "BaseColor", typeof(Color), typeof(ContextualTabGroup), new RadElementPropertyMetadata(
               Color.Empty, ElementPropertyOptions.AffectsDisplay));

        /// <summary>Gets or sets the displayed text.</summary>
        [RadPropertyDefaultValue("BaseColor", typeof(ContextualTabGroup)), Category(RadDesignCategory.AppearanceCategory)]
        public Color BaseColor
        {
            get
            {
                return (Color)this.GetValue(BaseColorProperty);
            }
            set
            {
                this.SetValue(BaseColorProperty, value);
            }
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

            this.tabItems = new RadItemCollection();
            this.tabItems.ItemTypes = new Type[] { typeof(RibbonTab) };
#pragma warning disable 0618
            this.tabItems.ExcludedTypes = new Type[] { typeof(RadRibbonBarCommandTab) };
#pragma warning restore 0618
            this.tabItems.ItemsChanged += new ItemChangedDelegate(commandTabs_ItemsChanged);
        }

        private void commandTabs_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            if (operation == ItemsChangeOperation.Inserted && target != null)
            {
                target.RadPropertyChanged += new RadPropertyChangedEventHandler(tabStrip_RadPropertyChanged);

                if (this.IsInValidState(true))
                {
                    RadRibbonBar ribbonBar = (this.ElementTree.Control as RadRibbonBar);

                    if (ribbonBar != null)
                    {
                        ribbonBar.RibbonBarElement.RibbonCaption.CaptionLayout.InvalidateMeasure();
                        ribbonBar.RibbonBarElement.RibbonCaption.CaptionLayout.InvalidateArrange();
                        ribbonBar.RibbonBarElement.RibbonCaption.CaptionLayout.UpdateLayout();
                    }
                }
            }
            else if (operation == ItemsChangeOperation.Removed)
            {
                target.RadPropertyChanged -= new RadPropertyChangedEventHandler(tabStrip_RadPropertyChanged);

                if (this.IsInValidState(true))
                {
                    RadRibbonBar ribbonBar = (this.ElementTree.Control as RadRibbonBar);

                    if (ribbonBar != null)
                    {
                        ribbonBar.RibbonBarElement.RibbonCaption.CaptionLayout.InvalidateMeasure();
                        ribbonBar.RibbonBarElement.RibbonCaption.CaptionLayout.InvalidateArrange();
                        ribbonBar.RibbonBarElement.RibbonCaption.CaptionLayout.UpdateLayout();
                    }
                }
            }
            else if (operation == ItemsChangeOperation.Clearing)
            {
                foreach (RadItem item in this.tabItems)
                {
                    item.RadPropertyChanged -= new RadPropertyChangedEventHandler(tabStrip_RadPropertyChanged);
                }
            }

        }

        private void tabStrip_RadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
        {
            if (e.Metadata is RadElementPropertyMetadata)
            {
                RadElementPropertyMetadata metadata = e.Metadata as RadElementPropertyMetadata;
                if (metadata.AffectsLayout || metadata.InvalidatesLayout)
                {
                    this.InvalidateMeasure();
                    this.InvalidateArrange();
                }
            }
        }


        protected override SizeF MeasureOverride(SizeF proposedSize)
        {

            base.MeasureOverride(proposedSize);

            if (proposedSize.Width == float.PositiveInfinity || proposedSize.Height == float.PositiveInfinity)
            {
                return SizeF.Empty;
            }
            else
            {
                return new SizeF(proposedSize.Width, proposedSize.Height - 4f);
            }
        }

        private FillPrimitive fillPrimitive;
        private BorderPrimitive borderPrimitive;
        private TextPrimitive captionText;

        protected override void CreateChildElements()
        {
            this.borderPrimitive = new BorderPrimitive();
            this.borderPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.borderPrimitive.Class = "ContextualTabBorder";
            this.borderPrimitive.BoxStyle = BorderBoxStyle.SingleBorder;
            this.borderPrimitive.GradientStyle = GradientStyles.Linear;
            this.borderPrimitive.GradientAngle = 90;

            this.borderPrimitive.ForeColor = Color.Transparent;
            this.borderPrimitive.ForeColor2 = Color.FromArgb(196, 194, 206);
            this.borderPrimitive.ForeColor3 = Color.FromArgb(196, 194, 206);
            this.borderPrimitive.ForeColor4 = Color.Transparent;


            this.fillPrimitive = new FillPrimitive();
            this.fillPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.fillPrimitive.Class = "ContextualTabFill";
            this.fillPrimitive.GradientStyle = GradientStyles.Glass;
            this.fillPrimitive.BackColor = Color.FromArgb(10, this.BaseColor);
            this.fillPrimitive.BackColor2 = Color.FromArgb(90, this.BaseColor);
            this.fillPrimitive.BackColor3 = Color.FromArgb(130, this.BaseColor);
            this.fillPrimitive.BackColor4 = Color.FromArgb(180, this.BaseColor);
            this.fillPrimitive.GradientPercentage = 0.001F;
            this.fillPrimitive.GradientPercentage2 = 0.92F;
            this.fillPrimitive.NumberOfColors = 4;

            this.captionText = new TextPrimitive();
            this.captionText.BindProperty(TextPrimitive.TextProperty, this, ContextualTabGroup.TextProperty, PropertyBindingOptions.TwoWay);
            this.captionText.Alignment = ContentAlignment.MiddleLeft;
            this.captionText.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.captionText.Class = "ContextualTabCaption";
            this.captionText.Margin = new Padding(0, 5, 0, 0);
            this.Margin = new Padding(0, -2, 0, -3);
            this.Children.Add(this.fillPrimitive);
            this.Children.Add(this.borderPrimitive);
            this.Children.Add(this.captionText);
            this.ShouldPaint = true;
        }

        /// <summary>
        /// Processes bubble events.
        /// </summary>
        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
        {
            if (sender == this && args.RoutedEvent == RadElement.MouseClickedEvent)
            {
                if (this.tabItems.Count > 0)
                {
                    RadPageViewElement tabStrip = (this.tabItems[0] as RadPageViewItem).Owner;

                    if (tabStrip == null)
                    {
                        throw new NullReferenceException(
                            string.Format("{0} has no parent TabStrip", this.tabItems[0].ToString()));
                    }

                    if (tabStrip.SelectedItem != this.tabItems[0])
                    {
                        tabStrip.SetSelectedItem((RadPageViewItem)this.tabItems[0]);
                    }
                }
            }

            base.OnBubbleEvent(sender, args);
        }

        protected override void OnPropertyChanging(RadPropertyChangingEventArgs args)
        {
            base.OnPropertyChanging(args);

            if (args.Property == RadItem.VisibilityProperty)
            {
                if (this.tabItems.Count > 0)
                {
                    RadPageViewElement tabStrip = (this.tabItems[0] as RadPageViewItem).Owner;

                    if (tabStrip == null)
                    {
                        throw new NullReferenceException(
                            string.Format("{0} has no parent TabStrip", this.tabItems[0].ToString()));
                    }


                    for (int i = 0; i < this.tabItems.Count; i++)
                    {
                        RadPageViewItem currentTab = this.tabItems[i] as RadPageViewItem;
                        currentTab.Visibility = (ElementVisibility)args.NewValue;
                    }

                    if (this.TabItems.Contains(tabStrip.SelectedItem as RibbonTab))
                    {
                        foreach (RibbonTab tab in tabStrip.Items)
                        {
                            if (!this.TabItems.Contains(tab))
                            {
                                tabStrip.SelectedItem = tab;
                                break;
                            }
                        }
                    }

                    tabStrip.InvalidateMeasure();
                    tabStrip.InvalidateArrange();
                    tabStrip.UpdateLayout();
                }
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ContextualTabGroup.BaseColorProperty)
            {
                this.fillPrimitive.BackColor = Color.FromArgb(10, this.BaseColor);
                this.fillPrimitive.BackColor2 = Color.FromArgb(90, this.BaseColor);
                this.fillPrimitive.BackColor3 = Color.FromArgb(130, this.BaseColor);
                this.fillPrimitive.BackColor4 = Color.FromArgb(180, this.BaseColor);
            }
        }

        protected override void PaintElement(Telerik.WinControls.Paint.IGraphics graphics, float angle, SizeF scale)
        {
            RadRibbonBar parentRibbon = this.ElementTree.Control as RadRibbonBar;

            if (parentRibbon != null)
            {
                if (parentRibbon.CompositionEnabled && !this.IsDesignMode
                    && this.TabItems.Count > 0 && this.ControlBoundingRectangle.Size != Size.Empty)
                {

                    if (this.captionText.Visibility == ElementVisibility.Visible)
                    {
                        this.captionText.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Hidden);
                    }

                    Color foreColor = Color.Black;

                    Form parentForm = parentRibbon.FindForm();

                    if (parentForm != null &&
                        parentForm.WindowState == FormWindowState.Maximized)
                    {
                        foreColor = Color.White;
                    }

                    GraphicsPath graphicsPath = new GraphicsPath();

                    graphicsPath.AddString(
                        this.captionText.Text,
                        this.Font.FontFamily,
                        (int)this.Font.Style,
                        (this.Font.Size / 72) * 96,
                        this.captionText.BoundingRectangle,
                        this.captionText.CreateStringFormat());
                    graphics.FillPath(foreColor, graphicsPath);

                }
            }
            else
            {
                if (this.captionText.Visibility == ElementVisibility.Hidden)
                {
                    this.captionText.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Visible);
                }
            }

        }
    }
}
