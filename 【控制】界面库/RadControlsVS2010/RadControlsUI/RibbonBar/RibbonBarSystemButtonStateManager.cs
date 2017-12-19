using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    class RibbonBarSystemButtonStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            CompositeStateNode formStatesComposite = new CompositeStateNode("FormStates");

            StateNodeWithCondition simple = new StateNodeWithCondition("FormMaximized", new SimpleCondition(RadRibbonBarElement.RibbonFormWindowStateProperty, FormWindowState.Maximized));
            formStatesComposite.AddState(simple);

            simple = new StateNodeWithCondition("FormNormal", new SimpleCondition(RadRibbonBarElement.RibbonFormWindowStateProperty, FormWindowState.Normal));
            formStatesComposite.AddState(simple);

            simple = new StateNodeWithCondition("FormMinimized", new SimpleCondition(RadRibbonBarElement.RibbonFormWindowStateProperty, FormWindowState.Minimized));
            formStatesComposite.AddState(simple);



            return formStatesComposite;
        }

    }
}