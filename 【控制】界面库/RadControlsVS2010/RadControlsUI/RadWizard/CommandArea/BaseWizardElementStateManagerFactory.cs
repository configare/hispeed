using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class BaseWizardElementStateManagerFactory : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            StateNodeBase isWelcomePage = new StateNodeWithCondition("IsWelcomePage", new SimpleCondition(BaseWizardElement.IsWelcomePageProperty, true));
            StateNodeBase isCompletionPage = new StateNodeWithCondition("IsCompletionPage", new SimpleCondition(BaseWizardElement.IsCompletionPageProperty, true));

            CompositeStateNode all = new CompositeStateNode("all states");
            all.AddState(isWelcomePage);
            all.AddState(isCompletionPage);
            return all;
        }

        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            base.AddDefaultVisibleStates(sm);

            sm.AddDefaultVisibleState("IsWelcomePage");
            sm.AddDefaultVisibleState("IsCompletionPage");
        }
    }
}