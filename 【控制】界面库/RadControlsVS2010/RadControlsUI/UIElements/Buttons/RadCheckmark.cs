using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Primitives;
using System.Drawing;
using Telerik.WinControls.Enumerations;
using System.ComponentModel;
using Telerik.WinControls.Design;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
    ///<exclude/>
    /// <summary>Represents checkmark.</summary>
	public class RadCheckmark : RadItem
    {
        #region Dependency properties

        public static RadProperty CheckStateProperty = RadProperty.Register("CheckState", typeof(ToggleState), typeof(RadCheckmark),
            new RadElementPropertyMetadata(ToggleState.Off, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout |
                ElementPropertyOptions.AffectsDisplay));

        public static RadProperty IsImageProperty = RadProperty.Register("IsImageElement", typeof(bool), typeof(RadCheckmark),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty ImageProperty = RadProperty.Register("Image", typeof(Image), typeof(RadCheckmark),
            new RadElementPropertyMetadata(null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty ImageKeyProperty = RadProperty.Register("ImageKey", typeof(string), typeof(RadCheckmark),
            new RadElementPropertyMetadata(string.Empty, ElementPropertyOptions.None));

        public static RadProperty ImageIndexProperty = RadProperty.Register("ImageIndex", typeof(int), typeof(RadCheckmark),
            new RadElementPropertyMetadata(-1, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty ManageElementVisibilityProperty = RadProperty.Register("ManageElementVisibility", typeof(bool), typeof(RadCheckmark),
            new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        #endregion

        #region Fields

        private BorderPrimitive borderPrimitive;
		private FillPrimitive fillPrimitive;
		private ImagePrimitive imageElement;
		private CheckPrimitive checkElement;
        private bool enableVisualStates;

        #endregion

        #region Initialization

        static RadCheckmark()
        {
            new Telerik.WinControls.Themes.ControlDefault.ControlDefault_RadCheckmark().DeserializeTheme();
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.StretchHorizontally = false;
            this.StretchVertically = false;
            this.ShouldHandleMouseInput = false;
        }

        protected override void CreateChildElements()
        {
            fillPrimitive = new FillPrimitive();
            fillPrimitive.Class = "CheckMarkBackGround";

            borderPrimitive = new BorderPrimitive();
            borderPrimitive.Class = "CheckMarkBorder";

            this.Children.Add(fillPrimitive);
            this.Children.Add(borderPrimitive);
            this.Children.Add(this.CheckElement);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance of BorderPrimitive wrapped by this element. 
        /// </summary>
        public BorderPrimitive Border
        {
            get { return this.borderPrimitive; }
        }

        /// <summary>
        /// Gets the instance of FillPrimitive wrapped by this element. 
        /// </summary>
        public FillPrimitive Fill
        {
            get { return this.fillPrimitive; }
        }

        /// <summary>
        /// Gets the instance of ImagePrimitive wrapped by this element. 
        /// </summary>
        public ImagePrimitive ImageElement
        {
            get { return this.imageElement; }
        }

        /// <summary>
        /// Gets the instance of CheckPrimitive wrapped by this element. 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual CheckPrimitive CheckElement
        {
            get
            {
                if (this.checkElement == null)
                {
                    this.checkElement = new CheckPrimitive();
                    this.checkElement.Visibility = ElementVisibility.Hidden;
                }
                return this.checkElement;
            }
        }

        ///<summary>
        /// Gets or sets the image that is displayed on a button element.
        /// </summary>		
        [RadPropertyDefaultValue("Image", typeof(RadCheckmark)),
        Category(RadDesignCategory.AppearanceCategory),
        Description("Gets or sets the image that is displayed on a button element."),
        RefreshProperties(RefreshProperties.All),
        TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        public virtual Image Image
        {
            get
            {
                return (Image)this.GetValue(ImageProperty);
            }
            set
            {
                this.SetValue(ImageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the image list index value of the image displayed as a checkmark.
        /// </summary>
        [RadPropertyDefaultValue("ImageIndex", typeof(RadCheckmark)),
        Category(RadDesignCategory.AppearanceCategory),
        Description("Gets or sets the image list index value of the image displayed as a checkmark."),
        RefreshProperties(RefreshProperties.All),
        RelatedImageList("ElementTree.Control.ImageList"),
        Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor)),
        TypeConverter(DesignerConsts.NoneExcludedImageIndexConverterString)]
        public virtual int ImageIndex
        {
            get
            {
                return (int)this.GetValue(ImageIndexProperty);
            }
            set
            {
                this.SetValue(ImageIndexProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the key accessor for the image in the ImageList.
        /// </summary>		
        [RadPropertyDefaultValue("ImageKey", typeof(RadCheckmark)),
        Category(RadDesignCategory.AppearanceCategory),
        Description("Gets or sets the key accessor for the image in the ImageList."),
        RefreshProperties(RefreshProperties.All),
        RelatedImageList("ElementTree.Control.ImageList"),
        Editor(DesignerConsts.ImageKeyEditorString, typeof(UITypeEditor)),
        TypeConverter(DesignerConsts.RadImageKeyConverterString)]
        public virtual string ImageKey
        {
            get
            {
                return (string)this.GetValue(ImageKeyProperty);
            }
            set
            {
                this.SetValue(ImageKeyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value indicating RadCheckmark checkstate.
        /// </summary>
        public ToggleState CheckState
        {
            get
            {
                return (ToggleState)this.GetValue(CheckStateProperty);
            }
            set
            {
                this.SetValue(CheckStateProperty, value);
            }
        }

        /// <summary>
        /// Determines whether the item will be used as a separate item in another element.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableVisualStates
        {
            get
            {
                return this.enableVisualStates;
            }
            set
            {
                if (this.enableVisualStates == value)
                {
                    return;
                }

                this.enableVisualStates = value;
                this.OnEnableVisualStatesChanged(EventArgs.Empty);
            }
        }
        
        internal override bool VsbVisible
        {
            get
            {
                return this.enableVisualStates;
            }
        }

        protected internal override bool CanHaveOwnStyle
        {
            get
            {
                return this.enableVisualStates;
            }
        }

        public virtual bool ManagerElementVisibility
        {
            get
            {
                return (bool)GetValue(ManageElementVisibilityProperty);
            }
            set
            {
                SetValue(ManageElementVisibilityProperty, value);
            }
        }
        
        #endregion

        #region Event handlers

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.EnsureImageElement();
        }

        protected override void OnChildrenChanged(RadElement child, ItemsChangeOperation changeOperation)
        {
            if ((this.imageElement == null || this.checkElement == null) && changeOperation == ItemsChangeOperation.Inserted)
            {
                if ((bool)child.GetValue(IsImageProperty))
                {
                    this.imageElement = child as ImagePrimitive;
                    SetCheckState();
                }
            }

            base.OnChildrenChanged(child, changeOperation);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == CheckStateProperty)
            {
                SetCheckState();
            }
            else if (e.Property == ImageProperty)
            {
                this.EnsureImageElement();
                this.imageElement.Image = (Image)e.NewValue;
                SetCheckState();
            }
            else if (e.Property == ImageIndexProperty)
            {
                this.EnsureImageElement();
                this.imageElement.ImageIndex = (int)e.NewValue;
                SetCheckState();
            }
            else if (e.Property == ImageKeyProperty)
            {
                this.EnsureImageElement();
                this.imageElement.ImageKey = (string)e.NewValue;
                SetCheckState();
            }
        }

        private void OnEnableVisualStatesChanged(EventArgs e)
        {
            if (this.enableVisualStates)
            {
                this.ThemeRole = "RadCheckmark";
                this.StateManager = new RadCheckmarkStateManager();
            }
            else
            {
                this.ThemeRole = null;
                this.StateManager = null;
            }
        }

        #endregion

        protected virtual void SetCheckState()
        {
            RadElement element = this.CheckElement;

            if (this.imageElement != null && !this.imageElement.IsEmpty)
            {
                element = this.imageElement;
            }

            if ((bool)GetValue(ManageElementVisibilityProperty))
            {
                if ((this.CheckState == ToggleState.On) || (this.CheckState == ToggleState.Indeterminate))
                {
                    element.Visibility = ElementVisibility.Visible;
                }
                else
                {
                    element.Visibility = ElementVisibility.Hidden;
                }
            }
        }

        private void EnsureImageElement()
        {
            //no foreign image is set, create our own instance
            if (this.imageElement == null)
            {
                this.imageElement = new ImagePrimitive();
                this.imageElement.Alignment = ContentAlignment.MiddleCenter;
                this.Children.Add(this.imageElement);
            }
        }
	}
}