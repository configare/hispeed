using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Security;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;

namespace Telerik.WinControls.Layout
{
    public class LengthConverter : TypeConverter
    {
        // Fields
        private static float[] PixelUnitFactors = new float[] { 1f, 96f, 37.795275590551178f, 1.3333333333333333f };
        private static string[] PixelUnitStrings = new string[] { "px", "in", "cm", "pt" };

        // Methods
        public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
        {
            switch (Type.GetTypeCode(sourceType))
            {
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.String:
                    return true;
            }
            return false;
        }

        public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType)
        {
            if ((destinationType != typeof(InstanceDescriptor)) && (destinationType != typeof(string)))
            {
                return false;
            }
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source)
        {
            if (source == null)
            {
                throw base.GetConvertFromException(source);
            }
            if (source is string)
            {
                return FromString((string)source, cultureInfo);
            }
            return Convert.ToDouble(source, cultureInfo);
        }

        [SecurityCritical]
        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if ((value != null) && (value is float))
            {
                float num = (float)value;
                if (destinationType == typeof(string))
                {
                    if (float.IsNaN(num))
                    {
                        return "Auto";
                    }
                    return Convert.ToString(num, cultureInfo);
                }
                if (destinationType == typeof(InstanceDescriptor))
                {
                    return new InstanceDescriptor(typeof(float).GetConstructor(new Type[] { typeof(float) }), new object[] { num });
                }
            }
            throw base.GetConvertToException(value, destinationType);
        }

        internal static float FromString(string s, CultureInfo cultureInfo)
        {
            float num6;
            string text = s.Trim();
            string text2 = text.ToLowerInvariant();
            int length = text2.Length;
            int num2 = 0;
            float num3 = 1;
            if (text2 == "auto")
            {
                return float.NaN;
            }
            for (int i = 0; i < PixelUnitStrings.Length; i++)
            {
                if (text2.EndsWith(PixelUnitStrings[i], StringComparison.Ordinal))
                {
                    num2 = PixelUnitStrings[i].Length;
                    num3 = PixelUnitFactors[i];
                    break;
                }
            }
            text = text.Substring(0, length - num2);
            try
            {
                float num5 = Convert.ToSingle(text, cultureInfo) * num3;
                num6 = num5;
            }
            catch (FormatException)
            {
                throw new FormatException(SR.GetString("LengthFormatError", new object[] { text }));
            }
            return num6;
        }

        internal static string ToString(float l, CultureInfo cultureInfo)
        {
            if (float.IsNaN(l))
            {
                return "Auto";
            }
            return Convert.ToString(l, cultureInfo);
        }
    }
    public class WrapLayoutPanel : LayoutPanel
    {
        // Fields
        private Orientation orientation;
        private bool stretchItems = false;

        public static readonly RadProperty ItemHeightProperty = RadProperty.Register(
            "ItemHeight", typeof(float), typeof(WrapLayoutPanel),
            new RadElementPropertyMetadata(float.NaN, ElementPropertyOptions.AffectsMeasure),
            new ValidateValueCallback(WrapLayoutPanel.IsWidthHeightValid));

        public static readonly RadProperty ItemWidthProperty = RadProperty.Register(
            "ItemWidth", typeof(float), typeof(WrapLayoutPanel),
            new RadElementPropertyMetadata(float.NaN, ElementPropertyOptions.AffectsMeasure),
            new ValidateValueCallback(WrapLayoutPanel.IsWidthHeightValid));

        public static readonly RadProperty OrientationProperty = StackLayoutPanel.OrientationProperty.AddOwner(
            typeof(WrapLayoutPanel),
            new RadElementPropertyMetadata(Orientation.Horizontal, ElementPropertyOptions.AffectsMeasure,
            new PropertyChangedCallback(WrapLayoutPanel.OnOrientationChanged)));

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.orientation = (Orientation)OrientationProperty.GetMetadata(this).GetDefaultValue(this, OrientationProperty);
        }

        private int ElementComparisonBySizeDesc(RadElement element1, RadElement element2)
        {
            UVSize size1 = new UVSize(this.Orientation, element1.DesiredSize.Width, element1.DesiredSize.Height);
            UVSize size2 = new UVSize(this.Orientation, element2.DesiredSize.Width, element2.DesiredSize.Height);
            return size1.U.CompareTo(size2.U) * -1;
        }

        private void ArrangeLine(float VPosition, float lineV, int start, int end, bool useItemU, float itemU, float totalU)
        {
            float currentUPosition = 0;
            bool isHorizontal = this.Orientation == Orientation.Horizontal;
            RadElementCollection internalChildren = base.Children;
            float stretchedItemU = 0;
            if (this.stretchItems)
            {
                stretchedItemU = DetermineStretchedItemU(start, end, internalChildren, totalU);
            }

            for (int i = start; i < end; i++)
            {
                RadElement element = internalChildren[i];
                if (element != null)
                {
                    UVSize size = new UVSize(this.Orientation, element.DesiredSize.Width, element.DesiredSize.Height);
                    float actualItemU = useItemU ? itemU : size.U;

                    if (this.stretchItems && element.Visibility != ElementVisibility.Collapsed && actualItemU < stretchedItemU)
                    {
                        actualItemU = stretchedItemU;
                    }

                    RectangleF rect = new RectangleF(isHorizontal ? currentUPosition : VPosition,
                        isHorizontal ? VPosition : currentUPosition,
                        isHorizontal ? actualItemU : lineV,
                        isHorizontal ? lineV : actualItemU);
                    if (this.RightToLeft)
                    {
                        if (isHorizontal)
                        {
                            rect.X = totalU - rect.X - rect.Width;
                        }
                    }

                    element.Arrange(rect);
                    currentUPosition += actualItemU;   
                }
            }
        }

        private float DetermineStretchedItemU(int start, int end, RadElementCollection internalChildren, float totalU)
        {
            List<RadElement> childrenSorted = new List<RadElement>();
            for (int i = start; i < end; i++)
            {
                RadElement element = internalChildren[i];
                if (element != null && element.Visibility != ElementVisibility.Collapsed)
                {
                    childrenSorted.Add(element);
                }
            }

            int itemsCount = childrenSorted.Count;
            float stretchedItemU = totalU / itemsCount;
            float totalStretchU = totalU;
            childrenSorted.Sort(new Comparison<RadElement>(ElementComparisonBySizeDesc));

            for (int i = 0; i < childrenSorted.Count; i++)
            {
                RadElement element = childrenSorted[i];
                UVSize size = new UVSize(this.Orientation, element.DesiredSize.Width, element.DesiredSize.Height);
                if (size.U > stretchedItemU)
                {
                    totalStretchU -= size.U;
                    itemsCount--;
                    stretchedItemU = totalStretchU / itemsCount;
                }
            }
            return stretchedItemU;
        }


        protected void FillList(List<PreferredSizeData> list, Size availableSize)
        {
            foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
            {
                list.Add(new PreferredSizeData(child, availableSize));
            }
        }

        public Size GetMaxSize()
        {
            List<PreferredSizeData> list = new List<PreferredSizeData>();
            FillList(list, this.AvailableSize);
            return GetMaxSize(list);
        }

        protected Size GetMaxSize(List<PreferredSizeData> list)
        {
            Size maxSize = Size.Empty;
            foreach (PreferredSizeData data in list)
            {
                maxSize.Width = Math.Max(maxSize.Width, data.PreferredSize.Width + data.Element.Margin.Horizontal);
                maxSize.Height = Math.Max(maxSize.Height, data.PreferredSize.Height + data.Element.Margin.Vertical);
            }
            return maxSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            int start = 0;
            float itemWidth = this.ItemWidth;
            float itemHeight = this.ItemHeight;
            float v = 0;
            float itemU = (this.Orientation == Orientation.Horizontal) ? itemWidth : itemHeight;
            float totalSize = (this.Orientation == Orientation.Horizontal) ? finalSize.Width : finalSize.Height; 
            UVSize size = new UVSize(this.Orientation);
            UVSize size2 = new UVSize(this.Orientation, finalSize.Width, finalSize.Height);
            bool flag = !float.IsNaN(itemWidth);
            bool flag2 = !float.IsNaN(itemHeight);
            bool useItemU = (this.Orientation == Orientation.Horizontal) ? flag : flag2;
            RadElementCollection internalChildren = base.Children;
            int end = 0;
            int count = internalChildren.Count;
            while (end < count)
            {
                RadElement element = internalChildren[end];
                if (element != null)
                {
                    UVSize size3 = new UVSize(this.Orientation, flag ? itemWidth : element.DesiredSize.Width, flag2 ? itemHeight : element.DesiredSize.Height);
                    if (DoubleUtil.GreaterThan(size.U + size3.U, size2.U))
                    {
                        this.ArrangeLine(v, size.V, start, end, useItemU, itemU, totalSize);
                        v += size.V;
                        size = size3;
                        if (DoubleUtil.GreaterThan(size3.U, size2.U))
                        {
                            this.ArrangeLine(v, size3.V, end, ++end, useItemU, itemU, totalSize);
                            v += size3.V;
                            size = new UVSize(this.Orientation);
                        }
                        start = end;
                    }
                    else
                    {
                        size.U += size3.U;
                        size.V = Math.Max(size3.V, size.V);
                    }
                }
                end++;
            }
            if (start < internalChildren.Count)
            {
                this.ArrangeLine(v, size.V, start, internalChildren.Count, useItemU, itemU, totalSize);
            }
            return finalSize;
        }

        private static bool IsWidthHeightValid(object value, RadObject obj)
        {
            float num = (float)value;
            if (float.IsNaN(num))
            {
                return true;
            }
            if (num >= 0)
            {
                return !float.IsPositiveInfinity(num);
            }
            return false;
        }

        protected override SizeF MeasureOverride(SizeF constraint)
        {
            UVSize size = new UVSize(this.Orientation);
            UVSize size2 = new UVSize(this.Orientation);
            UVSize size3 = new UVSize(this.Orientation, constraint.Width, constraint.Height);
            float itemWidth = this.ItemWidth;
            float itemHeight = this.ItemHeight;
            bool flag = !float.IsNaN(itemWidth);
            bool flag2 = !float.IsNaN(itemHeight);
            SizeF availableSize = new SizeF(flag ? itemWidth : constraint.Width, flag2 ? itemHeight : constraint.Height);
            RadElementCollection internalChildren = base.Children;
            int num3 = 0;
            int count = internalChildren.Count;
            while (num3 < count)
            {
                RadElement element = internalChildren[num3];
                if (element != null)
                {
                    element.Measure(availableSize);
                    UVSize size5 = new UVSize(this.Orientation, flag ? itemWidth : element.DesiredSize.Width, flag2 ? itemHeight : element.DesiredSize.Height);
                    if (DoubleUtil.GreaterThan(size.U + size5.U, size3.U))
                    {
                        size2.U = Math.Max(size.U, size2.U);
                        size2.V += size.V;
                        size = size5;
                        if (DoubleUtil.GreaterThan(size5.U, size3.U))
                        {
                            size2.U = Math.Max(size5.U, size2.U);
                            size2.V += size5.V;
                            size = new UVSize(this.Orientation);
                        }
                    }
                    else
                    {
                        size.U += size5.U;
                        size.V = Math.Max(size5.V, size.V);
                    }
                }
                num3++;
            }
            size2.U = Math.Max(size.U, size2.U);
            size2.V += size.V;
            return new SizeF(size2.Width, size2.Height);
        }

        private static void OnOrientationChanged(RadObject d, RadPropertyChangedEventArgs e)
        {
            WrapLayoutPanel panel = (WrapLayoutPanel)d;
            panel.orientation = (Orientation)e.NewValue;
        }

        // Properties
        public bool StretchItems
        {
            get
            {
                return this.stretchItems;
            }
            set
            {
                if (this.stretchItems != value)
                {
                    this.stretchItems = value;
                    this.InvalidateMeasure();
                }
            }
        }

        [TypeConverter(typeof(LengthConverter))]
        public float ItemHeight
        {
            get
            {
                return (float)base.GetValue(ItemHeightProperty);
            }
            set
            {
                base.SetValue(ItemHeightProperty, value);
            }
        }

        [TypeConverter(typeof(LengthConverter))]
        public float ItemWidth
        {
            get
            {
                return (float)base.GetValue(ItemWidthProperty);
            }
            set
            {
                base.SetValue(ItemWidthProperty, value);
            }
        }

        public Orientation Orientation
        {
            get
            {
                return this.orientation;
            }
            set
            {
                base.SetValue(OrientationProperty, value);
            }
        }

        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        private struct UVSize
        {
            internal float U;
            internal float V;
            private Orientation orientation;
            internal UVSize(Orientation orientation, float width, float height)
            {
                this.U = this.V = 0;
                this.orientation = orientation;
                this.Width = width;
                this.Height = height;
            }

            internal UVSize(Orientation orientation)
            {
                this.U = this.V = 0;
                this.orientation = orientation;
            }

            internal float Width
            {
                get
                {
                    if (this.orientation != Orientation.Horizontal)
                    {
                        return this.V;
                    }
                    return this.U;
                }
                set
                {
                    if (this.orientation == Orientation.Horizontal)
                    {
                        this.U = value;
                    }
                    else
                    {
                        this.V = value;
                    }
                }
            }
            internal float Height
            {
                get
                {
                    if (this.orientation != Orientation.Horizontal)
                    {
                        return this.U;
                    }
                    return this.V;
                }
                set
                {
                    if (this.orientation == Orientation.Horizontal)
                    {
                        this.V = value;
                    }
                    else
                    {
                        this.U = value;
                    }
                }
            }
        }
    }

    
}