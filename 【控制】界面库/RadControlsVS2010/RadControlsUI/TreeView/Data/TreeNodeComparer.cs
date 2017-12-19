using System;
using System.Collections.Generic;
using System.ComponentModel;
using Telerik.Data.Expressions;
using Telerik.WinControls.Data;

namespace Telerik.WinControls.UI
{
    public class TreeNodeComparer : IComparer<RadTreeNode>
    {
        private RadTreeViewElement treeView;
        private List<TreeNodeDescriptor> context = new List<TreeNodeDescriptor>();

        class TreeNodeDescriptor
        {
            public PropertyDescriptor Descriptor;
            public int Index;
            public bool Descending;
        }

        public TreeNodeComparer(RadTreeViewElement treeView)
        {
            this.treeView = treeView;

            this.Update();
        }

        public void Update()
        {
            this.context.Clear();

            if (!this.treeView.ListSource.IsDataBound)
            {
                return;
            }

            //if (this.treeView.IsValidParentMember)
            //{
            //    TreeNodeDescriptor descriptor = new TreeNodeDescriptor();
            //    descriptor.Descriptor = this.treeView.ListSource.BoundProperties[this.treeView.ParentBoundIndex];
            //    descriptor.Index = this.treeView.ParentBoundIndex;
            //    descriptor.Descending = false;
            //    this.context.Add(descriptor);
            //}

            for (int i = 0; i < this.treeView.SortDescriptors.Count; i++)
            {
                SortDescriptor sd = this.treeView.SortDescriptors[i];
                PropertyDescriptor pd = this.treeView.ListSource.BoundProperties.Find(sd.PropertyName, true);
                if (pd != null)
                {
                    TreeNodeDescriptor descriptor = new TreeNodeDescriptor();
                    descriptor.Descriptor = pd;
                    descriptor.Index = this.treeView.ListSource.BoundProperties.IndexOf(pd);
                    descriptor.Descending = (sd.Direction == ListSortDirection.Descending);
                    this.context.Add(descriptor);
                }
            }
        }

        #region IComparer<TreeNode> Members

        public int Compare(RadTreeNode x, RadTreeNode y)
        {
            //for (int i = 0; i < context.Count; i++)
            //{
                object xValue = x.Text;// context[i].Descriptor.GetValue(x.DataBoundItem);
                object yValue = y.Text;// context[i].Descriptor.GetValue(y.DataBoundItem);

                int result = 0;
                IComparable xCompVal = xValue as IComparable;
                if (xCompVal != null && yValue != null && yValue.GetType() == xValue.GetType())
                {
                    result = ((IComparable)xValue).CompareTo(yValue);
                }
                else
                {
                    result = DataStorageHelper.CompareNulls(xValue, yValue);
                }

                if (result != 0 && treeView.SortOrder == System.Windows.Forms.SortOrder.Descending)
                {
                    return -result;
                }

                return result;

                //if (result != 0)
                //{
                //    if (context[i].Descending)
                //    {
                //        return -result;
                //    }

                //    return result;
                //}
            //}

            //return 0;
        }

        #endregion
    }
}
