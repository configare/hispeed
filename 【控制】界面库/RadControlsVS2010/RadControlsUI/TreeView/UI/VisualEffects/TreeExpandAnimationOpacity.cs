using System.Collections.Generic;

namespace Telerik.WinControls.UI
{
    public class TreeExpandAnimationOpacity : TreeExpandAnimation
    {
        #region Constructor

        public TreeExpandAnimationOpacity(RadTreeViewElement treeView)
            : base(treeView)
        {

        }

        #endregion

        #region Methods

        public override void Expand(RadTreeNode node)
        {
            this.UpdateViewOnExpandChanged(node);

            if (node.Nodes.Count == 0)
            {
                return;
            }

            List<TreeNodeElement> children = this.GetAssociatedNodes(node);

            for (int i = children.Count - 1; i >= 0; i--)
            {
                TreeNodeElement child = children[i];
                AnimatedPropertySetting animatedExpand = new AnimatedPropertySetting(RadItem.OpacityProperty, (double)1, (double)1, 4, 40);
                animatedExpand.StartValue = (double)0.0;
                animatedExpand.EndValue = (double)1.0;
                animatedExpand.RemoveAfterApply = true;
                animatedExpand.ApplyEasingType = RadEasingType.Linear;
                animatedExpand.UnapplyEasingType = RadEasingType.Linear;
                animatedExpand.ApplyValue(child);
            }
        }

        public override void Collapse(RadTreeNode node)
        {
            if (node.Nodes.Count == 0)
            {
                this.UpdateViewOnExpandChanged(node);
                return;
            }

            List<TreeNodeElement> children = this.GetAssociatedNodes(node);

            for (int i = children.Count - 1; i >= 0; i--)
            {
                TreeNodeElement child = children[i];

                TreeAnimatedPropertySetting animatedCollapse = new TreeAnimatedPropertySetting(RadItem.OpacityProperty,
                                                                                               (double)1, (double)1, 4, 40);
                animatedCollapse.StartValue = (double)1.0;
                animatedCollapse.EndValue = (double)0.0;
                animatedCollapse.ApplyEasingType = RadEasingType.Linear;
                animatedCollapse.UnapplyEasingType = RadEasingType.Linear;

                if (i == 0)
                {
                    animatedCollapse.AnimationFinished += new AnimationFinishedEventHandler(AnimatedCollapse_Finished);
                    animatedCollapse.Node = node;
                }

                animatedCollapse.ApplyValue(child);
            }
        }

        #endregion

        #region Event Handlers

        private void AnimatedCollapse_Finished(object sender, AnimationStatusEventArgs e)
        {
            TreeAnimatedPropertySetting setting = sender as TreeAnimatedPropertySetting;
            setting.AnimationFinished -= AnimatedCollapse_Finished;
            this.UpdateViewOnExpandChanged(setting.Node);
        }

        #endregion
    }
}
