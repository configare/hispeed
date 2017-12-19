using System;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the method that will handle events in <see cref="HierarchyTraverser"/>.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void TreeViewTraversingEventHandler(object sender, TreeViewTraversingEventArgs e);

    /// <summary>
    /// Provides data for all events used in <see cref="HierarchyTraverser"/>
    /// </summary>
    public class TreeViewTraversingEventArgs : EventArgs
    {
        #region Fields

        private RadTreeNode node;
        private bool process;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TraversingEventArgs"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public TreeViewTraversingEventArgs(RadTreeNode content)
        {
            this.node = content;
            this.process = content != null ? (content.Visible || content.IsInDesignMode) : true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="HierarchalObject"/> instance to be processed by <see cref="HierarchyTraverser"/>.
        /// </summary>
        /// <value><c>true</c> if [process hierarchy object]; otherwise, <c>false</c>.</value>
        public bool Process
        {
            get
            {
                return this.process;
            }
            set
            {
                this.process = value;
            }
        }


        /// <summary>
        /// Gets the node.
        /// </summary>
        /// <value>The node.</value>
        public RadTreeNode Node
        {
            get
            {
                return this.node;
            }
        }

        #endregion
    }
}
