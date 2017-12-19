using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents the custom editor
    /// shown when the FadeAnimationType of the popup 
    /// is adjusted in the Visual Studio Designer.
    /// </summary>
    public class FadeAnimationTypeEditor : UITypeEditor
    {
        #region Nested

        private class FadeAnimationTypeEditorUI : Control
        {
            #region Fields

            private RadToggleButton fadeInToggleButton;
            private RadToggleButton fadeOutToggleButton;

            private FadeAnimationType result;

            #endregion

            #region Constructor

            /// <summary>
            /// Creates an instance of the FadeAnimationTypeEditorUI class.
            /// This class represents the control used to set the
            /// FadeAnimationType property while in the Visual Studio
            /// Designer.
            /// </summary>
            /// <param name="input">The inital value of the property.</param>
            internal FadeAnimationTypeEditorUI(FadeAnimationType input)
            {
                this.result = input;
                this.InitializeComponent();
            }

            private void InitializeComponent()
            {
                this.fadeInToggleButton = new RadToggleButton();
                this.fadeOutToggleButton = new RadToggleButton();

                this.fadeInToggleButton.Text = "Fade In";
                this.fadeOutToggleButton.Text = "Fade Out";

                this.Size = new System.Drawing.Size(80, 80);
                this.fadeInToggleButton.Dock = DockStyle.Top;
                this.fadeOutToggleButton.Dock = DockStyle.Bottom;
                this.fadeInToggleButton.Height = 40;
                this.fadeOutToggleButton.Height = 40;

                this.fadeInToggleButton.ToggleState = ((this.result & FadeAnimationType.FadeIn) != 0)
                    ? Telerik.WinControls.Enumerations.ToggleState.On : Telerik.WinControls.Enumerations.ToggleState.Off;

                this.fadeOutToggleButton.ToggleState = ((this.result & FadeAnimationType.FadeOut) != 0)
                    ? Telerik.WinControls.Enumerations.ToggleState.On : Telerik.WinControls.Enumerations.ToggleState.Off;

                this.fadeInToggleButton.ToggleStateChanged += new StateChangedEventHandler(fadeInToggleButton_ToggleStateChanged);
                this.fadeOutToggleButton.ToggleStateChanged += new StateChangedEventHandler(fadeOutToggleButton_ToggleStateChanged);

                this.Controls.Add(this.fadeInToggleButton);
                this.Controls.Add(this.fadeOutToggleButton);
            }

           

            #endregion

            #region Properties

            /// <summary>
            /// Gets the result of the editor execution.
            /// </summary>
            internal FadeAnimationType Result
            {
                get
                {
                    return this.result;
                }
            }

            #endregion

            #region Events

            private void fadeOutToggleButton_ToggleStateChanged(object sender, StateChangedEventArgs args)
            {
                switch(args.ToggleState)
                {
                    case Telerik.WinControls.Enumerations.ToggleState.On:
                        {
                            this.result |= FadeAnimationType.FadeOut;
                            break;
                        }
                    default:
                        {
                            this.result &= ~FadeAnimationType.FadeOut;
                            break;
                        }
                }
            }

            private void fadeInToggleButton_ToggleStateChanged(object sender, StateChangedEventArgs args)
            {
                switch (args.ToggleState)
                {
                    case Telerik.WinControls.Enumerations.ToggleState.On:
                        {
                            this.result |= FadeAnimationType.FadeIn;
                            break;
                        }
                    default:
                        {
                            this.result &= ~FadeAnimationType.FadeIn;
                            break;
                        }
                }
            }

            #endregion
        }

        #endregion

        #region Fields

        private FadeAnimationTypeEditorUI fadeAnimationTypeEditorUI;

        #endregion


        #region Methods

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc == null)
                {
                    return value;
                }
                if (this.fadeAnimationTypeEditorUI == null)
                {
                    this.fadeAnimationTypeEditorUI = new FadeAnimationTypeEditorUI((FadeAnimationType)value);
                }

                edSvc.DropDownControl(this.fadeAnimationTypeEditorUI);
                value = this.fadeAnimationTypeEditorUI.Result;
            }

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            base.PaintValue(e);
        }

        #endregion
    }
}
