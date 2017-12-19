using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Styles
{
    public class StateNodeWithCondition : StateNodeBase
    {
        private Condition condition;

        public Condition Condition
        {
            set
            {
                condition = value;
            }

            get
            {
                return condition;
            }
        }

        public StateNodeWithCondition(string name)
            : base(name)
        { }

        public StateNodeWithCondition(string name, Condition condition)
            : this(name)
        {
            this.condition = condition;
        }

        public override ICollection<string> EvaluateState(RadItem targetItem)
        {
            ICollection<string> res = new LinkedList<string>();

            if (this.Condition.Evaluate(targetItem))
            {
                if (this.TrueStateLink != null)
                {
                    res = this.TrueStateLink.EvaluateState(targetItem);
                }
                else
                {
                    res.Add(this.Name);
                }
            }
            else
            {
                if (this.FalseStateLink != null)
                {
                    res = this.FalseStateLink.EvaluateState(targetItem);
                }
            }

            return res;
        }

        public override void AddAvailableStates(ICollection<StateDescriptionNode> newNodes, StateDescriptionNode node)
        {
            if (this.TrueStateLink != null)
            {
                this.TrueStateLink.AddAvailableStates(newNodes, node);
            }
            else
            {
                newNodes.Add(node.AddNode(this.Name));                
            }

            if (this.FalseStateLink != null)
            {
                this.FalseStateLink.AddAvailableStates(newNodes, node);
            }
        }

        public override void AttachToElement(RadItem item, StateManagerAttachmentData attachmentData)
        {
            base.AttachToElement(item, attachmentData);

            if (this.condition == null)
            {
                return;
            }

            attachmentData.AddEventHandlers(condition.AffectedProperties);
        }
    }
}
