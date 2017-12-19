using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace CodeCell.AgileMap.Core
{
    public abstract class ContainerSymbol:IContainerSymbol,IDisposable
    {
        protected SizeF _sizef = SizeF.Empty;
        protected bool _isFixedSize = true;

        public ContainerSymbol()
        { 
        }

        #region IContainerSymbol Members

        [DisplayName("符号大小")]
        public SizeF Size
        {
            get { return _sizef; }
            set { _sizef = value; }
        }

        [DisplayName("固定大小")]
        public bool IsFixedSize
        {
            get { return _isFixedSize; }
            set { _isFixedSize = value; }
        }

        public abstract SizeF Draw(Graphics g, PointF location, SizeF size);

        #endregion

        #region IPersistable Members

        public abstract PersistObject ToPersistObject();

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
