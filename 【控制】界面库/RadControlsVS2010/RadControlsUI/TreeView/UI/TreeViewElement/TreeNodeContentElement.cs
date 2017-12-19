using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.UI.StateManagers;

namespace Telerik.WinControls.UI
{
    public class TreeNodeContentElement: TreeViewVisual
    {
        #region Dependency properties

        public static RadProperty IsRootNodeProperty = RadProperty.Register(
            "IsRootNode", typeof(bool), typeof(TreeNodeContentElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty IsControlInactiveProperty = RadProperty.Register(
            "IsControlInactive", typeof(bool), typeof(TreeNodeContentElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty FullRowSelectProperty = RadProperty.Register(
            "FullRowSelect", typeof(bool), typeof(TreeNodeContentElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty IsSelectedProperty = RadProperty.Register(
            "IsSelected", typeof(bool), typeof(TreeNodeContentElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty IsCurrentProperty = RadProperty.Register(
            "IsCurrent", typeof(bool), typeof(TreeNodeContentElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty HotTrackingProperty = RadProperty.Register(
            "HotTracking", typeof(bool), typeof(TreeNodeContentElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));
        
        public static RadProperty IsExpandedProperty = RadProperty.Register(
            "IsExpanded", typeof(bool), typeof(TreeNodeContentElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty HasChildrenProperty = RadProperty.Register(
            "HasChildren", typeof(bool), typeof(TreeNodeContentElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        #endregion

        #region Fields

        private SizeF fullDesiredSize;

        #endregion

        #region Initialization

        static TreeNodeContentElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new TreeNodeContentElementStateManager(), typeof(TreeNodeContentElement));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.StretchHorizontally = false;            
            this.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.TextAlignment = ContentAlignment.MiddleLeft;
            this.AutoEllipsis = true;
        }

        #endregion

        #region Properties

        public TreeNodeElement NodeElement
        {
            get { return FindAncestor<TreeNodeElement>(); }
        }

        /// <summary>
        /// Gets or sets the full desired size calculated by the virtualized container
        /// </summary>
        public SizeF FullDesiredSize
        {
            get { return this.fullDesiredSize; }
            set { this.fullDesiredSize = value; }
        }

        #endregion

        #region Methods

        public virtual void Synchronize()
        {
            TreeNodeElement treeNodeElement = NodeElement;
            if (treeNodeElement == null || treeNodeElement.Data == null)
            {
                return;
            }

            this.Text = treeNodeElement.Data.Text;
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF desiredSize = base.MeasureOverride(availableSize);

            desiredSize.Width = Math.Min(desiredSize.Width, availableSize.Width);
            desiredSize.Height = Math.Min(desiredSize.Height, availableSize.Height);

            if (fullDesiredSize != SizeF.Empty && fullDesiredSize.Width > availableSize.Width)
            {
                desiredSize.Width = availableSize.Width;
            }
            
            return desiredSize;
        }

        #endregion
    }
}
