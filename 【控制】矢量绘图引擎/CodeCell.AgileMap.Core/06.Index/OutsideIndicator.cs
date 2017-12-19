using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    internal class OutsideIndicator:IOutsideIndicator
    {
        private bool _isoutside = false;

        public OutsideIndicator()
        { 
        }

        #region IOutsideIndicator Members

        public void SetOutside(bool isOutside)
        {
            _isoutside = isOutside;
        }

        public bool IsOutside
        {
            get { return _isoutside; }
        }
        #endregion
    }
}
