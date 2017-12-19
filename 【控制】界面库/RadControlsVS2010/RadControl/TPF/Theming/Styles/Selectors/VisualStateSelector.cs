using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls;

namespace Telerik.WinControls.Styles
{
    public class VisualStateSelector : HierarchicalSelector
    {
        private string visualState;

        public VisualStateSelector()
        {
        }

		public VisualStateSelector(string itemVisualState)
		{
			this.visualState = itemVisualState;
		}

        /// <summary>
        /// Gets or sets value corresponding to the VisualState of the item that the selector targets
        /// </summary>
        public string VisualState
        {
            get
            {
                return this.visualState;
            }
            set
            {
                this.visualState = value;
            }
        }

		protected override bool CanSelectOverride(RadElement element)
		{
            RadItem item = element as RadItem;
            return item != null && string.CompareOrdinal(item.VisualState, this.visualState) == 0;
		}

		internal protected override bool CanUseCache
		{
			get
			{
				return false;
			}
		}

		protected override int GetKey()
		{
            if (!string.IsNullOrEmpty(this.visualState))
            {
                return GetSelectorKey(this.visualState);
            }

			return 0;
		}

        public static int GetSelectorKey(string visualState)
        {
            return ("State=" + visualState).GetHashCode();
        }

        public override bool Equals(IElementSelector elementSelector)
        {
            VisualStateSelector selector = elementSelector as VisualStateSelector;
            return selector != null && selector.visualState == this.visualState;
        }

		protected override XmlElementSelector CreateSerializableInstance()
		{			
			return new XmlVisualStateSelector(this.visualState);
		}

        public override string ToString()
        {
            if (visualState == null)
            {
                return "VisualState == NotSpecified";
            }

            return "VisualState == " + this.visualState;
        }
    }
}
