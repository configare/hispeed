using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Layouts;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Layout;
using System.Collections;

namespace Telerik.WinControls.UI
{

    /// <summary>
    ///     Represents a RadStatusBarElement.
    /// </summary>
    public class RadStatusBarElement : RadItem
    {
        //members
        private RadItemOwnerCollection items;        
        private WrapLayoutPanel/*DockLayoutPanel*/ /*BoxLayout*/ itemsWrapLayoutPanel;
        //private StackLayoutPanel itemsBoxLayout;
        private  StatusBarBoxLayout itemsBoxLayout;
        private BorderPrimitive borderPrimitive;
        private FillPrimitive fillPrimitive;
        private RadStatusBarLayoutStyle layoutStyle = RadStatusBarLayoutStyle.Stack;
        private Orientation orientation = Orientation.Horizontal;
        private RadGripElement grip;
        private ToolStripGripStyle gripStyle;        

       
        /// <summary>
        /// this event fired before Layout Style Changing 
        /// </summary>
        public event ValueChangingEventHandler LayoutStyleChanging;


        /// <summary>
        /// this event fired after LayoutStyle Changed
        /// </summary>
        public event EventHandler LayoutStyleChanged;

        static RadStatusBarElement()
        {
            new Themes.ControlDefault.StatusStrip().DeserializeTheme();
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.items = new RadItemOwnerCollection();
            this.items.ItemTypes = new Type[] {
                typeof(RadButtonElement),                 
                typeof(RadLabelElement),
                typeof(RadProgressBarElement),
                typeof(RadStatusBarPanelElement),
                typeof(RadToolStripSeparatorItem),
                typeof(RadTrackBarElement),
                typeof(RadSplitButtonElement)
            };

            this.items.DefaultType = typeof(RadButtonElement);
            this.items.ItemsChanged += new ItemChangedDelegate(this.ItemsChanged);

            this.StretchHorizontally = true;
            this.StretchVertically = false;
        }

        /// <summary>
        /// Gets a collection representing the "View changing" items contained in this statusbar.
        /// </summary>
        [RadEditItemsAction]
        [RadNewItem("Type here", true)]
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadItemOwnerCollection Items
        {
            get
            {
                return this.items;
            }
        }

        
        private void ItemsChanged(RadItemCollection changed, RadElement target, ItemsChangeOperation operation)
        {
            RadItem item = target as RadItem;
            if (item != null)
            {
                if (operation == ItemsChangeOperation.Inserted || operation == ItemsChangeOperation.Set)
                {
                    item.Margin = new Padding(1, 1, 1, 1);
                }
            }
        }


