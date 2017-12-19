using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Drawing;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    internal class RadListVisualGroupItem : RadListVisualItem
    {

        class RadListVisualGroupItemStateManagerFactory : ItemStateManagerFactory
        {
            protected StateNodeBase CreateCollapseStates()
            {
                StateNodeWithCondition collapsibleState = new StateNodeWithCondition("Collapsible", new SimpleCondition(RadListVisualGroupItem.CollapsibleProperty, true));
                StateNodeWithCondition collapsedState = new StateNodeWithCondition("Collapsed", new SimpleCondition(RadListVisualGroupItem.CollapsedProperty, true));
                CompositeStateNode collapseStates = new CompositeStateNode("list group header item states");
                collapseStates.AddState(collapsibleState);
                collapseStates.AddState(collapsedState);
                return collapseStates;
            }

            public override StateNodeBase CreateRootState()
            {
                CompositeStateNode all = new CompositeStateNode("all states");
                all.AddState(this.CreateEnabledStates());
                all.AddState(this.CreateCollapseStates()); 
                all.AddState(this.CreateSpecificStates());

                StateNodeWithCondition enabled = new StateNodeWithCondition("Enabled", new SimpleCondition(RadElement.EnabledProperty, true));
                enabled.TrueStateLink = all;
                enabled.FalseStateLink = new StatePlaceholderNode("Disabled");

                return enabled;
            }

            protected override ItemStateManagerBase CreateStateManager()
            {
                ItemStateManagerBase sm = base.CreateStateManager();

                sm.AddDefaultVisibleState("Collapsible");
                sm.AddDefaultVisibleState("Collapsible.Collapsed");
                sm.AddDefaultVisibleState("MouseOver.Collapsible");
                sm.AddDefaultVisibleState("MouseDown.Collapsible"); 
                
                return sm;
            }
        }


        public static readonly RadProperty CollapsibleProperty =
            RadProperty.Register("Collapsible", typeof(bool), typeof(RadListVisualGroupItem),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsParentMeasure | ElementPropertyOptions.AffectsParentArrange));
     
        public static readonly RadProperty CollapsedProperty =
                    RadProperty.Register("Collapsed", typeof(bool), typeof(RadListVisualGroupItem),
                    new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsParentMeasure | ElementPropertyOptions.AffectsParentArrange));

        private LinePrimitive linePrimitive;

        static RadListVisualGroupItem()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadListVisualGroupItemStateManagerFactory(), typeof(RadListVisualGroupItem));
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
             
            this.linePrimitive = new LinePrimitive();
            linePrimitive.Alignment = ContentAlignment.MiddleCenter;
            linePrimitive.BackColor = Color.Black ;
            
            this.Children.Add(linePrimitive);
        }

        public override bool IsCompatible(RadListDataItem data, object context)
        {
            return (data is RadListDataGroupItem);
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
           // this.MinSize = new System.Drawing.Size(20, 20);
        }

        public bool Collapsible
        {
            get { return this.Data.Collapsible; }
            set { this.Data.Collapsible = value; }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (this.Collapsible)
            {
                this.Data.Collapsed ^= true;
            }
        }

        public new RadListDataGroupItem Data
        {
            get
            {
                return base.Data as RadListDataGroupItem;
            }
        }

        private const int endOffset = 4;

        protected override System.Drawing.SizeF ArrangeOverride(System.Drawing.SizeF finalSize)
        {
            RectangleF clientRect = GetClientRectangle(finalSize);

            this.layoutManagerPart.Arrange(clientRect);


            float layoutManagerWidth = this.layoutManagerPart.DesiredSize.Width;
            float linePrimitiveX = (this.RightToLeft) ? clientRect.Left + endOffset : clientRect.Left + layoutManagerWidth;
            
                this.linePrimitive.Arrange(
                 new RectangleF(linePrimitiveX, clientRect.Top + (clientRect.Height - this.linePrimitive.LineWidth) / 2,
                     clientRect.Width - layoutManagerWidth - endOffset, clientRect.Height));
            
            return finalSize;
        }

        public override void Attach(RadListDataItem data, object context)
        {
            this.BindProperty(RadListVisualGroupItem.CollapsibleProperty, data, RadListDataGroupItem.CollapsibleProperty, PropertyBindingOptions.OneWay);
            this.BindProperty(RadListVisualGroupItem.CollapsedProperty, data, RadListDataGroupItem.CollapsedProperty, PropertyBindingOptions.OneWay);  
            base.Attach(data, context); 
        }

        public override void Detach()
        {
            this.UnbindProperty(RadListVisualGroupItem.CollapsibleProperty);
            this.UnbindProperty(RadListVisualGroupItem.CollapsedProperty);
            base.Detach();
        }
    }
}
