using System.Collections;
using System.Drawing;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
    public abstract class BaseVirtualizedContainer<T> : LayoutPanel
    {
        IVirtualizedElementProvider<T> elementProvider;
        IEnumerable dataProvider;

        #region Initialization

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.ClipDrawing = true;
        }

        #endregion

        #region Properties

        public IVirtualizedElementProvider<T> ElementProvider
        {
            get
            {
                return elementProvider;
            }
            set
            {
                if (elementProvider != value)
                {
                    elementProvider = value;
                    this.Children.Clear();
                }
            }
        }

        public IEnumerable DataProvider
        {
            get
            {
                return dataProvider;
            }
            set
            {
                if (dataProvider != value)
                {
                    dataProvider = value;
                    InvalidateMeasure();
                }
            }

        }

        protected bool DataProviderIsEmpty
        {
            get
            {
                IEnumerator enumerator = (IEnumerator)this.DataProvider.GetEnumerator();
                return enumerator == null;
            }
        }

        #endregion

        #region Public methods

        public virtual void UpdateItems()
        {
            foreach (RadElement element in this.Children)
            {
                element.InvalidateMeasure();
            }
            this.InvalidateMeasure();
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            if (BeginMeasure(availableSize))
            {
                MeasureElements();
            }
            return EndMeasure();
        }

        protected virtual void MeasureElements()
        {
            int position = 0;
            foreach (T data in DataProvider)
            {
                if (!IsItemVisible(data))
                {
                    continue;
                }
                IVirtualizedElement<T> element = UpdateElement(position, data);
                if (element == null)
                {
                    continue;
                }
                position++;
                if (!MeasureElement(element))
                {
                    break;
                }
            }
            while (position < this.Children.Count)
            {
                RemoveElement(position);
            }
        }

        protected virtual bool BeginMeasure(SizeF availableSize)
        {
            return this.DataProvider != null && this.ElementProvider != null && !DataProviderIsEmpty;
        }

        protected abstract bool MeasureElement(IVirtualizedElement<T> element);

        protected abstract SizeF EndMeasure();

        protected virtual bool IsItemVisible(T data)
        {
            return true;
        }

        #endregion

        #region Element management

        protected virtual object GetElementContext()
        {
            return null;
        }

        protected virtual void RemoveElement(int position)
        {
            IVirtualizedElement<T> virtualizedElement = (IVirtualizedElement<T>)this.Children[position];
            ElementProvider.CacheElement(virtualizedElement);
            this.BitState[InvalidateMeasureOnRemoveStateKey] = false;
            this.Children.RemoveAt(position);
            this.BitState[InvalidateMeasureOnRemoveStateKey] = true;
            virtualizedElement.Detach();
        }

        protected virtual void InsertElement(int position, IVirtualizedElement<T> element, T data)
        {
            if (element != null)
            {
                ((RadElement)element).Visibility = ElementVisibility.Visible;
                this.Children.checkForAlreadyAddedItems = false;
                this.Children.Insert(position, (RadElement)element);
                this.Children.checkForAlreadyAddedItems = true;
                element.Attach(data, GetElementContext());
            }
        }

        protected virtual int FindCompatibleElement(int position, T data)
        {
            return -1;
        }

        protected virtual IVirtualizedElement<T> UpdateElement(int position, T data)
        {
            IVirtualizedElement<T> element = null;
            object elementContext = this.GetElementContext();

            if (position < this.Children.Count)
            {
                element = (IVirtualizedElement<T>)this.Children[position];
                if (ElementProvider.ShouldUpdate(element, data, elementContext))
                {
                    if (position < this.Children.Count - 1)
                    {
                        IVirtualizedElement<T> nextElement = (IVirtualizedElement<T>)this.Children[position + 1];

                        if (nextElement.Data.Equals(data))
                        {
                            this.Children.Move(position + 1, position);
                            nextElement.Synchronize();
                            return nextElement;
                        }
                    }

                    if (ElementProvider.IsCompatible(element, data, elementContext))
                    {
                        element.Detach();
                        element.Attach(data, elementContext);
                    }
                    else
                    {
                        int index = FindCompatibleElement(position, data);
                        if (index > position)
                        {
                            this.Children.Move(index, position);
                            element = (IVirtualizedElement<T>)this.Children[position];
                            element.Synchronize();
                        }
                        else
                        {
                            element = ElementProvider.GetElement(data, elementContext);
                            InsertElement(position, element, data);
                        }
                    }
                }
            }
            else
            {
                element = ElementProvider.GetElement(data, elementContext);
                InsertElement(position, element, data);
            }

            return element;
        }

        #endregion
    }
}
