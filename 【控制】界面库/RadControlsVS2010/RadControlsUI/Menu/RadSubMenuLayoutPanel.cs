using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    ///// <summary>
    ///// Represents a layout for the sub menu. It is used in the RadDropDownButton and
    ///// RadMenu.
    ///// </summary>
    //public class RadSubMenuLayoutPanel : LayoutPanel
    //{
    //    private FillPrimitive leftFillPrimitive;
    //    private FillPrimitive fillPrimitive;
    //    private BorderPrimitive borderPrimitive;
    //    private StackLayoutPanel leftLayoutPanel;
    //    private StackLayoutPanel rightLayoutPanel;
    //    private StripLayoutPanel mainLayoutPanel;
    //    private StripLayoutPanel bottomStripLayoutPanel;
    //    private RadElement rightElement;
    //    private RadElement leftElement;
    //    private BorderPrimitive leftPanelBorder;

    //    private static readonly int minImageWidth = 20;

    //    public static RadProperty IsTwoColumnMenuProperty = RadProperty.Register(
    //        "IsTwoColumnMenu", typeof(bool), typeof(RadSubMenuLayoutPanel), new RadElementPropertyMetadata(
    //            false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

    //    /// <summary>
    //    ///		Indicates whether the layout panel contains one column or two columns of items.
    //    /// </summary>
    //    public bool IsTwoColumnMenu
    //    {
    //        get
    //        {
    //            return (bool) this.GetValue(IsTwoColumnMenuProperty);
    //        }
    //        set
    //        {
    //            this.SetValue(IsTwoColumnMenuProperty, value);
    //        }
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public RadElement LayoutPanel
    //    {
    //        get
    //        {

    //            return this.leftLayoutPanel;
    //        }
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public RadElement RightColumnLayoutPanel
    //    {
    //        get
    //        {
    //            return this.rightLayoutPanel;
    //        }
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public StripLayoutPanel BottomStrip
    //    {
    //        get
    //        {
    //            return this.bottomStripLayoutPanel;
    //        }
    //    }

    //    protected override void CreateChildElements()
    //    {
    //        this.leftFillPrimitive = new FillPrimitive();
    //        this.leftFillPrimitive.Class = "RadSubMenuPanelFillPrimitive";
    //        this.leftFillPrimitive.ZIndex = 1;

    //        this.leftLayoutPanel = new StackLayoutPanel();
    //        this.leftLayoutPanel.Orientation = Orientation.Vertical;
    //        this.leftLayoutPanel.EqualChildrenWidth = true;

    //        this.borderPrimitive = new BorderPrimitive();
    //        this.borderPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
    //        this.borderPrimitive.Class = "RadSubMenuPanelBorderPrimitive";

    //        this.fillPrimitive = new FillPrimitive();
    //        this.fillPrimitive.Class = "RadSubMenuPanelBackFillPrimitive";
    //        this.fillPrimitive.BackColor = Color.White;
    //        this.fillPrimitive.GradientStyle = GradientStyles.Solid;

    //        this.mainLayoutPanel = new StripLayoutPanel();
    //        this.mainLayoutPanel.Orientation = Orientation.Horizontal;
    //        this.mainLayoutPanel.EqualChildrenHeight = true;

    //        this.rightLayoutPanel = new StackLayoutPanel();
    //        this.rightLayoutPanel.Orientation = Orientation.Vertical;

    //        this.leftPanelBorder = new BorderPrimitive();
    //        this.leftPanelBorder.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
    //        this.leftPanelBorder.Visibility = ElementVisibility.Collapsed;
    //        this.leftPanelBorder.Class = "RadSubMenuLeftPanelBorder";
			
    //        this.leftElement = new RadElement();
    //        this.leftElement.Class = "RadSubMenuLeftPanel";
    //        this.leftElement.Children.Add(this.leftLayoutPanel);
    //        this.leftElement.Children.Add(this.leftPanelBorder);

    //        StripLayoutPanel wrapperLayoutPanel = new StripLayoutPanel();
    //        wrapperLayoutPanel.Orientation = Orientation.Vertical;
    //        wrapperLayoutPanel.EqualChildrenWidth = true;
    //        wrapperLayoutPanel.ZIndex = 2;

    //        BorderPrimitive rightPanelBorder = new BorderPrimitive();
    //        rightPanelBorder.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
    //        rightPanelBorder.BoxStyle = BorderBoxStyle.FourBorders;
    //        rightPanelBorder.Class = "RadSubMenuRightPanelBorder";
    //        rightPanelBorder.LeftWidth = 0;
    //        this.rightElement = new RadElement();
    //        this.rightElement.Class = "RadSubMenuRightPanel";
    //        this.rightElement.Visibility = ElementVisibility.Collapsed;
    //        this.rightElement.Margin = new Padding(0, 10, 3, 0);
    //        this.rightElement.Children.Add(this.rightLayoutPanel);
    //        this.rightElement.Children.Add(rightPanelBorder);

    //        this.mainLayoutPanel.Children.Add(this.leftElement);
    //        this.mainLayoutPanel.Children.Add(this.rightElement);

    //        this.bottomStripLayoutPanel = new StripLayoutPanel();
    //        this.bottomStripLayoutPanel.MinSize = new Size(0, 10);
    //        this.bottomStripLayoutPanel.Visibility = ElementVisibility.Collapsed;
    //        this.bottomStripLayoutPanel.Orientation = Orientation.Horizontal;

    //        wrapperLayoutPanel.Children.Add(this.mainLayoutPanel);
    //        wrapperLayoutPanel.Children.Add(this.bottomStripLayoutPanel);

    //        this.Children.Add(this.fillPrimitive);
    //        this.Children.Add(this.leftFillPrimitive);
    //        this.Children.Add(wrapperLayoutPanel);
    //        this.Children.Add(this.borderPrimitive);
    //    }

    //    /// <commentsfrom cref="Telerik.WinControls.Layouts.IRadLayoutElement.PerformLayout" filter=""/>
    //    public override void PerformLayoutCore(RadElement affectedElement)
    //    {
    //        base.PerformLayoutCore(affectedElement);

    //        SetMenuItemCorrections(this.leftLayoutPanel);
    //        if (this.IsTwoColumnMenu)
    //            SetMenuItemCorrections(this.rightLayoutPanel);
    //    }

    //    private void SetMenuItemCorrections(LayoutPanel layoutPanel)
    //    {
    //        int maxWidth = minImageWidth;
    //        int maxWidthWithPadding = minImageWidth;
    //        int maxItemWidth = 0;

    //        foreach (RadElement child in layoutPanel.Children)
    //        {
    //            RadMenuItemBase menuItem = child as RadMenuItemBase;
    //            if (menuItem != null)
    //            {
    //                maxWidth = Math.Max(menuItem.OffsetWidth, maxWidth);
    //                maxWidthWithPadding = Math.Max(menuItem.OffsetWidth + menuItem.Padding.Left + menuItem.Margin.Left, maxWidthWithPadding);
    //                maxItemWidth = Math.Max(menuItem.AlignRightOffsetWidth, maxItemWidth);
    //            }
    //        }
    //        int separatorOffset = maxWidth + 6;

    //        if (!this.IsTwoColumnMenu)
    //            this.leftFillPrimitive.Size = new Size(maxWidthWithPadding, this.FieldSize.Height);

    //        this.mainLayoutPanel.SuspendLayout();
    //        foreach (RadElement child in layoutPanel.Children)
    //        {
    //            RadMenuItemBase menuItem = child as RadMenuItemBase;

    //            if (menuItem == null)
    //                continue;

    //            if (child is RadMenuSeparatorItem)
    //            {
    //                child.PositionOffset = new SizeF(child.Location.X + separatorOffset, 0);
    //                child.AutoSize = false;
    //                child.Size = new Size(layoutPanel.Size.Width - separatorOffset, child.Size.Height);
    //                continue;
    //            }

    //            if (menuItem.ElementToOffset == null)
    //                continue;


    //            if ((maxWidth <= menuItem.OffsetWidth) || (child is RadMenuContentItem && !((RadMenuContentItem)child).ShowImageOffset))
    //            {
    //                Padding padding = new Padding(0, 0, 0, 0);
    //                menuItem.ElementToOffset.Margin = padding;
    //                menuItem.SetElementOffsets(padding);
    //            }
    //            else
    //            {
    //                Padding padding = new Padding(maxWidth - menuItem.OffsetWidth, 0, 0, 0);
    //                menuItem.ElementToOffset.Margin = padding;
    //                menuItem.SetElementOffsets(padding);
    //            }
				
    //            if (menuItem.ElementToAlignRight != null)
    //                menuItem.ElementToAlignRight.Margin = new Padding(maxItemWidth - menuItem.AlignRightOffsetWidth + 4, 0, 0, 0);

    //            // TODO: We shouldn't do this. Obviously there's a layout problem.
    //            layoutPanel.IsLayoutInvalidated = true;
    //        }
    //        this.mainLayoutPanel.ResumeLayout(true);
    //    }

    //    protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
    //    {
    //        base.OnPropertyChanged(e);

    //        if (e.Property == IsTwoColumnMenuProperty)
    //        {
    //            if ((bool) e.NewValue)
    //            {
    //                this.leftFillPrimitive.Visibility = ElementVisibility.Collapsed;
    //                this.rightElement.Visibility = ElementVisibility.Visible;
    //                this.leftPanelBorder.Visibility = ElementVisibility.Visible;
    //                this.leftElement.Margin = new Padding(3, 10, 0, 0);
    //                this.bottomStripLayoutPanel.Visibility = ElementVisibility.Visible;
    //            }
    //            else
    //            {
    //                this.leftFillPrimitive.Visibility = ElementVisibility.Visible;
    //                this.rightElement.Visibility = ElementVisibility.Collapsed;
    //                this.leftPanelBorder.Visibility = ElementVisibility.Collapsed;
    //                this.leftElement.Margin = Padding.Empty;
    //                this.bottomStripLayoutPanel.Visibility = ElementVisibility.Collapsed;
    //            }
    //        }
    //        if (e.Property == RadElement.MinSizeProperty)
    //        {
    //            Debug.WriteLine(e.OldValue + "->" + e.NewValue);
    //        }
    //    }
    //}
}