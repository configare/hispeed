using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Telerik.WinControls.Styles;
using System.ComponentModel;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.Design;
using Telerik.WinControls.UI.Properties;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a toggle button in <see cref="CommandBarStripElement"/>.
    /// </summary>
    public class CommandBarToggleButton : RadCommandBarBaseItem
    {

        class CommandBarToggleButtonStateManagerFactory : ItemStateManagerFactory
        {
            protected override StateNodeBase CreateSpecificStates()
            {
                StateNodeWithCondition toggled = new StateNodeWithCondition("Toggled", new SimpleCondition(CommandBarToggleButton.ToggleStateProperty, ToggleState.On));
                 CompositeStateNode all = new CompositeStateNode("command bar toggle button states");
                all.AddState(toggled); 

                return all;
            }

            protected override ItemStateManagerBase CreateStateManager()
            {
                ItemStateManagerBase sm = base.CreateStateManager();

                sm.AddDefaultVisibleState("Toggled");
                  
                return sm;
            }
        }

        public CommandBarToggleButton()
        {
             this.Image = Resources.DefaultButton;
        }

        #region Static members

        static CommandBarToggleButton()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new CommandBarToggleButtonStateManagerFactory(), typeof(CommandBarToggleButton));
        }

        #endregion

        #region Dependency Properties

        public static RadProperty ToggleStateProperty = RadProperty.Register("ToggleState", typeof(ToggleState), typeof(CommandBarToggleButton),
            new RadElementPropertyMetadata(ToggleState.Off, ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region Fields

        private bool useVerticalText; 
        private bool isTreeState = false;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the IsChecked property is changed.
        /// </summary>
        public event EventHandler IsCheckedChanged;

        /// <summary>
        /// Occurs before the toggle state is changed.
        /// </summary>
        public event StateChangingEventHandler ToggleStateChanging;

        /// <summary>
        /// Occurs when the toggle state is changed.
        /// </summary>
        public event StateChangedEventHandler ToggleStateChanged;
         
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether the orientation of the text should be affected by its parent's orientation.
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets whether the orientation of the text should be affected by its parent's orientation.")]
        [Obsolete("This property is obsolete. Use the InheritsParentOrientation property instead")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool UseVerticalText
        {
            get
            {
                return useVerticalText;
            }
            set
            {
                useVerticalText = value;
            }

        }

         /// <summary>
        ///     Gets or sets the <see cref="Telerik.WinControls.Enumerations.ToggleState">toggle
        ///     state</see>. Toggle state enumeration defines the following values: Off,
        ///     Indeterminate, and On.
        /// </summary>
		[Browsable(true), Bindable(true),
		Category(RadDesignCategory.AppearanceCategory), 
	    RadPropertyDefaultValue("ToggleState", typeof(RadToggleButtonElement)),
		Description("Gets or sets the toggle state. Toggle state enumeration defines the following values: Off, Indeterminate, and On.")]
        public ToggleState ToggleState
        {
            get
            {
                return (ToggleState)this.GetValue(CommandBarToggleButton.ToggleStateProperty);
            }
            set
            {
                ToggleState state = this.ToggleState;
                if (state != value)
                {
                    StateChangingEventArgs eventArgs = new StateChangingEventArgs(state, value, false);
                    
                    this.OnToggleStateChanging(eventArgs);
                    if (eventArgs.Cancel)
                    {
                        return;
                    }

                    this.SetValue(CommandBarToggleButton.ToggleStateProperty, value);
                    this.OnToggleStateChanged(new StateChangedEventArgs(value));
                }
            }
        }

        /// <summary>Gets or sets a value indicating whether the button is checked.</summary>
		[Browsable(false), Bindable(true), RefreshProperties(RefreshProperties.All),
		Category(RadDesignCategory.AppearanceCategory), 
		Description("Gets or sets a value indicating whether the button is checked.")]
        [Obsolete("This property is obsolete. The ToggleState property must be used instead.")] // Skarlatov 1/10/2009
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsChecked
        {
            get
            {
                return (this.ToggleState != ToggleState.Off);                
            }
            set
            {
                if (value != this.IsChecked)
                {
                    this.ToggleState = value ? ToggleState.On : ToggleState.Off;					
                }

                OnIsCheckedChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the toggle button has three or two
        /// states.
        /// </summary>
		[Browsable(true), Bindable(true),
		Category(RadDesignCategory.BehaviorCategory),
		DefaultValue(false),
		Description("Gets or sets a value indicating whether the toggle button has three or two states.")]
        public bool IsThreeState
        {
            get
            {
                return this.isTreeState;
            }
            set
            {
                this.isTreeState = value;
            }
        }
        #endregion
         
        #region Overrides

        protected override void OnOrientationChanged(EventArgs e)
        {
            if (this.orientation == System.Windows.Forms.Orientation.Vertical)
            {
                this.SetDefaultValueOverride(TextImageRelationProperty, System.Windows.Forms.TextImageRelation.ImageAboveText);
            }
            else
            {
                this.SetDefaultValueOverride(TextImageRelationProperty, System.Windows.Forms.TextImageRelation.ImageBeforeText);
            }

            this.SetDefaultValueOverride(TextOrientationProperty, this.orientation);
            this.InvalidateMeasure(true);

            base.OnOrientationChanged(e);
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Raises the StateChanging event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnToggleStateChanging(StateChangingEventArgs e)
        {
            if (e.NewValue == ToggleState.Indeterminate && !this.isTreeState)
            {
                e.Cancel = true;
            }
             
            if (this.ToggleStateChanging != null)
            {
                this.ToggleStateChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the StateChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnToggleStateChanged(StateChangedEventArgs e)
        { 
            if (this.ToggleStateChanged != null)
            {
                this.ToggleStateChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the IsCheckedChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnIsCheckedChanged(EventArgs e)
        {
             
            if (this.IsCheckedChanged != null)
            {
                this.IsCheckedChanged(this, e);
            }
        }
         
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            this.OnToggle();
        }

        protected internal virtual void OnToggle()
        {
            if (this.isTreeState)
            {
                switch (this.ToggleState)
                {
                    case ToggleState.Off:
                        this.ToggleState = ToggleState.On;
                        break;

                    case ToggleState.On:
                        this.ToggleState = ToggleState.Indeterminate;
                        break;

                    case ToggleState.Indeterminate:
                        this.ToggleState = ToggleState.Off;
                        break;

                    default:
                        break;
                }
            }
            else
            {
                switch (this.ToggleState)
                {
                    case ToggleState.Off:
                        this.ToggleState = ToggleState.On;
                        break;

                    case ToggleState.On:
                        this.ToggleState = ToggleState.Off;
                        break;
                }
            }
        }

        #endregion
    }
}
