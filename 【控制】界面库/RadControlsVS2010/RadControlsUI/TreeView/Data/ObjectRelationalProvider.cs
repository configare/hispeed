using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Telerik.WinControls.Data;

namespace Telerik.WinControls.UI
{
    internal class ObjectRelationalProvider : TreeNodeProvider
    {
        private string[] childMembers;
        private string[] displayMembers;

        public ObjectRelationalProvider(RadTreeViewElement treeView)
            :base(treeView)
        {
            Reset();
        }
         
        public override IList<RadTreeNode> GetNodes(RadTreeNode parent)
        {
            if (parent is RadTreeViewElement.RootTreeNode)
            {
                for (int i = 0; i < this.TreeView.ListSource.Count; i++)
                {
                    this.TreeView.ListSource[i].Parent = parent;
                }

                return this.TreeView.ListSource;
            }

            if(ResolveDescriptors(parent.BoundIndex + 1, parent.DataBoundItem))
            {
                List<RadTreeNode> nodes = new List<RadTreeNode>();
                IEnumerable children = this.TreeView.BoundDescriptors[parent.BoundIndex + 1].ChildDescriptor.GetValue(parent.DataBoundItem) as IEnumerable;
                if (children != null)
                {
                    if (parent.BoundIndex + 1 < this.displayMembers.Length)
                    {
                        PropertyDescriptorCollection properties = ListBindingHelper.GetListItemProperties(children);
                        this.TreeView.BoundDescriptors[parent.BoundIndex + 1].DisplayDescriptor = properties.Find(this.displayMembers[parent.BoundIndex + 1], true);
                    }

                    foreach (object item in children)
                    {
                        RadTreeNode node = this.TreeView.CreateNewNode();
                        ((IDataItem)node).DataBoundItem = item;
                        node.Parent = parent;
                        node.BoundIndex = parent.BoundIndex + 1;
                        nodes.Add(node);
                    }

                    parent.NodesLoaded = true;
                }

                return nodes;
            }

            return RadTreeNodeCollection.Empty;
        }

        public override void Reset()
        {
            if (string.IsNullOrEmpty(this.TreeView.ChildMember))
            {
                this.childMembers = new string[0];
                this.displayMembers = new string[0];
                return;
            }

            this.childMembers = this.TreeView.ChildMember.Split('\\');
            this.displayMembers = this.TreeView.DisplayMember.Split('\\');

            while (this.TreeView.BoundDescriptors.Count > 1)
            {
                this.TreeView.BoundDescriptors.RemoveAt(1);
            }
        }

        private bool ResolveDescriptors(int level, object dataBoundItem)
        {
            if (level < this.childMembers.Length)
            {
                if(level < this.TreeView.BoundDescriptors.Count)
                {
                    return true;
                }

                PropertyDescriptorCollection properties = ListBindingHelper.GetListItemProperties(dataBoundItem);
                PropertyDescriptor childDescriptor = properties.Find(this.childMembers[level], true);
                if (childDescriptor != null)
                {
                    TreeNodeDescriptor treeNodeDescriptor = new TreeNodeDescriptor();
                    treeNodeDescriptor.ChildDescriptor = childDescriptor;
                    this.TreeView.BoundDescriptors.Add(treeNodeDescriptor);
                    return true;
                }

                return false;
            }

            return false;
        }
    }
}
