using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Paint;

namespace Telerik.WinControls
{
    public class InnerObjectConverter : ReferenceConverter
    {
        
        public InnerObjectConverter(Type innerObjectType)
            : base(innerObjectType)
        {
        }

        //Does not work as expected

        //public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        //{
        //    return true;
        //}

        //public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        //{
        //    StandardValuesCollection baseValue = base.GetStandardValues(context);

        //    ArrayList list = new ArrayList();
        //    list.Add(Activator.CreateInstance(context.PropertyDescriptor.PropertyType));
        //    list.AddRange(baseValue);

        //    return new StandardValuesCollection(list);
        //}

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
