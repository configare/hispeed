using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class BackstageItemStateManagerFactory : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateEnabledStates()
        {
            CompositeStateNode itemStates = new CompositeStateNode("Mouse states");
             
            StateNodeBase mouseOver = new StateNodeWithCondition("MouseOver", 
                new ComplexCondition(new SimpleCondition(RadElement.IsMouseOverProperty, true),
                    BinaryOperator.OrOperator, new SimpleCondition(BackstageButtonItem.IsCurrentProperty, true)));
            StateNodeBase mouseDown = new StateNodeWithCondition("MouseDown", new SimpleCondition(RadElement.IsMouseDownProperty, true));
            StateNodeBase containsMouse = new StateNodeWithCondition("ContainsMouse", new SimpleCondition(RadElement.ContainsMouseProperty, true));
            StateNodeBase rtl = new StateNodeWithCondition("RightToLeft", new SimpleCondition(RadElement.RightToLeftProperty, true));
            
            itemStates.AddState(containsMouse);
            itemStates.AddState(mouseOver);
            itemStates.AddState(mouseDown);
            itemStates.AddState(rtl);

            return itemStates;
        }
    }
}
