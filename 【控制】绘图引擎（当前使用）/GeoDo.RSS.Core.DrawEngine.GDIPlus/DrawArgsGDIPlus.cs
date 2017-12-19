using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine.GDIPlus
{
    public class DrawArgsGDIPlus:IDrawArgs
    {
        private QuickTransform _quickTransformArgs;
        private Graphics _graphics = null;

        public DrawArgsGDIPlus(QuickTransform quickTransformArgs)
        {
            _quickTransformArgs = quickTransformArgs;
        }

        public DrawArgsGDIPlus(Graphics graphics, QuickTransform quickTransformArgs)
        {
            _graphics = graphics;
            _quickTransformArgs = quickTransformArgs;
        }

        public object Graphics
        {
            get { return _graphics; }
        }

        #region IDrawArgs 成员

        public QuickTransform QuickTransformArgs
        {
            get { return _quickTransformArgs; }
        }

        public void Reset(object graphics)
        {
            Dispose();
            _graphics = graphics as Graphics;
        }

        #endregion

        public void Dispose()
        {
            if (_graphics != null)
            {
                _graphics = null;
            }
        }
    }
}
