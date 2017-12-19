using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;

namespace CodeCell.Bricks.ModelFabric
{
    public abstract class PropertyValue
    {
        public enumArgType ArgType = enumArgType.Value;
    }

    public class PropertyValueByValue : PropertyValue
    {
        public object Value = null;

        public override string ToString()
        {
            return Value != null ? Value.ToString() : string.Empty;
        }
    }

    public class PropertyValueByVar : PropertyValue
    {
        public enumVarType VarType = enumVarType.Env;
        public string VarValue = null;

        public override string ToString()
        {
            return "Var:"+VarType.ToString() + "@" + VarValue.ToString();
        }
    }

    public class PropertyValueByRef : PropertyValue
    {
        public IAction RefAction = null;
        public PropertyInfo RefPropertyInfo = null;
        public BindingAttribute RefBindingAttribute = null;

        public override string ToString()
        {
            return "Ref:" + RefAction.Guid.ToString() + "@" + RefPropertyInfo.Name;
        }

        public string ToString(int id)
        {
            return "Ref:" + id.ToString() + "@" + RefPropertyInfo.Name;
        }
    }

    public class BindingPair
    {
        public PropertyInfo PropertyInfo = null;
        public PropertyValue PropertyValue = null;

        public BindingPair(PropertyInfo propertyInfo, PropertyValue propertyValue)
        {
            PropertyInfo = propertyInfo;
            PropertyValue = propertyValue;
        }

        public override string ToString()
        {
            string v = "<Arg name =\"{0}\" value=\"{1}\" />";
            return string.Format(v, PropertyInfo.Name, PropertyValue.ToString());
        }
    }
}
