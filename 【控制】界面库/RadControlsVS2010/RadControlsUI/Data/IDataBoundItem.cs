using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Data
{
    public interface IDataBoundItem
    {
		object DataItem
		{
			get;
		}

		void SetDataItem(object dataItem);
    }
}
