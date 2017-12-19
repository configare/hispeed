using System;
using System.ComponentModel;
using System.Drawing.Design;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Themes.Design;

namespace Telerik.WinControls.UI
{
    public class RadBreadCrumbElement : RadItem
    {
        private FillPrimitive fill;
        private BorderPrimitive border;
        private StripLayoutPanel strip;
        private ImagePrimitive imgPr;

        private RadItemOwnerCollection items;

        public static RadProperty SpacingBetweenItemsProperty = RadProperty.Register(
        "SpacingBetweenItems", typeof(int), typeof(RadBreadCrumbElement), new RadElementPropertyMetadata(
            2));

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.items = new RadItemOwnerCollection();
            this.items.ItemTypes = new Type[] { typeof(RadDropDownButtonElement) };
            this.items.ItemsChanged += new ItemChangedDelegate(items_ItemsChanged);
        }

        static RadBreadCrumbElement()
        {
            ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, "Telerik.WinControls.UI.Resources.UIElementsThemes.BreadCrumbThemes.VistaBreadCrumb.xml");
        }

        private void items_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            SetSpacingBetweenItems();
        }

        [DefaultValue(2)]
        [Browsable(true)]
        [Category("Layout")]
        [Description("Gets or sets the spacing between the items in the breadcrumb")]
        public int SpacingBetweenItems
        {
            get
            {
                return (int)this.GetValue(SpacingBetweenItemsProperty);
            }
            set
            {
                this.SetValue(SpacingBetweenItemsProperty, value);
            }
        }

        /// <summary>
        ///		Gets a collection of items which are children of the TabStrip element.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor))]
        [Description("Gets the collection of items which are children of the RadBreadCrumb element.")]
        public RadItemOwnerCollection Items
        {
            get
            {
                return this.items;
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (e.Property == SpacingBetweenItemsProperty)
            {
                SetSpacingBetweenItems();
            }


            base.OnPropertyChanged(e);

        }

        private void SetSpacingBetweenItems()
        {
            for (int i = 0; i < this.Items.Count - 1; i++)
            {
                RadItem item = this.Items[i];

                item.Margin = new System.Windows.Forms.Padding(item.Margin.Left, item.Margin.Top,
                    this.SpacingBetweenItems, item.Margin.Bottom);

            }
        }

        protected override void CreateChildElements()
        {
            this.fill = new FillPrimitive();
            this.fill.Class = "BreadCrumbFill";

            this.border = new BorderPrimitive();
            this.border.Class = "BreadCrumbBorder";
            this.strip = new StripLayoutPanel();
            this.imgPr = new ImagePrimitive();
            this.imgPr.Class = "BreadCrumbImage";

            this.fill.Visibility = ElementVisibility.Hidden;
            this.border.Visibility = ElementVisibility.Hidden;

            this.Children.Add(this.fill);
            this.Children.Add(this.border);
            this.Children.Add(this.strip);

            this.items.Owner = this.strip;
        }

    }

    public class RadBreadCrumbDesignTimeData : RadControlDesignTimeData
    {
        public RadBreadCrumbDesignTimeData()
        { }

        public RadBreadCrumbDesignTimeData(string name)
            : base(name)
        { }

        public override ControlStyleBuilderInfoList GetThemeDesignedControls(System.Windows.Forms.Control previewSurface)
        {
            RadTreeView tree = new RadTreeView();
            tree.Nodes.Add(new RadTreeNode("Node1"));
            tree.Nodes[0].Nodes.Add(new RadTreeNode("Node2"));
            tree.Nodes[0].Nodes.Add(new RadTreeNode("Node3"));
            tree.Nodes[0].Nodes.Add(new RadTreeNode("Node4"));
            tree.Nodes[0].Nodes.Add(new RadTreeNode("Node5"));


            RadBreadCrumb radBreadCrumb = new RadBreadCrumb();
            radBreadCrumb.Size = new System.Drawing.Size(200, 50);
            radBreadCrumb.AutoSize = true;

            radBreadCrumb.Text = "breadCrumb";

            RadBreadCrumb breadCrumbStructure = new RadBreadCrumb();
            breadCrumbStructure.Size = new System.Drawing.Size(200, 50);
            breadCrumbStructure.AutoSize = true;

            breadCrumbStructure.Text = "breadCrumb";

            radBreadCrumb.DefaultTreeView = tree;
            breadCrumbStructure.DefaultTreeView = tree;

            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(radBreadCrumb, breadCrumbStructure.RootElement);
            designed.MainElementClassName = typeof(RadBreadCrumbElement).FullName;

            ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

            tree.SelectedNode = tree.Nodes[0].Nodes[2];

            res.Add(designed);

            return res;
        }
    }
}
