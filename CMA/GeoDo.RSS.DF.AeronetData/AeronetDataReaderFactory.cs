using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.DF.AeronetData
{
    public static class AeronetDataReaderFactory
    {
        public static IVectorFeatureDataReader GetVectorFeatureDataReader(string fileDirName, params object[] args)
        {
            //可以为文件名也可以为文件夹名
            AeronetDataReader reader = new AeronetDataReader();
            try
            {
                if (reader.TryOpen(fileDirName, null, args))
                    return reader;
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
