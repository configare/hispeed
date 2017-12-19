using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls
{

    public delegate void RadPropertyChangingEventHandler(object sender, RadPropertyChangingEventArgs args);

	public class RadPropertyChangingEventArgs : CancelEventArgs
	{
		private RadProperty property;
		private object oldValue;
        private object newValue;
		private RadPropertyMetadata metadata;
	    private bool isNewValueSet;

	    public RadPropertyChangingEventArgs(RadProperty property, object oldValue, object newValue, RadPropertyMetadata metadata)
		{
			this.property = property;
            this.oldValue = oldValue;
			this.newValue = newValue;
			this.metadata = metadata;
		}

		public RadProperty Property
		{
			get { return property; }
		}

		public object NewValue
		{
			get
			{
			    return newValue;
			}
            set
            {
                isNewValueSet = true;
                newValue = value;
            }
		}

        public object OldValue
        {
            get { return oldValue; }
        }

		public RadPropertyMetadata Metadata
		{
			get { return metadata; }
		}

	    internal bool IsNewValueSet
	    {
	        get { return isNewValueSet; }
	    }
	}
}
