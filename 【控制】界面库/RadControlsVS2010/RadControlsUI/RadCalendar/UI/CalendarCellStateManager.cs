using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class CalendarCellStateManagerFactory: ItemStateManagerFactory
    {
        public override StateNodeBase CreateRootState()
        {
            StateNodeWithCondition enabledState = new StateNodeWithCondition("Enabled", new SimpleCondition(RadElement.EnabledProperty, true));
            enabledState.TrueStateLink = this.CreateSpecificStates();
            enabledState.FalseStateLink = new StatePlaceholderNode("Disabled");

            return enabledState;
        }

        protected override StateNodeBase CreateSpecificStates()
        {
            CompositeStateNode calendarCellStates = new CompositeStateNode("Cell States");
            StateNodeWithCondition selectedState = new StateNodeWithCondition("Selected", new SimpleCondition(CalendarCellElement.SelectedProperty, true));

            StateNodeWithCondition mouseOver = new StateNodeWithCondition("MouseOver", new SimpleCondition(RadElement.IsMouseOverProperty, true));
            StateNodeWithCondition mouseDown = new StateNodeWithCondition("MouseDown", new SimpleCondition(RadElement.IsMouseDownProperty, true));
            mouseOver.FalseStateLink = new StateNodeWithCondition("Focused", new SimpleCondition(CalendarCellElement.FocusedProperty, true));

            CompositeStateNode mouseStateTree = new CompositeStateNode("mouse state tree");
            mouseStateTree.AddState(mouseOver);
            mouseStateTree.AddState(mouseDown);

            StateNodeWithCondition IsHeader = new StateNodeWithCondition("Header", new SimpleCondition(CalendarCellElement.IsHeaderCellProperty, true));
            calendarCellStates.AddState(IsHeader);
            calendarCellStates.AddState(selectedState);
            calendarCellStates.AddState(mouseStateTree);

            CompositeStateNode dayCellsGroup = new CompositeStateNode("Day cells");
            IsHeader.FalseStateLink = dayCellsGroup;

            StateNodeWithCondition outOfRangeState = new StateNodeWithCondition("OutOfRange", new SimpleCondition(CalendarCellElement.OutOfRangeProperty, true));
            StateNodeWithCondition otherMonthState = new StateNodeWithCondition("OtherMonth", new SimpleCondition(CalendarCellElement.OtherMonthProperty, true));
            StateNodeWithCondition weekendState = new StateNodeWithCondition("Weekend", new SimpleCondition(CalendarCellElement.WeekEndProperty, true));
            StateNodeWithCondition specialDayState = new StateNodeWithCondition("SpecialDay", new SimpleCondition(CalendarCellElement.SpecialDayProperty, true));
            StateNodeWithCondition todayState = new StateNodeWithCondition("Today", new SimpleCondition(CalendarCellElement.TodayProperty, true));

            dayCellsGroup.AddState(outOfRangeState);
            dayCellsGroup.AddState(otherMonthState);
            dayCellsGroup.AddState(todayState);
            dayCellsGroup.AddState(weekendState);
            dayCellsGroup.AddState(specialDayState);

            return calendarCellStates;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("Selected");
            sm.AddDefaultVisibleState("Today");
            sm.AddDefaultVisibleState("Weekend");
        }
    }
}
