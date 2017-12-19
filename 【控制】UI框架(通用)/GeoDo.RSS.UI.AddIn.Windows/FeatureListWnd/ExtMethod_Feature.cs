using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    internal static class ExtMethod_Feature
    {
        public static ListViewItem ToListViewItem(this CodeCell.AgileMap.Core.Feature fet)
        {
            string[] values = fet.FieldValues;
            if (values == null || values.Length == 0)
                return null;
            ListViewItem it = new ListViewItem(values[0]);
            for (int i = 1; i < values.Length; i++)
            {
                it.SubItems.Add(values[i]);
            }
            return it;
        }
    }
}
