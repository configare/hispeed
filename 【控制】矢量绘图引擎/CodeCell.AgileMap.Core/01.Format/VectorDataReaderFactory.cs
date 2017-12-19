using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Core
{
    public static class VectorDataReaderFactory
    {
        private static Type[] DataReaderRegister = new Type[] 
                                                           {         
                                                               typeof(EsriShapeFilesFeatureReader)
                                                               ,typeof(HdfGlobalFirePointReader)
                                                           };

        static VectorDataReaderFactory()
        {
        }

        private static IUniversalVectorDataReader GetDataReader(Type type)
        {
            if (DataReaderRegister == null || DataReaderRegister.Length == 0)
                return null;
            return Activator.CreateInstance(type) as IUniversalVectorDataReader;
        }

        public static IUniversalVectorDataReader GetUniversalDataReader(string filename, params object[] args)
        {
            if (DataReaderRegister == null || DataReaderRegister.Length == 0)
            {
                Log.WriterError("AgileMap", "GetUniversalDataReader(...)", "没有已经注册的数据读取器！");
                return null;
            }
            if (!File.Exists(filename))
            {
                Log.WriterWarning("AgileMap", "GetUniversalDataReader(...)", "输入的参数文件名为空！");
                return null;
            }
            byte[] bs = null;
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader sr = new BinaryReader(fs))
                    {
                        long len = 1024;
                        if (fs.Length < 1024)
                            len = fs.Length;
                        bs = sr.ReadBytes((int)len);
                    }
                }
            }
            catch(Exception ex)
            {
                Log.WriterWarning("AgileMap.DataReaderFactory", "GetUniversalDataReader(...)", ex.Message);
                return null; 
            }
            //
            foreach (Type readerType in DataReaderRegister)
            {
                try
                {
                    IUniversalVectorDataReader rd = GetDataReader(readerType);
                    if (rd == null)
                        continue;
                    if (rd.TryOpen(filename, bs))
                        return rd;
                }
                catch(Exception Ex) 
                {
                    Log.WriterException("AgileMap.DataReaderFactory", "GetUniversalDataReader(...)", Ex);
                    continue;
                }
            }
            return null;
        }
    }
}
