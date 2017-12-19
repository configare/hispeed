using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace GeoDo.RSS.VideoMark
{
    public class ViedoMarkerToGIF:IViedoMarkProcesser
    {
        #region IViedoMarkProcesser 成员

        public bool Support(string aviFilename)
        {
            if (string.IsNullOrEmpty(aviFilename) || string.IsNullOrEmpty(Path.GetFileName(aviFilename)) || string.IsNullOrEmpty(Path.GetExtension(aviFilename)))
                return false;
            if (Path.GetExtension(aviFilename).Trim().ToUpper() == ".GIF")
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

            GIFCreater gifCreate = null;
            try
            {
                if (size == null || size == Size.Empty)
                    gifCreate = new GIFCreater(res,progress);
                else
                    gifCreate = new GIFCreater(size.Width, size.Height, res,progress);
                if (gifCreate != null)
                    gifCreate.Interval = interval;

                gifCreate.Create(avifilename);
            }
            catch(Exception ex) 
            {
            // Log.WriterException(ex);
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
