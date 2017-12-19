using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class RadPageViewControlCollection : Control.ControlCollection
    {
        #region Fields

        private RadPageView owner;
        private byte suspendOwnerNotify;

        #endregion

        #region Constructor

        public RadPageViewControlCollection(RadPageView owner)
            : base(owner)
        {
            this.owner = owner;
        }

        #endregion

        public override void Add(Control value)
        {
            RadPageViewPage page = value as RadPageViewPage;
            this.ValidatePage(page);

            if (this.suspendOwnerNotify == 0)
            {
                RadPageViewCancelEventArgs args = new RadPageViewCancelEventArgs(page);
                this.owner.OnPageAdding(args);

                if (args.Cancel)
                {
                    return;
                }
            }

            base.Add(value);

            if (this.suspendOwnerNotify == 0)
            {
                this.owner.OnPageAdded(new RadPageViewEventArgs(page));
            }
        }

        public override void Clear()
        {
            if (this.Count == 0)
            {
                return;
            }

            if (this.suspendOwnerNotify == 0)
            {
                CancelEventArgs e = new CancelEventArgs();
                this.owner.OnPagesClearing(e);
                if (e.Cancel)
                {
                    return;
                }
            }

            base.Clear();

            if (this.suspendOwnerNotify == 0)
            {
                this.owner.OnPagesCleared(EventArgs.Empty);
            }
        }

        public override void Remove(Control value)
        {
            RadPageViewPage page = value as RadPageViewPage;
            this.ValidatePage(page);

            if (this.suspendOwnerNotify == 0)
            {
                RadPageViewCancelEventArgs cancelArgs = new RadPageViewCancelEventArgs(page);
                this.owner.OnPageRemoving(cancelArgs);

                if (cancelArgs.Cancel)
                {
                    return;
                }
            }

            base.Remove(value);

            if (this.suspendOwnerNotify == 0)
            {
                this.owner.OnPageRemoved(new RadPageViewEventArgs(page));
            }
        }

        public override void SetChildIndex(Control child, int newIndex)
        {
            //unwanted index changes may occur due to internal z-order update, so allow only intended ones.
            if (!this.owner.AllowPageIndexChange)
            {
                return;
            }

            this.owner.DisablePageIndexChange();

            RadPageViewPage page = child as RadPageViewPage;
            this.ValidatePage(page);

            int currentIndex = this.IndexOf(page);

            if (this.suspendOwnerNotify == 0)
            {
                RadPageViewIndexChangingEventArgs args = new RadPageViewIndexChangingEventArgs(page, newIndex, currentIndex);
                this.owner.OnPageIndexChanging(args);

                if (args.Cancel)
                {
                    return;
                }
            }

            base.SetChildIndex(child, newIndex);

            if (this.suspendOwnerNotify == 0)
            {
                this.owner.OnPageIndexChanged(new RadPageViewIndexChangedEventArgs(page, currentIndex, newIndex));
            }
        }

        internal void SuspendOwnerNotify()
        {
            this.suspendOwnerNotify++;
        }

        internal void ResumeOwnerNotify()
        {
            if (this.suspendOwnerNotify > 0)
            {
                this.suspendOwnerNotify--;
            }
        }

        private void ValidatePage(RadPageViewPage page)
        {
            if (page == null)
            {
                throw new ArgumentException("RadPageView accepts only RadPageViewPage instances as child controls");
            }
        }
    }
}
