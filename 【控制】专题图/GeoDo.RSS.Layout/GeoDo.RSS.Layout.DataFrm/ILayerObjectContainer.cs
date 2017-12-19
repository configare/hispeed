using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Layout.DataFrm
{
    public interface ILayerObjectBase
    {
        string Text { get; }
        bool Visible { get; set; }
    }

    public interface ILayerObject : ILayerObjectBase
    {
        object Tag { get; }
    }

    public interface ILayerObjecGroup : ILayerObjectBase
    {
        List<ILayerObjectBase> Children { get; }
    }

    public interface ILayerObjectContainer
    {
        List<ILayerObjectBase> LayerObjects { get; }
    }

    internal class LayerObjectBase : ILayerObjectBase
    {
        protected string _text;

        public LayerObjectBase(string text)
        {
            _text = text;
        }

        public string Text
        {
            get { return _text; }
        }

        public virtual bool Visible
        {
            get { return true; }
            set { ;}
        }
    }

    internal class LayerObject:LayerObjectBase, ILayerObject
    {
        protected object _tag;

        public LayerObject(string text, object tag)
            :base(text)
        {
            _tag = tag;
        }

        public object Tag
        {
            get { return _tag; }
        }

        public override bool Visible
        {
            get
            {
                if (_tag == null)
                    return false;
                if (_tag is GeoDo.RSS.Core.DrawEngine.IRenderLayer)
                    return (_tag as GeoDo.RSS.Core.DrawEngine.IRenderLayer).Visible;
                else if (_tag is CodeCell.AgileMap.Core.ILayerDrawable)
                    return (_tag as CodeCell.AgileMap.Core.ILayerDrawable).Visible;
                return false;
            }
            set
            {
                if (_tag == null)
                    return;
                if (_tag is GeoDo.RSS.Core.DrawEngine.IRenderLayer)
                    (_tag as GeoDo.RSS.Core.DrawEngine.IRenderLayer).Visible = value;
                else if (_tag is CodeCell.AgileMap.Core.ILayerDrawable)
                    (_tag as CodeCell.AgileMap.Core.ILayerDrawable).Visible = value;
            }
        }
    }

    internal class LayerObjectGroup :LayerObjectBase ,ILayerObjecGroup
    {
        private List<ILayerObjectBase> _children = new List<ILayerObjectBase>();

        public LayerObjectGroup(string text)
            :base(text)
        { 
        }

        public List<ILayerObjectBase> Children
        {
            get { return _children; }
        }
    }

}
