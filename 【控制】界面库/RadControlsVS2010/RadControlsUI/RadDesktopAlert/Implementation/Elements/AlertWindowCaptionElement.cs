using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents the caption of a <see cref="RadDesktopAlert"/>.
    /// It contains caption grip which is used to move the alert window, close
    /// button and options drop-down button.
    /// </summary>
    public class AlertWindowCaptionElement : RadElement
    {
        #region Fields

        private AlertWindowCaptionGrip captionGrip;
        private AlertWindowTextAndSystemButtonsElement textAndButtonsElement;
        private StackLayoutPanel mainLayoutPanel;

        #endregion

        #region Properties

        /// <summary>
        /// Gets an instance of the <see cref="AlertWindowCaptionGrip"/>class
        /// that represents the part of a <see cref="RadDesktopAlert"/>that
        /// can be used to move the component on the screen.
        /// </summary>
        public AlertWindowCaptionGrip CaptionGrip
        {
            get
            {
                return this.captionGrip;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="AlertWindowTextAndSystemButtonsElement"/>class
        /// that represents the part of a <see cref="RadDesktopAlert"/>that contains
        /// the text and the system buttons.
        /// </summary>
        public AlertWindowTextAndSystemButtonsElement TextAndButtonsElement
        {
            get
            {
                return this.textAndButtonsElement;
            }
        }

        #endregion

        #region Methods

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.captionGrip = new AlertWindowCaptionGrip();
            this.textAndButtonsElement = new AlertWindowTextAndSystemButtonsElement();
            this.mainLayoutPanel = new StackLayoutPanel();
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.mainLayoutPanel.Class = "AlertWindowCaptionLayoutPanel";
            this.mainLayoutPanel.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.mainLayoutPanel.Children.Add(this.captionGrip);
            this.mainLayoutPanel.Children.Add(this.textAndButtonsElement);
            this.Children.Add(this.mainLayoutPanel);
        }

        #endregion
    }
}
