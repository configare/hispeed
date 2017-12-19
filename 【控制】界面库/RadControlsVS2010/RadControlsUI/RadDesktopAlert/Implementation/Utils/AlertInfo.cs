using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class encapsulates information needed for displaying a <see cref="RadDesktopAlert"/>.
    /// The class contains caption text, content text, content image and a collection of buttons.
    /// </summary>
    public class AlertInfo
    {
        #region Fields

        private RadItemCollection alertButtons;
        private string captionText;
        private string contentText;
        private Image contentImage;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates an instance of the <see cref="AlertInfo"/>class
        /// with specified content text.
        /// </summary>
        /// <param name="contentText">The text which will be displayed as a content of the <see cref="RadDesktopAlert"/></param>
        public AlertInfo(string contentText)
        {
            this.contentText = contentText;
        }

        /// <summary>
        /// Creates an instance of the <see cref="AlertInfo"/> class
        /// with specified content text and caption text.
        /// </summary>
        /// <param name="contentText">The text which will be displayed as a content of the <see cref="RadDesktopAlert"/></param>
        /// <param name="captionText">The text which will be displayed as a caption of the <see cref="RadDesktopAlert"/></param>
        public AlertInfo(string contentText, string captionText) : this(contentText)
        {
            this.captionText = captionText;
        }

        /// <summary>
        /// Creates an instance of the <see cref="AlertInfo"/> class
        /// with specified content text, caption text and content image.
        /// </summary>
        /// <param name="contentText">The text which will be displayed as a content of the <see cref="RadDesktopAlert"/></param>
        /// <param name="captionText">The text which will be displayed as a caption of the <see cref="RadDesktopAlert"/></param>
        /// <param name="alertImage">An instance of the <see cref="Image"/>class that will be displayed as a content image of the <see cref="RadDesktopAlert"/></param>
        public AlertInfo(string contentText, string captionText, Image alertImage)
            : this(contentText, captionText)
        {
            this.contentImage = alertImage;
        }

        /// <summary>
        /// Creates an instance of the <see cref="AlertInfo"/> class
        /// with specified content text, caption text, content image and a collection of buttons.
        /// </summary>
        /// <param name="contentText">The text which will be displayed as a content of the <see cref="RadDesktopAlert"/></param>
        /// <param name="captionText">The text which will be displayed as a caption of the <see cref="RadDesktopAlert"/></param>
        /// <param name="alertImage">An instance of the <see cref="Image"/>class that will be displayed as a content image of the <see cref="RadDesktopAlert"/></param>
        /// <param name="alertButtons">An instance of the <see cref="RadItemCollection"/>class that holds the buttons which will be displayed in the <see cref="RadDesktopAlert"/></param>
        public AlertInfo(string contentText, string captionText, Image alertImage, RadItemCollection alertButtons)
            : this(contentText, captionText, alertImage)
        {
            this.alertButtons = alertButtons;
        }


        #endregion
    }
}
