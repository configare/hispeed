using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Design;
using System.Windows.Forms;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Represents a special container, which is used to store document tab strips.
    /// </summary>
    [ToolboxItem(false)]
    [Designer("Telerik.WinControls.UI.Design.DocumentContainerDesigner, Telerik.WinControls.UI.Design, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e")]
    public class DocumentContainer : RadSplitContainer
    {
        #region Constructor

        static DocumentContainer()
        {
            //ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, "Telerik.WinControls.UI.Docking.Resources.ControlDefault.DocumentContainer.xml");
            new Telerik.WinControls.Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_Docking_DocumentContainer().DeserializeTheme();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DocumentContainer()
        {
            this.SizeInfo.SizeMode = SplitPanelSizeMode.Fill;
            this.Behavior.BitmapRepository.DisableBitmapCache = true;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentContainer">DocumentContainer</see>
        /// class and associates it with the provided <see cref="RadDock">RadDock</see> instance.
        /// </summary>
        /// <param name="dock"></param>
        public DocumentContainer(RadDock dock) : this()
        {
            this.DockManager = dock;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected internal override bool CanSelectAtDesignTime()
        {
            //we do not want to be selected at design-time.
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal override void UpdateCollapsed()
        {
            if (this.dockManager != null)
            {
                this.Collapsed = !this.dockManager.MainDocumentContainerVisible;
            }
            else
            {
                base.UpdateCollapsed();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dockManager = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (e.Control is ToolTabStrip)
            {
                throw new InvalidOperationException("Could not add ToolTabStrip to a DocumentContainer.");
            }
        }

        #endregion

        #region Properties

        public override string ThemeClassName
        {
            get
            {
                return typeof(DocumentContainer).FullName;
            }
            set
            {
                base.ThemeClassName = value;
            }
        }

        /// <summary>
        /// Gets the RadDock, where this container resides.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadDock DockManager
        {
            get
            {
                return this.dockManager;
            }
            internal set
            {
                if (this.dockManager == value)
                {
                    return;
                }

                this.dockManager = value;
                this.UpdateCollapsed();
                if (this.dockManager != null && this.setMainDocumentContainer)
                {
                    this.setMainDocumentContainer = false;
                    this.dockManager.MainDocumentContainer = this;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is the main document container for the associated RadDock.
        /// This property is used primarily for serialization purposes and is not intended to be used directly by code.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsMainDocumentContainer
        {
            get
            {
                if (this.dockManager != null)
                {
                    return this == this.dockManager.MainDocumentContainer;
                }

                return this.setMainDocumentContainer;
            }
            set
            {
                if (value)
                {
                    if (this.DockManager != null)
                    {
                        this.DockManager.MainDocumentContainer = this;
                    }
                }

                this.setMainDocumentContainer = value;
            }
        }

        #endregion

        #region Fields

        private RadDock dockManager;
        private bool setMainDocumentContainer;

        #endregion
    }
}
