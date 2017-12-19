
namespace Telerik.WinControls.UI
{
    public class TreeNodeImageElement : LightVisualElement
    {
        #region Dependency Properties

        public static RadProperty IsSelectedProperty = RadProperty.Register(
            "IsSelected", typeof(bool), typeof(TreeNodeImageElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty IsCurrentProperty = RadProperty.Register(
            "IsCurrent", typeof(bool), typeof(TreeNodeImageElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty IsExpandedProperty = RadProperty.Register(
            "IsExpanded", typeof(bool), typeof(TreeNodeImageElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty IsRootNodeProperty = RadProperty.Register(
            "IsRootNode", typeof(bool), typeof(TreeNodeImageElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty HotTrackingProperty = RadProperty.Register(
            "HotTracking", typeof(bool), typeof(TreeNodeImageElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty HasChildrenProperty = RadProperty.Register(
            "HasChildren", typeof(bool), typeof(TreeNodeImageElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        #endregion

        #region Properties

        public TreeNodeElement NodeElement
        {
            get { return FindAncestor<TreeNodeElement>(); }
        }

        #endregion

        #region Initialization

        static TreeNodeImageElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new TreeNodeImageElementStateManager(), typeof(TreeNodeImageElement));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.Class = "ImageElement";
            this.NotifyParentOnMouseInput = true;
            this.SetDefaultValueOverride(DrawFillProperty, false);
            this.SetDefaultValueOverride(DrawBorderProperty, false);
            this.SetDefaultValueOverride(ImageLayoutProperty, System.Windows.Forms.ImageLayout.Center);
        }

        #endregion

        #region Methods

        public virtual void Synchronize()
        {
            TreeNodeElement nodeElement = this.NodeElement;

            if (nodeElement == null)
            {
                return;
            }

            this.SetValue(IsSelectedProperty, nodeElement.IsSelected);
            this.SetValue(IsCurrentProperty, nodeElement.IsCurrent);
            this.SetValue(IsExpandedProperty, nodeElement.IsExpanded);
            this.SetValue(IsRootNodeProperty, nodeElement.IsRootNode);
            this.SetValue(HotTrackingProperty, nodeElement.HotTracking);
            this.SetValue(HasChildrenProperty, nodeElement.HasChildren);

            if (nodeElement.Data.Image != null)
            {
                this.Image = nodeElement.Data.Image;
            }
            else
            {
                this.ResetValue(LightVisualElement.ImageProperty, ValueResetFlags.Local);
            }
        }

        #endregion
    }
}
