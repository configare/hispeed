using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using Telerik.WinControls.Keyboard;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Encapsulates all the settings that control the appearance and behavior of the QuickNavigator for a RadDock.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class QuickNavigatorSettings : RadDockObject
    {
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public QuickNavigatorSettings()
        {
            this.listItemType = typeof(QuickNavigatorListItem);
            this.showHeader = true;
            this.showFooter = true;
            this.showToolPanes = true;
            this.showDocumentPanes = true;
            this.showPreview = true;
            this.enabled = true;
            this.dropShadow = true;
            this.forceSnapshot = true;
            this.toolPaneColumns = 0;
            this.documentPaneColumns = 0;
            this.listItemSize = DefaultListItemSize;
            this.previewSize = DefaultPreviewSize;
            this.headerPadding = new Padding(4);
            this.footerPadding = new Padding(4);
            this.itemsPerColumn = 20;
            this.columnSpacing = 5;
            this.documentPaneListHeader = DefaultDocumentPaneListHeader;
            this.toolPaneListHeader = DefaultToolPaneListHeader;
            this.displayPosition = QuickNavigatorDisplayPosition.CenterDockManager;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines the display position of the QuickNavigator.
        /// </summary>
        [DefaultValue(QuickNavigatorDisplayPosition.CenterDockManager)]
        [Description("Determines the display position of the QuickNavigator.")]
        public QuickNavigatorDisplayPosition DisplayPosition
        {
            get
            {
                return this.displayPosition;
            }
            set
            {
                if (this.displayPosition == value)
                {
                    return;
                }

                this.displayPosition = value;
                this.OnPropertyChanged("DisplayPosition");
            }
        }

        /// <summary>
        /// Gets or sets the Type used to create items in the QuickNavigator's lists.
        /// This should be a valid QuickNavigatorListItem assignable type.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Type ListItemType
        {
            get
            {
                return this.listItemType;
            }
            set
            {
                if (!value.IsAssignableFrom(typeof(QuickNavigatorListItem)))
                {
                    throw new ArgumentException("Allowed types for a QuickNavigator ListItem are QuickNavigatorListItem descendants only.");
                }

                this.listItemType = value;
                this.OnPropertyChanged("ListItemType");
            }
        }

        /// <summary>
        /// Gets or sets the padding to be applied on the header part of the navigator.
        /// </summary>
        [Description("Gets or sets the padding to be applied on the header part of the navigator.")]
        [DefaultValue(typeof(Padding), "4,4,4,4")]
        public Padding HeaderPadding
        {
            get
            {
                return this.headerPadding;
            }
            set
            {
                if (this.headerPadding == value)
                {
                    return;
                }

                this.headerPadding = value;
                this.OnPropertyChanged("HeaderPadding");
            }
        }

        /// <summary>
        /// Gets or sets the padding to be applied on the footer part of the navigator.
        /// </summary>
        [Description("Gets or sets the padding to be applied on the footer part of the navigator.")]
        [DefaultValue(typeof(Padding), "4,4,4,4")]
        public Padding FooterPadding
        {
            get
            {
                return this.footerPadding;
            }
            set
            {
                if (this.footerPadding == value)
                {
                    return;
                }

                this.footerPadding = value;
                this.OnPropertyChanged("FooterPadding");
            }
        }

        /// <summary>
        /// Determines whether the navigator is enabled (displayed when the shortcut combination is triggered).
        /// </summary>
        [DefaultValue(true)]
        [Description("Determines whether the navigator is enabled (displayed when the shortcut combination is triggered).")]
        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                if (this.enabled == value)
                {
                    return;
                }

                this.enabled = value;
                this.OnPropertyChanged("Enabled");
            }
        }

        /// <summary>
        /// Determines whether the header area of the QuickNavigator is visible.
        /// </summary>
        [DefaultValue(true)]
        [Description("Determines whether the header area of the QuickNavigator is visible.")]
        public bool ShowHeader
        {
            get
            {
                return this.showHeader;
            }
            set
            {
                if (this.showHeader == value)
                {
                    return;
                }

                this.showHeader = value;
                this.OnPropertyChanged("ShowHeader");
            }
        }

        /// <summary>
        /// Determines whether the footer area of the QuickNavigator is visible.
        /// </summary>
        [DefaultValue(true)]
        [Description("Determines whether the footer area of the QuickNavigator is visible.")]
        public bool ShowFooter
        {
            get
            {
                return this.showFooter;
            }
            set
            {
                if (this.showFooter == value)
                {
                    return;
                }

                this.showFooter = value;
                this.OnPropertyChanged("ShowFooter");
            }
        }

        /// <summary>
        /// Determines whether the ToolWindow list is visible.
        /// </summary>
        [DefaultValue(true)]
        [Description("Determines whether the ToolWindow list is visible.")]
        public bool ShowToolPanes
        {
            get
            {
                return this.showToolPanes;
            }
            set
            {
                if (this.showToolPanes == value)
                {
                    return;
                }

                this.showToolPanes = value;
                this.OnPropertyChanged("ShowToolPanes");
            }
        }

        /// <summary>
        /// Determines whether the DocumentWindow list is visible.
        /// </summary>
        [DefaultValue(true)]
        [Description("Determines whether the DocumentWindow list is visible.")]
        public bool ShowDocumentPanes
        {
            get
            {
                return this.showDocumentPanes;
            }
            set
            {
                if (this.showDocumentPanes == value)
                {
                    return;
                }

                this.showDocumentPanes = value;
                this.OnPropertyChanged("ShowDocumentPanes");
            }
        }

        /// <summary>
        /// Determines whether the Preview element is visible.
        /// The Preview displays a snapshot of the selected DockWindow in either of the QuickNavigator's lists.
        /// </summary>
        [DefaultValue(true)]
        [Description("Determines whether the Preview element is visible. The Preview displays a snapshot of the selected DockWindow in either of the QuickNavigator's lists.")]
        public bool ShowPreview
        {
            get
            {
                return this.showPreview;
            }
            set
            {
                if (this.showPreview == value)
                {
                    return;
                }

                this.showPreview = value;
                this.OnPropertyChanged("ShowPreview");
            }
        }

        /// <summary>
        /// Determines whether the Form that hosts the QuickNavigator will drop shadow or not.
        /// </summary>
        [DefaultValue(true)]
        [Description("Determines whether the Form that hosts the QuickNavigator will drop shadow or not.")]
        public bool DropShadow
        {
            get
            {
                return this.dropShadow;
            }
            set
            {
                if (this.dropShadow == value)
                {
                    return;
                }

                this.dropShadow = value;
                this.OnPropertyChanged("DropShadow");
            }
        }

        /// <summary>
        /// Determines the number of columns in the ToolWindow list.
        /// Defaults to zero which specifies that the number of columns is automatically calculated.
        /// </summary>
        [DefaultValue(0)]
        [Description("Determines the number of columns in the ToolWindow list. Defaults to zero which specifies that the number of columns is automatically calculated.")]
        public int ToolPaneColumns
        {
            get
            {
                return this.toolPaneColumns;
            }
            set
            {
                value = Math.Max(0, value);
                if (this.toolPaneColumns == value)
                {
                    return;
                }

                this.toolPaneColumns = value;
                this.OnPropertyChanged("ToolPaneColumns");
            }
        }

        /// <summary>
        /// Determines the number of columns in the DocumentWindow list.
        /// Defaults to zero which specifies that the number of columns is automatically calculated.
        /// </summary>
        [DefaultValue(0)]
        [Description("Determines the number of columns in the DocumentWindow list. Defaults to zero which specifies that the number of columns is automatically calculated.")]
        public int DocumentPaneColumns
        {
            get
            {
                return this.documentPaneColumns;
            }
            set
            {
                value = Math.Max(0, value);
                if (this.documentPaneColumns == value)
                {
                    return;
                }

                this.documentPaneColumns = value;
                this.OnPropertyChanged("DocumentPaneColumns");
            }
        }

        /// <summary>
        /// Gets or sets the minimum size of the Preview element.
        /// </summary>
        [Description("Gets or sets the minimum size of the Preview element.")]
        public Size PreviewSize
        {
            get
            {
                return this.previewSize;
            }
            set
            {
                if (this.previewSize == value)
                {
                    return;
                }

                this.previewSize = value;
                this.OnPropertyChanged("PreviewMinSize");
            }
        }

        bool ShouldSerializePreviewSize()
        {
            return this.previewSize != DefaultPreviewSize;
        }

        /// <summary>
        /// Gets or sets the maximum size the navigator will occupy on the screen.
        /// Defaults to the screen bounds and will always be truncated
        /// to that value if the provided custom size exceeds screen's bounds.
        /// </summary>
        [Description("Gets or sets the maximum size the navigator will occupy on the screen. Defaults to the screen bounds and will always be truncated to that value if the provided custom one exceeds screen's bounds.")]
        public Size MaxSize
        {
            get
            {
                return this.maxSize;
            }
            set
            {
                value.Width = Math.Max(0, value.Width);
                value.Height = Math.Max(0, value.Height);

                if (this.maxSize == value)
                {
                    return;
                }

                this.maxSize = value;
                this.OnPropertyChanged("MaxSize");
            }
        }

        bool ShouldSerializeMaxSize()
        {
            return this.maxSize != Size.Empty;
        }

        /// <summary>
        /// Gets or sets the size of a single item in the pane lists.
        /// </summary>
        [Description("Gets or sets the size of a single item in the pane lists.")]
        public Size ListItemSize
        {
            get
            {
                return this.listItemSize;
            }
            set
            {
                if (this.listItemSize == value)
                {
                    return;
                }

                this.listItemSize = value;
                this.OnPropertyChanged("ListItemSize");
            }
        }

        bool ShouldSerializeListItemSize()
        {
            return this.listItemSize != DefaultListItemSize;
        }

        /// <summary>
        /// Gets or sets the number of items per column in the dock pane lists.
        /// </summary>
        [DefaultValue(20)]
        [Description("Gets or sets the number of items per column in the dock pane lists.")]
        public int ItemsPerColumn
        {
            get
            {
                return this.itemsPerColumn;
            }
            set
            {
                value = Math.Max(1, value);
                if (this.itemsPerColumn == value)
                {
                    return;
                }

                this.itemsPerColumn = value;
                this.OnPropertyChanged("ItemsPerColumn");
            }
        }

        /// <summary>
        /// Gets or sets the spacing between columns in panes lists.
        /// </summary>
        [DefaultValue(5)]
        [Description("Gets or sets the spacing between columns in panes lists.")]
        public int ColumnSpacing
        {
            get
            {
                return this.columnSpacing;
            }
            set
            {
                value = Math.Max(0, value);
                if (this.columnSpacing == value)
                {
                    return;
                }

                this.columnSpacing = value;
                this.OnPropertyChanged("ColumnSpacing");
            }
        }

        /// <summary>
        /// Gets or sets the text for the header of the ToolWindow list.
        /// </summary>
        [DefaultValue(DefaultToolPaneListHeader)]
        [Description("Gets or sets the text for the header of the ToolWindow list.")]
        public string ToolPaneListHeader
        {
            get
            {
                return this.toolPaneListHeader;
            }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                if (this.toolPaneListHeader == value)
                {
                    return;
                }

                this.toolPaneListHeader = value;
                this.OnPropertyChanged("ToolPaneListHeader");
            }
        }

        /// <summary>
        /// Gets or sets the text for the header of the DocumentWindow list.
        /// </summary>
        [DefaultValue(DefaultDocumentPaneListHeader)]
        [Description("Gets or sets the text for the header of the DocumentWindow list.")]
        public string DocumentPaneListHeader
        {
            get
            {
                return this.documentPaneListHeader;
            }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                if (this.documentPaneListHeader == value)
                {
                    return;
                }

                this.documentPaneListHeader = value;
                this.OnPropertyChanged("DocumentPaneListHeader");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the QuickNavigator will try to
        /// update the selected DockWindow's bounds, so that a preview snapshot may be taken.
        /// </summary>
        [DefaultValue(true)]
        [Description("Gets or sets a value indicating whether the QuickNavigator will try to update the selected DockWindow's bounds, so that a preview snapshot may be taken.")]
        public bool ForceSnapshot
        {
            get
            {
                return this.forceSnapshot;
            }
            set
            {
                if (this.forceSnapshot == value)
                {
                    return;
                }

                this.forceSnapshot = value;
                this.OnPropertyChanged("ForceSnapshot");
            }
        }

        #endregion

        #region Fields

        private Type listItemType;
        private int itemsPerColumn;
        private int columnSpacing;
        private int toolPaneColumns;
        private int documentPaneColumns;
        private Padding headerPadding;
        private Padding footerPadding;
        private bool showHeader;
        private bool showFooter;
        private bool showToolPanes;
        private bool showDocumentPanes;
        private bool showPreview;
        private bool dropShadow;
        private bool enabled;
        private bool forceSnapshot;
        private Size listItemSize;
        private Size previewSize;
        private Size maxSize;
        private string toolPaneListHeader;
        private string documentPaneListHeader;
        private QuickNavigatorDisplayPosition displayPosition;

        #endregion

        #region Constants

        private const string DefaultToolPaneListHeader = "Active Tool Windows";
        private const string DefaultDocumentPaneListHeader = "Active Document Windows";

        private static readonly Size DefaultListItemSize = new Size(200, 24);
        private static readonly Size DefaultPreviewSize = new Size(200, 200);

        #endregion
    }
}
