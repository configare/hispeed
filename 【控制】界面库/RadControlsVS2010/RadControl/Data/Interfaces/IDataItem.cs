using System;
using System.Windows.Forms;

namespace Telerik.WinControls.Data
{
    public interface IDataItem
    {
        object DataBoundItem { get; set; }

        int FieldCount { get; }
        object this[string name] { get; set; }
        object this[int index] { get; set; }

        int IndexOf(string name);
    }
}
