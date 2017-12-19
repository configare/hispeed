using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents element's layout data. 
    /// </summary>
	public class ElementLayoutData
	{
		private RadElement element;
		private PerformLayoutType performLayoutType;
		private bool performed = false;
        
        /// <summary>
        /// Gets or sets the element. 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
		public PerformLayoutType PerformLayoutType
		{
			get
			{
				return this.performLayoutType;
			}
			set
			{
				this.performLayoutType = value;
			}
		}

        /// <summary>
        ///
        /// </summary>
		public bool Performed
		{
			get
			{
				return this.performed;
			}
			set
			{
				this.performed = value;
			}
		}

        /// <summary>
        /// Initializes a new instance of the ElementLayoutData class from 
        /// the element and its PerformLayoutType.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="performLayoutType"></param>
		public ElementLayoutData(RadElement element, PerformLayoutType performLayoutType)
		{
			this.element = element;
			this.performLayoutType = performLayoutType;
		}
	}
}