using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
	public abstract class RadDateTimePickerBehaviorDirector
	{
		public abstract void CreateChildren();
		public abstract void SetDateByValue(DateTime? date, DateTimePickerFormat formatType);
        public abstract RadDateTimePickerElement DateTimePickerElement { get; }
	}
}
