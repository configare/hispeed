using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Layout.GDIPlus
{
    public class SelectedEditBoxManager : ISelectedEditBoxManager, IDisposable
    {
        private Dictionary<ISizableElement, ISelectedEditBox> _boxes = new Dictionary<ISizableElement, ISelectedEditBox>();

        public SelectedEditBoxManager()
        {
        }

        public void Update(ILayoutRuntime layoutRuntime)
        {
            _boxes.Clear();
            IElement[] selection = layoutRuntime.Selection;
            if (selection == null)
                return;
            foreach (IElement ele in selection)
                if (ele.IsSelected && ele is ISizableElement)
                    _boxes.Add(ele as ISizableElement, new SelectedEditBox(ele as ISizableElement));
        }

        public ISelectedEditBox Get(ISizableElement element)
        {
            if (_boxes.ContainsKey(element))
                return _boxes[element];
            return null;
        }

        public void Attach(ISizableElement element)
        {
            if (_boxes.ContainsKey(element))
                return;
            _boxes.Add(element, new SelectedEditBox(element));
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            foreach (ISelectedEditBox box in _boxes.Values)
                box.Render(sender, drawArgs);
        }

        public void Dispose()
        {
            if (_boxes != null)
            {
                _boxes.Clear();
                _boxes = null;
            }
        }
    }
}
