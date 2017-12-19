using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    [Serializable]
    public class ColorItemPersist
    {
        private string _name = null ;
        private ColorItem[] _colorItems = null;

        public ColorItemPersist()
        { 
        }

        public string Name
        {
            get{return _name;}
            set{_name = value ;}
        }

        public ColorItem[] ColorItems
        {
            get { return _colorItems; }
            set { _colorItems = value; }
        }
    }

    public static  class LinearGradientTableFactory
    {
        public static ColorItemPersist GetByName(string name)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "LinearColorTable";
            string fname = dir + "\\" + name + ".lgt";
            if (!File.Exists(fname))
                return null;
            return ObjectToDisk.DerializeClassFromBinary(fname) as ColorItemPersist;
        }

        public static ColorItemPersist[] GetAll()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "LinearColorTable";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string[] fnames = System.IO.Directory.GetFiles(dir);
            if (fnames == null || fnames.Length == 0)
                return null;
            List<ColorItemPersist> ps = new List<ColorItemPersist>();
            foreach (string f in fnames)
            {
                ColorItemPersist p = GetByName(Path.GetFileNameWithoutExtension(f));
                if (p == null)
                    continue;
                ps.Add(p);
            }
            return ps.ToArray();
        }

        public static void SaveToFile(ColorItem[] colorItems, string name)
        {
            ColorItemPersist colorPersist = new ColorItemPersist();
            colorPersist.ColorItems = colorItems;
            colorPersist.Name = name;
            string dir = AppDomain.CurrentDomain.BaseDirectory + "LinearColorTable";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string fname = dir + "\\" + name + ".lgt";
            ObjectToDisk.SerializeClassToBinary(colorPersist, fname);
        }

        internal static void Delete(string name)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "LinearColorTable";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string fname = dir + "\\" + name + ".lgt";
            if (File.Exists(fname))
                File.Delete(fname);
        }
    }

    [Serializable]
    public class ColorItem
    {
        public float Position = 0f;
        public Color Color = Color.Black;
        [NonSerialized]
        public GraphicsPath Bounds = new GraphicsPath();
        public int X = 0;

        public ColorItem(float position, Color color)
        {
            Position = position;
            Color = color;
        }
    }
}
