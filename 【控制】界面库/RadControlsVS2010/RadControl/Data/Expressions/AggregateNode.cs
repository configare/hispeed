namespace Telerik.Data.Expressions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;

    interface IDataAggregate
    {
        IEnumerable GetData();
    }

    /// <summary>
    /// 
    /// </summary>
    class AggregateNode : FunctionNode
    {
        static MethodInfo[] methods;

        MethodInfo methodInfo;
        //DataRelation relation;
        //DataTable childTable;

        public static bool IsAggregare(string name)
        {
            return (null != AggregateNode.LookupFunc(name));
        }

        public AggregateNode(string name) : base(null, name)
        {
            if (null == (this.methodInfo = AggregateNode.LookupFunc(name)))
            {
                throw InvalidExpressionException.UndefinedFunction(name);
            }
        }

        public override object Eval(object row, object context)
        {
            IDataAggregate agg = row as IDataAggregate;
            if (null != agg)
            {
                ArrayList args = new ArrayList();
                args.Add(new FunctionContext(this.Culture, context));
                args.Add(agg.GetData());
                args.AddRange(base.Arguments);
                return this.methodInfo.Invoke(null, args.ToArray());
            }

            return null;
        }

        static MethodInfo LookupFunc(string name)
        {
            try
            {
                if (null == AggregateNode.methods)
                {
                    AggregateNode.methods = typeof(Aggregates).GetMethods(BindingFlags.Public | BindingFlags.Static);
                }

                if (null != AggregateNode.methods)
                {
                    name = name.ToUpper();
                    for (int i = 0; i < AggregateNode.methods.Length; i++)
                    {
                        MethodInfo mi = AggregateNode.methods[i];
                        if (mi.Name.ToUpper() == name)
                        {
                            return mi;
                        }
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                Trace.WriteLine("AggregateNode.LookupFunc EXCEPTION:" + e);
                return null;
            }
        }
    }
}