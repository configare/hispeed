using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Manages the document windows in a RadDock instance.
    /// Supports additional collection of all DockWindow instances that reside within a DocumentTabStrip. The collection's sort order depends on the activation precedence.
    /// Provides methods for navigating to next and previous document. Keeps track of the currently active window.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DocumentManager : RadDockObject
    {
        #region Fields

        private List<DockWindow> documents;
        private List<DockWindow> documentsSorted;
        private RadDock dockManager;
        private ActiveDocumentListSortOrder activeListMenuSortOrder;
        private bool boldActiveDocument = true;
        private DocumentCloseActivation docCloseActivation;
        private DockWindowInsertOrder documentInsertOrder;

        #endregion

        #region Initialize/Uninitialize

        /// <summary>
        /// Constructs a new DocumentManager instance.
        /// </summary>
        /// <param name="owner"></param>
        internal DocumentManager(RadDock owner)
        {
            this.documents = new List<DockWindow>();
            //list of registered documents, sorted by their Text value
            this.documentsSorted = new List<DockWindow>();
            this.dockManager = owner;
            this.activeListMenuSortOrder = ActiveDocumentListSortOrder.ByText;
            this.documentInsertOrder = DockWindowInsertOrder.Default;
            this.docCloseActivation = DocumentCloseActivation.Default;
        }

        /// <summary>
        /// Builds the default list of documents. Called upon initialization completion.
        /// </summary>
        internal void BuildDocumentList()
        {
            this.ClearDocuments();

            foreach (Control child in ControlHelper.EnumChildControls(this.dockManager.MainDocumentContainer, true))
            {
                DockWindow document = child as DockWindow;
                if (document != null && document.DockManager == this.dockManager)
                {
                    document.InnerDockState = DockState.TabbedDocument;
                    this.InsertDocument(-1, document);
                }
            }
        }

        private void EnsureActiveDocument()
        {
            //search all DocumentTabStrip instances and select the first opened document
            foreach (Control child in ControlHelper.EnumChildControls(this.dockManager.MainDocumentContainer, true))
            {
                DocumentTabStrip strip = child as DocumentTabStrip;
                if (strip != null && strip.TabPanels.Count > 0 && strip.DockManager == this.dockManager)
                {
                    DockWindow active = strip.ActiveWindow;
                    this.InsertDocument(-1, active);
                    strip.UpdateActiveWindow(active, true);
                    break;
                }
            }
        }

        /// <summary>
        /// The manager gets notified that the owning RadDock instance has been sucessfully loaded.
        /// </summary>
        protected internal void OnDockManagerLoaded()
        {
            //update active document
            DockWindow activeDoc = this.ActiveDocument;
            if (activeDoc != null)
            {
                activeDoc.UpdateActiveState(true);
            }
        }

        protected override void DisposeManagedResources()
        {
            this.ClearDocuments();
            base.DisposeManagedResources();
        }

        #endregion

        #region Notifications

        /// <summary>
        /// The manager receives notification from its owning RadDock instance for a change in the currently active window.
        /// </summary>
        /// <param name="currActive"></param>
        internal void OnActiveWindowChanged(DockWindow currActive)
        {
            if (currActive.DockState == DockState.TabbedDocument)
            {
                this.DeactivatePreviousActive(currActive);

                //remove the window if it exists in the document collection
                int index = this.documents.IndexOf(currActive);
                if (index >= 0)
                {
                    this.documents.RemoveAt(index);
                }

                //put it at the beginning of the list.
                this.documents.Insert(0, currActive);
                currActive.UpdateActiveState(true);
            }
            else
            {
                DockWindow active = this.ActiveDocument;
                if (active != null)
                {
                    active.UpdateActiveState(true);
                }
            }
        }

        /// <summary>
        /// Receives a notification from the owning RadDock that a LoadFromXML operation is upcoming.
        /// </summary>
        internal void OnLoadingFromXML()
        {
            this.ClearDocuments();
        }

        /// <summary>
        /// 
        /// </summary>
        internal void OnLoadedFromXML()
        {
            this.BuildDocumentList();
        }

        /// <summary>
        /// Receives a notification for a change in the DockState property of a DockWindow.
        /// We may have a situation where a ToolWindow becomes a TabbedDocument and vice-versa.
        /// </summary>
        /// <param name="window"></param>
        internal void OnDockWindowDockStateChanged(DockWindow window)
        {
            //update the documents collection
            if (window.DockState == DockState.TabbedDocument)
            {
                this.InsertDocument(-1, window);
            }
            else
            {
                this.RemoveDocument(-1, window);
            }
        }

        /// <summary>
        /// Receives a notification for a DockWindow instance added to the owning RadDock.
        /// </summary>
        /// <param name="window"></param>
        internal void OnDockWindowAdded(DockWindow window)
        {
            if (window.DockState == DockState.TabbedDocument)
            {
                this.InsertDocument(-1, window);
            }
        }

        /// <summary>
        /// Receives a notification for a DockWindow instance removed from the owning RadDock.
        /// </summary>
        /// <param name="window"></param>
        internal void OnDockWindowRemoved(DockWindow window)
        {
            this.RemoveDocument(-1, window);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Activates the next document in the z-ordered list.
        /// </summary>
        public void ActivateNextDocument()
        {
            this.ActivateDocument(true);
        }

        /// <summary>
        /// Activates the previous document in the z-ordered list.
        /// </summary>
        public void ActivatePreviousDocument()
        {
            this.ActivateDocument(false);
        }

        /// <summary>
        /// Gets the list of menu items to be displayed on the ActiveWindowList on the specified DocumentTabStrip.
        /// </summary>
        /// <param name="strip"></param>
        /// <returns></returns>
        public IEnumerable<DockWindow> GetActiveWindowList(DocumentTabStrip strip)
        {
            IEnumerable collection = null;

            switch(this.activeListMenuSortOrder)
            {
                case ActiveDocumentListSortOrder.None:
                    collection = strip.TabPanels;
                    break;
                case ActiveDocumentListSortOrder.ZOrdered:
                    collection = this.documents;
                    break;
                case ActiveDocumentListSortOrder.ByText:
                    collection = this.documentsSorted;
                    break;
            }

            foreach (DockWindow window in collection)
            {
                if (window.Parent == strip)
                {
                    yield return window;
                }
            }
        }

        private void OnActiveDocumentMenuItemClick(object sender, EventArgs e)
        {
            RadMenuItem item = sender as RadMenuItem;
            if (item == null)
            {
                return;
            }

            DockWindow window = item.Tag as DockWindow;
            if (window != null)
            {
                window.EnsureVisible();
            }
        }

        private void ActivateDocument(bool next)
        {
            int docCount = this.documents.Count;
            //do nothing if we have one or less documents
            if (docCount <= 1)
            {
                return;
            }

            DockWindow currActive = this.documents[0];

            if (next)
            {
                //send the first document to back of the list
                DockWindow firstDocument = this.documents[0];
                this.documents.RemoveAt(0);
                this.documents.Add(firstDocument);
            }
            else
            {
                //bring the last document in front of the list
                DockWindow lastDocument = this.documents[docCount - 1];
                this.documents.RemoveAt(docCount - 1);
                this.documents.Insert(0, lastDocument);
            }

            currActive.UpdateActiveState(false);
            //activate the document in front of the z-order
            this.dockManager.ActiveWindow = this.documents[0];
        }

        private void InsertDocument(int index, DockWindow document)
        {
            if (this.documents.Contains(document))
            {
                return;
            }

            if (index == -1)
            {
                index = 0;
            }

            this.DeactivatePreviousActive(document);
            this.documents.Insert(index, document);
            //add the document in the sorted list
            this.documentsSorted.Insert(this.FindInsertIndex(document), document);

            DockWindow activeDoc = this.ActiveDocument;
            if (activeDoc != null)
            {
                activeDoc.UpdateActiveState(true);
            }
        }

        private void RemoveDocument(int index, DockWindow document)
        {
            if (index == -1)
            {
                index = this.documents.IndexOf(document);
            }

            if (index < 0 || index > this.documents.Count - 1)
            {
                return;
            }

            this.DeactivatePreviousActive(null);
            this.documents.RemoveAt(index);
            this.documentsSorted.Remove(document);

            if (this.docCloseActivation == DocumentCloseActivation.FirstInZOrder && this.documents.Count > 0)
            {
                this.dockManager.ActiveWindow = this.documents[0];
            }

            DockWindow activeDoc = this.ActiveDocument;
            if (activeDoc != null)
            {
                activeDoc.UpdateActiveState(true);
            }
        }

        private void DeactivatePreviousActive(DockWindow newActive)
        {
            if (this.documents.Count == 0)
            {
                return;
            }

            DockWindow currActive = this.documents[0];
            if (currActive != null && currActive != newActive)
            {
                currActive.UpdateActiveState(false);
            }
        }

        private void ClearDocuments()
        {
            this.documents.Clear();
            this.documentsSorted.Clear();
        }

        /// <summary>
        /// Finds the insert index for the specified document, using binary search.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        private int FindInsertIndex(DockWindow window)
        {
            int end = this.documentsSorted.Count;
            if (end == 0)
            {
                return 0;
            }

            int start = 0;
            DockWindow currWindow;

            //use binary search to locate the desired index
            do
            {
                int middle = (start + end) >> 1;
                currWindow = this.documentsSorted[middle];

                int compareResult = window.Text.CompareTo(currWindow.Text);
                if (compareResult == 0)
                {
                    return middle;
                }

                if (compareResult < 0)
                {
                    end = middle;
                }
                else
                {
                    start = middle + 1;
                }
            } while (start < end);

            return start;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines which document should become active when the current active one is closed.
        /// </summary>
        [DefaultValue(DocumentCloseActivation.Default)]
        [Description("Determines which document should become active when the current active one is closed.")]
        public DocumentCloseActivation DocumentCloseActivation
        {
            get
            {
                return this.docCloseActivation;
            }
            set
            {
                this.docCloseActivation = value;
            }
        }

        /// <summary>
        /// Gets or sets the insert order to be used when adding new documents.
        /// </summary>
        [Description("Gets or sets the insert order to be used when adding new documents.")]
        public DockWindowInsertOrder DocumentInsertOrder
        {
            get
            {
                if (this.documentInsertOrder == DockWindowInsertOrder.Default)
                {
                    return DockWindowInsertOrder.InFront;
                }

                return this.documentInsertOrder;
            }
            set
            {
                this.documentInsertOrder = value;
            }
        }

        bool ShouldSerializeNewWindowInsertOrder()
        {
            return this.documentInsertOrder != DockWindowInsertOrder.Default;
        }

        /// <summary>
        /// Determines whether the currently active document's Text will be displayed in bold Font in its corresponding TabItem.
        /// </summary>
        [DefaultValue(true)]
        [Description("Determines whether the currently active document's Text will be displayed in bold Font in its corresponding TabItem.")]
        public bool BoldActiveDocument
        {
            get
            {
                return this.boldActiveDocument;
            }
            set
            {
                if (this.boldActiveDocument == value)
                {
                    return;
                }

                this.boldActiveDocument = value;
                if (!this.dockManager.ShouldProcessNotification())
                {
                    return;
                }

                DockWindow activeDocument = this.ActiveDocument;
                if (activeDocument != null)
                {
                    activeDocument.UpdateActiveState(this.boldActiveDocument);
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ActiveDocumentListSortOrder">ActiveListMenuSortOrder</see> value,
        /// which defines how menu items will be sorted in the active document list menu.
        /// </summary>
        [DefaultValue(ActiveDocumentListSortOrder.ByText)]
        [Description("Gets or sets the a value, which defines how will menu items be sorted in the active document list menu.")]
        public ActiveDocumentListSortOrder ActiveDocumentMenuSortOrder
        {
            get
            {
                return this.activeListMenuSortOrder;
            }
            set
            {
                this.activeListMenuSortOrder = value;
            }
        }

        /// <summary>
        /// Gets an array of DockWindow instances, which DockState equals to DockState.TabbedDocument, in the order they appear in their parent strips.
        /// </summary>
        [Browsable(false)]
        public DockWindow[] DocumentArray
        {
            get
            {
                return DockHelper.GetDockWindows(this.dockManager.MainDocumentContainer, true, this.dockManager).ToArray();
            }
        }

        /// <summary>
        /// Gets an array of DockWindow instances, which DockState equals to DockState.TabbedDocument.
        /// The array is sorted by each window's z-order.
        /// </summary>
        [Browsable(false)]
        public DockWindow[] DocumentArrayZOrdered
        {
            get
            {
                return this.documents.ToArray();
            }
        }

        /// <summary>
        /// Gets an array of <see cref="DockWindow">DockWindow</see> instances, which <see cref="DockWindow.DockState">DockState</see> equals <see cref="DockState.TabbedDocument">TabbedDocument</see>.
        /// The array is sorted by the Text value of each document.
        /// </summary>
        [Browsable(false)]
        public DockWindow[] DocumentArraySortedByText
        {
            get
            {
                return this.documentsSorted.ToArray();
            }
        }

        /// <summary>
        /// Gets the currently active document in the owning RadDock instance.
        /// </summary>
        [Browsable(false)]
        public DockWindow ActiveDocument
        {
            get
            {
                if (this.documents.Count == 0)
                {
                    this.EnsureActiveDocument();
                }

                if (this.documents.Count > 0)
                {
                    DockWindow activeDoc = this.documents[0];
                    DockTabStrip strip = activeDoc.DockTabStrip;
                    if (strip != null)
                    {
                        activeDoc = strip.ActiveWindow;
                    }

                    return activeDoc;
                }

                return null;
            }
        }

        /// <summary>
        /// Gest an enumerator, which allows for iterating all registered documents in the order they appear in their parent strips.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<DockWindow> DocumentEnumerator
        {
            get
            {
                foreach (Control child in ControlHelper.EnumChildControls(this.dockManager.MainDocumentContainer, true))
                {
                    DockWindow window = child as DockWindow;
                    if (window != null && window.DockManager == this.dockManager)
                    {
                        yield return window;
                    }
                }
            }
        }

        /// <summary>
        /// Gets an enumerator, which allows for iterating all registered documents in their z-order.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<DockWindow> DocumentEnumeratorZOrdered
        {
            get
            {
                foreach (DockWindow window in this.documents)
                {
                    yield return window;
                }
            }
        }

        /// <summary>
        /// Gets an enumerator, which allows for iterating all registered documents in a sorted-by-text manner.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<DockWindow> DocumentEnumeratorSortedByText
        {
            get
            {
                foreach (DockWindow window in this.documentsSorted)
                {
                    yield return window;
                }
            }
        }

        #endregion
    }
}
