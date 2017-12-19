using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     Represents a button control. The button control serves as a
    ///     <see cref="RadButtonElement">RadButtonElement Class</see> wrapper. All logic and
    ///     presentation features are implemented in a parallel hierarchy of objects. For this
    ///     reason, <see cref="RadButtonElement">RadButtonElement Class</see> may be nested in
    ///     any other telerik control, item, or element.
    /// </summary>
    [RadThemeDesignerData(typeof(RadButtonDesignTimeData))]
    [ToolboxItem(true)]
    [Description("Responds to user clicks.")]
    [DefaultBindingProperty("Text"), DefaultEvent("Click"), DefaultProperty("Text")]
    public class RadButton : RadButtonBase, IButtonControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the RadButton class.
        /// </summary>
        public RadButton()
        {
        }

        #endregion

        #region IButtonControl Members

        /// <summary>
        /// Gets or sets the DialogResult for this button.
        /// </summary>
        [DefaultValue(DialogResult.None)]
        public DialogResult DialogResult
        {
            get
            {
                return ((IButtonControl)this.ButtonElement).DialogResult;
            }
            set
            {
                ((IButtonControl)this.ButtonElement).DialogResult = value;
            }
        }

        public void NotifyDefault(bool value)
        {
            ((IButtonControl)this.ButtonElement).NotifyDefault(value);
        }



        #endregion
    }

    /// <exclude/>
    public class RadButtonDesignTimeData : RadControlDesignTimeData
    {
        public RadButtonDesignTimeData()
        { }

        public RadButtonDesignTimeData(string name)
            : base(name)
        { }

        public override ControlStyleBuilderInfoList GetThemeDesignedControls(System.Windows.Forms.Control previewSurface)
        {

            RadButton button = new RadButton();
            //button.AutoSize = false;
            button.Size = new Size(120, 65);

            button.Text = "RadButton";

            RadButton buttonStructure = new RadButton();
            //button.AutoSize = true;

            buttonStructure.Text = "RadButton";

            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(button, buttonStructure.RootElement);
            designed.MainElementClassName = typeof(RadButtonElement).FullName;

            ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

            res.Add(designed);

            return res;
        }
    }
}
