using System;
using System.ComponentModel;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls.Design;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     Represents a check box. The RadCheckBox class is a simple wrapper for the
    ///     <see cref="RadCheckBoxElement">RadCheckBoxElement</see> class. The RadCheckBox acts
    ///     to transfer events to and from its corresponding
    ///     <see cref="RadCheckBoxElement">RadCheckBoxElement</see>. The
    ///     <see cref="RadCheckBoxElement">RadCheckBoxElement</see> which is essentially the
    ///     RadCheckBox control may be nested in other telerik controls.
    /// </summary>
    [RadThemeDesignerData(typeof(RadCheckBoxStyleBuilderData))]
	[ToolboxItem(true)]
	[Description("Enables the user to select or clear the associated option.")]
	public class RadCheckBox : RadToggleButton
    { 
        #region Properties

        /// <summary>
        /// Gets the instance of RadCheckBoxElement wrapped by this control. RadCheckBoxElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadCheckBox.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new RadCheckBoxElement ButtonElement
		{
            get { return (RadCheckBoxElement) base.ButtonElement; }
		}

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 18);
            }
        }

        /// <summary>
        /// 	<para>Gets or sets a value indicating whether the control is automatically resized
        ///     to display its entire contents.</para>
        /// </summary>
        // ND - DefaultValue should be true, ticket 304065
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Description("Gets or sets a value indicating whether the control is automatically resized to display its entire contents."),
        Browsable(true), Category(RadDesignCategory.LayoutCategory),
        EditorBrowsable(EditorBrowsableState.Always)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

        /// <summary>Gets or sets a value indicating the alignment of the check box.</summary>
        [Obsolete("Use CheckAlignment property instead.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
		public System.Drawing.ContentAlignment CheckAligment
		{
            get 
            {
                return this.ButtonElement.CheckAlignment; 
            }
            set 
            {
                this.ButtonElement.CheckAlignment = value; 
            }
		}

        /// <summary>Gets or sets value idicating the checked state of the checkbox.</summary>
        /// <remarks>
        /// Since RadCheckBox is tri-state based (ToggleState property) the Checked property is provided for compatibility only.
        /// Checked=true corresponds to ToggleState.On and Checked=false corresponds to ToggleState.Off. 
        /// If value of ToggleState property equals <see cref="Telerik.WinControls.Enumerations.ToggleState.Indeterminate"/>, 
        /// value of Checked property is 'false'.
        /// </remarks>
        [Description("Gets or sets value idicating the checked state of the checkbox.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false), Bindable(true)]
        public bool Checked
        {
            get
            {
                return (this.ButtonElement as RadCheckBoxElement).Checked;
            }

            set
            {
                (this.ButtonElement as RadCheckBoxElement).Checked = value;
            }

        }

        [Browsable(false)]        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]        
        public override bool IsChecked
        {
            get
            {
                return base.ToggleState == Telerik.WinControls.Enumerations.ToggleState.On;
            }
            set
            {
                base.ToggleState = value ? Telerik.WinControls.Enumerations.ToggleState.On : Telerik.WinControls.Enumerations.ToggleState.Off;
            }
        }

        /// <summary>
        /// Gets or sets a value indication whether mnemonics are used.
        /// </summary>
        [Category("Appearance"),
        Description("Determines whether the button can be clicked by using mnemonic characters."),
        DefaultValue(true)]
        new public bool UseMnemonic
        {
            get
            {
                return this.ButtonElement.TextElement.UseMnemonic;
            }
            set
            {
                this.ButtonElement.TextElement.UseMnemonic = value;
            }
        }

        /// <summary>Gets or sets a value indicating the alignment of the check box.</summary>
        [RadPropertyDefaultValue("CheckAlignment", typeof(RadCheckBoxElement))]
        [RadDescription("CheckAlignment", typeof(RadCheckBoxElement))]
        [Category(RadDesignCategory.LayoutCategory)]
        public ContentAlignment CheckAlignment
        {
            get
            {
                return this.ButtonElement.CheckAlignment;
            }
            set
            {
                this.ButtonElement.CheckAlignment = value;
            }
        }

        #endregion

        #region Initalization

        /// <summary>Initializes a new instance of the RadCheckBoxElement class.</summary>
		public RadCheckBox()
		{
            this.AutoSize = true;
        }

        /// <summary>
        /// Create main button element that is specific for RadCheckBox.
        /// </summary>
        /// <returns>The element that encapsulates the funtionality of RadCheckBox</returns>
        protected override RadButtonElement CreateButtonElement()
        {
            RadCheckBoxElement res = new RadCheckBoxElement();
            res.UseNewLayoutSystem = true;
            res.ToggleStateChanging += new StateChangingEventHandler(ButtonElement_ToggleStateChanging);
            res.ToggleStateChanged += new StateChangedEventHandler(ButtonElement_ToggleStateChanged);
            res.PropertyChanged += new PropertyChangedEventHandler(res_PropertyChanged);
            return res;
        }

        #endregion

        #region Event handlers

        private void ButtonElement_ToggleStateChanging(object sender, StateChangingEventArgs args)
		{
			base.OnToggleStateChanging(args);
		}

		private void ButtonElement_ToggleStateChanged(object sender, StateChangedEventArgs args)
		{
			base.OnToggleStateChanged(args);
			base.OnNotifyPropertyChanged("Checked");
		}

		private void res_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsChecked")
			{
				base.OnNotifyPropertyChanged("IsChecked");
			}
        }

        #endregion

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            if (element is RadCheckBoxElement || element is RadCheckmark)
                return true;

            return base.ControlDefinesThemeForElement(element);
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadCheckBoxAccessibleObject(this);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            bool oldVal = this.isInitializing;
            this.isInitializing = false;
            this.LoadElementTree();
            this.isInitializing = oldVal;

            base.OnEnabledChanged(e);
        }
    }
}
