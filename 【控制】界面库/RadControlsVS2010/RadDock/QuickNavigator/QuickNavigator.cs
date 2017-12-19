using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.UI;
using Telerik.WinControls.Layout;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.Drawing.Imaging;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Design;
using System.ComponentModel;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Represents a predefined RadControl instance which hosts a QuickNavigatorElement.
    /// </summary>
    [RadThemeDesignerData(typeof(QuickNavigatorThemeDesignerData))]
    [ToolboxItem(false)]
    public class QuickNavigator : RadControl
    {
        #region Constructor

        /// <summary>
        /// Default constructor used by the VisualStyleBuilder to allow for control's styling.
        /// Internally mock dock windows are created and displayed in the navigator.
        /// </summary>
        public QuickNavigator()
        {
            this.CreateVisualStyleBuilderMocks();
            this.Initialize();
        }

        private void CreateVisualStyleBuilderMocks()
        {
            this.settings = new QuickNavigatorSettings();
            this.settings.ShowPreview = false;
            this.dockWindows = new SortedList<string, DockWindow>(5);

            //add 5 tool windows
            for (int i = 1; i < 6; i++)
            {
                ToolWindow tool = new ToolWindow("Tool Window " + i);
                this.dockWindows.Add(tool.Text, tool);
            }

            //add 5 document windows
            for (int i = 1; i < 6; i++)
            {
                DocumentWindow doc = new DocumentWindow("Document Window " + i);
                this.dockWindows.Add(doc.Text, doc);
            }

            this.activeWindow = this.dockWindows.Values[0];
        }

        internal QuickNavigator(QuickNavigatorSettings settings, SortedList<string, DockWindow> windows, DockWindow active)
        {
            this.settings = settings;
            this.activeWindow = active;
            this.dockWindows = windows;
        }

        internal QuickNavigator(RadDock dockManager) : 
            this(dockManager.QuickNavigatorSettings, dockManager.InnerList, dockManager.DocumentManager.ActiveDocument)
        {
            this.docWindowsZOrdered = dockManager.DocumentManager.DocumentArrayZOrdered;
            if (this.activeWindow == null)
            {
                this.activeWindow = dockManager.ActiveWindow;
            }
        }

        static QuickNavigator()
        {
            //ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, "Telerik.WinControls.UI.Docking.Resources.ControlDefault.QuickNavigator.xml");
            new Telerik.WinControls.Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_Docking_QuickNavigator().DeserializeTheme();
        }

        #endregion

        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        public override string ThemeClassName
        {
            get
            {
                return typeof(QuickNavigator).FullName;
            }
            set
            {
                base.ThemeClassName = value;
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
                if (this.previewBitmap != null)
                {
                    this.previewBitmap.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        protected override void CreateChildItems(RadElement parent)
        {
            this.navigatorElement = new QuickNavigatorElement();
            this.RootElement.Children.Add(this.navigatorElement);

            this.border = new BorderPrimitive();
            this.border.Class = "QuickNavigatorBorder";
            this.border.ZIndex = 10;
            this.RootElement.Children.Add(this.border);

            base.CreateChildItems(parent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootElement"></param>
        protected override void InitializeRootElement(RootRadElement rootElement)
        {
            base.InitializeRootElement(rootElement);
            rootElement.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            //attempt to change selection
            Point pt = new Point(e.X, e.Y);
            QuickNavigatorListItem item = this.ElementTree.GetElementAtPoint(pt) as QuickNavigatorListItem;
            if (item != null && item != this.selectedItem)
            {
                this.SelectItem(item);
            }

            //we are not above an item, proceed with the default implementation
            if (item == null)
            {
                base.OnMouseDown(e);
                return;
            }

            //close the owning form
            QuickNavigatorPopup owner = this.FindForm() as QuickNavigatorPopup;
            if (owner != null)
            {
                this.Capture = false;
                owner.Close();
            }
        }

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            if (element is RadScrollViewer)
            {
                return true;
            }

            return base.ControlDefinesThemeForElement(element);
        }

        #endregion

        #region Implementation

        #region Initialization

        /// <summary>
        /// Fills tool window and document lists.
        /// </summary>
        public void Initialize()
        {
            if (!this.IsLoaded)
            {
                this.LoadElementTree();
            }
            //fill two lists - one with the ToolWindows and one with the DocumentPanes
            this.BuildPanes();
            //get the available size
            //maximum is the screen we will be displayed onto
            this.CalculateAvailableSize();
            //calculate the tool and document pane columns
            this.CalculateColumns();
            //init navigator element
            this.InitLists();
            this.InitHeader();
            this.InitFooter();
            this.InitPreview();
        }

        private void CalculateColumns()
        {
            int availableWidth = availableSize.Width - this.navigatorElement.ToolWindowList.Parent.Margin.Horizontal;
            if (this.settings.ShowPreview)
            {
                availableWidth -= this.settings.PreviewSize.Width;
            }

            //calculate how many columns we may display on the screen
            int maxColumns = availableWidth / this.settings.ListItemSize.Width;
            if (maxColumns == 0)
            {
                return;
            }

            //subtract the spacing
            availableWidth -= (maxColumns - 1) * this.settings.ColumnSpacing;
            //re-calculate the column count again
            maxColumns = availableWidth / this.settings.ListItemSize.Width;

            //calculate desired columns for the tool panes
            this.toolPaneColumns = this.toolPanes.Count / this.settings.ItemsPerColumn;
            if (this.toolPanes.Count % this.settings.ItemsPerColumn > 0)
            {
                this.toolPaneColumns++;
            }
            //check whether we have custom columns specified
            if (settings.ToolPaneColumns > 0)
            {
                this.toolPaneColumns = Math.Min(this.toolPaneColumns, settings.ToolPaneColumns);
            }

            //calculate desired columns for the doc panes
            this.docPaneColumns = this.docPanes.Count / this.settings.ItemsPerColumn;
            if (this.docPanes.Count % this.settings.ItemsPerColumn > 0)
            {
                this.docPaneColumns++;
            }
            //check whether we have custom columns specified
            if (settings.DocumentPaneColumns > 0)
            {
                this.docPaneColumns = Math.Min(this.docPaneColumns, settings.DocumentPaneColumns);
            }

            //check whether we exceed maximum available columns
            if (this.toolPaneColumns + this.docPaneColumns <= maxColumns)
            {
                return;
            }

            //we need to clamp columns
            int exceed = (this.toolPaneColumns + this.docPaneColumns) - maxColumns;
            if (this.toolPaneColumns <= 1)
            {
                this.docPaneColumns = Math.Max(1, this.docPaneColumns - exceed);
                return;
            }
            if (this.docPaneColumns <= 1)
            {
                this.toolPaneColumns = Math.Max(1, this.toolPaneColumns - exceed);
                return;
            }

            //reduce column count
            while (exceed > 0)
            {
                if (this.toolPaneColumns > 1)
                {
                    this.toolPaneColumns--;
                    exceed--;
                }

                if (exceed > 0)
                {
                    this.docPaneColumns--;
                    exceed--;
                }

                if (this.toolPaneColumns == 1 && this.docPaneColumns == 1)
                {
                    break;
                }
            }
        }

        private void CalculateAvailableSize()
        {
            Rectangle screenBounds = Screen.FromControl(this).WorkingArea;
            this.availableSize = screenBounds.Size;

            Size customMaxSize = this.settings.MaxSize;
            if (customMaxSize.Width > 0)
            {
                this.availableSize.Width = Math.Min(customMaxSize.Width, this.availableSize.Width);
            }
            if (customMaxSize.Height > 0)
            {
                this.availableSize.Height = Math.Min(customMaxSize.Height, this.availableSize.Height);
            }
        }

        private void InitLists()
        {
            if (!this.settings.ShowToolPanes || this.toolPanes.Count == 0)
            {
                this.navigatorElement.ToolWindowList.Visibility = ElementVisibility.Collapsed;
            }
            else
            {
                this.InitToolPaneList();
                this.PopulateItems(this.navigatorElement.ToolWindowList, this.toolPanes);
            }

            if (!this.settings.ShowDocumentPanes || this.docPanes.Count == 0)
            {
                this.navigatorElement.DocumentWindowList.Visibility = ElementVisibility.Collapsed;
            }
            else
            {
                this.InitDocumentPaneList();
                this.PopulateItems(this.navigatorElement.DocumentWindowList, this.docPanes);
            }

            this.EnsureSelectedItem();
        }

        private void EnsureSelectedItem()
        {
            //we have already selected the currently active window
            if (this.selectedItem != null)
            {
                return;
            }

            if (this.settings.ShowToolPanes && this.toolPanes.Count > 0)
            {
                this.selectedItem = this.navigatorElement.ToolWindowList.ItemsPanel.Children[0] as QuickNavigatorListItem;
            }
            else if (this.settings.ShowDocumentPanes && this.docPanes.Count > 0)
            {
                this.selectedItem = this.navigatorElement.DocumentWindowList.ItemsPanel.Children[0] as QuickNavigatorListItem;
            }
        }

        private void PopulateItems(QuickNavigatorList list, IEnumerable panes)
        {
            WrapLayoutPanel itemsList = list.ItemsPanel;
            foreach (DockWindow pane in panes)
            {
                QuickNavigatorListItem item = this.CreateListItem(pane);
                item.Owner = list;
                itemsList.Children.Add(item);

                //detect the active item
                if (pane == this.activeWindow)
                {
                    this.navigatorElement.Header.Text = pane.Text;
                    this.selectedItem = item;
                }
            }
        }

        /// <summary>
        /// Separate tool from document panes to determine the size of each visual list.
        /// </summary>
        private void BuildPanes()
        {
            this.toolPanes = new LinkedList<DockWindow>();
            this.docPanes = new LinkedList<DockWindow>();

            foreach (KeyValuePair<string, DockWindow> pair in this.dockWindows)
            {
                DockWindow pane = pair.Value;
                if (pane.DockState == DockState.Hidden)
                {
                    continue;
                }

                switch (pane.DockState)
                {
                    case DockState.TabbedDocument:
                        //if we do not have z-ordered windows, add the documents sorted by their name
                        if (this.docWindowsZOrdered == null)
                        {
                            this.docPanes.AddLast(pane);
                        }
                        break;
                    default:
                        this.toolPanes.AddLast(pane);
                        break;
                }
            }

            if (this.docWindowsZOrdered != null)
            {
                foreach (DockWindow doc in this.docWindowsZOrdered)
                {
                    this.docPanes.AddLast(doc);
                }
            }
        }

        private void InitHeader()
        {
            if (!this.settings.ShowHeader)
            {
                this.navigatorElement.Header.Visibility = ElementVisibility.Collapsed;
            }
            else
            {
                RadLabelElement header = this.navigatorElement.Header;
                header.Padding = this.settings.HeaderPadding;

                this.headerSize = this.GetDesiredSize(header);
            }
        }

        private void InitToolPaneList()
        {
            QuickNavigatorList toolList = this.navigatorElement.ToolWindowList;
            toolList.Header.Text = this.settings.ToolPaneListHeader;
            Size itemSize = this.settings.ListItemSize;

            //calculate the size of the tool pane list
            int colItemCount = Math.Min(this.toolPanes.Count, this.settings.ItemsPerColumn);
            int toolListWidth = this.toolPaneColumns * itemSize.Width + (this.toolPaneColumns - 1) * settings.ColumnSpacing;
            int toolListHeight = colItemCount * this.settings.ListItemSize.Height;

            //adjust pane's settings
            toolList.ItemsPanel.ItemWidth = itemSize.Width;
            toolList.ItemsPanel.ItemHeight = itemSize.Height;
            //we need to set the Min-max size in order to achieve a fixed size
            Size minMax = new Size(toolListWidth, 0);
            toolList.MinSize = minMax;
            toolList.MaxSize = minMax;
            toolList.ItemsPanel.MaxSize = minMax;

            //retrieve the size of the header
            Size headerSize = this.GetDesiredSize(toolList.Header);
            toolListWidth = Math.Max(headerSize.Width, toolListWidth);
            toolListHeight += headerSize.Height;
            this.paneListHeaderHeight = headerSize.Height;
            if (headerSize.Height > 0)
            {
                toolListHeight += toolList.Header.Margin.Bottom;
            }

            //remeber the metrics
            this.toolPaneSize = new Size(toolListWidth, toolListHeight);

            //check whether we will display a horizontal scrollbar
            int desiredColumns = this.toolPanes.Count / this.settings.ItemsPerColumn;
            if (this.toolPanes.Count % this.settings.ItemsPerColumn > 0)
            {
                desiredColumns++;
            }

            if (desiredColumns > this.toolPaneColumns)
            {
                this.toolPaneSize.Height += SystemInformation.HorizontalScrollBarHeight;
            }
        }

        private void InitDocumentPaneList()
        {
            QuickNavigatorList docList = this.navigatorElement.DocumentWindowList;
            docList.Header.Text = this.settings.DocumentPaneListHeader;
            Size itemSize = this.settings.ListItemSize;

            //calculate the size of the tool pane list
            int colItemCount = Math.Min(this.docPanes.Count, this.settings.ItemsPerColumn);
            int docListWidth = this.docPaneColumns * itemSize.Width + (this.docPaneColumns - 1) * settings.ColumnSpacing;
            int docListHeight = colItemCount * this.settings.ListItemSize.Height;

            //adjust pane's settings
            docList.ItemsPanel.ItemWidth = itemSize.Width;
            docList.ItemsPanel.ItemHeight = itemSize.Height;
            //we need to set the Min-max size in order to achieve a fixed size
            Size minMax = new Size(docListWidth, 0);
            docList.MinSize = minMax;
            docList.MaxSize = minMax;
            docList.ItemsPanel.MaxSize = new Size(0, docListHeight);

            //retrieve the size of the header
            Size headerSize = this.GetDesiredSize(docList.Header);
            docListWidth = Math.Max(headerSize.Width, docListWidth);
            docListHeight += headerSize.Height;
            this.paneListHeaderHeight = Math.Max(headerSize.Height, this.paneListHeaderHeight);
            if (headerSize.Height > 0)
            {
                docListHeight += docList.Header.Margin.Bottom;
            }

            //remmeber the metrics
            this.docPaneSize = new Size(docListWidth, docListHeight);
            if (this.toolPaneSize.Width > 0)
            {
                //add spacing for the document pane
                this.docPaneSize.Width += this.settings.ColumnSpacing;
                docList.Margin = new Padding(this.settings.ColumnSpacing, 0, 0, 0);
            }

            //check whether we will display a horizontal scrollbar
            int desiredColumns = this.docPanes.Count / this.settings.ItemsPerColumn;
            if (this.docPanes.Count % this.settings.ItemsPerColumn > 0)
            {
                desiredColumns++;
            }

            if (desiredColumns > this.docPaneColumns)
            {
                this.docPaneSize.Height += SystemInformation.HorizontalScrollBarHeight;
            }
        }

        private void InitPreview()
        {
            if (!this.settings.ShowPreview)
            {
                this.navigatorElement.Preview.Visibility = ElementVisibility.Collapsed;
            }
            else
            {
                this.previewSize = this.settings.PreviewSize;
                this.navigatorElement.Preview.MinSize = this.previewSize;
                this.navigatorElement.Preview.MaxSize = this.previewSize;

                if (this.toolPaneSize.Width > 0 || this.docPaneSize.Width > 0)
                {
                    this.navigatorElement.Preview.BorderThickness = Padding.Empty;
                    this.navigatorElement.Preview.Margin = new Padding(this.settings.ColumnSpacing, this.paneListHeaderHeight, 0, 0);
                    this.previewSize.Width += this.settings.ColumnSpacing;
                }
            }

            if (this.selectedItem != null)
            {
                this.SelectItem(selectedItem);
            }
        }

        private void InitFooter()
        {
            if (!this.settings.ShowFooter)
            {
                this.navigatorElement.Footer.Visibility = ElementVisibility.Collapsed;
            }
            else
            {
                RadLabelElement footer = this.navigatorElement.Footer;
                footer.Padding = this.settings.FooterPadding;
                footer.Text = "Sample description for document";

                this.footerSize = this.GetDesiredSize(footer);
            }
        }

        /// <summary>
        /// Calculates the preferred size for the control, 
        /// including tool windows, documents, preview as well as header and footer.
        /// </summary>
        /// <returns></returns>
        internal Size GetPreferredSize()
        {
            //no need to display the navigator if neither of the pane lists in visible
            if (this.navigatorElement.ToolWindowList.Visibility == ElementVisibility.Collapsed &&
                this.navigatorElement.DocumentWindowList.Visibility == ElementVisibility.Collapsed)
            {
                return Size.Empty;
            }

            if (!this.IsLoaded)
            {
                this.LoadElementTree();
            }

            Size prefSize = this.headerSize;
            //add pane lists width
            prefSize.Width = Math.Max(prefSize.Width, this.toolPaneSize.Width + this.docPaneSize.Width);
            //calculate dock pane height - it is the max of lists height vs preview one
            prefSize.Width += this.previewSize.Width;
            int paneListHeight = Math.Max(this.toolPaneSize.Height, this.docPaneSize.Height);
            prefSize.Height += Math.Max(paneListHeight, this.previewSize.Height + this.paneListHeaderHeight);
            //add footer size
            prefSize.Width = Math.Max(this.footerSize.Width, prefSize.Width);
            prefSize.Height += this.footerSize.Height;

            Padding paneListMargins = this.navigatorElement.Preview.Parent.Margin;
            prefSize.Width += paneListMargins.Horizontal;
            prefSize.Height += paneListMargins.Vertical;

            return prefSize;
        }

        #endregion

        #region Keyboard Navigation

        /// <summary>
        /// Moves the current selection to the left.
        /// </summary>
        internal void NavigateLeft()
        {
            if (this.selectedItem == null)
            {
                return;
            }

            QuickNavigatorList owningList = this.selectedItem.Owner;
            WrapLayoutPanel itemsPanel = owningList.ItemsPanel;
            int index = itemsPanel.Children.IndexOf(this.selectedItem);

            int newIndex = index - this.settings.ItemsPerColumn;
            if (newIndex < 0)
            {
                //move the selection to the tool window list
                if (owningList == this.navigatorElement.DocumentWindowList &&
                    this.navigatorElement.ToolWindowList.Visibility == ElementVisibility.Visible)
                {
                    itemsPanel = this.navigatorElement.ToolWindowList.ItemsPanel;
                    newIndex = (this.toolPaneColumns - 1) * this.settings.ItemsPerColumn + index;
                    newIndex = Math.Max(0, newIndex);
                    newIndex = Math.Min(itemsPanel.Children.Count - 1, newIndex);
                }
            }

            if (newIndex >= 0 && newIndex < itemsPanel.Children.Count)
            {
                this.SelectItem((QuickNavigatorListItem)itemsPanel.Children[newIndex]);
            }
        }

        /// <summary>
        /// Moves the current selection to the right.
        /// </summary>
        internal void NavigateRight()
        {
            if (this.selectedItem == null)
            {
                return;
            }

            QuickNavigatorList owningList = this.selectedItem.Owner;
            WrapLayoutPanel itemsPanel = owningList.ItemsPanel;
            int index = itemsPanel.Children.IndexOf(this.selectedItem);

            int newIndex = index + this.settings.ItemsPerColumn;
            if (newIndex > itemsPanel.Children.Count - 1)
            {
                //move the selection to the document window list
                if (owningList == this.navigatorElement.ToolWindowList &&
                    this.navigatorElement.DocumentWindowList.Visibility == ElementVisibility.Visible)
                {
                    itemsPanel = this.navigatorElement.DocumentWindowList.ItemsPanel;
                    newIndex = index - (this.docPaneColumns - 1) * this.settings.ItemsPerColumn;
                    newIndex = Math.Max(0, newIndex);
                    newIndex = Math.Min(itemsPanel.Children.Count - 1, newIndex);
                }
            }

            if (newIndex >= 0 && newIndex < itemsPanel.Children.Count)
            {
                this.SelectItem((QuickNavigatorListItem)itemsPanel.Children[newIndex]);
            }
        }

        /// <summary>
        /// Moves the current selection upwards.
        /// </summary>
        internal void NavigateUp()
        {
            if (this.selectedItem == null)
            {
                return;
            }

            this.ModifySelectedIndex(-1);
        }

        /// <summary>
        /// Moves the current selection downwards.
        /// </summary>
        internal void NavigateDown()
        {
            if (this.selectedItem == null)
            {
                return;
            }

            this.ModifySelectedIndex(1);
        }

        private void ModifySelectedIndex(int correction)
        {
            QuickNavigatorList owningList = this.selectedItem.Owner;
            WrapLayoutPanel itemsPanel = owningList.ItemsPanel;
            int index = itemsPanel.Children.IndexOf(this.selectedItem);

            int newIndex = index + correction;
            if (newIndex < 0)
            {
                newIndex = itemsPanel.Children.Count - 1;
            }
            else if (newIndex > itemsPanel.Children.Count - 1)
            {
                newIndex = 0;
            }

            if (newIndex >= 0 && newIndex < itemsPanel.Children.Count)
            {
                this.SelectItem((QuickNavigatorListItem)itemsPanel.Children[newIndex]);
            }
        }

        #endregion

        #region Item Selection

        private void SelectItem(QuickNavigatorListItem item)
        {
            if (this.selectedItem != null)
            {
                this.selectedItem.IsActive = false;
            }

            RadScrollLayoutPanel scrollPanel = item.FindAncestor<RadScrollLayoutPanel>();
            if (scrollPanel != null)
            {
                scrollPanel.ScrollElementIntoView(item);
            }

            this.selectedItem = item;
            this.selectedItem.IsActive = true;
            this.navigatorElement.Header.Text = item.Text;
            this.navigatorElement.Footer.Text = item.Text;

            //update preview
            if (this.settings.ShowPreview)
            {
                this.UpdatePreview();
            }
        }

        private void UpdatePreview()
        {
            this.navigatorElement.Preview.Image = null;
            if (this.previewBitmap != null)
            {
                this.previewBitmap.Dispose();
                this.previewBitmap = null;
            }

            DockWindow window = (DockWindow)this.selectedItem.Tag;
            Control toPrint = window;

            //check whether custom preview image is provided
            if (window.DockManager != null && this.Visible)
            {
                DockWindowSnapshotEventArgs args = new DockWindowSnapshotEventArgs(window);
                window.DockManager.OnQuickNavigatorSnapshotNeeded(args);
                if (args.SnapShot != null)
                {
                    this.navigatorElement.Preview.Image = args.SnapShot;
                    return;
                }
            }

            //check for empty size or not yet displayed panel
            //if so, try to update the pane, so that we may take a snapshot
            if (!toPrint.IsHandleCreated && this.settings.ForceSnapshot)
            {
                DockTabStrip parentStrip = window.DockTabStrip;
                if (parentStrip != null)
                {
                    bool visible = window.Visible;
                    bool canUpdate = parentStrip.CanUpdateChildIndex;
                    
                    //prevent SetChildIndex notifications
                    parentStrip.CanUpdateChildIndex = false;
                    window.CreateControl();
                    window.Visible = true;
                    window.Bounds = parentStrip.TabPanelBounds;
                    window.Visible = visible;
                    parentStrip.CanUpdateChildIndex = canUpdate;
                }
            }

            //for some reason bounds are still empty, do nothing
            if (toPrint.Width <= 0 || toPrint.Height <= 0)
            {
                return;
            }

            //capture the image of the selected pane
            Size sz = this.settings.PreviewSize;
            this.previewBitmap = new Bitmap(sz.Width, sz.Height, PixelFormat.Format32bppArgb);
            toPrint.DrawToBitmap(this.previewBitmap, new Rectangle(Point.Empty, sz));

            this.navigatorElement.Preview.Image = this.previewBitmap;
        }

        #endregion

        private QuickNavigatorListItem CreateListItem(DockWindow pane)
        {
            QuickNavigatorListItem element = (QuickNavigatorListItem)Activator.CreateInstance(this.settings.ListItemType);
            element.ImagePrimitive.ImageLayout = ImageLayout.Zoom;
            element.Image = pane.Image;
            element.Text = pane.Text;
            element.TextElement.TextWrap = false;
            element.TextElement.AutoEllipsis = true;
            element.TextElement.StretchHorizontally = true;
            element.TextElement.StretchVertically = true;
            element.TextElement.AutoSize = true;
            element.MinSize = this.settings.ListItemSize;
            //pass the associated pane as a Tag
            element.Tag = pane;

            return element;
        }

        private Size GetDesiredSize(RadItem item)
        {
            Size maxSize = new Size(Int32.MaxValue, Int32.MaxValue);
            item.Measure(maxSize);

            return item.DesiredSize.ToSize();
        }

        #endregion

        #region Properties

        internal QuickNavigatorListItem SelectedItem
        {
            get
            {
                return this.selectedItem;
            }
        }

        internal QuickNavigatorElement NavigatorElement
        {
            get
            {
                return this.navigatorElement;
            }
        }

        #endregion

        #region Fields

        private Bitmap previewBitmap;

        private QuickNavigatorListItem selectedItem;
        private DockWindow activeWindow;
        private LinkedList<DockWindow> toolPanes;
        private LinkedList<DockWindow> docPanes;

        private int paneListHeaderHeight;
        private int toolPaneColumns;
        private int docPaneColumns;
        private Size availableSize;
        private Size toolPaneSize;
        private Size docPaneSize;
        private Size headerSize;
        private Size footerSize;
        private Size previewSize;

        private QuickNavigatorSettings settings;
        private SortedList<string, DockWindow> dockWindows;
        private DockWindow[] docWindowsZOrdered;
        private QuickNavigatorElement navigatorElement;
        private BorderPrimitive border;
        #endregion
    }
}
