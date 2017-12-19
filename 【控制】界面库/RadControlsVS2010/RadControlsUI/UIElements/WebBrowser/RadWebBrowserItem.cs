using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// RadWebBrowserItem hosts WebBrowser control to allow using it in the TPF structure.
    /// </summary>
    [ToolboxItem(false), ComVisible(false)]
    public class RadWebBrowserItem : RadHostItem
    {
        #region Static EventKeys and Constructor

        private static readonly object DocumentCompletedEventKey;
        private static readonly object FileDownloadEventKey;
        private static readonly object NavigatedEventKey;
        private static readonly object NavigatingEventKey;
        private static readonly object NewWindowEventKey;
        private static readonly object PreviewKeyDownEventKey;
        private static readonly object ProgressChangedEventKey;
        private static readonly object SystemColorsChangedEventKey;

        static RadWebBrowserItem()
        {
            DocumentCompletedEventKey = new object();
            FileDownloadEventKey = new object();
            NavigatedEventKey = new object();
            NavigatingEventKey = new object();
            NewWindowEventKey = new object();
            PreviewKeyDownEventKey = new object();
            ProgressChangedEventKey = new object();
            SystemColorsChangedEventKey = new object();
        }

        #endregion

        #region RadProperties

        #endregion

        #region Constructors & Initialization

        public RadWebBrowserItem()
            : base(new RadWebBrowserBase())
        {
            WebBrowser browser = this.WebBrowserControl;
            // Events fired
            browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(WebBrowserControl_DocumentCompleted);
            browser.FileDownload += new EventHandler(WebBrowserControl_FileDownload);
            browser.Navigating += new WebBrowserNavigatingEventHandler(WebBrowserControl_Navigating);
            browser.Navigated += new WebBrowserNavigatedEventHandler(WebBrowserControl_Navigated);
            browser.NewWindow += new CancelEventHandler(WebBrowserControl_NewWindow);
            browser.PreviewKeyDown += new PreviewKeyDownEventHandler(WebBrowserControl_PreviewKeyDown);
            browser.ProgressChanged += new WebBrowserProgressChangedEventHandler(WebBrowserControl_ProgressChanged);
            browser.SystemColorsChanged += new EventHandler(WebBrowserControl_SystemColorsChanged);

            // Events on property changed
            browser.CausesValidationChanged += new EventHandler(WebBrowserControl_CausesValidationChanged);
            browser.CanGoBackChanged += new EventHandler(WebBrowserControl_CanGoBackChanged);
            browser.CanGoForwardChanged += new EventHandler(WebBrowserControl_CanGoForwardChanged);
            browser.ContextMenuChanged += new EventHandler(WebBrowserControl_ContextMenuChanged);
            browser.ContextMenuStripChanged += new EventHandler(WebBrowserControl_ContextMenuStripChanged);
            browser.DockChanged += new EventHandler(WebBrowserControl_DockChanged);
            browser.DocumentTitleChanged += new EventHandler(WebBrowserControl_DocumentTitleChanged);
            browser.EncryptionLevelChanged += new EventHandler(WebBrowserControl_EncryptionLevelChanged);
            browser.StatusTextChanged += new EventHandler(WebBrowserControl_StatusTextChanged);
        }

        #endregion

        #region Properties

        public WebBrowser WebBrowserControl
        {
            get { return (WebBrowser)this.HostedControl; }
        }

        /// <summary>
        /// Gets or Sets the Url that is to be browsed.
        /// </summary>
        /// <seealso cref="WebBrowser.Url"/>
        public System.Uri Url
        {
            get { return this.WebBrowserControl.Url; }
            set
            {
                if (this.WebBrowserControl.Url != value)
                {
                    this.WebBrowserControl.Url = value;
                }
            }
        }

        /// <summary>
        /// Gets or Sets the HTML document content.
        /// </summary>
        /// <seealso cref="WebBrowser.DocumentText"/>
        public string DocumentText
        {
            get { return this.WebBrowserControl.DocumentText; }
            set { this.WebBrowserControl.DocumentText = value; }
        }

        /// <summary>
        /// Gets the HTML document title content.
        /// </summary>
        /// <seealso cref="WebBrowser.DocumentTitle"/>
        public string DocumentTitle
        {
            get { return this.WebBrowserControl.DocumentTitle; }
        }

        #endregion

        #region WebBrowserControl properties changed event handlers

        private void WebBrowserControl_CausesValidationChanged(object sender, EventArgs e)
        {
            this.OnNotifyPropertyChanged("CausesValidation");
        }

        private void WebBrowserControl_CanGoBackChanged(object sender, EventArgs e)
        {
            this.OnNotifyPropertyChanged("CanGoBack");
        }

        private void WebBrowserControl_CanGoForwardChanged(object sender, EventArgs e)
        {
            this.OnNotifyPropertyChanged("CanGoForward");
        }

        private void WebBrowserControl_ContextMenuChanged(object sender, EventArgs e)
        {
            this.OnNotifyPropertyChanged("ContextMenu");
        }

        private void WebBrowserControl_ContextMenuStripChanged(object sender, EventArgs e)
        {
            this.OnNotifyPropertyChanged("ContextMenuStrip");
        }

        private void WebBrowserControl_DockChanged(object sender, EventArgs e)
        {
            this.OnNotifyPropertyChanged("Dock");
        }

        private void WebBrowserControl_DocumentTitleChanged(object sender, EventArgs e)
        {
            this.OnNotifyPropertyChanged("DocumentTitle");
        }

        private void WebBrowserControl_EncryptionLevelChanged(object sender, EventArgs e)
        {
            this.OnNotifyPropertyChanged("EncryptionLevel");
        }

        private void WebBrowserControl_StatusTextChanged(object sender, EventArgs e)
        {
            this.OnNotifyPropertyChanged("StatusText");
        }

        #endregion

        #region WebBrowserControl event handlers

        private void WebBrowserControl_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.OnDocumentCompleted(e);
        }

        private void WebBrowserControl_FileDownload(object sender, EventArgs e)
        {
            this.OnFileDownload(e);
        }

        private void WebBrowserControl_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            this.OnNavigating(e);
        }

        private void WebBrowserControl_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            this.OnNavigated(e);
        }

        private void WebBrowserControl_NewWindow(object sender, CancelEventArgs e)
        {
            this.OnNewWindow(e);
        }

        private void WebBrowserControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            this.OnPreviewKeyDown(e);
        }

        private void WebBrowserControl_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            this.OnProgressChanged(e);
        }

        private void WebBrowserControl_SystemColorsChanged(object sender, EventArgs e)
        {
            this.OnSystemColorsChanged(e);
        }

        private void WebBrowserControl_Validated(object sender, EventArgs e)
        {
            this.OnValidated(e);
        }

        private void WebBrowserControl_Validating(object sender, CancelEventArgs e)
        {
            this.OnValidating(e);
        }

        #endregion

        #region Events (public event ... )

        /// <summary>
        /// Fires when document loading has completed.
        /// </summary>
        /// <seealso cref="WebBrowser.DocumentCompleted"/>
        public event WebBrowserDocumentCompletedEventHandler DocumentCompleted
        {
            add { this.Events.AddHandler(DocumentCompletedEventKey, value); }
            remove { this.Events.RemoveHandler(DocumentCompletedEventKey, value); }
        }

        /// <summary>
        /// Fires when file has been downloaded
        /// </summary>
        /// <seealso cref="WebBrowser.FileDownload"/>
        public event EventHandler FileDownload
        {
            add { this.Events.AddHandler(FileDownloadEventKey, value); }
            remove { this.Events.RemoveHandler(FileDownloadEventKey, value); }
        }

        /// <summary>
        /// Fires when the browser has navigated to a new document and has begun loading it.
        /// </summary>
        /// <seealso cref="WebBrowser.Navigated"/>
        /// <seealso cref="Navigating"/>
        public event WebBrowserNavigatedEventHandler Navigated
        {
            add { this.Events.AddHandler(NavigatedEventKey, value); }
            remove { this.Events.RemoveHandler(NavigatedEventKey, value); }
        }

        /// <summary>
        /// Fires before the browser navigates to a new document
        /// </summary>
        /// <seealso cref="WebBrowser.Navigating"/>
        /// <seealso cref="Navigated"/>
        public event WebBrowserNavigatingEventHandler Navigating
        {
            add { this.Events.AddHandler(NavigatingEventKey, value); }
            remove { this.Events.RemoveHandler(NavigatingEventKey, value); }
        }

        /// <summary>
        /// Fires before new browser window is opened
        /// </summary>
        /// <seealso cref="WebBrowser.NewWindow"/>
        public event CancelEventHandler NewWindow
        {
            add { this.Events.AddHandler(NewWindowEventKey, value); }
            remove { this.Events.RemoveHandler(NewWindowEventKey, value); }
        }

        /// <summary>
        /// Fires before System.Windows.Forms.Control.KeyDown event when a key is pressed while focus is on this control.
        /// </summary>
        public event PreviewKeyDownEventHandler PreviewKeyDown
        {
            add { this.Events.AddHandler(PreviewKeyDownEventKey, value); }
            remove { this.Events.RemoveHandler(PreviewKeyDownEventKey, value); }
        }

        /// <summary>
        /// Fires when the RadWebBrowserItem has updated information on the download progress of a document it is navigating to.
        /// </summary>
        /// <seealso cref="WebBrowser.ProgressChanged"/>
        public event WebBrowserProgressChangedEventHandler ProgressChanged
        {
            add { this.Events.AddHandler(ProgressChangedEventKey, value); }
            remove { this.Events.RemoveHandler(ProgressChangedEventKey, value); }
        }

        /// <summary>
        /// Fires when the System Colors change
        /// </summary>
        public event EventHandler SystemColorsChanged
        {
            add { this.Events.AddHandler(SystemColorsChangedEventKey, value); }
            remove { this.Events.RemoveHandler(SystemColorsChangedEventKey, value); }
        }

        #endregion

        #region Events (protected virtual void On...)

        protected virtual void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowserDocumentCompletedEventHandler handler = (WebBrowserDocumentCompletedEventHandler)this.Events[DocumentCompletedEventKey];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnFileDownload(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[FileDownloadEventKey];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnNavigated(WebBrowserNavigatedEventArgs e)
        {
            WebBrowserNavigatedEventHandler handler = (WebBrowserNavigatedEventHandler)this.Events[NavigatedEventKey];
            if (handler != null)
            {
                handler(this, e);

            }
        }

        protected virtual void OnNavigating(WebBrowserNavigatingEventArgs e)
        {
            WebBrowserNavigatingEventHandler handler = (WebBrowserNavigatingEventHandler)this.Events[NavigatingEventKey];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnNewWindow(CancelEventArgs e)
        {
            CancelEventHandler handler = (CancelEventHandler)this.Events[NewWindowEventKey];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            PreviewKeyDownEventHandler handler = (PreviewKeyDownEventHandler)this.Events[PreviewKeyDownEventKey];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnProgressChanged(WebBrowserProgressChangedEventArgs e)
        {
            WebBrowserProgressChangedEventHandler handler = (WebBrowserProgressChangedEventHandler)this.Events[ProgressChangedEventKey];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnSystemColorsChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[SystemColorsChangedEventKey];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion
    }
}
