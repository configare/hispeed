using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Styles
{
    public class VoidStateNode: StateNodeBase
    {
        public VoidStateNode()
            : base("No State")
        {
        }

        public override ICollection<string> EvaluateState(RadItem targetItem)
        {
            return new string[0];
        }

        public override void AddAvailableStates(ICollection<StateDescriptionNode> newNodes, StateDescriptionNode node)
        {
            //void
        }
    }
}
