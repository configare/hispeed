using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.IO;

namespace GeoDo.RSS.Core.UI
{
    public class EmmbedUIResourceProvider:IUIResourceProvider,IDisposable
    {
        private string _namespace;
        private Assembly _assembly;

        public EmmbedUIResourceProvider(string assembly, string namespacestring)
        {
            _namespace = namespacestring;
            _assembly = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + assembly);
        }

        public Image GetImage(string resIdentify)
        {
            using(Stream stream = _assembly.GetManifestResourceStream(_namespace + "." + resIdentify))
            {
                if (stream == null)
                    return null;
                Bitmap bmp = (Bitmap)Image.FromStream(stream);
                return bmp == null ? null : new Bitmap(bmp, bmp.Size);
            }
        }

        public void Dispose()
        {
            if (_assembly != null)
            {
                _assembly = null;
            }
        }
    }
}
