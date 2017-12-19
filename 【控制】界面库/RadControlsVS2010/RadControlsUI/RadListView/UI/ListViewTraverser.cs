using System;

namespace Telerik.WinControls.UI
{
    public class ListViewTraverser : ITraverser<ListViewDataItem>
    {
        #region Fields

        private RadListViewElement owner;
        private ListViewDataItem position;
        private ListViewTraverser enumerator;

        private int currentGroupIndex = -1;
        private int currentItemIndex = -1;

        #endregion

        #region Ctor

        public ListViewTraverser(RadListViewElement owner)
        {
            this.owner = owner;
            this.Position = null;
        }

        #endregion

        #region Events

        public event ListViewTraversingEventHandler Traversing;

        #endregion

        #region Properties
        
        private bool IsGroupingEnabled
        {
            get
            {
                return this.owner.ShowGroups && this.owner.Groups.Count > 0 &&
                ((this.owner.EnableGrouping && !this.owner.IsDesignMode) || this.owner.EnableCustomGrouping);
            }
        }

        public object Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value as ListViewDataItem;

                if (position == null)
                {
                    if (this.IsGroupingEnabled)
                    {
                        this.currentItemIndex = -2;
                        this.currentGroupIndex = -1;
                    }
                    else
                    {
                        this.currentItemIndex = -1;
                        this.currentGroupIndex = -1;
                    }

                    return;
                }

                SetPositionCore(value);
            }
        }

        public ListViewDataItem Current
        {
            get
            {
                return this.position;
            }
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }
        
        #endregion

        #region Public Methods
         
        public bool MovePrevious()
        {
            while (this.MovePreviousCore())
            {
                if (this.OnTraversing())
                {
                    return true;
                }
            }

            return false;
        }

        public bool MoveToEnd()
        {
            if (this.MoveNext())
            {
                while (this.MoveNext()) ;
                return true;
            }

            return false;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            while (this.MoveNextCore())
            {
                if (this.OnTraversing())
                {
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            this.position = null;
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            if (enumerator == null)
            {
                this.enumerator = new ListViewTraverser(this.owner);
            }

            this.enumerator.Position = this.position;
            this.enumerator.currentItemIndex = this.currentItemIndex;
            this.enumerator.currentGroupIndex = this.currentGroupIndex;
            return enumerator;
        }

        #endregion

        #region Virtual Methods

        protected virtual void OnTraversing(ListViewTraversingEventArgs e)
        {
            ListViewTraversingEventHandler handler = this.Traversing;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual bool OnTraversing()
        {
            if (this.IsGroupingEnabled && this.position != null &&
                this.position.Group != null &&
                !this.position.Group.Expanded)
            {
                return false;
            }

            if (this.position != null && !this.position.Visible)
            {
                return false;
            }

            ListViewTraversingEventArgs e = new ListViewTraversingEventArgs(this.position);
            this.OnTraversing(e);
            return e.Process;
        }

        protected virtual bool MovePreviousCore()
        {
            if (position == null)
            {
                return false;
            }

            if (this.IsGroupingEnabled)
            {
                if (this.currentGroupIndex == -1)
                {
                    this.Position = null;
                    return false;
                }

                this.currentItemIndex--;

                if (this.currentItemIndex == -1)
                {
                    position = this.owner.Groups[this.currentGroupIndex];
                    return true;
                }
                if (this.currentItemIndex == -2)
                {
                    currentGroupIndex--;
                    if (currentGroupIndex == -1)
                    {
                        this.position = null;
                        return true;
                    }

                    if (this.owner.Groups.Count > currentGroupIndex)
                    {
                        currentItemIndex = this.owner.Groups[currentGroupIndex].Items.Count - 1;
                        if (currentItemIndex >= 0)
                            position = this.owner.Groups[currentGroupIndex].Items[currentItemIndex];
                        else
                        {
                            position = this.owner.Groups[currentGroupIndex];
                        }
                        return true;
                    }
                    else return false;
                }

                if (this.owner.Groups[currentGroupIndex].Items.Count > currentItemIndex)
                {
                    position = this.owner.Groups[currentGroupIndex].Items[currentItemIndex];
                    return true;
                }

                return false;
            }
            else
            {
                if (this.owner.Items.Count > currentItemIndex && currentItemIndex > 0)
                {
                    currentItemIndex--;
                    position = this.owner.Items[currentItemIndex];
                    return true;
                }

                position = null;
                return false;
            }
        }
         
        protected virtual bool MoveNextCore()
        {
            if (position == null)
            {
                if (this.IsGroupingEnabled)
                {
                    position = this.owner.Groups[0];
                    currentGroupIndex = 0;
                    currentItemIndex = -1;
                    return true;
                }
                else if (this.owner.Items.Count > 0)
                {
                    position = this.owner.Items[0];
                    currentItemIndex = 0;
                    return true;
                }

                return false;
            }

            if (this.IsGroupingEnabled)
            {
                if (this.currentGroupIndex == this.owner.Groups.Count - 1 && this.currentItemIndex == this.owner.Groups[this.currentGroupIndex].Items.Count - 1)
                {
                    return false;
                }

                currentItemIndex++;

                if (owner.Groups[currentGroupIndex].Items.Count > currentItemIndex)
                {
                    position = owner.Groups[currentGroupIndex].Items[currentItemIndex];
                    return true;
                }
                else
                {
                    currentItemIndex = -1;
                    currentGroupIndex++;
                    if (owner.Groups.Count > currentGroupIndex)
                    {
                        position = owner.Groups[currentGroupIndex];
                        return true;
                    }
                    else return false;
                }
            }
            else
            {
                if (currentItemIndex < owner.Items.Count - 1)
                {
                    currentItemIndex++;
                    position = owner.Items[currentItemIndex];
                    return true;
                }

                return false;
            }
        }

        protected virtual void SetPositionCore(object value)
        {
            if (this.IsGroupingEnabled)
            {
                ListViewDataItemGroup group = value as ListViewDataItemGroup;
                if (group != null)
                {
                    this.currentGroupIndex = this.owner.Groups.IndexOf(group);
                    this.currentItemIndex = -1;
                }
                else
                {
                    int groupCount = this.owner.Groups.Count;
                    for (int i = 0; i < groupCount; i++)
                    {
                        if (this.owner.Groups[i].Items.Contains(position))
                        {
                            this.currentGroupIndex = i;
                            this.currentItemIndex = this.owner.Groups[i].Items.IndexOf(position);
                            break;
                        }
                    }
                }
            }
            else
            {
                this.currentGroupIndex = 0;
                this.currentItemIndex = this.owner.Items.IndexOf(position);
            }
        }

        #endregion
    }
}