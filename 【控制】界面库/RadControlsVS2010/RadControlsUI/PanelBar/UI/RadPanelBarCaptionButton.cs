using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Layouts;
using Telerik.WinControls;
using System.Reflection;
using System.Drawing;
using System.ComponentModel;
using Telerik.WinControls.Design;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
    public class RadPanelBarCaptionButton : RadButtonElement
    {
        private GroupStatePrimitive state;
        private RadPanelBarGroupElement owner;
   
        public RadPanelBarCaptionButton(RadPanelBarGroupElement owner)
        {
            this.owner = owner;
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.state = new GroupStatePrimitive();

            this.Alignment = System.Drawing.ContentAlignment.MiddleRight;
            this.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
			
            this.Children.Add(this.state);
			
            state.Visibility = ElementVisibility.Hidden;
            this.fillPrimitive.Visibility = ElementVisibility.Hidden;
            this.BorderElement.Visibility = ElementVisibility.Hidden;
        }

        private void SetImage(string searchPattern)
        {
            this.Image = new Bitmap(16, 16);
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            string[] s = currentAssembly.GetManifestResourceNames();
            foreach (string str in s)
            {
                if (str.EndsWith(searchPattern))//why this sir!!!???!!!
                {
                    this.Image = Image.FromStream(Telerik.WinControls.TelerikHelper.GetStreamFromResource(currentAssembly, str));
                }
            }
        }

        private Image GetImage(string searchPattern)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            string[] s = currentAssembly.GetManifestResourceNames();            
            foreach (string str in s)
            {
                if (str.EndsWith(searchPattern))//why this sir!!!???!!!
                {
                    return Image.FromStream(Telerik.WinControls.TelerikHelper.GetStreamFromResource(currentAssembly, str));
                }
            }

            return new Bitmap(16, 16);
        }

        internal void SetDefaultImages()
        {
            this.owner.GroupExpandExplorerBarImage = GetImage("arrowUp.png");
            this.owner.GroupCollapseVSImage = GetImage("plus.png");
            this.owner.GroupCollapseExplorerBarImage = GetImage("arrowDown.png");
            this.owner.GroupExpandVSImage = GetImage("minus.png");     
        }

        public static RadProperty UseCustomImagesProperty = RadProperty.Register(
            "UseCustomImages", typeof(bool), typeof(RadPanelBarCaptionButton), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.None));

        public virtual bool UseCustomImages
        {
            get
            {
                return (bool)this.GetValue(UseCustomImagesProperty);
            }
            set
            {
                this.SetValue(UseCustomImagesProperty, value);
            }
        }

        public static RadProperty CustomExpandImageProperty = RadProperty.Register(
            "CustomExpandImage",
            typeof(Image),
            typeof(RadPanelBarCaptionButton),
            new RadElementPropertyMetadata(
                null,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsDisplay));

        [RadPropertyDefaultValue("CustomExpandImage", typeof(RadPanelBarCaptionButton)),
        Localizable(true),
        Category(RadDesignCategory.AppearanceCategory),
        Description("Gets or sets the image that is displayed on a button element."),
        RefreshProperties(RefreshProperties.All),
        TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        public virtual Image CustomExpandImage
        {
            get
            {
                return (Image)this.GetValue(CustomExpandImageProperty);
            }
            set
            {
                this.SetValue(CustomExpandImageProperty, value);
            }
        }

        public static RadProperty CustomCollapseImageProperty = RadProperty.Register(
            "CustomCollapseImage",
            typeof(Image),
            typeof(RadPanelBarCaptionButton),
            new RadElementPropertyMetadata(
                null,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsDisplay));

        [RadPropertyDefaultValue("CustomCollapseImage", typeof(RadPanelBarCaptionButton)),
        Localizable(true),
        Category(RadDesignCategory.AppearanceCategory),
        Description("Gets or sets the image that is displayed on a button element."),
        RefreshProperties(RefreshProperties.All),
        TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        public virtual Image CustomCollapseImage
        {
            get
            {
                return (Image)this.GetValue(CustomCollapseImageProperty);
            }
            set
            {
                this.SetValue(CustomCollapseImageProperty, value);
            }
        }

        public void ChangeStyle(GroupStatePrimitive.GroupState state)
        {
            this.Image = null;

            if (UseCustomImages)
            {
                if (state == GroupStatePrimitive.GroupState.Expanded)
                {
                    this.Image = this.CustomExpandImage;
                }
                else
                {
                    this.Image = this.CustomCollapseImage;
                }
                return;
            }

            if (state == GroupStatePrimitive.GroupState.Expanded && this.state.PanelBarStyle == PanelBarStyles.ExplorerBarStyle)
            {
                //this.Image = this.owner.GroupExpandExplorerBarImage;
                SetImage("arrowUp.png");
                //TODO: remove this
                if (this.ElementTree != null && (this.ElementTree.Control as RadPanelBar).ThemeName == "Desert")
                {
                    SetImage("arrowUpDesert.png");
                }
            }

            if (state == GroupStatePrimitive.GroupState.Collapsed && this.state.PanelBarStyle == PanelBarStyles.ExplorerBarStyle)
            {
                //this.Image = this.owner.GroupCollapseExplorerBarImage;
                SetImage("arrowDown.png");
                //TODO: remove this
                if (this.ElementTree != null && (this.ElementTree.Control as RadPanelBar).ThemeName == "Desert")
                {
                    SetImage("arrowDownDesert.png");
                }
            }

            if (state == GroupStatePrimitive.GroupState.Expanded && this.state.PanelBarStyle == PanelBarStyles.VisualStudio2005ToolBox)
            {
                //this.Image = this.owner.GroupExpandVSImage;
                SetImage("minus.png");
                //TODO: remove this
                if (this.ElementTree != null && (this.ElementTree.Control as RadPanelBar).ThemeName == "Desert")
                {
                    SetImage("minusDesert.png");
                }
            }

            if (state == GroupStatePrimitive.GroupState.Collapsed && this.state.PanelBarStyle == PanelBarStyles.VisualStudio2005ToolBox)
            {
                //this.Image = this.owner.GroupCollapseVSImage;
                SetImage("plus.png");
                //TODO: remove this
                if (this.ElementTree != null && (this.ElementTree.Control as RadPanelBar).ThemeName == "Desert")
                {
                    SetImage("plusDesert.png");
                }
            }
            
            this.state.State = state;
        }

        public GroupStatePrimitive GetGroupState()
        {
            return this.state;
        }
    }
}
