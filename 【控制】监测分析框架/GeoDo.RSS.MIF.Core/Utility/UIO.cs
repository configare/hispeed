using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public class UIO
    {
        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadFile(string path)
        {
            TextReader reader = new StreamReader(path, UXmlConvert.DefaultEncoding);
            var content = reader.ReadToEnd();
            reader.Close();
            return content;
        }

        private static readonly object _lockobj = new object();
        public static void SaveFile(string info, string fileName)
        {
            StreamWriter myWriter = null;
            try
            {
                lock (_lockobj)
                {
                    myWriter = new StreamWriter(fileName, false, UXmlConvert.DefaultEncoding);
                    myWriter.Write(info);
                }
            }
            catch
            {
            }
            finally
            {
                if (myWriter != null)
                {
                    myWriter.Close();
                }
            }
        }
    }
}