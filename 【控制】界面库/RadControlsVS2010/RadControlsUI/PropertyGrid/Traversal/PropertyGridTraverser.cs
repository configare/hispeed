
namespace Telerik.WinControls.UI
{
    public class PropertyGridTraverser : ITraverser<PropertyGridItemBase>
    {
        #region Fields

        private PropertyGridTableElement propertyGridElement;
        private PropertyGridTraverser enumerator;
        private PropertyGridGroupItem group;
        private PropertyGridItemBase item;
        private int index;
        private int groupIndex;

        #endregion

        #region Constructor

        public PropertyGridTraverser(PropertyGridTableElement propertyGridElement)
        {
            this.propertyGridElement = propertyGridElement;
        }

        #endregion

        #region Events

        public event PropertyGridTraversingEventHandler Traversing;

        protected virtual void OnTraversing(PropertyGridTraversingEventArgs e)
        {
            PropertyGridTraversingEventHandler handler = this.Traversing;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected bool OnTraversing()
        {
            PropertyGridTraversingEventArgs e = new PropertyGridTraversingEventArgs(this.item);
            this.OnTraversing(e);
            return e.Process;
        }

        #endregion

        #region ITraverser<PropertyGridDataItemBase> Members

        object ITraverser<PropertyGridItemBase>.Position
        {
            get
            {
                return new PropertyGridTraverserState(this.group, this.item, this.index, this.groupIndex);
            }
            set
            { 
                PropertyGridTraverserState state = (PropertyGridTraverserState)value;
                this.group = state.Group;
                this.item = state.Item;
                this.index = state.Index;
                this.groupIndex = state.GroupIndex;
            }
        }

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

        public bool MoveToEnd()
        {
            while (this.MoveNext())
            { }

            return true;
        }

        protected virtual PropertyGridItemBase GetLastChild(PropertyGridItemBase currentItem)
        {
            if (currentItem.Expandable && currentItem.Expanded)
            {
                this.index = currentItem.GridItems.Count - 1;
                return GetLastChild(currentItem.GridItems[this.index]);
            }

            return currentItem;
        }

        public bool MoveToFirst()
        {
            if (this.propertyGridElement.CollectionView.Groups.Count > 0)
            {
                this.group = ((PropertyGridGroup)propertyGridElement.CollectionView.Groups[0]).GroupItem;
                this.groupIndex = 0;
                this.item = group;
                this.index = 0;
                return true;
            }

            if (this.propertyGridElement.CollectionView.Count > 0)
            {
                this.index = 0;
                this.item = propertyGridElement.CollectionView[0];
                return true;
            }

            return false;
        }

        protected virtual bool MovePreviousCore()
        {
            if (this.item == null)
            {
                return false;
            }

            if (this.item is PropertyGridGroupItem)
            {
                PropertyGridGroupItem group = this.item as PropertyGridGroupItem;
                return MovePreviousFromGroupItem(group);
            }
            else
            {
                PropertyGridItem currentItem = this.item as PropertyGridItem;
                return MovePreviousFromDataItem(currentItem);
            }
        }

        protected virtual bool MoveNextCore()
        {
            if (this.item == null)
            {
                return MoveToFirst();
            }

            if (this.item is PropertyGridGroupItem)
            {
                PropertyGridGroupItem group = this.item as PropertyGridGroupItem;
                return MoveNextFromGroupItem(group);
            }
            else
            {
                PropertyGridItem currentItem = this.item as PropertyGridItem;
                return MoveNextFromDataItem(currentItem, true, false);
            }
        }

        protected virtual bool MoveNextFromGroupItem(PropertyGridGroupItem currentGroup)
        {
            if (currentGroup.Expanded && currentGroup.GridItems.Count > 0)
            {
                this.item = currentGroup.GridItems[0];
                this.index = 0;
                return true;
            }

            if (groupIndex + 1 < this.propertyGridElement.CollectionView.Groups.Count)
            {
                this.group = ((PropertyGridGroup)(object)propertyGridElement.CollectionView.Groups[++this.groupIndex]).GroupItem;
                this.item = this.group;
                return true;
            }
                        
            return false;
        }

        protected virtual bool MoveNextFromDataItem(PropertyGridItemBase currentItem, bool checkIfExpandable, bool resetIndex)
        {
            if (currentItem.Expandable && checkIfExpandable)
            {
                if (currentItem.Expanded && currentItem.GridItems.Count > 0)
                {
                    this.item = currentItem.GridItems[0];
                    this.index = 0;
                    return true;
                }
            }

            if (currentItem.Parent != null)
            {
                if (resetIndex)
                {
                    // check for groups
                        //if groups => check in current group
                        //else => collectionView
                    this.index = currentItem.Parent.GridItems.IndexOf(currentItem as PropertyGridItem);
                }

                if (this.index + 1 < currentItem.Parent.GridItems.Count)
                {
                    this.item = currentItem.Parent.GridItems[++this.index];
                    return true;
                }

                return MoveNextFromDataItem(currentItem.Parent, false, true);
            }

            if (currentItem.Parent == null && this.propertyGridElement.CollectionView.Groups.Count == 0)
            {
                if (resetIndex)
                {
                    this.index = this.propertyGridElement.CollectionView.IndexOf(currentItem as PropertyGridItem);
                }

                if (index + 1 < this.propertyGridElement.CollectionView.Count)
                {
                    this.item = this.propertyGridElement.CollectionView[++this.index];
                    return true;
                }

                return false;
            }

            if (currentItem.Parent == null && this.propertyGridElement.CollectionView.Groups.Count != 0)
            {
                if (resetIndex)
                {
                    this.index = this.propertyGridElement.CollectionView.Groups[this.groupIndex].IndexOf(currentItem as PropertyGridItem);
                }

                if (this.index + 1 < this.propertyGridElement.CollectionView.Groups[this.groupIndex].Items.Count)
                {
                    this.item = this.propertyGridElement.CollectionView.Groups[this.groupIndex].Items[++this.index];
                    return true;
                }

                if (this.groupIndex + 1 < this.propertyGridElement.CollectionView.Groups.Count)
                {
                    this.group = ((PropertyGridGroup)(object)propertyGridElement.CollectionView.Groups[++this.groupIndex]).GroupItem;
                    this.item = this.group;
                    this.index = -1;
                    return true;
                }
            }

            return false;
        }

        protected virtual bool MovePreviousFromGroupItem(PropertyGridGroupItem currentGroup)
        {
            if (this.groupIndex > 0)
            {
                PropertyGridGroupItem prevGroup = ((PropertyGridGroup)(object)propertyGridElement.CollectionView.Groups[--this.groupIndex]).GroupItem;
                if (prevGroup.Expanded)
                {
                    this.index = prevGroup.GridItems.Count - 1;
                    this.item = this.GetLastChild(prevGroup.GridItems[this.index]);
                    return true;
                }

                this.group = prevGroup;
                this.item = this.group;
                this.index = -1;
                return true;
            }
            else if (this.groupIndex == 0)
            {
                this.Reset();
                return true;
            }

            return false;
        }

        protected virtual bool MovePreviousFromDataItem(PropertyGridItemBase currentItem)
        {
            if (currentItem.Parent != null && currentItem.Parent.GridItems.Count > 1)
            {
                if (this.index > 0)
                {
                    PropertyGridItemBase prevItem = currentItem.Parent.GridItems[--this.index];
                    if (prevItem.Expandable && prevItem.Expanded)
                    {
                        prevItem = this.GetLastChild(prevItem);
                    }

                    this.item = prevItem;
                    return true;
                }

                this.item = currentItem.Parent;
                this.index = -1;
                if (currentItem.Parent.Parent != null)
                {
                    this.index = currentItem.Parent.Parent.GridItems.IndexOf(currentItem.Parent as PropertyGridItem);
                }
                else
                {
                    if (this.propertyGridElement.CollectionView.Groups.Count == 0)
                    {
                        this.index = this.propertyGridElement.CollectionView.IndexOf(currentItem.Parent as PropertyGridItem);
                    }
                    else
                    {
                        this.index = this.propertyGridElement.CollectionView.Groups[this.groupIndex].IndexOf(currentItem.Parent as PropertyGridItem);
                    }
                }
                return true;
            }

            if (currentItem.Parent == null && this.propertyGridElement.CollectionView.Groups.Count != 0 && this.index > 0 &&
                this.index < this.propertyGridElement.CollectionView.Groups[this.groupIndex].Items.Count &&
                this.groupIndex < this.propertyGridElement.CollectionView.Groups.Count)
            {
                this.item = this.propertyGridElement.CollectionView.Groups[this.groupIndex].Items[--this.index];
                return true;
            }

            if (currentItem.Parent == null && this.propertyGridElement.CollectionView.Groups.Count != 0 && this.index == 0)
            {
                this.group = ((PropertyGridGroup)(object)propertyGridElement.CollectionView.Groups[this.groupIndex]).GroupItem;
                this.item = this.group;
                this.index = -1;
                return true;
            }

            if (currentItem.Parent == null && this.propertyGridElement.CollectionView.Groups.Count == 0)
            {
                if (this.index > 0 && this.index < this.propertyGridElement.CollectionView.Count)
                {
                    PropertyGridItemBase prevItem = this.propertyGridElement.CollectionView[--this.index];
                    if (prevItem != null)
                    {
                        prevItem = this.GetLastChild(prevItem);
                    }

                    this.item = prevItem;
                    return true;
                }
                else if (this.index == 0)
                {
                    this.Reset();
                    return true;
                }                
            }

            return false;
        }

        public void Reset()
        {
            this.item = null;
            this.group = null;
            this.index = -1;
            this.groupIndex = -1;
        }

        #endregion

        #region IEnumerator<PropertyGridDataItemBase> Members

        public PropertyGridItemBase Current
        {
            get
            {
                return this.item;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return this.item;
            }
        }

        #endregion

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            if (enumerator == null)
            {
                this.enumerator = new PropertyGridTraverser(this.propertyGridElement);
            }

            this.enumerator.item = this.item;
            this.enumerator.group = this.group;
            this.enumerator.index = this.index;
            this.enumerator.groupIndex = this.groupIndex;

            return enumerator;
        }

        #endregion     

        public void MoveTo(int index)
        {
            Reset();
            while (index != 0 && MoveNext())
            {
                index++;
            }
        }

        public int MoveTo(PropertyGridItemBase item)
        {
            int index = -1;
            
            Reset();
            while (MoveNext())
            {
                index++;
                if (Current == item)
                {
                    return index;
                }
            }

            return -1;
        }

        public int GetIndex(PropertyGridItemBase item)
        {
            object position = ((ITraverser<PropertyGridItemBase>)this).Position;
            int index = MoveTo(item);
            ((ITraverser<PropertyGridItemBase>)this).Position = position;
            return index;
        }
    }    
}