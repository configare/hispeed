using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a collection which stores RadElement instances
    /// and is sorted by ZIndex property of each element.
    /// </summary>
    internal class RadElementZOrderCollection
    {
        #region Constructor

        public RadElementZOrderCollection(RadElement owner)
        {
            this.owner = owner;
            this.list = new List<RadElement>();
        }

        #endregion

        #region Collection Methods

        internal void Add(RadElement element)
        {
            int index = this.FindInsertIndex(element);
            this.list.Insert(index, element);
            if (element.Visibility != ElementVisibility.Collapsed)
            {
                this.layoutableCount++;
            }
        }

        internal void Remove(RadElement element)
        {
            int index = this.list.IndexOf(element);
            this.RemoveAt(index);
        }

        internal void OnElementSet(RadElement element)
        {
            int index = this.list.IndexOf(element);
            if (index >= 0)
            {
                this.OnElementZIndexChanged(element);
            }
            else
            {
                this.Add(element);
            }
        }

        private void RemoveAt(int index)
        {
            //get the element at the specified index to update visible count
            RadElement element = this.list[index];
            if (element.Visibility != ElementVisibility.Collapsed)
            {
                this.layoutableCount--;
            }
            this.list.RemoveAt(index);
        }

        /// <summary>
        /// The collection gets notified for a change in the ZIndex of the specified property.
        /// </summary>
        /// <param name="element"></param>
        internal void OnElementZIndexChanged(RadElement element)
        {
            //remove the element from its previous position and add it again - this will put it in the correct position
            this.Remove(element);
            this.Add(element);
        }

        /// <summary>
        /// The collection gets notified for a change in the Visibility property of the specified element.
        /// </summary>
        /// <param name="element"></param>
        internal void OnElementVisibilityChanged(RadElement element)
        {
            if (element.Visibility != ElementVisibility.Collapsed)
            {
                this.layoutableCount++;
            }
            else
            {
                this.layoutableCount--;
            }
        }

        internal void Clear()
        {
            this.list.Clear();
            this.layoutableCount = 0;
        }

        #endregion

        #region Public Methods

        public List<RadElement> Elements
        {
            get
            {
                return this.list;
            }
        }

        /// <summary>
        /// Puts the specified element at the beginning of the collection
        /// </summary>
        /// <param name="element"></param>
        public void SendToBack(RadElement element)
        {
            if (!this.list.Contains(element))
            {
                return;
            }

            int firstZIndex = this.list[0].ZIndex;
            element.ZIndex = firstZIndex - 1;
        }

        /// <summary>
        /// Puts the specified element at the end of the collection
        /// </summary>
        /// <param name="element"></param>
        public void BringToFront(RadElement element)
        {
            if (!this.list.Contains(element))
            {
                return;
            }

            int lastZIndex = this.list[this.list.Count - 1].ZIndex;
            element.ZIndex = lastZIndex + 1;
        }

        #endregion

        #region Private Implementation

        /// <summary>
        /// Finds the insert index for the specified element.
        /// Since the collection is sorted by each element's Z-index, 
        /// we perform a binary search to determine at which position the element should be inserted.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private int FindInsertIndex(RadElement element)
        {
            int end = this.list.Count;
            if (end == 0)
            {
                return 0;
            }

            int start = 0;
            RadElement currElement;

            //use binary search to locate the desired index
            do
            {
                int middle = (start + end) >> 1;
                currElement = this.list[middle];

                int compareResult = this.CompareElements(element, currElement);
                switch (compareResult)
                {
                    case -1:
                        end = middle;
                        break;
                    case 1:
                        start = middle + 1;
                        break;
                    default:
                        Debug.Assert(false, "Invalid compare result");
                        break;
                }

            } while (start < end);

            return start;
        }

        /// <summary>
        /// Compares two elements by their z-index first
        /// and if they equals, the index in their Parent collection is used.
        /// </summary>
        /// <param name="el1"></param>
        /// <param name="el2"></param>
        /// <returns></returns>
        private int CompareElements(RadElement el1, RadElement el2)
        {
            int indexToCompare1 = el1.ZIndex;
            int indexToCompare2 = el2.ZIndex;

            if (indexToCompare1 == indexToCompare2)
            {
                indexToCompare1 = this.owner.Children.IndexOf(el1);
                indexToCompare2 = this.owner.Children.IndexOf(el2);
            }

            return indexToCompare1.CompareTo(indexToCompare2);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the count of all elements, which visibility is not ElementVisibility.Collapsed.
        /// </summary>
        public int LayoutableCount
        {
            get
            {
                return this.layoutableCount;
            }
        }

        #endregion

        #region Fields

        private List<RadElement> list;
        private RadElement owner;
        private int layoutableCount;

        #endregion
    }
}
