namespace Telerik.Data.Expressions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlTypes;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    class FunctionNode : ExpressionNode
    {
        ExpressionNode node;
        string name;
        List<ExpressionNode> arguments;
        MethodInfo method;

        public override bool IsConst
        {
            get
            {
                if (null != this.arguments)
                {
                    for (int i = 0; i < this.arguments.Count; i++)
                    {
                        if (!this.arguments[i].IsConst)
                            return false;
                    }
                }
                return true;
            }
        }

        public List<ExpressionNode> Arguments
        {
            get { return this.arguments; }
        }
        
        public string Name
        {
            get { return this.name; }
        }

        public bool IsGlobal
        {
            get { return (this.node == null); }
        }

        public FunctionNode(ExpressionNode node, string name)
        {
            this.node = node;
            this.name = name;
        }

        /// <summary>
        /// Normalize the value of the function's argument 
        /// to ensure the correct overload is matched.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static object NormalizeValue(object value)
        {
            // Converting DBNull to NULL 
            // DBNull comes from the db and cannot be used when an
            // argument of a reference type is needed: e.g. GetDecodedText(string text) 
            // while it is expected this method to be called with NULL.
            if (DBNull.Value == value)
            {
                return null;
            }
            return value;
        }

        public override object Eval(object row, object context)
        {
            ArrayList list = new ArrayList();
            if (null != this.arguments && this.arguments.Count > 0)
            {
                for (int i = 0; i < this.arguments.Count; i++)
                {
                    list.Add(NormalizeValue(this.arguments[i].Eval(row, context)));
                }
            }

            object[] args = list.ToArray();

            object target = context;
            if (null != this.node)
            {
                target = this.node.Eval(row, context);
            }

            if (null == this.method)
            {
                this.method = this.GetMethod(target, args);
            }

            MethodInfo mi = this.method; //this.GetMethod(target, args);
            if (null == mi)
            {
                throw InvalidExpressionException.UndefinedFunction(name);
            }

            try
            {
                return mi.Invoke(target, args);
            }
            catch (Exception ex)
            {
                throw InvalidExpressionException.ErrorInFunc(mi.Name, ex);
            }
        }

        public void AddArgument(ExpressionNode argument)
        {
            if (null == this.arguments)
            {
                this.arguments = new List<ExpressionNode>();
            }
            this.arguments.Add(argument);
        }

        public void Check()
        {
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.name);
            sb.Append("(");
            if (null != this.arguments)
            {
                for (int i = 0; i < this.arguments.Count; i++)
                {
                    if (i > 0)
                        sb.Append(", ");
                    sb.Append(this.arguments[i].ToString());
                }
            }
            sb.Append(")");
            return sb.ToString();
        }

        MethodInfo GetMethod(object target, object[] args)
        {
            try
            {
                foreach (MethodInfo info in FunctionNode.GetMethods(target))
                {
                    if (0 == string.Compare(info.Name, this.name, StringComparison.OrdinalIgnoreCase))
                    {
                        ParameterInfo[] parameters = info.GetParameters();
                        if (parameters.Length == args.Length)
                        {
                            bool found = true;
                            for (int i = 0; i < args.Length; i++)
                            {
                                Type paramType = parameters[i].ParameterType;
                                object arg = args[i];

                                if (null == arg)
                                {
                                    // null can be used only if reference types (!IsValueType)
                                    // or nullable type (int?)
                                    if (!paramType.IsValueType || null != Nullable.GetUnderlyingType(paramType))
                                        continue;
                                }
                                else
                                {
                                    if (paramType.IsAssignableFrom(arg.GetType()))
                                        continue;
                                }

                                found = false;
                                break;
                            }

                            if (found)
                            {
                                return info;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                Trace.WriteLine("" + e);
                return null;
            }
        }

        static IEnumerable<MethodInfo> GetMethods(object target)
        {
            if (target is IEnumerable<MethodInfo>)
            {
                return (IEnumerable<MethodInfo>)target;
            }
            else
            {
                return target.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            }
        }


        public override IEnumerable<ExpressionNode> GetChildNodes()
        {
            return this.Arguments;
        }
    }
}