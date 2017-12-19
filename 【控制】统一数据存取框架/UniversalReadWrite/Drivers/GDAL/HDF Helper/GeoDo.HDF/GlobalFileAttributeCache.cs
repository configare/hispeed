using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.HDF
{
    /// <summary>
    /// 全局数据属性缓存，程序执行全程不释放，自己控制缓存容量
    /// </summary>
    public class GlobalFileAttributeCache : IDisposable
    {
        private Dictionary<string, Dictionary<string, string>> _cacheData = new Dictionary<string, Dictionary<string, string>>();
        private static GlobalFileAttributeCache _instance = new GlobalFileAttributeCache();

        private GlobalFileAttributeCache()
        {
        }

        /// <summary>
        /// 将正在使用的文件移到最后
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetInstanceFileAttributes(string filename)
        {
            if (_cacheData.ContainsKey(filename))
            {
                Dictionary<string, string> dic = _cacheData[filename];
                _cacheData.Remove(filename);
                _cacheData.Add(filename, dic);
                return dic;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 将文件属性放入缓存
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="attributes"></param>
        private void SetInstanceFileAttributes(string filename, Dictionary<string, string> attributes)
        {
            if (_cacheData.ContainsKey(filename))
            {
                Dictionary<string, string> dic = _cacheData[filename];//将该文件移到最前
                _cacheData.Remove(filename);
                _cacheData.Add(filename, dic);
                return;
            }
            if (_cacheData.Count >= 20)//缓存容量不超过20个
            {
                string key = _cacheData.First().Key;
                _cacheData.Remove(key);
            }
            _cacheData.Add(filename, attributes);
        }

        /// <summary>
        /// 获取缓存的属性，如果尝试在外面释放缓存的属性将会造成未知的错误。
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetFileAttributes(string filename)
        {
            return _instance.GetInstanceFileAttributes(filename);
        }

        public static void SetFileAttributes(string filename, Dictionary<string, string> attributes)
        {
            _instance.SetInstanceFileAttributes(filename, attributes);
        }

        public void Dispose()
        {
            _cacheData.Clear();
        }
    }
}
