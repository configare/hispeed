namespace Telerik.Data.Expressions
{
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Collections;

    /// <summary>
    /// 
    /// </summary>
    abstract class ExpressionNode
    {
        protected CultureInfo Culture
        {
            get
            {
                return CultureInfo.InvariantCulture;
            }
        }

        public virtual bool IsConst
        {
            get { return false; }
        }

        public abstract object Eval(object row, object context);

        internal static bool IsFloat(StorageType type)
        {
            if (type != StorageType.Single && type != StorageType.Double)
            {
                return (type == StorageType.Decimal);
            }
            return true;
        }

        internal static bool IsFloatSql(StorageType type)
        {
            if (type != StorageType.Single
                && type != StorageType.Double
                && type != StorageType.Decimal
                && type != StorageType.SqlDouble
                && type != StorageType.SqlDecimal
                && type != StorageType.SqlMoney)
            {
                return (type == StorageType.SqlSingle);
            }
            return true;
        }

        internal static bool IsInteger(StorageType type)
        {
            if (type != StorageType.Int16 
                && type != StorageType.Int32 
                && type != StorageType.Int64 
                && type != StorageType.UInt16
                && type != StorageType.UInt32
                && type != StorageType.UInt64
                && type != StorageType.SByte)
            {
                return (type == StorageType.Byte);
            }
            return true;
        }

        internal static bool IsIntegerSql(StorageType type)
        {
            if (type != StorageType.Int16
                && type != StorageType.Int32
                && type != StorageType.Int64
                && type != StorageType.UInt16
                && type != StorageType.UInt32
                && type != StorageType.UInt64
                && type != StorageType.SByte
                && type != StorageType.Byte
                && type != StorageType.SqlInt64
                && type != StorageType.SqlInt32
                && type != StorageType.SqlInt16)
            {
                return (type == StorageType.SqlByte);
            }
            return true;
        }

        internal static bool IsNumeric(StorageType type)
        {
            if (!IsFloat(type))
            {
                return IsInteger(type);
            }
            return true;
        }

        internal static bool IsNumericSql(StorageType type)
        {
            if (!IsFloatSql(type))
            {
                return IsIntegerSql(type);
            }
            return true;
        }

        internal static bool IsSigned(StorageType type)
        {
            if (type != StorageType.Int16
                && type != StorageType.Int32 
                && type != StorageType.Int64 
                && type != StorageType.SByte)
            {
                return IsFloat(type);
            }
            return true;
        }

        internal static bool IsSignedSql(StorageType type)
        {
            if (type != StorageType.Int16 
                && type != StorageType.Int32 
                && type != StorageType.Int64 
                && type != StorageType.SByte 
                && type != StorageType.SqlInt64 
                && type != StorageType.SqlInt32 
                && type != StorageType.SqlInt16)
            {
                return IsFloatSql(type);
            }
            return true;
        }

        internal static bool IsUnsigned(StorageType type)
        {
            if (type != StorageType.UInt16 
                && type != StorageType.UInt32 
                && type != StorageType.UInt64)
            {
                return (type == StorageType.Byte);
            }
            return true;
        }

        internal static bool IsUnsignedSql(StorageType type)
        {
            if (type != StorageType.UInt16 
                && type != StorageType.UInt32 
                && type != StorageType.UInt64 
                && type != StorageType.SqlByte)
            {
                return (type == StorageType.Byte);
            }
            return true;
        }

        public static List<T> GetNodes<T>(ExpressionNode node)
            where T : ExpressionNode
        {
            List<T> nodes = new List<T>();
            if (node == null)
            {
                return nodes;
            }
            
            Stack<ExpressionNode> nodeStack = new Stack<ExpressionNode>();
            nodeStack.Push(node);

            while (nodeStack.Count > 0)
            {
                ExpressionNode currentNode = nodeStack.Pop();
                if (currentNode is T)
                {
                    nodes.Add((T)currentNode);
                }

                IEnumerable<ExpressionNode> childNodesCollection = currentNode.GetChildNodes();
                if (childNodesCollection != null)
                {
                    foreach (ExpressionNode childNode in childNodesCollection)
                    {
                        if (childNode != null)
                        {
                            nodeStack.Push(childNode);
                        }
                    }
                }
            }

            return nodes;
        }

       

        public virtual IEnumerable<ExpressionNode> GetChildNodes()
        {
            return new ExpressionNode[0];
        }
    }
}