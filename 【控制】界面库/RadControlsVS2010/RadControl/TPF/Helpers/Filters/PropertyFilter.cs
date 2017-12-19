using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls
{
    public class PropertyFilter : Filter
    {
        #region Static Members

        public static readonly PropertyFilter LayoutFilter;
        public static readonly PropertyFilter AppearanceFilter;
        public static readonly PropertyFilter BehaviorFilter;
        public static readonly PropertyFilter FillPrimitiveFilter;
        public static readonly PropertyFilter BorderPrimitiveFilter;
        public static readonly PropertyFilter ImagePrimitiveFilter;
        public static readonly PropertyFilter TextPrimitiveFilter;
        public static readonly PropertyFilter ExcludeFilter;

        #endregion

        #region Fields

        private Dictionary<int, RadProperty> availableProperties;
        private UnaryOperator unaryOperator;

        #endregion

        #region Constructor/Initializer

        public PropertyFilter()
        {
            this.availableProperties = new Dictionary<int,RadProperty>();
            this.unaryOperator = UnaryOperator.None;
        }

        public PropertyFilter(IEnumerable<RadProperty> properties)
            : this()
        {
            foreach (RadProperty property in properties)
            {
                if (!this.availableProperties.ContainsKey(property.NameHash))
                {
                    this.availableProperties.Add(property.NameHash, property);
                }
            }
        }

        static PropertyFilter()
        {
            ExcludeFilter = new PropertyFilter();
            ExcludeFilter.unaryOperator = UnaryOperator.NotOperator;
            AddExcludeProperties();

            LayoutFilter = new PropertyFilter(
                new RadProperty[] {
                        RadElement.AngleTransformProperty,
                        RadElement.BorderThicknessProperty,
                        RadElement.MarginProperty,
                        RadElement.MaxSizeProperty,
                        RadElement.MinSizeProperty,
                        RadElement.PaddingProperty,
                        RadElement.PositionOffsetProperty,
                        RadElement.ScaleTransformProperty,
                        RadElement.StretchHorizontallyProperty,
                        RadElement.StretchVerticallyProperty });

            AppearanceFilter = new PropertyFilter(
                new RadProperty[] {
                        RadElement.ShapeProperty,
                        VisualElement.BackColorProperty,
                        VisualElement.ForeColorProperty,
                        VisualElement.FontProperty,
                        VisualElement.TextRenderingHintProperty,
                        VisualElement.SmoothingModeProperty,
                        VisualElement.OpacityProperty });

            BehaviorFilter = new PropertyFilter(
                new RadProperty[] { 
                        RadElement.ShouldPaintProperty, 
                        RadElement.ZIndexProperty, 
                        RadElement.VisibilityProperty });

            FillPrimitiveFilter = new PropertyFilter(GetPropertiesDeclaredByType(typeof(FillPrimitive)));
            FillPrimitiveFilter.availableProperties.Add(RadElement.MarginProperty.NameHash, RadElement.MarginProperty);

            BorderPrimitiveFilter = new PropertyFilter(GetPropertiesDeclaredByType(typeof(BorderPrimitive)));
            BorderPrimitiveFilter.availableProperties.Add(RadElement.BorderThicknessProperty.NameHash, RadElement.BorderThicknessProperty);
            BorderPrimitiveFilter.availableProperties.Add(RadElement.MarginProperty.NameHash, RadElement.MarginProperty);

            ImagePrimitiveFilter = new PropertyFilter(GetPropertiesDeclaredByType(typeof(ImagePrimitive)));
            ImagePrimitiveFilter.availableProperties.Add(RadElement.MarginProperty.NameHash, RadElement.MarginProperty);
            ImagePrimitiveFilter.availableProperties.Add(RadElement.ShouldPaintProperty.NameHash, RadElement.ShouldPaintProperty);

            TextPrimitiveFilter = new PropertyFilter(GetPropertiesDeclaredByType(typeof(TextPrimitive)));
            TextPrimitiveFilter.availableProperties.Add(RadElement.MarginProperty.NameHash, RadElement.MarginProperty);
            TextPrimitiveFilter.availableProperties.Add(RadElement.ShouldPaintProperty.NameHash, RadElement.ShouldPaintProperty);
            TextPrimitiveFilter.availableProperties.Add(VisualElement.ForeColorProperty.NameHash, VisualElement.ForeColorProperty);
            TextPrimitiveFilter.availableProperties.Remove(TextPrimitive.TextProperty.NameHash);
        }

        private static void AddExcludeProperties()
        {
            ExcludeFilter.availableProperties.Add("AccessibleDescription".GetHashCode(), null);
            ExcludeFilter.availableProperties.Add("AccessibleName".GetHashCode(), null);
            ExcludeFilter.availableProperties.Add("AccessibleRole".GetHashCode(), null);
            ExcludeFilter.availableProperties.Add("AutoToolTip".GetHashCode(), null);
            ExcludeFilter.availableProperties.Add(RadElement.CanFocusProperty.NameHash, RadElement.CanFocusProperty);
            ExcludeFilter.availableProperties.Add(RadItem.ClickModeProperty.NameHash, RadItem.ClickModeProperty);
            ExcludeFilter.availableProperties.Add("CommandBinding".GetHashCode(), null);
            ExcludeFilter.availableProperties.Add(RadElement.EnabledProperty.NameHash, RadElement.EnabledProperty);
            ExcludeFilter.availableProperties.Add("IsSharedImage".GetHashCode(), null);
            ExcludeFilter.availableProperties.Add("KeyTip".GetHashCode(), null);
            ExcludeFilter.availableProperties.Add("ScreenTip".GetHashCode(), null);
            ExcludeFilter.availableProperties.Add(RadItem.TextProperty.NameHash, RadItem.TextProperty);
            ExcludeFilter.availableProperties.Add("ToolTipText".GetHashCode(), null);
            ExcludeFilter.availableProperties.Add("DataBindings".GetHashCode(), null);
            ExcludeFilter.availableProperties.Add("Children".GetHashCode(), null);
            ExcludeFilter.availableProperties.Add("Tag".GetHashCode(), null);
            ExcludeFilter.availableProperties.Add("SerializeChildren".GetHashCode(), null);
            ExcludeFilter.availableProperties.Add(RadElement.BoundsProperty.NameHash, RadElement.BoundsProperty);
            ExcludeFilter.availableProperties.Add("Location".GetHashCode(), null);
            ExcludeFilter.availableProperties.Add("Size".GetHashCode(), null);
            ExcludeFilter.availableProperties.Add("UseSystemSkin".GetHashCode(), null);
            ExcludeFilter.availableProperties.Add(RadElement.ClassProperty.NameHash, RadElement.ClassProperty);
            ExcludeFilter.availableProperties.Add(RadElement.StyleProperty.NameHash, RadElement.StyleProperty);
        }

        #endregion

        #region Properties

        public Dictionary<int, RadProperty> AvailableProperties
        {
            get
            {
                return this.availableProperties;
            }
        }

        #endregion

        #region Implementation

        public static void AddPropertiesDeclaredByType(Dictionary<int, RadProperty> properties, Type type)
        {
            foreach (RadProperty property in GetPropertiesDeclaredByType(type))
            {
                if (!properties.ContainsKey(property.NameHash))
                {
                    properties.Add(property.NameHash, property);
                }
            }
        }

        public static IEnumerable<RadProperty> GetPropertiesDeclaredByType(Type type)
        {
            //properties, declared in FillPrimitive
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (FieldInfo fi in fields)
            {
                RadProperty property = fi.GetValue(null) as RadProperty;
                if (property != null)
                {
                    yield return property;
                }
            }
        }

        public override bool Match(object obj)
        {
            PropertyDescriptor propDesc = obj as PropertyDescriptor;
            if (propDesc == null)
            {
                return false;
            }

            bool containsKey = this.availableProperties.ContainsKey(propDesc.Name.GetHashCode());
            if (this.unaryOperator == UnaryOperator.NotOperator)
            {
                return !containsKey;
            }

            return containsKey;
        }

        #endregion
    }
}
