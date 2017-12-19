using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Themes.Design;
using System.Windows.Forms;
using System.Drawing;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.UI
{
    class RadCalendarDesignTimeData : RadControlDesignTimeData
    {
        #region Constructors
        public RadCalendarDesignTimeData()
        {
        }

        public RadCalendarDesignTimeData( string name )
            : base( name )
        {
        }
        #endregion

        public override Dictionary<string, XmlCondition> GetVisualStatesConditions()
        {
            Dictionary<string, XmlCondition> conditions = base.GetVisualStatesConditions();
            conditions.Add("OtherMonth", this.GetIsOtherMonthCondition());
            conditions.Add("Selected", this.GetIsSelectedCondition());
            conditions.Add("Today", this.GetIsTodayCondition());
            conditions.Add("Weekend", this.GetIsWeekendCondition());
            conditions.Add("SpecialDay", this.GetIsSpecialDayCondition());
            conditions.Add("Header", this.GetIsHeaderCondition());

            conditions.Add("SelectedAndIsMouseOver", this.GetSelectedAndIsMouseOver());
            conditions.Add("SelectedAndNotIsMouseOver", this.GetSelectedAndNotIsMouseOver());
            conditions.Add("NotSelectedAndNotIsMouseOver", this.GetNotSelectedAndNotIsMouseOver());
          
            conditions.Add("HeaderAndIsMouseOver", this.GetHeaderAndMouseOver());
            conditions.Add("HeaderAndNotIsMouseOver", this.GetHeaderAndNotMouseOver());
       
            conditions.Add("WeekendAndSelectedAndIsMouseOver", this.GetWeekendSelectedAndMouseOverCondition());
            conditions.Add("WeekendAndIsMouseOver", this.GetWeekendAndMouseOver());
            conditions.Add("WeekendAndNotIsMouseOver", this.GetWeekendAndNotMouseOver());
        
            conditions.Add("OtherMonthAndIsMouseOver", this.GetOtherMonthAndMouseOver());
            conditions.Add("OtherMonthAndNotIsMouseOver", this.GetOtherMonthAndNotMouseOver());
          
            conditions.Add("TodayAndIsMouseOver", this.GetTodayAndMouseOver());
            conditions.Add("TodayAndNotIsMouseOver", this.GetTodayAndNotMouseOver());
   
            conditions.Add("SpecialDayAndIsMouseOver", this.GetSpecialDayAndMouseOver());
            conditions.Add("SpecialDayAndNotIsMouseOver", this.GetSpecialDayAndNotMouseOver());

            conditions.Add("NotIsMouseOverAndNotWeekendAndNotOtherMonth", this.GetNotIsMouseOverAndNotWeekendAndNotOtherMonth());

            return conditions;
        }

        private XmlCondition GetNotIsMouseOverAndNotWeekendAndNotOtherMonth()
        {
            XmlComplexCondition condition = new XmlComplexCondition();

            XmlSimpleCondition condition1 = new XmlSimpleCondition();
            condition1.Setting = new XmlPropertySetting();
            condition1.Setting.Property = VisualElement.IsMouseOverProperty.FullName;
            condition1.Setting.Value = true;
            condition1.UnaryOperator = UnaryOperator.NotOperator;

            XmlSimpleCondition condition2 = new XmlSimpleCondition();
            condition2.Setting = new XmlPropertySetting();
            condition2.Setting.Property = CalendarCellElement.WeekEndProperty.FullName;
            condition2.Setting.Value = true;
            condition2.UnaryOperator = UnaryOperator.NotOperator;

            condition.Condition1 = condition1;
            condition.Condition2 = condition2;

            condition.BinaryOperator = BinaryOperator.AndOperator;

            XmlSimpleCondition condition3 = new XmlSimpleCondition();
            condition3.Setting = new XmlPropertySetting();
            condition3.Setting.Property = CalendarCellElement.OtherMonthProperty.FullName;
            condition3.Setting.Value = true;
            condition3.UnaryOperator = UnaryOperator.NotOperator;

            XmlComplexCondition mainCondition = new XmlComplexCondition();

            mainCondition.Condition1 = condition;
            mainCondition.Condition2 = condition3;

            mainCondition.BinaryOperator = BinaryOperator.AndOperator;

            return mainCondition;
        }

        private XmlCondition GetNotSelectedAndNotIsMouseOver()
        {
            XmlComplexCondition condition = new XmlComplexCondition();

            XmlSimpleCondition condition1 = new XmlSimpleCondition();
            condition1.Setting = new XmlPropertySetting();
            condition1.Setting.Property = VisualElement.IsMouseOverProperty.FullName;
            condition1.Setting.Value = true;
            condition1.UnaryOperator = UnaryOperator.NotOperator;

            XmlSimpleCondition condition2 = new XmlSimpleCondition();
            condition2.Setting = new XmlPropertySetting();
            condition2.Setting.Property = CalendarCellElement.SelectedProperty.FullName;
            condition2.Setting.Value = true;
            condition2.UnaryOperator = UnaryOperator.NotOperator;

            condition.Condition1 = condition1;
            condition.Condition2 = condition2;

            condition.BinaryOperator = BinaryOperator.AndOperator;
            return condition;
        }


        private XmlCondition GetSelectedAndIsMouseOver()
        {
            XmlComplexCondition condition = new XmlComplexCondition();

            XmlSimpleCondition condition1 = new XmlSimpleCondition();
            condition1.Setting = new XmlPropertySetting();
            condition1.Setting.Property = VisualElement.IsMouseOverProperty.FullName;
            condition1.Setting.Value = true;

            XmlSimpleCondition condition2 = new XmlSimpleCondition();
            condition2.Setting = new XmlPropertySetting();
            condition2.Setting.Property = CalendarCellElement.SelectedProperty.FullName;
            condition2.Setting.Value = true;

            condition.Condition1 = condition1;
            condition.Condition2 = condition2;

            condition.BinaryOperator = BinaryOperator.AndOperator;
            return condition;
        }

        private XmlCondition GetSelectedAndNotIsMouseOver()
        {
            XmlComplexCondition condition = new XmlComplexCondition();

            XmlSimpleCondition condition1 = new XmlSimpleCondition();
            condition1.Setting = new XmlPropertySetting();
            condition1.Setting.Property = VisualElement.IsMouseOverProperty.FullName;
            condition1.Setting.Value = true;
            condition1.UnaryOperator = UnaryOperator.NotOperator;

            XmlSimpleCondition condition2 = new XmlSimpleCondition();
            condition2.Setting = new XmlPropertySetting();
            condition2.Setting.Property = CalendarCellElement.SelectedProperty.FullName;
            condition2.Setting.Value = true;

            condition.Condition1 = condition1;
            condition.Condition2 = condition2;

            condition.BinaryOperator = BinaryOperator.AndOperator;
            return condition;
        }

        private XmlCondition GetSpecialDayAndMouseOver()
        {
            XmlComplexCondition condition = new XmlComplexCondition();

            XmlSimpleCondition condition1 = new XmlSimpleCondition();
            condition1.Setting = new XmlPropertySetting();
            condition1.Setting.Property = VisualElement.IsMouseOverProperty.FullName;
            condition1.Setting.Value = true;

            XmlSimpleCondition condition2 = new XmlSimpleCondition();
            condition2.Setting = new XmlPropertySetting();
            condition2.Setting.Property = CalendarCellElement.SpecialDayProperty.FullName;
            condition2.Setting.Value = true;

            condition.Condition1 = condition1;
            condition.Condition2 = condition2;

            condition.BinaryOperator = BinaryOperator.AndOperator;
            return condition;
        }

        private XmlCondition GetSpecialDayAndNotMouseOver()
        {
            XmlComplexCondition condition = new XmlComplexCondition();

            XmlSimpleCondition condition1 = new XmlSimpleCondition();
            condition1.Setting = new XmlPropertySetting();
            condition1.Setting.Property = VisualElement.IsMouseOverProperty.FullName;
            condition1.Setting.Value = true;
            condition1.UnaryOperator = UnaryOperator.NotOperator;

            XmlSimpleCondition condition2 = new XmlSimpleCondition();
            condition2.Setting = new XmlPropertySetting();
            condition2.Setting.Property = CalendarCellElement.SpecialDayProperty.FullName;
            condition2.Setting.Value = true;

            condition.Condition1 = condition1;
            condition.Condition2 = condition2;

            condition.BinaryOperator = BinaryOperator.AndOperator;
            return condition;
        }


        private XmlCondition GetTodayAndMouseOver()
        {
            XmlComplexCondition condition = new XmlComplexCondition();

            XmlSimpleCondition condition1 = new XmlSimpleCondition();
            condition1.Setting = new XmlPropertySetting();
            condition1.Setting.Property = VisualElement.IsMouseOverProperty.FullName;
            condition1.Setting.Value = true;

            XmlSimpleCondition condition2 = new XmlSimpleCondition();
            condition2.Setting = new XmlPropertySetting();
            condition2.Setting.Property = CalendarCellElement.TodayProperty.FullName;
            condition2.Setting.Value = true;

            condition.Condition1 = condition1;
            condition.Condition2 = condition2;

            condition.BinaryOperator = BinaryOperator.AndOperator;
            return condition;
        }

        private XmlCondition GetTodayAndNotMouseOver()
        {
            XmlComplexCondition condition = new XmlComplexCondition();

            XmlSimpleCondition condition1 = new XmlSimpleCondition();
            condition1.Setting = new XmlPropertySetting();
            condition1.Setting.Property = VisualElement.IsMouseOverProperty.FullName;
            condition1.Setting.Value = true;
            condition1.UnaryOperator = UnaryOperator.NotOperator;

            XmlSimpleCondition condition2 = new XmlSimpleCondition();
            condition2.Setting = new XmlPropertySetting();
            condition2.Setting.Property = CalendarCellElement.TodayProperty.FullName;
            condition2.Setting.Value = true;

            condition.Condition1 = condition1;
            condition.Condition2 = condition2;

            condition.BinaryOperator = BinaryOperator.AndOperator;
            return condition;
        }


        private XmlCondition GetOtherMonthAndMouseOver()
        {
            XmlComplexCondition condition = new XmlComplexCondition();

            XmlSimpleCondition condition1 = new XmlSimpleCondition();
            condition1.Setting = new XmlPropertySetting();
            condition1.Setting.Property = VisualElement.IsMouseOverProperty.FullName;
            condition1.Setting.Value = true;

            XmlSimpleCondition condition2 = new XmlSimpleCondition();
            condition2.Setting = new XmlPropertySetting();
            condition2.Setting.Property = CalendarCellElement.OtherMonthProperty.FullName;
            condition2.Setting.Value = true;

            condition.Condition1 = condition1;
            condition.Condition2 = condition2;

            condition.BinaryOperator = BinaryOperator.AndOperator;
            return condition;
        }

        private XmlCondition GetOtherMonthAndNotMouseOver()
        {
            XmlComplexCondition condition = new XmlComplexCondition();

            XmlSimpleCondition condition1 = new XmlSimpleCondition();
            condition1.Setting = new XmlPropertySetting();
            condition1.Setting.Property = VisualElement.IsMouseOverProperty.FullName;
            condition1.Setting.Value = true;
            condition1.UnaryOperator = UnaryOperator.NotOperator;

            XmlSimpleCondition condition2 = new XmlSimpleCondition();
            condition2.Setting = new XmlPropertySetting();
            condition2.Setting.Property = CalendarCellElement.OtherMonthProperty.FullName;
            condition2.Setting.Value = true;

            condition.Condition1 = condition1;
            condition.Condition2 = condition2;

            condition.BinaryOperator = BinaryOperator.AndOperator;
            return condition;
        }


        private XmlCondition GetWeekendAndMouseOver()
        {
            XmlComplexCondition condition = new XmlComplexCondition();

            XmlSimpleCondition condition1 = new XmlSimpleCondition();
            condition1.Setting = new XmlPropertySetting();
            condition1.Setting.Property = VisualElement.IsMouseOverProperty.FullName;
            condition1.Setting.Value = true;

            XmlSimpleCondition condition2 = new XmlSimpleCondition();
            condition2.Setting = new XmlPropertySetting();
            condition2.Setting.Property = CalendarCellElement.WeekEndProperty.FullName;
            condition2.Setting.Value = true;

            condition.Condition1 = condition1;
            condition.Condition2 = condition2;

            condition.BinaryOperator = BinaryOperator.AndOperator;
            return condition;
        }

        private XmlCondition GetWeekendAndNotMouseOver()
        {
            XmlComplexCondition condition = new XmlComplexCondition();

            XmlSimpleCondition condition1 = new XmlSimpleCondition();
            condition1.Setting = new XmlPropertySetting();
            condition1.Setting.Property = VisualElement.IsMouseOverProperty.FullName;
            condition1.Setting.Value = true;
            condition1.UnaryOperator = UnaryOperator.NotOperator;

            XmlSimpleCondition condition2 = new XmlSimpleCondition();
            condition2.Setting = new XmlPropertySetting();
            condition2.Setting.Property = CalendarCellElement.WeekEndProperty.FullName;
            condition2.Setting.Value = true;

            condition.Condition1 = condition1;
            condition.Condition2 = condition2;

            condition.BinaryOperator = BinaryOperator.AndOperator;
            return condition;
        }

        private XmlCondition GetHeaderAndMouseOver()
        {
            XmlComplexCondition condition = new XmlComplexCondition();

            XmlSimpleCondition condition1 = new XmlSimpleCondition();
            condition1.Setting = new XmlPropertySetting();
            condition1.Setting.Property = VisualElement.IsMouseOverProperty.FullName;
            condition1.Setting.Value = true;

            XmlSimpleCondition condition2 = new XmlSimpleCondition();
            condition2.Setting = new XmlPropertySetting();
            condition2.Setting.Property = CalendarCellElement.IsHeaderCellProperty.FullName;
            condition2.Setting.Value = true;

            condition.Condition1 = condition1;
            condition.Condition2 = condition2;

            condition.BinaryOperator = BinaryOperator.AndOperator;
            return condition;
        }

        private XmlCondition GetHeaderAndNotMouseOver()
        {
            XmlComplexCondition condition = new XmlComplexCondition();

            XmlSimpleCondition condition1 = new XmlSimpleCondition();
            condition1.Setting = new XmlPropertySetting();
            condition1.Setting.Property = VisualElement.IsMouseOverProperty.FullName;
            condition1.Setting.Value = true;
            condition1.UnaryOperator = UnaryOperator.NotOperator;

            XmlSimpleCondition condition2 = new XmlSimpleCondition();
            condition2.Setting = new XmlPropertySetting();
            condition2.Setting.Property = CalendarCellElement.IsHeaderCellProperty.FullName;
            condition2.Setting.Value = true;

            condition.Condition1 = condition1;
            condition.Condition2 = condition2;

            condition.BinaryOperator = BinaryOperator.AndOperator;
            return condition;
        }

        
        private XmlCondition GetWeekendSelectedAndMouseOverCondition()
        {
            XmlComplexCondition condition = new XmlComplexCondition();

            XmlSimpleCondition condition1 = new XmlSimpleCondition();
            condition1.Setting = new XmlPropertySetting();
            condition1.Setting.Property = VisualElement.IsMouseOverProperty.FullName;
            condition1.Setting.Value = true;

            XmlSimpleCondition condition2 = new XmlSimpleCondition();
            condition2.Setting = new XmlPropertySetting();
            condition2.Setting.Property = CalendarCellElement.SelectedProperty.FullName;
            condition2.Setting.Value = true;

            XmlSimpleCondition condition3 = new XmlSimpleCondition();
            condition2.Setting = new XmlPropertySetting();
            condition2.Setting.Property = CalendarCellElement.WeekEndProperty.FullName;
            condition2.Setting.Value = true;

            XmlComplexCondition condition4 = new XmlComplexCondition();
            condition4.Condition1 = condition2;
            condition4.Condition1 = condition3;

            condition.Condition1 = condition1;
            condition.Condition2 = condition4;

            condition.BinaryOperator = BinaryOperator.AndOperator;
            return condition;
        }

        private XmlCondition GetIsOtherMonthCondition()
        {
            XmlSimpleCondition condition = new XmlSimpleCondition();
            condition.Setting = new XmlPropertySetting();
            condition.Setting.Property = CalendarCellElement.OtherMonthProperty.FullName;
            condition.Setting.Value = true;
            return condition;
        }

        private XmlCondition GetIsSelectedCondition()
        {
            XmlSimpleCondition condition = new XmlSimpleCondition();
            condition.Setting = new XmlPropertySetting();
            condition.Setting.Property = CalendarCellElement.SelectedProperty.FullName;
            condition.Setting.Value = true;
            return condition;
        }

        private XmlCondition GetIsTodayCondition()
        {
            XmlSimpleCondition condition = new XmlSimpleCondition();
            condition.Setting = new XmlPropertySetting();
            condition.Setting.Property = CalendarCellElement.TodayProperty.FullName;
            condition.Setting.Value = true;
            return condition;
        }

        private XmlCondition GetIsWeekendCondition()
        {
            XmlSimpleCondition condition = new XmlSimpleCondition();
            condition.Setting = new XmlPropertySetting();
            condition.Setting.Property = CalendarCellElement.WeekEndProperty.FullName;
            condition.Setting.Value = true;
            return condition;
        }

        private XmlCondition GetIsSpecialDayCondition()
        {
            XmlSimpleCondition condition = new XmlSimpleCondition();
            condition.Setting = new XmlPropertySetting();
            condition.Setting.Property = CalendarCellElement.SpecialDayProperty.FullName;
            condition.Setting.Value = true;
            return condition;
        }

        private XmlCondition GetIsHeaderCondition()
        {
            XmlSimpleCondition condition = new XmlSimpleCondition();
            condition.Setting = new XmlPropertySetting();
            condition.Setting.Property = CalendarCellElement.IsHeaderCellProperty.FullName;
            condition.Setting.Value = true;
            return condition;
        }

     

        public override ControlStyleBuilderInfoList GetThemeDesignedControls( Control previewSurface )
        {
            ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();
            //preview
            RadCalendar radCalendarPreview = new RadCalendar();
            radCalendarPreview.ShowColumnHeaders = true;
            radCalendarPreview.ShowRowHeaders = true;
            radCalendarPreview.ShowViewHeader = true;
            radCalendarPreview.ShowViewSelector = true;
            radCalendarPreview.ShowFooter = true;
            radCalendarPreview.Bounds = new Rectangle(0, 0, 250, 250);
            radCalendarPreview.AutoSize = true;
            radCalendarPreview.AllowSelect = true;
            radCalendarPreview.AllowMultipleSelect = true;
            radCalendarPreview.AllowViewSelector = true;
            radCalendarPreview.AllowRowHeaderSelectors = true;
            radCalendarPreview.AllowColumnHeaderSelectors = true;

            radCalendarPreview.SpecialDays.Add(new RadCalendarDay(DateTime.Now.Date.AddDays(3)));
            
            //structure menu
          
            RadCalendar radCalendarStructure = new RadCalendar();
            radCalendarStructure.ShowFooter = true;
            radCalendarStructure.ShowColumnHeaders = true;
            radCalendarStructure.ShowRowHeaders = true;
            radCalendarStructure.ShowViewHeader = true;
            radCalendarStructure.ShowViewSelector = true;

            radCalendarStructure.Bounds = new Rectangle(0, 0, 250, 250);

            //here we are trying to remove all children
            //to display only one cell in structure tree in VSB
            LightVisualElement firstDayName = null;
            LightVisualElement firstDay = radCalendarStructure.CalendarElement.CalendarVisualElement.Children[0] as LightVisualElement;
            if (firstDay != null )
            {
                if (firstDay is CalendarHeaderCellElement)//found days headers cells (M,T,S,T,F,S,S) 
                                                //first day should be with offset from 7 cells
                {
                    firstDayName = firstDay;
                    firstDay = radCalendarStructure.CalendarElement.CalendarVisualElement.Children[ 7 ] as LightVisualElement;
                }

                radCalendarStructure.CalendarElement.CalendarVisualElement.Children.Clear();
                if( firstDayName != null)
                    radCalendarStructure.CalendarElement.CalendarVisualElement.Children.Add(firstDayName);
                radCalendarStructure.CalendarElement.CalendarVisualElement.Children.Add(firstDay);
            }


            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo( radCalendarPreview, radCalendarStructure.RootElement );
            designed.Placemenet = PreviewControlPlacemenet.MiddleCenter;
            res.Add( designed );           

            return res;
        }
    }
}

