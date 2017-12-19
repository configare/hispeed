using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Drawing;

namespace CodeCell.Bricks.ModelFabric
{
    public interface IBindingEnvironment
    {
        Dictionary<IAction, PropertyInfo[]> QueryCompatibleProperty(IAction action, BindingAttribute bindingAttribute,PropertyInfo propertyInfo);
        void UpdateBindingPair(IAction act, BindingPair[] bindingPairs);
        BindingPair[] GetBindingPairs(IAction action);
    }
}
