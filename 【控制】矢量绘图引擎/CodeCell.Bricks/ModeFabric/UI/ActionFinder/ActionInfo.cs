using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CodeCell.Bricks.ModelFabric
{
    public class ActionInfo:ICloneable
    {
        public string AssemblyUrl = null;
        public string ClassName = null;
        public ActionAttribute ActionAttribute = null;

        public ActionInfo(string assemblyUrl, string className, ActionAttribute name)
        {
            AssemblyUrl = assemblyUrl;
            ClassName = className;
            ActionAttribute = name;
        }

        public override string ToString()
        {
            return ActionAttribute.Name;
        }

        public IAction ToAction()
        {
            Assembly ass = Assembly.LoadFrom(AssemblyUrl);
            object obj = ass.CreateInstance(ClassName);
            return obj as IAction;
        }

        #region ICloneable 成员

        public object Clone()
        {
            return new ActionInfo(AssemblyUrl, ClassName, ActionAttribute);
        }

        #endregion
    }
}
