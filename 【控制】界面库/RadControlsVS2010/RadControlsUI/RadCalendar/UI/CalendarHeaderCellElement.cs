using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class CalendarHeaderCellElement : CalendarCellElement
    {
        internal protected CalendarHeaderCellElement(CalendarVisualElement owner, string text)
            : base(owner, text)
        {
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.Class = "CalendarHeaderCellElement";
        }
    }
}
