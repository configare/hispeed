using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class TitleBarButtonStateManager : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            CompositeStateNode formStatesComposite = new CompositeStateNode("FormStates");

            StateNodeWithCondition simple = new StateNodeWithCondition("FormMaximized", new SimpleCondition(RadFormElement.FormWindowStateProperty, FormWindowState.Maximized));
            formStatesComposite.AddState(simple);

            simple = new StateNodeWithCondition("FormNormal", new SimpleCondition(RadFormElement.FormWindowStateProperty, FormWindowState.Normal));
            formStatesComposite.AddState(simple);

            simple = new StateNodeWithCondition("FormMinimized", new SimpleCondition(RadFormElement.FormWindowStateProperty, FormWindowState.Minimized));
            formStatesComposite.AddState(simple);

            simple = new StateNodeWithCondition("IsFormActive", new SimpleCondition(RadFormElement.IsFormActiveProperty, true));
            formStatesComposite.AddState(simple);

            return formStatesComposite;
        }

    }
}
