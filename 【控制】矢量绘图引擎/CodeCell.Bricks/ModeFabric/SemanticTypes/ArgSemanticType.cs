using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.ModelFabric
{
    public class ArgSemanticType:ISemanticTypeEditor
    {
        protected Type _dataType = null;

        public ArgSemanticType()
        { 
        }

        public ArgSemanticType(Type dataType)
        {
            _dataType = dataType;
        }

        #region ISemanticTypeEditor 成员

        public Type DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        public virtual object GetValue(object sender)
        {
            return null;
        }

        public virtual bool TryParse(string text,out object value)
        {
            value = null;
            return false;
        }

        public virtual string ToString(object value)
        {
            if (value == null)
                return string.Empty;
            return value.ToString();
        }

        public virtual bool IsNeedInput
        {
            get
            {
                return true;
            }
        }

        #endregion
    }
}
