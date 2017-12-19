using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.RedoUndo;
using CodeCell.AgileMap.Core;


namespace CodeCell.AgileMap.Components
{
    internal abstract class OprBudGIS:IOperation
    {
        protected string _name = null;
        protected IMapControl _mapControl = null;

        public OprBudGIS(IMapControl mapControl)
        {
            _mapControl = mapControl;
        }

        #region IOperation Members

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public abstract void Do();
        public virtual void Redo()
        {
            Do();
        }

        public abstract void Undo();

        public object Data
        {
            get
            {
                return null;
            }
            set
            {
                ;
            }
        }

        #endregion
    }
}
