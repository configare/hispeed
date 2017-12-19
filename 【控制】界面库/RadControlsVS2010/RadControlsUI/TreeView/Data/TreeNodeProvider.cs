using System.Collections.Generic;
using System;
using System.Collections;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public abstract class TreeNodeProvider : IDisposable
    {
        private RadTreeViewElement treeView;
        private int update = 0;

        public TreeNodeProvider(RadTreeViewElement treeView)
        {
            this.treeView = treeView;
        }

        /// <summary>
        /// Gets the nodes.
        /// </summary>
        /// <returns></returns>
        public abstract IList<RadTreeNode> GetNodes(RadTreeNode parent);

        /// <summary>
        /// Sets the current.
        /// </summary>
        /// <param name="node">The node.</param>
        public virtual void SetCurrent(RadTreeNode node)
        {

        }

        /// <summary>
        /// Gets the tree view.
        /// </summary>
        /// <value>The tree view.</value>
        public RadTreeViewElement TreeView
        {
            get { return treeView; }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public virtual void Reset()
        {
           
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            
        }

        /// <summary>
        /// Suspends the update.
        /// </summary>
        public virtual void SuspendUpdate()
        {
            this.update++;
        }

        /// <summary>
        /// Resumes the update.
        /// </summary>
        public virtual void ResumeUpdate()
        {
            this.update--;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is suspended.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is suspended; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuspended
        {
            get
            {
                return this.update > 0;
            }
        }
    }
}