        /// <summary>
        /// get or set RadStatusBarElement orienatation
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return this.orientation;
            }
            set
            {
                if (this.orientation != value)
                {
                    this.orientation = value;

                    this.itemsBoxLayout.Orientation = this.orientation;
                    this.itemsWrapLayoutPanel.Orientation = this.orientation;
                    this.SetStreching();
                    this.UpdateGripStyle();
                    this.UpdateSeparatorsItems();
                }
            }
        }

        private void UpdateSeparatorsItems()
        {
            int angleTransform = 0;
            if (this.Orientation == Orientation.Vertical)
            {
                angleTransform = 90;
            }
            foreach (RadItem entry in this.Items)
            {
                if (entry is RadToolStripSeparatorItem)
                {
                    entry.AngleTransform = angleTransform;
                }
            }
        }


        /// <summary>
        /// show or hide the Grip element in RadStatusStrip
        /// </summary>
        public ToolStripGripStyle GripStyle
        {
            get
            {
                return this.gripStyle;
            }
            set
            {
                this.gripStyle = value;
                switch (this.gripStyle)
                {
                    case ToolStripGripStyle.Hidden:
                        this.grip.Image.Visibility = ElementVisibility.Collapsed;
                        this.itemsBoxLayout.Margin = new Padding(0, 0, 0, 0);
                        this.itemsWrapLayoutPanel.Margin = new Padding(0, 0, 0, 0);
                        break;
                    case ToolStripGripStyle.Visible:
                        this.grip.Image.Visibility = ElementVisibility.Visible;
                        this.grip.Image.AngleTransform = (this.RightToLeft) ? 90 : 0;
                        this.itemsBoxLayout.Margin = (this.RightToLeft) ? new Padding(14, 0, 0, 0) : new Padding(0, 0, 14, 0);
                        this.itemsWrapLayoutPanel.Margin = (this.RightToLeft) ? new Padding(14, 0, 0, 0) : new Padding(0, 0, 14, 0);
                        break;
                    default:
                        break;
                };

            }
        }

        private void UpdateGripStyle()
        {
            switch (this.gripStyle)
            {
                case ToolStripGripStyle.Hidden:
                    this.grip.Image.Visibility = ElementVisibility.Collapsed;
                    break;
                case ToolStripGripStyle.Visible:
                    switch (this.ElementTree.Control.Dock)
                    {
                        case DockStyle.Bottom:                            
                        case DockStyle.Fill:                            
                        case DockStyle.Left:
                            this.grip.Image.Visibility = ElementVisibility.Visible;
                            this.grip.Image.AngleTransform = (this.RightToLeft) ? 90 : 0;
                            this.itemsBoxLayout.Margin = (this.RightToLeft) ? new Padding(14, 0, 0, 0) : new Padding(0, 0, 14, 0);
                            this.itemsWrapLayoutPanel.Margin = (this.RightToLeft) ? new Padding(14, 0, 0, 0) : new Padding(0, 0, 14, 0);
                            break;
                        case DockStyle.None:                           
                        case DockStyle.Right:                        
                        case DockStyle.Top:
                            this.grip.Image.Visibility = ElementVisibility.Collapsed;
                            this.itemsBoxLayout.Margin = new Padding(0, 0, 0, 0);
                            this.itemsWrapLayoutPanel.Margin = new Padding(0, 0, 0, 0);
                            break;
                    
                    }
                    break;             
            }
        }
    
        
        /// <summary>
        /// Set the RadStatusStrip's layout style 
        /// </summary>
        [Description("ToolStripLayoutStyle"),
        AmbientValue(0),
        Category("Layout")]
        public RadStatusBarLayoutStyle LayoutStyle
        {
            get
            {
                return this.layoutStyle;
            }

            set
            {
                if (this.layoutStyle != value)
                {
                    ValueChangingEventArgs args = new ValueChangingEventArgs( this.layoutStyle, value );
                    this.OnLayoutStyleChanging(this, args);
                    if (args.Cancel)//LayoutStyle changing was canceled
                    {
                        return;
                    }

                    this.layoutStyle = value;

                    switch (this.layoutStyle)
                    {
                        case RadStatusBarLayoutStyle.Stack:
                            this.itemsBoxLayout.Visibility = ElementVisibility.Visible;
                            this.itemsWrapLayoutPanel.Visibility = ElementVisibility.Collapsed;
                            this.items.Owner = this.itemsBoxLayout;
                            break;
                        case RadStatusBarLayoutStyle.Overflow:
                            this.itemsBoxLayout.Visibility = ElementVisibility.Collapsed;
                            this.itemsWrapLayoutPanel.Visibility = ElementVisibility.Visible;
                            this.items.Owner = this.itemsWrapLayoutPanel;
                            //this.ElementTree.RootElement.UpdateLayout();
                            break;                    
                    }
                    this.SetStreching();                    

                    this.OnLayoutStyleChanged(this, EventArgs.Empty);
                }
            }

        }

        private void SetStreching()
        {
            this.ElementTree.RootElement.StretchHorizontally = false;
            this.ElementTree.RootElement.StretchVertically = false;
            this.StretchHorizontally = false;
            this.itemsBoxLayout.StretchHorizontally = false;
            this.StretchVertically = false;
            this.itemsBoxLayout.StretchVertically = false;

            if (this.LayoutStyle == RadStatusBarLayoutStyle.Stack)
            {
                this.ElementTree.RootElement.StretchVertically = true;
                this.StretchVertically = true;
                this.itemsBoxLayout.StretchVertically = true;

                this.ElementTree.RootElement.StretchHorizontally = true;
                this.StretchHorizontally = true;
                this.itemsBoxLayout.StretchHorizontally = true;

                if (this.Orientation == Orientation.Vertical)
                {
                    this.ElementTree.RootElement.StretchHorizontally = false;
                    this.StretchHorizontally = false;
                    this.itemsBoxLayout.StretchHorizontally = false;
                }
                else
                {
                    this.ElementTree.RootElement.StretchVertically = false;                    
                    this.StretchVertically = false;
                    this.itemsBoxLayout.StretchVertically = false;
                }
            }
            else//RadStatusBarLayoutStyle.Stack
            {
                if (this.Orientation == Orientation.Vertical)
                {
                    this.ElementTree.RootElement.StretchVertically = true;
                    this.StretchVertically = true;
                    this.itemsBoxLayout.StretchVertically = true;

                    this.ElementTree.RootElement.StretchHorizontally = false;
                    this.StretchHorizontally = false;
                    this.itemsBoxLayout.StretchHorizontally = false;
                }
                else
                {
                    this.ElementTree.RootElement.StretchVertically = false;
                    this.StretchVertically = false;
                    this.itemsBoxLayout.StretchVertically = false;

                    this.ElementTree.RootElement.StretchHorizontally = true;
                    this.StretchHorizontally = true;
                    this.itemsBoxLayout.StretchHorizontally = true;
                }
            }
           this.ElementTree.Control.PerformLayout();
        }

        private void OnLayoutStyleChanged(object sender, EventArgs eventArgs)
        {
            if (this.LayoutStyleChanged != null)
            {
                this.LayoutStyleChanged(sender, eventArgs);
            }
        }

        private void OnLayoutStyleChanging(object sender, ValueChangingEventArgs args)
        {
            if (this.LayoutStyleChanging != null)
            {
                this.LayoutStyleChanging(sender, args);
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == RadElement.RightToLeftProperty)
            {
                UpdateGripStyle();
            }
        }

        /// <summary>
        /// create elements in the RadStatusBarElement
        /// </summary>
        protected override void CreateChildElements()
        {            
            this.itemsWrapLayoutPanel = new WrapLayoutPanel();
            this.itemsBoxLayout = new StatusBarBoxLayout();
            //hide this panel by default
            this.itemsWrapLayoutPanel.Visibility = ElementVisibility.Collapsed;
            
            DockLayoutPanel.SetDock(itemsWrapLayoutPanel, Dock.Right);
            
            this.items.Owner = this.itemsBoxLayout;
            this.items.ItemTypes = new Type[] { typeof(RadButtonElement), typeof(RadToggleButtonElement), typeof(RadRepeatButtonElement), typeof(RadCheckBoxElement), typeof(RadImageButtonElement),
                                                typeof(RadRadioButtonElement), typeof(RadLabelElement), typeof(RadProgressBarElement), typeof(RadStatusBarPanelElement), typeof(RadToolStripSeparatorItem),
                                                typeof(RadTrackBarElement), typeof(RadSplitButtonElement)};
            this.fillPrimitive = new FillPrimitive();
            

            this.borderPrimitive = new BorderPrimitive();
            this.borderPrimitive.BoxStyle = BorderBoxStyle.OuterInnerBorders;
            this.borderPrimitive.Class = "StatusBarBorder";
            this.borderPrimitive.Width = 2f;
            this.borderPrimitive.BackColor = Color.DarkBlue;
            this.borderPrimitive.InnerColor = Color.White;            

            this.fillPrimitive = new FillPrimitive();
            this.fillPrimitive.Class = "StatusBarFill";

            

            this.Children.Add(borderPrimitive);            
            this.Children.Add(fillPrimitive);

            this.Children.Add(itemsWrapLayoutPanel);
            this.Children.Add(itemsBoxLayout);
            
            DockLayoutPanel.SetDock(itemsWrapLayoutPanel, Dock.Left);
            DockLayoutPanel.SetDock(itemsBoxLayout, Dock.Left);
            
            this.grip = new RadGripElement();
            this.Children.Add(grip);
        }
    }
}
