using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;
using System.Drawing;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents a separator line which can be put between
    /// items in a <see cref="RadRibbonBarGroup"/> or <see cref="RadRibbonBarButtonGroup"/>.
    /// This separator is built of two <see cref="LinePrimitive"/>instances which are layout
    /// together to allow two-coloured separator appearance.
    /// </summary>
    public class RibbonBarGroupSeparator : RadItem
    {
        #region Nested

        public class RibbonBarGroupSeparatorStateManager : ItemStateManagerFactory
        {
            protected override StateNodeBase CreateSpecificStates()
            {
                CompositeStateNode compositeNode = new CompositeStateNode("SeparatorStates");
                StateNodeWithCondition stateNodeHorizontal = new StateNodeWithCondition("HorizontalOrientation", new SimpleCondition(RibbonBarGroupSeparator.OrientationProperty, Orientation.Horizontal));
                StateNodeWithCondition stateNodeVertical = new StateNodeWithCondition("VerticalOrientation", new SimpleCondition(RibbonBarGroupSeparator.OrientationProperty, Orientation.Vertical));
                stateNodeHorizontal.FalseStateLink = stateNodeVertical;
                compositeNode.AddState(stateNodeHorizontal);
                compositeNode.AddState(stateNodeVertical);
                return compositeNode;
            }
        }

        #endregion

        #region Ctor

        static RibbonBarGroupSeparator()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RibbonBarGroupSeparatorStateManager(), typeof(RibbonBarGroupSeparator));
        }

        #endregion

        #region Fields

        private BorderPrimitive separatorPrimitive;

        #endregion

        #region RadProperties

        public static RadProperty OrientationProperty = RadProperty.Register(
            "Orientation",
            typeof(Orientation),
            typeof(RibbonBarGroupSeparator),
            new RadElementPropertyMetadata(Orientation.Horizontal, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.CanInheritValue)
            );

        #endregion

        #region Initialization

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.Class = "GroupSeparator";
            this.separatorPrimitive = new BorderPrimitive();
            this.separatorPrimitive.Class = "SeparatorPrimitive";
            this.Children.Add(this.separatorPrimitive);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the orientation of the separator. A separator
        /// can be positioned vertical or horizontal.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(OrientationProperty);
            }
            set
            {
                this.SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="BorderPrimitive"/>class
        /// which represents the primitive that is used to paint the separator.
        /// </summary>
        public BorderPrimitive SeparatorPrimitive
        {
            get
            {
                return this.separatorPrimitive;
            }
        }

        #endregion

        #region Layouts

        protected override System.Drawing.SizeF MeasureOverride(System.Drawing.SizeF availableSize)
        {
            //SizeF result = SizeF.Empty;
            SizeF result = base.MeasureOverride(availableSize);
            switch(this.Orientation)
            {
                case Orientation.Horizontal:
                    result = new SizeF(this.BorderThickness.Horizontal, result.Height);
                    break;
                case Orientation.Vertical:
                    result = new SizeF(result.Width, this.BorderThickness.Vertical);
                    break;
            }

            return result;
        }

        #endregion
    }
}
