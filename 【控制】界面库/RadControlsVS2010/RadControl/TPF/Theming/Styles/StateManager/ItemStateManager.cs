using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using System.Diagnostics;



namespace Telerik.WinControls.Styles
{

    public class ItemStateManager : ItemStateManagerBase
    {
        #region constructors

        public ItemStateManager(StateNodeBase rootState)
        {
            this.rootState = rootState;
        }

        #endregion

        #region properties

        public StateNodeBase RootState
        {
            get
            {
                return this.rootState;
            }
        }

        #endregion

        #region methods

        protected override void AttachToItemOverride(StateManagerAttachmentData attachmentData, RadItem item)
        {
            if (this.rootState == null)
            {
                return;
            }

            this.rootState.AttachToElement(item, attachmentData);
        }

        public virtual void StateInvalidated(RadItem item, StateNodeBase elementStateBase)
        {
            //StateDescriptionNode descr = this.GetAvailableStates(item.ThemeRole);

            ICollection<string> states = this.rootState.EvaluateState(item);

            if (states.Count == 0)
            {
                this.SetItemState(item, string.Empty);
                return;
            }

            string visualState = string.Empty;

            foreach (string state in states)
            {
                visualState = CombineStateNames(visualState, state);
            }

            this.SetItemState(item, visualState);
        }

        public override void ItemStateChanged(RadItem senderItem, RadPropertyChangedEventArgs changeArgs)
        {
            this.StateInvalidated(senderItem, this.rootState);
        }
        
        public override StateDescriptionNode GetAvailableStates(string themeRoleName)
        {
            StateDescriptionNode rootNode = new StateDescriptionNode(themeRoleName);
            this.rootState.AddAvailableStates(new LinkedList<StateDescriptionNode>(), rootNode);

            return rootNode;
        }

        #endregion

        #region fields

        private StateNodeBase rootState;

        #endregion
    }
}
