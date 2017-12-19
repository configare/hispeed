using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class RadComboBoxArrowButtonElement : RadArrowButtonElement
    {
        static RadComboBoxArrowButtonElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadDropDownArrowButtonElementStateManagerFactory(RadComboBoxElement.IsDropDownShownProperty), typeof(RadComboBoxArrowButtonElement));
        }
    }

    public class RadDropDownArrowButtonElementStateManagerFactory : ButtonItemStateManagerFactory
    {
        private RadProperty dropDownProperty = null;
        public RadDropDownArrowButtonElementStateManagerFactory(RadProperty dropDownProperty) : base()
        {
            this.dropDownProperty = dropDownProperty;
        }

        protected override StateNodeBase CreateSpecificStates()
        {
            CompositeStateNode baseStates = (CompositeStateNode)base.CreateSpecificStates();
            StateNodeBase isDropDownShown = new StateNodeWithCondition("IsDropDownShown", new SimpleCondition(this.dropDownProperty, true));
            baseStates.AddState(isDropDownShown);
            return baseStates;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            base.AddDefaultVisibleStates(sm);
            sm.AddDefaultVisibleState("IsDropDownShown");
        }
    }
}
