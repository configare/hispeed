namespace Telerik.Data.Expressions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using Telerik.WinControls.Data;

    interface IDataDescriptor
    {
        PropertyDescriptorCollection GetProperties();
    }

    interface IDataMap
    {
        object this[string name] { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    class NameNode : ExpressionNode
    {
        const string FildsObject = "Fields";

        string name;
        ExpressionNode parent;
        Dictionary<System.Type, PropertyDescriptor> properties = new Dictionary<Type, PropertyDescriptor>();

        public ExpressionNode Parent
        {
            get { return this.parent; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public NameNode(ExpressionNode node, string name)
        {
            this.name = NameNode.ParseName(name);

            this.parent = node;
        }

        public override object Eval(object row, object context)
        {
            object o = context;
            if (null != this.parent)
            {
                o = this.parent.Eval(row, context);
            }
            else if (NameNode.FildsObject == this.name)
            {
                return row;
            }

            if (null == o)
            {
                return null;
            }

            IDataItem map = row as IDataItem; //changed from IDataMap
			if (null != map)
			{
				return map[this.name];
				//// The "old" field notation when there was no Fields global object
				//// and fields was referenced by thier name only; ex. =ProductCategory
				//// instead of =Fields.ProductCategory
				//try
				//{
				//		return map[this.name];
				//}
				//catch (Exception)
				//{
				//    // hide this exception; UndefinedObject will be raised after that
				//}
			}

            PropertyDescriptor property = null;
            Type objectType = o.GetType();

            // Sometimes one expression may be evaluated on a different
            // type of data. Because of the caching of the expression trees
            // two expression (ex. the report's filters and a textbox
            // in the group header) may result in a single exp tree. The different
            // input data (same ex. - the filter operates with DataObject while
            // for the group sections is passed DataGroup objects) will result
            // in a different PropertyDescriptor, so we need to chache them
            // per object type.

            if (!this.properties.TryGetValue(objectType, out property))
            {
                PropertyDescriptorCollection collection = null;
                if (o is IDataDescriptor)
                {
                    collection = ((IDataDescriptor)o).GetProperties();
                }
                else
                {
                    collection = TypeDescriptor.GetProperties(o);
                }

                if (null != collection && collection.Count > 0)
                {
                    this.properties[objectType]
                     = property
                     = collection.Find(this.name, true);
                }
            }

            if (null != property)
            {
                return property.GetValue(o);
            }

            throw InvalidExpressionException.UndefinedObject(this.name);
        }

        public override string ToString()
        {
            return ("Identifier(" + this.Name + ")");
        }

        static string ParseName(string name)
        {
            // Unescape escaped chars
            char[] text = name.ToCharArray();
            char ch = '\0';
            string esc = string.Empty;
            if ('`' == text[0])
            {
                ch = '\\';
                esc = "`";
            }
            else if ('[' == text[0])
            {
                ch = '\\';
                esc = @"]\";
            }

            if ('\0' != ch)
            {
                string s = string.Empty;
                int length = text.Length - 1;
                for (int i = 1; i < length; i++)
                {
                    if (ch == text[i]
                        && (i + 1) < length
                        && esc.IndexOf(text[i + 1]) >= 0)
                    {
                        continue;
                    }
                    s += text[i];
                }
                return s;
            }

            return name;
        }


        public override IEnumerable<ExpressionNode> GetChildNodes()
        {
            yield return this.parent;
        }
    }
}