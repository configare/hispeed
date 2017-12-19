using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace GeoDo.RSS.Core.UI
{
    public class DirUIResourceProvider:IUIResourceProvider,IDisposable
    {
        private string _dir;

        public DirUIResourceProvider(string dir)
        {
            _dir = dir;
        }

        public Image GetImage(string resIdentify)
        {
            string path = Path.Combine(_dir, resIdentify);
            if (!File.Exists(path))
                return null;
            Bitmap bmp = (Bitmap)Image.FromFile(path);
            return bmp == null ? null : new Bitmap(bmp, bmp.Size);
        }

        public void Dispose()
        {
            
        }
    }
}
