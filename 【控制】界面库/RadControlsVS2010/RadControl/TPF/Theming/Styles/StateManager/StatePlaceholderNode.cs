using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Styles
{
    public class StatePlaceholderNode : StateNodeBase
    {
        public StatePlaceholderNode(string name)
            : base(name)
        {
        }

        public override ICollection<string> EvaluateState(RadItem targetItem)
        {
            return new string[1] { this.Name };
        }

        public override void AddAvailableStates(ICollection<StateDescriptionNode> newNodes, StateDescriptionNode node)
        {
            newNodes.Add(node.AddNode(this.Name));
        }        
    }
}
