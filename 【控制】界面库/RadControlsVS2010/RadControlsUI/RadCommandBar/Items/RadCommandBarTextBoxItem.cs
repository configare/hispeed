using Telerik.WinControls.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represetns a text box in <see cref="CommandBarStripElement"/>.
    /// </summary>
    public class CommandBarTextBox : RadCommandBarBaseItem
    {
        protected RadTextBoxElement textBoxElement;
         
        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.StretchHorizontally = this.StretchVertically = false;
            this.textBoxElement = new RadTextBoxElement();
            this.textBoxElement.MinSize = new System.Drawing.Size(60, 22);
            this.textBoxElement.Class = "RadCommandBarTextBoxElement";
            this.Children.Add(this.textBoxElement);
        }

        #region Properties

        /// <summary>
        ///		Show or hide item from the strip
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory), DefaultValue(true)]
        [Description("Indicates whether the item should be drawn in the strip.")]
        public override bool VisibleInStrip
        {
            get
            {
                return base.VisibleInStrip;
            }
            set
            {
                base.VisibleInStrip = value;
                if (this.textBoxElement != null)
                {
                    this.textBoxElement.SetValue(RadElement.VisibilityProperty, (value) ? ElementVisibility.Visible : ElementVisibility.Collapsed);
                }
            }
        }


        /// <summary>
        /// Gets or sets the hosted <see cref="RadTextBoxElement"/>.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Represent the textbox inside into CommandBarTextBox")]
        public RadTextBoxElement TextBoxElement
        {
            get
            {
                return textBoxElement;
            }
            set
            {
                textBoxElement = value;
            }
        }

        [DefaultValue("")]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets the text associated with this item.")]
        [Bindable(true)]
        [SettingsBindable(true)]
        [Localizable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get
            {
                if (this.textBoxElement != null)
                {
                    return this.textBoxElement.Text;
                }
                else
                {
                    return base.Text;
                }
            }
            set
            {
                if (this.textBoxElement != null)
                {
                    this.TextBoxElement.Text = value;
                }
                base.Text = value;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the CommandBarTextBox has focus and the user pressees a key
        /// </summary>
        [Category("Key"), Description("Occurs when the RadItem has focus and the user pressees a key")]
        public new event KeyPressEventHandler KeyPress
        {
            add
            {
                this.textBoxElement.TextBoxItem.KeyPress += value;
            }

            remove
            {
                this.textBoxElement.TextBoxItem.KeyPress -= value;
            }
        }

        /// <summary>
        /// Occurs when the CommandBarTextBox has focus and the user releases the pressed key up
        /// </summary>
        [Category("Key"), Description("Occurs when the RadItem has focus and the user releases the pressed key up")]
        public new event KeyEventHandler KeyUp
        {
            add
            {
                this.textBoxElement.TextBoxItem.KeyUp += value;
            }

            remove
            {
                this.textBoxElement.TextBoxItem.KeyUp -= value;
            }
        }

        /// <summary>
        /// Occurs when the CommandBarTextBox has focus and the user pressees a key down
        /// </summary>
        [Category("Key"), Description("Occurs when the RadItem has focus and the user pressees a key down")]
        public new event KeyEventHandler KeyDown 
        {
            add
            {
                this.textBoxElement.TextBoxItem.KeyDown += value;
            }

            remove
            {
                this.textBoxElement.TextBoxItem.KeyDown -= value;
            }
        }

        /// <summary>
        ///		Occurs when the Text property value is about to be changed.
        /// </summary>
        [Category("Property Changed"), Description("Occurs when the Text property value is about to be changed.")]
        public new event TextChangingEventHandler TextChanging
        {
            add
            {
                this.textBoxElement.TextBoxItem.TextChanging += value;
            }

            remove
            {
                this.textBoxElement.TextBoxItem.TextChanging -= value;
            }
        }

        /// <summary>
        ///		Occurs when the Text property value changes.
        /// </summary>
        [Category("Property Changed"), Description("Occurs when the Text property value changes.")]
        public new event EventHandler TextChanged
        {
            add
            {
                this.textBoxElement.TextBoxItem.TextChanged += value;
            }

            remove
            {
                this.textBoxElement.TextBoxItem.TextChanged -= value;
            }
        }

        /// <summary>
        /// Occurs when the element recieves focus.
        /// </summary>
        [Browsable(false),
        Category("Property Changed"),
        Description("Occurs when the element recieves focus.")]
        public event EventHandler GotFocus
        {
            add
            {
                this.textBoxElement.TextBoxItem.GotFocus += value;
            }

            remove
            {
                this.textBoxElement.TextBoxItem.GotFocus -= value;
            }
        }

        /// <summary>
        /// Occurs when the element loses focus.
        /// </summary>
        [Browsable(false),
        Category("Property Changed"),
        Description("Occurs when the element loses focus.")]
        public event EventHandler LostFocus
        {
            add
            {
                this.textBoxElement.TextBoxItem.LostFocus += value;
            }

            remove
            {
                this.textBoxElement.TextBoxItem.LostFocus -= value;
            }
        }

        #endregion
    }
}
