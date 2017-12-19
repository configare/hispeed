using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using Telerik.WinControls.Design;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a dialog that can be used to select color with rich UI and extended functionality.
    /// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.DialogsGroup)]
    [ToolboxItem(true)]
	[Description("Displays a dialog that can be used to select a color with rich UI and extended functionality.")]
	[DefaultProperty("SelectedColor")]
    public class RadColorDialog: CommonDialog
    {
        IRadColorDialog colorDialogForm = (IRadColorDialog)RadColorEditor.CreateColorDialogInstance();

        ///<summary>
        ///Resets the properties of a color dialog box to their default values. Replaces the underlaying ColorDialogForm
        /// with new instance 
        ///</summary>
        ///<filterpriority>1</filterpriority>
        public override void Reset()
        {
            this.colorDialogForm = (IRadColorDialog)RadColorEditor.CreateColorDialogInstance();
        }

        /// <summary>
        /// Gets the instance of RadColorDialogForm, which incorporates various settings of the
        /// underlaying color selection Form and ColorSelector user control.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IRadColorDialog ColorDialogForm
        {
            get
            {
                return this.colorDialogForm;
            }
        }

        /// <summary>Gets or sets a value indicating whether control's elements are aligned
        /// to support locales using right-to-left fonts.</summary>
        /// <returns>One of the <see cref="T:System.Windows.Forms.RightToLeft"/> values.
        /// The default is <see cref="F:System.Windows.Forms.RightToLeft.Inherit"/>.</returns>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The assigned
        /// value is not one of the <see cref="T:System.Windows.Forms.RightToLeft"/> values.
        /// </exception>
        [System.ComponentModel.LocalizableAttribute(true)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets a value indicating whether control's elements are aligned to support locales using right-to-left fonts.")]
        [System.ComponentModel.AmbientValueAttribute(0)]
        public virtual System.Windows.Forms.RightToLeft RightToLeft
        {
            get
            {
                return ((Form)this.ColorDialogForm).RightToLeft;
            }
            set
            {
                ((Form)this.ColorDialogForm).RightToLeft = value;
            }
        }

        /// <summary>
        /// Gets or sets the selected color. References to SelectedColor of <see cref="ColorDialogForm"/>.
        /// </summary>
        [DefaultValue("Red")]
        public Color SelectedColor
        {
            get { return this.colorDialogForm.SelectedColor; }
            set { this.colorDialogForm.SelectedColor = value; }
        }

        /// <summary>
        /// Gets or sets the selected color. References to SelectedColor of <see cref="ColorDialogForm"/>.
        /// </summary>
        [DefaultValue("Red")]
        public HslColor SelectedHslColor
        {
            get { return this.colorDialogForm.SelectedHslColor; }
            set { this.colorDialogForm.SelectedHslColor = value; }
        }

        /// <summary>
        /// Gets the user-defined colors. References to CustomColors of <see cref="ColorDialogForm"/>.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color[] CustomColors
        {
            get { return this.colorDialogForm.CustomColors; }
        }

        ///<summary>
        ///Shows modal dialog box.
        ///</summary>
        ///
        ///<returns>
        ///true if the dialog box was successfully run; otherwise, false.
        ///</returns>
        ///
        ///<param name="hwndOwner">A value that represents the window handle of the owner window for the common dialog box. </param>
        protected override bool RunDialog(IntPtr hwndOwner)
        {            
            //form.CreateControl();
            //NativeMethods.SetParent(new HandleRef(this, form.Handle), new HandleRef(this, hwndOwner));

            return ((Form)colorDialogForm).ShowDialog(NativeWindow.FromHandle(hwndOwner)) == DialogResult.OK;
        }
    }
}
