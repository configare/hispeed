using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Styles
{
    public class CompositeStateNode : StateNodeBase
    {
        LinkedList<StateNodeBase> states = new LinkedList<StateNodeBase>();

        public CompositeStateNode(string name)
            : base(name)
        {
        }

        public IEnumerable<StateNodeBase> States
        {
            get
            {
                return this.states;
            }
        }

        public void AddState(StateNodeBase state)
        {
            this.states.AddLast(state);
        }

        public override ICollection<string> EvaluateState(RadItem targetItem)
        {
            ICollection<string> res = new LinkedList<string>();

            foreach (StateNodeBase state in this.states)
            {
                foreach (string stateName in state.EvaluateState(targetItem))
                {
                    res.Add(stateName);
                }
            }

            return res;
        }

        public override void AddAvailableStates(ICollection<StateDescriptionNode> newNodes, StateDescriptionNode node)
        {
            //Permutate all available group states
            LinkedListNode<StateNodeBase> stateLink = this.states.First;

            while (stateLink != null)
            {
                LinkedList<StateDescriptionNode> addedNodes = new LinkedList<StateDescriptionNode>();

                stateLink.Value.AddAvailableStates(addedNodes, node);
                LinkedListNode<StateNodeBase> nextLink = stateLink.Next;

                foreach (StateDescriptionNode newStateNode in addedNodes)
                {
                    newNodes.Add(newStateNode);
                }

                while (nextLink != null)
                {
                    LinkedList <StateDescriptionNode> innerNodes = new LinkedList<StateDescriptionNode>();
                    foreach (StateDescriptionNode newStateNode in addedNodes)
                    {
                        //add inner nodes
                        nextLink.Value.AddAvailableStates(innerNodes, newStateNode);
                    }

                    foreach(StateDescriptionNode innerNode in innerNodes)
                    {
                        newNodes.Add(innerNode);
                        addedNodes.AddFirst(innerNode);
                    }

                    nextLink = nextLink.Next;
                }

                stateLink = stateLink.Next;
            }
        }

        public override void AttachToElement(RadItem item, StateManagerAttachmentData attachmentData)
        {
            base.AttachToElement(item, attachmentData);

            if (this.states == null)
            {
                return;
            }

            foreach (StateNodeBase node in this.states)
            {
                node.AttachToElement(item, attachmentData);
            }
        }
    }
}
