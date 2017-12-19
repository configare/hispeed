using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public class MoasicFileProvider : IDisposable
    {
        private Action<int, string> _progressCallback;
        private List<MosaicItem> _fileItems = new List<MosaicItem>();

        public MoasicFileProvider(Action<int, string> progressCallback)
        {
            _progressCallback = progressCallback;
        }

        public CoordEnvelope Envelope
        {
            get
            {
                if (_fileItems == null || _fileItems.Count == 0)
                    return null;
                CoordEnvelope env = null;
                for (int i = 0; i < _fileItems.Count; i++)
                {
                    if (env == null)
                        env = _fileItems[i].Envelope;
                    else
                        env = env.Union(_fileItems[i].Envelope);
                }
                return env;
            }
        }

        public MosaicItem[] FileItems
        {
            get { return _fileItems.ToArray(); }
        }

        public IRasterDataProvider Add(string file)
        {
            IRasterDataProvider prd = null;
            try
            {
                prd = GeoDataDriver.Open(file) as IRasterDataProvider;
                if(Add(prd))
                   return prd;
                if (prd != null)
                    prd.Dispose();
                return null;
            }
            catch
            {
                if (prd != null)
                    prd.Dispose();
                throw;
            }
        }

        public bool Add(IRasterDataProvider file)
        {
            if (!Contains(file))
            {
               _fileItems.Add(new MosaicItem(file));
               return true;
            }
            return false;
        }

        private bool Contains(IRasterDataProvider file)
        {
            if (_fileItems == null || _fileItems.Count == 0)
                return false;
            foreach (MosaicItem item in _fileItems)
            {
                IRasterDataProvider prd = item.MainFile;
                if (prd.fileName == file.fileName)
                    return true;
            }
            return false;
        }

        public bool Remove(IRasterDataProvider file)
        {
            if (_fileItems != null)
            {
                int i = _fileItems.RemoveAll(new Predicate<MosaicItem>(item => { return (item.MainFile == file); }));
                if (_fileItems.Count == 0)
                {
                    _fileItems.Clear();
                }
                return i != 0;
            }
            return false;
        }

        public void Clear()
        {
            for (int i = 0; i < _fileItems.Count; i++)
            {
                _fileItems[i].Dispose();
                _fileItems[i] = null;
            }
            _fileItems.Clear();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
