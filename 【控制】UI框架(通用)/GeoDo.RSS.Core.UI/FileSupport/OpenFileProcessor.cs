using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using System.IO;
using System.Drawing;

namespace GeoDo.RSS.Core.UI
{
    public abstract class OpenFileProcessor : IOpenFileProcessor
    {
        protected List<string> _extNames = new List<string>();
        protected ISmartSession _session = null;

        public OpenFileProcessor()
            : base()
        { 
        }

        public OpenFileProcessor(ISmartSession session)
        {
            _session = session;
        }

        public void SetSession(ISmartSession session)
        {
            _session = session;
        }

        public virtual bool IsSupport(string fname, string extName)
        {
            if (extName != null && !_extNames.Contains(extName))
                return false;
            return FileHeaderIsOK(fname, extName);
        }

        protected abstract bool FileHeaderIsOK(string fname, string extName);

        public abstract bool Open(string fname,out bool memoryIsNotEnough);

        public void RefreshLayerManager()
        {
            ISmartWindow layerManager = _session.SmartWindowManager.GetSmartWindow((wnd) => { return wnd is ILayerManager; });
            if (layerManager != null)
                (layerManager as ILayerManager).Update();
        }
    }
}
