using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class CacheNameHelper
    {
        private string _cacheDir = null;

        public CacheNameHelper(string cacheDir)
        {
            _cacheDir = cacheDir;
        }

        public string GetCacheDirByFilename(string fname)
        {
            return Path.Combine(_cacheDir, ToCacheDirName(fname));
        }

        public string GetCacheFilename(string dir, int band, int levelNo)
        {
            return Path.Combine(dir, ToCacheFilename(band, levelNo));
        }

        private string ToCacheFilename(int band, int levelNo)
        {
            return levelNo.ToString().PadLeft(2, '0') + band.ToString().PadLeft(4, '0') + ".cache";
        }

        private string ToCacheDirName(string filename)
        {
            string fname = Path.GetFileNameWithoutExtension(filename).ToUpper() + "_CACHE";
            return fname;
        }

        public string GetResourceKey(int bandNo, int levelNo)
        {
            return levelNo.ToString().PadLeft(2, '0') + bandNo.ToString().PadLeft(4, '0');
        }

        public bool ParseCacheFilename(string fname, out int bandNo, out int levelNo)
        {
            bandNo = levelNo = -1;
            string levelNoStr = fname.Substring(0, 2);
            string bandNoStr = fname.Substring(2, 4);
            if (int.TryParse(bandNoStr, out bandNo))
                if (int.TryParse(levelNoStr, out levelNo))
                    return true;
            return false;
        }
    }
}
