using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Styles
{
    public class StateDescriptionNode
    {
        private string stateName;


        public virtual string StateName
        {
            get
            {
                return this.stateName;
            }
        }

        public StateDescriptionNode(string nodeStateName)
        {
            this.stateName = nodeStateName;
        }

        List<StateDescriptionNode> nodes = new List<StateDescriptionNode>(0);

        public List<StateDescriptionNode> Nodes
        {
            get
            {
                return this.nodes;
            }
        }

        public IEnumerable<string> GetStatesEnumerator()
        {
            return this.GetStatesEnumerator(string.Empty);
        }

        public IEnumerable<string> GetStatesEnumerator(string parentNodeStateName)
        {
            if (parentNodeStateName != String.Empty)
            {
                parentNodeStateName += ItemStateManagerBase.stateDelimiter;
            }

            string nodeFullName = parentNodeStateName + this.StateName;
            yield return nodeFullName;

            foreach (StateDescriptionNode child in this.nodes)
            {
                foreach (string childState in child.GetStatesEnumerator(nodeFullName))
                {
                    yield return childState;
                }
            }
        }

        public StateDescriptionNode AddNode(string stateName)
        {
            StateDescriptionNode res = new StateDescriptionNode(stateName);
            this.nodes.Add(res);
            return res;
        }

        public override string ToString()
        {
            return this.StateName;
        }
    }    
}
