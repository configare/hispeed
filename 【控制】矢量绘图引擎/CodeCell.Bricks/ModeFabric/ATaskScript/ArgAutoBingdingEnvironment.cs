using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.ModelFabric
{
    public enum enumArgType
    {
        Var,   //变量
        Ref,   //引用
        Value//常量
    }

    public enum enumVarType
    {
        Env
    }

    internal class ActionArg
    {
        //本Action的PropertyName
        public string Name = null;
        public enumArgType ArgType = enumArgType.Value;
        public enumVarType VarType = enumVarType.Env;
        //引用的Action的Id
        public int RefActionId = -1;
        //引用Action的PropertyName
        public string RefName = null;
        //常量
        public object Value = null;

        public ActionArg()
        { 
        }

        public ActionArg(string name)
            :this()
        {
            Name = name;
        }
    }

    /// <summary>
    /// 某一个Action的参数绑定关系集合
    /// </summary>
    internal class ActionArgCollection
    {
        //assemblyUrl", "classname", "actionAttribute
        public string AssemblyUrl = null;
        public string ClassName = null;
        public ActionAttribute ActionAttribute = null;
        //
        public int Id = -1;
        public ActionArg[] ActionArgs = null;

        public ActionArgCollection()
        { 
        }

        public ActionArgCollection(int id, ActionArg[] actionArgs)
        {
            Id = id;
            ActionArgs = actionArgs;
        }
    }

    /// <summary>
    /// 一个任务的参数绑定关系集合
    /// </summary>
    internal class ArgAutoBingdingEnvironment
    {
        public ActionArgCollection[] ActionArgSettings = null;

        public ArgAutoBingdingEnvironment()
        { 
        }

        public ArgAutoBingdingEnvironment(ActionArgCollection[] actionArgSettings)
        {
            ActionArgSettings = actionArgSettings;
        }

        public ActionArgCollection GetActionArgCollection(int action)
        {
            if (ActionArgSettings == null || ActionArgSettings.Length == 0)
                return null;
            foreach (ActionArgCollection argCollection in ActionArgSettings)
                if (argCollection.Id == action)
                    return argCollection;
            return null;
        }
    }
}
