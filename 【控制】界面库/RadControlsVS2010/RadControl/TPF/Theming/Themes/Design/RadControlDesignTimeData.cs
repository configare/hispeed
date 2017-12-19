using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.Themes.Design
{
    ///<exclude/>
	public class RadControlDesignTimeData
	{
		private string controlName;

		public RadControlDesignTimeData()
		{
		}

		public RadControlDesignTimeData(string controlName)
		{
			this.controlName = controlName;
		}

		public string ControlName
		{
			get { return controlName; }
			set { controlName = value; }
		}

		public virtual ControlStyleBuilderInfoList GetThemeDesignedControls(Control previewSurface)
		{
			return null;
		}

        private XmlCondition GetIsMouseOverCondition()
        {
            XmlSimpleCondition condition = new XmlSimpleCondition();
            condition.Setting = new XmlPropertySetting();
            condition.Setting.Property = VisualElement.IsMouseOverProperty.FullName;
            condition.Setting.Value = true;
            return condition;
        }

        private XmlCondition GetNotIsMouseOverCondition()
        {
            XmlSimpleCondition condition = new XmlSimpleCondition();
            condition.Setting = new XmlPropertySetting();
            condition.Setting.Property = VisualElement.IsMouseOverProperty.FullName;
            condition.Setting.Value = true;
            condition.UnaryOperator = UnaryOperator.NotOperator;
            return condition;
        }

        private XmlCondition GetIsMouseDownCondition()
        {
            XmlSimpleCondition condition = new XmlSimpleCondition();
            condition.Setting = new XmlPropertySetting();
            condition.Setting.Property = VisualElement.IsMouseDownProperty.FullName;
            condition.Setting.Value = true;
            return condition;
        }

        private XmlCondition GetNotIsMouseDownCondition()
        {
            XmlSimpleCondition condition = new XmlSimpleCondition();
            condition.Setting = new XmlPropertySetting();
            condition.Setting.Property = VisualElement.IsMouseDownProperty.FullName;
            condition.Setting.Value = true;
            condition.UnaryOperator = UnaryOperator.NotOperator;
            return condition;
        }

        private XmlCondition GetIsMouseOverAndIsMouseDownCondition()
        {
            XmlComplexCondition condition = new XmlComplexCondition();

            XmlSimpleCondition condition1 = new XmlSimpleCondition();
            condition1.Setting = new XmlPropertySetting();
            condition1.Setting.Property = VisualElement.IsMouseOverProperty.FullName;
            condition1.Setting.Value = true;

            XmlSimpleCondition condition2 = new XmlSimpleCondition();
            condition2.Setting = new XmlPropertySetting();
            condition2.Setting.Property = VisualElement.IsMouseDownProperty.FullName;
            condition2.Setting.Value = true;

            condition.Condition1 = condition1;
            condition.Condition2 = condition2;

            condition.BinaryOperator = BinaryOperator.AndOperator;
            return condition;
        }

        private XmlCondition GetIsMouseOverAndNotIsMouseDownCondition()
        {
            XmlComplexCondition condition = new XmlComplexCondition();

            XmlSimpleCondition condition1 = new XmlSimpleCondition();
            condition1.Setting = new XmlPropertySetting();
            condition1.Setting.Property = VisualElement.IsMouseOverProperty.FullName;
            condition1.Setting.Value = true;

            XmlSimpleCondition condition2 = new XmlSimpleCondition();
            condition2.Setting = new XmlPropertySetting();
            condition2.Setting.Property = VisualElement.IsMouseDownProperty.FullName;
            condition2.Setting.Value = true;
            condition2.UnaryOperator = UnaryOperator.NotOperator;

            condition.Condition1 = condition1;
            condition.Condition2 = condition2;
        
            condition.BinaryOperator = BinaryOperator.AndOperator;
            return condition;
        }

        public virtual Dictionary<string, XmlCondition> GetVisualStatesConditions()
        {
            Dictionary<string, XmlCondition> conditions = new Dictionary<string, XmlCondition>();
            conditions.Add("IsMouseOver", GetIsMouseOverCondition());
            conditions.Add("NotIsMouseOver", GetNotIsMouseOverCondition());
            conditions.Add("IsMouseDown", GetIsMouseDownCondition());
            conditions.Add("NotIsMouseDown", GetNotIsMouseDownCondition());
            conditions.Add("IsMouseOverAndIsMouseDown", GetIsMouseOverAndIsMouseDownCondition());
            conditions.Add("IsMouseOverAndNotIsMouseDown", GetIsMouseOverAndNotIsMouseDownCondition());
            return conditions;
        }
	}
}
