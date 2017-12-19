using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Styles
{
    public abstract class StateNodeBase
    {
        private string name;

        private StateNodeBase trueStateLink;

        private StateNodeBase falseStateLink;

        public StateNodeBase FalseStateLink
        {
            get
            {
                return this.falseStateLink;
            }
            set
            {
                this.falseStateLink = value;
            }
        }

        public StateNodeBase TrueStateLink
        {
            get
            {
                return this.trueStateLink;
            }
            set
            {
                this.trueStateLink = value;
            }
        }

        public StateNodeBase(string stateName)
        {
            this.name = stateName;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }        

        public abstract ICollection<string> EvaluateState(RadItem targetItem);

        public abstract void AddAvailableStates(ICollection<StateDescriptionNode> newNodes, StateDescriptionNode node);

        public virtual void AttachToElement(RadItem item, StateManagerAttachmentData attachmentData)
        {
            if (this.trueStateLink != null)
            {
                this.trueStateLink.AttachToElement(item, attachmentData);
            }

            if (this.falseStateLink != null)
            {
                this.falseStateLink.AttachToElement(item, attachmentData);
            }
        }
    }
}
