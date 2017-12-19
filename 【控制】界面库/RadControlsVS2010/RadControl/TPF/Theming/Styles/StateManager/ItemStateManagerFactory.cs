
namespace Telerik.WinControls.Styles
{
    public class ItemStateManagerFactory : ItemStateManagerFactoryBase
    {
        private StateNodeBase rootState;

        public StateNodeBase RootState
        {
            get
            {
                return this.rootState;
            }
        }

        public virtual StateNodeBase CreateRootState()
        {
            CompositeStateNode all = new CompositeStateNode("all states");
            all.AddState(this.CreateEnabledStates());
            all.AddState(this.CreateSpecificStates());

            StateNodeWithCondition enabled = new StateNodeWithCondition("Enabled", new SimpleCondition(RadElement.EnabledProperty, true));
            enabled.TrueStateLink = all;
            enabled.FalseStateLink = new StatePlaceholderNode("Disabled");

            return enabled;
        }

        protected virtual StateNodeBase CreateEnabledStates()
        {
            CompositeStateNode mouseStates = new CompositeStateNode("Mouse states");
            StateNodeBase mouseOver = new StateNodeWithCondition("MouseOver", new SimpleCondition(RadElement.IsMouseOverProperty, true));
            StateNodeBase mouseDown = new StateNodeWithCondition("MouseDown", new SimpleCondition(RadElement.IsMouseDownProperty, true));
            StateNodeBase containsMouse = new StateNodeWithCondition("ContainsMouse", new SimpleCondition(RadElement.ContainsMouseProperty, true));
            //containsMouse.TrueStateLink = mouseOver;
            mouseStates.AddState(containsMouse);
            mouseStates.AddState(mouseOver);
            mouseStates.AddState(mouseDown);

            return mouseStates;
        }

        protected virtual StateNodeBase CreateSpecificStates()
        {
            return new VoidStateNode();
        }

        protected virtual void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("Disabled");
            sm.AddDefaultVisibleState("MouseOver");
            sm.AddDefaultVisibleState("MouseDown");
        }

        protected override ItemStateManagerBase CreateStateManager()
        {
            this.rootState = this.CreateRootState();
            ItemStateManager sm = this.CreateStateManagerCore();
            this.AddDefaultVisibleStates(sm);
            return sm;
        }

        protected virtual ItemStateManager CreateStateManagerCore()
        {
            ItemStateManager sm = new ItemStateManager(this.RootState);
            return sm;
        }
    }
}
