using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Data;
using System.ComponentModel;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a collection of WizardPage objects.
    /// </summary>
    [Serializable]
    [Editor(DesignerConsts.RadWizardPageCollectionEditorString, typeof(UITypeEditor))]
    public class WizardPageCollection : ObservableCollection<WizardPage>
    {
        #region Fields

        private RadWizardElement owner;

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a WizardPageCollection instance.
        /// </summary>
        /// <param name="owner">Owner of the element.</param>
        public WizardPageCollection(RadWizardElement owner)
        {
            this.owner = owner;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the owner RadWizardElement of the collection.
        /// </summary>
        public RadWizardElement Owner
        {
            get { return this.owner; }
        }

        #endregion

        #region Method

        /// <summary>
        /// Inserts a WizardPage before the RadWizard CompletionPage in the collection.
        /// </summary>
        /// <param name="page"></param>
        public new void Add(WizardPage page)
        {
            if (this.Owner.Pages.Count > 0)
            {
                if (this.Owner.CompletionPage != null && this.Owner.Pages.Contains(this.Owner.CompletionPage))
                {
                    int completionPageIndex = this.Owner.Pages.IndexOf(this.Owner.CompletionPage);
                    this.Owner.Pages.Insert(completionPageIndex, page);
                }
                else
                {
                    this.Owner.Pages.Insert(this.Owner.Pages.Count, page);
                }
            }
            else
            {
                this.Owner.Pages.Insert(0, page);
            }
        }

        protected override void InsertItem(int index, WizardPage item)
        {
            item.Owner = this.owner;
            base.InsertItem(index, item);

            if (item.Owner.View != null && !item.Owner.View.Children.Contains(item))
            {
                item.Owner.View.Children.Add(item);
            }
        }

        protected override void RemoveItem(int index)
        {
            WizardPage page = this[index];
            if (page.Owner != null && page.Owner.View != null && page.Owner.View.Children.Contains(page))
            {
                page.Owner.View.Children.Remove(page);
            }

            base.RemoveItem(index);
            page.Owner = null;
        }

        protected override void SetItem(int index, WizardPage item)
        {
            base.SetItem(index, item);
            item.Owner = this.owner;
        }

        #endregion
    }
}