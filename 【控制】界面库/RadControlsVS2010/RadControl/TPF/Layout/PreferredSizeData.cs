using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.Layouts
{
   
    ///<exclude/> 
   
	public class PreferredSizeData
	{
		private RadElement element;
		private Size preferredSize;

		public RadElement Element
		{
			get
			{
				return this.element;
			}
			set
			{
				this.element = value;
			}
		}

		public Size PreferredSize
		{
			get
			{
				return this.preferredSize;
			}
			set
			{
				this.preferredSize = value;
			}
		}

		public PreferredSizeData(RadElement element)
		{
			this.element = element;
			this.preferredSize = Size.Empty;
		}

        public PreferredSizeData(RadElement element, Rectangle bounds)
		{
			this.element = element;
			this.preferredSize = element.GetPreferredSize(bounds.Size);
		}

		public PreferredSizeData(RadElement element, Size preferredSize)
            : this(element, preferredSize, false)
		{
		}

        public PreferredSizeData(RadElement element, Size preferredSize, bool obligatorySize)
        {
            this.element = element;
            Size returnedSize = element.GetPreferredSize(preferredSize);
            this.preferredSize = obligatorySize ? preferredSize : returnedSize;
        }
	}
}