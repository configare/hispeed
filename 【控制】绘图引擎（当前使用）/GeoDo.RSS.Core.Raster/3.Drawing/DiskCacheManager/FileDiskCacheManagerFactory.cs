using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public static class FileDiskCacheManagerFactory
    {
        private static FileIndexer _fileIndexer;
        private static TileSetting _tileSetting;
        private static CacheSetting _cacheSetting;

        static FileDiskCacheManagerFactory()
        {
            CanvasSetting setting = null;
            string fname = Path.Combine(Path.GetDirectoryName((new TileSetting()).GetType().Assembly.Location), "GeoDo.RSS.Core.DrawEngine.GDIPlus.cnfg.xml");
            if (File.Exists(fname))
            {
                using (CanvasSettingParser p = new CanvasSettingParser())
                {
                    setting = p.Parse(fname);
                    if (!Directory.Exists(setting.CacheSetting.Dir))
                        Directory.CreateDirectory(setting.CacheSetting.Dir);
                    _tileSetting = setting.TileSetting;
                    _cacheSetting = setting.CacheSetting;
                }
            }
            _fileIndexer = new FileIndexer(setting.CacheSetting.Dir); ;
        }

        public static IFileDiskCacheManager Active(string fName)
        {
            string fileDir = _fileIndexer.GetFileDir(fName);
            if (!Directory.Exists(fileDir))
                Directory.CreateDirectory(fileDir);
            //
            DriveInfo drvInfo = new DriveInfo(fileDir.Substring(0, 1));
            //剩余磁盘空间不足，不启用缓存
            if (drvInfo.AvailableFreeSpace < (long)((long)_cacheSetting.MinSizeMB * (long)1024 * (long)1024))
                return null;
            //
            try
            {
                return new FileDiskCacheManager(_tileSetting, fName, fileDir);
            }
            finally 
            {
                _fileIndexer.Save();
            }
        }

        public static IFileDiskCacheManager Active(IRasterDataProvider dataProvider)
        {
            return Active(dataProvider.fileName);
        }

        public static void TryFreeDiskspace(string activeFileCacheDir)
        {
            string dir = _fileIndexer.GetOldestCacheDir();
            if (dir == null)
                return;
            Directory.Delete(dir, true);
        }
    }
}
