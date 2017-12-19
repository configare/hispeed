using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CodeCell.Bricks.AppFramework
{
    public class ShortcutProcessorDefault : IShortcutProcessor
    {
        private List<IShortcutFilter> _filters = null;

        internal ShortcutProcessorDefault()
        { 
        }

        #region IShortcutProcessor ≥…‘±

        public void AddShortcutFilter(IShortcutFilter filter)
        {
            if (filter == null)
                return;
            if (_filters == null)
                _filters = new List<IShortcutFilter>(1);
            _filters.Add(filter);
        }

        public void AddShortcutFilters(IShortcutFilterCollection filters)
        {
            if (filters == null)
                return;
            if (_filters == null)
                _filters = new List<IShortcutFilter>(filters.Count);
            foreach (IShortcutFilter filter in filters.ShortcutFilters)
                _filters.Add(filter);
        }

        public void PreviewKeyDown(PreviewKeyDownEventArgs previewKeyDownEventArgs)
        {
            if (previewKeyDownEventArgs == null || _filters == null)
                return;
            foreach (IShortcutFilter filter in _filters)
                if (filter.AcceptShortcut(previewKeyDownEventArgs))
                    return;
        }

        #endregion
    }
}
