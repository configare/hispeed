using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Telerik.WinControls.UI.Carousel
{
    public class CarouselPathConverter : ReferenceConverter
    {
        public CarouselPathConverter()
            : base(typeof(ICarouselPath))
        {
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            ArrayList list = new ArrayList();
            //list.Add(new BezierCalculator());
            //list.Add(new EllipseCalculator());

            StandardValuesCollection baseList = base.GetStandardValues(context);
            if (baseList != null)
            {
                for (int i = 0; i < baseList.Count; i++)
                {
                    if (i > 0) //skip the first item because it is always "(none)"
                    {
                        object item = baseList[i];
                        list.Add(item);
                    }
                }
            }

            return new StandardValuesCollection(list);
        }

        //Expandable object converter methods
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(value, attributes);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        //----
    }
}
