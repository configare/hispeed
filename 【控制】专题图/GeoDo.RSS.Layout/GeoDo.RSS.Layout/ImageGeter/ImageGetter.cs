using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.IO;

namespace GeoDo.RSS.Layout
{
    public class ImageGetter
    {
        public static Image GetImageByName(string fname)
        {
            string imageDll = AppDomain.CurrentDomain.BaseDirectory + "GeoDo.RSS.Layout.Elements.dll";
            if (!File.Exists(imageDll))
                return null;
            Assembly assembly = Assembly.LoadFile(imageDll);
            Image img = null;
            string path = "GeoDo.RSS.Layout.Elements.Resources." + fname;
            using (Stream sm = assembly.GetManifestResourceStream(path))
            {
                if (sm != null)
                    img = Image.FromStream(sm);
            }
            return img;
        }
    }
}
