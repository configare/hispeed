using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Layout
{
    public class DrawArgs:IDrawArgs
    {
        public static bool IsExporting = false;
        public static bool IsStrongRefreshData = false;
        protected object _graphics;
        protected ILayoutRuntime _runtime;

        public DrawArgs(ILayoutRuntime runtime)
        {
            _runtime = runtime;
        }

        public void Reset(object graphics)
        {
            _graphics = graphics;
        }

        public ILayoutRuntime Runtime
        {
            get { return _runtime; }
        }

        public object Graphics
        {
            get { return _graphics; }
        }
    }
}
