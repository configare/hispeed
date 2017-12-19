using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace GeoDo.RSS.VideoMark
{
    public class ViedoMarkerToAVI : IViedoMarkProcesser
    {
        #region IViedoMarkProcesser 成员

        public bool Support(string aviFilename)
        {
            if (string.IsNullOrEmpty(aviFilename) || string.IsNullOrEmpty(Path.GetFileName(aviFilename)) || string.IsNullOrEmpty(Path.GetExtension(aviFilename)))
                return false;
            if (Path.GetExtension(aviFilename).Trim().ToUpper() == ".AVI")
                return true;
            return false;
        }

        public bool Mark(string avifilename, Size size, int interval, Image[] res, Action<int, string> progress)
        {
            if (interval == 0 || res == null || res.Length == 0)
                return false;
            string path = Path.GetDirectoryName(avifilename);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            AVICreater aviCreate = null;
            try
            {
                aviCreate = new AVICreater(avifilename, interval, res,progress);
                aviCreate.AVICreate();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                res = null;
            }
            return true;
        }

        #endregion
    }
}
