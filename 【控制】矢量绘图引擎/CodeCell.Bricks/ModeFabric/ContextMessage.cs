using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.ModelFabric
{
    public class ContextMessage:IContextMessage
    {
        private List<string> _errorInfos = new List<string>();
        protected object _tag = null;

        #region IContextMessage 成员

        public void Reset()
        {
            _errorInfos.Clear();
        }

        public string GetErrorInfoString()
        {
            if (_errorInfos.Count == 0)
                return string.Empty;
            return string.Concat(_errorInfos.ToArray());
        }

        public void AddError(string errorInfo)
        {
            _errorInfos.Add(errorInfo);
        }

        public object Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }

        #endregion
    }
}
