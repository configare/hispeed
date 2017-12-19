using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CodeCell.Bricks.ModelFabric
{
    public class RenderArg:IDisposable
    {
        private Graphics _graphics = null;
        private IModelEditor _modelEditor = null;
        private Matrix _identityMatrix = new Matrix();
        private Matrix _originMatrix = null;

        public RenderArg(IModelEditor modelEditor)
        {
            _modelEditor = modelEditor;
        }

        internal void Update(Graphics graphics)
        {
            _graphics = graphics;
        }

        public Graphics Graphics
        {
            get { return _graphics; }
        }

        public void UsePixelCoordRender()
        {
            _originMatrix = _graphics.Transform;
            _graphics.Transform = _identityMatrix;
        }

        public void EndUsePixelCoordRender()
        {
            _graphics.Transform = _originMatrix;
            _originMatrix = null;
        }

        #region IDisposable 成员

        public void Dispose()
        {
            _identityMatrix.Dispose();
            _identityMatrix = null;
        }

        #endregion
    }
}
